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
    public class IncidentWorker_ColonySubversion_Hack : IncidentWorker
    {
        protected override bool CanFireNowSub(IncidentParms parms)
        {
            return Find.Storyteller.def == StorytellerDefOf.Zomuro_SHODAN && parms.target is Map map && map.IsPlayerHome &&
                StorytellerUtility.MapCompColonySubversion(map).CanFireIncident();
        }

		protected override bool TryExecuteWorker(IncidentParms parms)
		{
            MapComponent_ColonySubversion mapComp = StorytellerUtility.MapCompColonySubversion(parms.target as Map);
            if (mapComp.potentialHackable.Except(mapComp.potentialHacked).TryRandomElement(out Building building))
            {
                mapComp.potentialHacked.Add(building);
                return true;
            }

            return false;
		}
	}
}
