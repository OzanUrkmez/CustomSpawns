using CustomSpawns.Dialogues.DialogueAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Barterables;
using TaleWorlds.SaveSystem;

namespace CustomSpawns.Dialogues
{
    //TODO Improve upon delegate logic. add more options. 
    public class CustomSpawnsDialogueBehavior : CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.AddCustomDialogues));
            DialogueManager.CustomSpawnsDialogueBehavior = this;
        }

        private Data.DialogueDataManager dataManager;

        Dictionary<MobileParty, string> dialoguePartyRef = new Dictionary<MobileParty, string>();

        public override void SyncData(IDataStore dataStore)
        {
            dataStore.SyncData<Dictionary<MobileParty, string>>("dialoguePartyRef", ref dialoguePartyRef);
            if(dialoguePartyRef == null)
            {
                dialoguePartyRef = new Dictionary<MobileParty, string>();
            }
        }

        // basic dialogue overview: 
        // ids are used to differentiate similar dialogues (must be unique)
        // tokens are like impulses, so an out token leads into an in token. 'magic tokens' are tokens which have special functions, they are start (in token) and close_window (out token)
        // conditions are delegates that must anonymously return a bool, which will be wheteher or not the line is displayed
        // consequences are void delegates, just pieces of code run after a line has been selected
        // i think? priority determines which line is shown if multiple lines meet the requirements,
        // and also maybe what order player lines are displayed in (speculation)
        // -ComradeCheekiBreeki

        //It seems that the first condition that is met is run, and all after it are ignored.
        //The higher the priority (higher number), the more likelihood it has of being run first.
        //However, there also seems to be an option to turn off this sorting of conditions based on priority through
        //ConversationManager.[Enable/Disable]Sort(). Sorting seems to be enabled by default in the time of writing (Bannerlord 1.5.8)
        //-Ozan

        public void AddCustomDialogues(CampaignGameStarter starter)
        {
            if (dataManager == null)
            {
                GetData();
            }

            foreach (Data.DialogueData d in dataManager.Data) // handle the dialogues
            {

                starter.AddDialogLine(d.Dialogue_ID, "start", "", d.DialogueText,
                    delegate
                    {
                        return EvalulateDialogueCondition(d.Condition);
                    },
                    null,
                    int.MaxValue
                    );
            }
        }

        private bool EvalulateDialogueCondition(DialogueCondition condition)
        {
            var param = new DialogueParams()
            {
                AdversaryParty = PlayerEncounter.EncounteredParty.MobileParty,
                PlayerParty = Hero.MainHero.PartyBelongedTo
            };

            return condition.ConditionEvaluator(param);
        }

        public void RegisterParty(MobileParty mb, string partyTemplateID)
        {
            ModDebug.ShowMessage("party of " + mb.StringId + " has registered for dialogue detection", DebugMessageType.Dialogue);
            dialoguePartyRef.Add(mb, partyTemplateID);
        }

        private void GetData() // the classic
        {
            dataManager = Data.DialogueDataManager.Instance;
        }

        // final version uses no structs, yay!!! no more lazy copouts (well, at least a reduced amount of them)
    }
}
