using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Library;
using System.IO;
using System.Xml;
using CustomSpawns.Utils;

namespace CustomSpawns.ModIntegration
{
    public static class SubModManager
    {

        private static List<SubMod>? _cachedSubMods;
        
        public static List<SubMod> LoadAllValidDependentMods()
        {
            if (_cachedSubMods != null)
            {
                return _cachedSubMods;
            }
            //construct the array
            var loadedModules = TaleWorlds.Engine.Utilities.GetModulesNames();
            string basePath = Path.Combine(BasePath.Name, "Modules");
            var all = Directory.EnumerateDirectories(basePath);
            List<SubMod> subMods = new();
            foreach (string path in all)
            {
                string subModDefinitionPath = Path.Combine(path, "CustomSpawns", "CustomSpawnsSubMod.xml");
                if (!File.Exists(Path.Combine(path, "Submodule.xml")) || !File.Exists(subModDefinitionPath))
                {
                    continue;
                }

                XmlDocument doc = new();
                doc.Load(subModDefinitionPath);
                string? subModuleName = doc.DocumentElement?["SubModuleName"]?.InnerText;
                if (string.IsNullOrWhiteSpace(subModuleName))
                {
                    ErrorHandler.HandleException(new System.Exception("The submodule in path " + path + " in the CustomSpawnsSubMod.xml file is not valid. " +
                                                                      "Either the SubModuleName element is missing in the CustomSpawnsSubMod.xml or its value is empty"));
                }

                if (loadedModules.Contains(subModuleName)) //load mod only if it is enabled.
                {
                    SubMod mod = new(subModuleName!, Path.Combine(path, "CustomSpawns"));
                    subMods.Add(mod);
                }
            }

            _cachedSubMods = subMods; 
            return subMods;
        }

    }
}
