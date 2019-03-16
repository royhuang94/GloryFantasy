using UnityEngine;
using System;
using System.Collections.Generic;
using Unit = GameUnit.GameUnit;

namespace MapManager

{
    public enum Terrain{
        TERRAIN_GRASS,
        TERRAIN_HILL,
        TERRAIN_RIVER
        //TODO: 以上地形枚举只是随便写的，请依据需求改写
    }
    public class MapBlock : MonoBehaviour {
        public Terrain terrain;
        public Area area;
        public List <Unit> units_on_me = new List <Unit> ();

        public MapBlock(Terrain terrain, Area area) {
            this.terrain = terrain;
            this.area = area;
        }

        public MapBlock() {
            this.terrain = Terrain.TERRAIN_GRASS;
            this.area = Area.normal;
        }

        public void setTerrain(Terrain terrain) {
            this.terrain = terrain;
        }

        public void setArea(Area area) {
            this.area = area;
        }

        public Terrain GetTerrain() { return this.terrain; }
        public Area GetAreaNum() { return this.area; }
        public List<Unit> GetGameUnits() { return this.units_on_me; }

        public void addUnit(Unit unit) {
            Debug.Log("MapBlocks--Added unit:" + unit.ToString());
            units_on_me.Add(unit);
        }

        public void removeUnit(Unit unit) {
            Debug.Log("MapBlocks--Removed unit:" + unit.ToString());
            units_on_me.Remove(unit);
        }
    }
}