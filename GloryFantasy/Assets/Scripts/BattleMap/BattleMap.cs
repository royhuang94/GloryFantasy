using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using LitJson;
using System.IO;
using Unit = GameUnit.GameUnit;
using UnityEngine.UI;

namespace BattleMap
{
    public class BattleMap : MonoBehaviour
    {
        private static BattleMap instance = null;

        public static BattleMap getInstance()
        {
            return instance;
        }

        private BattleMap()
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
            IsColor = false;
            InitMap();
        }

        public void InitMap()
        {
            // TODO : 添加初始化地图的方法
            //for_demo_init();
            InitAndInstantiateMapBlocks();

            //池子初始化
            PoolManager.Instance.Init();
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

        public GameObject selectAction;
        public bool IsColor { get; set; }//控制是否高亮战区
        public Vector3 curMapPos;
        private BattleMapBlock[,] _mapBlocks;         //普通的地图方块
        private BattleMapBlock[,] _mapBlocksBurning;//灼烧的地图方块
        public GameObject normalMapBlocks;
        public Dictionary<Vector2, BattleMapBlock> mapBlockDict = new Dictionary<Vector2, BattleMapBlock>();//寻路字典
        public List<BattleMapBlock> aStarPath = new List<BattleMapBlock>();  //最优路径

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

        #endregion

        // 记录json中token不为空的坐标，待后续处理
        //private List <Vector3> specialPositions = new List <Vector3> ();

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
                    //Debug.Log(string.Format("固定的灼烧块{0},{0}", x, y));
                    _instance.gameObject.AddComponent<BattleMapBlockBurning>();

                    _instance.gameObject.AddComponent<BattleMapBlock>();
                    _mapBlocks[x, y] = _instance.gameObject.GetComponent<BattleMapBlock>();
                    _mapBlocks[x, y].area = area;
                    _mapBlocks[x, y].x = x;
                    _mapBlocks[x, y].y = y;
                    _mapBlocks[x, y].aStarState = AStarState.free;
                    _mapBlocks[x, y].blockType = EMapBlockType.normal;
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
                    _mapBlocks[x, y].blockType = EMapBlockType.normal;
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


            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    Vector2 t = new Vector2(j, i - 1);
                    Vector2 b = new Vector2(j, i + 1);
                    Vector2 r = new Vector2(j + 1, i);
                    Vector2 l = new Vector2(j - 1, i);

                    if (t.x >= 0 && t.y >= 0 && t.x < columns && t.y < rows && _mapBlocks[(int)t.x, (int)t.y].transform.childCount == 0)
                    {
                        mapBlockDict[new Vector2(j, i)].neighbourBlock[0] = mapBlockDict[t];
                    }
                    if (b.x >= 0 && b.y >= 0 && b.x < columns && b.y < rows && _mapBlocks[(int)b.x, (int)b.y].transform.childCount == 0)
                    {
                        mapBlockDict[new Vector2(j, i)].neighbourBlock[1] = mapBlockDict[b];
                    }
                    if (l.x >= 0 && l.y >= 0 && l.x < columns && l.y < rows && _mapBlocks[(int)l.x, (int)l.y].transform.childCount == 0)
                    {
                        mapBlockDict[new Vector2(j, i)].neighbourBlock[2] = mapBlockDict[l];
                    }
                    if (r.x >= 0 && r.y >= 0 && r.x < columns && r.y < rows && _mapBlocks[(int)r.x, (int)r.y].transform.childCount == 0)
                    {
                        mapBlockDict[new Vector2(j, i)].neighbourBlock[3] = mapBlockDict[r];
                    }
                }
            }
            #endregion
        }
        #endregion

        //更新寻路字典,避免再次遍历整个地图
        public void upDateNeighbourBlock(Vector2 position)
        {
            int j = (int)position.x;
            int i = (int)position.y;
            Vector2 t = new Vector2(j, i - 1);
            Vector2 b = new Vector2(j, i + 1);
            Vector2 r = new Vector2(j + 1, i);
            Vector2 l = new Vector2(j - 1, i);

            if (t.x >= 0 && t.y >= 0 && t.x < columns && t.y < rows)
            {
                mapBlockDict[new Vector2(t.x, t.y)].neighbourBlock[1] = mapBlockDict[position];
            }
            if (b.x >= 0 && b.y >= 0 && b.x < columns && b.y < rows)
            {
                mapBlockDict[new Vector2(b.x, b.x)].neighbourBlock[0] = mapBlockDict[position];
            }
            if (l.x >= 0 && l.y >= 0 && l.x < columns && l.y < rows)
            {
                mapBlockDict[new Vector2(l.x, l.y)].neighbourBlock[3] = mapBlockDict[position];
            }
            if (r.x >= 0 && r.y >= 0 && r.x < columns && r.y < rows)
            {
                mapBlockDict[new Vector2(r.x, r.y)].neighbourBlock[2] = mapBlockDict[position];
            }
        }

        /// <summary>
        /// 初始化GameUnit脚本函数，会根据id设定GameUnit脚本内数值
        /// </summary>
        /// <param name="id">地图单位中相应的id</param>
        /// <param name="owner">地图单位中相应的所属方</param>
        /// <param name="unit">GameUnit脚本类的引用</param>
        private void InitGameUnitScript(string id, string owner, Unit unit)
        {
            unit.id = id;
            unit.owner = owner;
            BmuScriptsHandler.GetInstance().InitGameUnit(unit);
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
                _object = Instantiate(obstacle, _mapBlocks[x, y].transform, true);
                _object.transform.position = new Vector3(x, y, 0f);
            }
            else
            {
                //_object =Instantiate(enemys[Random.Range(0, enemys.Length)], new Vector3(x, y, 0f),Quaternion.identity);
                _object = Instantiate(enemys[Random.Range(0, enemys.Length)], _mapBlocks[x, y].transform, true);
                _object.transform.position = new Vector3(x, y, 0f);

                //TODO 血量显示 test版本, 此后用slider显示
                var TextHp = _object.transform.GetComponentInChildren<Text>();
                var gameUnit = _object.GetComponent<GameUnit.GameUnit>();
                float hp = gameUnit.unitAttribute.HP/* - Random.Range(2, 6)*/;
                float maxHp = gameUnit.unitAttribute.MaxHp;
                float hpDivMaxHp = hp / maxHp * 100;

                TextHp.text = string.Format("Hp: {0}%", hpDivMaxHp);
                //TextHp.text = string.Format("HP: {0} / {1}", _object.GetComponent<GameUnit.GameUnit>().unitAttribute.HP.ToString(), _object.GetComponent<GameUnit.GameUnit>().unitAttribute.MaxHp.ToString());

            }
            // 在单位上挂载unit 脚本
            //_object.AddComponent<Unit>();

            // 获取该脚本对象并传入解析json函数赋值
            newUnit = _object.GetComponent<Unit>();
            InitGameUnitScript(data["name"].ToString(),
                data["Controler - Enemy, Friendly or Self"].ToString(),
                newUnit);
            /*
            ReadUnitDataInJason(this._unitsData[data["name"].ToString()],
                                data["Controler - Enemy, Friendly or Self"].ToString(),
                                (int)data["Damaged"],
                                newUnit);*/

            // 在单位上挂载展示数值显示脚本
            //_object.AddComponent<DisplayData>();

            // 不管是不是player单位都挂载展示范围脚本
            //_object.AddComponent<ShowRange>();

            return newUnit;
        }

        //TODO 根据坐标返回地图块儿 --> 在对应返回的地图块儿上抓取池内的对象，"投递上去"
        //TODO 相当于是召唤技能，可以与郑大佬的技能脚本产生联系
        //TODO 类似做一个召唤技能，通过UGUI的按钮实现

        //TODO 如何实现
        //1. 首先我们输入一个坐标 -> 传递给某个函数，此函数能够根据坐标获得地图块儿 -> 获取到地图块儿后便可以通过地图块儿，从池子中取出Unit “投递”到该地图块儿上
        //2. 完成
        //3. 转移成一个skill

        //TODO 测试召唤与对象池
        public void ButtonClickDown()
        {
            if (SummonOnBlock(Random.Range(0, 11), Random.Range(0, 11)))
                GFGame.UtilityHelper.Log("召唤成功", GFGame.LogColor.PURPLE);
            else
                GFGame.UtilityHelper.Log("召唤失败", GFGame.LogColor.BLUE);
        }

        public bool SummonOnBlock(int x = 0, int y = 1)
        {
            //TODO 测试坐标 (0,0)
            var tempBlock = GetSpecificMapBlock(x, y);

            if(tempBlock.transform.childCount == 0)
            {
                var tempUnit = PoolManager.Instance.GetInst("ShadowSoldier_1");


                if (tempUnit != null)
                {
                    tempUnit.transform.parent = tempBlock.transform;
                    tempUnit.transform.localPosition = Vector3.zero;
                    return true;
                }
            }

            return false;
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

        public EMapBlockType GetMapBlockType(Vector3 coordinate)
        {
            int x = (int)coordinate.x;
            int y = (int)coordinate.y;
            if (x < 0 || y < 0 || x >= columns || y >= rows)
            {
                // TODO: 添加异常坐标处理
            }

            return _mapBlocks[x, y].blockType;
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
                int x = (int)pos.x;
                int y = (int)pos.y;
                //Debug.Log(x + ":" + y);
                if (_mapBlocks[x, y].transform.childCount != 0)
                {
                    Unit unit = _mapBlocks[x, y].GetComponentInChildren<Unit>();
                    if (unit != null)
                    {
                        if (unit.owner.Equals("Enemy"))
                        {
                            Debug.Log("This WarZone has Enemy,you can`t summon");
                            return false;
                        }
                        if (unit.owner.Equals("Self"))
                        {
                            unit = _mapBlocks[x, y].GetComponentInChildren<Unit>();
                            units.Add(unit);
                        }
                    }
                }
                else
                {
                    //Debug.Log(x + ":" + y);
                    emptyMapBlocksPositions.Add(new Vector2(x, y));

                }
            }
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
                _mapBlocks[(int)pos.x, (int)pos.y].gameObject.GetComponent<Image>().color = Color.white;
            }

        }
    }
}