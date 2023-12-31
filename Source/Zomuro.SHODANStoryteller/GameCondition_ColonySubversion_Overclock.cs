﻿using System;
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
			HashSet<Building> buildings = new HashSet<Building>();
			buildings.AddRange(affectedHacked);
			//HashSet<Building> remove = new HashSet<Building>();
			foreach(var building in buildings)
            {
				// set interval for damage taken in settings
				// every three seconds, damage building and push heat
				if (building.IsHashIntervalTick(180)) 
				{
					GenTemperature.PushHeat(building, StorytellerUtility.settings.OverclockHeatPush); // put setting in here
					building.TakeDamage(dinfo);
				}
            }
		}

		public override void End()
		{
			base.End();
		}

		public DamageInfo dinfo = new DamageInfo(DamageDefOf.Flame, 3f, 1f);
	}
}
