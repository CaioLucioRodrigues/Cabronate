using Cabronate.Base;

namespace Cabronate.DAO.Value_Objects
{
    public interface IPersistChildren
    {
        ListaErros SaveChildren(int IDFather, DBContexto dbctx = null);
    }
}
