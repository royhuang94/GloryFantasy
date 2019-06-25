using BattleMap;
using IMessage;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GamePlay.FSM
{
    public class InputFSMSelectState : InputFSMState
    {
        public InputFSMSelectState(InputFSM fsm) : base(fsm)
        { }

        public override void OnEnter()
        {
            base.OnEnter();

            // 如果没有指定ability或者ability的TargetList是空的，直接发消息
            if (FSM.ability == null)
            {
                FSM.PushState(new InputFSMIdleState(FSM));
                
            }
            else if(FSM.ability.AbilityTargetList.Count == 0)
            {
                MsgDispatcher.SendMsg((int)MessageType.SelectionOver);
            }
            
            Debug.Log("*.*--Entering Selecting mod, " + FSM.ability.AbilityTargetList.Count + " target to choose");
        }

        public override void OnRightPointerDown()
        {
            base.OnRightPointerDown();

            //取消掉选择状态
            FSM.PushState(new InputFSMIdleState(FSM));
        }

        public override void OnPointerDownBlock(BattleMapBlock mapBlock, PointerEventData eventData)
        {
            base.OnPointerDownBlock(mapBlock, eventData);

            //如果不是左键直接跳出
            if (eventData.button != PointerEventData.InputButton.Left)
            {
                return;
            }

            //判断是是否符合Ability中的自制对象约束
            if (FSM.ability.MyTargetConstraintList[FSM.TargetList.Count](mapBlock) != true)
                return;

            // 如果点击地图块符合指令牌异能的对象约束
            if (FSM.ability.AbilityTargetList[FSM.TargetList.Count].TargetType == Ability.TargetType.Field ||
                FSM.ability.AbilityTargetList[FSM.TargetList.Count].TargetType == Ability.TargetType.All)
            {
                FSM.TargetList.Add(mapBlock.position);
            }
            // 如果已经选够了目标就发送选择完毕消息
            if (FSM.TargetList.Count == FSM.ability.AbilityTargetList.Count)
            {
                MsgDispatcher.SendMsg((int)MessageType.SelectionOver);
                FSM.PushState(new InputFSMIdleState(FSM));
            } else
            {
                MsgDispatcher.SendMsg((int)MessageType.SelectOneTarget);
            }
        }

        public override void OnPointerDownFriendly(GameUnit.GameUnit unit, PointerEventData eventData)
        {
            base.OnPointerDownFriendly(unit, eventData);

            //如果不是左键直接跳出
            if (eventData.button != PointerEventData.InputButton.Left)
            {
                return;
            }

            //判断是是否符合Ability中的自制对象约束
            if (FSM.ability.MyTargetConstraintList[FSM.TargetList.Count](unit) != true)
                return;

            if ((FSM.ability.AbilityTargetList[FSM.TargetList.Count].TargetType == Ability.TargetType.Friendly) ||
                (FSM.ability.AbilityTargetList[FSM.TargetList.Count].TargetType == Ability.TargetType.All))
            {
                FSM.TargetList.Add(BattleMap.BattleMap.Instance().GetUnitCoordinate(unit));
            }
            // 如果已经选够了目标就发送选择完毕消息
            if (FSM.TargetList.Count == FSM.ability.AbilityTargetList.Count)
            {
                MsgDispatcher.SendMsg((int)MessageType.SelectionOver);
                FSM.PushState(new InputFSMIdleState(FSM));
            } else
            {
                MsgDispatcher.SendMsg((int)MessageType.SelectOneTarget);
            }
        }

        public override void OnPointerDownEnemy(GameUnit.GameUnit unit, PointerEventData eventData)
        {
            base.OnPointerDownEnemy(unit, eventData);

            //如果不是左键直接跳出
            if (eventData.button != PointerEventData.InputButton.Left)
            {
                return;
            }

            //判断是是否符合Ability中的自制对象约束
            if (FSM.ability.MyTargetConstraintList[FSM.TargetList.Count](unit) != true)
                return;

            if ((FSM.ability.AbilityTargetList[FSM.TargetList.Count].TargetType == Ability.TargetType.Enemy) ||
                (FSM.ability.AbilityTargetList[FSM.TargetList.Count].TargetType == Ability.TargetType.All))
            {
                FSM.TargetList.Add(BattleMap.BattleMap.Instance().GetUnitCoordinate(unit));
            }
            // 如果已经选够了目标就发送选择完毕消息
            if (FSM.TargetList.Count == FSM.ability.AbilityTargetList.Count)
            {
                MsgDispatcher.SendMsg((int)MessageType.SelectionOver);
                FSM.PushState(new InputFSMIdleState(FSM));
            } else
            {
                MsgDispatcher.SendMsg((int)MessageType.SelectOneTarget);
            }
        }
    }
}