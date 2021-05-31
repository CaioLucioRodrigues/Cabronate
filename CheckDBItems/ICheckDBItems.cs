using Cabronate.Base;

namespace Cabronate.DAO.CheckDBItems
{
    public interface ICheckDBItems
    {
        ListaErros checkProcedureExists(DBContexto dbctx, string name);
        ListaErros checkViewExists(DBContexto dbctx, string name);
        ListaErros checkPackageExists(DBContexto dbctx, string name);
        ListaErros checkUDFExists(DBContexto dbctx, string name);
        ListaErros checkFieldExists(DBContexto dbctx, string field, string tabela);
        ListaErros checkTableExists(DBContexto dbctx, string tabela);
        ListaErros checkConstraintExists(DBContexto dbctx, string tabela);
    }
}
