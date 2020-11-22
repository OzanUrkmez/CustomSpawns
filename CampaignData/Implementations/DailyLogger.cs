using CustomSpawns.Data;
using CustomSpawns.UtilityBehaviours;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace CustomSpawns.CampaignData { 
    class DailyLogger : CustomCampaignDataBehaviour<DailyLogger, DailyLoggerConfig>
    {

        private string logDir;

        private FileStream logStream;

        public override void FlushSavedData()
        {

        }

        protected override void OnSaveStart()
        {
            if (!Directory.Exists(logDir))
            {
                Directory.CreateDirectory(logDir);
            }
            if (logStream != null && logStream.CanWrite)
                logStream.Close();
            logStream = File.Create(Path.Combine(logDir, "RudimentaryLastSessionLog.txt"));
        }

        protected override void SyncSaveData(IDataStore dataStore)
        {

        }

        protected override void OnRegisterEvents()
        {
            CampaignEvents.AfterDailyTickEvent.AddNonSerializedListener(this, OnAfterDailyTick);
            CampaignEvents.WarDeclared.AddNonSerializedListener(this, OnWarDeclared);
            CampaignEvents.MakePeace.AddNonSerializedListener(this, OnPeaceMade);
            CampaignEvents.OnSettlementOwnerChangedEvent.AddNonSerializedListener(this, OnSettlementChange);
        }



        public DailyLogger()
        {
            logDir = Path.Combine(BasePath.Name, "Modules", "CustomSpawns", "Logs");
        }

        private bool DataWrittenToday = false;
        private int dayCount;
        private void OnAfterDailyTick()
        {
            dayCount = (int)Campaign.Current.CampaignStartTime.ElapsedDaysUntilNow;

            DataWrittenToday = false;
        }

        private void WriteString(string s)
        {
            if (logStream != null && logStream.CanWrite)
            {
                if (!DataWrittenToday)
                {
                    DataWrittenToday = true;
                    WriteString("Day " + dayCount.ToString() + ":\n");
                }
                logStream.Write(Encoding.UTF8.GetBytes(s), 0, s.Length);
            }
            else
                ModDebug.ShowMessage("Unable to write daily log!", DebugMessageType.Development);
        }

        private void OnWarDeclared(IFaction fac1, IFaction fac2)
        {
            WriteString(fac1.Name + " and " + fac2.Name + " have declared war!\n");
        }

        private void OnPeaceMade(IFaction fac1, IFaction fac2)
        {
            WriteString(fac1.Name + " and " + fac2.Name + " have made peace!\n");
        }

        private void OnSettlementChange(Settlement s, bool b, Hero newOwner, Hero oldOwner, Hero h3, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail details)
        {
            if(details == ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail.BySiege)
            {
                WriteString(s.Name + " has been captured succesfully through siege, changing hands from " + oldOwner.Clan.Kingdom.Name + " to " + newOwner.Clan.Kingdom.Name + "\n");
            }
        }

        public static void ReportSpawn(MobileParty spawned, float chanceOfSpawnBeforeSpawn)
        {
            if (Singleton == null || spawned.Party.TotalStrength < Singleton.campaignConfig.MinimumSpawnLogValue || chanceOfSpawnBeforeSpawn > Singleton.campaignConfig.MinimumRarityToLog)
                return;

            Singleton.WriteString("New Spawn: " + spawned.StringId + "\n    Total Strength:" + spawned.Party.TotalStrength.ToString() + "\nChance of Spawn: " + chanceOfSpawnBeforeSpawn.ToString() +  "\n");
        }

        
    }
}
