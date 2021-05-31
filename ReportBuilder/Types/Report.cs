namespace Cabronate.DAO.ReportBuilder.Types
{
    /// <summary>
    /// ReportTypes.query -> Representa uma consulta que irá retornar um dataset para posteriormente ser incluído em um layout
    /// ReportTypes.nonQuery -> Representa um comando ( count, max, sum... ) que deverá ser comparado com algum parâmetro, simulando um gatilho
    /// </summary>    
    public enum Report
    {
        Undefined = 0,
        Query = 1,
        NonQuery = 2
    }

    public static class ReportConverter
    {
        public static string ToString(Report report)
        {
            switch (report)
            {
                case Report.Undefined:
                    return "";
                case Report.Query:
                    return "Relatório";
                case Report.NonQuery:
                    return "Comando";
                default:
                    return "";
            }
        }
    }
}
