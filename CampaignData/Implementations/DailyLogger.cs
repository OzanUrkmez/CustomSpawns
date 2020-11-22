using CustomSpawns.UtilityBehaviours;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
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

        }

        protected override void SyncSaveData(IDataStore dataStore)
        {

        }

        protected override void OnRegisterEvents()
        {
            CampaignEvents.AfterDailyTickEvent.AddNonSerializedListener(this, OnAfterDailyTick);
            CampaignEvents.WarDeclared.AddNonSerializedListener(this, OnWarDeclared);
            CampaignEvents.MakePeace.AddNonSerializedListener(this, OnPeaceMade);
        }



        public DailyLogger()
        {
            logDir = Path.Combine(BasePath.Name, "Modules", "CustomSpawns", "Logs");
        }



        public void OnGameStart()
        {

            if (!Directory.Exists(logDir))
            {
                Directory.CreateDirectory(logDir);
            }
            logStream = File.Create(Path.Combine(logDir, "RudimentaryLastSessionLog.txt"));
        }


        private void OnAfterDailyTick()
        {
            int dayCount = (int)Campaign.Current.CampaignStartTime.ElapsedDaysUntilNow;

            WriteString("Day " + dayCount.ToString() + ":\n");
        }

        private void WriteString(string s)
        {
            if (logStream != null && logStream.CanWrite)
                logStream.Write(Encoding.UTF8.GetBytes(s), 0, s.Length);
            else
                ModDebug.ShowMessage("Unable to write daily log!", DebugMessageType.Development);
        }

        private void OnWarDeclared(IFaction fac1, IFaction fac2)
        {
            WriteString(fac1.Name + " and " + fac2.Name + " have declared war!");
        }

        private void OnPeaceMade(IFaction fac1, IFaction fac2)
        {
            WriteString(fac1.Name + " and " + fac2.Name + " have made peace!");
        }


        public static void ReportSpawn(MobileParty spawned)
        {
            if (Singleton == null || spawned.Party.TotalStrength < Singleton.campaignConfig.MinimumSpawnLogValue)
                return;

            Singleton.WriteString("New Spawn: " + spawned.StringId + "\n    Total Strength:" + spawned.Party.TotalStrength.ToString() + "!\n");
        }

        
    }
}
