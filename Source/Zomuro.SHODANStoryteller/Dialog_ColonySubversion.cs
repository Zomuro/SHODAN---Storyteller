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
    public class Dialog_ColonySubversion : Window 
    {

        public override Vector2 InitialSize
        {
            get
            {
                return new Vector2(200f, 400f);
            }
        }

        protected override float Margin
        {
            get
            {
                return 10f;
            }
        }

        public Dialog_ColonySubversion() 
        {
            TriOptPic = ContentFinder<Texture2D>.Get("UI/Dialogs/TriOptFullLogo", true);
            draggable = true;
            preventCameraMotion = false; // allow the player to scroll around
            closeOnClickedOutside = true; // immersion element- can't check the PDA AND perform other tasks at the same time
            forcePause = false; // shouldn't stop the game
            closeOnCancel = true;
            doCloseX = true;
        }


        public override void DoWindowContents(Rect inRect)
        {
            Text.Font = GameFont.Small;
            if (Find.Storyteller.def != StorytellerDefOf.Zomuro_SHODAN || Find.CurrentMap is null) // failsafe scenario
            {
                Close();
                return;
            }

            Widgets.BeginGroup(inRect);

            // Top half - TriOptimum logo
            Rect topHalf = new Rect(0, 0, inRect.width, inRect.height / 2);
            Widgets.DrawTextureFitted(topHalf, TriOptPic, 1f);

            // draw horizontal line to split
            Color orgCol = GUI.color;
            GUI.color = Color.grey;
            Widgets.DrawLineHorizontal(0, inRect.height / 2f, inRect.width);
            GUI.color = orgCol;

            // Bottom half - critical mapcomp information
            Rect bottomPart = new Rect(0, inRect.height / 2f, inRect.width, inRect.height / 2f);
            Rect barRect = new Rect(bottomPart);
            barRect = barRect.ContractedBy(5);
            barRect.yMin = bottomPart.y;
            barRect.height = 25;

            // Label of bar
            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(barRect, "SHODAN_CS_BarTitle".Translate());
            barRect.y += barRect.height;

            Rect infoRect = new Rect(bottomPart);
            infoRect.yMin = barRect.yMax;
            infoRect.ContractedBy(10f);

            if (Find.CurrentMap is null) Widgets.Label(bottomPart, "SHODAN_CS_NullMap".Translate()); // in theory this case shouldn't happen ; // create keyed string for this
            else if (!Find.CurrentMap.IsPlayerHome)
            {
                Widgets.FillableBar(barRect, 1f, ErrorTex, EmptyBarTex, true);
                Widgets.Label(barRect, "SHODAN_CS_BarError".Translate());

                Text.Anchor = TextAnchor.UpperLeft;
                Text.Font = GameFont.Tiny;
                // create keyed string for this
                Widgets.Label(infoRect, "SHODAN_CS_NotPlayerHome".Translate()); // create keyed string for this
                Text.Font = GameFont.Small;
               
            }
            else
            {
                MapComponent_ColonySubversion mapComp =  StorytellerUtility.MapCompColonySubversion(Find.CurrentMap);
                if (mapComp.Hackable.Count() <= 10) // add setting- if the hackable items in the map are less than a certain amount, creates a bar to prevent SHODAN from instantly forcing a raid
                {
                    // Draw bar of control level
                    Widgets.FillableBar(barRect, 1f, ErrorTex, EmptyBarTex, true);
                    Widgets.Label(barRect, "SHODAN_CS_BarError".Translate());

                    Text.Anchor = TextAnchor.UpperLeft;
                    Text.Font = GameFont.Tiny;
                    // create keyed string for this
                    Widgets.Label(infoRect, "SHODAN_CS_LowNodeCount".Translate());
                    Text.Font = GameFont.Small;
                }
                else
                {
                    // Draw bar of control level
                    Widgets.FillableBar(barRect, mapComp.ControlPercentage, TriOptTex, EmptyBarTex, true);
                    Widgets.Label(barRect, mapComp.ControlPercentage.ToStringPercent());

                    // Draw information on effects of control level
                    Text.Anchor = TextAnchor.UpperLeft;
                    Text.Font = GameFont.Tiny;
                    string text = "SHODAN_CS_PassiveLog".Translate();
                    if (mapComp.ControlPercentage >= 0.25f) text += "SHODAN_CS_Passive25".Translate();
                    if (mapComp.ControlPercentage >= 0.5f) text += "SHODAN_CS_Passive50".Translate();
                    if (mapComp.ControlPercentage >= 0.75f) text += "SHODAN_CS_Passive75".Translate();
                    Widgets.Label(infoRect, text);

                    Text.Font = GameFont.Small;
                }
            }

            Widgets.EndGroup();
        }

        // private Map map;

        private Texture2D TriOptPic;

        private static readonly Texture2D EmptyBarTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.03f, 0.035f, 0.05f));

        private static readonly Texture2D ErrorTex = SolidColorMaterials.NewSolidColorTexture(new Color(1, 0f, 0f));

        private static readonly Texture2D TriOptTex = SolidColorMaterials.NewSolidColorTexture(new Color(1/255f, 172/255f, 18/255f));
    }
}
