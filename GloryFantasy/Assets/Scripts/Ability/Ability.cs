﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using LitJson;
using GameUtility;
using System.IO;
using GamePlay;

namespace Ability
{
    [Serializable]
    /// <summary>
    /// 异能变量类
    /// </summary>
    public class AbilityVariable
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
    /// 异能对象类型枚举
    /// </summary>
    public enum TargetType
    {
        Enemy,
        Friendly,
        Field,
        All
    }

    /// <summary>
    /// 非友方目标，战区主控制者类型
    /// </summary>
    public enum ControllerType
    {
        Enemy,    //敌人
        Friendly, //友军
        Neutral,  // 中立
        All        //任意类型
  }

    [Serializable]
    /// <summary>
    /// 异能对象类
    /// </summary>
    public class AbilityTarget
    {
        public TargetType TargetType = TargetType.All;
        /// <summary>
        /// 若本目标为使用者，则ControllerType无用
        /// </summary>
        public ControllerType ControllerType = ControllerType.All;
        public bool IsSpeller = false;
        public bool IsTarget = false;

        public List<string> color;
        public List<string> tag;

        public void SetControllerType(string controller)
        {
            switch (controller)
            {
                case "任意":
                    ControllerType = ControllerType.All;
                    break;
                case "友军":
                    ControllerType = ControllerType.Friendly;
                    break;
                case "中立":
                    ControllerType = ControllerType.Neutral;
                    break;
                case "敌人":
                    ControllerType = ControllerType.Enemy;
                    break;
                default:
                    ControllerType = ControllerType.All;
                    break;
            }
        }

        public AbilityTarget(string _targetType, bool _isSpeller, bool _isTarget)
        {
            TargetType targetType = TargetType.All;
            switch (_targetType)
            {
                case ("敌人"): targetType = TargetType.Enemy; break;
                case ("友军"): targetType = TargetType.Friendly; break;
                case ("地形"): targetType = TargetType.Field; break;
                case ("所有单位"): targetType = TargetType.All; break;
            }
            this.TargetType = targetType;
            this.IsSpeller = _isSpeller;
            this.IsTarget = _isTarget;

            color = null;
            tag = null;
        }

        public AbilityTarget(TargetType _targetType, bool _isSpeller, bool _isTarget)
        {
            this.TargetType = _targetType;
            this.IsSpeller = _isSpeller;
            this.IsTarget = _isTarget;
        }
    }
    /// <summary>
    /// 异能在数据库的存储格式
    /// </summary>
    public class AbilityFormat
    {
        /// <summary>
        /// 对象列表
        /// </summary>
        public List<AbilityTarget> AbilityTargetList;
        /// <summary>
        /// 异能可用变量
        /// </summary>
        public AbilityVariable AbilityVariable;

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
            AbilityTargetList = new List<AbilityTarget>();
            AbilityVariable = new AbilityVariable();
        }

    }

    public class Ability : MonoBehaviour, GameplayTool
    {
        /// <summary>
        /// Ability自行规定的Condition条件函数
        /// </summary>
        public List<Func<object, bool>> MyTargetConstraintList = new List<Func<object, bool>>();
        /// <summary>
        /// 对象列表从表格获得的约束
        /// </summary>
        public List<AbilityTarget> AbilityTargetList;
        /// <summary>
        /// 异能可用变量
        /// </summary>
        public AbilityVariable AbilityVariable;

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

        ///// <summary>
        /// 玩家选择的对象
        /// </summary>
        //public List<Vector2> TargetList;
        /// <summary>
        /// 异能发动者
        /// </summary>
        public GameUnit.GameUnit Speller;

        void Awake()
        {
            AbilityTargetList = new List<AbilityTarget>();
            //TargetList = new List<Vector2>();
        }

        /// <summary>
        /// 从异能数据库加载对应异能的参数
        /// </summary>
        /// <param name="AbilityID">要加载的异能的ID</param>
        protected void GetAbilityFactors(string AbilityID)
        {
            InitialAbility(AbilityID);
        }
        /// <summary>
        /// 从异能数据库加载对应异能的参数
        /// </summary>
        /// <param name="AbilityID">要加载的异能的ID</param>
        protected void InitialAbility(string AbilityID)
        {
            AbilityFormat abilityFormat = AbilityDatabase.GetInstance().GetAbilityFormat(AbilityID);

            //用序列化拷贝AbilityTargetList;
            Stream stream = GameUtility.Serializer.InstanceDataToMemory(abilityFormat.AbilityTargetList);
            stream.Position = 0;
            this.AbilityTargetList = (List<AbilityTarget>)GameUtility.Serializer.MemoryToInstanceData(stream);
            //用序列化拷贝AbilityVariable
            stream = GameUtility.Serializer.InstanceDataToMemory(abilityFormat.AbilityVariable);
            stream.Position = 0;
            this.AbilityVariable = (AbilityVariable) GameUtility.Serializer.MemoryToInstanceData(stream);
            //用AbilityTargetList的长度来初始化MyTargetConstraintList
            //初始化的lambda表达式表示无论输入的是什么，返回值都是true
            foreach (AbilityTarget temp in AbilityTargetList)
            {
                MyTargetConstraintList.Add(
                    (object other) => { return true; });
            }
            //拷贝变量
            this.AbilityID = abilityFormat.AbilityID;
            this.Group = abilityFormat.Group;
            this.AbilityName = abilityFormat.AbilityName;
            this.Description = abilityFormat.Description;
            this.TriggerID = abilityFormat.TriggerID;
        }

        /// <summary>
        /// 所有异能必须重写此方法，在此方法内实现初始化
        /// </summary>
        /// <param name="abilityId">异能id</param>
        public virtual void Init(string abilityId)
        {
            InitialAbility(abilityId);
        }
    }
}