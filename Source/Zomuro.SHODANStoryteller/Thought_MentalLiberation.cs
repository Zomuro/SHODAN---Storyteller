using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace Zomuro.SHODANStoryteller
{
	public class Thought_MentalLiberation : Thought_Situational
	{

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


	}
}
