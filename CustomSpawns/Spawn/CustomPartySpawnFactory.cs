using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Party.PartyComponents;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Localization;

namespace CustomSpawns.Spawn
{
    public class CustomPartySpawnFactory : MobilePartySpawnFactory
    {
        internal override MobileParty CreateParty(Settlement spawnedSettlement, Clan clan, PartyTemplateObject templateObject, TextObject partyName,
            float speed = 0)
        {

            PartyComponent.OnPartyComponentCreatedDelegate initParty = party =>
            {
                if (clan.Leader != null)
                {
                    party.Party.SetCustomOwner(clan.Leader);
                }
                else if (clan.Heroes.Count > 0)
                {
                    party.Party.SetCustomOwner(clan.Heroes.First());
                }

                if (clan.Leader?.HomeSettlement == null)
                {
                    clan.UpdateHomeSettlement(spawnedSettlement);
                }
                party.Party.Visuals.SetMapIconAsDirty();
                party.SetCustomName(partyName);
                party.ActualClan = clan;
                party.SetCustomHomeSettlement(spawnedSettlement);
                ((CustomPartyComponent) party.PartyComponent).CustomPartyBaseSpeed = speed;
                ((CustomPartyComponent) party.PartyComponent).AvoidHostileActions = false;
            };
            
            return MobileParty.CreateParty(templateObject.StringId + "_" + 1, new CustomPartyComponent(), initParty);
        }

        // private static MethodInfo InitializeQuestPartyPropertiesMethodInfo => typeof(CustomPartyComponent)
        //     .GetMethod("InitializeQuestPartyProperties", NonPublic)!;
        //
        // public MobileParty SpawnParty(Settlement homeSettlement, TextObject partyName, Clan clan, PartyTemplateObject partyTemplate,
        //     float partySpeed)
        // {
        //     if (InitializeQuestPartyPropertiesMethodInfo == null)
        //     {
        //         return MobileParty.CreateParty(partyTemplate.StringId + "_" + 1, null);
        //     }
        //
        //     return MobileParty.CreateParty(partyTemplate.StringId + "_" + 1, new CustomPartyComponent(), delegate(MobileParty mobileParty)
        //     {
        //         Hero leader = null!;
        //         if (clan.Leader != null)
        //         {
        //             leader = clan.Leader;
        //         } else if (clan.Heroes.Count > 0)
        //         {
        //             leader = clan.Heroes.First();
        //         }
        //         else
        //         {
        //             // log warn no heroes ?
        //         }
        //
        //         if (leader != null && leader.HomeSettlement == null)
        //         {
        //             clan.UpdateHomeSettlement(homeSettlement);
        //         }
        //
        //         
        //         mobileParty.InitializeMobilePartyAroundPosition(ConstructTroopRoster(templateObject, mobileParty.Party),
        //             new TroopRoster(mobileParty.Party), spawnedSettlement.GatePosition, 0);
        //         
        //         
        //         
        //         InitializeQuestPartyPropertiesMethodInfo.Invoke(new CustomPartyComponent(), new object[]
        //         {
        //             mobileParty,
        //             homeSettlement.GatePosition,
        //             0,
        //             homeSettlement,
        //             partyName,
        //             partyTemplate,
        //             leader!,
        //             "",
        //             "",
        //             partySpeed,
        //             false
        //         });
        //         mobileParty.ActualClan = clan;
        //     });
        // }
    }
}