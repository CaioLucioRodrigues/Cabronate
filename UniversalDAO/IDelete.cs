using System.Collections.Generic;

namespace Cabronate.DAO.UniversalDAO
{
    public interface IDelete<T>
    {
        void DeleteObject(T obj);

        void DeleteObjects(List<T> list);
    }
}
