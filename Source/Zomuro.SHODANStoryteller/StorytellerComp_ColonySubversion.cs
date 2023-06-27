using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace Zomuro.SHODANStoryteller
{
	public class StorytellerComp_ColonySubversion : StorytellerComp
	{
		// Token: 0x17000EDE RID: 3806
		// (get) Token: 0x060056A8 RID: 22184 RVA: 0x001D2C41 File Offset: 0x001D0E41
		protected StorytellerCompProperties_ColonySubversion Props
		{
			get
			{
				return (StorytellerCompProperties_ColonySubversion) props;
			}
		}

		public override IEnumerable<FiringIncident> MakeIntervalIncidents(IIncidentTarget target)
		{
			// add in setting for the hacking incident MTBdays
			if (Rand.MTBEventOccurs(1.5f, 60000f, 1000f))
			{
				IncidentParms parms = GenerateParms(Props.hackingIncident.category, target);
				yield return new FiringIncident(Props.hackingIncident, this, parms);
			}

			// add in setting for the subversion incident MTBdays
			if (Rand.MTBEventOccurs(5f, 60000f, 1000f))
			{
				float control = StorytellerUtility.MapCompColonySubversion(target as Map).ControlPercentage;

				IEnumerable<IncidentDef> possible = Props.subversionIncidents.Where(x => x.controlReq <= control).Select(x => x.incidentDef);

                if (!possible.EnumerableNullOrEmpty())
                {
					IncidentDef incident = possible.RandomElementByWeight(x => x.baseChance);
					IncidentParms parms = GenerateParms(incident.category, target);
					yield return new FiringIncident(incident, this, parms);
				}
			}

			yield break;
		}

		public override string ToString()
		{
			return base.ToString();
		}
	}
}
