using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomSpawns.Dialogues
{
    public class CustomSpawnsDialogueParams // this is super inefficient RIGHT NOW!! in the future it will (hopefully) have a lot more properties and be better than a struct (not as many floating around members in a class)
    {
        public CustomSpawnsDialogueParams(string partyTemplate, bool friendly, bool isPlayerSurrender)
        {
            this.c_partyTemplate = partyTemplate;
            this.c_isFriendly = friendly;

            this.cs_isPlayerSurrender = isPlayerSurrender;
        }

        // condition params
        public string c_partyTemplate;
        public bool c_isFriendly;
        // consequence params
        public bool cs_isPlayerSurrender;
    }
}
