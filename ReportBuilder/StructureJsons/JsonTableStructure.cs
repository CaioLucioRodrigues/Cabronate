using System.Collections.Generic;

namespace Cabronate.DAO.ReportBuilder.StructureJsons
{
    public class JsonTableStructure
    {
        public JsonTableStructure()
        {
            Relationships = new List<JsonRelationshipStructure>();
            Fields = new List<JsonFieldStructure>();
        }

        public string TableName { get; set; }

        public string TableFriendlyName { get; set; }

        public List<JsonRelationshipStructure> Relationships { get; set; }

        public List<JsonFieldStructure> Fields { get; set; }

        public override string ToString()
        {
            return (!string.IsNullOrWhiteSpace(TableFriendlyName) ? TableFriendlyName : TableName);
        }
    }
}
