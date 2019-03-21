using UnityEngine;
using System;

namespace GameUnit
{
    public abstract class GameUnit : MonoBehaviour
    {
        public string Name { get; set; }
        public string id { get; set; }
        public string[] tag { get; set; }
        public int cost { get; set; }
        public int atk { get; set; }
        public int def { get; set; }
        public int mov { get; set; }
        public int rng { get; set; }
        public string owner { get; set; }
        public string[] triggered { get; set; }
        public string[] active { get; set; }
        public int ralatedCardID { get; set; }
        
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
        }
        // TODO: 这是地图上单位的基类，请继承此类进行行为描述
    }
}