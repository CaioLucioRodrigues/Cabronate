using System;

namespace Cabronate.DAO.Errors
{
    /// <summary>
    /// Erro que indica quando o registro não existe no BD
    /// </summary>
    [Serializable]
    public class RecordNotFoundException : ApplicationException
    {
        /// <summary>
        /// Data em que o erro ocorreu
        /// </summary>
        public DateTime ErrorTimeStamp { get; set; }

        /// <summary>
        /// Constructor padrão
        /// </summary>
        public RecordNotFoundException() { }

        /// <summary>
        /// Constructor com os devidos detalhamentos
        /// </summary>
        /// <param name="message">Mensagem do erro</param>
        /// <param name="time">Hora em que o erro ocorreu</param>
        public RecordNotFoundException(string message, DateTime time)
            : base(message)
        {
            this.ErrorTimeStamp = time;
        }

        /// <summary>
        /// Constructor com os devidos detalhamentos, passa o inner para o constructor base
        /// </summary>        
        /// <param name="time">Hora em que o erro ocorreu</param>
        public RecordNotFoundException(string message, System.Exception inner)
            : base(message, inner) { }

        /// <summary>
        /// Constructor com as informações para a serialização
        /// </summary>                
        public RecordNotFoundException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
