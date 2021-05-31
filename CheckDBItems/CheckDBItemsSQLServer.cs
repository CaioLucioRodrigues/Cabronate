using Cabronate.Base;
using Cabronate.DAO.Errors;
using System;
using System.Data;

namespace Cabronate.DAO.CheckDBItems
{
    public class CheckDBItemsSQLServer : ICheckDBItems
    {
        #region ICheckDBItems Members

        public Base.ListaErros checkProcedureExists(Base.DBContexto dbctx, string name)
        {
            ListaErros erros = new ListaErros();
            using (IDbCommand command = dbctx.CreateCommand(""))
            {
                command.Connection = dbctx.GetConnection;
                command.CommandText = string.Format(
                    " SELECT COUNT(name) FROM sys.Procedures " +
                    " WHERE UPPER(name) = UPPER('{0}')", name);
                object resultScalar = command.ExecuteScalar();
                if (Convert.ToInt32(resultScalar) == 0)
                {
                    EcalcLog.LogError<CheckDBItemsFirebird>(ErrorMessages.DB_MISS_PROCEDURE, name);
                    erros.AddError(string.Format(ErrorMessages.DB_MISS_PROCEDURE, name), ErrorType.Error);
                }
            }
            return erros;
        }

        public Base.ListaErros checkViewExists(Base.DBContexto dbctx, string name)
        {
            ListaErros erros = new ListaErros();
            using (IDbCommand command = dbctx.CreateCommand(""))
            {
                command.Connection = dbctx.GetConnection;
                command.CommandText = string.Format(
                    " SELECT COUNT(name) FROM sys.views " +
                    " WHERE UPPER(name) = UPPER('{0}') ", name);
                object resultScalar = command.ExecuteScalar();
                if (Convert.ToInt32(resultScalar) == 0)
                {
                    EcalcLog.LogError<CheckDBItemsFirebird>(ErrorMessages.DB_MISS_VIEW, name);
                    erros.AddError(string.Format(ErrorMessages.DB_MISS_VIEW, name), ErrorType.Error);
                }
            }
            return erros;
        }

        public Base.ListaErros checkPackageExists(Base.DBContexto dbctx, string name)
        {
            EcalcLog.LogError<CheckDBItemsFirebird>(ErrorMessages.DB_PACKAGE_ERROR);
            ListaErros erros = new ListaErros();
            erros.AddError(ErrorMessages.DB_PACKAGE_ERROR, ErrorType.Error);
            return erros;
        }

        public Base.ListaErros checkUDFExists(Base.DBContexto dbctx, string name)
        {
            ListaErros erros = new ListaErros();
            using (IDbCommand command = dbctx.CreateCommand(""))
            {
                command.Connection = dbctx.GetConnection;
                command.CommandText = string.Format(
                    " SELECT COUNT(name) FROM sys.objects " +
                    " WHERE type_desc LIKE '%fun%' AND UPPER(name) = UPPER ('{0}') ", name);
                object resultScalar = command.ExecuteScalar();
                if (Convert.ToInt32(resultScalar) == 0)
                {
                    EcalcLog.LogError<CheckDBItemsFirebird>(ErrorMessages.DB_MISS_UDF, name);
                    erros.AddError(string.Format(ErrorMessages.DB_MISS_UDF, name), ErrorType.Error);
                }
            }
            return erros;
        }

        public ListaErros checkFieldExists(Base.DBContexto dbctx, string field, string table)
        {
            ListaErros erros = new ListaErros();
            using (IDbCommand command = dbctx.CreateCommand(""))
            {
                command.Connection = dbctx.GetConnection;
                command.CommandText = string.Format(
                    "SELECT COUNT(f.id)  FROM sysobjects T " +
                    "JOIN syscolumns F ON (F.ID = T.ID) " +
                    "WHERE t.name = '{0}' AND F.NAME = '{1}' ", table, field);
                object resultScalar = command.ExecuteScalar();
                if (Convert.ToInt32(resultScalar) == 0)
                {
                    EcalcLog.LogError<CheckDBItemsFirebird>(ErrorMessages.DB_MISS_FIELD, field, table);
                    erros.AddError(string.Format(ErrorMessages.DB_MISS_FIELD, field, table), ErrorType.Error);
                }
            }
            return erros;
        }

        public ListaErros checkTableExists(Base.DBContexto dbctx, string name)
        {
            ListaErros erros = new ListaErros();
            using (IDbCommand command = dbctx.CreateCommand(""))
            {
                command.Connection = dbctx.GetConnection;
                command.CommandText = string.Format(
                    " SELECT COUNT(t.id)  FROM sysobjects T " +
                    " WHERE t.name = {0} ", name);
                object resultScalar = command.ExecuteScalar();
                if (Convert.ToInt32(resultScalar) == 0)
                {
                    EcalcLog.LogError<CheckDBItemsFirebird>(ErrorMessages.DB_MISS_TABLE, name);
                    erros.AddError(string.Format(ErrorMessages.DB_MISS_TABLE, name), ErrorType.Error);
                }
            }
            return erros;
        }

        public ListaErros checkConstraintExists(Base.DBContexto dbctx, string name)
        {
            ListaErros erros = new ListaErros();
            using (IDbCommand command = dbctx.CreateCommand(""))
            {
                command.Connection = dbctx.GetConnection;
                command.CommandText = string.Format(
                    "SELECT COUNT(i.CONSTRAINT_NAME) " +
                    "FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS i " +
                    "WHERE i.CONSTRAINT_NAME = upper({0})", name);
                object resultScalar = command.ExecuteScalar();
                if (Convert.ToInt32(resultScalar) == 0)
                {
                    EcalcLog.LogError<CheckDBItemsFirebird>(ErrorMessages.DB_MISS_TABLE, name);
                    erros.AddError(string.Format(ErrorMessages.DB_MISS_TABLE, name), ErrorType.Error);
                }
            }
            return erros;
        }


        #endregion
    }
}
