using Cabronate.Base;
using System;

namespace Cabronate.DAO.CheckDBItems
{
    public class CheckDBItemsMySql : ICheckDBItems
    {
        #region ICheckDBItems Members

        public ListaErros checkProcedureExists(Base.DBContexto dbctx, string name)
        {
            throw new NotImplementedException();
        }

        public ListaErros checkViewExists(Base.DBContexto dbctx, string name)
        {
            throw new NotImplementedException();
        }

        public ListaErros checkPackageExists(Base.DBContexto dbctx, string name)
        {
            throw new NotImplementedException();
        }

        public ListaErros checkUDFExists(Base.DBContexto dbctx, string name)
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
