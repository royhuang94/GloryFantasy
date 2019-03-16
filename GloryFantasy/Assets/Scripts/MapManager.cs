using UnityEngine;
using System;
using System.Collections.Generic;
using MapManager;
using Random = UnityEngine.Random;

namespace MapManager

{
    public enum Area{
        A=1,
        B,
        C,
        D,
        E,
        normal,
        other
    }

    public class MapManager : MonoBehaviour {

        private static MapManager instance = new MapManager();

        public static MapManager getInstance() { return instance; }

        private MapManager() {
            // TODO : 添加初始化地图的方法
            for_demo_init();
            InitMapBlocks();
            InstantiateTiles();
        }


        public int columns = 8;                 // 地图方块每行的数量
        public int rows = 8;                    // 地图方块每列的数量

        private MapBlock[,] mapBlocks;
        public GameObject[] A_tiles;            // 区域 A prefabs的数组
        public GameObject[] B_tiles;            // 区域 B prefabs的数组
        public GameObject[] C_tiles;            // 区域 C prefabs的数组
        public GameObject[] D_tiles;            // 区域 D prefabs的数组
        public GameObject[] E_tiles;            // 区域 E prefabs的数组

        public GameObject[] normal_tiles;       // 白色区域 prefabs的数组
        public GameObject[] other_tiles;        // 黑色区域 prefabs的数组

        public GameObject[] enemys;             // 存储敌方单位素材的数组

        private GameObject[][] all_tiles;

        private Transform tilesHolder;          // 存储所有地图单位引用的变量

        private Area[,] map;
        
        // 记录怪物出现的坐标
        private List <Vector3> gridPositions = new List <Vector3> ();

        // 地图区域标注--为demo准备的
        private void for_demo_init()
        {
            map = new Area[,]
            {
                {Area.A,      Area.A,       Area.A,     Area.normal, Area.normal, Area.C, Area.C, Area.C},
                {Area.A,      Area.A,       Area.A,     Area.normal, Area.normal, Area.C, Area.C, Area.C},
                {Area.A,      Area.A,       Area.B,     Area.B,      Area.B,      Area.C, Area.C, Area.C},
                {Area.A,      Area.A,       Area.B,     Area.B,      Area.B,      Area.C, Area.C, Area.C},
                {Area.normal, Area.normal,  Area.other, Area.B,      Area.B,      Area.C, Area.C, Area.C},
                {Area.normal, Area.normal,  Area.other, Area.other,  Area.E,      Area.E, Area.E, Area.E},
                {Area.D,      Area.D,       Area.D,     Area.other,  Area.E,      Area.E, Area.E, Area.E},
                {Area.D,      Area.D,       Area.D,     Area.normal, Area.E,      Area.E, Area.E, Area.E}
            };
        }

        private void InitMapBlocks() {
            this.mapBlocks = new MapBlock[columns,rows];
            for(int i = columns-1; i > 0; i--) {
                for(int j = 0; j < rows; j++) {
                    this.mapBlocks[i, j] = new MapBlock(Terrain.TERRAIN_GRASS, map[i, j]);
                }
            }
        }

        private void InstantiateTiles()
        {
            all_tiles = new GameObject[][] {
                A_tiles,
                B_tiles,
                C_tiles,
                D_tiles,
                E_tiles,
                normal_tiles,
                other_tiles
            };

            for(int i = columns - 1; i > 0; i--)
            {
                for(int j = 0; j < rows; j++)
                {
                    GameObject[] target_tile = all_tiles[(int)map[i, j]];
                    GameObject toInstantiate = target_tile[Random.Range(0, target_tile.Length)];
                    GameObject instance =
                        Instantiate(toInstantiate, new Vector3(i, j, 0f), Quaternion.identity) as GameObject;

                    instance.transform.SetParent(tilesHolder);
                }
            }
        }

        public MapBlock GetSpecificMapBlock(int x, int y)
        {
            return this.mapBlocks[x, y];
        }

        public Vector2 GetCoordinate(MapBlock mapBlock)
        {
            for(int i = columns - 1; i > 0; i--)
            {
                for(int j = 0; j< rows; j++)
                {
                    if(mapBlocks[i,j] == mapBlock)
                    {
                        return new Vector2(i, j);
                    }
                }
            }
            return new Vector2(-1, -1);
        }

    }
}