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

        public string id;

        // 记录卡牌冷却回合数
        public int cooldownRounds { get; set; }
        // 存放卡牌预制件的引用
        public GameObject cardPrefabs { get; set; }

    }
    
}