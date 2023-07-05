using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using Verse.AI.Group;
using UnityEngine;


namespace Zomuro.SHODANStoryteller
{
    public static class StorytellerUtility 
    {
        // Cybernetic Domination  //
        public static bool TechImplantCheck(Hediff hediff)
        {
            // checks if the hediff is part/implant, if the thing used to implant isn't null
            // and if the implant (thing) tech level is high enough
            ThingDef implantThing = hediff.def.spawnThingOnRemoved;
            if (!hediff.def.countsAsAddedPartOrImplant || implantThing is null || 
                implantThing.techLevel < TechLevel.Industrial) return false;

            return true;
        }

        public static bool BrainImplantCheck(Hediff hediff)
        {
            // checks if the hediff is in the brain, its an implant/new part, and if the tech level check if fine
            if (hediff.Part.def != BodyPartDefOf.Brain || !TechImplantCheck(hediff)) return false;
            return true;
        }

        public static float CyberneticDominationChance(Pawn pawn)
        {
            float finalProb = 0;
            HashSet<Hediff> hediffs = pawn.health.hediffSet.hediffs.ToHashSet();
            if (hediffs.EnumerableNullOrEmpty()) return finalProb;

            // adjust setting here for chance to subvert in case of brain implant
            // and for the option to enable this optional mechanic
            if (settings.BrainImplantForceDom && hediffs.FirstOrDefault(x => BrainImplantCheck(x)) != null) return 1f;

            // adjust setting here for increased prob
            foreach (var hediff in hediffs) if (TechImplantCheck(hediff)) finalProb += settings.BreakChancePerImplant;

            return Mathf.Clamp(finalProb, 0, 1f);
        }

        // Colony Subversion //
        public static MapComponent_ColonySubversion MapCompColonySubversion(Map map)
        {
            return map?.GetComponent<MapComponent_ColonySubversion>();
        }

        public static StorytellerSettings settings = LoadedModManager.GetMod<StorytellerMod>().GetSettings<StorytellerSettings>();
    }
}
