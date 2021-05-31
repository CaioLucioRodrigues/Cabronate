namespace Cabronate.DAO.ReportBuilder.Structures.Extensions
{
    public static class StructureConnectionJoinExtensions
    {
        public static StructureConnectionJoin LeftJoin(this StructureConnectionJoin structure)
        {
            structure.JoinType = Types.Join.LeftJoin;
            return structure;
        }
    }
}
