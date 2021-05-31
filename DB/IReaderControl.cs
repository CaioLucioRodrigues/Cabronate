using Cabronate.Base;
using System.Collections.Generic;
using System.Data;

namespace Cabronate.DAO.DB
{
    /// <summary>
    /// Interface responsável por manter o contrato de seleções no banco de dados
    /// </summary>
    public interface IReaderControl
    {
        /// <summary>
        /// Método usado para executar um reader
        /// </summary>
        /// <param name="command">Comando a ser usado no banco</param>
        /// <param name="keyValue">Valor da chave primária do registro</param>
        /// <param name="keyColumn">Coluna que representa a chave primária do registro</param>
        /// <returns>Data reader preenchido</returns>
        IDataReader executeReader(string command, string keyColumn, int keyValue = 0);

        IDataReader executeReader(string command, List<ParameterData> listParameters);
    }
}
