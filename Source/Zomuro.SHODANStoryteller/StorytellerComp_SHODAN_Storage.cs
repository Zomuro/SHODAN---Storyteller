using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace Zomuro.SHODANStoryteller
{
    public class StorytellerComp_SHODAN_Storage : StorytellerComp
    {
        public void CompExposeData()
        {
            Scribe_Collections.Look(ref originalFaction, "originalFaction", LookMode.Reference, LookMode.Def);
        }

        public Dictionary<Pawn, FactionDef> originalFaction = new Dictionary<Pawn, FactionDef>();

    }
}
