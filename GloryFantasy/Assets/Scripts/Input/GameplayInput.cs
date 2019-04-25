using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BattleMap;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using GameGUI;
using GameUnit;
using GameCard;
using System;

namespace GamePlay.Input
{
    //这个类使用的方法要尽量保证简洁，复杂的逻辑封进command里
    //以便留更多的空间给写UI和操作逻辑的人

    /// <summary>
    /// 游戏输入控制类
    /// </summary>
    public class GameplayInput
    {
        /// <summary>
        /// 标记是否在选择移动目标
        /// </summary>
        public bool IsMoving { get; set; }
        /// <summary>
        /// 标记是否正在选择攻击目标
        /// </summary>
        public bool IsAttacking { get; set; }
        /// <summary>
        /// 存储点击的对象坐标
        /// </summary>
        public List<Vector2> TargetList = new List<Vector2>();

        /// <summary>
        /// 标记是否已经选择了一张手牌
        /// </summary>
        public bool IsSelectingSlot
        {
            get
            {
                return selectedSlot != null;
            }
        }
        /// <summary>
        /// 被鼠标选中的手牌槽Slot
        /// </summary>
        private UnitSlot selectedSlot;
        /// <summary>
        /// 被鼠标选中的手牌槽Slot
        /// </summary>
        public UnitSlot SelectedSlot
        {
            get
            {
                return selectedSlot; //pickedUnit永远不会为空，因为是从prefab中复制出去的
            }
        }

        /// <summary>
        /// 标记是否在释放行动牌
        /// </summary>
        public bool IsCasting
        {
            get;
            private set;
        }
        /// <summary>
        /// 正在释放的行动牌的异能的引用
        /// </summary>
        private Ability.Ability CastingCard;
        /// <summary>
        /// 用来存储释放异能的选择目标
        /// </summary>
        private List<Vector2> SelectingList = new List<Vector2>();
        /// <summary>
        /// 保存当前实例化到地图上的单位异能id
        /// </summary>
        public List<string> abilitiesID { get; set; }

