using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using UnityEngine;


namespace Zomuro.SHODANStoryteller
{
    public class MapComponent_ColonySubversion : MapComponent
    {
        public MapComponent_ColonySubversion(Map map) : base(map)
        {
            this.map = map;
        }

        public override void MapComponentOnGUI()
        {
            // if the map isn't a player's home, don't bother showing.
            if (Find.Storyteller.def != StorytellerDefOf.Zomuro_SHODAN || map is null || !map.IsPlayerHome) return;

            Widgets.BeginGroup(new Rect(0, 0, 200, 200));

            Widgets.Label(new Rect(0, 0, 100, 100), "{0} / {1}".Formatted(hackedBuildings.Count(), allHackableBuildings.Count()));

            Widgets.EndGroup();
            return;

            // add settings for minimum hackable buildings count
            if (allHackableBuildings.NullOrEmpty() || allHackableBuildings.Count() < 7) return;

            
        }

        public float ControlPercentage
        {
            get
            {
                if (allHackableBuildings.NullOrEmpty()) return -1f;
                return hackedBuildings.Count() / allHackableBuildings.Count();
            }
        }

        public bool AddHackable(Building building)
        {
            if (!building.Map.IsPlayerHome || !CheckHackable(building)) return false;
            allHackableBuildings.Add(building);
            Log.Message("Building is hackable: " + building.def.label);
            return true;
        }

        public bool CheckHackable(Building building)
        {
            // potentially useful, but unneeded
            //building.Map.powerNetManager.UpdatePowerNetsAndConnections_First();
            CompPowerTrader power = building.TryGetComp<CompPowerTrader>();
            //if (power is null) Log.Message(building.def.label + " has no power.");
            //if(power != null && power.PowerNet is null) Log.Message(building.def.label + " has no powernet.");
            //&& power.PowerNet != null
            if (power != null ) return true;

            return false;
        }

        public void RemoveBuilding(Building building)
        {
            hackedBuildings.Remove(building);
            allHackableBuildings.Remove(building);
        }

        public void CleanAll()
        {
            hackedBuildings.Clear();
            allHackableBuildings.Clear();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref hackedBuildings, "hackedBuildings", LookMode.Reference, Array.Empty<object>());
            Scribe_Collections.Look(ref allHackableBuildings, "allHackableBuildings", LookMode.Reference, Array.Empty<object>());
        }

        public IEnumerable<Building> PoweredOnHackables
        {
            get
            {
                return allHackableBuildings.Where(x => x.PowerComp.Props.idlePowerDraw > 0);
            }
        }

        

        public List<Building> hackedBuildings = new List<Building>();

        public List<Building> allHackableBuildings = new List<Building>();


    }
}
