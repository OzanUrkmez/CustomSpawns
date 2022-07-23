using System.Collections.Generic;

namespace CustomSpawns.Utils
{
    public static class Utils
    {
        public static bool IsCustomSpawnsStringID(string stringID)
        {
            return (stringID.StartsWith("cs_"));
        }

        public static int GetTotalPrisonerCounts(List<PrisonerInfo> prisonerInfos)
        {
            int returned = 0;
            foreach(var p in prisonerInfos)
            {
                returned += p.count;
            }
            return returned;
        }

    }
}
