using System;
using System.Collections.Generic;
using System.Linq;

namespace Cabronate.DAO.ReportBuilder.StructureJsons.Extensions
{
    public static class JsonTableExtensions
    {
        public static JsonTableStructure GetTable(this List<JsonTableStructure> tables, string tableName)
        {
            JsonTableStructure table = null;
            try
            {
                table = tables.FirstOrDefault(t => t.TableName == tableName || t.TableFriendlyName == tableName);
            }
            catch (Exception e)
            {
                throw new Exception($"Erro ao carregar a tabela {tableName}.", e);
            }
            if (table == null) throw new Exception($"Tabela {tableName} não encontrada");
            return table;
        }
    }
}
