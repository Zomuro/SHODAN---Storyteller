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

            if (Hackable.Count() < 5) Widgets.Label(new Rect(0, 0, 100, 100), "Not enough buildings");
            else Widgets.Label(new Rect(0, 0, 100, 100), "{0} / {1}".Formatted(Hacked.Count(), Hackable.Count()));

            Widgets.EndGroup();
            return;

            // add settings for minimum hackable buildings count
            if (potentialHackable.EnumerableNullOrEmpty() || potentialHackable.Count() < 7) return;

            
        }

        public float ControlPercentage
        {
            get
            {
                if (Hackable.EnumerableNullOrEmpty()) return -1f;
                return Hacked.Count() / Hackable.Count();
            }
        }

        public bool CanFireIncidentCheck()
        {
            if (!Hackable.EnumerableNullOrEmpty() && Hacked.Count() <= Hackable.Count()) return true;
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
            RemoveFromGameCondition(building);
            //building.Map.powerNetManager.UpdatePowerNetsAndConnections_First(); // ?? check, since base consumption will be affected
            dirtyHacked = true;
            
        }

        public void RemoveBuilding(Building building) // used when building is destroyed- force the building to be removed from potential
        {
            potentialHacked.Remove(building);
            potentialHackable.Remove(building);
            CleanCache();
            //ClearGameConditionCache();
            RemoveFromGameCondition(building);
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
            //Scribe_References.Look(ref map, "map");
            Scribe_Collections.Look(ref potentialHacked, "potentialHacked", LookMode.Reference);
            Scribe_Collections.Look(ref potentialHackable, "potentialHackable", LookMode.Reference);
        }

        public IEnumerable<Building> Hacked
        {
            get
            {
                if (cachedHacked is null || dirtyHacked)
                {
                    cachedHacked = potentialHacked.Where(CheckActivePowerNet);
                    dirtyHacked = false;
                }

                return cachedHacked;
            }
        }

        public IEnumerable<Building> Hackable
        {
            get
            {
                if(cachedHackable is null || dirtyHackable)
                {
                    cachedHackable = potentialHackable.Where(CheckActivePowerNet);
                    dirtyHackable = false;
                }

                return cachedHackable;
            }
        }

        public bool CheckActivePowerNet(Building building)
        {
            if (building.GetComp<CompPowerTrader>().PowerNet is PowerNet pn && pn != null && pn.hasPowerSource) return true;
            return false;
        }


        public GameCondition_ColonySubversion_LightSap GameConditionLightSap
        {
            get
            {
                if (cachedLightSap is null && map.IsPlayerHome)
                {
                    cachedLightSap = map.GameConditionManager.GetActiveCondition<GameCondition_ColonySubversion_LightSap>();
                }
                return cachedLightSap;
            }
        }

        public GameCondition_ColonySubversion_Production GameConditionProduction
        {
            get
            {
                if (cachedProd is null && map.IsPlayerHome)
                {
                    cachedProd = map.GameConditionManager.GetActiveCondition<GameCondition_ColonySubversion_Production>();
                }
                return cachedProd;
            }
        }

        public GameCondition_ColonySubversion_Overclock GameConditionOverclock
        {
            get
            {
                if (cachedOverclock is null && map.IsPlayerHome)
                {
                    cachedOverclock = map.GameConditionManager.GetActiveCondition<GameCondition_ColonySubversion_Overclock>();
                }
                return cachedOverclock;
            }
        }

        public GameCondition_ColonySubversion_TempSuspend GameConditionTempSuspend
        {
            get
            {
                if (cachedTemp is null && map.IsPlayerHome)
                {
                    cachedTemp = map.GameConditionManager.GetActiveCondition<GameCondition_ColonySubversion_TempSuspend>();
                }
                return cachedTemp;
            }
        }

        public HashSet<Building> AggregGameCondition()
        {
            if (cachedTotalAffected is null && map.IsPlayerHome)
            {
                HashSet<Building> total = new HashSet<Building>();
                total.AddRange(GameConditionLightSap?.affectedHacked ?? new HashSet<Building>());
                total.AddRange(GameConditionProduction?.affectedHacked ?? new HashSet<Building>());
                total.AddRange(GameConditionOverclock?.affectedHacked ?? new HashSet<Building>());
                total.AddRange(GameConditionTempSuspend?.affectedHacked ?? new HashSet<Building>());
                cachedTotalAffected = total;
            }

            return cachedTotalAffected;
        }

        public void RemoveFromGameCondition(Building building)
        {
            GameConditionLightSap?.affectedHacked.Remove(building);
            GameConditionProduction?.affectedHacked.Remove(building);
            GameConditionOverclock?.affectedHacked.Remove(building);
            GameConditionTempSuspend?.affectedHacked.Remove(building);
            building.SetFactionDirect(Faction.OfPlayer);
            cachedTotalAffected = null;
        }

        public void ClearGameConditionCache()
        {
            cachedLightSap = null;
            cachedProd = null;
            cachedOverclock = null;
            cachedTemp = null;
            cachedTotalAffected = null;
        }

        public HashSet<Building> potentialHacked = new HashSet<Building>();

        public HashSet<Building> potentialHackable = new HashSet<Building>();

        private IEnumerable<Building> cachedHacked;

        private IEnumerable<Building> cachedHackable;

        public GameCondition_ColonySubversion_LightSap cachedLightSap;

        public GameCondition_ColonySubversion_Production cachedProd;

        public GameCondition_ColonySubversion_Overclock cachedOverclock;

        public GameCondition_ColonySubversion_TempSuspend cachedTemp;

        public HashSet<Building> cachedTotalAffected;

        public bool dirtyHacked = true;

        public bool dirtyHackable = true;

        

    }
}
