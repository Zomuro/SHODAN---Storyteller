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

        public Dialog_ColonySubversion() 
        {
            //KarmaRing = ContentFinder<Texture2D>.Get("UI/Dialogs/KaiyiKarmicRing", true);
            draggable = true;
            closeOnClickedOutside = false;
            forcePause = false;
            closeOnCancel = true;
            doCloseX = true;
        }


        public override void DoWindowContents(Rect inRect)
        {
            if (Find.CurrentMap is null || !Find.CurrentMap.IsPlayerHome)
            {
                Close();
                return;
            }

            Widgets.BeginGroup(inRect);

            // Top half - TriOptimum logo
            Rect topHalf = new Rect(0, 0, inRect.width, inRect.height);
            //Widgets.DrawTextureFitted(topHalf.ContractedBy(5), TriOptPic, 0.95f);

            // Bottom half - critical mapcomp information
            Rect bottomHalf = new Rect(0, inRect.height / 2, inRect.width, inRect.height / 2);
            if(Find.CurrentMap is null) Widgets.Label(bottomHalf, "No component network detected"); // in theory this case shouldn't happen ; // create keyed string for this
            else if (!Find.CurrentMap.IsPlayerHome) Widgets.Label(bottomHalf, "No access to the detected component network."); // create keyed string for this
            else
            {
                MapComponent_ColonySubversion mapComp =  StorytellerUtility.MapCompColonySubversion(Find.CurrentMap);
                if(mapComp.Hackable.Count() <= 10)
                {
                    // create keyed string for this
                    Widgets.Label(bottomHalf, "Component network integrity issue detected due to low node count. The observer is unable to accurately assess control levels.\n\nPlease add additional units to the grid in order to improve stability.");
                }
                else
                {
                    Rect barRect = new Rect(bottomHalf);
                    barRect = barRect.ContractedBy(5);
                    barRect.y = bottomHalf.y;
                    barRect.height = 30;
                    Widgets.FillableBar(barRect, mapComp.ControlPercentage, TriOptTex, EmptyBarTex, true);
                    
                    Text.Anchor = TextAnchor.MiddleCenter;
                    Widgets.Label(barRect, mapComp.ControlPercentage.ToStringPercent());
                    Text.Anchor = TextAnchor.UpperLeft;
                }
            }

            Widgets.EndGroup();
        }

        // private Map map;

        private Texture2D TriOptPic;

        private static readonly Texture2D EmptyBarTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.03f, 0.035f, 0.05f));

        private static readonly Texture2D TriOptTex = SolidColorMaterials.NewSolidColorTexture(new Color(1/255f, 172/255f, 18/255f));
    }
}
