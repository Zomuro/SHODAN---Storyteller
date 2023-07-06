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
    public class GameCondition_ColonySubversion_Overclock : GameCondition_ColonySubversion
	{
		public override void Init()
		{
			base.Init();
			affectedHacked = DetermineAffected();
		}

		public override HashSet<Building> DetermineAffected()
		{
			return MapCompSubversion.Hacked.ToHashSet();
		}

		public override void GameConditionTick()
		{
			HashSet<Building> buildings = affectedHacked;
			foreach(var building in buildings)
            {
				// set interval for damage taken in settings
				if (building.IsHashIntervalTick(180)) 
				{
					GenTemperature.PushHeat(building, 3f); // put setting in here
					building.TakeDamage(dinfo);
					if (building.Destroyed) affectedHacked.Remove(building);
				}
            }
		}

		public override void End()
		{
			base.End();
		}

		public DamageInfo dinfo = new DamageInfo(DamageDefOf.Flame, 1f, 1f);
	}
}
