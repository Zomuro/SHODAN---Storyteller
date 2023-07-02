using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace Zomuro.SHODANStoryteller
{
	public class IncidentWorker_ColonySubversion_IFF : IncidentWorker_MakeGameCondition
	{
		protected override bool CanFireNowSub(IncidentParms parms)
		{
			return base.CanFireNowSub(parms) && TargetExists(parms);
		}

		public bool TargetExists(IncidentParms parms)
		{
			return StorytellerUtility.MapCompColonySubversion((Map)parms.target)?.Hacked.FirstOrDefault(x => x.def.thingClass == typeof(Building_TurretGun)) != null;
		}
	}
}
