using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using LitJson;
using System.IO;
using Unit = GameUnit.GameUnit;
using IMessage;
using UnityEngine.UI;
using GameUnit;
using System.Collections;
using GamePlay.Encounter;

namespace BattleMap
{
    public class BattleMap : UnitySingleton<BattleMap>, MsgReceiver
    {

        private void Awake()
        {
            _unitsList = new List<Unit>();
            _instance = this;
            IsColor = false;
            MapNavigator = new MapNavigator();
            battleAreaData = new BattleAreaData();
            debuffBM = new DebuffBattleMapBlovk();
            drawBattleArea = new DrawBattleArea();
            BattleMapPath = "Assets/Scripts/BattleMap/BattleMapData/";
        }

        private void Start()
        {
            InitMap();
        }


        /// <summary>
        /// 注册信息，以免第二次之后进来不注册
        /// </summary>
        public void RegisterMSG()
        {
            MsgDispatcher.RegisterMsg(
                this.GetMsgReceiver(),
                (int)MessageType.MPBegin,
                canDoMPAction,
                MpBegin,
                "Mp Begin Trigger"
            );

            MsgDispatcher.RegisterMsg(
                this.GetMsgReceiver(),
                (int)MessageType.MPEnd,
                canDoMPEndAction,
                MpEnd,
                "Mp End Trigger"
            );
            
            MsgDispatcher.RegisterMsg(
                this.GetMsgReceiver(),
                (int)MessageType.WIN,
                () => { return true; },
                exitBattleMap,
                "Win to exit Trigger"
            );
            
            MsgDispatcher.RegisterMsg(
                this.GetMsgReceiver(),
                (int)MessageType.LOSE,
                () => { return true; },
                exitBattleMap,
                "Lose to exit Trigger"
            );
        }
        
        
        /// <summary>
        /// 检测是否能进行主要阶段，现在暂时设定为永true,是主要阶段的condition函数
        /// </summary>
        /// <returns>根据实际情况确定是否能进入主要阶段</returns>
        public bool canDoMPAction()
        {
            return true;
        }       
        /// <summary>
        /// 检测是否能进行主要阶段，现在暂时设定为永true,是主要阶段的condition函数
        /// </summary>
        /// <returns>根据实际情况确定是否能进入主要阶段</returns>
        public bool canDoMPEndAction()
        {
            return true;
        }

        /// <summary>
        /// 主要阶段j开始
        /// </summary>
        public void MpBegin()
        {

        }
        /// <summary>
        /// 主要阶段结束
        /// </summary>
        public void MpEnd()
        {

        }

        public void exitBattleMap()
        {
            Debug.Log("win, ready to exit");
            SceneSwitchController.Instance().Switch("BattleMapTest", "BattleMapTest");
        }

        public void InitMap()
        {
            //下面的初始顺序不能变

            //读取并存储遭遇
            EncouterData.Instance().InitEncounter();            
            //初始化地图
            InitAndInstantiateMapBlocks();//
            //初始战区事件
            EncouterData.Instance().InitBattleFieldEvent("Forest_Shadow_1");//TODO等待对接
            //初始战区状态，战区对象并添加事件模块进入仲裁器；
            battleAreaData.InitBattleArea();           
            //初始战斗地图上的单位 
            UnitManager.InitAndInstantiateGameUnit("Forest_Shadow_1", _mapBlocks);
        }

        //初始化地图的地址
        //更改地图数据位置则需修改此处路径
        private string BattleMapPath;
        // 获取战斗地图上的所有单位
        private List<Unit> _unitsList;
        public List<Unit> UnitsList{get{return _unitsList;}}              
        private int columns;                 // 地图方块每列的数量
        private int rows;                    // 地图方块每行的数量
        public int Columns{get{return columns;}}
        public int Rows{get{return rows;}}                    
        public int BlockCount{get{return columns * rows;}}
        public bool IsColor { get; set; }//控制是否高亮战区
        private BattleMapBlock[,] _mapBlocks; 
        public Transform _tilesHolder;          // 存储所有地图单位引用的变量
        public Transform battleCanvas;
        public MapNavigator MapNavigator;//寻路类
        public BattleAreaData battleAreaData;//战区类
        public DebuffBattleMapBlovk debuffBM;//异常地图快类
        public DrawBattleArea drawBattleArea;//画战区边框
        private string[][] nstrs;//存战斗地图的数组
        public GameObject battlePanel;//战斗地图，用于初始战斗地图大小
        private string encounterID;//遭遇id

        #region 各种类型地格
        public GameObject flat;//平地
        public Sprite firing;//灼烧
        public Sprite viscous;//粘滞
        #endregion

