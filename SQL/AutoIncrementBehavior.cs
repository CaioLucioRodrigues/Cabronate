using Cabronate.DAO.Errors;
using System;

namespace Cabronate.DAO.SQL
{
    public interface IAutoIncrementBehavior
    {
        string getAutoIncrementQuery(string table, string keyfield);
        string getKeyValueQuery(string table, string keyfield);
        bool keyFieldInInsert();
    }

    public class AutoIncrementFireBirdBehavior : IAutoIncrementBehavior
    {
        public string getAutoIncrementQuery(string table, string keyfield)
        {
            return (" SELECT GEN_ID(GEN_" + table + "_" + keyfield + ",1) FROM RDB$DATABASE ");
        }

        public string getKeyValueQuery(string table, string keyfield)
        {
            return (" SELECT GEN_ID(GEN_" + table + "_" + keyfield + ",0) FROM RDB$DATABASE ");
        }

        public bool keyFieldInInsert() { return true; }
    }

    public class AutoIncrementOracleBehavior : IAutoIncrementBehavior
    {
        public string getAutoIncrementQuery(string table, string keyfield)
        {
            return (" SELECT GEN_" + table + "_" + keyfield + ".NEXTVAL FROM DUAL ");
        }

        public string getKeyValueQuery(string table, string keyfield)
        {
            return (" SELECT GEN_" + table + "_" + keyfield + ".CURRVAL FROM DUAL ");
        }

        public bool keyFieldInInsert() { return true; }
    }

    public class AutoIncrementSQLServerBehavior : IAutoIncrementBehavior
    {
        public string getAutoIncrementQuery(string table, string keyfield)
        {
            return (" SELECT " + keyfield + " FROM " + table + " WHERE 1 = 2 "); //Para não trazer nada.
        }

        public string getKeyValueQuery(string table, string keyfield)
        {
            return (" SELECT IDENT_CURRENT('" + table + "') ");
        }

        public bool keyFieldInInsert() { return false; }
    }

    public static class AutoIncrementBehaviorFactory
    {
        public static IAutoIncrementBehavior getAutoIncrementBehavior(string provider)
        {
            switch (provider)
            {
                case "System.Data.SqlClient":
                    return new AutoIncrementSQLServerBehavior();
                case "FirebirdSql.Data.FirebirdClient":
                    return new AutoIncrementFireBirdBehavior();
                case "Oracle.Data.OracleClient":
                case "ODP.NET, Managed Driver":
                    return new AutoIncrementOracleBehavior();
                default:
                    throw new Exception(ErrorMessages.PROVIDER_ERROR);
            }
        }
    }
}
