﻿using System;
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
			if (Rand.MTBEventOccurs(StorytellerUtility.settings.MTBDaysHack, 60000f, 1000f))
			{
				IncidentParms parms = GenerateParms(Props.hackingIncident.category, target);
				yield return new FiringIncident(Props.hackingIncident, this, parms);
			}

			// add in setting for the subversion incident MTBdays
			if (Rand.MTBEventOccurs(StorytellerUtility.settings.MTBDaysSubversions, 60000f, 1000f))
			{
				//float control = StorytellerUtility.MapCompColonySubversion(target as Map).ControlPercentage;
				IncidentParms parms = new IncidentParms();
				parms.target = target;

				IEnumerable<IncidentDef> possible = Props.subversionIncidents;

                if (!possible.EnumerableNullOrEmpty())
                {
					IncidentDef incident = possible.Where(x => x.Worker.CanFireNow(parms)).RandomElementByWeight(x => x.baseChance);
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
