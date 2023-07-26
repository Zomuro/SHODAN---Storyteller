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

            // Cybernetic Domination //
            // CurrentPossibleMoodBreaks_Prefix: if a pawn has a mental break, have chance to force SHODAN's incident
            harmony.Patch(AccessTools.Method(typeof(MentalBreaker), "get_CurrentPossibleMoodBreaks"),
                new HarmonyMethod(typeof(HarmonyPatches), nameof(CurrentPossibleMoodBreaks_Prefix)));

            // ButcherProducts_Postfix: adds a chance for any added part/implants to be added to the butcher products
            harmony.Patch(AccessTools.Method(typeof(Corpse), "ButcherProducts"),
                null, new HarmonyMethod(typeof(HarmonyPatches), nameof(ButcherProducts_Postfix)));

            // Colony Subversion //
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

            // get_PowerOutput_Postfix: postfixes the power output to be affected by the mapcomp & if SHODAN has hacked the building
            harmony.Patch(AccessTools.Method(typeof(CompPowerTrader), "get_PowerOutput"),
                null, new HarmonyMethod(typeof(HarmonyPatches), nameof(get_PowerOutput_Postfix)));

            // GetGizmos_Postfix: adds a gizmo that allows a pawn to access the PDA.
            harmony.Patch(AccessTools.Method(typeof(Pawn), "GetGizmos"),
                null, new HarmonyMethod(typeof(HarmonyPatches), nameof(GetGizmos_Postfix)));

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

        public static void ButcherProducts_Postfix(Corpse __instance, ref IEnumerable<Thing> __result)
        {
            if (__instance.InnerPawn is null) return;
            IEnumerable<Thing> products = ImplantProducts(__instance.InnerPawn);
            if (products.EnumerableNullOrEmpty()) return;

            List<Thing> finalProd = __result.ToList();
            finalProd.AddRange(products);
            __result = finalProd;
        }


        public static IEnumerable<Thing> ImplantProducts(Pawn pawn)
        {
            foreach(var hediff in pawn.health.hediffSet.hediffs)
            {
                if (!hediff.def.countsAsAddedPartOrImplant) continue; // if this isn't an implant/added part, skip
                if(hediff.def.spawnThingOnRemoved is null)  // if this thing doesn't spawn a thing on removal, just spawn a component
                {
                    yield return ThingMaker.MakeThing(ThingDefOf.ComponentIndustrial, null);
                    continue;
                }

                // but if it doesn't spawn a thing on removal and succeeds the rng roll, spawn the implant item
                if(UnityEngine.Random.Range(0f,1f) <= 1f) yield return ThingMaker.MakeThing(hediff.def.spawnThingOnRemoved, null);
                else // failing the chance will spawn a portion of the components used in the implant instead
                {
                    // no cost list (things used to make the implant)? skip it
                    if (hediff.def.spawnThingOnRemoved.costList.NullOrEmpty()) continue;
                    foreach(var thingCount in hediff.def.spawnThingOnRemoved.costList) // spawn components
                    {
                        if(thingCount.thingDef == ThingDefOf.ComponentIndustrial || thingCount.thingDef == ThingDefOf.ComponentSpacer) continue;
                        Thing component = ThingMaker.MakeThing(thingCount.thingDef, null);
                        component.stackCount = Mathf.FloorToInt(0.25f * thingCount.count); // add settings of the chance for component retrieval
                        yield return component;
                    }
                }
            }

            yield break;
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

        // POSTFIX: alters the power output (generation and consumption) depending on SHODAN's control level & if SHODAN has hacked the building
        public static void get_PowerOutput_Postfix(CompPowerTrader __instance, ref float __result)
        {
            float num = __result;
            if (__instance.parent.Map is null || !__instance.parent.Map.IsPlayerHome || (__instance.parent.Faction != null && __instance.parent.Faction.IsPlayer)) return;
            num *= 1f + StorytellerUtility.settings.BasePowerFactor;

            MapComponent_ColonySubversion mapComp = MapCompColonySubversion(__instance.parent.Map);
            if (mapComp is null || !mapComp.Hacked.Contains(__instance.parent))
            {
                __result = num;
                return;
            }

            float flat = StorytellerUtility.settings.PowerFlatDebuff; // set setting here mapComp.ControlPercentage >= 0.25f ? 0f : 100f

            // add control percent scaling here
            if (__result < 0) 
                __result = Mathf.Min(num * (1f + mapComp.ControlPercentage) - flat, 0); // negative power output = consumption
            else 
                __result = Mathf.Max(num * (1f - mapComp.ControlPercentage) - flat, 0); // positive power output = generation
        }

        // POSTFIX: adds a PDA viewing gizmo that can be used to pull up the dialog
        public static void GetGizmos_Postfix(Pawn __instance, ref IEnumerable<Gizmo> __result)
        {
            // only for pawns on a map, only for colonists
            // IEnumerable<Gizmo> gizmos = __result;
            if (Find.Storyteller.def == StorytellerDefOf.Zomuro_SHODAN && __instance.Map != null && __instance.IsColonist)
            {
                Gizmo gizmo = new Command_Action
                {
                    icon = ContentFinder<Texture2D>.Get("UI/Dialogs/TriOptLogo", true), // get custom texture
                    defaultLabel = "Zomuro_SHODAN_CS_ViewLabel".Translate(),
                    defaultDesc = "Zomuro_SHODAN_CS_ViewDesc".Translate(),
                    action = delegate ()
                    {
                        if (!Find.WindowStack.IsOpen(typeof(Dialog_ColonySubversion))) Find.WindowStack.Add(new Dialog_ColonySubversion());
                        else Find.WindowStack.TryRemove(typeof(Dialog_ColonySubversion));
                    }
                };
                __result = __result.AddItem(gizmo);
            }
        }

        // helper method to nab the right mapcomponent
        public static MapComponent_ColonySubversion MapCompColonySubversion(Map map)
        {
            return StorytellerUtility.MapCompColonySubversion(map);
        }

    }
}
