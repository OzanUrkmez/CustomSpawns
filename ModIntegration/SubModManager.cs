using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Library;
using System.IO;
using System.Xml;

namespace CustomSpawns.ModIntegration
{
    public static class SubModManager
    {

        private static string[] dependentModsArray;

        public static string[] LoadAllValidDependentMods()
        {
            if (dependentModsArray == null)
            {
                List<string> validPaths = new List<string>();
                //construct the array
                string basePath = Path.Combine(BasePath.Name, "Modules");
                var all = Directory.EnumerateDirectories(basePath);
                foreach (string path in all)
                {
                    if (Directory.Exists(Path.Combine(path, "CustomSpawns")))
                    {
                        //check if mod is a valid M&B mod.
                        try
                        {
                            XmlDocument doc = new XmlDocument();
                            doc.LoadXml(Path.Combine(path, "Submodule.xml"));
                        }
                        catch
                        {
                            ErrorHandler.HandleException(new Exception("The submodule in path " + path + " does not have a SubModule.xml file or has an invalid one!"));
                        }
                        //check if mod is a valid Custom Spawns mod. 
                        try
                        {
                            XmlDocument doc = new XmlDocument();
                            doc.LoadXml(Path.Combine(path, "CustomSpawns", "CustomSpawnsSubMod.xml"));
                        }
                        catch
                        {
                            ErrorHandler.HandleException(new Exception("The submodule in path " + path + " does not have a CustomSpawnsSubMod.xml file or has an invalid one!"));
                        }

                        var loadedMods = new List<ModuleInfo>();
                        foreach (var moduleName in )
                        {

                        }
                        validPaths.Add(Path.Combine(path, "CustomSpawns"));
                        string modName = "";
                        UX.ShowMessage(modName + " is now integrated into the Custom Spawns API!", Color.ConvertStringToColor("#001FFFFF"));
                    }
                }
                dependentModsArray = validPaths.ToArray();
            }
            return dependentModsArray;
        }

    }
}
