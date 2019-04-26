using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using LitJson;
using System.IO;
using Unit = GameUnit.GameUnit;
using UnityEngine.UI;

using GameUnit;

namespace BattleMap
{
    public class BattleMap : UnitySingleton<BattleMap>
    {

        private void Awake()
        {
            _unitsList = new List<Unit>();
            _instance = this;
            IsColor = false;
            MapNavigator = new MapNavigator();
        }

        private void Start()
        {
            InitMap();
        }

        public void InitMap()
        {
            //初始化地图
            InitAndInstantiateMapBlocks();
        }

        //初始化地图的地址
        //更改地图数据位置则需修改此处路径
        public string InitialMapDataPath = "/Scripts/BattleMap/eg1.json";

        /// <summary>
        /// 获取战斗地图上的所有单位
        /// </summary>
        public List<Unit> UnitsList
        {
            get
            {
                return _unitsList;
            }
        }
        private List<Unit> _unitsList;
        
        //这些变量自己要清理一下啊，哪些是要用的，哪些是老代码遗留下来的
        //看到有几个变量貌似重复定义了类似的功能，这后面来看的人怎么用嘛
        //而且变量分成了两个地方写，更加难看了
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
        public GameObject selcetAction_Cancel;
        public bool IsColor { get; set; }//控制是否高亮战区
        public Vector3 curMapPos;
        private BattleMapBlock[,] _mapBlocks;         //普通的地图方块
        private BattleMapBlock[,] _mapBlocksBurning;//灼烧的地图方块
        //为什么要给烧灼的地图方块开一个二维数组？难道烧灼地块就没有普通地块的成员和方法了吗？
        //而且为什么烧灼地块是用继承的方式来实现？如果我们现在不只是2种特殊地形，而是10种，那难道你要写10个不同的地图方块类嘛？
        //所以正确的写法应该是把对负面地形的处理写成一个类，然后让MapBlock去持有
        //TODO：review plz

        public GameObject normalMapBlocks;
        //这玩意儿有什么用？意义不明
        //public Dictionary<Vector2, BattleMapBlock> mapBlockDict = new Dictionary<Vector2, BattleMapBlock>();//寻路字典
        //为什么路径要保存在这里？寻路类没地方放嘛？
        //public List<BattleMapBlock> aStarPath = new List<BattleMapBlock>();  //最优路径
        /// <summary>
        /// 寻路类
        /// </summary>
        public MapNavigator MapNavigator;
        
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

        //这是什么？
        public GameObject player { get; set; }

        public Transform _tilesHolder;          // 存储所有地图单位引用的变量

        #endregion

        #region  初始地图相关
        private void InitAndInstantiateMapBlocks()
        {
            
            JsonData mapData = JsonMapper.ToObject(File.ReadAllText(Application.dataPath + InitialMapDataPath));

            int mapDataCount = mapData.Count;
            this.columns = (int)mapData[mapDataCount - 1]["y"] + 1;
            this.rows = (int)mapData[mapDataCount - 1]["x"] + 1;

            _mapBlocks = new BattleMapBlock[rows, columns];
            GameObject instance = new GameObject();

            //这种战区数据结构是什么天才想法……这么写你看看自己下面复制黏贴了多少行代码
            //实在不行，Dictionary套List也行啊，Dictionary<int, List<Vector2>>，有空记得重构下
            //TODO: Review plz
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
                }
                #endregion


                //实例化地图块
                instance = GameObject.Instantiate(normalMapBlocks, new Vector3(x, y, 0f), Quaternion.identity);
                instance.transform.SetParent(_tilesHolder);

                //如果是边缘地图块，就上烧灼块，否则上普通块
                //if (x == 0 && y == 0)
                //{
                //    instance.gameObject.AddComponent<BattleMapBlockBurning>();
                //}
                //else
                //{
                    instance.gameObject.AddComponent<BattleMapBlock>();
                //}
                //初始化mapBlock成员
                _mapBlocks[x, y] = instance.gameObject.GetComponent<BattleMapBlock>();
                _mapBlocks[x, y].area = area;
                _mapBlocks[x, y].x = x;
                _mapBlocks[x, y].y = y;
                //_mapBlocks[x, y].aStarState = AStarState.free;
                _mapBlocks[x, y].blockType = EMapBlockType.normal;
                //mapBlockDict.Add(new Vector2(x, y), _mapBlocks[x, y]);//寻路字典


