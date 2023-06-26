using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace Zomuro.SHODANStoryteller
{
    public class IncidentWorker_ColonySubversion_AddHack : IncidentWorker
    {
        protected override bool CanFireNowSub(IncidentParms parms)
        {
            Map map = parms.target as Map;
            return Find.Storyteller.def == StorytellerDefOf.Zomuro_SHODAN && map != null && map.IsPlayerHome && 
                StorytellerUtility.MapCompColonySubversion(map).ControlPercentage >= 0;
        }

		protected override bool TryExecuteWorker(IncidentParms parms)
		{
            MapComponent_ColonySubversion mapComp = StorytellerUtility.MapCompColonySubversion(parms.target as Map);
            if (mapComp.allHackableBuildings.Except(mapComp.hackedBuildings).TryRandomElement(out Building building))
            {
                mapComp.hackedBuildings.Add(building);
                return true;
            }

            return false;
		}
	}
}
