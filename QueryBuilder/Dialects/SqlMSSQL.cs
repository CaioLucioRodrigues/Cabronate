using Cabronate.DAO.QueryBuilder.Statements;
using System.Text;

namespace Cabronate.DAO.QueryBuilder.Dialects
{
    public class SqlMSSQL : SqlANSI
    {
        public override string OffsetPaginationStatement(Pagination pagination)
        {
            StringBuilder str = new StringBuilder();
            if (pagination.Number <= 0 && pagination.Offset > 0)
                pagination.Number = 1;

            if (pagination.Number > 0)
                str.Append($"OFFSET {pagination.Number} ROWS");

            if (pagination.Offset > 0)
                str.Append($" FETCH NEXT {pagination.Offset - pagination.Number - 1} ROWS ONLY");

            return str.ToString();
        }
    }
}
