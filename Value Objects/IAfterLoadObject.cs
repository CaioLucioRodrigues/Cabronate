using Cabronate.Base;

namespace Cabronate.DAO.Value_Objects
{
    // Interface para dar uma funcionalidade do tipo "AfterLoad", para se poder fazer algo com os dados recém-carregados
    public interface IAfterLoadObject
    {
        void AfterLoadObject(DBContexto dbctx);
    }
}
