using Cabronate.Base;

namespace Cabronate.DAO.Value_Objects
{
    public interface IDeleteChildren
    {
        ListaErros DeleteChildren(DBContexto dbctx = null);
    }
}
