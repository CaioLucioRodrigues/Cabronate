using Cabronate.Base;
using Cabronate.DAO.QueryBuilder.Dialects;
using Cabronate.DAO.QueryBuilder.Interfaces;

namespace Cabronate.DAO.QueryBuilder
{
    public static class QueryFactory
    {
        public static IQueryStatements GetQueryBuilder(DBContexto dbctx)
        {
            switch (dbctx.tipoBanco)
            {
                case TipoBanco.Firebird:
                    return new SqlFirebird();
                case TipoBanco.SQLServer:
                    return new SqlMSSQL();
                case TipoBanco.Oracle:
                case TipoBanco.MySql:
                    throw new System.NotImplementedException();
                default:
                    return new SqlANSI();
            }
        }
    }
}
