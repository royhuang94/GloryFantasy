using System.Collections;
using System.Collections.Generic;
using BattleMap;
using GameCard;
using GamePlay.Input;
using GameUnit;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GamePlay.FSM
{
    public class InputFSMMoveState : InputFSMState
    {
        //移动状态的构造函数，传入所属状态机，并且调用基类的的构造函数
        public InputFSMMoveState(InputFSM fsm) : base(fsm)
        {
                
        }

        public override void OnEnter()
        {
            base.OnEnter();
            FSM.CancelList.Add(CancelMove);
        }

        public override void OnExit()
        {
            base.OnExit();
            FSM.CancelList.Remove(CancelMove);
        }

        public void CancelMove()
        {
            FSM.HandleMovCancel();
            BattleMap.BattleMap.Instance().IsMoveColor = false;
            FSM.PushState(new InputFSMIdleState(FSM));
        }

        public override void OnPointerDownBlock(BattleMapBlock mapBlock, PointerEventData eventData)
        {
            base.OnPointerDownBlock(mapBlock, eventData);
            switch (eventData.button)
            {
                // 中键（无效果）
                case PointerEventData.InputButton.Middle:
                    return;
                // 右键（取消）
                case PointerEventData.InputButton.Right:
                    
                    break;
                // 左键
                case PointerEventData.InputButton.Left:
                    //获取第一个选择的对象
                    GameUnit.GameUnit unit = (GameUnit.GameUnit)FSM.TargetList[0];
                    //创建移动指令
                    Vector2 startPos = unit.CurPos;
                    Vector2 endPos = mapBlock.position;
                    UnitMoveCommand unitMove = new UnitMoveCommand(unit, startPos, endPos, mapBlock.GetCoordinate());
                    //如果移动指令合法
                    if (unitMove.Judge() && BattleMap.BattleMap.Instance().MapNavigator.PathSearch(startPos, endPos))
                    {
                        //移动完毕关闭移动范围染色
                        Vector2 pos = BattleMap.BattleMap.Instance().GetUnitCoordinate(unit);
                        FSM.HandleMovCancel();
                        GameUtility.UtilityHelper.Log("移动完成", GameUtility.LogColor.RED);
                        unitMove.Excute();
                        BattleMap.BattleMap.Instance().IsMoveColor = false;
                        unit.MT--;// 扣除MT
                        unit.lastAction = UnitState.Move;// 记录上一行为为移动
                        //FSM.HandleAtkConfirm(endPos, BattleMap.BattleMap.Instance().GetUnitsOnMapBlock(endPos));//移动完成，显示攻击范围
                        
                        //unit.restrain = true;

                        FSM.PushState(new InputFSMIdleState(FSM));//状态机压入新的静止状态
                    }
                    else//点到不能移动或移动范围外的地格
                    {
                        return;
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
                // 右键（无效果）
                case PointerEventData.InputButton.Right:
                    break;
                // 左键（放弃移动）
                case PointerEventData.InputButton.Left:
                    if (FSM.TargetList.Count > 0 && (GameUnit.GameUnit)FSM.TargetList[0] == unit)
                    {
                        GameUtility.UtilityHelper.Log("放弃移动", GameUtility.LogColor.RED);
                        unit.MT--;
                        unit.lastAction = UnitState.Move; // 如同移动过一般修改单位属性
                        FSM.HandleMovCancel();//关闭移动范围染色
                        BattleMap.BattleMap.Instance().IsMoveColor = false;
                        FSM.PushState(new InputFSMIdleState(FSM));//状态机压入新的静止状态
                    }
                    else
                    {
                        //点到其他单位什么都不做
                    }
                    break;
            }
        }
    }
}