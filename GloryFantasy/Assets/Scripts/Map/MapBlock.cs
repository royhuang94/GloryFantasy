using UnityEngine;
using System;
using System.Collections.Generic;
using Unit = GameUnit.GameUnit;

namespace MapManager

{
    public class MapBlock: MonoBehaviour{
        public int area { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public string[] data { get; set; }
        public List <Unit> units_on_me = new List <Unit> ();

        public MapBlock(int area)
        {
            this.area = area;
            this.data = null;
        }

        public MapBlock(int area, string[] data) {
            this.area = area;
            this.data = data;
        }

        public MapBlock() {
            this.area = 0;
            this.data = null;
        }
        
        public List<Unit> GetGameUnits() { return this.units_on_me; }

        public void AddUnit(Unit unit) {
            Debug.Log("MapBlocks--Added unit:" + unit.ToString());
            units_on_me.Add(unit);
        }

        public void AddUnits(Unit[] units)
        {
            Debug.Log("MapBlocks--Adding Units");
            foreach (Unit gameUnit in units)
            {
                units_on_me.Add(gameUnit);
            }
        }

        public void RemoveUnit(Unit unit) {
            Debug.Log("MapBlocks--Removed unit:" + unit.ToString());
            units_on_me.Remove(unit);
        }

        public Vector3 GetCoordinate()
        {
            return new Vector3(this.x, this.y, 0f);
        }
    }
}