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
        }

        public void ColonySubversionReset()
        {
            settings.MTBDaysHack = 1.5f;
            settings.MTBDaysSubversions = 5f;
            settings.PowerFlatIncrease = 100f;
            settings.PowerGenerationFactor = 0.5f;
            settings.PowerConsumptionFactor = 1.5f;
            settings.OverloadBoomChance = 0.15f;
            settings.OverclockHeatPush = 1f;
        }

        public void CyberneticDominationSettings(ref Listing_Standard listing)
        {
            // Farseer Fan
            Text.Font = GameFont.Medium;
            listing.Label("Cybernetic Domination");
            Text.Font = GameFont.Small;
            listing.GapLine();

            listing.Label("SHODANSetting_CD_MoodDebuff".Translate(settings.MoodDebuffPerImplant), -1, "SHODANSetting_CD_MoodDebuffDesc".Translate());
            settings.MoodDebuffPerImplant = listing.Slider((int)settings.MoodDebuffPerImplant, 0f, 5f);

            listing.Label("SHODANSetting_CD_ChancePerImplant".Translate(settings.BreakChancePerImplant), -1, "SHODANSetting_CD_ChancePerImplantDesc".Translate());
            settings.BreakChancePerImplant = listing.Slider((float)settings.BreakChancePerImplant, 0f, 1f);

            listing.CheckboxLabeled("SHODANSetting_CD_BrainImplantForce".Translate(settings.BrainImplantForceDom.ToString()),
                ref settings.BrainImplantForceDom, "SHODANSetting_CD_BrainImplantForceDesc".Translate());

            listing.Gap(16f);
            if (listing.ButtonText("Reset to default"))
            {
                CyberneticDominationReset();
            }
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
