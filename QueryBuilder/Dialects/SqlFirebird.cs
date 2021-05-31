using Cabronate.DAO.QueryBuilder.Statements;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cabronate.DAO.QueryBuilder.Dialects
{
    public class SqlFirebird : SqlANSI
    {
        public override string OffsetPaginationStatement(Pagination pagination)
        {
            StringBuilder str = new StringBuilder();
            if (pagination.Number <= 0 && pagination.Offset > 0)
                pagination.Number = 1;

            if (pagination.Number > 0)
                str.Append($"ROWS {pagination.Number}");

            if (pagination.Offset > 0)
                str.Append($" TO {pagination.Offset}");
             
            return str.ToString();
        }
    }
}
