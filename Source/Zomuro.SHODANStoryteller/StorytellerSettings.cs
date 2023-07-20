using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using UnityEngine;

namespace Zomuro.SHODANStoryteller
{
    public class StorytellerSettings : ModSettings
    {
        // Cybernetic Domination
        public float MoodDebuffPerImplant = 2f;

        public float BreakChancePerImplant = 0.1f;

        public bool BrainImplantForceDom = false;

        public float ImplantExtractChance = 0.15f;

        public float ComponentSalvageProp = 0.25f;

        // Colony Subversion

        public float MTBDaysHack = 1.5f;

        public float MTBDaysSubversions = 5f;

        public float OverclockHeatPush = 1f;

        public float BasePowerFactor = 0.25f;

        public float PowerFlatDebuff = 100f;

        public float PowerGenerationFactor = 0.5f;

        public float PowerConsumptionFactor = 1.5f;

        public float OverloadBoomChance = 0.15f;

       

        public override void ExposeData()
        {
            Scribe_Values.Look(ref MoodDebuffPerImplant, "MoodDebuffPerImplant");
            Scribe_Values.Look(ref BreakChancePerImplant, "BreakChancePerImplant");
            Scribe_Values.Look(ref BrainImplantForceDom, "BrainImplantForceDom");
            Scribe_Values.Look(ref ImplantExtractChance, "ImplantExtractChance");
            Scribe_Values.Look(ref ComponentSalvageProp, "ComponentSalvageProp");

            Scribe_Values.Look(ref MTBDaysHack, "MTBDaysHack");
            Scribe_Values.Look(ref MTBDaysSubversions, "MTBDaysSubversions");
            Scribe_Values.Look(ref BasePowerFactor, "BasePowerFactor");
            Scribe_Values.Look(ref PowerFlatDebuff, "PowerFlatIncrease");
            Scribe_Values.Look(ref PowerGenerationFactor, "PowerGenerationFactor");
            Scribe_Values.Look(ref PowerConsumptionFactor, "PowerConsumptionFactor");
            Scribe_Values.Look(ref OverloadBoomChance, "OverloadBoomChance");
            Scribe_Values.Look(ref OverclockHeatPush, "OverclockHeatPush");

            base.ExposeData();
        }
    }
}
