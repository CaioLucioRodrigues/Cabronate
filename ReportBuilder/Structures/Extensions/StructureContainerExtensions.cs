using Cabronate.Base;
using Cabronate.DAO.ReportBuilder.StructureJsons;
using Cabronate.DAO.ReportBuilder.StructureJsons.Extensions;
using Cabronate.DAO.ReportBuilder.Types;
using Cabronate.DAO.SQL;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cabronate.DAO.ReportBuilder.Structures.Extensions
{
    public static class StructureContainerExtensions
    {
        public static StructureContainer Query(this StructureContainer container)
        {
            container.ReportType = Report.Query;
            return container;
        }

        public static StructureContainer NonQuery(this StructureContainer container)
        {
            container.ReportType = Report.NonQuery;
            return container;
        }

        public static StructureContainer AddTable(this StructureContainer container, string tableOrigin, string tableTarget = null, JsonRelationshipStructure relation = null)
        {
            JsonTableStructure origin = container.ProcessedJson.GetTables().GetTable(tableOrigin);            

            if (string.IsNullOrEmpty(tableTarget))
            {
                container.FirstTable(origin); 
            }
            else
            {
                JsonTableStructure target = container.ProcessedJson.GetTables().GetTable(tableTarget);
                container.Join(origin, target, relation);
            }

            return container;
        }

        public static StructureContainer RemoveTable(this StructureContainer container, string table)
        {
            container.Joins
                     .RemoveAll(j => j.TableOrign.TableName == table || j.TableTarget?.TableName == table);
            
            container.Fields
                     .RemoveAll(f => f.TableName == table);

            container.Filters
                     .RemoveAll(f => f.Field?.TableName == table);

            return container;
        }

        public static StructureContainer RemoveField(this StructureContainer container, string table, string field)
        {
            container.Fields
                     .RemoveAll(f => f.TableName == table && f.FieldName == field);

            return container;
        }

        public static StructureContainer RemoveFilter(this StructureContainer container, string table, string field)
        {
            container.Filters
                     .RemoveAll(f => f.Field.TableName == table && f.Field.FieldName == field);

            return container;
        }

        public static StructureContainer FirstTable(this StructureContainer container, JsonTableStructure table)
        {
            container.Joins.Add(new StructureConnectionJoin()
            {
                TableOrign = table,
                TableTarget = null,
                Relationship = null,
                FirstTable = true
            });
            return container;
        }

        public static StructureContainer Join(this StructureContainer container, JsonTableStructure tableOrign, JsonTableStructure tableTaget, JsonRelationshipStructure relation)
        {
            container.Joins.Add(new StructureConnectionJoin()
            {
                TableOrign = tableOrign,
                TableTarget = tableTaget,
                Relationship = relation,
                FirstTable = false
            });
            return container;
        }

        public static StructureContainer Join(this StructureContainer container, string tableTarget)
        { 
            JsonTableStructure target = container.ProcessedJson.GetTables().GetTable(tableTarget);
            container.Joins.Add(new StructureConnectionJoin()
            {
                TableOrign = null,
                TableTarget = target,
                Relationship = null,
                FirstTable = false
            });
            return container;
        }

        public static StructureContainer In(this StructureContainer container, string tableOrigin)
        {
            JsonTableStructure origin = container.ProcessedJson.GetTables().GetTable(tableOrigin);
            StructureConnectionJoin join = container.Joins
                                                    .Single(j => !j.FirstTable && j.TableOrign == null);

            join.TableOrign = origin;            
            return container;
        }

        public static StructureContainer On(this StructureContainer container, params string[] fields)
        {
            StructureConnectionJoin join = container.Joins.Single(j => !j.FirstTable && j.Relationship == null);            
            JsonTableStructure structure = container.ProcessedJson.GetTables().GetTable(join.TableOrign.TableName);

            JsonRelationshipStructure relation = new JsonRelationshipStructure(join.TableTarget.TableName);

            foreach (string field in fields)
            {
                relation.Links.Add(new JsonLinksStructure()
                {
                    TableTargetConnectoinField = structure.Relationships.SelectMany(r => r.Links).Single(l => l.TableTargetConnectoinField == field).TableTargetConnectoinField,
                    SelfConnectionField = structure.Relationships.SelectMany(r => r.Links).Single(l => l.TableTargetConnectoinField == field).SelfConnectionField
                });
            }
            join.Relationship = relation;
            return container;
        }

        public static StructureContainer Sum(this StructureContainer container, string tableName, string fieldName)
        {
            container.addFunction(Operation.Sum, tableName, fieldName);
            return container;
        }

        public static StructureContainer Min(this StructureContainer container, string tableName, string fieldName)
        {
            container.addFunction(Operation.Min, tableName, fieldName);
            return container;
        }

        public static StructureContainer Average(this StructureContainer container, string tableName, string fieldName)
        {
            container.addFunction(Operation.Average, tableName, fieldName);
            return container;
        }

        public static StructureContainer Count(this StructureContainer container, string tableName, string fieldName)
        {
            container.addFunction(Operation.Count, tableName, fieldName);
            return container;
        }

        public static StructureContainer Max(this StructureContainer container, string tableName, string fieldName)
        {
            container.addFunction(Operation.Max, tableName, fieldName);
            return container;
        }

        public static StructureContainer addFunction(this StructureContainer container, Operation function, string tableName, string fieldName)
        {
            if (container.Fields.Any(f => f.TableName == tableName && f.FieldName == fieldName))
            {
                container.Fields
                         .Where(f => f.TableName == tableName && f.FieldName == fieldName)
                         .First()
                         .Operation = function;
            }
            else
            {
                JsonFieldStructure field = container.Tables.GetField(tableName, fieldName);
                field.Operation = function;
                container.Fields.Add(field);
            }   
            return container;
        }

        public static StructureContainer Formula(this StructureContainer container, string tableName, string fieldName, Operation operation = Operation.Undefined)
        {
            var field = new JsonFieldStructure();
            field.InMountingFormula = true;
            field.Operation = Operation.Formula;            

            JsonFieldStructure fieldForFormula = container.Tables.GetField(tableName, fieldName);
            fieldForFormula.Operation = operation;
            field.FieldsForFormula.Add(fieldForFormula);
            container.Fields.Add(field);    
            
            return container;
        }

        public static StructureContainer Addition(this StructureContainer container, string tableName, string fieldName, Operation operation = Operation.Undefined)
        {
            JsonFieldStructure field = container.Fields.Where(f => f.InMountingFormula).First();
            JsonFieldStructure otherField = container.Tables.GetField(tableName, fieldName);
            otherField.Sign = MathematicalSign.Addition;
            otherField.Operation = operation;
            field.FieldsForFormula.Add(otherField);

            return container;
        }

        public static StructureContainer Subtraction(this StructureContainer container, string tableName, string fieldName, Operation operation = Operation.Undefined)
        {
            JsonFieldStructure field = container.Fields.Where(f => f.InMountingFormula).First();
            JsonFieldStructure otherField = container.Tables.GetField(tableName, fieldName);
            otherField.Sign = MathematicalSign.Subtraction;
            otherField.Operation = operation;
            field.FieldsForFormula.Add(otherField);

            return container;
        }

        public static StructureContainer Multiplication(this StructureContainer container, string tableName, string fieldName, Operation operation = Operation.Undefined)
        {
            JsonFieldStructure field = container.Fields.Where(f => f.InMountingFormula).First();
            JsonFieldStructure otherField = container.Tables.GetField(tableName, fieldName);
            otherField.Sign = MathematicalSign.Multiplication;
            otherField.Operation = operation;
            field.FieldsForFormula.Add(otherField);

            return container;
        }

        public static StructureContainer Division(this StructureContainer container, string tableName, string fieldName, Operation operation = Operation.Undefined)
        {
            JsonFieldStructure field = container.Fields.Where(f => f.InMountingFormula).First();
            JsonFieldStructure otherField = container.Tables.GetField(tableName, fieldName);
            otherField.Sign = MathematicalSign.Division;
            otherField.Operation = operation;
            field.FieldsForFormula.Add(otherField);

            return container;
        }

        public static StructureContainer EndFormula(this StructureContainer container)
        {
            JsonFieldStructure field = container.Fields.Where(f => f.InMountingFormula).First();
            field.InMountingFormula = false;
            return container;
        }

        public static StructureContainer AddFields(this StructureContainer container, List<JsonFieldStructure> fields)
        {
            container.Fields.AddRange(fields);
            return container;
        }

        public static StructureContainer AddField(this StructureContainer container, string tableName, string fieldName)
        {
            container.Fields.Add(container.Tables.GetField(tableName, fieldName));
            return container;
        }

        public static StructureContainer AddCondition(this StructureContainer structureContainer, DBContexto dbctx, OperatorType operatorType, string table, string field, dynamic value)
        {   
            structureContainer.Filters.Add(
                new StructureFilter(
                    dbctx,
                    operatorType, 
                    structureContainer.ProcessedJson.GetTables().GetTable(table).Fields.Single(f => f.FieldName == field), 
                    value,
                    _fixed: true));
            return structureContainer;
        }

        public static StructureContainer AddDynamicCondition(this StructureContainer structureContainer, DBContexto dbctx, OperatorType operatorType, string table, string field, dynamic value)
        {
            if (operatorType == OperatorType._in || operatorType == OperatorType.notin)
            {
                value = EcalcCSV.ListaToCSV(((string)value).Split(',').ToList());
            }

            structureContainer.Filters.Add(
                new StructureFilter(
                    dbctx,
                    operatorType,
                    structureContainer.ProcessedJson.GetTables().GetTable(table).Fields.Single(f => f.FieldName == field),
                    value,
                    _fixed: false));
            return structureContainer;
        }

        public static StructureContainer OrderBy(this StructureContainer container, string tableName, string fieldName)
        {
            container.OrderBys.Add($"{tableName}.{fieldName}");            
            return container;
        }

        public static StructureContainer OrderByDesc(this StructureContainer container, string tableName, string fieldName)
        {
            container.OrderBys.Add($"{tableName}.{fieldName} desc");
            return container;
        }

        public static StructureContainer RowsCount(this StructureContainer container, int rowsCount)
        {
            container.RowsCount = rowsCount;
            return container;
        }
    }
}
