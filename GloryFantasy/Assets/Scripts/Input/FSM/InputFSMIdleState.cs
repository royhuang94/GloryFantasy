using System.Collections;
using System.Collections.Generic;
using FairyGUI;
using GameCard;
using GameUnit;
using LitJson;
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
                // 右键（显示单位详细信息）
                case PointerEventData.InputButton.Right:
                    if (FSM.CancelList.Count == 0) // 取消操作队列为空才接收相应信息
                    {
                        // 显示单位详细信息
                    }
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
                if (!Player.Instance().CanConsumeAp(card.Cost))
                {
                    // TODO : 并实现AP值震动效果
                    Debug.Log("Ran out of AP, cant use this one");
                    return;
                }
                Ability.Spell spell = card.gameObject.GetComponent<Ability.Spell>();
                FSM.effect = spell;
                FSM.TargetList.Clear();
                FSM.PushState(new InputFSMCastState(FSM));
            }
            else if(card is UnitCard)
            {
                //进入召唤状态
                FSM.selectedCard = card;
                FSM.PushState(new InputFSMSummonState(FSM));
            }
        }

        public override void OnPointerDownCDObject(UnitHero hero, EventContext context)
        {
            base.OnPointerDownCDObject(hero, context);

            switch (context.inputEvent.button)
            {
                case 0:// 左键点击
                    //JsonData data = CardManager.Instance().GetCardJsonData(cardID);
                    // 显示英雄信息，还是应该UI那边做个接口来做这件事。
                    //FGUIInterfaces.Instance().title.text = data["name"].ToString();
                    //FGUIInterfaces.Instance().effect.text = data["effect"].ToString();
                    //FGUIInterfaces.Instance().value.text = "总冷却：" + data["cd"] + "     剩余冷却：" + cooldownCard.leftCd;
                    //FGUIInterfaces.Instance().cardDescribeWindow.Show();
                    break;
                case 1:// 右键点击
                    // TODO: 操作栈
                    break;
                default: // 其他情况
                    break;

            }
        }
    }
}