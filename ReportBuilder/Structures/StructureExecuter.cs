using Cabronate.Base;
using Cabronate.DAO.ReportBuilder.DTOs;
using Cabronate.DAO.ReportBuilder.Processed;
using Cabronate.DAO.ReportBuilder.StructureJsons;
using Cabronate.DAO.ReportBuilder.StructureJsons.Extensions;
using Cabronate.DAO.ReportBuilder.Structures.Factories;
using Cabronate.DAO.ReportBuilder.Types;
using Cabronate.DAO.SQL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Cabronate.DAO.ReportBuilder.Structures
{
    public class StructureExecuter
    {
        private readonly StructureContainer _structureContainer = null;
        private readonly string _script = string.Empty;
        private readonly DBContexto _dbctx;
        private bool _updateFilters;
        private DataTable _dataTable;

        public StructureExecuter(DBContexto dbctx, StructureContainer structureContainer)
        {
            _structureContainer = structureContainer;
            _dbctx = dbctx;
            _updateFilters = false;
        }

        public StructureExecuter(DBContexto dbctx, string script)
        {
            _script = script;            
            _dbctx = dbctx;
            _updateFilters = false;
        }

        public string Command { get; set; }

        public DataTable Execute()
        {
            return _dataTable = _dbctx.GetDataTable(Command);
        }	

        public void SetUpCommand(List<FilterDTO> filters = null, List<ColumnDTO> columns = null)
        {
            if (_structureContainer != null)
            {
                StringBuilder command = new StringBuilder();

                command.AppendLine(StructureBaseCommandFactory.GetBaseCommand(_structureContainer.ReportType)
                                                              .BaseCommand(_structureContainer, _dbctx.tipoBanco, columns));
                command.AppendLine(SetUpJoins());
                command.AppendLine(SetUpConditions(filters));
                command.AppendLine(SetUpGroupBy());
                command.AppendLine(SetUpOrderBy());

                Command = command.ToString();
            }
            else Command = _script;
        }

        public bool UpdateFilters => _updateFilters;

        private string SetUpJoins()
        {
            StringBuilder joinsText = new StringBuilder();            
            string noLock = (_dbctx.tipoBanco == TipoBanco.SQLServer ? "(NoLock)" : string.Empty);

            _structureContainer.Joins.ForEach(join =>
            {
                if (join.FirstTable)
                {
                    joinsText.AppendLine($" FROM {join.TableOrign.TableName} {join.TableOrign.TableName} {noLock}");
                }
                else
                {
                    if (join.Relationship == null)
                        join.Relationship = join.TableOrign.GetRelation(join.TableTarget);
                    joinsText.AppendLine($"{SetUpJoinText(join)} {join.TableOrign.TableName} {join.TableOrign.TableName} {noLock} ON ({SetUpJoinConnections(join)})");
                }
            });
            return joinsText.ToString();
        }

        private string SetUpJoinText(StructureConnectionJoin structure) => structure.JoinType == Join.LeftJoin ? "LEFT JOIN" : "JOIN";

        private string SetUpJoinConnections(StructureConnectionJoin structureConnection)
        {
            StringBuilder connectionsText = new StringBuilder();

            structureConnection.Relationship.Links.ForEach(condition =>
            {
                connectionsText.AppendLine($" {structureConnection.TableOrign.TableName}.{condition.SelfConnectionField} = {structureConnection.TableTarget.TableName}.{condition.TableTargetConnectoinField} AND");
            });

            return EcalcString.RemoverUltimosCaracteres(connectionsText.ToString(), "AND").Trim();
        }

        private string SetUpConditions(List<FilterDTO> filters = null)
        {
            StringBuilder conditionsStrut = new StringBuilder();

            conditionsStrut.AppendLine(" WHERE 1 = 1 ");

            SetFilters(filters);

            foreach (StructureFilter item in _structureContainer.Filters
                                                                .Where(f => f.Value != null))
            {
                item.Dbctx = _dbctx;
                GetKeyValue(item);

                conditionsStrut.AppendLine(ConditioningFactory.GetCondition(item.Condition));
            }

            return conditionsStrut.ToString();
        }

        private void SetFilters(List<FilterDTO> filters)
        {
            if (filters != null)
            {
                _structureContainer.Filters.ForEach(f =>
                {
                    var filter = filters.Where(dynamic =>
                                               dynamic.FieldName == f.Field.FieldName &&
                                               dynamic.TableName == f.Field.TableName &&
                                               !f.Fixed &&
                                               dynamic.Value != null &&
                                               dynamic.Value.ToString() != f.Value.ToString());

                    if (filter.Any())
                    {
                        f.Value = filter.First()
                                        .Value;
                        _updateFilters = true;
                    }
                });
            }
        }

        private void GetKeyValue(StructureFilter item)
        {
            var fieldStructure = _structureContainer.ProcessedJson
                                                    .GetTables()
                                                    .GetField(item.Field.TableName, item.Field.FieldName);

            if (fieldStructure.Values.Count > 0)
            {
                JsonValuesStructure value = fieldStructure.Values
                                                          .Where(v => v.Value == (string)item.Value)
                                                          ?.First();

                if (value != null)
                {
                    item.Value = value.Key;
                }
            }
        }

        private string SetUpOrderBy()
        {
            if (_structureContainer.OrderBys.Count == 0) return "";
            return $" ORDER BY {string.Join(",", _structureContainer.OrderBys)}";
        }

        private string SetUpGroupBy()
        {
            if ((_structureContainer.NeedGroupBy) && (_structureContainer.GroupBys.Count > 0))
                return $" GROUP BY {string.Join(",", _structureContainer.GroupBys)}";
            else
                return "";
        }    
    }
}
