using System;

namespace Cabronate.DAO.Errors
{
    /// <summary>
    /// Exceção customizada que indica que um Value Object não possui as especificações mínimas
    /// como falta do nome da tabela, nome dos fields ou campo que representa a chave primária
    /// </summary>
    [Serializable]
    public class MinimumSpecificationsException : ApplicationException
    {
        /// <summary>
        /// Data em que o erro ocorreu
        /// </summary>
        public DateTime ErrorTimeStamp { get; set; }

        /// <summary>
        /// Constructor padrão
        /// </summary>
        public MinimumSpecificationsException() { }

        /// <summary>
        /// Constructor com os devidos detalhamentos
        /// </summary>
        /// <param name="message">Mensagem do erro</param>
        /// <param name="time">Hora em que o erro ocorreu</param>
        public MinimumSpecificationsException(string message, DateTime time)
            : base(message)
        {
            this.ErrorTimeStamp = time;
        }

        /// <summary>
        /// Constructor com os devidos detalhamentos, passa o inner para o constructor base
        /// </summary>        
        /// <param name="time">Hora em que o erro ocorreu</param>
        public MinimumSpecificationsException(string message, System.Exception inner)
            : base(message, inner) { }

        /// <summary>
        /// Constructor com as informações para a serialização
        /// </summary>                
        public MinimumSpecificationsException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
