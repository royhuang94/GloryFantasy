using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using LitJson;
using System.IO;
using Unit = GameUnit.GameUnit;
using UnityEngine.UI;

namespace BattleMapManager

{
    public class BattleMapManager : MonoBehaviour {
        private static BattleMapManager instance = null;

        public static BattleMapManager getInstance()
        {
            return instance;
        }

        private BattleMapManager()
        {
            _unitsList = new List<Unit>();
        }

        //获取地图块上的单位
        public List<Unit> UnitsList
        {
            get
            {
                return _unitsList;
            }
        }


        private void Awake()
        {
            instance = this;
            InitMap();
        }

        public void InitMap()
        {
            // TODO : 添加初始化地图的方法
            //for_demo_init();
            InitGameUnitsTemplateDictionary();
            InitAndInstantiateMapBlocks();
        }

        #region 变量
        private int columns;                 // 地图方块每列的数量
        private int rows;                    // 地图方块每行的数量
        public int Columns
        {
            get
            {
                return columns;
            }
        }
        public int Rows
        {
            get
            {
                return rows;
            }
        }                    // 地图方块每行的数量
        public int BlockCount
        {
            get
            {
                return columns * rows;
            }
        }

        public Vector3 curMapPos;
        private BattleMapBlock[,] _mapBlocks;         //普通的地图方块
        private BattleMapBlock[,] _mapBlocksBurning;//灼烧的地图方块
        public GameObject normalMapBlocks;
        private Dictionary<Vector2, BattleMapBlock> mapBlockDict = new Dictionary<Vector2, BattleMapBlock>();//寻路字典

        #region 弃用
        //public GameObject[] A_tiles;            // 区域 A prefabs的数组
        //public GameObject[] B_tiles;            // 区域 B prefabs的数组
        //public GameObject[] C_tiles;            // 区域 C prefabs的数组
        //public GameObject[] D_tiles;            // 区域 D prefabs的数组
        //public GameObject[] E_tiles;            // 区域 E prefabs的数组

        //public GameObject[] normal_tiles;       // 白色区域 prefabs的数组
        //public GameObject[] other_tiles;        // 黑色区域 prefabs的数组
        #endregion

        #region 战区块
        private List<Vector2> battleArea_1;
        private List<Vector2> battleArea0;
        private List<Vector2> battleArea1;
        private List<Vector2> battleArea2;
        private List<Vector2> battleArea3;
        private List<Vector2> battleArea4;
        private List<Vector2> battleArea5;
        private List<Vector2> battleArea6;
        private List<Vector2> battleArea7;
        public List<Vector2> BattleArea_1 { get { return battleArea_1; } }
        public List<Vector2> BattleArea0 { get { return battleArea0; } }
        public List<Vector2> BattleArea1 { get { return battleArea1; } }
        public List<Vector2> BattleArea2 { get { return battleArea2; } }
        public List<Vector2> BattleArea3 { get { return battleArea3; } }
        public List<Vector2> BattleArea4 { get { return battleArea4; } }
        public List<Vector2> BattleArea5 { get { return battleArea5; } }
        public List<Vector2> BattleArea6 { get { return battleArea6; } }
        public List<Vector2> BattleArea7 { get { return battleArea7; } }
        #endregion

        public GameObject[] enemys;             // 存储敌方单位素材的数组
        public GameObject[] enemy_sets;         //存储敌方群体单位素材的数组
        public GameObject player_assete;       // 存放玩家单位素材的引用
        public GameObject obstacle;


        public GameObject player { get; set; }

        public Transform _tilesHolder;          // 存储所有地图单位引用的变量

        private List<Unit> _unitsList;

        private Dictionary<string, JsonData> _unitsData;    //存储所有单位类型的模板Json数据
        public Dictionary<string, JsonData> UnitData
        {
            get
            {
                return _unitsData;
            }
        }
#endregion

        // 记录json中token不为空的坐标，待后续处理
        //private List <Vector3> specialPositions = new List <Vector3> ();


        // 初始化存储所有单元模板信息数据的字典，方便后续使用
        private void InitGameUnitsTemplateDictionary()
        {
            this._unitsData = new Dictionary<string, JsonData>();

            JsonData unitsTemplate =
                JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/Scripts/textToken2.json"));

            int dataAmount = unitsTemplate.Count;
            for (int i = 0; i < dataAmount; i++)
            {
                _unitsData.Add(unitsTemplate[i]["id"].ToString(), unitsTemplate[i]);
            }
        }

