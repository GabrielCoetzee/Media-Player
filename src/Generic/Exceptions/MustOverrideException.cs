using System;

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
    }
}
