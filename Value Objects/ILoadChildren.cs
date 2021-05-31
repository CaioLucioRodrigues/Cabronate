using Cabronate.Base;

namespace Cabronate.DAO.Value_Objects
{
    public interface ILoadChildren
    {
        void LoadChildren(DBContexto dbctx = null);
    }
}
