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
    public class MentalBreakWorker_CyberneticDomination : MentalBreakWorker
    {
		public override float CommonalityFor(Pawn pawn, bool moodCaused = false)
		{
			
			return 0; // cannot occur normally - intentional, as SHODAN will force this break via rng in a prefix
		}

		public override bool BreakCanOccur(Pawn pawn)
		{
			return pawn.IsColonist && pawn.Spawned && base.BreakCanOccur(pawn);
		}

	}
}
