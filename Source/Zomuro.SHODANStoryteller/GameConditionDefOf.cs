using RimWorld;
using Verse;
using Verse.AI;

namespace Zomuro.SHODANStoryteller
{
    [DefOf]
    public static class GameConditionDefOf
    {
        public static GameConditionDef Zomuro_SHODAN_CyberSubversion_LightSap;

        static GameConditionDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(GameConditionDefOf));
        }
    }
}
