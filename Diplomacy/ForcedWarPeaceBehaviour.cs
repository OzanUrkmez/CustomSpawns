using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;
using TaleWorlds.Localization;

namespace CustomSpawns.Diplomacy
{
    class ForcedWarPeaceBehaviour : CampaignBehaviorBase
    {
        public ForcedWarPeaceBehaviour()
        {

        }

        private Data.DiplomacyDataManager dataManager;

        public override void RegisterEvents()
        {
            CampaignEvents.DailyTickClanEvent.AddNonSerializedListener(this, DailyClanBehaviour);
        }

        public override void SyncData(IDataStore dataStore)
        {

        }

        private void DailyClanBehaviour(Clan c)
        {
            try
            {
                if (dataManager == null)
                {
                    GetData();
                }
                if (dataManager.Data.ContainsKey(c.StringId) && dataManager.Data[c.StringId].ForcedWarPeaceDataInstance != null)
                {
                    var forcedWarPeaceInstance = dataManager.Data[c.StringId].ForcedWarPeaceDataInstance;
                    foreach (Clan declared in forcedWarPeaceInstance.atWarClans)
                    {
                        if (declared == null)
                            continue;
                        if (declared.Kingdom == null)
                            FactionManager.DeclareWar(c, declared);
                        else
                            FactionManager.DeclareWar(c, declared.Kingdom);
                    }
                }
            }catch(Exception e)
            {
                ErrorHandler.HandleException(e, " daily clan behaviour processing of ForcedWarPeaceBehaviour.cs ");
            }
        }

        private void GetData()
        {
            dataManager = Data.DiplomacyDataManager.Instance;
        }
    }
}
