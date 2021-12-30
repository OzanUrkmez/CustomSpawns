namespace ModIntegration
{
    public class SubMod
    {

        public string SubModuleName { get; private set; }

        public string CustomSpawnsDirectoryPath { get; private set; }

        public SubMod(string subModuleName, string customSpawnsPath)
        {
            SubModuleName = subModuleName;
            CustomSpawnsDirectoryPath = customSpawnsPath;
        }
    }
}
