using Cabronate.Base;
using Cabronate.Base.Caching;

namespace Cabronate.DAO.DB
{
    /// <summary>
    /// Classe responsável pelo controle do acesso e elementos da conexão.
    /// Utilização de instancias estáticas em sigleton.
    /// Para fazer uso dessa classe, a aplicação precisa possuir um Application Configuration File
    /// com os elementos 'provider' e 'cnStr' preenchidos
    /// </summary>
    public sealed class ConnectionFramework
    {

        /// <summary>
        /// field interno que representa a conexão
        /// </summary>
        static DBContexto connection = null;

        /// <summary>
        /// Objeto usado para travar a requisição da conexão
        /// </summary>
        static readonly object padlock = new object();

        /// <summary>
        /// Propriedade em singleton e estática da conexão
        /// </summary>
        public static DBContexto Connection
        {
            get
            {
                lock (padlock)
                {
                    if ((connection == null) || (!connection.isConnected))
                    {
                        Connect();
                    }
                    return connection;
                }
            }
        }

        /// <summary>
        /// Método que se conecta ao banco
        /// </summary>
        public static void Connect(DBContexto dbctx)
        {
            connection = dbctx;
        }

        /// <summary>
        /// Método que se conecta ao banco
        /// </summary>
        public static void Connect(string Session)
        {
            connection = new DBContexto();
            connection.ConfigurarMultiBase(Session);
            connection.ConectarMultiBase();
        }

        /// <summary>
        /// Método que se conecta ao banco
        /// </summary>
        private static void Connect()
        {
            Connect((string)Manager.Cache
                .GetObjCache<string>(CacheManager.TypeCache.Session, EcalcConsts.CONNECTIONSESSIONKEY));
        }
    }
}