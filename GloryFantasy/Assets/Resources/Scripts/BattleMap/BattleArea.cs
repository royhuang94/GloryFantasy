using IMessage;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GamePlay.Event;
using GamePlay.Encounter;


/// <summary>
/// 战区
/// </summary>
/// 
namespace BattleMap
{
    public enum BattleAreaSate//战区所属状态
    {
        Player,//属于玩家
        Enmey,//属于敌人
        Neutrality,//中立
        Battle,//处于争夺
    }

    public class BattleArea : IMessage.MsgReceiver
    {
        public int _battleAreaID;//战区id
        public BattleAreaSate _battleAreaSate;//战区状态
        public List<Vector2> _battleArea;//该战区上的所有地图块坐标
        public string[] _TID;//该战区的triggerID
        public List<EventModule.EventWithWeight> _modules;//该战区的事件

        Trigger trigger;

        /// <summary>
        /// 事件具体效果实施的次数
        /// 强化/弱化
        /// </summary>
        public int delta_x_amount { get; set; }
        /// <summary>
        /// 事件具体效果实施的强度
        /// 强化/弱化
        /// </summary>
        public int delta_y_strenth { get; set; }

        public BattleArea(int battleAreaID,BattleAreaSate battleAreaSate, List<Vector2> battleArea,string[] tid,List<EventModule.EventWithWeight> modules)
        {
            _battleAreaID = battleAreaID;
            _battleArea = battleArea;
            _battleAreaSate = battleAreaSate;
            _TID = tid;
            _modules = modules;

            //创建Trigger实例
            trigger = new BattleAreaTrigger(this.GetMsgReceiver(), this._battleAreaID,tid);
            //注册Trigger进消息中心
            MsgDispatcher.RegisterMsg(trigger, "BattleState");
        }

        public T GetUnit<T>() where T : MonoBehaviour
        {
            return this as T;
        }

        public class BattleAreaTrigger : Trigger
        {
            int id;//战区id
            string[] trrigerIDs;
            public BattleAreaTrigger(MsgReceiver speller,int _id,string[] _trrigerIDs)
            {
                register = speller;
                //初始化响应时点,为战区状态改变
                msgName = (int)MessageType.BattleSate;
                //初始化条件函数和行为函数
                condition = Condition;
                action = Action;
                id = _id;
                trrigerIDs = _trrigerIDs;
            }

            private bool Condition()
            {
                return true;
            }

            private void Action()
            {
                BattleArea battleArea =null;
                BattleMap.Instance().battleAreaData.battleAreas.TryGetValue(id, out battleArea);
                Debug.Log(string.Format("战区：{0}，之前状态：{1}", id, battleArea._battleAreaSate));
                if (battleArea._battleAreaSate == BattleMap.Instance().battleAreaData.WarZoneBelong(id))
                {
                    //该战区所属状态没改变
                }
                else
                {
                    if(BattleMap.Instance().battleAreaData.WarZoneBelong(id) != BattleAreaSate.Neutrality)//中立就不更新
                    {
                        battleArea._battleAreaSate = BattleMap.Instance().battleAreaData.WarZoneBelong(id);//更新该战区所属状态
                        BattleMap.Instance().ShowAndUpdataBattleZooe();
                    }                   
                    Debug.Log(string.Format("战区：{0}，当前状态：{1}",id,battleArea._battleAreaSate));

                    if(battleArea._TID != null)
                    {
                        for (int i = 0; i < battleArea._TID.Length; i++)
                        {
                            if (battleArea._TID[i] == "MainBF_Friendly" && battleArea._battleAreaSate == BattleAreaSate.Enmey)
                            {
                                Debug.Log("you lose");
                            }
                            else if (battleArea._TID[i] == "MainBF_Enemy" && battleArea._battleAreaSate == BattleAreaSate.Player)
                            {
                                Debug.Log("you win");
                            }
                        }
                    }   
                }
            }
        }
    }

    public class BattleAreaData
    {
        #region
        private List<int> areas = new List<int>();
        public Dictionary<int, List<Vector2>> BattleAreaDic = new Dictionary<int, List<Vector2>>();
        #endregion
        public Dictionary<int, BattleArea> battleAreas;//存储的所有战区的id和对应的战区对象，尽量用这个，不用上面旧的那个
        private bool isBattleAreaShow = true;

        //获取地图块所属战区
        public void GetAreas(string[][] nstrs)
        {
            for (int y = 0; y < nstrs.Length; y++)
            {
                for (int x = 0; x < nstrs[y].Length; x++)
                {
                    int area = int.Parse(nstrs[y][x].Split('-')[1]);
                    areas.Add(area);
                }
            }
            //移除重复元素
            for (int i = 0; i < areas.Count; i++)
            {
                for (int j = areas.Count - 1; j > i; j--)
                {

                    if (areas[i] == areas[j])
                    {
                        areas.RemoveAt(j);
                    }
                }
            }

            //动态增加战区数量,战区序号从1开始
            for (int i = 1; i <= areas.Count; i++)
            {
                List<Vector2> battleArea = new List<Vector2>();//同一个战区上的所有地图块坐标
                BattleAreaDic.Add(i, battleArea);
            }
        }

        //存储战区
        public void StoreBattleArea(int area, Vector2 mapPos)
        {
            BattleAreaDic[area].Add(mapPos);
        }

