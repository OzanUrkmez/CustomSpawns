using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;
using TaleWorlds.Library;
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
            List<MobileParty> toBeRemoved = new List<MobileParty>();
            foreach (MobileParty mb in MobileParty.All)
            {
                if (mb.StringId.StartsWith("cs"))
                {
                    //one of our parties.
                    toBeRemoved.Add(mb);
                }
            }
            for(int i = 0; i < toBeRemoved.Count; i++)
            {
                UX.ShowMessage("CustomSpawns: removing " + toBeRemoved[i].StringId, Color.ConvertStringToColor("#001FFFFF"));
                toBeRemoved[i].RemoveParty();
            }
            UX.ShowMessage("CustomSpawns is now safe to remove from your game.", Color.ConvertStringToColor("#001FFFFF"));
        }
    }
}
