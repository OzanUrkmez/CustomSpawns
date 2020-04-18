using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Barterables;
using TaleWorlds.CampaignSystem.CharacterDevelopment.Managers;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;

namespace CustomSpawns.Spawn
{
    class Spawner
    {

        public static MobileParty SpawnBanditAtHideout(Settlement spawnedSettlement, Clan clan, PartyTemplateObject templateObject,MobileParty.PartyTypeEnum partyType,  TextObject partyName = null)
        {
            //get name and show message.
            TextObject textObject = partyName ?? clan.Name;
            ModDebug.ShowMessage("CustomSpawns: Spawning " + textObject.ToString() + " at " + spawnedSettlement.GatePosition + " in settlement " + spawnedSettlement.Name.ToString());

            //create.
            int numberOfCreated = templateObject.NumberOfCreated;
            templateObject.IncrementNumberOfCreated();
            MobileParty mobileParty = MBObjectManager.Instance.CreateObject<MobileParty>(templateObject.StringId + "_" + numberOfCreated.ToString());
            mobileParty.InitializeMobileParty(textObject, templateObject, spawnedSettlement.GatePosition, 0, 0, partyType, -1);

            //initialize as bandit party
            Spawner.InitParty(mobileParty, textObject, clan, spawnedSettlement);

            //fill reminiscent
            CampaignUtils.FillReminiscentBanditParties(mobileParty, templateObject, MobileParty.PartyTypeEnum.Bandit);

            return mobileParty;
        }

        public static void InitParty(MobileParty banditParty, TextObject name, Clan faction, Settlement homeSettlement)
        {
            banditParty.Name = name;
            if (faction.Leader == null)
            {
                banditParty.Party.Owner = faction.Heroes.First();
            }
            else
            {
                banditParty.Party.Owner = faction.Leader;
            }
            banditParty.Party.Visuals.SetMapIconAsDirty();
            if (faction.Leader.HomeSettlement == null)
            {
                faction.UpdateHomeSettlement(homeSettlement);
            }
            banditParty.HomeSettlement = homeSettlement;
            TaleWorldsCode.BanditsCampaignBehaviour.CreatePartyTrade(banditParty);
            foreach (ItemObject itemObject in ItemObject.All)
            {
                if (itemObject.IsFood)
                {
                    int num = TaleWorldsCode.BanditsCampaignBehaviour.IsLooterFaction(banditParty.MapFaction) ? 8 : 16;
                    int num2 = MBRandom.RoundRandomized((float)banditParty.MemberRoster.TotalManCount * (1f / (float)itemObject.Value) * (float)num * MBRandom.RandomFloat * MBRandom.RandomFloat * MBRandom.RandomFloat * MBRandom.RandomFloat);
                    if (num2 > 0)
                    {
                        banditParty.ItemRoster.AddToCounts(itemObject, num2, true);
                    }
                }
            }
        }

    }
}
