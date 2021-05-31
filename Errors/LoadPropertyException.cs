using System;

namespace Cabronate.DAO.Errors
{
    [Serializable]
    public class LoadPropertyException : ApplicationException
    {
        public DateTime ErrorTimeStamp { get; set; }

        public LoadPropertyException() { }

        public LoadPropertyException(string message, DateTime time)
            : base(message)
        {
            this.ErrorTimeStamp = time;
        }

        public LoadPropertyException(string message, System.Exception inner)
            : base(message, inner) { }

        public LoadPropertyException(string message, System.Exception inner, DateTime time)
            : base(message, inner)
        {
            this.ErrorTimeStamp = time;
        }

        public LoadPropertyException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
