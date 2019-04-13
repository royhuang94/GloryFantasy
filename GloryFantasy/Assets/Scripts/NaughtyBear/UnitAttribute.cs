using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NBearUnit
{
    [CreateAssetMenu(menuName = "ScriptableObject/Battle unit attributes")]
    public class UnitAttribute : 
        ScriptableObject
    {
        public string uID;
        public string tag;
        public string uName;

        public int hp;
        public int maxHp;
        public int priority;
        public int mov;    //行动力
        public int atk;
        public int rng;

    }

}

