using System.Collections.Generic;
using System.IO;
using System.Xml;
using CustomSpawns.Exception;
using CustomSpawns.Utils;
using TaleWorlds.Library;

namespace CustomSpawns.Data
{
    public class NameSignifierData
    {

        static NameSignifierData _instance;

        public static NameSignifierData Instance
        {
            get
            {
                return _instance ?? new NameSignifierData();
            }
            private set
            {
                _instance = value;

            }
        }

        public static void ClearInstance(Main caller)
        {
            if (caller == null)
                return;
            _instance = null;
        }


        private Dictionary<string, string> IDToName = new Dictionary<string, string>();
        private Dictionary<string, float> IDToSpeedModifier = new Dictionary<string, float>();
        private Dictionary<string, bool> IDToFollowMainParty = new Dictionary<string, bool>();
        private Dictionary<string, float> IDToBaseSpeedOverride = new Dictionary<string, float>();
        private NameSignifierData()
        {
            string path = "";
#if !API_MODE
            path = Path.Combine(BasePath.Name, "Modules", "CustomSpawns", "ModuleData", "Data", "NameSignifiers.xml");
            ConstructFromXML(path);
#endif
            foreach(var subMod in ModIntegration.SubModManager.LoadAllValidDependentMods())
            {
                path = Path.Combine(subMod.CustomSpawnsDirectoryPath, "NameSignifiers.xml");
                if (File.Exists(path))
                    ConstructFromXML(path);
            }

        }

        private void ConstructFromXML(string path)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(path);

                foreach (XmlNode node in doc.DocumentElement)
                {
                    if (node.NodeType == XmlNodeType.Comment)
                        continue;
                    if (node.Attributes["id"] == null || node.Attributes["value"] == null)
                    {
                        ErrorHandler.HandleException(new System.Exception("There must be an id and value attribute defined for each element in NameSignifiers.xml"));
                        continue;
                    }
                    string id = node.Attributes["id"].InnerText;
                    if (!IDToName.ContainsKey(id))
                    {
                        IDToName.Add(id, node.Attributes["value"].InnerText);
                        if (!IDToSpeedModifier.ContainsKey(id))
                        {
                            if (node.Attributes["speed_modifier"] != null)
                            {

                                float result;
                                if (!float.TryParse(node.Attributes["speed_modifier"].InnerText, out result))
                                {
                                    throw new TechnicalException("Please enter a valid float for the speed modifier!");
                                }
                                IDToSpeedModifier.Add(id, result);
                            }
                            else
                            {
                                IDToSpeedModifier.Add(id, 0);
                            }
                        }
                        if (!IDToFollowMainParty.ContainsKey(id))
                        {
                            if (node.Attributes["escort_main_party"] != null)
                            {
                                bool result;
                                if(!bool.TryParse(node.Attributes["escort_main_party"].InnerText, out result))
                                {
                                    throw new TechnicalException("The value for escort_main_party must either be true or false!");
                                }
                                IDToFollowMainParty.Add(id, result);
                            }
                            else
                            {
                                IDToFollowMainParty.Add(id, true);
                            }
                        }
                        if (!IDToBaseSpeedOverride.ContainsKey(id))
                        {
                            if(node.Attributes["base_speed_override"] != null)
                            {
                                float result;
                                if (!float.TryParse(node.Attributes["base_speed_override"].InnerText, out result))
                                {
                                    throw new TechnicalException("Please enter a valid float for the base speed override!");
                                }
                                IDToBaseSpeedOverride.Add(id, result);
                            }
                            else
                            {
                                IDToBaseSpeedOverride.Add(id, float.MinValue);
                            }
                        }
                    }
                }

            }
            catch (System.Exception e)
            {
                ErrorHandler.HandleException(e, "Name Signifier Data Parsing of " + path);
            }
        }

        public string GetPartyNameFromID(string id)
        {
            if (!IDToName.ContainsKey(id))
            {
                ErrorHandler.ShowPureErrorMessage(id + " name signifier could not be found! This is potentially lethal!");
                return "";
            }
            return IDToName[id];
        }

        public float GetSpeedModifierFromID(string id)
        {
            if (!IDToSpeedModifier.ContainsKey(id))
            {
                ErrorHandler.ShowPureErrorMessage(id + " name signifier speed modifier could not be found! This is potentially lethal!");
                return 0;
            }
            return IDToSpeedModifier[id];
        }

        public float GetBaseSpeedModifierOverrideFromID(string id)
        {
            if (!IDToBaseSpeedOverride.ContainsKey(id))
            {
                ErrorHandler.ShowPureErrorMessage(id + " name signifier base speed modifier override could not be found! This is potentially lethal!");
                return 0;
            }
            return IDToBaseSpeedOverride[id];
        }

        public bool GetPartyFollowBehaviourFlagFromID(string id)
        {
            if (!IDToFollowMainParty.ContainsKey(id))
            {
                ErrorHandler.ShowPureErrorMessage(id + "name signifier follow main party could not be found! This is potentially lethal!");
                return false;
            }
            return IDToFollowMainParty[id];
        }
    }
}
