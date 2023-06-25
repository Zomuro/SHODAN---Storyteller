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
	public class JobGiver_CyberneticDominationAttack : JobGiver_AIFightEnemies
	{
		// Thank you Killathon, for helping with the jobgiver code.
		protected override Thing FindAttackTarget(Pawn pawn)
		{
			TargetScanFlags targetScanFlags = TargetScanFlags.NeedReachableIfCantHitFromMyPos | TargetScanFlags.NeedAutoTargetable;
			return (Thing)AttackTargetFinder.BestAttackTarget(pawn, targetScanFlags, null, 0f, 9999, default, float.MaxValue);
		}
	}
}
