using Cabronate.Base;
using Cabronate.DAO.ReportBuilder.DTOs;
using Cabronate.DAO.ReportBuilder.Interfaces;
using Cabronate.DAO.ReportBuilder.StructureJsons;
using Cabronate.DAO.ReportBuilder.StructureJsons.Extensions;
using Cabronate.DAO.ReportBuilder.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Cabronate.DAO.ReportBuilder.Structures.BaseCommands
{
    public class StructureBaseCommandQuery : StructureBaseCommand, IBaseCommand
    {
        public string BaseCommand(StructureContainer structureContainer, TipoBanco bd, List<ColumnDTO> columns = null)
        {
            var fields = new List<string>();
            OrderColumns(structureContainer, columns);

            structureContainer.Fields.ForEach(field =>
            {
                if (field.Operation == Operation.Undefined)
                {
                    string nick = MountNick(field);
                    string _default = GetDefaultValue(columns, field);
                    string caseWhen = MountCaseWhen(structureContainer, field, _default);

                    if (!string.IsNullOrWhiteSpace(caseWhen))
                        fields.Add($" {caseWhen} {nick}");
                    else if (!string.IsNullOrWhiteSpace(_default))
                        fields.Add($" coalesce({field.TableName}.{field.FieldName}, '{_default}') {nick}");
                    else
                        fields.Add($" {field.TableName}.{field.FieldName} {nick}");

                    structureContainer.GroupBys.Add($" {field.TableName}.{field.FieldName}");
                }
                else
                {
                    MountOperation(structureContainer, field, fields);
                }
            });

            return $"SELECT {RowsCount(structureContainer, bd)} {string.Join("," + Environment.NewLine, fields)}";
        }

        private string MountCaseWhen(StructureContainer structureContainer, JsonFieldStructure field, string _default)
        {
            var caseWhen = new StringBuilder();
            var fieldCase = structureContainer.ProcessedJson.GetTables().GetField(field.TableName, field.FieldName);

            if (fieldCase.Values.Count > 0)
            {
                caseWhen.AppendLine($" case {field.TableName}.{field.FieldName}");
                fieldCase.Values.ForEach(f => caseWhen.AppendLine($" when {f.Key} then '{f.Value}'"));
                caseWhen.AppendLine($" else '{_default}' end");
            }

            return caseWhen.ToString();
        }

        private string GetDefaultValue(List<ColumnDTO> columns, JsonFieldStructure field)
        {
            string _default = string.Empty;

            var column = columns.Where(c => c.TableName == field.TableName && c.FieldName == field.FieldName);
            if (column.FirstOrDefault() != null)
                _default = column.First().DefaultValue;

            return _default;
        }

        private void OrderColumns(StructureContainer structureContainer, List<ColumnDTO> columns)
        {
            columns?.ForEach(column =>
            {
                var field = structureContainer.Fields
                                              .Where(f => f.TableName == column.TableName && f.FieldName == column.FieldName)
                                              .First();

                if (field != null)
                    field.Order = column.Position;
            });

            structureContainer.Fields = structureContainer.Fields
                                                          .OrderBy(f => f.Order)
                                                          .ToList();
        }

        private string MountNick(JsonFieldStructure field)
        {
            return !string.IsNullOrWhiteSpace(field.FieldFriendlyName) ?
                $"as {EcalcString.RemoverAcentos(Regex.Replace(field.FieldFriendlyName, @"\s+", ""))}" :
                "";
        }

        private void MountOperation(StructureContainer structureContainer, JsonFieldStructure field, List<string> fields)
        {
            if (field.Operation != Operation.Formula)
            {   
                fields.Add(MountOperationStr(field));
                structureContainer.NeedGroupBy = true;
            }
            else
                fields.Add(Formula(structureContainer, field));
        }

        private string Formula(StructureContainer structureContainer, JsonFieldStructure field)
        {
            string command = string.Empty;
            string commandForGroupBy = string.Empty;
            bool goInGroupBy = true;

            field.FieldsForFormula.ForEach(f =>
            {
                if (f.Sign == MathematicalSign.Undefined)
                {
                    if (f.Operation == Operation.Undefined)
                    {
                        command += $" {f.TableName}.{f.FieldName} ";
                        commandForGroupBy += $" {f.TableName}.{f.FieldName} ";
                    }
                    else
                    {
                        command += MountOperationStr(f);
                        structureContainer.NeedGroupBy = true;
                        goInGroupBy = false;
                    }
                }
                else
                {
                    if (f.Operation == Operation.Undefined)
                    {
                        command += $"{MathematicalSignsConverter.ToString(f.Sign)} {f.TableName}.{f.FieldName} ";
                        commandForGroupBy += $"{MathematicalSignsConverter.ToString(f.Sign)} {f.TableName}.{f.FieldName} ";
                    }
                    else
                    {
                        command += $"{MathematicalSignsConverter.ToString(f.Sign)} {MountOperationStr(f)} ";
                        structureContainer.NeedGroupBy = true;
                        goInGroupBy = false;
                    }
                }
            });

            if (goInGroupBy)
                structureContainer.GroupBys.Add(commandForGroupBy);

            return command;
        }

        private string MountOperationStr(JsonFieldStructure field)
        {
            switch (field.Operation)
            {
                case Operation.Sum:
                    return $" SUM({GetFieldWithNick(field)})"; 
                case Operation.Max:
                    return $" MAX({GetFieldWithNick(field)})";   
                case Operation.Min:
                    return $" MIN({GetFieldWithNick(field)})";                 
                case Operation.Count:
                    return $" COUNT({GetFieldWithNick(field)})";   
                case Operation.Average:
                    return $" AVG({GetFieldWithNick(field)})";
                default:
                    return "";
            }
        }

        private string RowsCount(StructureContainer structureContainer, TipoBanco db)
        {
            if (structureContainer.RowsCount == 0)
                return "";
            else
            {
                if (db == TipoBanco.Firebird)
                    return $"first {structureContainer.RowsCount}";
                else
                    return $"top {structureContainer.RowsCount}";
            }
        }
    }
}
