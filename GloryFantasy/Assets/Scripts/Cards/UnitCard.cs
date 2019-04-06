using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameUnit
{
    /*
     * 暂时仅用单位牌，且使用超级生物的信息进行默认初始化
     */
    public class UnitCard : Card
    {

        public new string name { get; set; }
        public new string[] tag;
        public string id;
        public int atk { get; set; }
        public int hp { get; set; }
        public int mov { get; set; }
        public List<int> priority { get; set; }
        public int rng { get; set; }

        // 记录卡牌冷却回合数
        public int cooldownRounds { get; set; }
        // 存放卡牌预制件的引用
        public GameObject cardPrefabs { get; set; }
        
        private void Awake()
        {
            demo_init();
        }


        // 临时初始化函数，暂时使用超级生物的数值信息
        public void demo_init()
        {
            this.atk = 2;
            this.hp = 5;
            this.mov = 2;
            this.priority = new List<int>();
            this.priority.Add(2);
            
            this.rng = 1;
            this.tag = new []{ "生物", "精英"};
            this.name = "超级生物";
            this.id = "superCreature_01";
            
        }


        public void InitGameUnit(GameUnit unit)
        {
            unit.Name = this.name;
            unit.id = this.id;
            unit.atk = this.atk;
            unit.hp = this.hp;
            unit.mov = this.mov;
            unit.rng = this.rng;
            unit.owner = "Self";
            unit.priority = new List<int>();
            foreach (int VARIABLE in this.priority)
            {
                unit.priority.Add(VARIABLE);
            }

            unit.tag = new string[this.tag.Length];
            for (int i = 0; i < this.tag.Length; i++)
            {
                unit.tag[i] = (string)this.tag[i].Clone();
            }
        }
    }
    
}