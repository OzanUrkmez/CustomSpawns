using System;

namespace CustomSpawns.Exception
{
    public class TechnicalException : System.Exception
    {
        public TechnicalException()
        {
        }

        public TechnicalException(string message)
            : base(message)
        {
        }

        public TechnicalException(string message, System.Exception inner)
            : base(message, inner)
        {
        }
    }
}