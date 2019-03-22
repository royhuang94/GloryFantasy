using UnityEngine;
using System;
using System.Collections.Generic;

namespace GameUnit
{
    public class GameUnit : MonoBehaviour
    {
        public string Name { get; set; }
        public string id { get; set; }
        new public string[] tag { get; set; }
        public int cost { get; set; }
        public int atk { get; set; }
        public int def { get; set; }
        public int mov { get; set; }
        public int rng { get; set; }
        public string owner { get; set; }
        public string[] triggered { get; set; }
        public string[] active { get; set; }
        public int ralatedCardID { get; set; }
        //TODO:对priority进行初始化
        public List<int> priority { get; set; }

        public GameUnit(string name, string id, string[] tag, 
                        int cost, int atk, int def, int mov, int rng,
                        string owner, string[] triggered, string[] active, int ralatedCardID)
        {
            this.Name = name;
            this.id = id;
            this.tag = tag;
            this.cost = cost;
            this.atk = atk;
            this.def = def;
            this.mov = mov;
            this.rng = rng;
            this.owner = owner;
            this.triggered = triggered;
            this.active = active;
            this.ralatedCardID = ralatedCardID;
            //TODO:临时对priority初始化，需要根据策划要求修改
            priority = new List<int>(); priority.Add(1);
        }

        public GameUnit(string name, string id,
                        int cost, int atk, int def, int mov, int rng,
                        string owner, int ralatedCardId)
        {
            this.Name = name;
            this.id = id;
            this.cost = cost;
            this.atk = atk;
            this.def = def;
            this.mov = mov;
            this.rng = rng;
            this.owner = owner;
            this.ralatedCardID = ralatedCardId;

            this.triggered = null;
            this.active = null;
            this.tag = null;
        }
        // TODO: 这是地图上单位的基类，请继承此类进行行为描述

        //判断单位有无死亡
        public bool IsDead()
        {
            return !(def > 0);
        }
    }
}