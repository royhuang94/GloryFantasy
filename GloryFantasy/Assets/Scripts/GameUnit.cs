using UnityEngine;
using System;
using System.Collections.Generic;
using BattleMapManager;

namespace GameUnit
{
    public class GameUnit : MonoBehaviour
    {
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
        

        public GameUnit(string name, string id, string[] tag, 
                        int atk, int hp, int mov, int rng,
                        string owner, int priority)
        {
            this.Name = name;
            this.id = id;
            this.tag = tag;
            this.atk = atk;
            this.hp = hp;
            this.mov = mov;
            this.rng = rng;
            this.owner = owner;
            //TODO:临时对priority初始化，需要根据策划要求修改
            this.priority = new List<int>();
            this.priority.Add(priority);
        }

        public GameUnit(string name, string id,
                        int atk, int hp, int mov, int rng,
                        string owner)
        {
            this.Name = name;
            this.id = id;
            this.atk = atk;
            this.hp = hp;
            this.mov = mov;
            this.rng = rng;
            this.owner = owner;
            this.priority = new List<int>();
        }
        // TODO: 这是地图上单位的基类，请继承此类进行行为描述

        //判断单位有无死亡
        public bool IsDead()
        {
            return !(hp > 0);
        }
    }
}