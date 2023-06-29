using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using Verse.Grammar;

namespace Zomuro.SHODANStoryteller
{
    public class GameCondition_ColonySubversion_IFF : GameCondition_ColonySubversion
	{
		public override void Init()
		{
			base.Init();
			//affectedHacked = DetermineAffected();
			foreach(var building in DetermineAffected())
            {
				building.SetFactionDirect(Find.FactionManager.FirstFactionOfDef(FactionDefOf.Zomuro_SHODAN_Faction));
            }
		}

		public override HashSet<Building> DetermineAffected()
		{
			return MapCompSubversion.Hacked.Where(x => x.def.thingClass == typeof(Building_TurretGun)).ToHashSet();
		}

		public override void End()
		{
			base.End();
			foreach (var building in affectedHacked)
			{
				building.SetFactionDirect(Find.FactionManager.OfPlayer);
			}
		}
	}
}
