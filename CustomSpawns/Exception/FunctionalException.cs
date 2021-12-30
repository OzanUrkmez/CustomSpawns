using System;

namespace CustomSpawns
{
    public class FunctionalException : Exception
    {
        public FunctionalException()
        {
        }

        public FunctionalException(string message)
            : base(message)
        {
        }

        public FunctionalException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}