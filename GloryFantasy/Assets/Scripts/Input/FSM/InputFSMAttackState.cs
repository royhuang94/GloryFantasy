using System.Collections;
using System.Collections.Generic;
using GameCard;
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
            switch (eventData.button)
            {
                // 中键（无效果）
                case PointerEventData.InputButton.Middle:
                    return;
                // 右键（取消）
                case PointerEventData.InputButton.Right:
                    FSM.HandleAtkCancel();//攻击完工攻击范围隐藏
                    BattleMap.BattleMap.Instance().IsAtkColor = false;
                    FSM.PushState(new InputFSMIdleState(FSM));
                    break;
                // 左键（攻击）
                case PointerEventData.InputButton.Left:
                    //获取攻击者和被攻击者
                    GameUnit.GameUnit Attacker = (GameUnit.GameUnit)FSM.TargetList[0];
                    GameUnit.GameUnit AttackedUnit = unit;
                    //创建攻击指令
                    UnitAttackCommand unitAtk = new UnitAttackCommand(Attacker, AttackedUnit);
                    //如果攻击指令符合条件则执行
                    if (unitAtk.Judge() && BattleMap.BattleMap.Instance().IsAtkColor == true)
                    {
                        GameUtility.UtilityHelper.Log("触发攻击", GameUtility.LogColor.RED);
                        FSM.HandleAtkCancel();//攻击完工攻击范围隐藏
                        BattleMap.BattleMap.Instance().IsAtkColor = false;
                        unitAtk.Excute();
                        Attacker.AT--;
                        Attacker.lastAction = UnitState.Attack;
                        // 攻击完单位变灰，静止状态
                        //UnitManager.ColorUnitOnBlock(Attacker.mapBlockBelow.position, new Color(186 / 255f, 186 / 255f, 186 / 255f, 1f));

                        FSM.PushState(new InputFSMIdleState(FSM)); //状态机压入静止状态
                    }
                    else
                    {
                        //如果攻击指令不符合条件就什么都不做
                    }
                    break;
            }
            
        }

        public override void OnPointerDownFriendly(GameUnit.GameUnit unit, PointerEventData eventData)
        {
            base.OnPointerDownFriendly(unit, eventData);
            switch (eventData.button)
            {
                // 中键（无效果）
                case PointerEventData.InputButton.Middle:
                    return;
                // 右键（取消）
                case PointerEventData.InputButton.Right:
                    FSM.HandleAtkCancel();//攻击完工攻击范围隐藏
                    BattleMap.BattleMap.Instance().IsAtkColor = false;
                    FSM.PushState(new InputFSMIdleState(FSM));
                    break;
                // 左键（如果是点击自己，则放弃攻击）
                case PointerEventData.InputButton.Left:
                    if (unit == (GameUnit.GameUnit)FSM.TargetList[0])
                    {
                        GameUtility.UtilityHelper.Log("放弃攻击", GameUtility.LogColor.RED);
                        FSM.HandleAtkCancel();
                        BattleMap.BattleMap.Instance().IsAtkColor = false;
                        unit.AT--;
                        unit.lastAction = UnitState.Attack;
                        //// 攻击完单位变灰，静止状态
                        //UnitManager.ColorUnitOnBlock(unit.mapBlockBelow.position, new Color(186 / 255f, 186 / 255f, 186 / 255f, 1f));
                        FSM.PushState(new InputFSMIdleState(FSM)); //状态机压入静止状态
                    }
                    break;
            }
            
        }

        public override void OnPointerDownCard(BaseCard Card, PointerEventData eventData)
        {
            base.OnPointerDownCard(Card, eventData);
            switch (eventData.button)
            {
                // 中键（无效果）
                case PointerEventData.InputButton.Middle:
                    return;
                // 右键（取消）
                case PointerEventData.InputButton.Right:
                    FSM.HandleAtkCancel();//攻击完工攻击范围隐藏
                    BattleMap.BattleMap.Instance().IsAtkColor = false;
                    FSM.PushState(new InputFSMIdleState(FSM));
                    break;
                // 左键（无效果）
                case PointerEventData.InputButton.Left:

                    break;
            }
        }
    }
}