        /// <summary>
        /// 从大地图获取遭遇id
        /// </summary>
        /// <param name="encounterID"></param>
        public void GetEncounterIDFromMainMap(string encounterID)
        {
            this.encounterID = encounterID;
        }

        /// <summary>
        /// 初始战斗地图路径
        /// </summary>
        /// <param name="mapID">地图名字</param>
        public void  InitBattleMapPath(string mapID)
        {
            BattleMapPath = BattleMapPath + mapID + ".txt";
        }

        //初始战斗地图
        private void InitAndInstantiateMapBlocks()
        {
            EncouterData.Instance().InitEncounter("Forest_Shadow_1");
            //读取战斗地图文件
            string[] strs = File.ReadAllLines(BattleMapPath);
            nstrs = new string[strs.Length][];
            for(int i = 0;i < nstrs.Length; i++)
            {
                nstrs[i] = strs[i].Split('/');
            }

            this.rows = nstrs.Length;
            this.columns = nstrs[0].Length;
            battlePanel.GetComponent<GridLayoutGroup>().constraintCount = this.columns;//初始化战斗地图大小（列数）
            _mapBlocks = new BattleMapBlock[columns, rows];
            GameObject instance = null;
            battleAreaData.GetAreas(nstrs);//存储战区id;
            //实例地图块
            for (int y = 0; y < nstrs.Length; y++)
            {
                for(int x = 0;x <nstrs[y].Length; x++)
                {
                    int mapBlockType = int.Parse(nstrs[y][x].Split('-')[0]);
                    instance = SetMapBlockType(mapBlockType, x, y);
                    instance.transform.SetParent(_tilesHolder);
                    instance.gameObject.AddComponent<BattleMapBlock>();
                    //初始化mapBlock成员
                    _mapBlocks[x, y] = instance.gameObject.GetComponent<BattleMapBlock>();
                    int area = int.Parse(nstrs[y][x].Split('-')[1]);
                    _mapBlocks[x, y].area = area;
                    _mapBlocks[x, y].x = x;
                    _mapBlocks[x, y].y = y;
                    _mapBlocks[x, y].blockType = EMapBlockType.normal;
                    //初始化地图块儿的collider组件
                    _mapBlocks[x, y].bmbCollider.init(_mapBlocks[x, y]);
                    
                    GamePlay.Gameplay.Instance().bmbColliderManager.InitBMB(_mapBlocks[x, y].bmbCollider);
                    battleAreaData.StoreBattleArea(area, new Vector2(x, y));//存储战区
                }
            }         
        }

        /// <summary>
        /// 实例不同类型的地格
        /// </summary>
        /// <param name="type"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public GameObject SetMapBlockType(int type,int x,int y)
        {
            //TODO添加跟多类型地格
            GameObject instance = null;
            switch (type)
            {
                case 1://平地
                    instance = GameObject.Instantiate(flat, new Vector3(x, y, 0f), Quaternion.identity);
                    break;
                default:
                    break;
            }
            return instance;
        }
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
            if (GetSpecificMapBlock(x - 1, y) != null && GetSpecificMapBlock(x - 1, y).units_on_me.Count == 0)
            {
                neighbour.Add(GetSpecificMapBlock(x - 1, y));
            }
            if (GetSpecificMapBlock(x + 1, y) != null && GetSpecificMapBlock(x + 1, y).units_on_me.Count == 0)
            {
                neighbour.Add(GetSpecificMapBlock(x + 1, y));
            }
            if (GetSpecificMapBlock(x, y - 1) != null && GetSpecificMapBlock(x, y - 1).units_on_me.Count == 0)
            {
                neighbour.Add(GetSpecificMapBlock(x, y - 1));
            }
            if (GetSpecificMapBlock(x, y + 1) != null && GetSpecificMapBlock(x, y + 1).units_on_me.Count == 0)
            {
                neighbour.Add(GetSpecificMapBlock(x, y + 1));
            }
            return neighbour;
        }

