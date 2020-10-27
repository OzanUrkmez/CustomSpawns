using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomSpawns.Dialogues
{
    public class CustomSpawnsDialogueParams // this is super inefficient RIGHT NOW!! in the future it will (hopefully) have a lot more properties and be better than a struct
    {
        public CustomSpawnsDialogueParams(string partyTemplate)
        {
            this.c_partyTemplate = partyTemplate;
        }

        // condition params
        public string c_partyTemplate;
    }
}
