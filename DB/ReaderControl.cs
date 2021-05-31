using Cabronate.Base;
using System.Collections.Generic;
using System.Data;

namespace Cabronate.DAO.DB
{
    public class ReaderControl : IReaderControl
    {
        public ReaderControl(DBContexto connection) { this.connection = connection; }

        private readonly DBContexto connection;

        /// <summary>
        /// Método usado para executar um reader
        /// </summary>
        /// <param name="command">Comando a ser usado no banco</param>
        /// <param name="keyValue">Valor da chave primária do registro</param>
        /// <param name="keyColumn">Coluna que representa a chave primária do registro</param>
        /// <returns>Data reader preenchido</returns>
        public IDataReader executeReader(string command, string keyColumn, int keyValue = 0)
        {
            IDbCommand cmd = connection.CreateCommand(command);
            cmd.Connection = connection.GetConnection;

            connection.CreateParameter(cmd, keyColumn, DbType.Int32, keyValue);

            return cmd.ExecuteReader();
        }


        public IDataReader executeReader(string command, List<ParameterData> listParameters)
        {
            IDbCommand cmd = connection.CreateCommand(command);
            cmd.Connection = connection.GetConnection;

            if (listParameters != null)
            {
                foreach (ParameterData parameters in listParameters)
                {
                    connection.CreateParameter(cmd, parameters.name, parameters.dbType, parameters.value);
                }
            }

            return cmd.ExecuteReader();
        }

        public int executeNonQuery(string command)
        {
            IDbCommand cmd = connection.CreateCommand(command);
            cmd.Connection = connection.GetConnection;

            return cmd.ExecuteNonQuery();
        }

        public IDataReader executeCommand(IDbCommand command)
        {
            command.Connection = connection.GetConnection;

            return command.ExecuteReader();
        }
    }
}
