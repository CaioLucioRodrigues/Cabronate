using Cabronate.DAO.ReportBuilder.Interfaces;
using Cabronate.DAO.ReportBuilder.Structures.BaseCommands;
using Cabronate.DAO.ReportBuilder.Types;

namespace Cabronate.DAO.ReportBuilder.Structures.Factories
{
    public static class StructureBaseCommandFactory
    {
        public static IBaseCommand GetBaseCommand(Report reportType)
        {
            switch (reportType)
            {
                case Report.Undefined:
                    return null;
                case Report.Query:
                    return new StructureBaseCommandQuery();
                case Report.NonQuery:
                    return new StructureBaseCommandNonQuery();
                default:
                    return null;
            }
        }
    }
}
