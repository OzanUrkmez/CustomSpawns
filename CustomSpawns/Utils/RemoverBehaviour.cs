using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Library;


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
                removed = true;
                RemoveEverything();
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
                else
                {
                    //party will still exist but could still contain our troops or prisoners
                    List<CharacterObject> charObjectsToBeRemoved = new List<CharacterObject>();

                    foreach(var t in mb.MemberRoster.GetTroopRoster())
                    {
                        if(t.Character.StringId.StartsWith("cs_"))
                            charObjectsToBeRemoved.Add(t.Character);
                    }
                    for (int i = 0; i < charObjectsToBeRemoved.Count; i++) {
                            mb.MemberRoster.RemoveTroop(charObjectsToBeRemoved[i], mb.MemberRoster.GetTroopCount(charObjectsToBeRemoved[i]));
                    }
                    //now do it for prisoners
                    charObjectsToBeRemoved.Clear();
                    foreach (var t in mb.PrisonRoster.GetTroopRoster())
                    {
                        if (t.Character.StringId.StartsWith("cs_"))
                            charObjectsToBeRemoved.Add(t.Character);
                    }
                    for (int i = 0; i < charObjectsToBeRemoved.Count; i++)
                    {
                        mb.PrisonRoster.RemoveTroop(charObjectsToBeRemoved[i], mb.PrisonRoster.GetTroopCount(charObjectsToBeRemoved[i]));
                    }
                }
            }
            for(int i = 0; i < toBeRemoved.Count; i++)
            { 
                if(toBeRemoved[i].Party.MapEvent != null)
                {
                    UX.ShowMessage("CustomSpawns: the party " + toBeRemoved[i].StringId + " is currently engaged at a map event and thus cannot be removed until this event is completed.", Color.ConvertStringToColor("#001FFFFF"));
                    removed = false;
                }
                UX.ShowMessage("CustomSpawns: removing " + toBeRemoved[i].StringId, Color.ConvertStringToColor("#001FFFFF"));
                toBeRemoved[i].RemoveParty();
            }
            if(removed)
                UX.ShowMessage("CustomSpawns is now safe to remove from your game.", Color.ConvertStringToColor("#001FFFFF"));
        }
    }
}
