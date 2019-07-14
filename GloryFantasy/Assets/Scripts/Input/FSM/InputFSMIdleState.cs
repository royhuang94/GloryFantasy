using System.Collections;
using System.Collections.Generic;
using GameCard;
using GameUnit;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GamePlay.FSM
{
    public class InputFSMIdleState : InputFSMState
    {
        public InputFSMIdleState(InputFSM fsm) : base(fsm)
        { }

        //进入静止状态后，压入静止状态后将状态机的历史栈清空剩自己释放资源
        public override void OnEnter()
        {
            base.OnEnter();
          
            FSM.ClearStateWithoutTop();
            FSM.TargetList.Clear(); //清理点击列表
        }

        public override void OnPointerDownFriendly(GameUnit.GameUnit unit, PointerEventData eventData)
        {
            base.OnPointerDownFriendly(unit, eventData);

            // 对不同按键事件进行不同的判断。
            switch (eventData.button)
            {
                // 中键（无效果）
                case PointerEventData.InputButton.Middle:
                    return;
                // 右键（TODO: 显示单位详细信息）
                case PointerEventData.InputButton.Right:
                    
                    break;
                // 左键
                case PointerEventData.InputButton.Left:
                    // 如果单位的下一个行为为移动
                    if (unit.nextAction() == UnitState.Move)
                    {
                        //获得单位的位置
                        FSM.TargetList.Clear();
                        GameUtility.UtilityHelper.Log("准备移动，再次点击角色取消移动进入攻击.Unit position is " + unit.CurPos, GameUtility.LogColor.RED);
                        FSM.TargetList.Add(unit);
                        FSM.HandleMovConfirm(unit.CurPos, unit);// 移动范围显示
                        BattleMap.BattleMap.Instance().IsMoveColor = true;
                        FSM.PushState(new InputFSMMoveState(this.FSM));
                    }
                    // 如果单位的下一个行为为攻击
                    else if (unit.nextAction() == UnitState.Attack)
                    {
                        FSM.TargetList.Clear();
                        GameUtility.UtilityHelper.Log("准备攻击，点击角色取消攻击.Unit position is " + unit.CurPos, GameUtility.LogColor.RED);
                        FSM.TargetList.Add(unit);
                        FSM.HandleAtkConfirm(unit.CurPos, unit);// 显示攻击范围
                        BattleMap.BattleMap.Instance().IsAtkColor = true;
                        FSM.PushState(new InputFSMAttackState(this.FSM));
                    }
                    break;
            }
        }

        public override void OnPointerDownCard(BaseCard card, PointerEventData eventData)
        {
            base.OnPointerDownCard(card, eventData);
            if(card is OrderCard)
            {
                if (!Player.Instance().CanConsumeAp(card.cost))
                {
                    // TODO : 并实现AP值震动效果
                    Debug.Log("Ran out of AP, cant use this one");
                    return;
                }
                Ability.Spell spell = card.gameObject.GetComponent<Ability.Spell>();
                FSM.effect = spell;
                FSM.TargetList.Clear();
                FSM.PushState(new InputFSMCastState(this.FSM));
            }
            else if(card is UnitCard)
            {
                //进入召唤状态
                FSM.selectedCard = card;
                FSM.PushState(new InputFSMSummonState(FSM));
            }
        }
    }
}