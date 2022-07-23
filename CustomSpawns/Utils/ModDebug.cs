using CustomSpawns.CampaignData.Config;
using CustomSpawns.Config;
using TaleWorlds.Core;
using TaleWorlds.Library;


namespace CustomSpawns.Utils
{
    class ModDebug
    {

        public static void ShowMessage(string message, DebugMessageType messageType)
        {
            if (!ConfigLoader.Instance.Config.IsDebugMode)
                return;
            if (messageType == DebugMessageType.AI && !ConfigLoader.Instance.Config.ShowAIDebug)
                return;
            if (messageType == DebugMessageType.DeathTrack && !ConfigLoader.Instance.Config.ShowDeathTrackDebug)
                return;
            InformationManager.DisplayMessage(new InformationMessage(message, Color.ConvertStringToColor("#FF8F00FF")));
        }

        public static void ShowMessage(string message, ICampaignDataConfig config)
        {
            if (!config.ShowConfigDebug)
            {
                return;
            }

            InformationManager.DisplayMessage(new InformationMessage(message, Color.ConvertStringToColor("#FF8F00FF")));
        }

    }

    public enum DebugMessageType { Spawn, AI, Prisoner, Diplomacy, DeathTrack, Dialogue, Development }
}
