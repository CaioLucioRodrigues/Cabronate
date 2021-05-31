using Cabronate.DAO.ReportBuilder.Structures;
using System.Collections.Generic;
using System.Linq;

namespace Cabronate.DAO.ReportBuilder.StructureJsons.Extensions
{
    public static class JsonRelationshipExtensions
    {
        public static List<JsonTableStructure> GetRelations(this List<JsonTableStructure> jsonTablesStructure, string table)
        {
            List<JsonTableStructure> relationTables = new List<JsonTableStructure>();

            jsonTablesStructure.Where(t => t.TableName == table)
                               .FirstOrDefault()
                               .Relationships
                               .ForEach(r => {
                                   relationTables.Add(jsonTablesStructure.Where(t => t.TableName == r.TableTarget)
                                                                         .FirstOrDefault());
                               });
            return relationTables;
        }

        public static List<JsonTableStructure> GetRelationsDetailed(this List<JsonTableStructure> jsonTablesStructure, string table)
        {
            List<JsonTableStructure> relationTables = new List<JsonTableStructure>();

            jsonTablesStructure.Where(t => t.TableName == table)
                               .FirstOrDefault()
                               .Relationships
                               .ForEach(r => {
                                   relationTables.Add(jsonTablesStructure.Where(t => t.TableName == r.TableTarget)
                                                                         .FirstOrDefault());
                               });
            return relationTables;
        }

        public static JsonRelationshipStructure GetRelation(this JsonTableStructure tableOrign, JsonTableStructure tableTarget)
        {
            return tableOrign.Relationships.Where(r => r.TableTarget == tableTarget.TableName)
                                           .FirstOrDefault();
        }
    }
}
