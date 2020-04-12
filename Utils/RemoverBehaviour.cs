using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;
using TaleWorlds.Localization;


namespace CustomSpawns.Utils
{
    public class RemoverBehaviour : CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {
            CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, RemoverBehaviourFunc);
        }

        public override void SyncData(IDataStore dataStore)
        {
        }

        private bool removed = false;

        private void RemoverBehaviourFunc()
        {
            if (!removed)
            {
                RemoveEverything();
                removed = true;
            }
        }

        private void RemoveEverything()
        {
            foreach (MobileParty mb in MobileParty.All)
            {
                if (mb.StringId.StartsWith("cs"))
                {
                    //one of our parties.
                    UX.ShowMessage("CustomSpawns: removing " + mb.StringId, Color.ConvertStringToColor("#001FFFFF"));
                    mb.RemoveParty();
                }
            }
            UX.ShowMessage("CustomSpawns is now safe to remove from your game.", Color.ConvertStringToColor("#001FFFFF"));
        }
    }
}
