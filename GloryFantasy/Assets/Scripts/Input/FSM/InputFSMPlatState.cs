using BattleMap;
using FairyGUI;
using GameCard;
using GameUnit;
using IMessage;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GamePlay.FSM
{
    public class InputFSMPlatState : InputFSMState
    {
        public InputFSMPlatState(InputFSM fsm) : base(fsm)
        { }
        private UnitHero selectingHero;

        public void CancelDispose()
        {
            if (selectingHero == null) // 未选择英雄的场合
            {
                FSM.PushState(new InputFSMIdleState(FSM));
            }
            else // 选择了英雄的场合
            {
                selectingHero = null;
            }
        }

        public override void OnEnter()
        {
            base.OnEnter();
            
            if (GamePlay.Gameplay.Instance().heroManager.done.Count == 0)
                FSM.PushState(new InputFSMIdleState(FSM));
            FSM.CancelList.Add(CancelDispose);
        }

        public override void OnExit()
        {
            base.OnExit();
            FSM.CancelList.Remove(CancelDispose);
        }

        public override void OnPointerDownBlock(BattleMapBlock mapBlock, PointerEventData eventData)
        {
            base.OnPointerDownBlock(mapBlock, eventData);
            if (selectingHero == null)
                return;
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

                    if (GameplayToolExtend.GetRegion(mapBlock)._battleAreaSate != BattleAreaState.Player)
                    {
                        
                    }
                    else
                    {
                        selectingHero.dispose(mapBlock);
                        selectingHero = null;
                    }
                    break;
            }
        }

        public override void OnPointerDownCDObject(UnitHero hero, EventContext context)
        {
            base.OnPointerDownCDObject(hero, context);

            if (hero.CDObject.currentRecovery >= hero.CDObject.maxRecovery)
                selectingHero = hero;
        }

    }
}