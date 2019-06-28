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
using UnityEngine.SceneManagement;

namespace BattleMap
{
    public class BattleMap : UnitySingleton<BattleMap>, MsgReceiver
    {

        private void Awake()
        {
            _instance = this;
            IsColor = false;
            MapNavigator = new MapNavigator();
            battleAreaData = new BattleAreaData();
            debuffBM = new DebuffBattleMapBlock();
            drawBattleArea = new DrawBattleArea();
            BattleMapBlock = new BattleMapBlock();
        }

        private void Start()
        {
            InitMap(GetEncounterID());
            RegisterMSG();
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
            
            // 先默认为false，否则每次都会卸载该场景，等整合大地图需改成true
            MsgDispatcher.RegisterMsg(
                this.GetMsgReceiver(),
                (int)MessageType.WIN,
                canDoExitAction,
                () =>
                {
                    SceneSwitchController.Instance().win = true;    // 胜利，场景切换控制器保存结果，用于大地图界面显示
                    exitBattleMap();
                },
                "Win to exit Trigger"
            );
           
            MsgDispatcher.RegisterMsg(
                this.GetMsgReceiver(),
                (int)MessageType.LOSE,
                canDoExitAction,
                () =>
                {
                    SceneSwitchController.Instance().win = false;
                    exitBattleMap();
                },
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
        /// 判断是否可以退出战斗地图
        /// 如果直接用战斗地图打开，则不能退出
        /// 否则可以退回到大地图
        /// </summary>
        /// <returns></returns>
        public bool canDoExitAction()
        {
            if (SceneManager.sceneCount == 1)
                return false;
            return true;
        }
        
        /// <summary>
        /// 主要阶段开始
        /// </summary>
        public void MpBegin()
        {
            // 回合开始地图上所有单位可以移动，因此变亮
            // 这个是监听玩家回合还是整体回合，待验证，也不清楚是整体回合开始就亮还是怎么的
            List<Unit> friendlyUnits = GetFriendlyUnitsList();
            foreach (Unit unit in friendlyUnits)
            {
                UnitManager.ColorUnitOnBlock(unit.mapBlockBelow.position, Color.white);
            }
        }
        
        /// <summary>
        /// 主要阶段结束
        /// </summary>
        public void MpEnd()
        {

        }

        
        /// <summary>
        /// 退出战斗地图
        /// </summary>
        public void exitBattleMap()
        {
            Debug.Log("win, ready to exit");
            SceneSwitchController.Instance().Switch("BattleMapTest", "BattleMapTest");
        }

        public void InitMap(string encouterId)
        {
            //下面的初始顺序不能变
            this.init_encouterID = encouterId;
            _unitsList = new List<Unit>();//放在这里为了每次从遭遇选择器切换地图后，清空之前的

            //读取并存储遭遇
            EncouterData.Instance().InitEncounter(encouterId);            
            //初始化地图
            InitAndInstantiateMapBlocks(encouterId);
            //初始战区事件
            EncouterData.Instance().InitBattleFieldEvent(encouterId);
            //初始战区状态，战区对象并添加事件模块进入仲裁器；
            battleAreaData.InitBattleArea(encouterId);           
            //初始战斗地图上的单位 
            UnitManager.InitAndInstantiateGameUnit(encouterId, _mapBlocks);
            //该次遭遇中的一些临时数值
            EncouterData.Instance().dataOfThisBattle.InitData(encouterId);
            //设置回合为第一回合
            GamePlay.Gameplay.Instance().roundProcessController.SetFristRound();
            //一直显示战区所属
            drawBattleArea.ShowAndUpdateBattleArea();

            ScaleBattleMap();
        }

        /// <summary>
        /// 重新根据遭遇文件生成新的战斗地图
        /// </summary>
        /// <param name="encouterID"></param>
        public void RestatInitMap(string encouterID)
        {
            GamePlay.Gameplay.Instance().eventScroll.Clear();
            //初始一个遭遇id，供其他地方使用
            init_encouterID = encouterID;
            //删除之前的地图
            for (int i = 0; i < blocks.Count; i++)
                Destroy(blocks[i]);
            Destroy(BattleMapPanel);
            //重新生成
            InitMap(encouterID);
        }

        #region 变量
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
        public BattleMapBlock BattleMapBlock;
        public MapNavigator MapNavigator;//寻路类
        public BattleAreaData battleAreaData;//战区类
        public DebuffBattleMapBlock debuffBM;//异常地图快类
        public DrawBattleArea drawBattleArea;//画战区边框
        private string[][] nstrs;//存战斗地图的数组
        private string init_encouterID;//该次遭遇的遭遇id;
        public string EncouterID { get { return init_encouterID; } }
        public List<GameObject> blocks;//该次遭遇中的所有地图块实例
        GameObject BattleMapPanel;
        #endregion

        #region 各种类型地格
        public GameObject flat;//平地
        public Sprite[] flats;//各种颜色平地；
        public Sprite firing;//灼烧
        public Sprite viscous;//粘滞
        #endregion

        /// <summary>
        /// 获取遭遇id
        /// </summary>
        /// <returns></returns>
        private string GetEncounterID()
        {
            return SceneSwitchController.Instance().encounterId == null ? "planeshadow_1" : SceneSwitchController.Instance().encounterId;
            //return "Plain_Shadow_1";
        }

        //初始战斗地图
        private void InitAndInstantiateMapBlocks(string encouterId)
        {
            BattleMapPanel = new GameObject("BattleMap");
            BattleMapPanel.transform.position = Vector3.zero;
            BattleMapPanel.AddComponent<DragBattleMap>();//缩放拖动组件
            //战斗地图路径
            string battleMapPath = "BattleMapData/" + EncouterData.Instance()._encounterData[encouterId].mapID;

            //读取战斗地图文件
            string[] strs = Resources.Load<TextAsset>(battleMapPath).text.Split('\n');
            int length = strs.Length;
            if (string.IsNullOrEmpty(strs[strs.Length - 1]))
                length--;
            nstrs = new string[length][];
            
            for(int i = 0;i < nstrs.Length; i++)
            {
                nstrs[i] = strs[i].Split('/');
            }
            blocks = new List<GameObject>();

            this.rows = length;
            this.columns = nstrs[0].Length;
           
            float flatSize = flat.GetComponent<SpriteRenderer>().size.x; //获得地砖的图片边长
            Vector2 _leftTopPos = new Vector2((-columns) / 2f * flatSize + flatSize/2, (-rows) / 2f * flatSize-flatSize/2);
            //battlePanel.GetComponent<GridLayoutGroup>().constraintCount = this.columns;//初始化战斗地图大小（列数）
            _mapBlocks = new BattleMapBlock[columns, rows];
            GameObject instance = null;
            battleAreaData.GetAreas(nstrs);//存储战区id;
            //实例地图块
            for (int y = 0; y < length; y++)
            {
                for(int x = 0;x <nstrs[y].Length; x++)
                {
                    int mapBlockType = int.Parse(nstrs[y][x].Split('-')[0]);//地格类型（比如平地）
                    int mapBlockType_type = int.Parse(nstrs[y][x].Split('-')[1]);//地格类型的类型（比如平地还有不同的平地类型）
                    instance = SetMapBlockType(mapBlockType,mapBlockType_type, _leftTopPos.x + x * flatSize, _leftTopPos.y + (nstrs.Length - y) * flatSize);
                    instance.transform.SetParent(BattleMapPanel.transform);
                    instance.gameObject.AddComponent<BattleMapBlock>();
                    blocks.Add(instance);
                    //初始化mapBlock成员
                    _mapBlocks[x, y] = instance.gameObject.GetComponent<BattleMapBlock>();
                    int area = int.Parse(nstrs[y][x].Split('-')[2]);
                    _mapBlocks[x, y].area = area;
                    _mapBlocks[x, y].x = x;
                    _mapBlocks[x, y].y = y;
                    _mapBlocks[x, y].blockType = EMapBlockType.normal;
                    //初始化地图块儿的collider组件
                    //_mapBlocks[x, y].bmbCollider.init(_mapBlocks[x, y]);
                    
                    //GamePlay.Gameplay.Instance().bmbColliderManager.InitBMB(_mapBlocks[x, y].bmbCollider);
                    battleAreaData.StoreBattleArea(area, new Vector2(x, y));//存储战区

                }
            }         
        }

        //处理战斗地图缩放
        private void ScaleBattleMap()
        {
            //战斗地图总的长高，以6*10的大小为标准
            float block_size = flat.GetComponent<SpriteRenderer>().size.x; //单个地砖的边长（图片边长）
            float total_length = block_size * 10 * 0.7f;
            float total_heigth = block_size * 6 * 0.7f;

            //处理缩放和位置，以高度为基准来缩放，长度是足够的
            //float _scale = total_heigth / (block_size * rows);
            //if (_scale < 0.7f)
            //    _scale = 0.7f;
            //BattleMapPanel.transform.localScale = new Vector3(_scale, _scale, _scale);
            if(rows < 5)
                BattleMapPanel.transform.localScale = new Vector3(0.84f, 0.84f, 0.84f);
            else
                BattleMapPanel.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
            if(rows >=10)
                BattleMapPanel.transform.position = new Vector3(0f, 6.7f, 0f);//标准位置
            else
                BattleMapPanel.transform.position = new Vector3(0f, 1.5f, 0f);//标准位置

        }
        /// <summary>
        /// 实例不同类型的地格
        /// </summary>
        /// <param name="type"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public GameObject SetMapBlockType(int type,int type_type,float x, float y)
        {
            //TODO添加跟多类型地格
            GameObject instance = null;
            switch (type)
            {
                case 0://没有地格
                    instance = GameObject.Instantiate(flat, new Vector3(x, y, 0f), Quaternion.identity);
                    instance.SetActive(false);
                    break;
                case 1://平地
                    SpriteRenderer image = flat.transform.GetComponent<SpriteRenderer>();
                    image.sprite = flats[type_type];
                    instance = GameObject.Instantiate(flat, new Vector3(x, y, 0f), Quaternion.identity);
                    break;
                default:
                    break;
            }
            return instance;
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
            if (vector.x < columns && vector.y < rows
                && this._mapBlocks[(int)vector.x, (int)vector.y] != null&& this._mapBlocks[(int)vector.x, (int)vector.y].transform.childCount != 0
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
            if (CheckIfHasUnits(vector))
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
        /// 传入unit和坐标，将Unit一格一格移动到该坐标（仅做坐标变更，不做其他处理）
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


        #region AI优化
        /// <summary>
        /// 让正准备移动的单位等待一阵后移动
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="targetPosition"></param>
        /// <param name="callback"></param>
        /// <param name="waitTime"></param>
        /// <returns></returns>
        private IEnumerator WaitAndPrint(Unit unit, List<Vector2> targetPosition, System.Action callback,float waitTime = 0.2f)
        {
            yield return new WaitForSeconds(waitTime);
            yield return AIMoveUnitToCoordinate(unit, targetPosition, callback);
        }
        /// <summary>
        /// 同步调用协程WaitAndPrint
        /// 让正准备移动的单位等待一阵后移动
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="targetPosition"></param>
        /// <param name="callback"></param>
        public void AIMoveCondition(Unit unit, List<Vector2> targetPosition, System.Action callback)
        {
            StartCoroutine(WaitAndPrint(unit, targetPosition, callback));
        }
        #endregion
        /// <summary>
        /// AI移动
        /// </summary>
        /// <param name="unit">目标单位</param>
        /// <param name="targetPosition">最优路径</param>
        /// <param name="callback">攻击回调</param>
        /// <returns></returns>
        public IEnumerator AIMoveUnitToCoordinate(Unit unit, List<Vector2> targetPosition, System.Action callback)
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
                    yield return StartCoroutine(MapNavigator.moveStepByStepAI(unit, targetPosition, callback));                    
                    //return true;
                }
            }
            yield return new WaitForSeconds(0.2f);
            //return false;
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
                    _mapBlocks[(int)position.x, (int)position.y].gameObject.GetComponent<SpriteRenderer>().color = color;
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
        public BattleAreaState WarZoneBelong(Vector3 position)
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

        //隐藏战区
        public void HideBattleZooe()
        {
            drawBattleArea.HideBattleArea();
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
        /// <summary>
        /// 返回敌方所有单位
        /// </summary>
        /// <returns></returns>
        public List<Unit> GetEnemyUnitsList()
        {
            List<Unit> enemiesUnits = new List<Unit>();
            foreach (Unit unit in _unitsList)
            {
                if(unit.owner == OwnerEnum.Enemy)
                {
                    enemiesUnits.Add(unit);
                }
            }

            return enemiesUnits;
        }


        T IMessage.MsgReceiver.GetUnit<T>()
        {
            return this as T;
        }
    }
}