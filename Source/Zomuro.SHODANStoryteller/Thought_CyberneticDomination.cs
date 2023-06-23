using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace Zomuro.SHODANStoryteller
{
	public class Thought_CyberneticDomination : Thought_Situational
	{
		public override int CurStageIndex
		{
			get
			{
				return FindIndex(BaseMoodOffset);
			}
		}

		public int FindIndex(float mood)
		{
			int index = 0;
			float compare;

			for (int i = def.stages.Count - 1; i >= 0; i--)
			{
				// potentially allow settings to scale the requirements for mood stages
				compare = def.stages[i].baseMoodEffect;
				index = i;
				if (mood <= compare) return index;
			}
			return index;
		}	

		public override string LabelCap
		{
			get
			{
				return CurStage.label.Translate().CapitalizeFirst();
			}
		}

		public override string Description
		{
			get
			{
				return CurStage.description.Translate().CapitalizeFirst();
			}
		}

		protected override float BaseMoodOffset
		{
			get
			{
				return CalculateTotalMoodOffset(pawn);
			}
		}


		public float CalculateTotalMoodOffset(Pawn pawn)
		{
			float total = 0;
			HashSet<Hediff> hediffs = pawn.health.hediffSet.hediffs.ToHashSet();
			if (hediffs.EnumerableNullOrEmpty()) return total;

			// add chance to adjust mood debuff in settings
			foreach (var hediff in hediffs) if (StorytellerUtility.TechImplantCheck(hediff)) total -= 3;
			
			return total;
		}


	}
}
