using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using Verse.AI;
using UnityEngine;
using HarmonyLib;

namespace Zomuro.SHODANStoryteller
{
    [StaticConstructorOnStartup]
    static class HarmonyPatches
    {
        static HarmonyPatches()
        {
            Harmony harmony = new Harmony("Zomuro.SHODANStoryteller");

            // CurrentPossibleMoodBreaks_Prefix: if a pawn has a mental break, have chance to force SHODAN's incident
            harmony.Patch(AccessTools.Method(typeof(MentalBreaker), "get_CurrentPossibleMoodBreaks"),
                new HarmonyMethod(typeof(HarmonyPatches), nameof(CurrentPossibleMoodBreaks_Prefix)));
        }

        // test
        public static bool CurrentPossibleMoodBreaks_Prefix(MentalBreaker __instance, ref IEnumerable<MentalBreakDef> __result)
        {
            CyberneticDominationBreakWorker.commonalityCached = 0;
            if (Find.Storyteller.def == StorytellerDefOf.Zomuro_SHODAN)
            {
                Traverse traverse = Traverse.Create(__instance);
                Log.Message("Calc Chance: " + StorytellerUtility.CyberneticDominationChance(traverse.Field("pawn").GetValue<Pawn>()));

                //bool intensityCheck = (intensity != MentalBreakDefOf.Zomuro_SHODAN_CyberneticDomination_Break.intensity);
                // add setting here to ensure SHODAN's incident can occur on various break intensities
                // or just on major or extreme for the moment (make sure to add an addition condition for when Cognition Ascension is activated
                /*Traverse traverse = Traverse.Create(__instance);
                MentalBreakIntensity intensity = traverse.Method("get_CurrentPossibleMoodBreaks").GetValue<MentalBreakIntensity>();
                bool cognitionTest = true;
                bool intensityCheck = intensity == MentalBreakIntensity.Extreme || intensity == MentalBreakIntensity.Major || cognitionTest;*/


                if (UnityEngine.Random.Range(0f, 1f) <= StorytellerUtility.CyberneticDominationChance(traverse.Field("pawn").GetValue<Pawn>()))
                {
                    CyberneticDominationBreakWorker.commonalityCached = 1;
                    __result = new List<MentalBreakDef>() { MentalBreakDefOf.Zomuro_SHODAN_CyberneticDomination_Break };
                    return false;
                }
                
            }

            return true;
        }

        public static MentalBreakWorker_CyberneticDomination CyberneticDominationBreakWorker
        {
            get
            {
                return (MentalBreakDefOf.Zomuro_SHODAN_CyberneticDomination_Break.Worker as MentalBreakWorker_CyberneticDomination);
            }
        }
    }
}
