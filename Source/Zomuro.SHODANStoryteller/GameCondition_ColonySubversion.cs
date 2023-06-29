using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using Verse.Grammar;
using HarmonyLib;

namespace Zomuro.SHODANStoryteller
{
    public class GameCondition_ColonySubversion : GameCondition
    {
		public override void Init()
		{
			base.Init();

		}

		public virtual HashSet<Building> DetermineAffected()
        {
			return null;
		}

		public virtual void TurnOffBuilding(Building building)
        {
			CompFlickable flick = (CompFlickable) Traverse.Create(building?.TryGetComp<CompPowerTrader>())?.Field("flickableComp")?.GetValue();
			if (flick != null)
            {
				Traverse flickTrav = Traverse.Create(flick);
				flick.parent.BroadcastCompSignal("FlickedOff");
				flickTrav.Field("wantSwitchOn").SetValue(false);
				flickTrav.Field("switchOnInt").SetValue(false);
			}
		}

		/*public virtual void RecheckAffected()
		{
			
		}*/

		public void RemoveBuilding(Building b)
		{
			affectedHacked.Remove(b);
		}

		public override void End()
		{
			base.End();
			//MapCompSubversion.ClearGameConditionCache();
		}

		// expire the game condition if its duration is over OR if the map its for isn't a player home
		public override bool Expired 
		{
			get
			{
				return base.Expired || !SingleMap.IsPlayerHome;
			}
		}

		public override void ExposeData()
		{
			Scribe_Collections.Look(ref affectedHacked, "affectedHacked", LookMode.Reference);
			base.ExposeData();
		}

		// nabs mapcomp for methods and reference list when initalizing
		public MapComponent_ColonySubversion MapCompSubversion 
        {
            get
            {
				if(cachedMapComp is null)
                {
					cachedMapComp = StorytellerUtility.MapCompColonySubversion(SingleMap);
                }

				return cachedMapComp;
			}
        }

		// check for relevant querycomp extension, which will help us target relevant buildings with the right comp
		/*public bool ExtensionCheck(out GameCondition_QueryComp_ModExtension extension)
        {
			if(def.HasModExtension<GameCondition_QueryComp_ModExtension>())
            {
				extension = def.GetModExtension<GameCondition_QueryComp_ModExtension>();
				return true;
			}

			extension = null;
			return false;
        }*/

		public HashSet<Building> affectedHacked;

		private MapComponent_ColonySubversion cachedMapComp;


	}
}
