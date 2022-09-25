namespace CustomSpawns.Utils
{
    public static class Utils
    {
        public static bool IsCustomSpawnsStringID(string stringID)
        {
            return (stringID.StartsWith("cs_"));
        }
    }
}
