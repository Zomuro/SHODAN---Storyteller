using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace Zomuro.SHODANStoryteller
{
	public class IncidentWorker_ColonySubversion_Overclock : IncidentWorker_MakeGameCondition
	{
		protected override bool CanFireNowSub(IncidentParms parms)
		{
			return base.CanFireNowSub(parms) && TargetExists(parms);
		}

		public bool TargetExists(IncidentParms parms)
		{
			return StorytellerUtility.MapCompColonySubversion((Map)parms.target) is MapComponent_ColonySubversion mapComp &&
				mapComp != null && !mapComp.Hacked.EnumerableNullOrEmpty();
		}
	}
}
