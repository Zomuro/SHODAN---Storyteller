using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace Zomuro.SHODANStoryteller
{
	public class ThoughtWorker_CyberneticDomination: ThoughtWorker
	{
		protected override ThoughtState CurrentStateInternal(Pawn p)
		{
			HashSet<Hediff> hediffs = p.health.hediffSet.hediffs.ToHashSet();
			if (Find.Storyteller.def != StorytellerDefOf.Zomuro_SHODAN || hediffs.EnumerableNullOrEmpty() || 
				hediffs.FirstOrDefault(x => StorytellerUtility.TechImplantCheck(x)) is null) 
				return false;

			return true;
		}
	}
}
