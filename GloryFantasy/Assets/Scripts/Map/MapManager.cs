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

        private static MapManager instance = null;

        public static MapManager getInstance()
        {
            return instance;
        }

        private MapManager()
        {
            _unitsList = new List<Unit>();
        }

        private void Awake()
        {
            instance = this;
            InitMap();
        }

        public void InitMap() {
            // TODO : 添加初始化地图的方法
            //for_demo_init();
            InitGameUnitsTemplateDictionary();
            InitAndInstantiateMapBlocks();
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
        public GameObject[] enemy_sets;         //存储敌方群体单位素材的数组
        public GameObject player_assete;       // 存放玩家单位素材的引用

        public GameObject player { get; set; }
        
        public Transform _tilesHolder;          // 存储所有地图单位引用的变量

        private List<Unit> _unitsList;

        private Dictionary<string, JsonData> _unitsData;    //存储所有单位类型的模板Json数据
        
        // 记录json中token不为空的坐标，待后续处理
        //private List <Vector3> specialPositions = new List <Vector3> ();

        
        // 初始化存储所有单元模板信息数据的字典，方便后续使用
        private void InitGameUnitsTemplateDictionary()
        {
            this._unitsData = new Dictionary<string, JsonData>();

            JsonData unitsTemplate =
                JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/Scripts/textToken.json"));

            int dataAmount = unitsTemplate.Count;
            for (int i = 0; i < dataAmount; i++)
            {
                _unitsData.Add(unitsTemplate[i]["id"].ToString(), unitsTemplate[i]);
            }
        }

        private void InitAndInstantiateMapBlocks()
        {
            // 更改地图数据位置则需修改此处路径
            JsonData mapData = JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/Scripts/Map/Beginning.json"));

            int mapDataCount = mapData.Count;
            this.columns = (int) mapData[mapDataCount - 1]["y"] + 1;
            this.rows = (int) mapData[mapDataCount - 1]["x"] + 1;
            
            _mapBlocks = new MapBlock[rows, columns];
            
            for( int i =0; i< mapDataCount;i++)
            {
                int x = (int)mapData[i]["x"];
                int y = (int)mapData[i]["y"];
                int area = (int) mapData[i]["area"];

                GameObject[] target_tiles = InstantiateTilesRuler(area);

                GameObject toInstantiate = target_tiles[Random.Range(0, target_tiles.Length)];
                GameObject _instance =
                    Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity);
                _instance.transform.SetParent(_tilesHolder);
                
                _instance.gameObject.AddComponent<MapBlock>();
                
                _mapBlocks[x, y] = _instance.gameObject.GetComponent<MapBlock>();
                _mapBlocks[x, y].area = area;
                _mapBlocks[x, y].x = x;
                _mapBlocks[x, y].y = y;
                int tokenCount = mapData[i]["token"].Count;
                
                if (tokenCount > 0)
                {
                    if (tokenCount == 1)
                    {
                        Unit unit = InitAndInstantiateGameUnit(mapData[i]["token"][0], x, y);
                        unit.mapBlockBelow = _mapBlocks[x, y];
                        _unitsList.Add(unit);
                        _mapBlocks[x, y].AddUnit(unit);
                    }
                    else
                    {
                        Unit[] units = InitAndInstantiateGameUnits(mapData[i]["token"], tokenCount, x, y);
                        for (int j = 0; j < units.Length; j++)
                        {
                            units[j].mapBlockBelow = _mapBlocks[x, y];
                            _unitsList.Add(units[j]);
                        }
                        _mapBlocks[x, y].AddUnits(units);
                    }
                }
            }
        }

        private void ReadUnitDataInJason(JsonData data, string owner, int damaged, Unit unit)
        {
            unit.Name = data["name"].ToString();
            unit.id = data["id"].ToString();
            unit.atk = (int) data["atk"];
            unit.hp = (int) data["hp"];
            unit.mov = (int) data["mov"];
            unit.rng = (int) data["rng"];
            unit.owner = owner;
            unit.priority = new List<int>();
            unit.priority.Add((int)data["priority"]);
            int tagCount = data["tag"].Count;
            if (tagCount > 0)
            {
                unit.tag = new string[tagCount];
                for (int i = 0; i < tagCount; i++)
                {
                    unit.tag[i] = data["tag"][i].ToString();
                }
                
            } 
        }

        private Unit[] InitAndInstantiateGameUnits(JsonData units, int count, int x, int y)
        {
            Unit[] res = new Unit[count];
            
            for (int i = 0; i < count; i++)
            {
                Vector2 position = new Vector2(x, y);
                position += Random.insideUnitCircle * 0.5f;
                
                GameObject enemy =
                    Instantiate(enemys[Random.Range(0, enemys.Length)], new Vector3(position.x, position.y, 0f),
                                Quaternion.identity);

                enemy.AddComponent<Unit>();
                res[i] = enemy.GetComponent<Unit>();
                
                ReadUnitDataInJason(this._unitsData[units[i]["name"].ToString()],
                                    units[i]["Controler - Enemy, Friendly or Self"].ToString(),
                                    (int)units[i]["Damaged"],
                                    res[i]);

                enemy.AddComponent<DisplayData>();
                enemy.AddComponent<ShowRange>();
                
            }
            
            return res;
        }

        private Unit InitAndInstantiateGameUnit(JsonData data, int x, int y)
        {
            Unit newUnit;
            GameObject _object;
            if (data["Controler - Enemy, Friendly or Self"].ToString().Equals("Self"))
            {
                this.player = _object =
                    Instantiate(player_assete, new Vector3(x, y, 0f), Quaternion.identity);
            }
            else
            {
                _object =
                    Instantiate(enemys[Random.Range(0, enemys.Length)], new Vector3(x, y, 0f),
                                Quaternion.identity);
                
            }
            // 在单位上挂载unit 脚本
            _object.AddComponent<Unit>();
            
            // 获取该脚本对象并传入解析json函数赋值
            newUnit = _object.GetComponent<Unit>();
            ReadUnitDataInJason(this._unitsData[data["name"].ToString()],
                                data["Controler - Enemy, Friendly or Self"].ToString(), 
                                (int)data["Damaged"],
                                newUnit);
            
            // 在单位上挂载展示数值显示脚本
            _object.AddComponent<DisplayData>();
            
            // 不管是不是player单位都挂载展示范围脚本
            _object.AddComponent<ShowRange>();
            
            return newUnit;
        }

        private GameObject[] InstantiateTilesRuler(int area)
        {
            // TODO : 根据规则（暂时不明）,下面是我编的,进行地图实例化
            switch (area)
            {
                case -1:
                    return other_tiles;
                case 0:
                case 1:
                    return normal_tiles;
                case 2:
                    return A_tiles;
                case 3:
                case 4:
                    return B_tiles;
                case 5:
                case 6:
                    return C_tiles;
                case 7:
                case 8:
                    return D_tiles;
                case 9:
                case 10:
                    return E_tiles;
                default:
                    Debug.Log("Unknown area type.");
                    return other_tiles;
            }
        }

        public MapBlock GetSpecificMapBlock(int x, int y)
        {
            return this._mapBlocks[x, y];
        }

        public Vector3 GetCoordinate(MapBlock mapBlock)
        {
            for(int i = columns - 1; i > 0; i--)
            {
                for(int j = 0; j< rows; j++)
                {
                    if(_mapBlocks[i,j] == mapBlock)
                    {
                        return new Vector3(i, j, 0f);
                    }
                }
            }
            return new Vector3(-1, -1, 0f);
        }

        private Boolean _CheckVector(Vector3 vector)
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
        public Boolean CheckIfHasUnits(Vector3 vector)
        {
            if (!_CheckVector(vector)) return false;
            //return this._mapBlocks[(int)vector.x, (int)vector.y].units_on_me != null;
            return this._mapBlocks[(int)vector.x, (int)vector.y].units_on_me.Count != 0;
        }

        // 返回给定坐标上单位list，坐标不合法会返回null, 其他依据实际情况返回值
        public List<Unit> GetUnitsOnMapBlock(Vector3 vector)
        {
            if (_CheckVector(vector))
                return _mapBlocks[(int) vector.x, (int) vector.y].units_on_me;
            return null;
        }

        // 根据给定unit寻找其所处坐标，若找不到则会返回不合法坐标
        public Vector3 GetUnitCoordinate(Unit unit)
        {

            foreach (Unit gameUnit in _unitsList)
            {
                if (gameUnit == unit)
                {
                    return gameUnit.mapBlockBelow.GetCoordinate();
                }
            }

            return new Vector3(-1, -1, -1);
        }

        public bool MoveUnitToCoordinate(Unit unit, Vector3 destination)
        {
            foreach (Unit gameUnit in _unitsList)
            {
                if (gameUnit == unit)
                {
                    unit.mapBlockBelow.RemoveUnit(unit);
                    unit.mapBlockBelow = _mapBlocks[(int)destination.x, (int)destination.y];
                    unit.mapBlockBelow.AddUnit(unit);
                    unit.transform.position = destination;
                    return true;
                }
            }

            return false;
        }

        // 地图方块染色接口
        public void ColorMapBlocks(List<Vector2> positions, Color color)
        {
            foreach (Vector2 position in positions)
            {
                _mapBlocks[(int) position.x, (int) position.y].gameObject.GetComponent<SpriteRenderer>().color = color;
            }
        }
    }
}