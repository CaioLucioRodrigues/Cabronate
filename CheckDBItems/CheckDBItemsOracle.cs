using Cabronate.Base;
using Cabronate.DAO.Errors;
using System;
using System.Data;

namespace Cabronate.DAO.CheckDBItems
{
    public class CheckDBItemsOracle : ICheckDBItems
    {
        #region ICheckDBItems Members

        public Base.ListaErros checkProcedureExists(Base.DBContexto dbctx, string name)
        {
            ListaErros erros = new ListaErros();
            using (IDbCommand command = dbctx.CreateCommand(""))
            {
                command.Connection = dbctx.GetConnection;
                command.CommandText = string.Format(
                    " SELECT COUNT(object_name) FROM user_objects " +
                    " WHERE object_type = 'PROCEDURE' and UPPER(object_name) = UPPER('{0}')", name);
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
                    " SELECT COUNT(object_name) FROM user_objects " +
                    " WHERE object_type = 'VIEW' and UPPER(object_name) = UPPER('{0}')", name);
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
            ListaErros erros = new ListaErros();
            using (IDbCommand command = dbctx.CreateCommand(""))
            {
                command.Connection = dbctx.GetConnection;
                command.CommandText = string.Format(
                    " SELECT COUNT(object_name) FROM user_objects " +
                    " WHERE object_type = 'PACKAGE' and UPPER(object_name) = UPPER('{0}')", name);
                object resultScalar = command.ExecuteScalar();
                if (Convert.ToInt32(resultScalar) == 0)
                {
                    EcalcLog.LogError<CheckDBItemsFirebird>(ErrorMessages.DB_MISS_PACKAGE, name);
                    erros.AddError(string.Format(ErrorMessages.DB_MISS_PACKAGE, name), ErrorType.Error);
                }
            }
            return erros;
        }

        public Base.ListaErros checkUDFExists(Base.DBContexto dbctx, string name)
        {
            ListaErros erros = new ListaErros();
            using (IDbCommand command = dbctx.CreateCommand(""))
            {
                command.Connection = dbctx.GetConnection;
                command.CommandText = string.Format(
                    " SELECT COUNT(object_name) FROM user_objects " +
                    " WHERE object_type = 'FUNCTION' and UPPER(object_name) = UPPER('{0}')", name);
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
            throw new NotImplementedException();
        }

        public ListaErros checkTableExists(Base.DBContexto dbctx, string name)
        {
            throw new NotImplementedException();
        }

        public ListaErros checkConstraintExists(Base.DBContexto dbctx, string name)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
