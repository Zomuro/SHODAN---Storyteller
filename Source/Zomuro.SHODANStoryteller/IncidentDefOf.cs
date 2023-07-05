using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace Zomuro.SHODANStoryteller
{
    [DefOf]
    public static class IncidentDefOf
    {
        public static IncidentDef Zomuro_SHODAN_ColonySubversion_Incid_Raid;

        static IncidentDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(IncidentDefOf));
        }
    }
}
