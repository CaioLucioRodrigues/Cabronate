using Cabronate.Base;
using Cabronate.Base.Interfaces;
using Cabronate.DAO.ReportBuilder.StructureJsons;
using Cabronate.DAO.ReportBuilder.Structures;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Cabronate.DAO.ReportBuilder.Processed
{
    public class ProcessedJson : ISelfManagingCacheObject
    {
        private readonly StructureReader _reader;
        private List<JsonTableStructure> _jsonTableStructure;

        public ProcessedJson(string file)
        {
            _reader = new StructureReader(file);
        }

        public ListaErros Process()
        {
            ListaErros errors = new ListaErros();
            try
            {
                (_jsonTableStructure, errors) = _reader.Load();
                if (!errors.HasAnyError)
                {
                    NormalizeJsonWithFullConnections();
                    FillAuxiliaryData();
                }
            }
            catch (Exception e)
            {
                errors.AddError(e.Message, ErrorType.Error);
            }
            return errors;
        }

        public List<JsonTableStructure> GetTables() => _jsonTableStructure;

        private void FillAuxiliaryData()
        {
            FillTableNameInFields();
        }

        private void FillTableNameInFields()
        {
            _jsonTableStructure.ForEach(table =>
            {
                table.Fields.ForEach(field =>
                {
                    field.TableName = table.TableName;
                    field.TableFriendlyName = table.ToString();
                });
            });
        }

        private void NormalizeJsonWithFullConnections()
        {
            _jsonTableStructure.ForEach(tableBase =>
            {
                _jsonTableStructure.Where(t => t.TableName != tableBase.TableName)
                                   .ToList()
                                   .ForEach(currentTable =>
                {
                    ValidadeRelationships(tableBase, currentTable);
                });
            });
        }

        private void ValidadeRelationships(JsonTableStructure tableBase, JsonTableStructure currentTable)
        {
            currentTable.Relationships.Where(r => r.TableTarget == tableBase.TableName)
                                      .ToList()
                                      .ForEach(currentRelation =>
            {
                JsonRelationshipStructure inverseCurrentRelation = CreateInverseRelation(currentRelation, currentTable.TableName);

                bool hasRelation = false;
                tableBase.Relationships.ForEach(tableBaseRelation =>
                {
                    hasRelation |= (inverseCurrentRelation.Equals(tableBaseRelation));
                });
                if (!hasRelation)
                    tableBase.Relationships.Add(inverseCurrentRelation);
            });
        }

        private JsonRelationshipStructure CreateInverseRelation(JsonRelationshipStructure currentRelation, string originTableName)
        {
            JsonRelationshipStructure inverseRelation = new JsonRelationshipStructure(originTableName);
            currentRelation.Links.ForEach(currentLinks =>
            {
                inverseRelation.Links.Add(new JsonLinksStructure()
                {
                    SelfConnectionField = currentLinks.TableTargetConnectoinField,
                    TableTargetConnectoinField = currentLinks.SelfConnectionField
                });
            });
            return inverseRelation;
        }

        public double GetTimeOutInMinutes()
        {            
            return 60;
        }
    }
}
