using Cabronate.DAO.Mount;
using System.Collections.Generic;
using System.Data;

namespace Cabronate.DAO.UniversalDAO
{
    /// <summary>
    /// Interface que define o contrato de persistencia dos objetos
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IPersistence<T>
    {
        /// <summary>
        /// Método que persiste a informações de um objeto ( update / insert )
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        int SaveObject(T obj, List<string> fields = null, CommandCompiled cmdCompiled = null, bool? novo = null);

        /// <summary>
        /// Método que persiste a informações de um objeto ( update / insert )
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        int SaveObjects(List<T> objs, List<string> fields = null);
    }
}
