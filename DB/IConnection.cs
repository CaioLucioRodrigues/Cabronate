using System.Data;

namespace Cabronate.DAO.DB
{
    /// <summary>
    /// Interface que define o contrato principal de uma classe de acesso
    /// </summary>
    public interface IConnection
    {
        /// <summary>
        /// Propriedade que representa a conexão
        /// </summary>
        IDbConnection connection { get; set; }
    }
}
