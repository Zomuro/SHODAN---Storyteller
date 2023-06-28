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

		public override void RecheckAffected()
		{
			affectedHacked = DetermineAffected();
		}

		public override void End()
		{
			base.End();
		}
	}
}