using Cabronate.DAO.ReportBuilder.StructureJsons;
using Cabronate.DAO.ReportBuilder.Types;

namespace Cabronate.DAO.ReportBuilder.Structures
{
    public class StructureConnectionJoin
    {
        public StructureConnectionJoin()
        {
            FirstTable = false;
        }

        public bool FirstTable { get; set; }

        public JsonTableStructure TableOrign { get; set; }

        public JsonTableStructure TableTarget { get; set; }

        public Join JoinType { get; set; }

        public JsonRelationshipStructure Relationship { get; set; }
    }
}