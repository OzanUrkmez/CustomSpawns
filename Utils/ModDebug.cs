using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.Library;


namespace CustomSpawns
{
    class ModDebug
    {

        public static void ShowMessage(string message, DebugMessageType messageType)
        {
            if (!ConfigLoader.Instance.Config.IsDebugMode)
                return;
            if (messageType == DebugMessageType.AI && !ConfigLoader.Instance.Config.ShowAIDebug)
                return;
            if (messageType == DebugMessageType.Prisoner && !PrisonerRecruitment.PrisonerRecruitmentConfigLoader.Instance.Config.PrisonRecruitmentDebugEnabled)
                return;
            InformationManager.DisplayMessage(new InformationMessage(message, Color.ConvertStringToColor("#FF8F00FF")));
        }

    }

    public enum DebugMessageType { Spawn, AI, Prisoner, Diplomacy}
}
