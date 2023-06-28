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

            // SpawnSetup_Postfix: when a building is spawned, verify it can be added before adding a building to the hackable list.
            harmony.Patch(AccessTools.Method(typeof(Building), "SpawnSetup"),
                null, new HarmonyMethod(typeof(HarmonyPatches), nameof(SpawnSetup_Postfix)));

            // DeSpawn_Postfix: when a building is despawned, remove it from the hackable list and hacked list. // null, 
            harmony.Patch(AccessTools.Method(typeof(Building), "DeSpawn"),
                new HarmonyMethod(typeof(HarmonyPatches), nameof(DeSpawn_Postfix)));

            // Storyteller_PopulateHackable_Postfix: clean up and populate the mapcomponent on storyteller change
            harmony.Patch(AccessTools.Constructor(typeof(Storyteller), new[] {typeof(StorytellerDef), typeof(DifficultyDef), typeof(Difficulty)}),
                null, new HarmonyMethod(typeof(HarmonyPatches), nameof(Storyteller_PopulateHackable_Postfix)));

            // TryConnectToAnyPowerNet_Postfix: when connecting a building to a powernet, recheck the hackables and hacked portions of the mapcomp
            harmony.Patch(AccessTools.Method(typeof(CompPower), "ConnectToTransmitter"),
                null, new HarmonyMethod(typeof(HarmonyPatches), nameof(ConnectToTransmitter_Postfix)));
        }

        // PREFIX: if a pawn has a mental break, have a chance to force SHODAN's incident
        public static bool CurrentPossibleMoodBreaks_Prefix(MentalBreaker __instance, ref IEnumerable<MentalBreakDef> __result)
        {
            CyberneticDominationBreakWorker.commonalityCached = 0;
            if (Find.Storyteller.def == StorytellerDefOf.Zomuro_SHODAN)
            {
                Traverse traverse = Traverse.Create(__instance);
                //Log.Message("Calc Chance: " + StorytellerUtility.CyberneticDominationChance(traverse.Field("pawn").GetValue<Pawn>()));

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

        // POSTFIX: when a building is spawned, verify it can be added before adding a building to the mapcomp hackable list.
        public static void SpawnSetup_Postfix(Building __instance, Map __0)
        {
            if (__0 is null || !__0.IsPlayerHome) return;
            StorytellerUtility.MapCompColonySubversion(__0).AddHackable(__instance);
        }

        // POSTFIX: when a building is destroyed, remove it from the mapcomp hackable list and hacked list.
        public static void DeSpawn_Postfix(Building __instance)
        {
            if (__instance.Map is null || !__instance.Map.IsPlayerHome) return;
            StorytellerUtility.MapCompColonySubversion(__instance.Map).RemoveBuilding(__instance);
        }

        // POSTFIX: clean up and populate the mapcomponent on storyteller change
        public static void Storyteller_PopulateHackable_Postfix(StorytellerDef __0)
        {
            foreach (var map in Find.Maps.Where(x => x.IsPlayerHome))
            {
                MapComponent_ColonySubversion mapComp = StorytellerUtility.MapCompColonySubversion(map);
                mapComp.CleanAll();

                if (__0 == StorytellerDefOf.Zomuro_SHODAN)
                {
                    foreach (var building in map.listerBuildings.allBuildingsColonist) mapComp.AddHackable(building);
                }  
            }
        }

        // POSTFIX: when connecting a building to a powernet, recheck the hackables and hacked portions of the mapcomp
        public static void ConnectToTransmitter_Postfix(CompPower __instance)
        {
            ResetMapCompCache(__instance);
        }

        // helper method to more easily reset the MapComp's cache
        public static void ResetMapCompCache(CompPower pc)
        {
            if (MapCompColonySubversion(pc.parent.Map) is MapComponent_ColonySubversion mapComp && mapComp != null)
            {
                mapComp.CleanCache();
            }
        }

        // helper method to nab the right mapcomponent
        public static MapComponent_ColonySubversion MapCompColonySubversion(Map map)
        {
            return map?.GetComponent<MapComponent_ColonySubversion>();
        }

    }
}
