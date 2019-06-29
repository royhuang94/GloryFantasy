using System.Collections;
using System.Collections.Generic;
using BattleMap;
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

        public override void OnPointerDownBlock(BattleMapBlock mapBlock, PointerEventData eventData)
        {
            base.OnPointerDownBlock(mapBlock, eventData);

            //获取第一个选择的对象
            GameUnit.GameUnit unit = BattleMap.BattleMap.Instance().GetUnitsOnMapBlock(FSM.TargetList[0]);
            //创建移动指令
            Vector2 startPos = FSM.TargetList[0];
            Vector2 endPos = mapBlock.position;
            UnitMoveCommand unitMove = new UnitMoveCommand(unit, startPos, endPos, mapBlock.GetCoordinate());
            //如果移动指令合法
            if (unitMove.Judge() && BattleMap.BattleMap.Instance().MapNavigator.PathSearch(startPos, endPos))
            {
                //移动完毕关闭移动范围染色
                Vector2 pos = BattleMap.BattleMap.Instance().GetUnitCoordinate(unit);
                FSM.HandleMovCancel();
                GameUtility.UtilityHelper.Log("移动完成，进入攻击状态，点击敌人进行攻击，右键点击角色取消攻击", GameUtility.LogColor.RED);
                unitMove.Excute();
                BattleMap.BattleMap.Instance().IsMoveColor = false;
                
                //FSM.HandleAtkConfirm(endPos, BattleMap.BattleMap.Instance().GetUnitsOnMapBlock(endPos));//移动完成，显示攻击范围

                FSM.TargetList.Add(endPos);
                //unit.restrain = true;

                FSM.PushState(new InputFSMAttackState(FSM));//状态机压入新的攻击状态
            }
            else//点到不能移动或移动范围外的地格
            {
                FSM.HandleMovCancel();//仅取消移动范围显示
                FSM.PushState(new InputFSMIdleState(FSM));//回到上一个状态
                MessageBox.Instance().isShow = true;
            }
        }

        public override void OnPointerDownFriendly(GameUnit.GameUnit unit, PointerEventData eventData)
        {
            base.OnPointerDownFriendly(unit, eventData);

            
            Vector2 pos = BattleMap.BattleMap.Instance().GetUnitCoordinate(unit);
            //如果两次都点在同一个角色身上，就从移动转为攻击
            if(eventData.button == PointerEventData.InputButton.Left)//左键点击
            {
                if (FSM.TargetList.Count > 0 && FSM.TargetList[0] == pos)
                {
                    GameUtility.UtilityHelper.Log("取消移动，进入攻击,再次点击角色取消攻击", GameUtility.LogColor.RED);
                    FSM.HandleMovCancel();//关闭移动范围染色
                    BattleMap.BattleMap.Instance().IsMoveColor = false;
                    FSM.HandleAtkConfirm(pos, unit);//显示攻击范围
                    BattleMap.BattleMap.Instance().IsAtkColor = true;
                    unit.canNotMove = true;//横置单位
                    FSM.PushState(new InputFSMAttackState(FSM));//状态机压入新的攻击状态
                }
                else
                {
                    //点到其他单位什么都不做
                }
            }
            if(eventData.button == PointerEventData.InputButton.Right)//右键取消移动范围显示，不取消移动行为
            {
                FSM.HandleMovCancel();
                BattleMap.BattleMap.Instance().IsMoveColor = false;
                FSM.PushState(new InputFSMIdleState(FSM));
            }
        }
    }
}