        #region  初始地图相关
        private void InitAndInstantiateMapBlocks()
        {
            // 更改地图数据位置则需修改此处路径
            JsonData mapData = JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/Scripts/BattleMap/eg.json"));

            int mapDataCount = mapData.Count;
            this.columns = (int)mapData[mapDataCount - 1]["y"] + 1;
            this.rows = (int)mapData[mapDataCount - 1]["x"] + 1;

            _mapBlocks = new BattleMapBlock[rows, columns];
            GameObject _instance = new GameObject();

            battleArea_1 = new List<Vector2>();
            battleArea0 = new List<Vector2>();
            battleArea1 = new List<Vector2>();
            battleArea2 = new List<Vector2>();
            battleArea3 = new List<Vector2>();
            battleArea4 = new List<Vector2>();
            battleArea5 = new List<Vector2>();
            battleArea6 = new List<Vector2>();
            battleArea7 = new List<Vector2>();

            int x = 0;
            int y = 0;
            int area = 0;
            
            
            for (int i = 0; i < mapDataCount; i++)
            {
                x = (int)mapData[i]["x"];
                y = (int)mapData[i]["y"];
                area = (int)mapData[i]["area"];
                Vector2 mapPos = new Vector2(x, y);

                #region 存储同一战区的地图块
                switch (area)
                {
                    case -1:
                        battleArea_1.Add(mapPos);
                        break;
                    case 0:
                        battleArea0.Add(mapPos);
                        break;
                    case 1:
                        battleArea1.Add(mapPos);
                        break;
                    case 2:
                        battleArea2.Add(mapPos);
                        break;
                    case 3:
                        battleArea3.Add(mapPos);
                        break;
                    case 4:
                        battleArea4.Add(mapPos);
                        break;
                    case 5:
                        battleArea5.Add(mapPos);
                        break;
                    case 6:
                        battleArea6.Add(mapPos);
                        break;
                    case 7:
                        battleArea7.Add(mapPos);
                        break;
                    default:
                        break;
                }
                #endregion
               

                //实例化地图块
                _instance = GameObject.Instantiate(normalMapBlocks, new Vector3(x, y, 0f), Quaternion.identity);
                _instance.transform.SetParent(_tilesHolder);


                if (x == 0 && y == 0)
                {
                    Debug.Log(string.Format("固定的灼烧块{0},{0}", x, y));
                    _instance.gameObject.AddComponent<BattleMapBlockBurning>();

                    _instance.gameObject.AddComponent<BattleMapBlock>();
                    _mapBlocks[x, y] = _instance.gameObject.GetComponent<BattleMapBlock>();
                    _mapBlocks[x, y].area = area;
                    _mapBlocks[x, y].x = x;
                    _mapBlocks[x, y].y = y;
                    _mapBlocks[x, y].aStarState = AStarState.free;
                    _mapBlocks[x, y].blockType = EMapBlockType.Normal;
                    mapBlockDict.Add(new Vector2(x, y), _mapBlocks[x, y]);//寻路字典

                }
                else
                {
                    _instance.gameObject.AddComponent<BattleMapBlock>();
                    _mapBlocks[x, y] = _instance.gameObject.GetComponent<BattleMapBlock>();
                    _mapBlocks[x, y].area = area;
                    _mapBlocks[x, y].x = x;
                    _mapBlocks[x, y].y = y;
                    _mapBlocks[x, y].aStarState = AStarState.free;
                    _mapBlocks[x, y].blockType = EMapBlockType.Normal;
                    mapBlockDict.Add(new Vector2(x, y), _mapBlocks[x, y]);//寻路字典
                }

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
                    //else
                    //{
                    //    Unit[] units = InitAndInstantiateGameUnits(mapData[i]["token"], tokenCount, x, y);
                    //    for (int j = 0; j < units.Length; j++)
                    //    {
                    //        //units[j].mapBlockBelow = _mapBlocks[x, y];
                    //        _unitsList.Add(units[j]);
                    //    }
                    //    _mapBlocks[x, y].AddUnits(units);
                    //}
                }
            }

            #region 得到每块地图块周围的地图块
            
