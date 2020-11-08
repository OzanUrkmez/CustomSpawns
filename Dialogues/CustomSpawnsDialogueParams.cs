using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomSpawns.Dialogues
{
    public class CustomSpawnsDialogueParams
    {
        public CustomSpawnsDialogueParams(string partyTemplate, bool friendly, bool isPlayerSurrender, bool isPlayerTrait, string traitToCheck, int value, bool successfulBarter, string lordName, string faction)
        {
            this.c_partyTemplate = partyTemplate;
            this.c_isFriendly = friendly;
            this.c_isPlayerTrait = isPlayerTrait;
            this.c_traitToCheck = traitToCheck; // a lot of reused stuff here, probably should put everything of the same type as a single generic property (since there can only be 1 dialogue line associated with a parameter instance)
            this.c_value = value;
            this.c_lordName = lordName;
            this.c_faction = faction;
            this.c_barterSuccessful = successfulBarter;

            this.cs_isPlayerSurrender = isPlayerSurrender;
        }

        // condition params
        public string c_partyTemplate;
        public bool c_isFriendly;
        public bool c_isPlayerTrait;
        public string c_traitToCheck;
        public int c_value;
        public bool c_barterSuccessful;
        public string c_lordName;
        public string c_faction;
        // consequence params
        public bool cs_isPlayerSurrender;
    }
}