                int tokenCount = mapData[i]["token"].Count;
                if (tokenCount == 1)
                {
                    Unit unit = InitAndInstantiateGameUnit(mapData[i]["token"][0], x, y);
                    unit.mapBlockBelow = _mapBlocks[x, y];
                    unit.gameObject.GetComponent<GameUnit.GameUnit>().owner = GameUnit.OwnerEnum.Enemy;

                    _unitsList.Add(unit);
                    _mapBlocks[x, y].AddUnit(unit);
                }
            }

            #region 得到每块地图块周围的地图块
            //for (int i = 0; i < rows; i++)
            //{
            //    for (int j = 0; j < columns; j++)
            //    {
            //        Vector2 t = new Vector2(j, i - 1);
            //        Vector2 b = new Vector2(j, i + 1);
            //        Vector2 r = new Vector2(j + 1, i);
            //        Vector2 l = new Vector2(j - 1, i);

            //        if (t.x >= 0 && t.y >= 0 && t.x < columns && t.y < rows && _mapBlocks[(int)t.x, (int)t.y].transform.childCount == 0)
            //        {
            //            mapBlockDict[new Vector2(j, i)].neighbourBlock[0] = mapBlockDict[t];
            //        }
            //        if (b.x >= 0 && b.y >= 0 && b.x < columns && b.y < rows && _mapBlocks[(int)b.x, (int)b.y].transform.childCount == 0)
            //        {
            //            mapBlockDict[new Vector2(j, i)].neighbourBlock[1] = mapBlockDict[b];
            //        }
            //        if (l.x >= 0 && l.y >= 0 && l.x < columns && l.y < rows && _mapBlocks[(int)l.x, (int)l.y].transform.childCount == 0)
            //        {
            //            mapBlockDict[new Vector2(j, i)].neighbourBlock[2] = mapBlockDict[l];
            //        }
            //        if (r.x >= 0 && r.y >= 0 && r.x < columns && r.y < rows && _mapBlocks[(int)r.x, (int)r.y].transform.childCount == 0)
            //        {
            //            mapBlockDict[new Vector2(j, i)].neighbourBlock[3] = mapBlockDict[r];
            //        }
            //    }
            //}
            #endregion
        }
        #endregion

        //更新寻路字典,避免再次遍历整个地图
        //WTF IS IT? A*是每次寻路都要根据条件重新规划路线的啊，一次寻路消耗又低到可以忽略不计，省这点算力干什么啊？？？
        //public void UpDateNeighbourBlock(Vector2 position)
        //{
        //    int j = (int)position.x;
        //    int i = (int)position.y;
        //    Vector2 t = new Vector2(j, i - 1);
        //    Vector2 b = new Vector2(j, i + 1);
        //    Vector2 r = new Vector2(j + 1, i);
        //    Vector2 l = new Vector2(j - 1, i);

        //    if (t.x >= 0 && t.y >= 0 && t.x < columns && t.y < rows)
        //    {
        //        mapBlockDict[new Vector2(t.x, t.y)].neighbourBlock[1] = mapBlockDict[position];
        //    }
        //    if (b.x >= 0 && b.y >= 0 && b.x < columns && b.y < rows)
        //    {
        //        mapBlockDict[new Vector2(b.x, b.x)].neighbourBlock[0] = mapBlockDict[position];
        //    }
        //    if (l.x >= 0 && l.y >= 0 && l.x < columns && l.y < rows)
        //    {
        //        mapBlockDict[new Vector2(l.x, l.y)].neighbourBlock[3] = mapBlockDict[position];
        //    }
        //    if (r.x >= 0 && r.y >= 0 && r.x < columns && r.y < rows)
        //    {
        //        mapBlockDict[new Vector2(r.x, r.y)].neighbourBlock[2] = mapBlockDict[position];
        //    }
        //}

        /// <summary>
        /// 获取传入寻路结点相邻的方块列表
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public List<BattleMapBlock> GetNeighbourBlock(Node node)
        {
            List<BattleMapBlock> neighbour = new List<BattleMapBlock>();
            int x = (int)node.position.x;
            int y = (int)node.position.y;
            if (GetSpecificMapBlock(x - 1, y) != null)
            {
                neighbour.Add(GetSpecificMapBlock(x - 1, y));
            }
            if (GetSpecificMapBlock(x + 1, y) != null)
            {
                neighbour.Add(GetSpecificMapBlock(x + 1, y));
            }
            if (GetSpecificMapBlock(x, y - 1) != null)
            {
                neighbour.Add(GetSpecificMapBlock(x, y - 1));
            }
            if (GetSpecificMapBlock(x, y + 1) != null)
            {
                neighbour.Add(GetSpecificMapBlock(x, y + 1));
            }
            return neighbour;
        }
        
        /// <summary>
        /// 将单位设置在MapBlock下
        /// </summary>
        public void SetUnitToMapBlock()
        {
            //TODO：完善一下
        }

        //初始地图单位
        private Unit InitAndInstantiateGameUnit(JsonData data, int x, int y)
        {
            Unit newUnit;
            GameObject _object;
            #region //弃用
            //if (data["Controler - Enemy, Friendly or Self"].ToString().Equals("Self"))
            //{
            //    Debug.Log("instantiateUnit");
            //    //this.player = _object = Instantiate(player_assete, new Vector3(x, y, 0f), Quaternion.identity);
            //    this.player = _object = Instantiate(player_assete);
            //    player.transform.SetParent(_mapBlocks[x, y].transform);
            //    player.transform.position = new Vector3(x, y, 0f);

            //}
            //if (data["Controler - Enemy, Friendly or Self"].ToString().Equals("Obstacle"))
            //{
            //    _object = Instantiate(obstacle, _mapBlocks[x, y].transform, true);
            //    _object.transform.position = new Vector3(x, y, 0f);
            //}
            //else
            //{
            //_object =Instantiate(enemys[Random.Range(0, enemys.Length)], new Vector3(x, y, 0f),Quaternion.identity);
            //_object = Instantiate(enemys[Random.Range(0, enemys.Length)], _mapBlocks[x, y].transform, true);
            //_object.transform.position = new Vector3(x, y, 0f);
            //}
            // 在单位上挂载unit 脚本
            //_object.AddComponent<Unit>();
            /*
ReadUnitDataInJason(this._unitsData[data["name"].ToString()],
                    data["Controler - Enemy, Friendly or Self"].ToString(),
                    (int)data["Damaged"],
                    newUnit);*/

            // 在单位上挂载展示数值显示脚本
            //_object.AddComponent<DisplayData>();

            // 不管是不是player单位都挂载展示范围脚本
            //_object.AddComponent<ShowRange>();
            #endregion
            //TODO:怎么没有根据所有者分别做处理
            OwnerEnum owner;
            switch (data["Controler - Enemy, Friendly or Self"].ToString())
            {
                case ("Enemy"):
                    owner = OwnerEnum.Enemy; break;
                case ("Friendly"):
                    owner = OwnerEnum.Player; break;
                case ("Self"):
                    owner = OwnerEnum.neutrality; break;
                default:
                    owner = OwnerEnum.Enemy;break;
            }
            //从对象池获取单位
            _object = GameUnitPool.Instance().GetInst(data["name"].ToString(), owner, (int)data["Damaged"]);     
            //修改单位对象的父级为地图方块
            _object.transform.SetParent(_mapBlocks[x, y].transform);
            _object.transform.localPosition = Vector3.zero;


            //TODO 血量显示 test版本, 此后用slider显示
            var TextHp = _object.transform.GetComponentInChildren<Text>();
            var gameUnit = _object.GetComponent<GameUnit.GameUnit>();
            float hp = gameUnit.hp/* - Random.Range(2, 6)*/;
            float maxHp = gameUnit.MaxHP;
            float hpDivMaxHp = hp / maxHp * 100;
            TextHp.text = string.Format("Hp: {0}%", hpDivMaxHp);              
            
            newUnit = _object.GetComponent<Unit>();
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
        //WTF it is？测试代码请专门开个测试脚本做单元测试OK？
        #region 召唤功能测试代码
        //public void ButtonClickDown()
        //{
        //    if (SummonOnBlock(Random.Range(0, 11), Random.Range(0, 11)))
        //        GFGame.UtilityHelper.Log("召唤成功", GFGame.LogColor.PURPLE);
        //    else
        //        GFGame.UtilityHelper.Log("召唤失败", GFGame.LogColor.BLUE);
        //}

        //public bool SummonOnBlock(int x = 0, int y = 1)
        //{
        //    //TODO 测试坐标 (0,0)
        //    var tempBlock = GetSpecificMapBlock(x, y);

        //    if(tempBlock.transform.childCount == 0)
        //    {
        //        var tempUnit = GameUnitPool.Instance().GetInst("ShadowSoldier_1", OwnerEnum.Enemy);


        //        if (tempUnit != null)
        //        {
        //            tempUnit.transform.parent = tempBlock.transform;
        //            tempUnit.transform.localPosition = Vector3.zero;
        //            return true;
        //        }
        //    }

        //    return false;
        //}
        #endregion
        
        /// <summary>
        /// 传入坐标，获取对应的MapBlock。坐标不合法会返回null
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public BattleMapBlock GetSpecificMapBlock(int x, int y)
        {
            if (x >= 0 && y >= 0 && x < columns && y < rows)
                return this._mapBlocks[x, y];
            return null;
        }
        /// <summary>
        /// 传入MapBlock，返回该MapBlock的坐标
        /// </summary>
        /// <param name="mapBlock"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 确定给定坐标上是否含有单位，坐标不合法会返回false，其他依据实际情况返回值
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public Boolean CheckIfHasUnits(Vector3 vector)
        {
            Debug.Log(_mapBlocks.Length);
            if (this._mapBlocks[(int)vector.x, (int)vector.y] != null && this._mapBlocks[(int)vector.x, (int)vector.y].transform.childCount != 0
                && this._mapBlocks[(int)vector.x, (int)vector.y].GetComponentInChildren<Unit>() != null &&
                this._mapBlocks[(int)vector.x, (int)vector.y].GetComponentInChildren<Unit>().id != "Obstacle"/*units_on_me.Count != 0*/)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 返回给定坐标上单位list，坐标不合法会返回null, 其他依据实际情况返回值
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public Unit GetUnitsOnMapBlock(Vector3 vector)
        {
            if (this._mapBlocks[(int)vector.x, (int)vector.y] != null && this._mapBlocks[(int)vector.x, (int)vector.y].transform.childCount != 0)
            {
                return _mapBlocks[(int)vector.x, (int)vector.y].GetComponentInChildren<Unit>();
            }
            return null;
        }
        /// <summary>
        /// 传入坐标，返回该坐标的MapBlock的Type
        /// </summary>
        /// <param name="coordinate"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 根据给定unit寻找其所处坐标，若找不到则会返回不合法坐标
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 传入unit和坐标，将Unit瞬间移动到该坐标（仅做坐标变更，不做其他处理）
        /// <param name="unit">移动的目标单位</param>
        /// <param name="worldCoordinate">地图块儿所处的世界坐标</param>
        /// <param name="gameobjectCoordinate">地图块儿自身的物体坐标</param>
        /// <returns></returns>
        /// </summary>
        public bool MoveUnitToCoordinate(Unit unit, Vector3 worldCoordinate, Vector2 gameobjectCoordinate)
        {
            foreach (Unit gameUnit in _unitsList)
            {
                if (gameUnit == unit)
                {
                    unit.mapBlockBelow.RemoveUnit(unit);

                    //你看，我上面说什么了，负面地形越多，这种代码就越难写，要时刻小心继承带来的“类爆炸”
                    //TODO: review plz
                    if (_mapBlocks[(int)gameobjectCoordinate.x, (int)gameobjectCoordinate.y] != null)
                    {
                        unit.mapBlockBelow = _mapBlocks[(int)gameobjectCoordinate.x, (int)gameobjectCoordinate.y];
                    }
                    unit.mapBlockBelow.AddUnit(unit);
                    unit.transform.localPosition = Vector3.zero;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 地图方块染色接口
        /// </summary>
        /// <param name="positions">染色的方块坐标</param>
        /// <param name="color">方块要被染的颜色</param>
        public void ColorMapBlocks(List<Vector2> positions, Color color)
        {
            foreach (Vector3 position in positions)
            {
                if (position.x < columns && position.y < rows && position.x >= 0 && position.y >= 0)
                {
                    _mapBlocks[(int)position.x, (int)position.y].gameObject.GetComponent<Image>().color = color;
                }
            }
        }


        //为什么要把战区相关的代码写进BattleMap里来，到这里这个类已经有500行了（原版）
        //原则上除了极特殊的类，每个类的长度不应该超过300行
        //战区相关的代码完全可以移去另外的类里，然后让BattaleMap持有这个战区类
        //TODO:review plz

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
                        if (unit.Name == "黑影战士")
                        {
                            Debug.Log("This WarZone has Enemy,you can`t summon");
                            return false;
                        }
                        else
                        {
                            unit = _mapBlocks[x, y].GetComponentInChildren<Unit>();
                            units.Add(unit);
                        }
                    }
                }
                else
                {
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