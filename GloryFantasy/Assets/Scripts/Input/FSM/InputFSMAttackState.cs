using System.Collections;
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
            
            GameUnit.GameUnit Attacker = BattleMap.BattleMap.Instance().GetUnitsOnMapBlock(FSM.TargetList[0]);
            GameUnit.GameUnit AttackedUnit = unit;
            //创建攻击指令
            UnitAttackCommand unitAtk = new UnitAttackCommand(Attacker, AttackedUnit);
            //如果攻击指令符合条件则执行
            if (unitAtk.Judge())
            {
                GameUtility.UtilityHelper.Log("触发攻击", GameUtility.LogColor.RED);
                unitAtk.Excute();
                unit.disarm = true; //单位横置不能攻击
                FSM.HandleAtkCancel(BattleMap.BattleMap.Instance().GetUnitCoordinate(Attacker));////攻击完工攻击范围隐藏  
                FSM.TargetList.Clear(); //清空对象列表
                FSM.PushState(new InputFSMIdleState(FSM)); //状态机压入静止状态
            }
            else
            {
                //如果攻击指令不符合条件就什么都不做
            }
        }
    }
}