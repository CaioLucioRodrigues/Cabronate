using Cabronate.Base;
using Cabronate.DAO.ReportBuilder.Types;
using Cabronate.DAO.SQL;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cabronate.DAO.ReportBuilder.StructureJsons.Extensions
{
    public static class JsonTableFieldExtensions
    {
        public static List<JsonFieldStructure> GetFields(this List<JsonTableStructure> tables)
        {
            List<JsonFieldStructure> fields = new List<JsonFieldStructure>();
            tables.Distinct()
                  .ToList()
                  .ForEach(table => {
                      table.Fields.ForEach(field => { fields.Add(field); });
                  });

            return fields;
        }

        public static JsonFieldStructure GetField(this List<JsonTableStructure> tables, string tableName, string fieldName)
        {
            JsonFieldStructure field = null;
            try
            {
                field = tables.GetTable(tableName)
                              .Fields
                              .FirstOrDefault(f => f.FieldName == fieldName || f.FieldFriendlyName == fieldName);
            }
            catch (Exception e)
            {
                throw new Exception($"Erro ao carregar o campo {fieldName} da tabela {tableName}.", e);
            }
            if (field == null) throw new Exception($"Campo {fieldName} não encontrada");
            return field.DeepCopy<JsonFieldStructure>();
        }

        public static List<JsonFieldStructure> GetFieldsWithOperation(this List<JsonTableStructure> tables, Operation operation)
        {
            switch (operation)
            {
                case Operation.Undefined:
                    return tables.GetFields();
                case Operation.Sum:
                    return tables.GetNumericFields();
                case Operation.Max:
                    return tables.GetNumericFields();
                case Operation.Min:
                    return tables.GetNumericFields();
                case Operation.Count:
                    return tables.GetNumericFields();
                case Operation.Average:
                    return tables.GetNumericFields();
                default:
                    return tables.GetFields();
            }
        }

        private static List<JsonFieldStructure> GetNumericFields(this List<JsonTableStructure> tables)
        {
            List<JsonFieldStructure> fields = new List<JsonFieldStructure>();
            tables.Distinct()
                  .ToList()
                  .ForEach(table => {
                      table.Fields.Where(field => field.TypeData == DataType.Integer || field.TypeData == DataType.Double)
                                  .ToList()
                                  .ForEach(field => { fields.Add(field); });
                  });

            return fields;
        }

        private static List<JsonFieldStructure> GetStringFields(this List<JsonTableStructure> tables)
        {
            List<JsonFieldStructure> fields = new List<JsonFieldStructure>();
            tables.Distinct()
                  .ToList()
                  .ForEach(table => {
                      table.Fields.Where(field => field.TypeData == DataType.String)
                                  .ToList()
                                  .ForEach(field => { fields.Add(field); });
                  });

            return fields;
        }

        private static List<JsonFieldStructure> GetDateFields(this List<JsonTableStructure> tables)
        {
            List<JsonFieldStructure> fields = new List<JsonFieldStructure>();
            tables.Distinct()
                  .ToList()
                  .ForEach(table => {
                      table.Fields.Where(field => field.TypeData == DataType.Date)
                                  .ToList()
                                  .ForEach(field => { fields.Add(field); });
                  });

            return fields;
        }

        public static List<string> GetFieldsText(this List<JsonTableStructure> tables)
        {
            List<string> fields = new List<string>();
            tables.Distinct()
                  .ToList()
                  .ForEach(table => {
                      table.Fields.ForEach(field =>
                      {
                          fields.Add($"{table.ToString()} - {field.ToString()}");
                      });
                  });

            return fields;
        }
    }
}
