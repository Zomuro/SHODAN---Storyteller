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

            Widgets.Label(new Rect(0, 0, 100, 100), "{0} / {1}".Formatted(Hacked.Count(), Hackables.Count()));

            Widgets.EndGroup();
            return;

            // add settings for minimum hackable buildings count
            if (potentialHackable.NullOrEmpty() || potentialHackable.Count() < 7) return;

            
        }

        public float ControlPercentage
        {
            get
            {
                if (Hackables.NullOrEmpty()) return -1f;
                return Hacked.Count() / Hackables.Count();
            }
        }

        public bool CanFireIncident()
        {
            if (!Hackables.NullOrEmpty() && potentialHacked.Count() < potentialHackable.Count()) return true;
            return false;
        }

        public bool AddHackable(Building building) // used when a building gets spawned
        {
            // check the map is player home AND has CompPowerTrader before adding to potential hackables
            if (!building.Map.IsPlayerHome || !CheckHackable(building)) return false;
            potentialHackable.Add(building);
            //building.Map.powerNetManager.UpdatePowerNetsAndConnections_First(); // update powernet for later
            dirtyHackable = true; // force recache of hackables linked to powernet
            //Log.Message("Building is hackable: " + building.def.label);
            return true;
        }

        public bool CheckHackable(Building building) // makes sure that the building has a CompPowerTrader
        {
            // potentially useful, but unneeded
            //building.Map.powerNetManager.UpdatePowerNetsAndConnections_First();
            CompPowerTrader power = building.TryGetComp<CompPowerTrader>();
            if (power != null) return true;
            return false;
        }

        public void UnhackBuilding(Building building) // used when pawns go through a reset job on a building
        {
            potentialHacked.Remove(building);
            //building.Map.powerNetManager.UpdatePowerNetsAndConnections_First(); // ?? check, since base consumption will be affected
            dirtyHacked = true;
        }

        public void RemoveBuilding(Building building) // used when building is destroyed- force the building to be removed from potential
        {
            

            //if (building.GetComp<CompPowerTrader>() is null) return;
            //building.Map.powerNetManager.UpdatePowerNetsAndConnections_First(); // ?? check, since base consumption will be affected
            potentialHacked.Remove(building);
            potentialHackable.Remove(building);

            CleanCache();

        }

        public void CleanAll()
        {
            potentialHacked.Clear();
            potentialHackable.Clear();
            CleanCache();
        }

        public void CleanCache()
        {
            dirtyHacked = true; 
            dirtyHackable = true;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref potentialHacked, "hackedBuildings", LookMode.Reference, Array.Empty<object>());
            Scribe_Collections.Look(ref potentialHackable, "potentialHackable", LookMode.Reference, Array.Empty<object>());
        }

        public List<Building> Hacked
        {
            get
            {
                if (cachedHacked is null || dirtyHacked)
                {
                    cachedHacked = potentialHacked.Where(x => x.GetComp<CompPowerTrader>().PowerNet != null).ToList();
                    dirtyHacked = false;
                }

                return cachedHacked;
            }
        }

        public List<Building> Hackables
        {
            get
            {
                if(cachedHackable is null || dirtyHackable)
                {
                    cachedHackable = potentialHackable.Where(x => x.GetComp<CompPowerTrader>().PowerNet != null).ToList();
                    dirtyHackable = false;
                }

                return cachedHackable;
            }
        }




        public List<Building> potentialHacked = new List<Building>();

        public List<Building> potentialHackable = new List<Building>();

        private List<Building> cachedHacked;

        private List<Building> cachedHackable;

        public bool dirtyHacked = true;

        public bool dirtyHackable = true;

        

    }
}
