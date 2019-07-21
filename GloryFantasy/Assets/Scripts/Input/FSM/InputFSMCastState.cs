using System.Collections;
using System.Collections.Generic;
using Ability;
using BattleMap;
using GameCard;
using GameUnit;
using IMessage;
using UI.FGUI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GamePlay.FSM
{
    public class InputFSMCastState : InputFSMState, GameplayTool
    {
        public InputFSMCastState(InputFSM fsm) : base(fsm)
        { }
        
        public void CancelCast()
        {
            // 已经选取了目标，撤回
            if (FSM.TargetList.Count > 0)
            {
                // TODO: 重新渲染合法范围
                FSM.TargetList.RemoveAt(FSM.TargetList.Count - 1);
            }
            // 选取队列为空，根据效果的设定决定是否允许取消
            else if (FSM.effect.allowCancel)
            {
                // TODO: 停止渲染合法范围
                FSM.PushState(new InputFSMIdleState(FSM));
                FSM.effect.cancel();
            }
        }

        public override void OnEnter()
        {
            base.OnEnter();

            FSM.CancelList.Add(CancelCast);
            // 设置当前正在选取的效果
            this.SetSpellingAbility(FSM.effect);
            // 如果发动的指令牌不需要指定目标则直接发动
            // 并且状态机压入回正常状态
            if (FSM.effect.targets == null || FSM.effect.targets.Count == 0)
            {
                FSM.PushState(new InputFSMIdleState(FSM));
                FSM.effect.Cast();
            }
            else
            {
                // TODO：合法对象渲染
            }
        }

        public override void OnExit()
        {
            base.OnExit();

            FSM.CancelList.Remove(CancelCast);
        }

        public override void OnPointerDownBlock(BattleMapBlock mapBlock, PointerEventData eventData)
        {
            base.OnPointerDownBlock(mapBlock, eventData);

            // 对不同按键事件进行不同的判断。
            switch (eventData.button)
            {
                // 中键（无效果）
                case PointerEventData.InputButton.Middle:
                    return;
                // 右键（撤回选取或取消选取）
                case PointerEventData.InputButton.Right:
                    
                    break;
                // 左键
                case PointerEventData.InputButton.Left:

                    // TODO: 终止合法对象渲染。

                    // 进行目标条件判断，先筛取类型保证约束函数内部强转不出错。
                    if (FSM.effect.targets[FSM.TargetList.Count].TargetType == TargetType.Block
                        && FSM.effect.targets[FSM.TargetList.Count].TargetConstrain(mapBlock))
                    {
                        FSM.TargetList.Add(mapBlock);
                    }

                    // 如果已经选够了目标就发动效果
                    if (FSM.TargetList.Count == FSM.effect.targets.Count)
                    {
                        FSM.PushState(new InputFSMIdleState(FSM));
                        FSM.effect.Cast();
                        return;
                    }
                    else
                    {
                        // TODO: 更新合法对象重新渲染。
                    }
                    break;
            }
        }

        public override void OnPointerDownFriendly(GameUnit.GameUnit unit, PointerEventData eventData)
        {
            base.OnPointerDownFriendly(unit, eventData);
            // 对不同按键事件进行不同的判断。
            if (FSM.effect.targets[FSM.TargetList.Count].TargetType == TargetType.Block)
            {
                OnPointerDownBlock(unit.mapBlockBelow, eventData);
                return;
            }
            switch (eventData.button)
            {
                // 中键（无效果）
                case PointerEventData.InputButton.Middle:
                    return;
                // 右键（无效果）
                case PointerEventData.InputButton.Right:
                    break;
                // 左键
                case PointerEventData.InputButton.Left:

                    // TODO: 终止合法对象渲染。

                    // 进行目标条件判断，先筛取类型保证约束函数内部强转不出错。
                    if (FSM.effect.targets[FSM.TargetList.Count].TargetType == TargetType.Unit
                        && FSM.effect.targets[FSM.TargetList.Count].TargetConstrain(unit))
                    {
                        FSM.TargetList.Add(unit);
                    }

                    // 如果已经选够了目标就发动效果
                    if (FSM.TargetList.Count == FSM.effect.targets.Count)
                    {
                        FSM.PushState(new InputFSMIdleState(FSM));
                        FSM.effect.Cast();
                        return;
                    }
                    else
                    {
                        // TODO: 更新合法对象重新渲染。
                    }
                    break;
            }
        }

        public override void OnPointerDownEnemy(GameUnit.GameUnit unit, PointerEventData eventData)
        {
            base.OnPointerDownEnemy(unit, eventData);
            if (FSM.effect.targets[FSM.TargetList.Count].TargetType == TargetType.Block)
            {
                OnPointerDownBlock(unit.mapBlockBelow, eventData);
                return;
            }

            base.OnPointerDownFriendly(unit, eventData);
            // 对不同按键事件进行不同的判断。
            switch (eventData.button)
            {
                // 中键（无效果）
                case PointerEventData.InputButton.Middle:
                    return;
                // 右键（无效果）
                case PointerEventData.InputButton.Right:
                    
                    break;
                // 左键
                case PointerEventData.InputButton.Left:

                    // TODO: 终止合法对象渲染。

                    // 进行目标条件判断，先筛取类型保证约束函数内部强转不出错。
                    if (FSM.effect.targets[FSM.TargetList.Count].TargetType == TargetType.Unit
                        && FSM.effect.targets[FSM.TargetList.Count].TargetConstrain(unit))
                    {
                        FSM.TargetList.Add(unit);
                    }

                    // 如果已经选够了目标就发动效果
                    if (FSM.TargetList.Count == FSM.effect.targets.Count)
                    {
                        FSM.PushState(new InputFSMIdleState(FSM));
                        FSM.effect.Cast();
                        return;
                    }
                    else
                    {
                        // TODO: 更新合法对象重新渲染。
                    }
                    break;
            }
        }

        public override void OnPointerDownCard(BaseCard card, PointerEventData eventData)
        {
            base.OnPointerDownCard(card, eventData);

            // TODO: 终止合法对象渲染。

            if (FSM.effect.targets[FSM.TargetList.Count].TargetType == TargetType.Card
                && FSM.effect.targets[FSM.TargetList.Count].TargetConstrain(card))
            {
                FSM.TargetList.Add(card);
            }
            if (FSM.TargetList.Count == FSM.effect.targets.Count)
            {
                FSM.PushState(new InputFSMIdleState(FSM));
                FSM.effect.Cast();
                return;
            }
            else
            {
                // TODO: 更新合法对象重新渲染。
            }
        }
    }
}