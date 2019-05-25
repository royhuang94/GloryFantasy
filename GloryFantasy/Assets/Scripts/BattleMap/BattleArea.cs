using IMessage;
using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using IMessage;

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
        private int _battleAreaID;//战区id
        private BattleAreaSate _battleAreaSate;//战区状态
        private List<Vector2> _battleArea;//该战区上的所有地图块坐标

        Trigger trigger;

        public BattleArea(int battleAreaID,BattleAreaSate battleAreaSate, List<Vector2> battleArea)
        {
            _battleAreaID = battleAreaID;
            _battleArea = battleArea;
            _battleAreaSate = battleAreaSate;

            //创建Trigger实例
            trigger = new BattleAreaTrigger(this.GetMsgReceiver(), this._battleAreaID);
            //注册Trigger进消息中心
            MsgDispatcher.RegisterMsg(trigger, "BattleState");
        }

        public T GetUnit<T>() where T : MonoBehaviour
        {
            throw new System.NotImplementedException();
        }

        public class BattleAreaTrigger : Trigger
        {
            int id;
            public BattleAreaTrigger(MsgReceiver speller,int _id)
            {
                register = speller;
                //初始化响应时点,为战区状态改变
                msgName = (int)MessageType.BattleSate;//
                //初始化条件函数和行为函数
                condition = Condition;
                action = Action;
                id = _id;
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
                    }                   
                    Debug.Log(string.Format("战区：{0}，当前状态：{1}",id,battleArea._battleAreaSate));

                    //占领特定id的战区胜利或失败宣告
                    //TODO 获取那个特定id
                    int testID = 1;//测试
                    if(this.id == testID && battleArea._battleAreaSate == BattleAreaSate.Player)
                    {
                        Debug.Log("you win");
                    }else if(this.id == testID && battleArea._battleAreaSate == BattleAreaSate.Enmey)
                    {
                        Debug.Log("you lose");
                    }
                }
            }
        }
    }

    public class BattleAreaData
    {
        private List<int> areas = new List<int>();
        private Dictionary<int, List<Vector2>> battleAreaDic = new Dictionary<int, List<Vector2>>();//战区id与战区相对应的字典
        public Dictionary<int, List<Vector2>> BattleAreaDic { get { return battleAreaDic; } }

        public Dictionary<int, BattleArea> battleAreas = new Dictionary<int, BattleArea>();//存储的所有战区的id和对应的战区对象，尽量用这个，不用上面旧的那个

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
                battleAreaDic.Add(i, battleArea);
            }
        }

        //存储战区
        public void StoreBattleArea(int area, Vector2 mapPos)
        {
            battleAreaDic[area].Add(mapPos);
        }

        //
        public void InitBattleArea()
        {
            foreach(int id in battleAreaDic.Keys)
            {
                List<Vector2> list = null;
                battleAreaDic.TryGetValue(id, out list);
                BattleArea battleArea = new BattleArea(id, WarZoneBelong(id), list);
                battleAreas.Add(id, battleArea);
            }
        }

        //显示战区
        public void ShowBattleZooe(Vector2 position, BattleMapBlock[,] mapBlock)
        {
            int area = mapBlock[(int)position.x, (int)position.y].area;
            List<Vector2> battleAreas = null;
            battleAreaDic.TryGetValue(area, out battleAreas);
            foreach (Vector2 pos in battleAreas)
            {
                if (WarZoneBelong(area) == BattleAreaSate.Battle || WarZoneBelong(area) == BattleAreaSate.Enmey)
                {
                    mapBlock[(int)pos.x, (int)pos.y].gameObject.GetComponent<Image>().color = Color.red;
                }
                else
                {
                    mapBlock[(int)pos.x, (int)pos.y].gameObject.GetComponent<Image>().color = Color.yellow;
                }
            }
        }

        //隐藏战区
        public void HideBattleZooe(Vector2 position, BattleMapBlock[,] mapBlock)
        {
            int area = mapBlock[(int)position.x, (int)position.y].area;
            List<Vector2> battleAreas = null;
            battleAreaDic.TryGetValue(area, out battleAreas);
            foreach (Vector2 pos in battleAreas)
            {
                mapBlock[(int)pos.x, (int)pos.y].gameObject.GetComponent<Image>().color = Color.white;
            }
        }

        //战斗胜利条件之一：战区所属权
        public BattleAreaSate WarZoneBelong(int area)
        {
            int unitAmout = 0;//战区上单位的数量
            int enemyAmout = 0;//战区上敌方单位数量
            int friendlyAmout = 0;//战区上我方单位数量
            int neutralityAmout = 0;//战区上中立单位数量
            List<Vector2> battleAreas = null;
            battleAreaDic.TryGetValue(area, out battleAreas);
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
            battleAreaDic.TryGetValue(area, out battleAreas);
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
            battleAreaDic.TryGetValue(area, out battleAreas);
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
    }
}
