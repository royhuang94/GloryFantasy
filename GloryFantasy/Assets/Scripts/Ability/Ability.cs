using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using LitJson;
using GameUtility;
using System.IO;
using GamePlay;

namespace Ability
{
    public delegate void EffectAction();
    /// <summary>
    /// 效果堆叠
    /// </summary>
    static class EffectStack
    {
        public static List<EffectAction> actions;
        private static bool _canPumpActions;
        public static void push(EffectAction action)
        {
            actions.Add(action);
        }

        public static void turnsOff()
        {
            _canPumpActions = false;
        }

        public static void turnsOn()
        {
            _canPumpActions = true;
            while (actions.Count > 0)
            {
                actions[actions.Count - 1]();
                actions.RemoveAt(actions.Count - 1);
                if (!_canPumpActions)
                    break;
            }
        }
    }
    [Serializable]
    /// <summary>
    /// 异能变量类
    /// </summary>
    public class Variable
    {
        /// <summary>
        /// 距离
        /// </summary>
        public Nullable<Int32> Range;
        /// <summary>
        /// 伤害量
        /// </summary>
        public Nullable<Int32> Damage;
        /// <summary>
        /// 数目
        /// </summary>
        public Nullable<Int32> Amount;
        /// <summary>
        /// 抽牌数
        /// </summary>
        public Nullable<Int32> Draws;
        /// <summary>
        /// 回合数
        /// </summary>
        public Nullable<Int32> Turns;

        /// <summary>
        /// 范围大小
        /// </summary>
        public Nullable<Int32> Area;
        //public Nullable<Vector2> Area;
        /// <summary>
        /// 治疗量
        /// </summary>
        public Nullable<Int32> Curing;
    }
    /// <summary>
    /// 异能在数据库的存储格式
    /// </summary>
    public class AbilityFormat
    {
        ///// <summary>
        ///// 对象列表
        ///// </summary>
        //public List<AbilityTarget> AbilityTargetList;
        /// <summary>
        /// 异能可用变量
        /// </summary>
        public Variable Variable;

        /// <summary>
        /// 异能ID
        /// </summary>
        public string AbilityID;
        /// <summary>
        /// 组号
        /// </summary>
        public int Group;
        /// <summary>
        /// 异能中文名
        /// </summary>
        /// <param name="_abilityID"></param>
        public string AbilityName;
        /// <summary>
        /// 异能描述
        /// </summary>
        public string Description;
        /// <summary>
        /// 异能TriggerID
        /// </summary>
        /// <param name="_abilityID"></param>
        public string TriggerID;

        public AbilityFormat(string _abilityID)
        {
            AbilityID = _abilityID;
            //AbilityTargetList = new List<AbilityTarget>();
            Variable = new Variable();
        }

    }

    public class Ability : MonoBehaviour, GameplayTool
    {
        /// <summary>
        /// 异能可用变量
        /// </summary>
        public Variable Variable;

        /// <summary>
        /// 异能ID
        /// </summary>
        public string AbilityID;
        /// <summary>
        /// 异能中文名
        /// </summary>
        /// <param name="_abilityID"></param>
        public string AbilityName;
        /// <summary>
        /// 异能描述
        /// </summary>
        public string Description;

        void Awake()
        {
            //AbilityTargetList = new List<AbilityTarget>();
            //TargetList = new List<Vector2>();
        }
        
        /// <summary>
        /// 所有异能必须重写此方法，在此方法内实现初始化
        /// </summary>
        /// <param name="abilityId">异能id</param>
        public virtual void Init(string abilityId)
        {

        }
    }
}