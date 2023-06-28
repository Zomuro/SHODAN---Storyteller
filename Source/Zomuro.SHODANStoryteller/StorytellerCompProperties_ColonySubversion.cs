using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace Zomuro.SHODANStoryteller
{
	public class StorytellerCompProperties_ColonySubversion : StorytellerCompProperties
	{
		// Token: 0x060056AC RID: 22188 RVA: 0x001D2C82 File Offset: 0x001D0E82
		public StorytellerCompProperties_ColonySubversion()
		{
			compClass = typeof(StorytellerComp_ColonySubversion);
		}

		
		// not necessarily - implement mtbdays for certain incidents in settings
		public float mtbDays = -1f;

		public IncidentDef hackingIncident;

		public List<IncidentDef> subversionIncidents;
	}
}
