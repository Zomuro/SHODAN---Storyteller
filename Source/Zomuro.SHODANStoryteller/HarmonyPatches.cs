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

            // DeSpawn_Prefix: when a building is despawned, remove it from the hackable list and hacked list. , 
            harmony.Patch(AccessTools.Method(typeof(Building), "DeSpawn"),
                new HarmonyMethod(typeof(HarmonyPatches), nameof(DeSpawn_Prefix)));

            // Storyteller_PopulateHackable_Postfix: clean up and populate the mapcomponent on storyteller change
            harmony.Patch(AccessTools.Constructor(typeof(Storyteller), new[] {typeof(StorytellerDef), typeof(DifficultyDef), typeof(Difficulty)}),
                null, new HarmonyMethod(typeof(HarmonyPatches), nameof(Storyteller_PopulateHackable_Postfix)));

            // TryConnectToAnyPowerNet_Postfix: when connecting a building to a powernet, recheck the hackables and hacked portions of the mapcomp
            harmony.Patch(AccessTools.Method(typeof(CompPower), "ConnectToTransmitter"),
                null, new HarmonyMethod(typeof(HarmonyPatches), nameof(ConnectToTransmitter_Postfix)));

            // set_PowerOn_Prefix: when a pawn tries to flick a building, if it is found in a hashset nothing happens. 
            harmony.Patch(AccessTools.Method(typeof(CompFlickable), "DoFlick"),
                new HarmonyMethod(typeof(HarmonyPatches), nameof(DoFlick_Prefix)));

            // CompGetGizmosExtra_Postfix: adds the unhacking gizmo to comppowertrader buildings
            harmony.Patch(AccessTools.Method(typeof(CompPower), "CompGetGizmosExtra"),
                null, new HarmonyMethod(typeof(HarmonyPatches), nameof(CompGetGizmosExtra_Postfix)));

            // get_PowerOutput_Postfix: postfixes the power output to be affected by the mapcomp
            harmony.Patch(AccessTools.Method(typeof(CompPowerTrader), "get_PowerOutput"),
                null, new HarmonyMethod(typeof(HarmonyPatches), nameof(get_PowerOutput_Postfix)));

        }

        // PREFIX: if a pawn has a mental break, have a chance to force SHODAN's incident
        public static bool CurrentPossibleMoodBreaks_Prefix(MentalBreaker __instance, ref IEnumerable<MentalBreakDef> __result)
        {
            CyberneticDominationBreakWorker.commonalityCached = 0;
            if (Find.Storyteller.def == StorytellerDefOf.Zomuro_SHODAN)
            {
                Traverse traverse = Traverse.Create(__instance);
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
        public static void DeSpawn_Prefix(Building __instance)
        {
            if (__instance.Map is null || !__instance.Map.IsPlayerHome) return;
            MapCompColonySubversion(__instance.Map).RemoveBuilding(__instance);
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

        // PREFIX: stops flicking of switch from occuring if affected by gamecondition
        public static bool DoFlick_Prefix(CompFlickable __instance)
        {
            if (MapCompColonySubversion(__instance.parent.Map).AggregGameCondition().Contains(__instance.parent)) 
            {
                Messages.Message("Zomuro_SHODAN_CyberneticSubversion_Off".Translate(__instance.parent.Label), __instance.parent, MessageTypeDefOf.RejectInput, true);
                return false;
            }
            return true;
        }

        // POSTFIX: adds the unhacking gizmo to comppowertrader buildings
        public static void CompGetGizmosExtra_Postfix(CompPower __instance, ref IEnumerable<Gizmo> __result)
        {
            bool factionCheck = __instance.parent.Faction == Faction.OfPlayer || __instance.parent.Faction == Find.FactionManager.FirstFactionOfDef(FactionDefOf.Zomuro_SHODAN_Faction);
            if (__instance as CompPowerTrader != null && Find.Storyteller.def == StorytellerDefOf.Zomuro_SHODAN && factionCheck)
            {
                Gizmo gizmo = new Command_Action
                {
                    icon = Traverse.Create(__instance).Property("CommandTex").GetValue<Texture2D>(),
                    defaultLabel = "Zomuro_SHODAN_ColonySubversion_Label".Translate(),
                    defaultDesc = "Zomuro_SHODAN_ColonySubversion_Desc".Translate(),
                    action = delegate ()
                    {
                        Designation designation = __instance.parent.Map.designationManager.DesignationOn(__instance.parent, DesignationDefOf.Zomuro_SHODAN_Designation_ResetFlick);
                        if (designation == null)
                        {
                            __instance.parent.Map.designationManager.AddDesignation(new Designation(__instance.parent, DesignationDefOf.Zomuro_SHODAN_Designation_ResetFlick, null));
                        }
                    }
                };

                __result = __result.AddItem(gizmo);
            }
        }

        public static void get_PowerOutput_Postfix(CompPowerTrader __instance, ref float __result)
        {
            float num = __result;
            if (__instance.parent.Map is null || !__instance.parent.Map.IsPlayerHome) return;
            MapComponent_ColonySubversion mapComp = MapCompColonySubversion(__instance.parent.Map);
            if (mapComp is null) return;

            float flat = mapComp.ControlPercentage >= 0.25f ? 0f : 100f; // set setting here 
            if(__result < 0) 
                __result = Mathf.Clamp(num * (mapComp.ControlPercentage >= 0.5f ? 1f : 1.5f) - flat, num, 0); // negative power output = consumption
            else 
                __result = Mathf.Clamp(num * (mapComp.ControlPercentage >= 0.75f ? 1f : 0.5f) + flat, 0, num); // positive power output = generation
        }

        // helper method to nab the right mapcomponent
        public static MapComponent_ColonySubversion MapCompColonySubversion(Map map)
        {
            return StorytellerUtility.MapCompColonySubversion(map);
        }

    }
}
