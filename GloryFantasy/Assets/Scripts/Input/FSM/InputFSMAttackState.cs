﻿using System.Collections;
using System.Collections.Generic;
using GamePlay.Input;
using GameUnit;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GamePlay.FSM
{
    public class InputFSMAttackState : InputFSMState
    {
        public InputFSMAttackState(InputFSM fsm) : base(fsm)
        {

        }

        public override void OnPointerDownEnemy(GameUnit.GameUnit unit, PointerEventData eventData)
        {
            base.OnPointerDownEnemy(unit, eventData);

            //获取攻击者和被攻击者
            
            GameUnit.GameUnit Attacker = BattleMap.BattleMap.Instance().GetUnitsOnMapBlock(FSM.TargetList[FSM.TargetList.Count - 1]);
            GameUnit.GameUnit AttackedUnit = unit;
            //创建攻击指令
            UnitAttackCommand unitAtk = new UnitAttackCommand(Attacker, AttackedUnit);
            //如果攻击指令符合条件则执行
            if (unitAtk.Judge() && BattleMap.BattleMap.Instance().IsAtkColor == true)
            {
                GameUtility.UtilityHelper.Log("触发攻击", GameUtility.LogColor.RED);
                FSM.HandleAtkCancel();////攻击完工攻击范围隐藏
                BattleMap.BattleMap.Instance().IsAtkColor = false;
                unitAtk.Excute();
                Attacker.canNotAttack = true; //单位横置不能攻击
                Attacker.canNotMove = true;
                // 攻击完单位变灰，静止状态
                UnitManager.ColorUnitOnBlock(Attacker.mapBlockBelow.position, new Color(186 / 255f, 186 / 255f, 186 / 255f, 1f));

                FSM.PushState(new InputFSMIdleState(FSM)); //状态机压入静止状态
            }
            else
            {
                //如果攻击指令不符合条件就什么都不做
            }
        }

        public override void OnPointerDownFriendly(GameUnit.GameUnit unit, PointerEventData eventData)
        {
            //如果单位不能攻击了无法显示
            if (unit.canNotAttack == true)
                return;

            base.OnPointerDownFriendly(unit, eventData);
            //鼠标右键取消攻击范围显示，不取消攻击行为
            if (eventData.button == PointerEventData.InputButton.Right && BattleMap.BattleMap.Instance().IsAtkColor == true)
            {
                GameUtility.UtilityHelper.Log("取消攻击显示", GameUtility.LogColor.RED);
                FSM.HandleAtkCancel();
                BattleMap.BattleMap.Instance().IsAtkColor = false;
            }
            if(eventData.button == PointerEventData.InputButton.Left&& BattleMap.BattleMap.Instance().IsAtkColor == true)
            {
                GameUtility.UtilityHelper.Log("取消攻击", GameUtility.LogColor.RED);
                FSM.HandleAtkCancel();
                BattleMap.BattleMap.Instance().IsAtkColor = false;
                unit.canNotAttack = true; //单位横置不能攻击
                unit.canNotMove = true;
                // 攻击完单位变灰，静止状态
                UnitManager.ColorUnitOnBlock(unit.mapBlockBelow.position, new Color(186 / 255f, 186 / 255f, 186 / 255f, 1f));

                FSM.PushState(new InputFSMIdleState(FSM)); //状态机压入静止状态
            }
            //再次显示攻击范围
            else if (eventData.button == PointerEventData.InputButton.Left && BattleMap.BattleMap.Instance().IsAtkColor == false)
            {
                GameUtility.UtilityHelper.Log("显示攻击范围", GameUtility.LogColor.RED);
                Vector2 target = BattleMap.BattleMap.Instance().GetUnitCoordinate(unit);
                FSM.HandleAtkConfirm(target, unit);
                BattleMap.BattleMap.Instance().IsAtkColor = true;
            }
        }
    }
}