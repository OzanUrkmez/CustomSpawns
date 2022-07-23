namespace CustomSpawns.Exception
{
    public class FunctionalException : System.Exception
    {
        public FunctionalException()
        {
        }

        public FunctionalException(string message)
            : base(message)
        {
        }

        public FunctionalException(string message, System.Exception inner)
            : base(message, inner)
        {
        }
    }
}