        /// <summary>
        /// 处理地图方块的鼠标点击
        /// </summary>
        /// <param name="mapBlock"></param>
        /// <param name="eventData"></param>
        public void OnPointerDown(BattleMapBlock mapBlock, PointerEventData eventData)
        {
            //获得点击的地图方块的坐标
            //Vector2 targetPositon = mapBlock.GetSelfPosition();
            //UnitMoveCommand unitMove = new UnitMoveCommand(tempUnit, unitPositon, targetPositon);
               
            if (IsMoving)
            {
                GameUnit.GameUnit unit = BattleMap.BattleMap.Instance().GetUnitsOnMapBlock(TargetList[0]);
                Vector2 startPos = TargetList[0];
                Vector2 endPos = mapBlock.position;
                UnitMoveCommand unitMove = new UnitMoveCommand(unit, startPos, endPos);
                if (unitMove.Judge())
                {
                    unitMove.Excute();
                    SetMovingIsFalse(unit);
                }
                else
                {
                    //如果不符合移动条件，什么都不做
                }
            }
            //如果已经选中了一张手牌
            else if (IsSelectingSlot)
            {
                //如果不是自己的战区，则无操作
                if (!BattleMap.BattleMap.Instance().WarZoneBelong(mapBlock.GetSelfPosition())) return;
                //做个判断，如果选中的手牌不是单位卡则返回不操作
                //if (selectedSlot.GetBaseCard().) //BaseCard的成员都没写好……什么鬼
                //在对应MapBlock生成单位
                UnitManager.InstantiationUnit(selectedSlot.GetBaseCard().id , OwnerEnum.Player, mapBlock.transform);
                //把这张手牌从手牌里删掉
                //删掉对应手牌槽的引用
                selectedSlot.RemoveItem();
                selectedSlot = null;
                //关闭鼠标所在战区的高光显示
                BattleMap.BattleMap.Instance().IsColor = false;
                BattleMap.BattleMap.Instance().HideBattleZooe(mapBlock.GetSelfPosition());
            }
            //如果正在释放指令牌，就视为正在选择目标
            else if (IsCasting)
            {
                
            }

            BattleMap.BattleMap.Instance().curMapPos = mapBlock.GetSelfPosition();
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
                BattleMap.BattleMap.Instance().ShowBattleZooe(mapBlock.GetSelfPosition());
            }
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
                BattleMap.BattleMap.Instance().HideBattleZooe(mapBlock.GetSelfPosition());
            }
        }
        /// <summary>
        /// 处理单位的鼠标点击
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="eventData"></param>
        public void OnPointerDown(GameUnit.GameUnit unit, PointerEventData eventData)
        {
            if (IsMoving)
            {
                //如果移动两次都选择同一个单位，就进行一次待机
                Vector2 pos = BattleMap.BattleMap.Instance().GetUnitCoordinate(unit);
                if (TargetList[0] == pos )
                {
                    SetMovingIsFalse(unit);
                    unit.restrain = true;
                }
                else
                {
                    //点到其他单位什么都不做
                }
            }
            else if (IsAttacking)
            {
                //获取攻击者和被攻击者
                GameUnit.GameUnit Attacker = BattleMap.BattleMap.Instance().GetUnitsOnMapBlock(TargetList[0]);
                GameUnit.GameUnit AttackedUnit = unit;
                //创建攻击指令
                UnitAttackCommand unitAtk = new UnitAttackCommand(Attacker, AttackedUnit);
                //如果攻击指令符合条件则执行
                if (unitAtk.Judge())
                {
                    GameUtility.UtilityHelper.Log("触发攻击", GameUtility.LogColor.RED);
                    unitAtk.Excute();                   
                    IsAttacking = false;
                    TargetList.Clear();
                    if (!unit.IsDead())
                    {
                        float hpDivMaxHp = (float)unit.hp / unit.MaxHP * 100;
                        var textHp = unit.transform.GetComponentInChildren<Text>();
                        textHp.text = string.Format("Hp: {0}%", hpDivMaxHp);
                        //Attacker.GetComponentInChildren<hpUpdate>().UpdateHp();
                    }
                }
                else
                {
                    //如果攻击指令不符合条件就什么都不做
                }
            }
            //如果单位可以移动
            else if (unit.restrain == false)
            {
                SetMovingIsTrue(unit);
            }
            //如果单位已经不能移动，但是可以攻击
            else if (unit.restrain == true && unit.disarm == false)
            {
                IsAttacking = true;
                TargetList.Add(BattleMap.BattleMap.Instance().GetUnitCoordinate(unit));
            }

        }

        /// <summary>
        /// 设置IsMoving为True
        /// </summary>
        /// <param name="target"></param>
        public void SetMovingIsTrue(GameUnit.GameUnit unit)
        {
            IsMoving = true;
            Vector2 pos = BattleMap.BattleMap.Instance().GetUnitCoordinate(unit);
            Debug.Log("Unit position is " + pos);
            TargetList.Add(pos);
            HandleMovConfirm(pos);
        }
        public void SetMovingIsFalse(GameUnit.GameUnit unit)
        {
            IsMoving = false;
            Vector2 pos = BattleMap.BattleMap.Instance().GetUnitCoordinate(unit);
            TargetList.Clear();
            HandleMovCancel(pos);
        }

        //移动范围染色
        public void HandleMovConfirm(Vector2 target)
        {
            BattleMap.BattleMap map = BattleMap.BattleMap.Instance();
            if (map.CheckIfHasUnits(target))
            {
                GameUnit.GameUnit unit = BattleMap.BattleMap.Instance().GetUnitsOnMapBlock(target);
                unit.GetComponent<ShowRange>().MarkMoveRange(target);
            }
        }

        public void HandleMovCancel(Vector2 target)
        {
            GameUnit.GameUnit unit = BattleMap.BattleMap.Instance().GetUnitsOnMapBlock(target);
            unit.GetComponent<ShowRange>().CancleMoveRangeMark(target);
        }

        //攻击范围染色
        public void HandleAtkConfirm(Vector2 target)
        {
            BattleMap.BattleMap map = BattleMap.BattleMap.Instance();
            if (map.CheckIfHasUnits(target))
            {
                GameUnit.GameUnit unit = BattleMap.BattleMap.Instance().GetUnitsOnMapBlock(target);
                unit.GetComponent<ShowRange>().MarkAttackRange(target);
            }
        }

        public void HandleAtkCancel(Vector2 target)
        {
            GameUnit.GameUnit unit = BattleMap.BattleMap.Instance().GetUnitsOnMapBlock(target);
            unit.GetComponent<ShowRange>().CancleAttackRangeMark(target);
        }

        /// <summary>
        /// 设置被选中的手牌槽
        /// </summary>
        /// <param name="currentItemUI"></param>
        internal void SelectSlotUnit(UnitSlot currentItemUI)
        {
            selectedSlot = currentItemUI;
        }

        //public void HandleConfirm(Vector2 target)
        //{
        //    BattleMap.BattleMap map = BattleMap.BattleMap.getInstance();
        //    if (TargetList.Count == 0)
        //    {
        //        if (map.CheckIfHasUnits(target))
        //        {
        //            TargetList.Add(target);
        //            GameUnit.GameUnit unit = BattleMap.BattleMap.getInstance().GetUnitsOnMapBlock(TargetList[0]);
        //            unit.GetComponent<ShowRange>().MarkMoveRange();
        //            unit.GetComponent<ShowRange>().MarkAttackRange();
        //        }
        //    }
        //    else
        //    if (TargetList.Count == 1)
        //    {
        //        if (map.CheckIfHasUnits(target))
        //        {
        //            //TargetList.Add(target);
        //            GameUnit.GameUnit unit1 = map.GetUnitsOnMapBlock(TargetList[0]);
        //            //GameUnit.GameUnit unit2 = mapManager.GetUnitsOnMapBlock(TargetList[1])[0];
        //            GameUnit.GameUnit unit2 = map.GetUnitsOnMapBlock(target);
        //            UnitAttackCommand attackCommand = new UnitAttackCommand(unit1, unit2);
        //            if (attackCommand.Judge())
        //            {
        //                //关闭染色
        //                unit1.GetComponent<ShowRange>().CancleAttackRangeMark();
        //                unit1.GetComponent<ShowRange>().CancleMoveRangeMark();

        //                attackCommand.Excute();
        //                TargetList.Clear();
        //            }
        //        }
        //        else
        //        {
        //            //TargetList.Add(target);
        //            GameUnit.GameUnit unit1 = map.GetUnitsOnMapBlock(TargetList[0]);
        //            Vector2 unit2 = target;
        //            UnitMoveCommand moveCommand = new UnitMoveCommand(unit1, unit2);
        //            if (moveCommand.Judge())
        //            {
        //                //关闭染色
        //                unit1.GetComponent<ShowRange>().CancleAttackRangeMark();
        //                unit1.GetComponent<ShowRange>().CancleMoveRangeMark();

        //                moveCommand.Excute();
        //                TargetList.Clear();
        //            }
        //        }
        //    }
        //}
    }
}
