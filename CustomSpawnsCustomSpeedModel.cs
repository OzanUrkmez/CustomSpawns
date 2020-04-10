using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;


namespace CustomSpawns
{
   
    public class CustomSpawnsCustomSpeedModel : DefaultPartySpeedCalculatingModel
    {
        private static readonly TextObject explanationText = new TextObject("Custom Spawns Modification");

        public override float CalculatePureSpeed(MobileParty mobileParty, StatExplainer explanation, int additionalTroopOnFootCount = 0, int additionalTroopOnHorseCount = 0)
        {
            return base.CalculatePureSpeed(mobileParty, explanation, additionalTroopOnFootCount, additionalTroopOnHorseCount);
        }

        public override float CalculateFinalSpeed(MobileParty mobileParty, float baseSpeed, StatExplainer explanation)
        {
            float calc = base.CalculateFinalSpeed(mobileParty, baseSpeed, explanation);
            string key = string.Join("_", Utils.Utils.TakeAllButLast<string>(mobileParty.StringId.Split('_')).ToArray<string>()); //TODO if this is non-trivial make it more efficient
            if (partyIDToExtraSpeed.ContainsKey(key))
            {
                ExplainedNumber explainedNumber = new ExplainedNumber(calc, explanation);
                float extra = partyIDToExtraSpeed[key];
                explainedNumber.AddFactor(extra, explanationText);
                return calc + extra;
            }
            return calc;
        }

        private Dictionary<string, float> partyIDToExtraSpeed = new Dictionary<string, float>();

        public void RegisterPartyExtraSpeed(string partyBaseID, float extraSpeed)
        {
            if (partyIDToExtraSpeed.ContainsKey(partyBaseID))
                return;
            partyIDToExtraSpeed.Add(partyBaseID, extraSpeed);
        }
    }
}
