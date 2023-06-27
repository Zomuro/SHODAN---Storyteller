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
    public class IncidentWorker_ColonySubversion_Raid : IncidentWorker_RaidEnemy
    {
        protected override bool CanFireNowSub(IncidentParms parms)
        {
            return Find.Storyteller.def == StorytellerDefOf.Zomuro_SHODAN && parms.target is Map map && map.IsPlayerHome &&
                StorytellerUtility.MapCompColonySubversion(map).CanFireIncident();
        }

		protected override bool TryExecuteWorker(IncidentParms parms)
		{
            if (!base.TryExecuteWorker(parms)) return false;

            MapComponent_ColonySubversion mapComp = StorytellerUtility.MapCompColonySubversion(parms.target as Map);

            List<Building> targets = mapComp.potentialHacked;
            mapComp.potentialHacked.Clear();

            // add setting here on what exactly gets blown up- for now, about 15% will do
            foreach(var target in targets)
            {
                if (Rand.Chance(0.15f)) 
                    GenExplosion.DoExplosion(target.Position, target.Map, target.PowerComp.Props.PowerConsumption / 1000f, DamageDefOf.Bomb, target);
            }

            return true;
		}

        protected override bool TryResolveRaidFaction(IncidentParms parms)
        {
            parms.faction = Find.FactionManager.FirstFactionOfDef(FactionDefOf.Zomuro_SHODAN_Faction);

            Map map = (Map)parms.target;
            return (parms.faction != null && parms.faction.HostileTo(Faction.OfPlayer));
        }
    }
}
