using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BattleMap;
using UnityEngine.EventSystems;
using GameGUI;
using GameCard;
using GameUtility;

namespace GamePlay.FSM
{
    /// <summary>
    /// 输入控制状态机，用来控制玩家的UI操作输入
    /// </summary>
    public class InputFSM : FSMachine<InputFSMState>
    {
        public InputFSM()
        {
            this.PushState(new InputFSMIdleState(this));
        }

        /// <summary>
        /// 存储点击的对象坐标
        /// </summary>
        public List<object> TargetList = new List<object>();
        /// <summary>
        /// 存储点击的手牌
        /// </summary>
        public BaseCard selectedCard;
        /// <summary>
        /// 存储发动的指令牌的异能
        /// </summary>
        public Ability.Effect effect;
        
        /// <param name="mapBlock"></param>
        /// <param name="eventData"></param>
        public void OnPointerDownBlock(BattleMapBlock mapBlock, PointerEventData eventData)
        {
            StateStack.Peek().OnPointerDownBlock(mapBlock, eventData);
        }
        /// <summary>
        /// 处理玩家单位的鼠标点击
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="eventData"></param>
        public void OnPointerDownFriendly(GameUnit.GameUnit unit, PointerEventData eventData)
        {
            StateStack.Peek().OnPointerDownFriendly(unit, eventData);
        }
        /// <summary>
        /// 处理敌人单位的鼠标点击
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="eventData"></param>
        public void OnPointerDownEnemy(GameUnit.GameUnit unit, PointerEventData eventData)
        {
            StateStack.Peek().OnPointerDownEnemy(unit, eventData);
        }
        /// <summary>
        /// 处理地图方块的鼠标进入
        /// </summary>
        /// <param name="mapBlock"></param>
        /// <param name="eventData"></param>
        public void OnPointerEnter(BattleMapBlock mapBlock, PointerEventData eventData)
        {
            if (BattleMap.BattleMap.Instance().IsColor == true)
            {
                //BattleMap.BattleMap.Instance().ShowBattleZooe(mapBlock.GetSelfPosition());
            }
            
            StateStack.Peek().OnPointerEnter(mapBlock, eventData);
            //TODO显示技能伤害的范围
        }
        /// <summary>
        /// 处理地图方块的鼠标移出
        /// </summary>
        /// <param name="mapBlock"></param>
        /// <param name="eventData"></param>
        public void OnPointerExit(BattleMapBlock mapBlock, PointerEventData eventData)
        {
            if (BattleMap.BattleMap.Instance().IsColor == true)
            {
                //BattleMap.BattleMap.Instance().HideBattleZooe(mapBlock.GetSelfPosition());
            }
            
            StateStack.Peek().OnPointerExit(mapBlock, eventData);
            //TODO
        }
        /// <summary>
        /// 处理单位牌的点击召唤
        /// </summary>
        /// <param name="unitCard"></param>
        public void OnPointerDownCard(BaseCard card, PointerEventData eventData)
        {
            StateStack.Peek().OnPointerDownCard(card, eventData);
            //if (StateStack.Peek() is InputFSMPlatState)
            //    return;
            //selectedCard = card;
            //this.PushState(new InputFSMSummonState(this));
        }
        /// <summary>
        /// 处理指令牌的释放
        /// </summary>
        /// <param name="ability"></param>
        public void OnEffectExcute(Ability.Effect effect)
        {
            //if (StateStack.Peek() is InputFSMPlatState)
            //    return;
            this.effect = effect;
            this.TargetList.Clear();
            this.PushState(new InputFSMCastState(this));
        }


        /// <summary>
        /// 处理进入选择模式
        /// </summary>
        /// <param name="ability">需要选定的异能的引用，因为需要核对target是否符合</param>
        //public void OnSelectState(Ability.Effect effect)
        //{
        //    this.effect = effect;
        //    TargetList.Clear();
        //    PushState(new InputFSMSelectState(this));
        //}

        public void OnPlatState()
        {
            PushState(new InputFSMPlatState(this));
        }

        //移动范围染色
        public void HandleMovConfirm(Vector2 target,GameUnit.GameUnit unit)
        {
            BattleMap.BattleMap map = BattleMap.BattleMap.Instance();
            if (map.CheckIfHasUnits(target))
            {
                ShowRange.Instance().MarkMoveRange(target,unit);
            }
        }

        public void HandleMovCancel()
        {
            //if (BattleMap.BattleMap.Instance().CheckIfHasUnits(target))
            //{
            //    ShowRange.Instance().CancleMoveRangeMark(target);
            //}
            //else
            //{
            //    ShowRange.Instance().CancleMoveRangeMark(TargetList[0]);
            //}
            ShowRange.Instance().CancleMoveRangeMark();
        }

        //攻击范围染色
        public void HandleAtkConfirm(Vector2 target, GameUnit.GameUnit unit)
        {
            BattleMap.BattleMap map = BattleMap.BattleMap.Instance();
            if (map.CheckIfHasUnits(target))
            {
                ShowRange.Instance().MarkAttackRange(target, unit);
            }
        }

        public void HandleAtkCancel()
        {
            //if (BattleMap.BattleMap.Instance().CheckIfHasUnits(target))
            //{
            //    ShowRange.Instance().CancleAttackRangeMark();
            //}
            //else
            //{
            //    ShowRange.Instance().CancleAttackRangeMark(TargetList[0]);
            //}
            ShowRange.Instance().CancleAttackRangeMark();
        }
    }
}