            BattleMapBlock[] neighbourBlock = new BattleMapBlock[4];
            
            
            for(int i = 0;i < rows; i++)
            {
                for(int j = 0;j< columns; j++)
                {
                    Vector2 t = new Vector2(j, i - 1);
                    Vector2 b = new Vector2(j, i + 1);
                    Vector2 r = new Vector2(j + 1, i);
                    Vector2 l = new Vector2(j - 1, i);

                    if (t.x >= 0 && t.y >= 0 && t.x < columns && t.y < rows && _mapBlocks[(int)t.x, (int)t.y].transform.childCount == 0)
                    {
                        neighbourBlock[0] = mapBlockDict[t];
                        Debug.Log(neighbourBlock[0].x);
                    }
                    if (b.x >= 0 && b.y >= 0 && b.x < columns && b.y < rows && _mapBlocks[(int)b.x, (int)b.y].transform.childCount == 0)
                    {
                        neighbourBlock[1] = mapBlockDict[b];
                    }
                    if (r.x >= 0 && r.y >= 0 && r.x < columns && r.y < rows && _mapBlocks[(int)r.x, (int)r.y].transform.childCount == 0)
                    {
                        neighbourBlock[2] = mapBlockDict[r];
                    }
                    if (l.x >= 0 && l.y >= 0 && l.x < columns && l.y < rows && _mapBlocks[(int)l.x, (int)l.y].transform.childCount == 0)
                    {
                        neighbourBlock[3] = mapBlockDict[l];
                    }
                }
            }
            #endregion
        }
        #endregion

