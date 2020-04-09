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

        public static MobileParty SpawnBanditAtHideout(Settlement spawnedSettlement, Clan clan, PartyTemplateObject templateObject, TextObject partyName = null)
        {
            //get name and show message.
            TextObject textObject = partyName ?? clan.Name;
            ModDebug.ShowMessage("CustomSpawns: Spawning " + textObject.ToString() + " at " + spawnedSettlement.GatePosition + " in settlement " + spawnedSettlement.Name.ToString());

            //create.
            int numberOfCreated = templateObject.NumberOfCreated;
            templateObject.IncrementNumberOfCreated();
            MobileParty mobileParty = MBObjectManager.Instance.CreateObject<MobileParty>(clan.StringId + "_" + numberOfCreated.ToString());
            mobileParty.InitializeMobileParty(textObject, templateObject, spawnedSettlement.GatePosition, 0, 0, MobileParty.PartyTypeEnum.Bandit, -1);

            //initialize as bandit party
            TaleWorldsCode.BanditsCampaignBehaviour.InitBanditParty(mobileParty, textObject, clan, spawnedSettlement);

            return mobileParty;
        }

    }
}
