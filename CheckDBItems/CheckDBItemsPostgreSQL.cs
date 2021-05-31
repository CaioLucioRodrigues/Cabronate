using Cabronate.Base;
using System;

namespace Cabronate.DAO.CheckDBItems
{
    public class CheckDBItemsPostgreSQL : ICheckDBItems
    {
        #region ICheckDBItems Members

        public ListaErros checkProcedureExists(DBContexto dbctx, string name)
        {
            throw new NotImplementedException();
        }

        public ListaErros checkViewExists(DBContexto dbctx, string name)
        {
            throw new NotImplementedException();
        }

        public ListaErros checkPackageExists(DBContexto dbctx, string name)
        {
            throw new NotImplementedException();
        }

        public ListaErros checkUDFExists(DBContexto dbctx, string name)
        {
            throw new NotImplementedException();
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