        private void ReadUnitDataInJason(JsonData data, string owner, int damaged, Unit unit)
        {
            unit.Name = data["name"].ToString();
            unit.id = data["id"].ToString();
            unit.atk = (int)data["atk"];
            unit.hp = (int)data["hp"];
            unit.mov = (int)data["mov"];
            unit.rng = (int)data["rng"];
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
        
        //初始地图单位
        private Unit InitAndInstantiateGameUnit(JsonData data, int x, int y)
        {
            Unit newUnit;
            GameObject _object;
            if (data["Controler - Enemy, Friendly or Self"].ToString().Equals("Self"))
            {
                Debug.Log("instantiateUnit");
                //this.player = _object = Instantiate(player_assete, new Vector3(x, y, 0f), Quaternion.identity);
                this.player = _object = Instantiate(player_assete);
                player.transform.SetParent(_mapBlocks[x, y].transform);
                player.transform.position = new Vector3(x, y, 0f);

            }
            if (data["Controler - Enemy, Friendly or Self"].ToString().Equals("Obstacle"))
            {
                _object = Instantiate(obstacle);
                _object.transform.SetParent(_mapBlocks[x, y].transform);
                _object.transform.position = new Vector3(x, y, 0f);
            }
            else
            {
                //_object =Instantiate(enemys[Random.Range(0, enemys.Length)], new Vector3(x, y, 0f),Quaternion.identity);
                _object = Instantiate(enemys[Random.Range(0, enemys.Length)]);
                _object.transform.SetParent(_mapBlocks[x, y].transform);
                _object.transform.position = new Vector3(x, y, 0f);

            }
            // 在单位上挂载unit 脚本
            //_object.AddComponent<Unit>();

            // 获取该脚本对象并传入解析json函数赋值
            newUnit = _object.GetComponent<Unit>();
            Debug.Log(this._unitsData[data["name"].ToString()]);
            ReadUnitDataInJason(this._unitsData[data["name"].ToString()],
                                data["Controler - Enemy, Friendly or Self"].ToString(),
                                (int)data["Damaged"],
                                newUnit);

            // 在单位上挂载展示数值显示脚本
            //_object.AddComponent<DisplayData>();

            // 不管是不是player单位都挂载展示范围脚本
            //_object.AddComponent<ShowRange>();

            return newUnit;
        }

        public BattleMapBlock GetSpecificMapBlock(int x, int y)
        {
            return this._mapBlocks[x, y];
        }

        public Vector3 GetCoordinate(BattleMapBlock mapBlock)
        {
            for (int i = columns - 1; i > 0; i--)
            {
                for (int j = 0; j < rows; j++)
                {
                    if (_mapBlocks[i, j] == mapBlock)
                    {
                        return new Vector3(i, j, 0f);
                    }
                }
            }
            return new Vector3(-1, -1, 0f);
        }

        //实现点击地图块后弃用
        //private Boolean _CheckVector(Vector3 vector)
        //{
        //    if ((int)vector.x > rows*62.5 || (int)vector.y > columns*62.5)
        //    {
        //        Debug.Log(string.Format("Invalid coordinate: {0}, {1} !", (int)vector.x, (int)vector.y));
        //        return false;
        //    }

        //    if ((int)vector.x < 0 || (int)vector.y < 0)
        //    {
        //        Debug.Log(string.Format("Error ! Outranged coordinate: {0}, {1} !", (int)vector.x, (int)vector.y));
        //        return false;
        //    }

        //    return true;
        //}



        // 确定给定坐标上是否含有单位，坐标不合法会返回false，其他依据实际情况返回值
        public Boolean CheckIfHasUnits(Vector3 vector)
        {
            //if (!_CheckVector(vector)) return false;
            //return this._mapBlocks[(int)vector.x, (int)vector.y].units_on_me != null;
            if (this._mapBlocks[(int)vector.x, (int)vector.y] != null && this._mapBlocks[(int)vector.x, (int)vector.y].transform.childCount != 0
                && this._mapBlocks[(int)vector.x, (int)vector.y].GetComponentInChildren<Unit>() != null &&
                this._mapBlocks[(int)vector.x, (int)vector.y].GetComponentInChildren<Unit>().owner != "Obstacle"/*units_on_me.Count != 0*/)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // 返回给定坐标上单位list，坐标不合法会返回null, 其他依据实际情况返回值
        public Unit GetUnitsOnMapBlock(Vector3 vector)
        {
            if (this._mapBlocks[(int)vector.x, (int)vector.y] != null && this._mapBlocks[(int)vector.x, (int)vector.y].transform.childCount != 0)
            {
                return _mapBlocks[(int)vector.x, (int)vector.y].GetComponentInChildren<Unit>();
            }
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
                    if (_mapBlocks[(int)destination.x, (int)destination.y] != null)
                    {
                        unit.mapBlockBelow = _mapBlocks[(int)destination.x, (int)destination.y];
                    }
                    if (_mapBlocksBurning[(int)destination.x, (int)destination.y] != null)
                    {
                        unit.mapBlockBelow = _mapBlocksBurning[(int)destination.x, (int)destination.y];
                    }
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
            foreach (Vector3 position in positions)
            {
                if (/*_mapBlocks[(int)position.x, (int)position.y] != null &&*/position.x < columns && position.y < rows
                    && position.x >= 0 && position.y >= 0)
                {
                    _mapBlocks[(int)position.x, (int)position.y].gameObject.GetComponent<Image>().color = color;
                }
                else
                {
                }
            }
        }


        
        private List<Vector2> emptyMapBlocksPositions = new List<Vector2>();//保存战区空的格子
        private List<Unit> units = new List<Unit>();//获取战区上我方所有单位
        private Unit unit;
        #region 战区相关
        //战区增益
        public void WarZooeBuff()
        {
            //TODO增加一点资源点上限

            //立即部署一个临时友方单位
            if (emptyMapBlocksPositions.Count != 0)
            {
                foreach (Vector2 pos in emptyMapBlocksPositions)
                {
                    GameObject go = GameObject.Instantiate(player);
                    go.transform.SetParent(_mapBlocks[(int)pos.x, (int)pos.y].transform);
                    go.transform.localPosition = Vector3.zero;
                }
            }
            //额外的行动力
            foreach (Unit unit in units)
            {
                unit.mov += 1;
            }
        }

        //移除战区增益
        public void RemoveWarZooeBuff()
        {
            //移除额外行动力
            foreach (Unit unit in units)
            {
                unit.mov -= 1;
            }
        }

        //判断该战区能不能召唤（所属权）
        public bool WarZoneBelong(Vector3 position)
        {
            int area;
            area = _mapBlocks[(int)position.x, (int)position.y].area;
            //units.Clear();
            //emptyMapBlocks.Clear();

            List<Vector2> battleArea = null;
            
            switch (area)
            {
                case -1:
                    battleArea = battleArea_1;
                    break;
                case 0:
                    battleArea = battleArea0;
                    break;
                case 1:
                    battleArea = battleArea1;
                    break;
                case 2:
                    battleArea = battleArea2;
                    break;
                case 3:
                    battleArea = battleArea3;
                    break;
                case 4:
                    battleArea = battleArea4;
                    break;
                case 5:
                    battleArea = battleArea5;
                    break;
                case 6:
                    battleArea = battleArea6;
                    break;
                case 7:
                    battleArea = battleArea7;
                    break;
                default:
                    Debug.Log(string.Format("Invalid area value {0}", area));
                    return false;
            }
            
            foreach (Vector2 pos in battleArea)
            {
                int x = (int) pos.x;
                int y = (int) pos.y;
                Debug.Log(x + ":" + y);
                if (_mapBlocks[x, y].transform.childCount != 0)
                {
                    if (_mapBlocks[x, y].GetComponentInChildren<Unit>() != null)
                    {
                        if (_mapBlocks[x, y].GetComponentInChildren<Unit>().owner.Equals("Enemy"))
                        {
                            Debug.Log("This WarZone has Enemy,you can`t summon");
                            return false;
                        }
                        if (_mapBlocks[x, y].GetComponentInChildren<Unit>().owner.Equals("Self"))
                        {
                            unit = _mapBlocks[x, y].GetComponentInChildren<Unit>();
                            units.Add(unit);
                        }
                    }
                }
                else
                {
                    Debug.Log(x + ":" + y);
                    emptyMapBlocksPositions.Add(new Vector2(x, y));

                }
            }
            #region 弃用
            /*
            if (area == -1)
            {
                foreach (BattleMapBlock map in battleArea_1)
                {
                    Debug.Log(map.x + ":" + map.y);
                    if (_mapBlocks[map.x, map.y].transform.childCount != 0)
                    {
                        if (_mapBlocks[map.x, map.y].GetComponentInChildren<Unit>() != null)
                        {
                            if (_mapBlocks[map.x, map.y].GetComponentInChildren<Unit>().owner == "Enemy")
                            {
                                Debug.Log("This WarZone has Enemy,you can`t summon");
                                return false;
                            }
                            else if (_mapBlocks[map.x, map.y].GetComponentInChildren<Unit>().owner == "Self")
                            {
                                unit = _mapBlocks[map.x, map.y].GetComponentInChildren<Unit>();
                                units.Add(unit);
                            }
                        }
                    }
                    else
                    {
                        emptyMbpBlock = new BattleMapBlock(map.x, map.y);
                        Debug.Log(emptyMbpBlock);
                        emptyMapBlocks.Add(emptyMbpBlock);

                    }
                }
            }
            if (area == 0)
            {
                foreach (BattleMapBlock map in battleArea0)
                {
                    Debug.Log(map.x + ":" + map.y);
                    if (_mapBlocks[map.x, map.y].transform.childCount != 0)
                    {
                        if (_mapBlocks[map.x, map.y].GetComponentInChildren<Unit>() != null)
                        {
                            if (_mapBlocks[map.x, map.y].GetComponentInChildren<Unit>().owner == "Enemy")
                            {
                                Debug.Log("This WarZone has Enemy,you can`t summon");
                                return false;
                            }
                            else if (_mapBlocks[map.x, map.y].GetComponentInChildren<Unit>().owner == "Self")
                            {
                                unit = _mapBlocks[map.x, map.y].GetComponentInChildren<Unit>();
                                units.Add(unit);
                            }
                        }
                    }
                    else
                    {
                        emptyMbpBlock = new BattleMapBlock(map.x, map.y);
                        Debug.Log(emptyMbpBlock);
                        emptyMapBlocks.Add(emptyMbpBlock);

                    }
                }
            }
            if (area == 1)
            {
                foreach (BattleMapBlock map in battleArea1)
                {
                    Debug.Log(map.x + ":" + map.y);
                    if (_mapBlocks[map.x, map.y].transform.childCount != 0)
                    {
                        if (_mapBlocks[map.x, map.y].GetComponentInChildren<Unit>() != null)
                        {
                            if (_mapBlocks[map.x, map.y].GetComponentInChildren<Unit>().owner == "Enemy")
                            {
                                Debug.Log("This WarZone has Enemy,you can`t summon");
                                return false;
                            }
                            else if (_mapBlocks[map.x, map.y].GetComponentInChildren<Unit>().owner == "Self")
                            {
                                unit = _mapBlocks[map.x, map.y].GetComponentInChildren<Unit>();
                                units.Add(unit);
                            }
                        }
                    }
                    else
                    {
                        emptyMbpBlock = new BattleMapBlock(map.x, map.y);
                        Debug.Log(emptyMbpBlock);
                        emptyMapBlocks.Add(emptyMbpBlock);

                    }
                }
            }
            if (area == 2)
            {
                foreach (BattleMapBlock map in battleArea2)
                {
                    Debug.Log(map.x + ":" + map.y);
                    if (_mapBlocks[map.x, map.y].transform.childCount != 0)
                    {
                        if (_mapBlocks[map.x, map.y].GetComponentInChildren<Unit>() != null)
                        {
                            if (_mapBlocks[map.x, map.y].GetComponentInChildren<Unit>().owner == "Enemy")
                            {
                                Debug.Log("This WarZone has Enemy,you can`t summon");
                                return false;
                            }
                            else if (_mapBlocks[map.x, map.y].GetComponentInChildren<Unit>().owner == "Self")
                            {
                                unit = _mapBlocks[map.x, map.y].GetComponentInChildren<Unit>();
                                units.Add(unit);
                            }
                        }
                    }
                    else
                    {
                        emptyMbpBlock = new BattleMapBlock(map.x, map.y);
                        Debug.Log(emptyMbpBlock);
                        emptyMapBlocks.Add(emptyMbpBlock);

                    }
                }
            }
            if (area == 3)
            {
                foreach (BattleMapBlock map in battleArea3)
                {
                    Debug.Log(map.x + ":" + map.y);
                    if (_mapBlocks[map.x, map.y].transform.childCount != 0)
                    {
                        if (_mapBlocks[map.x, map.y].GetComponentInChildren<Unit>() != null)
                        {
                            if (_mapBlocks[map.x, map.y].GetComponentInChildren<Unit>().owner == "Enemy")
                            {
                                Debug.Log("This WarZone has Enemy,you can`t summon");
                                return false;
                            }
                            else if (_mapBlocks[map.x, map.y].GetComponentInChildren<Unit>().owner == "Self")
                            {
                                unit = _mapBlocks[map.x, map.y].GetComponentInChildren<Unit>();
                                units.Add(unit);
                            }
                        }
                    }
                    else
                    {
                        emptyMbpBlock = new BattleMapBlock(map.x, map.y);
                        Debug.Log(emptyMbpBlock);
                        emptyMapBlocks.Add(emptyMbpBlock);

                    }
                }
            }
            if (area == 4)
            {
                foreach (BattleMapBlock map in battleArea4)
                {
                    Debug.Log(map.x + ":" + map.y);
                    if (_mapBlocks[map.x, map.y].transform.childCount != 0)
                    {
                        if (_mapBlocks[map.x, map.y].GetComponentInChildren<Unit>() != null)
                        {
                            if (_mapBlocks[map.x, map.y].GetComponentInChildren<Unit>().owner == "Enemy")
                            {
                                Debug.Log("This WarZone has Enemy,you can`t summon");
                                return false;
                            }
                            else if (_mapBlocks[map.x, map.y].GetComponentInChildren<Unit>().owner == "Self")
                            {
                                unit = _mapBlocks[map.x, map.y].GetComponentInChildren<Unit>();
                                units.Add(unit);
                            }
                        }
                    }
                    else
                    {
                        emptyMbpBlock = new BattleMapBlock(map.x, map.y);
                        Debug.Log(emptyMbpBlock);
                        emptyMapBlocks.Add(emptyMbpBlock);

                    }
                }
            }
            if (area == 5)
            {
                foreach (BattleMapBlock map in battleArea5)
                {
                    Debug.Log(map.x + ":" + map.y);
                    if (_mapBlocks[map.x, map.y].transform.childCount != 0)
                    {
                        if (_mapBlocks[map.x, map.y].GetComponentInChildren<Unit>() != null)
                        {
                            if (_mapBlocks[map.x, map.y].GetComponentInChildren<Unit>().owner == "Enemy")
                            {
                                Debug.Log("This WarZone has Enemy,you can`t summon");
                                return false;
                            }
                            else if (_mapBlocks[map.x, map.y].GetComponentInChildren<Unit>().owner == "Self")
                            {
                                unit = _mapBlocks[map.x, map.y].GetComponentInChildren<Unit>();
                                units.Add(unit);
                            }
                        }
                    }
                    else
                    {
                        emptyMbpBlock = new BattleMapBlock(map.x, map.y);
                        Debug.Log(emptyMbpBlock);
                        emptyMapBlocks.Add(emptyMbpBlock);

                    }
                }
            }
            if (area == 6)
            {
                foreach (BattleMapBlock map in battleArea6)
                {
                    Debug.Log(map.x + ":" + map.y);
                    if (_mapBlocks[map.x, map.y].transform.childCount != 0)
                    {
                        if (_mapBlocks[map.x, map.y].GetComponentInChildren<Unit>() != null)
                        {
                            if (_mapBlocks[map.x, map.y].GetComponentInChildren<Unit>().owner == "Enemy")
                            {
                                Debug.Log("This WarZone has Enemy,you can`t summon");
                                return false;
                            }
                            else if (_mapBlocks[map.x, map.y].GetComponentInChildren<Unit>().owner == "Self")
                            {
                                unit = _mapBlocks[map.x, map.y].GetComponentInChildren<Unit>();
                                units.Add(unit);
                            }
                        }
                    }
                    else
                    {
                        emptyMbpBlock = new BattleMapBlock(map.x, map.y);
                        Debug.Log(emptyMbpBlock);
                        emptyMapBlocks.Add(emptyMbpBlock);

                    }
                }
            }
            if (area == 7)
            {
                foreach (BattleMapBlock map in battleArea7)
                {
                    Debug.Log(map.x + ":" + map.y);
                    if (_mapBlocks[map.x, map.y].transform.childCount != 0)
                    {
                        if (_mapBlocks[map.x, map.y].GetComponentInChildren<Unit>() != null)
                        {
                            if (_mapBlocks[map.x, map.y].GetComponentInChildren<Unit>().owner == "Enemy")
                            {
                                Debug.Log("This WarZone has Enemy,you can`t summon");
                                return false;
                            }
                            else if (_mapBlocks[map.x, map.y].GetComponentInChildren<Unit>().owner == "Self")
                            {
                                unit = _mapBlocks[map.x, map.y].GetComponentInChildren<Unit>();
                                units.Add(unit);
                            }
                        }
                    }
                    else
                    {
                        emptyMbpBlock = new BattleMapBlock(map.x, map.y);
                        Debug.Log(emptyMbpBlock);
                        emptyMapBlocks.Add(emptyMbpBlock);

                    }
                }
            }*/
            #endregion
            return true;
        }

        //显示战区
        public void ShowBattleZooe(Vector3 position)
        {
            int area = _mapBlocks[(int)position.x, (int)position.y].area;
            
            List<Vector2> battleArea = null;
            
            switch (area)
            {
                case -1:
                    battleArea = battleArea_1;
                    break;
                case 0:
                    battleArea = battleArea0;
                    break;
                case 1:
                    battleArea = battleArea1;
                    break;
                case 2:
                    battleArea = battleArea2;
                    break;
                case 3:
                    battleArea = battleArea3;
                    break;
                case 4:
                    battleArea = battleArea4;
                    break;
                case 5:
                    battleArea = battleArea5;
                    break;
                case 6:
                    battleArea = battleArea6;
                    break;
                case 7:
                    battleArea = battleArea7;
                    break;
                default:
                    Debug.Log(string.Format("Invalid area value {0}", area));
                    return;
            }
            
            foreach (Vector2 pos in battleArea)
            {
                _mapBlocks[(int)pos.x, (int)pos.y].gameObject.GetComponent<Image>().color = Color.yellow;
            }
            #region 弃用
            /*
            if (area == -1)
            {
                foreach (BattleMapBlock map in battleArea_1)
                {
                    _mapBlocks[map.x, map.y].gameObject.GetComponent<Image>().color = Color.yellow;
                }
            }
            if (area == 0)
            {
                foreach (BattleMapBlock map in battleArea0)
                {
                    _mapBlocks[map.x, map.y].gameObject.GetComponent<Image>().color = Color.yellow;
                }
            }
            if (area == 1)
            {
                foreach (BattleMapBlock map in battleArea1)
                {
                    _mapBlocks[map.x, map.y].gameObject.GetComponent<Image>().color = Color.yellow;
                }
            }
            if (area == 2)
            {
                foreach (BattleMapBlock map in battleArea2)
                {
                    _mapBlocks[map.x, map.y].gameObject.GetComponent<Image>().color = Color.yellow;
                }
            }
            if (area == 3)
            {
                foreach (BattleMapBlock map in battleArea3)
                {
                    _mapBlocks[map.x, map.y].gameObject.GetComponent<Image>().color = Color.yellow;
                }
            }
            if (area == 4)
            {
                foreach (BattleMapBlock map in battleArea4)
                {
                    _mapBlocks[map.x, map.y].gameObject.GetComponent<Image>().color = Color.yellow;
                }
            }
            if (area == 5)
            {
                foreach (BattleMapBlock map in battleArea5)
                {
                    _mapBlocks[map.x, map.y].gameObject.GetComponent<Image>().color = Color.yellow;
                }
            }
            if (area == 6)
            {
                foreach (BattleMapBlock map in battleArea6)
                {
                    _mapBlocks[map.x, map.y].gameObject.GetComponent<Image>().color = Color.yellow;
                }
            }
            if (area == 7)
            {
                foreach (BattleMapBlock map in battleArea7)
                {
                    _mapBlocks[map.x, map.y].gameObject.GetComponent<Image>().color = Color.yellow;
                }
            }*/
            #endregion
        }
        
        //隐藏战区
        public void HideBattleZooe(Vector3 position)
        {
            int area  = _mapBlocks[(int)position.x, (int)position.y].area;
            
            List<Vector2> battleArea = null;
            
            switch (area)
            {
                case -1:
                    battleArea = battleArea_1;
                    break;
                case 0:
                    battleArea = battleArea0;
                    break;
                case 1:
                    battleArea = battleArea1;
                    break;
                case 2:
                    battleArea = battleArea2;
                    break;
                case 3:
                    battleArea = battleArea3;
                    break;
                case 4:
                    battleArea = battleArea4;
                    break;
                case 5:
                    battleArea = battleArea5;
                    break;
                case 6:
                    battleArea = battleArea6;
                    break;
                case 7:
                    battleArea = battleArea7;
                    break;
                default:
                    Debug.Log(string.Format("Invalid area value {0}", area));
                    return;
            }
            
            foreach (Vector2 pos in battleArea)
            {
                _mapBlocks[(int)pos.x, (int)pos.y].gameObject.GetComponent<Image>().color = Color.white;
            }
            
            #region 弃用
            /*
            if (area == -1)
            {
                foreach (BattleMapBlock map in battleArea_1)
                {
                    _mapBlocks[map.x, map.y].gameObject.GetComponent<Image>().color = Color.white;
                }
            }
            if (area == 0)
            {
                foreach (BattleMapBlock map in battleArea0)
                {
                    _mapBlocks[map.x, map.y].gameObject.GetComponent<Image>().color = Color.white;
                }
            }
            if (area == 1)
            {
                foreach (BattleMapBlock map in battleArea1)
                {
                    _mapBlocks[map.x, map.y].gameObject.GetComponent<Image>().color = Color.white;
                }
            }
            if (area == 2)
            {
                foreach (BattleMapBlock map in battleArea2)
                {
                    _mapBlocks[map.x, map.y].gameObject.GetComponent<Image>().color = Color.white;
                }
            }
            if (area == 3)
            {
                foreach (BattleMapBlock map in battleArea3)
                {
                    _mapBlocks[map.x, map.y].gameObject.GetComponent<Image>().color = Color.white;
                }
            }
            if (area == 4)
            {
                foreach (BattleMapBlock map in battleArea4)
                {
                    _mapBlocks[map.x, map.y].gameObject.GetComponent<Image>().color = Color.yellow;
                }
            }
            if (area == 5)
            {
                foreach (BattleMapBlock map in battleArea5)
                {
                    _mapBlocks[map.x, map.y].gameObject.GetComponent<Image>().color = Color.white;
                }
            }
            if (area == 6)
            {
                foreach (BattleMapBlock map in battleArea6)
                {
                    _mapBlocks[map.x, map.y].gameObject.GetComponent<Image>().color = Color.yellow;
                }
            }
            if (area == 7)
            {
                foreach (BattleMapBlock map in battleArea7)
                {
                    _mapBlocks[map.x, map.y].gameObject.GetComponent<Image>().color = Color.white;
                }
            }
            */
            #endregion
        }
        #endregion
    }
}