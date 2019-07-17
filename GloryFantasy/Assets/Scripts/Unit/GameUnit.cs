using UnityEngine;
using System;
using System.Collections.Generic;
using BattleMap;
using GamePlay;
using IMessage;

namespace GameUnit
{
    public enum OwnerEnum
    {
        Player,
        Enemy,
        Neutrality //中立
    }

    public enum UnitState
    {
        None,
        Move,
        Attack
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
        /// 获取单位的攻击力。
        /// </summary>
        public int getATK()
        {
            if (atk < 0)
                return 0;
            return atk;
        }
        /// <summary>
        /// 增减单位的最大生命值，减少到0或以下时死亡。
        /// </summary>
        /// <param name="delta">增加量，负数时为减少。</param>
        public void changeMHP(int delta)
        {
            MaxHP += delta;
            if (MaxHP <= 0)
            {
                UnitManager.Kill(null, this);
            }
            if (hp > MaxHP)
            {
                hp = MaxHP;
            }
        }
        /// <summary>
        /// 设置单位的最大生命值。设置为0或以下时死亡。
        /// </summary>
        /// <param name="newNum">新的最大生命值。</param>
        public void setMHP(int newNum)
        {
            MaxHP = newNum;
            if (MaxHP < 0)
            {
                UnitManager.Kill(null, this);
            }
            if (hp > MaxHP)
            {
                hp = MaxHP;
            }
        }
        /// <summary>
        /// 获取单位的最大生命值。
        /// </summary>
        public int getMHP()
        {
            return MaxHP;
        }
        /// <summary>
        /// 增减单位的移动力。
        /// </summary>
        /// <param name="delta">增加量，负数时为减少。</param>
        public void changeMOV(int delta)
        {
            mov += delta;
        }
        /// <summary>
        /// 设置单位的移动力。
        /// </summary>
        /// <param name="newNum">新的移动力。</param>
        public void setMOV(int newNum)
        {
            mov = newNum;
        }
        /// <summary>
        /// 获取单位的移动力。
        /// </summary>
        public int getMOV()
        {
            if (mov < 0)
                return 0;
            return mov;
        }
        /// <summary>
        /// 增减单位的攻击范围。
        /// </summary>
        /// <param name="delta">增加量，负数时为减少。</param>
        public void changeRNG(int delta)
        {
            rng += delta;
        }
        /// <summary>
        /// 设置单位的攻击范围。
        /// </summary>
        /// <param name="newNum">新的攻击范围。</param>
        public void setRNG(int newNum)
        {
            rng = newNum;
        }
        /// <summary>
        /// 获取单位的攻击范围。
        /// </summary>
        public int getRNG()
        {
            if (rng < 0)
                return 0;
            return rng;
        }
        /// <summary>
        /// 修改单位血量，修改到0或以下的时候会导致单位死亡（造成伤害时不要用此方法，会无法追溯伤害来源）。
        /// </summary>
        public void changeHP(int delta)
        {
            hp += delta;
            
            // 变动后更新血条数值
            Gameplay.Instance().gamePlayInput.UpdateHp(this);
            
            if (hp > MaxHP)
                hp = MaxHP;
            if (hp <= 0)
                UnitManager.Kill(null, this);
        }
        public int isLeader;
        public OwnerEnum owner;
        /// <summary>
        /// 单位攻击力
        /// </summary>
        private int atk { get; set; }
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
        public bool HasCD { get; set; }
        /// <summary>
        /// 单位的生命值上限
        /// </summary>
        private int MaxHP { get; set; }
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
        private int mov { get; set; }
        /// <summary>
        /// 单位的中文名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 单位的伤害请求优先级序列
        /// </summary>
        public List<int> priority { get; set; }
        /// <summary>
        /// 单位的射程
        /// </summary>
        private int rng { get; set; }
        /// <summary>
        /// 单位的标签
        /// </summary>
        new public List<string> tag { get; set; }

        /// <summary>
        /// 单位的SPD修正值，适用到每次伤害请求
        /// </summary>
        public int priSPD { get; set; }
       
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

        public int AT { get; set; }
        public int CT { get; set; }
        public int MT { get; set; }
        public UnitState lastAction;
        /// <summary>
        /// 为真单位不能攻击
        /// </summary>
        public bool canNotAttack { get; set; }
        public bool canAttack()
        {
            return !canNotAttack && (AT > 0);
        }
        /// <summary>
        /// 为真单位不能移动
        /// </summary>
        //public bool restrain { get; set; }
        public bool canNotMove { get; set; }
        public bool canMove()
        {
            return !canNotMove && (MT > 0);
        }
        public UnitState nextAction()
        {
            if (canMove() && (lastAction == UnitState.None || 
                lastAction == UnitState.Attack || 
                (lastAction == UnitState.Move && !canAttack())))
                return UnitState.Move;
            if (canAttack() && ((lastAction == UnitState.None && !canMove()) ||
                lastAction == UnitState.Move ||
                (lastAction == UnitState.Attack && !canMove())))
                return UnitState.Attack;
            return UnitState.None;
        }
        /// <summary>
        /// 单位的护甲回复值，每个回合开始给护甲值补回这个值
        /// </summary>
        public int armorRestore { get; set; }
        /// <summary>
        /// 单位的护甲值
        /// </summary>
        public int armor { get; set; }
                    
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
        public bool IsDead { get; set; }

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