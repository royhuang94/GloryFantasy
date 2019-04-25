using UnityEngine;
using System;
using System.Collections.Generic;
using BattleMap;

namespace GameUnit
{
    public enum OwnerEnum
    {
        Player,
        Enemy,
        neutrality //中立
    }

    public class GameUnit : MonoBehaviour, IMessage.MsgReceiver
    {
        //文件数量超过两位数的数据不要使用ScriptableObject实现

        /// <summary>
        /// 单位属性，决定废弃，请勿使用
        /// </summary>
        //public NBearUnit.UnitAttribute UnitAttribute;

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
        /// 标记单位是否为飞行单位
        /// </summary>
        public bool fly { get; set; }
        
        /// <summary>
        /// ？？？？什么玩意儿
        /// </summary>
        public string damaged { get; set; }       

        /// <summary>
        /// 为真单位不能攻击
        /// </summary>
        public bool disarm { get; set; }
        /// <summary>
        /// 为真单位不能移动
        /// </summary>
        public bool restrain { get; set; }
        /// <summary>
        /// 单位的护甲回复值，每个回合开始给护甲值补回这个值
        /// </summary>
        public int armorRestore { get; set; }
        /// <summary>
        /// 单位的护甲值
        /// </summary>
        public int armor { get; set; }

        public BattleMapBlock mapBlockBelow;

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
    }
}