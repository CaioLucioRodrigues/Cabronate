using Cabronate.Base;
using Cabronate.DAO.QueryBuilder.Extensions;
using System.Collections.Generic;
using System.Data;

namespace Cabronate.DAO.QueryBuilder
{
    public static class BaseQueries
    {
        public static List<string> GetDistinctFieldInList(this DBContexto dbctx, string table, string field)
        {
            var values = new List<string>();

            Query query = dbctx.From(table)
                               .Distinct()
                               .Select(field);            

            foreach (DataRow row in query.Run().Rows)
                values.Add(row[0].ToString());

            return values;
        }

        public static DataTable GetDistinctFieldInDataTable(this DBContexto dbctx, string table, string field)
        {
            var values = new List<string>();

            Query query = dbctx.From(table)
                               .Distinct()
                               .Select(field);

            return query.Run();
        }
    }
}
