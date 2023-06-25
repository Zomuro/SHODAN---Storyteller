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
	public class JobGiver_CyberneticDominationSap : ThinkNode_JobGiver
	{
		// Thank you Killathon, for the jobgiver code.
		protected override Job TryGiveJob(Pawn pawn)
        {
			if (pawn.TryGetAttackVerb(null) is null) return null;

            Pawn target = FindAttackTarget(pawn);
            if (pawn != null)
            {
                using (PawnPath pawnPath = pawn.Map.pathFinder.FindPath(pawn.Position, target.Position, TraverseParms.For(pawn, Danger.Deadly, TraverseMode.PassDoors)))
                {
                    if (!pawnPath.Found) return null;
                    Thing blocker = pawnPath.FirstBlockingBuilding(out IntVec3 cellBeforeBlock, pawn);
                    if (blocker != null) return DigUtility.PassBlockerJob(pawn, blocker, cellBeforeBlock, false, true);
                }
            }

            target = (Pawn)GenClosest.ClosestThing_Global(pawn.Position, pawn.Map.mapPawns.AllPawnsSpawned, validator: delegate (Thing t) { return t.def != pawn.def; });
            if (target != null)
            {
                using (PawnPath pawnPath = pawn.Map.pathFinder.FindPath(pawn.Position, target.Position, TraverseParms.For(pawn, Danger.Deadly, TraverseMode.PassAllDestroyableThings)))
                {
                    if (!pawnPath.Found) return null;
                    Thing blocker = pawnPath.FirstBlockingBuilding(out IntVec3 cellBeforeBlock, pawn);
                    if (blocker != null) return DigUtility.PassBlockerJob(pawn, blocker, cellBeforeBlock, false, true);
                }
            }

            return null;
        }

        protected virtual Pawn FindAttackTarget(Pawn pawn)
		{
			TargetScanFlags targetScanFlags = TargetScanFlags.NeedAutoTargetable;
			return (Pawn)AttackTargetFinder.BestAttackTarget(pawn, targetScanFlags, (Thing x) => x is Pawn && pawn.HostileTo(x), 0f, 9999, default, float.MaxValue,
				true, true, true);
		}
	}
}
