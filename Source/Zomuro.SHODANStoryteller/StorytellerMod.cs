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
    public class StorytellerMod : Mod
    {
        StorytellerSettings settings;

        public StorytellerMod(ModContentPack content) : base(content)
        {
            this.settings = GetSettings<StorytellerSettings>();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            tabsList.Clear();
            tabsList.Add(new TabRecord("SHODANSetting_CD".Translate(), delegate ()
            {
                this.tab = Tab.CyberneticDomination;
            }, this.tab == Tab.CyberneticDomination));
            tabsList.Add(new TabRecord("SHODANSetting_CS".Translate(), delegate ()
            {
                this.tab = Tab.ColonySubversion;
            }, this.tab == Tab.ColonySubversion));
            /*tabsList.Add(new TabRecord("YS_SettingsJianghuJin".Translate(), delegate ()
            {
                this.tab = Tab.JianghuJin;
            }, this.tab == Tab.JianghuJin));*/
            Rect tabRect = new Rect(inRect);
            tabRect.yMin = 80;
            TabDrawer.DrawTabs<TabRecord>(tabRect, tabsList, 200);

            Rect leftThird = new Rect(tabRect);
            leftThird.width = inRect.width / 3;
            Rect otherTwoThird = new Rect(tabRect);
            otherTwoThird.xMin += tabRect.width / 3;

            /*switch (this.tab)
            {
                case Tab.FarseerFan:
                    break;

                case Tab.KaiyiKarmic:
                    break;

                case Tab.DeathlessDaji:
                    break;

                case Tab.JianghuJin:
                    break;

                default: break;
            }*/

            var listing = new Listing_Standard();
            listing.Begin(leftThird);
            listing.Gap(16f);

            switch (this.tab)
            {
                case Tab.CyberneticDomination:
                    CyberneticDominationSettings(ref listing);
                    break;
                case Tab.ColonySubversion:
                    ColonySubversionSettings(ref listing);
                    break;
                default: break;
            }

            // Reset to default
            listing.Gap(16f);
            if (listing.ButtonText("Reset to global default"))
            {
                CyberneticDominationReset();
                ColonySubversionReset();
            }
            listing.End();

            base.DoSettingsWindowContents(inRect);
        }

        public void CyberneticDominationReset()
        {
            settings.MoodDebuffPerImplant = 2f;
            settings.BreakChancePerImplant = 0.1f;
            settings.BrainImplantForceDom = false;
            settings.ImplantExtractChance = 0.15f;
            settings.ComponentSalvageProp = 0.25f;
        }

        public void ColonySubversionReset()
        {
            settings.MTBDaysHack = 1.5f;
            settings.MTBDaysSubversions = 5f;
            settings.BasePowerFactor = 0.25f;
            settings.PowerFlatDebuff = 100f;
            //settings.PowerGenerationFactor = 0.5f;
            //settings.PowerConsumptionFactor = 1.5f;
            settings.OverclockHeatPush = 3f;
            settings.OverloadBoomChance = 0.15f;
        }

        public void CyberneticDominationSettings(ref Listing_Standard listing)
        {
            // title of settings
            Text.Font = GameFont.Medium;
            listing.Label("SHODANSetting_CD".Translate());
            Text.Font = GameFont.Small;
            listing.GapLine();

            listing.Label("SHODANSetting_CD_MoodDebuff".Translate(settings.MoodDebuffPerImplant), -1, "SHODANSetting_CD_MoodDebuffDesc".Translate());
            settings.MoodDebuffPerImplant = listing.Slider((int)settings.MoodDebuffPerImplant, 0f, 5f);

            listing.Label("SHODANSetting_CD_ChancePerImplant".Translate(settings.BreakChancePerImplant.ToStringPercentEmptyZero()), -1, "SHODANSetting_CD_ChancePerImplantDesc".Translate());
            settings.BreakChancePerImplant = listing.Slider((float)RoundToNearestHundredth(settings.BreakChancePerImplant), 0f, 1f);

            listing.CheckboxLabeled("SHODANSetting_CD_BrainImplantForce".Translate(settings.BrainImplantForceDom.ToString()),
                ref settings.BrainImplantForceDom, "SHODANSetting_CD_BrainImplantForceDesc".Translate());

            listing.Label("SHODANSetting_CD_ImplantExtractChance".Translate(settings.ImplantExtractChance.ToStringPercentEmptyZero()), -1, "SHODANSetting_CD_ImplantExtractChanceDesc".Translate());
            settings.ImplantExtractChance = listing.Slider((float) RoundToNearest5Percent(settings.ImplantExtractChance), 0f, 1f);

            listing.Label("SHODANSetting_CD_ComponentProp".Translate(settings.ComponentSalvageProp.ToStringPercentEmptyZero()), -1, "SHODANSetting_CD_ComponentPropDesc".Translate());
            settings.ComponentSalvageProp = listing.Slider((float) RoundToNearestTenth(settings.ComponentSalvageProp), 0f, 1f);

            listing.Gap(16f);
            if (listing.ButtonText("Reset to default"))
            {
                CyberneticDominationReset();
            }
        }

        public void ColonySubversionSettings(ref Listing_Standard listing)
        {
            // title of settings
            Text.Font = GameFont.Medium;
            listing.Label("SHODANSetting_CS".Translate());
            Text.Font = GameFont.Small;
            listing.GapLine();

            listing.Label("SHODANSetting_CS_MTBHack".Translate(settings.MTBDaysHack.ToString("F1")), -1, "SHODANSetting_CS_MTBHackDesc".Translate());
            settings.MTBDaysHack = listing.Slider((float) RoundToNearestTenth(settings.MTBDaysHack), 0.5f, 5f);

            listing.Label("SHODANSetting_CS_MTBSubversion".Translate(settings.MTBDaysSubversions.ToString("F1")), -1, "SHODANSetting_CS_MTBSubversionDesc".Translate());
            settings.MTBDaysSubversions = listing.Slider((float) RoundToNearestTenth(settings.MTBDaysSubversions), 3f, 10f);

            listing.Label("SHODANSetting_CS_BasePowerFactor".Translate((1f + settings.BasePowerFactor).ToString("F2")), -1, 
                "SHODANSetting_CS_BasePowerFactorDesc".Translate());
            settings.BasePowerFactor = listing.Slider((float)RoundToNearest5Percent(settings.BasePowerFactor), -0.5f, 0.5f);

            listing.Label("SHODANSetting_CS_PowerFlat".Translate(settings.PowerFlatDebuff), -1, "SHODANSetting_CS_PowerFlatDesc".Translate());
            settings.PowerFlatDebuff = listing.Slider((int)settings.PowerFlatDebuff, 0f, 200f);

            /*listing.Label("SHODANSetting_CS_ConsumpFactor".Translate(settings.PowerConsumptionFactor), -1, "SHODANSetting_CS_ConsumpFactorDesc".Translate());
            settings.PowerConsumptionFactor = listing.Slider((int)settings.PowerConsumptionFactor, 0f, 200f);

            listing.Label("SHODANSetting_CS_GenFactor".Translate(settings.PowerFlatDebuff), -1, "SHODANSetting_CS_GenFactorDesc".Translate());
            settings.PowerFlatDebuff = listing.Slider((int)settings.PowerFlatDebuff, 0f, 200f);*/

            listing.Label("SHODANSetting_CS_HeatPush".Translate(settings.OverclockHeatPush), -1, "SHODANSetting_CS_HeatPushDesc".Translate());
            settings.OverclockHeatPush = listing.Slider((int)settings.OverclockHeatPush, 0f, 15f);

            listing.Label("SHODANSetting_CS_OverloadBoomChance".Translate(settings.OverloadBoomChance.ToStringPercent()), -1, "SHODANSetting_CS_OverloadBoomChanceDesc".Translate());
            settings.OverloadBoomChance = listing.Slider((float)RoundToNearestHundredth(settings.OverloadBoomChance), 0f, 1f);



            listing.Gap(16f);
            if (listing.ButtonText("Reset to default"))
            {
                ColonySubversionReset();
            }
        }


        public float RoundToNearest5Percent(float number)
        {
            return Mathf.Round(number * 20f) / 20f;
        }

        public float RoundToNearestTenth(float number)
        {
            return Mathf.Round(number * 10f) / 10f;
        }

        public float RoundToNearestHundredth(float number)
        {
            return Mathf.Round(number * 100f) / 100f;
        }



        public override string SettingsCategory()
        {
            return "SHODANSetting".Translate();
        }

        private static List<TabRecord> tabsList = new List<TabRecord>();

        private Tab tab;

        private enum Tab
        {
            CyberneticDomination,
            ColonySubversion
        }
    }
}
