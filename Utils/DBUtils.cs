using System.Data;
using System.Data.Common;

namespace Cabronate.DAO.Utils
{
    public static class DBUtils
    {
        public static void AddParam(this IDataParameterCollection collection, DbProviderFactory df, string paramName, object value)
        {
            if (collection.IndexOf(paramName) >= 0)
                collection.RemoveAt(paramName);

            DbParameter param = df.CreateParameter();
            param.ParameterName = paramName;
            param.Value = value;
            collection.Add(param);
        }

        public enum TipoItemsBanco
        {
            Procedure = 1,
            View = 2,
            Package = 3,
            UDF = 4,
            Field = 5,
            Table = 6,
            Constraint = 7
        }
    }
}
