using Cabronate.Base;
using Cabronate.DAO.Errors;
using Cabronate.DAO.Utils;
using System;

namespace Cabronate.DAO.CheckDBItems
{
    public static class CheckDBItemsFactory
    {
        public static ICheckDBItems getCheckDBItemsClass(TipoBanco tipoBanco)
        {
            switch (tipoBanco)
            {
                case TipoBanco.Firebird: return new CheckDBItemsFirebird();
                case TipoBanco.SQLServer: return new CheckDBItemsSQLServer();
                case TipoBanco.Oracle: return new CheckDBItemsOracle();
                case TipoBanco.MySql: return new CheckDBItemsMySql();
                case TipoBanco.PostgreSQL: return new CheckDBItemsPostgreSQL();
                default: throw new Exception(ErrorMessages.PROVIDER_ERROR);
            }
        }

        public static ListaErros getCheckDBItemsMethod(DBContexto dbctx, DBUtils.TipoItemsBanco tipoItem, ICheckDBItems checkDBItem, string name, string aux = "")
        {
            switch (tipoItem)
            {
                case DBUtils.TipoItemsBanco.Package: return checkDBItem.checkPackageExists(dbctx, name);
                case DBUtils.TipoItemsBanco.Procedure: return checkDBItem.checkProcedureExists(dbctx, name);
                case DBUtils.TipoItemsBanco.UDF: return checkDBItem.checkUDFExists(dbctx, name);
                case DBUtils.TipoItemsBanco.View: return checkDBItem.checkViewExists(dbctx, name);
                case DBUtils.TipoItemsBanco.Field: return checkDBItem.checkFieldExists(dbctx, name, aux);
                case DBUtils.TipoItemsBanco.Table: return checkDBItem.checkTableExists(dbctx, name);
                case DBUtils.TipoItemsBanco.Constraint: return checkDBItem.checkTableExists(dbctx, name);
                default: throw new Exception(string.Format(ErrorMessages.INVALID_DB_ITEMS, tipoItem.ToString()));
            }
        }
    }
}
