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
    public class GameCondition_ColonySubversion_LightSap : GameCondition_ColonySubversion
	{
		public override void Init()
		{
			base.Init();
			HashSet<Building> targets = DetermineAffected();
			foreach(var building in targets)
            {
				TurnOffBuilding(building);
			}
			affectedHacked = targets;
		}

		public override HashSet<Building> DetermineAffected()
		{
			return MapCompSubversion.Hacked.Where(x => x.TryGetComp<CompGlower>() != null && x.TryGetComp<CompTempControl>() is null).ToHashSet();
		}

		/*public override void RecheckAffected()
		{
			affectedHacked = DetermineAffected();
		}*/

		public override void End()
		{
			base.End();
			foreach (var building in affectedHacked)
			{
				if (building.TryGetComp<CompFlickable>() is CompFlickable comp && comp != null) comp.ResetToOn();
			}
			affectedHacked.Clear();
		}
	}
}
