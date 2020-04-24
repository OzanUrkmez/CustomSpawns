using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;
using Helpers;

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
            PartyBase party = mobileParty.Party;
            ExplainedNumber explainedNumber = new ExplainedNumber(baseSpeed, explanation, null);
            explainedNumber.LimitMin(1f);
            string key = CampaignUtils.IsolateMobilePartyStringID(mobileParty); //TODO if this is non-trivial make it more efficient
            if (partyIDToBaseSpeed.ContainsKey(key))
            {
                float bs = partyIDToBaseSpeed[key];
                explainedNumber.Add(bs - explainedNumber.ResultNumber, explanationText);
            }
            else if (partyIDToExtraSpeed.ContainsKey(key))
            {
                float extra = partyIDToExtraSpeed[key];
                explainedNumber.Add(extra, explanationText);
            }
            TerrainType faceTerrainType = Campaign.Current.MapSceneWrapper.GetFaceTerrainType(mobileParty.CurrentNavigationFace);
            float num = explainedNumber.ResultNumber;
            if (partyIDToMinimumSpeed.ContainsKey(key))//minimum adjustment
                 num = Math.Max(num, partyIDToMinimumSpeed[key]);
            else
                explainedNumber.LimitMin(1f);

            if (partyIDToMaximumSpeed.ContainsKey(key))//maximum adjustment
                 num = Math.Min(num, partyIDToMaximumSpeed[key]);

            explainedNumber.Add(num - explainedNumber.ResultNumber, new TextObject("Custom Spawns final modification"));
            if (faceTerrainType == TerrainType.Forest)
            {
                explainedNumber.AddFactor(-0.3f, _movingInForest);
                PerkHelper.AddFeatBonusForPerson(DefaultFeats.Cultural.BattanianForestAgility, mobileParty.Leader, ref explainedNumber);
            }
            else if (faceTerrainType == TerrainType.Water || faceTerrainType == TerrainType.River || faceTerrainType == TerrainType.Bridge || faceTerrainType == TerrainType.ShallowRiver)
            {
                explainedNumber.AddFactor(-0.3f, _fordEffect);
            }
            if (Campaign.Current.IsNight)
            {
                explainedNumber.AddFactor(-0.25f, _night);
            }
            if (faceTerrainType == TerrainType.Snow)
            {
                explainedNumber.AddFactor(-0.1f, _snow);
                if (party.Leader != null)
                {
                    PerkHelper.AddFeatBonusForPerson(DefaultFeats.Cultural.SturgianSnowAgility, party.Leader, ref explainedNumber);
                }
            }


            return explainedNumber.ResultNumber;
        }

        private Dictionary<string, float> partyIDToExtraSpeed = new Dictionary<string, float>();
        private Dictionary<string, float> partyIDToBaseSpeed = new Dictionary<string, float>();
        private Dictionary<string, float> partyIDToMinimumSpeed = new Dictionary<string, float>();
        private Dictionary<string, float> partyIDToMaximumSpeed = new Dictionary<string, float>();

        public void RegisterPartyExtraSpeed(string partyBaseID, float extraSpeed)
        {
            if (partyIDToExtraSpeed.ContainsKey(partyBaseID) || !ConfigLoader.Instance.Config.ModifyPartySpeeds)
                return;
            partyIDToExtraSpeed.Add(partyBaseID, extraSpeed);
        }

        public void RegisterPartyMinimumSpeed(string partyBaseID, float minimumSpeed)
        {
            if (partyIDToMinimumSpeed.ContainsKey(partyBaseID) || !ConfigLoader.Instance.Config.ModifyPartySpeeds)
                return;
            partyIDToMinimumSpeed.Add(partyBaseID, minimumSpeed);
        }

        public void RegisterPartyMaximumSpeed(string partyBaseID, float maximumSpeed)
        {
            if (partyIDToMaximumSpeed.ContainsKey(partyBaseID) || !ConfigLoader.Instance.Config.ModifyPartySpeeds)
                return;
            partyIDToMaximumSpeed.Add(partyBaseID, maximumSpeed);
        }

        public void RegisterPartyBaseSpeed(string partyBaseID, float maximumSpeed)
        {
            if (partyIDToBaseSpeed.ContainsKey(partyBaseID) || !ConfigLoader.Instance.Config.ModifyPartySpeeds)
                return;
            partyIDToBaseSpeed.Add(partyBaseID, maximumSpeed);
        }

        //ALL THIS TAKEN FROM TALEWORLDS GAME FILES:
        private static readonly TextObject _textCargo = new TextObject("{=fSGY71wd}Cargo within capacity", null);

        private static readonly TextObject _textOverburdened = new TextObject("{=xgO3cCgR}Overburdened", null);

        private static readonly TextObject _textOverPartySize = new TextObject("{=bO5gL3FI}Men within party size", null);

        private static readonly TextObject _textOverPrisonerSize = new TextObject("{=Ix8YjLPD}Men within prisoner size", null);

        private static readonly TextObject _textCavalry = new TextObject("{=YVGtcLHF}Cavalry", null);

        private static readonly TextObject _textKhuzaitCavalryBonus = new TextObject("{=yi07dBks}Khuzait Cavalry Bonus", null);

        private static readonly TextObject _textMountedFootmen = new TextObject("{=5bSWSaPl}Footmen on horses", null);

        private static readonly TextObject _textWounded = new TextObject("{=aLsVKIRy}Wounded Members", null);

        private static readonly TextObject _textPrisoners = new TextObject("{=N6QTvjMf}Prisoners", null);

        private static readonly TextObject _textHerd = new TextObject("{=NhAMSaWU}Herd", null);

        private static readonly TextObject _difficulty = new TextObject("{=uG2Alcat}Game Difficulty", null);

        private static readonly TextObject _textHighMorale = new TextObject("{=aDQcIGfH}High Morale", null);

        private static readonly TextObject _textLowMorale = new TextObject("{=ydspCDIy}Low Morale", null);

        private static readonly TextObject _textDisorganized = new TextObject("{=JuwBb2Yg}Disorganized", null);

        private static readonly TextObject _movingInForest = new TextObject("{=rTFaZCdY}Forest", null);

        private static readonly TextObject _fordEffect = new TextObject("{=NT5fwUuJ}Fording", null);

        private static readonly TextObject _night = new TextObject("{=fAxjyMt5}Night", null);

        private static readonly TextObject _snow = new TextObject("{=vLjgcdgB}Snow", null);

        private static readonly TextObject _desert = new TextObject("{=ecUwABe2}Desert", null);

        private static readonly TextObject _sturgiaSnowBonus = new TextObject("{=0VfEGekD}Sturgia Snow Bonus", null);
    }
}
