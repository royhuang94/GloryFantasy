using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using LitJson;
using System.IO;
using Unit = GameUnit.GameUnit;

namespace MapManager

{
    public class MapManager : MonoBehaviour {

        private static MapManager instance = new MapManager();

        public static MapManager getInstance() { return instance; }

        private MapManager()
        {
            
        }

        private void Awake()
        {
            InitMap();
        }

        public void InitMap() {
            // TODO : 添加初始化地图的方法
            //for_demo_init();
            InitMapBlocks();
            InstantiateTiles();
        }


        public int columns = 8;                 // 地图方块每列的数量
        public int rows = 8;                    // 地图方块每行的数量

        private MapBlock[,] _mapBlocks;
        public GameObject[] A_tiles;            // 区域 A prefabs的数组
        public GameObject[] B_tiles;            // 区域 B prefabs的数组
        public GameObject[] C_tiles;            // 区域 C prefabs的数组
        public GameObject[] D_tiles;            // 区域 D prefabs的数组
        public GameObject[] E_tiles;            // 区域 E prefabs的数组

        public GameObject[] normal_tiles;       // 白色区域 prefabs的数组
        public GameObject[] other_tiles;        // 黑色区域 prefabs的数组

        public GameObject[] enemys;             // 存储敌方单位素材的数组

        private Transform _tilesHolder;          // 存储所有地图单位引用的变量
        
        // 记录json中token不为空的坐标，待后续处理
        private List <Vector3> specialPositions = new List <Vector3> ();

        private void InitMapBlocks()
        {
            // 更改地图数据位置则需修改此处路径
            JsonData mapData = JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/Scripts/Map/测试地图.json"));

            _mapBlocks = new MapBlock[columns, rows];
            for( int i =0; i< mapData.Count;i++)
            {
                int x = (int)mapData[i]["x"];
                int y = (int)mapData[i]["y"];
                _mapBlocks[x, y] = new MapBlock((int)mapData[i]["area"]);

                int tokenCount = mapData[i]["token"].Count;
                if (tokenCount > 0)
                {
                    specialPositions.Add(new Vector3(x, y, 0f));
                    //_mapBlocks[x, y].data = new string[tokenCount];
                    for ( int j =0; j < tokenCount; j++)
                        _mapBlocks[x, y].addUnit(InitGameUnit(mapData[i]["token"][j]));
                }
            }
        }

        private Unit InitGameUnit(JsonData unit)
        {
            Unit newUnit = gameObject.AddComponent<Unit>();
            newUnit.Name = unit["name"].ToString();
            newUnit.id = unit["id"].ToString();
            newUnit.cost = (int) unit["cost"];
            newUnit.atk = (int) unit["atk"];
            newUnit.def = (int) unit["def"];
            newUnit.mov = (int) unit["mov"];
            newUnit.rng = (int) unit["rng"];
            newUnit.owner = unit["owner"].ToString();
            newUnit.ralatedCardID = (int)unit["ralatedCardID"];
            /*
            Unit newUnit = new Unit(
                unit["name"].ToString(),
                unit["id"].ToString(),
                (int) unit["cost"],
                (int) unit["atk"],
                (int) unit["def"],
                (int) unit["mov"],
                (int) unit["rng"],
                unit["owner"].ToString(),
                (int) unit["ralatedCardID"]
            );*/
            string[] labes = {"tag", "triggered", "active"};
            for (int i = 0; i < labes.Length; i++)
            {
                int count = unit[labes[i]].Count;
                if (count > 0)
                {
                    string[] data = new string[count];
                    for (int j = 0; j < count; j++)
                    {
                        data[j] = unit[labes[i]][j].ToString();
                    }

                    switch (i)
                    {
                        case 0:
                            newUnit.tag = data;
                            break;
                        case 1:
                            newUnit.triggered = data;
                            break;
                        case 2:
                            newUnit.active = data;
                            break;
                        default:
                            Debug.Log("detected wrong index");
                            break;
                    }
                }

            }

            return newUnit;
        }

        private void InstantiateTiles()
        {
            // TODO : 根据规则（暂时不明）,下面是我编的,进行地图实例化
            for(int i = rows - 1; i > 0; i--)
            {
                for(int j = 0; j < columns; j++)
                {
                    GameObject[] target_tile = null;
                    switch (_mapBlocks[i, j].area)
                    {
                        case -1:
                            target_tile = other_tiles;
                            break;
                        case 1:
                            target_tile = normal_tiles;
                            break;
                        case 2:
                            target_tile = A_tiles;
                            break;
                        case 3:
                        case 4:
                            target_tile = B_tiles;
                            break;
                        case 5:
                        case 6:
                            target_tile = C_tiles;
                            break;
                        case 7:
                        case 8:
                            target_tile = D_tiles;
                            break;
                        case 9:
                        case 10:
                            target_tile = E_tiles;
                            break;
                        default:
                            Debug.Log("Unknown area type.");
                            break;
                    }

                    if (target_tile == null)
                        return;
                    
                    GameObject toInstantiate = target_tile[Random.Range(0, target_tile.Length)];
                    GameObject _instance =
                        Instantiate(toInstantiate, new Vector3(i, j, 0f), Quaternion.identity) as GameObject;

                    _instance.transform.SetParent(_tilesHolder);
                }
            }
        }

        public MapBlock GetSpecificMapBlock(int x, int y)
        {
            return this._mapBlocks[x, y];
        }

        public Vector2 GetCoordinate(MapBlock mapBlock)
        {
            for(int i = columns - 1; i > 0; i--)
            {
                for(int j = 0; j< rows; j++)
                {
                    if(_mapBlocks[i,j] == mapBlock)
                    {
                        return new Vector2(i, j);
                    }
                }
            }
            return new Vector2(-1, -1);
        }

        private Boolean _CheckVector(Vector2 vector)
        {
            if ((int) vector.x > rows || (int) vector.y > columns)
            {
                Debug.Log(string.Format("Invalid coordinate: {0}, {1} !", (int)vector.x, (int)vector.y));
                return false;
            }

            if ((int) vector.x < 0 || (int) vector.y < 0)
            {
                Debug.Log(string.Format("Error ! Outranged coordinate: {0}, {1} !", (int)vector.x, (int)vector.y));
                return false;
            }

            return true;
        }
        
        // 确定给定坐标上是否含有单位，坐标不合法会返回false，其他依据实际情况返回值
        public Boolean CheckIfHasUnits(Vector2 vector)
        {
            if (!_CheckVector(vector)) return false;
            return this._mapBlocks[(int)vector.x, (int)vector.y].units_on_me == null;
        }

        // 返回给定坐标上单位list，坐标不合法会返回null, 其他依据实际情况返回值
        public List<Unit> GetUnitsOnMapBlock(Vector2 vector)
        {
            if (_CheckVector(vector))
                return _mapBlocks[(int) vector.x, (int) vector.y].units_on_me;
            return null;
        }
    }
}