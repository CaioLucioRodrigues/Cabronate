using Cabronate.Base;
using System;

namespace Cabronate.DAO.Types
{
    public static class TypeFactory
    {
        public static ICTypecs getCType(Type type, TipoBanco provider)
        {
            switch (provider)
            {
                case TipoBanco.SQLServer:
                    return getCTypeSQLServer(type);
                case TipoBanco.Firebird:
                    return getCTypeFireBird(type);
                case TipoBanco.Oracle:
                    return getCTypeOracle(type);
                default:
                    return getCTypeSQLServer(type);
            }
        }

        public static ICTypecs getCType(Type type, string provider)
        {
            switch (provider)
            {
                case "System.Data.SqlClient":
                    return getCTypeSQLServer(type);
                case "FirebirdSql.Data.FirebirdClient":
                    return getCTypeFireBird(type);
                case "Oracle.Data.OracleClient":
                case "ODP.NET, Managed Driver":
                    return getCTypeOracle(type);
                default:
                    return getCTypeSQLServer(type);
            }
        }

        public static ICTypecs getCTypeSQLServer(Type type)
        {
            switch (type.ToString())
            {
                case "System.String":
                    return new CString();
                case "System.DateTime":
                case "System.Nullable`1[System.DateTime]":
                    return new CDateTimeSQLServer();
                case "System.Boolean":
                case "System.Nullable`1[System.Boolean]":
                    return new CBoolean();
                case "System.IO.Stream":
                    return new CStream();
                case "System.Int32":
                case "System.Nullable`1[System.Int32]":
                    return new CInt();
                case "System.Double":
                case "System.Nullable`1[System.Double]":
                    return new CDouble();
                case "System.Decimal":
                case "System.Nullable`1[System.Decimal]":
                    return new CDecimal();
                default:
                    return new CDefault();
            }
        }

        public static ICTypecs getCTypeFireBird(Type type)
        {

            switch (type.ToString())
            {
                case "System.String":
                    return new CString();
                case "System.DateTime":
                case "System.Nullable`1[System.DateTime]":
                    return new CDateTimeFireBird();
                case "System.Boolean":
                case "System.Nullable`1[System.Boolean]":
                    return new CBoolean();
                case "System.IO.Stream":
                    return new CStream();
                case "System.Int32":
                case "System.Nullable`1[System.Int32]":
                    return new CInt();
                case "System.Nullable`1[System.Double]":
                case "System.Double":
                    return new CDouble();
                case "System.Decimal":
                case "System.Nullable`1[System.Decimal]":
                    return new CDecimal();
                default:
                    return new CDefault();
            }
        }

        public static ICTypecs getCTypeOracle(Type type)
        {
            switch (type.ToString())
            {
                case "System.String":
                    return new CString();
                case "System.DateTime":
                case "System.Nullable`1[System.DateTime]":
                    return new CDateTimeOracle();
                case "System.Boolean":
                case "System.Nullable`1[System.Boolean]":
                    return new CBoolean();
                case "System.IO.Stream":
                    return new CStream();
                case "System.Int32":
                case "System.Nullable`1[System.Int32]":
                    return new CInt();
                case "System.Double":
                case "System.Nullable`1[System.Double]":
                    return new CDouble();
                case "System.Decimal":
                case "System.Nullable`1[System.Decimal]":
                    return new CDecimal();
                default:
                    return new CDefault();
            }
        }
    }
}
