using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using Verse.AI;

namespace Zomuro.SHODANStoryteller
{
	public class WorkGiver_ColonySubversion_ResetFlick : WorkGiver_Flick
	{
		public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
		{
			foreach (Designation designation in pawn.Map.designationManager.designationsByDef[DesignationDefOf.Zomuro_SHODAN_Designation_ResetFlick])
			{
				yield return designation.target.Thing;
			}

			yield break;
		}

		public override bool ShouldSkip(Pawn pawn, bool forced = false)
		{
			return !pawn.Map.designationManager.AnySpawnedDesignationOfDef(DesignationDefOf.Zomuro_SHODAN_Designation_ResetFlick);
		}

		public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			return pawn.Map.designationManager.DesignationOn(t, DesignationDefOf.Zomuro_SHODAN_Designation_ResetFlick) != null && pawn.CanReserve(t, 1, -1, null, forced);
		}

		public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			return JobMaker.MakeJob(JobDefOf.Zomuro_SHODAN_Job_ResetFlick, t);
		}
	}
}