        /// <summary>
        /// 初始战区对象
        /// </summary>
        public void InitBattleArea()
        {
            battleAreas = new Dictionary<int, BattleArea>();
            foreach (int id in BattleAreaDic.Keys)
            {
                List<Vector2> list = null;
                BattleAreaDic.TryGetValue(id, out list);
                string[] trrigers = null;
                trrigers = GamePlay.Encounter.EncouterData.Instance().GetBattleAreaTriggerByRegionID(id);
                List<EventModule.EventWithWeight> models = EncouterData.Instance().GetBattleFieldEvent(id);
                BattleArea battleArea = new BattleArea(id, WarZoneBelong(id), list,trrigers,models);
                battleAreas.Add(id, battleArea);

                if (battleArea._modules == null)
                    continue;
                EventModule eventModule = new EventModule(battleArea);
                eventModule.AddEvent(models);

                GamePlay.Gameplay.Instance().eventScroll.AddEventModule(eventModule);
            }
            //生成战区内框
            BattleMap.Instance().drawBattleArea.GetBattleAreaBorder();
        }
      

        //战区所属权
        public BattleAreaSate WarZoneBelong(int area)
        {
            int unitAmout = 0;//战区上单位的数量
            int enemyAmout = 0;//战区上敌方单位数量
            int friendlyAmout = 0;//战区上我方单位数量
            int neutralityAmout = 0;//战区上中立单位数量
            List<Vector2> battleAreas = null;
            BattleAreaDic.TryGetValue(area, out battleAreas);
            foreach (Vector2 pos in battleAreas)
            {
                int x = (int)pos.x;
                int y = (int)pos.y;
                if (BattleMap.Instance().CheckIfHasUnits(pos))
                {
                    unitAmout++;
                    GameUnit.GameUnit unit = BattleMap.Instance().GetUnitsOnMapBlock(new Vector2(x,y));
                    if (unit.owner == GameUnit.OwnerEnum.Enemy)
                        enemyAmout++;
                    else if (unit.owner == GameUnit.OwnerEnum.Player)
                        friendlyAmout++;
                    else
                        neutralityAmout++;
                }
            }
            if (unitAmout == 0)
            {
                //中立状态，只存在于初始化
                return BattleAreaSate.Neutrality;
            }
            if (enemyAmout == unitAmout - neutralityAmout)
            {
                //该战区被敌方控制
                return BattleAreaSate.Enmey;
                
            }
            else if (friendlyAmout == unitAmout - neutralityAmout)
            {
                //该战区被玩家控制
                return BattleAreaSate.Player;
            }
            else
                return BattleAreaSate.Battle;
        }

        /// <summary>
        ///  战斗胜利条件之一：守卫某战区存活指定回合数
        /// </summary>
        /// <param name="area">要守卫的战区</param>
        /// <param name="curRounds">当前回合数</param>
        /// <param name="targetRounds">要守卫的目标回合数</param>
        /// <param name="mapBlock"></param>
        /// <returns></returns>
        public bool ProtectBattleZooe(int area, int curRounds, int targetRounds)
        {
            int unitAmout = 0;//该战区上我方单位数量
            List<Vector2> battleAreas = null;
            BattleAreaDic.TryGetValue(area, out battleAreas);
            foreach (Vector2 pos in battleAreas)
            {
                if (BattleMap.Instance().CheckIfHasUnits(pos))
                {
                    GameUnit.GameUnit unit = BattleMap.Instance().GetUnitsOnMapBlock(pos);
                    if (unit.owner == GameUnit.OwnerEnum.Player && curRounds <= targetRounds)
                    {
                        unitAmout++;
                    }
                }
            }
            if (unitAmout == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 战斗胜利条件与失败之一:将某单位护送到指定战区/某敌人进入指定战区
        /// </summary>
        /// <param name="area">玩家单位或敌方单位要到到达的目标战区（同一战区）</param>
        /// <param name="player">哪个玩家的单位到达</param>
        /// /// <param name="enemy">哪个敌方单位到达</param>
        /// <returns></returns>
        public int ProjectUnit(int area, GameUnit.GameUnit player, GameUnit.GameUnit enemy)//不好直接返回bool值，万一都还没见进入这个战区该返回什么？暂时就这样吧
        {
            List<Vector2> battleAreas = null;
            BattleAreaDic.TryGetValue(area, out battleAreas);
            foreach (Vector2 pos in battleAreas)
            {
                if (BattleMap.Instance().CheckIfHasUnits(pos))
                {
                    GameUnit.GameUnit tempUnit = BattleMap.Instance().GetUnitsOnMapBlock(pos);
                    if (player != null && tempUnit.id == player.id)
                    {
                        return 0;//胜利
                    }

                    else if (enemy != null && tempUnit.id == enemy.id)
                    {
                        return 1;//失败
                    }
                }
            }
            return -1;//都还没进入指定战区
        }

        /// <summary>
        /// 通过战区id获取该战区对象
        /// </summary>
        /// <param name="reginID"></param>
        /// <returns></returns>
        public BattleArea GetBattleAreaByID(int reginID)
        {
            BattleArea battleArea;
            if (battleAreas.ContainsKey(reginID))
            {
                battleAreas.TryGetValue(reginID, out battleArea);
            }
            else
            {
                Debug.Log("该战区不存在");
                return null;
            }
            //Debug.Log(battleArea._modules[0].EventName);
            return battleArea;
        }

        /// <summary>
        /// 通过战区id获取该战区中的地图块坐标
        /// </summary>
        /// <param name="reginID"></param>
        /// <returns></returns>
        public List<Vector2> GetBattleAreaAllPosByID(int reginID)
        {
            List<Vector2> vector2s = new List<Vector2>();
            if (BattleAreaDic.ContainsKey(reginID))
            {
                BattleAreaDic.TryGetValue(reginID, out vector2s);
            }
            else
            {
                Debug.Log("该战区不存在");
                return null;
            }
            return vector2s;
        }
    }
}
