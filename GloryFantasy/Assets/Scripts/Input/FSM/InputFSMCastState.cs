﻿using System.Collections;
using System.Collections.Generic;
using BattleMap;
using GameCard;
using GameUnit;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GamePlay.FSM
{
    public class InputFSMCastState : InputFSMState
    {
        public InputFSMCastState(InputFSM fsm) : base(fsm)
        { }
        /// <summary>
        /// 获取当前可以选取的对象。
        /// </summary>
        /// <returns>由坐标组成的列表。</returns>
        public List<Vector2> legalPostions()
        {
            List<Vector2> res = new List<Vector2>();
            foreach(GameObject instance in BattleMap.BattleMap.Instance().blocks)
            {
                BattleMapBlock block;
                if (instance.activeSelf == true)
                    block = instance.GetComponent<BattleMapBlock>();
                else
                    continue;
                if (FSM.ability.MyTargetConstraintList[FSM.TargetList.Count](block))
                    res.Add(block.position);
            }
            return res;
        }

        public override void OnEnter()
        {
            base.OnEnter();

            //如果发动的指令牌不需要指定目标则直接发动
            //并且状态机压入回正常状态
            if (FSM.ability.AbilityTargetList.Count == 0)
            {
                Gameplay.Info.CastingCard = FSM.ability.GetComponent<OrderCard>();
                // 消耗Ap值
                CardManager.Instance().OnTriggerCurrentCard();

                FSM.PushState(new InputFSMIdleState(FSM));
            }
        }

        public override void OnRightPointerDown()
        {
            base.OnRightPointerDown();
            //这整个类都是应该要被重构的了……

            //取消掉选择状态
            FSM.PushState(new InputFSMIdleState(FSM));
        }

        public override void OnPointerDownBlock(BattleMapBlock mapBlock, PointerEventData eventData)
        {
            base.OnPointerDownBlock(mapBlock, eventData);
            //判断是是否符合Ability中的自制对象约束
            if (FSM.ability.MyTargetConstraintList[FSM.TargetList.Count](mapBlock) != true)
                return;

            //如果点击地图块符合指令牌异能的对象约束
            if (FSM.ability.AbilityTargetList[FSM.TargetList.Count].TargetType == Ability.TargetType.Field ||
                    FSM.ability.AbilityTargetList[FSM.TargetList.Count].TargetType == Ability.TargetType.All)
            {
                FSM.TargetList.Add(mapBlock.position);
            }
            //如果已经选够了目标就发动卡片
            //这里应该让Card那边写个发动卡片的函数，写在Input里不科学
            if (FSM.TargetList.Count == FSM.ability.AbilityTargetList.Count)
            {
                Gameplay.Info.CastingCard = FSM.ability.GetComponent<OrderCard>();
                CardManager.Instance().OnTriggerCurrentCard();
                FSM.PushState(new InputFSMIdleState(FSM));
            }
            //Gameplay.Instance().gamePlayInput.HandleSkillCancel(FSM.TargetList[0], 4);
        }

        public override void OnPointerDownFriendly(GameUnit.GameUnit unit, PointerEventData eventData)
        {
            base.OnPointerDownFriendly(unit, eventData);

            //判断是是否符合Ability中的自制对象约束
            if (FSM.ability.MyTargetConstraintList[FSM.TargetList.Count](unit) != true)
                return;

            //如果点击地图块符合指令牌异能的对象约束
            if ((FSM.ability.AbilityTargetList[FSM.TargetList.Count].TargetType == Ability.TargetType.Friendly) ||
                    (FSM.ability.AbilityTargetList[FSM.TargetList.Count].TargetType == Ability.TargetType.All))
            {
                FSM.TargetList.Add(BattleMap.BattleMap.Instance().GetUnitCoordinate(unit));
            }
            //如果已经选够了目标就发动卡片
            //这里应该让Card那边写个发动卡片的函数，写在Input里不科学
            if (FSM.TargetList.Count == FSM.ability.AbilityTargetList.Count)
            {
                Gameplay.Info.CastingCard = FSM.ability.GetComponent<OrderCard>();
                CardManager.Instance().OnTriggerCurrentCard();
                FSM.PushState(new InputFSMIdleState(FSM));
            }
            //Gameplay.Instance().gamePlayInput.HandleSkillConfim(FSM.TargetList[0], 4);
        }

        public override void OnPointerDownEnemy(GameUnit.GameUnit unit, PointerEventData eventData)
        {
            base.OnPointerDownEnemy(unit, eventData);

            //判断是是否符合Ability中的自制对象约束
            if (FSM.ability.MyTargetConstraintList[FSM.TargetList.Count](unit) != true)
                return;

            //如果点击地图块符合指令牌异能的对象约束
            if ((FSM.ability.AbilityTargetList[FSM.TargetList.Count].TargetType == Ability.TargetType.Enemy) ||
                    (FSM.ability.AbilityTargetList[FSM.TargetList.Count].TargetType == Ability.TargetType.All))
            {
                FSM.TargetList.Add(BattleMap.BattleMap.Instance().GetUnitCoordinate(unit));
            }
            //如果已经选够了目标就发动卡片
            //这里应该让Card那边写个发动卡片的函数，写在Input里不科学
            if (FSM.TargetList.Count == FSM.ability.AbilityTargetList.Count)
            {
                Gameplay.Info.CastingCard = FSM.ability.GetComponent<OrderCard>();
                CardManager.Instance().OnTriggerCurrentCard();
                FSM.PushState(new InputFSMIdleState(FSM));
            }
        }
    }
}