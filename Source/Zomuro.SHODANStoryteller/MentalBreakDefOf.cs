using RimWorld;
using Verse;
using Verse.AI;

namespace Zomuro.SHODANStoryteller
{
    [DefOf]
    public static class MentalBreakDefOf
    {
        public static MentalBreakDef Zomuro_SHODAN_CyberneticDomination_Break;

        static MentalBreakDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(MentalBreakDefOf));
        }
    }
}
