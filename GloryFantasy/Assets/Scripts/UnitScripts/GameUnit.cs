using UnityEngine;
using System;
using System.Collections.Generic;
using BattleMap;

namespace GameUnit
{
    public class GameUnit : MonoBehaviour, IMessage.MsgReceiver
    {
        public NBearUnit.UnitAttribute unitAttribute;

        //TODO 考虑属性应该通过ScriptableObject还是通过下面方法
        public int atk { get; set; }
        public int hp { get; set; }
        public string id { get; set; }
        public int mov { get; set; }
        public string Name { get; set; }
        
        //TODO:对priority进行初始化
        public List<int> priority { get; set; }
        //SPD修正值，迅击和滞击
        public int priSPD { get; set; }
        //DS修正值，连击
        public int priDS { get; set; }

        //是否为飞行单位
        public bool fly { get; set; }

        public int rng { get; set; }
        
        new public string[] tag { get; set; }
        
        public string owner { get; set; }
        
        public string damaged { get; set; }        // TODO: 待确定数值后进行修改

        /// <summary>
        /// 不能攻击
        /// </summary>
        public bool disarm { get; set; }
        /// <summary>
        /// 不能移动
        /// </summary>
        public bool restrain { get; set; }
        /// <summary>
        /// 护甲回复值，每个回合开始给护甲值补回这个值
        /// </summary>
        public int armorRestore
        {
            get { return armorRestore; }
            set { if (value > armorRestore) armorRestore = value; }
        }
        /// <summary>
        /// 护甲值
        /// </summary>
        public int armor { get; set; }

        public BattleMapBlock mapBlockBelow;

        // TODO: 这是地图上单位的基类，请继承此类进行行为描述

        public GameUnit GetGameUnit()
        {
            return this;
        }

        //判断单位有无死亡
        public bool IsDead()
        {
            return !(hp > 0);
        }
    }
}