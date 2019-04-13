using UnityEngine;
using System;
using System.Collections.Generic;
using BattleMap;

namespace GameUnit
{
    public class GameUnit : MonoBehaviour
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
        public int rng { get; set; }
        
        new public string[] tag { get; set; }
        
        public string owner { get; set; }
        
        public string damaged { get; set; }        // TODO: 待确定数值后进行修改

        public BattleMapBlock mapBlockBelow;
        

        // TODO: 这是地图上单位的基类，请继承此类进行行为描述

        //判断单位有无死亡
        public bool IsDead()
        {
            return !(hp > 0);
        }
    }
}