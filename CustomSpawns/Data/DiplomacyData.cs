using System.Collections.Generic;

namespace CustomSpawns.Data
{
    public class DiplomacyData
    {
         public string clanString;

         public class ForcedWarPeaceData
         {
             public List<string> AtPeaceWithClans = new List<string>();
             public List<string> ExceptionKingdoms = new List<string>();
         }

         public ForcedWarPeaceData ForcedWarPeaceDataInstance;

         public bool ForceNoKingdom { get; set; }
    }
}