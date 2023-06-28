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
                StorytellerUtility.MapCompColonySubversion(map).CanFireIncidentCheck();
        }

		protected override bool TryExecuteWorker(IncidentParms parms)
		{
            MapComponent_ColonySubversion mapComp = StorytellerUtility.MapCompColonySubversion(parms.target as Map);
            if (mapComp.Hackable.Except(mapComp.Hacked).TryRandomElement(out Building building))
            {
                mapComp.potentialHacked.Add(building);
                mapComp.dirtyHacked = true;
                return true;
            }

            return false;
		}
	}
}
