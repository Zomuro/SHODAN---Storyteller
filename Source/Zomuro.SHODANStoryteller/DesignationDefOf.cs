using RimWorld;
using Verse;

namespace Zomuro.SHODANStoryteller
{
    [DefOf]
    public static class DesignationDefOf
    {
        public static DesignationDef Zomuro_SHODAN_Designation_ResetFlick;

        static DesignationDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(DesignationDefOf));
        }
    }
}