        /// <summary>
        /// 传入坐标，获取对应的MapBlock。坐标不合法会返回null
        /// </summary>
        /// <returns></returns>
        public BattleMapBlock GetSpecificMapBlock(Vector2 pos)
        {
            return GetSpecificMapBlock((int)pos.x, (int)pos.y);
        }
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
            for (int i = columns - 1; i >= 0; i--)
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
        /// <param name="gameobjectCoordinate">地图块儿自身的物体坐标</param>
        /// <returns></returns>
        /// </summary>
        public bool MoveUnitToCoordinate(Vector2 gameobjectCoordinate, Unit unit)
        {
            foreach (Unit gameUnit in _unitsList)
            {
                if (gameUnit == unit)
                {
                    unit.mapBlockBelow.RemoveUnit(unit);
                    if (_mapBlocks[(int)gameobjectCoordinate.x, (int)gameobjectCoordinate.y] == null)
                        return false;
                    unit.mapBlockBelow = _mapBlocks[(int)gameobjectCoordinate.x, (int)gameobjectCoordinate.y];
                    _mapBlocks[(int)gameobjectCoordinate.x, (int)gameobjectCoordinate.y].AddUnit(unit);
                    unit.transform.localPosition = Vector3.zero;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 传入unit和坐标，将Unit瞬间移动到该坐标（仅做坐标变更，不做其他处理）
        /// <param name="unit">移动的目标单位</param>
        /// <param name="gameobjectCoordinate">地图块儿自身的物体坐标</param>
        /// <returns></returns>
        /// </summary>
        public bool MoveUnitToCoordinate(Unit unit,  Vector2 gameobjectCoordinate)
        {
            foreach (Unit gameUnit in _unitsList)
            {
                if (gameUnit == unit)
                {
                    unit.mapBlockBelow.RemoveUnit(unit);
                    if (_mapBlocks[(int)gameobjectCoordinate.x, (int)gameobjectCoordinate.y] != null)
                    {
                        unit.mapBlockBelow = _mapBlocks[(int)gameobjectCoordinate.x, (int)gameobjectCoordinate.y];
                    }
                    StartCoroutine(MapNavigator.moveStepByStep(unit));                    
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// AI移动
        /// </summary>
        /// <param name="unit">目标单位</param>
        /// <param name="targetPosition">最优路径</param>
        /// <param name="callback">攻击回调</param>
        /// <returns></returns>
        public bool AIMoveUnitToCoordinate(Unit unit, List<Vector2> targetPosition, System.Action callback)
        {
            foreach (Unit gameUnit in _unitsList)
            {
                if (gameUnit == unit)
                {
                    unit.mapBlockBelow.RemoveUnit(unit);
                    if (_mapBlocks[(int)targetPosition[0].x, (int)targetPosition[0].y] != null)
                    {
                        unit.mapBlockBelow = _mapBlocks[(int)targetPosition[0].x, (int)targetPosition[0].y];
                    }
                    StartCoroutine(MapNavigator.moveStepByStepAI(unit, targetPosition, callback));                    
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

        /// <summary>
        /// 用于确定给定坐标地图块所属的接口
        /// </summary>
        /// <param name="position">合法的坐标</param>
        /// <returns>若地图块拥有单位，则返回对应的单位所属，若无，则返回中立</returns>
        public GameUnit.OwnerEnum GetMapblockBelong(Vector3 position)
        {
            if (CheckIfHasUnits(position))
            {
                return _mapBlocks[(int)position.x, (int)position.y].GetComponentInChildren<GameUnit.GameUnit>().owner;
            }

            return OwnerEnum.Neutrality;
        }

        #region 战区
        /// <summary>
        /// 战区所属权，传入一个坐标，判断该坐标所在的战区的所属权(胜利条件之一)
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public BattleAreaSate WarZoneBelong(Vector3 position)
        {
            int area = _mapBlocks[(int)position.x, (int)position.y].area;
            return battleAreaData.WarZoneBelong(area);
        }

        //战区胜利条件之一：守卫战区指定回合数
        public bool ProtectBattleZooe(int area, int curRounds, int targetRounds)
        {
            return battleAreaData.ProtectBattleZooe(area,curRounds,targetRounds);
        }

        //战区胜利条件之一：将某单位护送到指定战区/某敌人进入指定战区
        public int ProjectUnit(int area, Unit player = null, Unit enemy = null)
        {
            return battleAreaData.ProjectUnit(area,player,enemy);
        }

        //显示战区
        public void ShowAndUpdataBattleZooe()
        {
            drawBattleArea.ShowAndUpdateBattleArea();
        }

        /// <summary>
        /// 移除BattleBlock下的 unit
        /// </summary>
        public void RemoveUnitOnBlock(Unit deadUnit)
        {
            //获取死亡单位的Pos
            Vector2 unitPos = GetUnitCoordinate(deadUnit);
            //通过unitPos的坐标获取对应的地图块儿
            BattleMapBlock battleMap = GetSpecificMapBlock(unitPos);
            //移除对应地图块儿下的deadUnit
            battleMap.units_on_me.Remove(deadUnit);
        }
        #endregion 

        /// <summary>
        /// 返回我方所有单位
        /// </summary>
        /// <returns></returns>
        public List<Unit> GetFriendlyUnitsList()
        {
            List<Unit> friendlyUnits = new List<Unit>();
            foreach (Unit unit in _unitsList)
            {
                if(unit.owner == OwnerEnum.Player)
                {
                    friendlyUnits.Add(unit);
                }
            }

            return friendlyUnits;
        }


        T IMessage.MsgReceiver.GetUnit<T>()
        {
            return this as T;
        }
    }
}