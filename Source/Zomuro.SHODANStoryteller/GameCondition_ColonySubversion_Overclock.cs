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
			HashSet<Building> targets = DetermineAffected();
			foreach (var building in targets)
			{
				TurnOffBuilding(building);
			}
			affectedHacked = targets;
		}

		public override HashSet<Building> DetermineAffected()
		{
			return MapCompSubversion.Hacked.ToHashSet();
		}

		public override void GameConditionTick()
		{
			foreach(var building in affectedHacked)
            {
				// set interval for damage taken in settings
				if (building.IsHashIntervalTick(60)) building.TakeDamage(dinfo);
            }
		}

		public override void End()
		{
			base.End();
		}

		public DamageInfo dinfo = new DamageInfo(DamageDefOf.Flame, 1f, 1f);
	}
}
