using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using HarmonyLib;

namespace Zomuro.SHODANStoryteller
{
    [StaticConstructorOnStartup]
    static class HarmonyPatches
    {
        static HarmonyPatches()
        {
            Harmony harmony = new Harmony("Zomuro.SHODANStoryteller");
        }

        // POSTFIX: save values in storyteller comps for certain storytellers
        /*public static void ExposeData_Post(Storyteller __instance)
        {
            if (Find.Storyteller.def == StorytellerDefOf.Zomuro_SHODAN)
            {
                StorytellerComp storage = __instance.storytellerComps.FirstOrDefault(x => x.GetType() == typeof(StorytellerComp_SHODAN_Storage));
                if (storage != null)
                {
                    (storage as StorytellerComp_SHODAN_Storage).CompExposeData();
                }
            }
        }*/
    }
}
