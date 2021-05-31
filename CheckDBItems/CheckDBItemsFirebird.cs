using Cabronate.Base;
using Cabronate.DAO.Errors;
using System;
using System.Data;

namespace Cabronate.DAO.CheckDBItems
{
    public class CheckDBItemsFirebird : ICheckDBItems
    {
        #region ICheckDBItems Members

        public ListaErros checkProcedureExists(Base.DBContexto dbctx, string name)
        {
            ListaErros erros = new ListaErros();
            using (IDbCommand command = dbctx.CreateCommand(""))
            {
                command.Connection = dbctx.GetConnection;
                command.CommandText = string.Format(
                    " SELECT COUNT(rdb$procedure_name) FROM rdb$procedures  " +
                    " WHERE UPPER(rdb$procedure_name) = UPPER('{0}') ", name);
                object resultScalar = command.ExecuteScalar();
                if (Convert.ToInt32(resultScalar) == 0)
                {
                    EcalcLog.LogError<CheckDBItemsFirebird>(ErrorMessages.DB_MISS_PROCEDURE, name);
                    erros.AddError(string.Format(ErrorMessages.DB_MISS_PROCEDURE, name), ErrorType.Error);
                }
            }
            return erros;
        }

        public ListaErros checkViewExists(Base.DBContexto dbctx, string name)
        {
            ListaErros erros = new ListaErros();
            using (IDbCommand command = dbctx.CreateCommand(""))
            {
                command.Connection = dbctx.GetConnection;
                command.CommandText = string.Format(
                    " SELECT COUNT(rdb$view_name) FROM rdb$view_relations " +
                    " WHERE UPPER(rdb$view_name) = UPPER('{0}') ", name);
                object resultScalar = command.ExecuteScalar();
                if (Convert.ToInt32(resultScalar) == 0)
                {
                    EcalcLog.LogError<CheckDBItemsFirebird>(ErrorMessages.DB_MISS_VIEW, name);
                    erros.AddError(string.Format(ErrorMessages.DB_MISS_VIEW, name), ErrorType.Error);
                }
            }
            return erros;
        }

        public ListaErros checkPackageExists(Base.DBContexto dbctx, string name)
        {
            EcalcLog.LogError<CheckDBItemsFirebird>(ErrorMessages.DB_PACKAGE_ERROR);
            ListaErros erros = new ListaErros();
            erros.AddError(ErrorMessages.DB_PACKAGE_ERROR, ErrorType.Error);
            return erros;
        }

        public ListaErros checkUDFExists(Base.DBContexto dbctx, string name)
        {
            ListaErros erros = new ListaErros();
            using (IDbCommand command = dbctx.CreateCommand(""))
            {
                command.Connection = dbctx.GetConnection;
                command.CommandText = string.Format(
                    " SELECT COUNT(rdb$function_name) FROM rdb$functions " +
                    " WHERE UPPER(rdb$function_name) = UPPER('{0}') ", name);
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
                    "SELECT COUNT(rdb$relation_name) FROM rdb$relation_fields " +
                    "WHERE UPPER(rdb$relation_name) = UPPER('{0}') " +
                    "AND UPPER(rdb$field_name) = UPPER('{1}') ", table, field);
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
                    "SELECT COUNT(rdb$relation_name) FROM rdb$relation_fields " +
                    "WHERE UPPER(rdb$relation_name) = UPPER('{0}') ", name);
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
                    "SELECT COUNT(R.RDB$CONSTRAINT_NAME) FROM RDB$REF_CONSTRAINTS R " +
                    "WHERE RDB$CONSTRAINT_NAME = UPPER('{0}') ", name);
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
