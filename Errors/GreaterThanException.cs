using System;

namespace Cabronate.DAO.Errors
{
    [Serializable]
    public class GreaterThanException : ApplicationException
    {
        public DateTime ErrorTimeStamp { get; set; }

        public GreaterThanException(string message)
            : base(message) { }

        public GreaterThanException(string message, System.Exception inner)
            : base(message, inner) { }

        public GreaterThanException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
