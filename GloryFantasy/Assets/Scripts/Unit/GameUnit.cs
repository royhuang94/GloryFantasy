using UnityEngine;
using System;
using System.Collections.Generic;
using BattleMap;
using IMessage;

namespace GameUnit
{
    public enum OwnerEnum
    {
        Player,
        Enemy,
        Neutrality //中立
    }
    public class GameUnit : MonoBehaviour, IMessage.MsgReceiver
    {
        //文件数量超过两位数的数据不要使用ScriptableObject实现
        /// <summary>
        /// 增减单位的攻击力。
        /// </summary>
        /// <param name="delta">增加量，负数时为减少。</param>
        public void changeATK(int delta)
        {
            atk += delta;
        }
        /// <summary>
        /// 设置单位的攻击力。
        /// </summary>
        /// <param name="newNum">新的攻击力。</param>
        public void setATK(int newNum)
        {
            atk = newNum;
        }

        /// <summary>
        /// 单位的所有者
        /// </summary>
        public OwnerEnum owner;
        /// <summary>
        /// 单位攻击力
        /// </summary>
        public int atk { get; set; }
        /// <summary>
        /// 单位对应的那张牌的ID
        /// </summary>
        public string CardID { get; set; }
        /// <summary>
        /// 单位的颜色
        /// </summary>
        public string Color { get; set; }
        /// <summary>
        /// 单位的效果文字
        /// </summary>
        public string Effort { get; set; }
        /// <summary>
        /// 单位死亡后进入冷却区的冷却时间
        /// </summary>
        public int CD { get; set; }
        /// <summary>
        /// 单位的生命值上限
        /// </summary>
        public int MaxHP { get; set; }
        /// <summary>
        /// 单位生命值
        /// </summary>
        public int hp { get; set; }
        /// <summary>
        /// 单位id
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 单位移动力
        /// </summary>
        public int mov { get; set; }
        /// <summary>
        /// 单位的中文名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 单位的优先级
        /// </summary>
        public List<int> priority { get; set; }
        /// <summary>
        /// 单位的射程
        /// </summary>
        public int rng { get; set; }
        /// <summary>
        /// 单位的标签
        /// </summary>
        new public List<string> tag { get; set; }

        /// <summary>
        /// 单位的SPD修正值，迅击和滞击
        /// </summary>
        public int priSPD { get; set; }
        /// <summary>
        /// 单位的DS修正值，连击
        /// </summary>
        public int priDS { get; set; }
       
        /// <summary>
        /// 单位异能
        /// </summary>
        public List<string> abilities { get; set; }               
       
        /// <summary>
        /// 单位事件
        /// </summary>
        public List<GamePlay.Event.EventModule.EventWithWeight> eventsInfo { get; set; }

        GamePlay.Event.EventModule eventModule;
        public GamePlay.Event.EventModule EventModule
        {
            get
            {
                return eventModule;
            }
            set
            {
                eventModule = value;
            }
        }


        /// <summary>
        /// 为真单位不能攻击
        /// </summary>
        //public bool disarm { get; set; }
        public bool canNotAttack { get; set; }
        /// <summary>
        /// 为真单位不能移动
        /// </summary>
        //public bool restrain { get; set; }
        public bool canNotMove { get; set; }
        /// <summary>
        /// 单位的护甲回复值，每个回合开始给护甲值补回这个值
        /// </summary>
        public int armorRestore { get; set; }
        /// <summary>
        /// 单位的护甲值
        /// </summary>
        public int armor { get; set; }

        public BattleMapBlock mapBlockBelow;


        private Vector2 curPos = new Vector2(-1, -1);
        /// <summary>
        /// 当前单位的坐标
        /// </summary>
        public Vector2 CurPos
        {
            get
            {
                if (mapBlockBelow != null)
                {
                    curPos = mapBlockBelow.position;
                    return curPos;
                }

                return curPos;
            }
            set
            {
                curPos = value;
            }
        }
        /// <summary>
        /// 单位将要移动到的下一步坐标
        /// </summary>
        public Vector2 nextPos { get; set; }


        // TODO: 这是地图上单位的基类，请继承此类进行行为描述
        T IMessage.MsgReceiver.GetUnit<T>()
        {
            return this as T;
        }

        /// <summary>
        /// 判断单位有无死亡
        /// </summary>
        /// <returns></returns>
        public bool IsDead()
        {
            return !(hp > 0);
        }

        /// <summary>
        /// 异能携带检测
        /// </summary>
        /// <returns>带有异能 true，反之 false</returns>
        public bool IsIncludeAbility()
        {
            if (abilities != null && abilities.Count <= 0)
                return false;

            return true;
        }

        /// <summary>
        /// 判断单位id
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public override bool Equals(object other)
        {
            if(other != null && other is GameUnit)
            {
                return ((GameUnit)other).id == this.id;
            }
            return false;
        }
    }
}