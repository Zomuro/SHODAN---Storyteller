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
            //KarmaRing = ContentFinder<Texture2D>.Get("UI/Dialogs/KaiyiKarmicRing", true);
            draggable = true;
            preventCameraMotion = false;
            closeOnClickedOutside = false;
            forcePause = false;
            closeOnCancel = true;
            doCloseX = true;
        }


        public override void DoWindowContents(Rect inRect)
        {
            Text.Font = GameFont.Small;
            if (Find.Storyteller.def != StorytellerDefOf.Zomuro_SHODAN || Find.CurrentMap is null || !Find.CurrentMap.IsPlayerHome)
            {
                Close();
                return;
            }

            Widgets.BeginGroup(inRect);

            // Top half - TriOptimum logo
            Rect topHalf = new Rect(0, 0, inRect.width, inRect.height);
            //Widgets.DrawTextureFitted(topHalf.ContractedBy(5), TriOptPic, 0.95f);

            // Bottom half - critical mapcomp information
            Rect bottomPart = new Rect(0, inRect.height / 2f, inRect.width, inRect.height * 2f);
            Rect barRect = new Rect(bottomPart);
            barRect = barRect.ContractedBy(5);
            barRect.y = bottomPart.y;
            barRect.height = 25;

            // Label of bar
            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(barRect, "Network Corruption");
            barRect.y += barRect.height;

            Rect infoRect = new Rect(bottomPart);
            infoRect.yMin = barRect.yMax;
            infoRect.ContractedBy(10f);

            if (Find.CurrentMap is null) Widgets.Label(bottomPart, "No component network detected"); // in theory this case shouldn't happen ; // create keyed string for this
            else if (!Find.CurrentMap.IsPlayerHome)
            {
                Widgets.FillableBar(barRect, 1f, ErrorTex, EmptyBarTex, true);
                Widgets.Label(barRect, "ERROR");

                Text.Anchor = TextAnchor.UpperLeft;
                Text.Font = GameFont.Tiny;
                // create keyed string for this
                Widgets.Label(infoRect, "No access to the network.\n\nUsage of the TriOptimum (R) Personal Diagnostic Assistant in an unauthorized setting will void any and all protections on it."); // create keyed string for this
                Text.Font = GameFont.Small;
               
            }
            else
            {
                MapComponent_ColonySubversion mapComp =  StorytellerUtility.MapCompColonySubversion(Find.CurrentMap);
                if (mapComp.Hackable.Count() <= 10)
                {
                    // Draw bar of control level
                    Widgets.FillableBar(barRect, 1f, ErrorTex, EmptyBarTex, true);
                    Widgets.Label(barRect, "ERROR");

                    Text.Anchor = TextAnchor.UpperLeft;
                    Text.Font = GameFont.Tiny;
                    // create keyed string for this
                    Widgets.Label(infoRect, "Poor network integrity due to low node count. The observer is unable to accurately assess control levels." +
                        "\n\nPlease add additional units to the grid in order to improve stability.");
                    Text.Font = GameFont.Small;
                }
                else
                {
                    /*Rect barRect = new Rect(bottomPart);
                    barRect = barRect.ContractedBy(5);
                    barRect.y = bottomPart.y;
                    barRect.height = 25;
                    
                    // Label of bar
                    Text.Anchor = TextAnchor.MiddleCenter;
                    Widgets.Label(barRect, "Network Infection");
                    barRect.y += barRect.height;*/

                    // Draw bar of control level
                    Widgets.FillableBar(barRect, mapComp.ControlPercentage, TriOptTex, EmptyBarTex, true);
                    Widgets.Label(barRect, mapComp.ControlPercentage.ToStringPercent());

                    // Draw information on effects of control level
                    Text.Anchor = TextAnchor.UpperLeft;
                    Text.Font = GameFont.Tiny;
                    string text = "Logged Issues: ";
                    if (mapComp.ControlPercentage >= 0.25f) text += "\n* (25%) Power delivery and reception issues.";
                    if (mapComp.ControlPercentage >= 0.5f) text += "\n* (50%) Power consumption increased.";
                    if (mapComp.ControlPercentage >= 0.75f) text += "\n* (75%) Power generation decreased.";
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
