using RimWorld;

namespace Zomuro.SHODANStoryteller
{
    [DefOf]
    public static class FactionDefOf
    {
        public static FactionDef Zomuro_SHODAN_Faction;

        static FactionDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(FactionDefOf));
        }
    }
}
