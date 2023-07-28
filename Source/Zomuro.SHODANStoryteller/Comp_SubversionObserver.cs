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
    public class Comp_SubversionObserver : ThingComp
    {
        public CompProperties_SubversionObserver Props
        {
            get
            {
                return (CompProperties_SubversionObserver) props;
            }
        }

        public override IEnumerable<StatDrawEntry> SpecialDisplayStats()
        {
            yield return new StatDrawEntry(StatCategoryDefOf.Building, "SHODAN_CS_StatEntryLabel".Translate(), Props.range.ToString("F2"), "SHODAN_CS_StatEntryDesc".Translate(), 99999);
        }

        public override string CompInspectStringExtra()
        {
            if (MapComp is null || MapComp.Hacked.Contains(parent) || InRange.EnumerableNullOrEmpty()) return "";
            return "SHODAN_CS_InspectLine".Translate(InRange.Count());
        }

        public override void PostDrawExtraSelectionOverlays()
        {
            // draw radius of the building
            GenDraw.DrawRadiusRing(parent.Position, Props.range, Color.white);

            if (MapComp is null || MapComp.Hacked.Contains(parent)) return;
            foreach(var building in InRange) GenDraw.DrawLineBetween(building.DrawPos, parent.DrawPos, AltitudeLayer.Blueprint.AltitudeFor(), redLine, 0.2f);

            // consider other altitiudes for zoom-in bug
        }

        public MapComponent_ColonySubversion MapComp
        {
            get
            {
                if(cachedMapComp is null && parent.Map != null)
                {
                    cachedMapComp = StorytellerUtility.MapCompColonySubversion(parent.Map);
                }
                return cachedMapComp;
            }

        }

        public IEnumerable<Building> InRange
        {
            get
            {
                return MapComp.Hacked.Where(x => x.Position.DistanceTo(parent.Position) <= Props.range);
            }
        }

        private MapComponent_ColonySubversion cachedMapComp;

        private Material redLine = MaterialPool.MatFrom(GenDraw.LineTexPath, ShaderDatabase.Transparent, Color.red);
    }
}
