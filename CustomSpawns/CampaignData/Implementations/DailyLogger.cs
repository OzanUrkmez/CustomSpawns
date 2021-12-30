using CustomSpawns.Data;
using System;
using System.IO;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Library;

namespace CustomSpawns.CampaignData { 
    class DailyLogger : CustomCampaignDataBehaviour<DailyLogger, DailyLoggerConfig>
    {

        private string _logDir;

        private string _filename;

        public override void FlushSavedData()
        {

        }

        protected override void OnSaveStart()
        {
            if (!Directory.Exists(_logDir))
            {
                Directory.CreateDirectory(_logDir);
            }

            var filepath = _logDir + "\\" + _filename;
            if(!File.Exists(filepath))
                File.Create(filepath);   
        }

        protected override void SyncSaveData(IDataStore dataStore)
        {

        }

        protected override void OnRegisterEvents()
        {
            CampaignEvents.AfterDailyTickEvent.AddNonSerializedListener(this, OnAfterDailyTick);
            CampaignEvents.WarDeclared.AddNonSerializedListener(this, OnWarDeclared);
            CampaignEvents.MakePeace.AddNonSerializedListener(this, OnPeaceMade);
            CampaignEvents.ClanChangedKingdom.AddNonSerializedListener(this, ClanChangedKingdom);
            CampaignEvents.OnSettlementOwnerChangedEvent.AddNonSerializedListener(this, OnSettlementChange);
        }

        public DailyLogger()
        {
            _filename  = "RudimentaryLastSessionLog_" + DateTime.Now.ToString("yyyy-MM-dd_h-mm_tt") + ".txt";
            _logDir = Path.Combine(BasePath.Name, "Modules", "CustomSpawns", "Logs");
        }

        private bool DataWrittenToday = false;
        private int dayCount;
        private void OnAfterDailyTick()
        {
            dayCount = (int)Campaign.Current.CampaignStartTime.ElapsedDaysUntilNow;
        }

        public void Info(String s)
        {
            WriteString(s);
        }
        
        private void WriteString(string s)
        {
            try
            {
                var a = DateTime.Now.ToString("yyyy-MM-dd h:mm tt");
                using (StreamWriter w = File.AppendText(_logDir + "\\" + _filename))
                {
                    w.WriteLine("[{0} {1}][Campaign Day {2}] {3}", DateTime.Now.ToLongTimeString(),
                        a, dayCount, s);
                }
            }
            catch (Exception ex)
            {
                throw new TechnicalException("Could not write into the log file", ex);
            }
        }

        private void OnWarDeclared(IFaction fac1, IFaction fac2)
        {
            WriteString(fac1.Name + " and " + fac2.Name + " have declared war!\n");
        }
        
        private void ClanChangedKingdom(Clan c, Kingdom k1, Kingdom k2, ChangeKingdomAction.ChangeKingdomActionDetail details, Boolean b)
        {
            if (k1 != null && k2 != null)
            {
                WriteString("Clan " + c.Name + " has left " + k1.Name + " to join " + k2.Name + ". (reason: " + details + ")");   
            } else if (k1 != null && k2 == null)
            {
                WriteString("Clan " + c.Name + " left " + k1.Name + ". (reason: " + details + ")");   
            } else if (k1 == null && k2 != null)
            {
                WriteString("Clan " + c.Name + " joined " + k2.Name + ". (reason: " + details + ")");
            }
        }

        private void OnPeaceMade(IFaction fac1, IFaction fac2)
        {
            WriteString(fac1.Name + " and " + fac2.Name + " have made peace!\n");
        }

        private void OnSettlementChange(Settlement s, bool b, Hero newOwner, Hero oldOwner, Hero h3, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail details)
        {


            if(details == ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail.BySiege)
            {
                if (s == null || oldOwner == null || newOwner == null || oldOwner.Clan == null || newOwner.Clan == null || oldOwner.Clan.Kingdom == null || newOwner.Clan.Kingdom == null) //absolutely disgusting.
                    WriteString("There has been a siege. \n");
                else
                    WriteString(s.Name + " has been captured successfully through siege, changing hands from " + oldOwner.Clan.Kingdom.Name + " to " + newOwner.Clan.Kingdom.Name + "\n");
            }
        }

        public static void ReportSpawn(MobileParty spawned, float chanceOfSpawnBeforeSpawn)
        {
            if (Singleton == null || spawned.Party.TotalStrength < Singleton.campaignConfig.MinimumSpawnLogValue || chanceOfSpawnBeforeSpawn > Singleton.campaignConfig.MinimumRarityToLog)
                return;

            string msg = "New Spawn: " + spawned.StringId +
                "\nTotal Strength:" + spawned.Party.TotalStrength.ToString() +
                "\nChance of Spawn: " + chanceOfSpawnBeforeSpawn.ToString();

            var spawnData = DynamicSpawnData.Instance.GetDynamicSpawnData(spawned).spawnBaseData;

            if (spawnData.DynamicSpawnChanceEffect > 0)
            {
                msg += "\nDynamic Spawn Chance Effect: " + spawnData.DynamicSpawnChanceEffect;
                msg += "\nDynamic Spawn Chance Base Value During Spawn: " + DataUtils.GetCurrentDynamicSpawnCoeff(spawnData.DynamicSpawnChancePeriod);
            }

            var spawnSettlement = DynamicSpawnData.Instance.GetDynamicSpawnData(spawned).latestClosestSettlement;

            if (spawnSettlement.IsVillage)
            {
                msg += "\nDevestation at spawn settlement: " +
                    DevestationMetricData.Singleton.GetDevestation(spawnSettlement);
            }


            msg += "\n";

            Singleton.WriteString(msg);
        }

        
    }
}
