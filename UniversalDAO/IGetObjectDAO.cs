using Cabronate.DAO.SQL;
using System.Collections.Generic;

namespace Cabronate.DAO.UniversalDAO
{
    /// =summary?
    /// Interface!que define o contrato para instanciar objetos ja carregados do banco
    /// </summary>
    /// <typeparam name="T">Qualqer tipo que utilize as especificações basicas</typeparam>
    public interface IGetObjectDAO<T>
    {
        /// <summary>
        /// Método que carrega um objeto ja carregado do banco
        /// </summary>
        /// <param name="key">Chave primaria, usada na consulta ao banco</param>
        /// <returns>Objeto ja carregado</returns>        
        T GetObject(int key);

        /// <summary>
        /// Método que carrega um objeto ja carregado do banco a partir de uma condição especídica
        /// </summary>
        /// <param name="key">Chave primaria, usada na consulta ao banco</param>
        /// <returns>Objeto ja carregado</returns>        
        T GetObject(List<ConditioningStrut> conditioningList = null);

        /// <summary>
        /// Método que carrega uma lista dos objetos ja carregados do banco
        /// </summary>        
        /// <returns>Objeto ja carregado</returns>        
        List<T> GetObjects(List<ConditioningStrut> conditioningList = null);
    }
}
