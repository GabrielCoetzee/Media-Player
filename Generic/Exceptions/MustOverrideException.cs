using System;
using System.Runtime.Serialization;

namespace Generic.Exceptions
{
    public class MustOverrideException : Exception
    {
        public MustOverrideException()
        {
        }

        public MustOverrideException(string message) 
            : base(message)
        {
        }

        public MustOverrideException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }

        protected MustOverrideException(SerializationInfo info, StreamingContext context) 
            : base(info, context)
        {
        }
    }
}
