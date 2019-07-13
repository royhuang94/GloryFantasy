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

            //如果单位可以移动
            if (unit.canNotMove == false)
            {
                //获得单位的位置
                Vector2 pos = BattleMap.BattleMap.Instance().GetUnitCoordinate(unit);
                GameUtility.UtilityHelper.Log("准备移动，再次点击角色取消移动进入攻击.Unit position is " + pos, GameUtility.LogColor.RED);
                FSM.TargetList.Add(pos);
                FSM.HandleMovConfirm(pos,BattleMap.BattleMap.Instance().GetUnitsOnMapBlock(pos));//移动范围显示
                BattleMap.BattleMap.Instance().IsMoveColor = true;
                FSM.PushState(new InputFSMMoveState(this.FSM));
            }
            //如果单位已经不能移动，但是可以攻击
            else if (unit.canNotMove == true && unit.canNotAttack == false)
            {
                Vector2 pos = BattleMap.BattleMap.Instance().GetUnitCoordinate(unit);
                GameUtility.UtilityHelper.Log("准备攻击，右键取消攻击.Unit position is " + pos, GameUtility.LogColor.RED);
                FSM.TargetList.Add(pos);
                FSM.PushState(new InputFSMAttackState(this.FSM));
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