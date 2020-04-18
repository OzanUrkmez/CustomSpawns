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
using Helpers;

namespace CustomSpawns.Spawn
{
    class Spawner
    {

        public static MobileParty SpawnParty(Settlement spawnedSettlement, Clan clan, PartyTemplateObject templateObject,MobileParty.PartyTypeEnum partyType,  TextObject partyName = null)
        {
            //get name and show message.
            TextObject textObject = partyName ?? clan.Name;
            ModDebug.ShowMessage("CustomSpawns: Spawning " + textObject.ToString() + " at " + spawnedSettlement.GatePosition + " in settlement " + spawnedSettlement.Name.ToString());

            //create.
            int numberOfCreated = templateObject.NumberOfCreated;
            templateObject.IncrementNumberOfCreated();
            MobileParty mobileParty = MBObjectManager.Instance.CreateObject<MobileParty>(templateObject.StringId + "_" + numberOfCreated.ToString());
            mobileParty.InitializeMobileParty(textObject, ConstructTroopRoster(templateObject), new TroopRoster(), spawnedSettlement.GatePosition, 0);

            //initialize
            Spawner.InitParty(mobileParty, textObject, clan, spawnedSettlement);

            return mobileParty;
        }

        private static void InitParty(MobileParty banditParty, TextObject name, Clan faction, Settlement homeSettlement)
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

        private static TroopRoster ConstructTroopRoster(PartyTemplateObject pt, int troopNumberLimit = -1) //TODO implement troop number limit.
        {
            TroopRoster returned = new TroopRoster();
            float gameProcess = MiscHelper.GetGameProcess();
            float num = 0.25f + 0.75f * gameProcess;
            int num2 = MBRandom.RandomInt(2);
            float num3 = (num2 == 0) ? MBRandom.RandomFloat : (MBRandom.RandomFloat * MBRandom.RandomFloat * MBRandom.RandomFloat * 4f);
            float num4 = (num2 == 0) ? (num3 * 0.8f + 0.2f) : (1f + num3);
            float randomFloat = MBRandom.RandomFloat;
            float randomFloat2 = MBRandom.RandomFloat;
            float randomFloat3 = MBRandom.RandomFloat;
            for (int i = 0; i < pt.Stacks.Count; i++)
            {
                float f = (pt.Stacks.Count > 0) ? ((float)pt.Stacks[i].MinValue + num * num4 * randomFloat * (float)(pt.Stacks[i].MaxValue - pt.Stacks[i].MinValue)) : 0f;
                returned.AddToCounts(pt.Stacks[i].Character, MBRandom.RoundRandomized(f), false);
            }
            return returned;
        }

    }
}
