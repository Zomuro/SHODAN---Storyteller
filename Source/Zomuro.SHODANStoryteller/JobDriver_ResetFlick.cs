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
	public class JobDriver_ResetFlick : JobDriver_Flick
	{
		protected override IEnumerable<Toil> MakeNewToils()
		{
			this.FailOnDespawnedOrNull(TargetIndex.A);
			this.FailOn(() => Map.designationManager.DesignationOn(this.TargetThingA, DesignationDefOf.Zomuro_SHODAN_Designation_ResetFlick) == null);
			yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
			yield return Toils_General.Wait(60, TargetIndex.None).FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
			Toil finalize = ToilMaker.MakeToil("MakeNewToils");
			finalize.initAction = delegate ()
			{
				Building building = (Building) finalize.actor.CurJob.targetA.Thing;
				if(building != null && building.TryGetComp<CompFlickable>() is CompFlickable flickable && flickable != null)
                {
					MapComponent_ColonySubversion mapComp = StorytellerUtility.MapCompColonySubversion(building.Map);
					if (mapComp != null)
					{
						mapComp.UnhackBuilding(building);
						flickable.ResetToOn();
					}
				}

				Designation designation = Map.designationManager.DesignationOn(building, DesignationDefOf.Zomuro_SHODAN_Designation_ResetFlick);
				if (designation != null)
				{
					designation.Delete();
				}
			};
			finalize.defaultCompleteMode = ToilCompleteMode.Instant;
			yield return finalize;
			yield break;
		}
	}
}
