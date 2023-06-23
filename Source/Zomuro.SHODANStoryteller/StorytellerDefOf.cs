using RimWorld;

namespace Zomuro.SHODANStoryteller
{
    [DefOf]
    public static class StorytellerDefOf
    {
        public static StorytellerDef Zomuro_SHODAN;

        static StorytellerDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(StorytellerDefOf));
        }
    }
}
