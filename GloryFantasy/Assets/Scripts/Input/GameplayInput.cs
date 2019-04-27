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
        //保存移动前的单位
        public List<GameUnit.GameUnit> BeforeMoveGameUnits = new List<GameUnit.GameUnit>();

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
            if (IsMoving)
            {
                
                GameUnit.GameUnit unit = BattleMap.BattleMap.Instance().GetUnitsOnMapBlock(TargetList[0]);
                Vector2 startPos = TargetList[0];
                Vector2 endPos = mapBlock.position;
                UnitMoveCommand unitMove = new UnitMoveCommand(unit, startPos, endPos,  mapBlock.GetSelfPosition() );
                if (unitMove.Judge())
                {
                    GameUtility.UtilityHelper.Log("移动完成，进入待机，再次点击进入攻击", GameUtility.LogColor.RED);
                    unitMove.Excute();                   
                    SetMovingIsFalse(unit);
                    IsAttacking = false;
                    unit.restrain = true;
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
            Debug.Log("fdsf");
            //鼠标右键取消攻击
            if (IsAttacking == true && eventData.button == PointerEventData.InputButton.Right)
            {
                GameUtility.UtilityHelper.Log("取消攻击", GameUtility.LogColor.RED);
                HandleAtkCancel(TargetList[0]);
                IsAttacking = false;
                unit.restrain = false;
            }
            else if (IsMoving)
            {
                //如果移动两次都选择同一个单位，就进行一次待机
                Vector2 pos = BattleMap.BattleMap.Instance().GetUnitCoordinate(unit);
                if (TargetList[0] == pos )
                {
                    GameUtility.UtilityHelper.Log("进入待机，再次点击进入攻击", GameUtility.LogColor.RED);
                    SetMovingIsFalse(unit);
                    unit.restrain = true;
                    IsAttacking = false;
                }
                else
                {
                    //点到其他单位什么都不做
                }
            }
            //如果单位可以移动
            else if (unit.restrain == false)
            {
                GameUtility.UtilityHelper.Log("准备移动", GameUtility.LogColor.RED);
                SetMovingIsTrue(unit);
            }
            //如果单位已经不能移动，但是可以攻击
            else if (unit.restrain == true && unit.disarm == false)
            {
                BeforeMoveGameUnits.Add(unit);//
                GameUtility.UtilityHelper.Log("准备攻击，右键取消攻击", GameUtility.LogColor.RED);
                IsAttacking = true;
                TargetList.Add(BattleMap.BattleMap.Instance().GetUnitCoordinate(unit));
                //显示攻击范围
                HandleAtkConfirm(TargetList[0]);//显示攻击范围
            }
        }

        public void OnPointerDownEnemy(GameUnit.GameUnit unit, PointerEventData eventData)
        {
            if (IsAttacking)
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
                    BeforeMoveGameUnits[0].restrain = false;
                    IsMoving = false;
                    HandleAtkCancel(TargetList[0]);////攻击完工攻击范围隐藏  
                    BeforeMoveGameUnits.Clear();
                    TargetList.Clear();              

                }
                else
                {
                    //如果攻击指令不符合条件就什么都不做
                }
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
            BeforeMoveGameUnits.Add(unit);
            HandleMovConfirm(pos);
        }
        public void SetMovingIsFalse(GameUnit.GameUnit unit)
        {
            IsMoving = false;
            Vector2 pos = BattleMap.BattleMap.Instance().GetUnitCoordinate(unit);
            HandleMovCancel(pos);//移动完毕关闭移动范围染色
            BeforeMoveGameUnits.Clear();
            TargetList.Clear();
            
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
            GameUnit.GameUnit unit = null;
            if (BattleMap.BattleMap.Instance().CheckIfHasUnits(target))
            {
                unit = BattleMap.BattleMap.Instance().GetUnitsOnMapBlock(target);
            }
            else
            {
                unit = BeforeMoveGameUnits[0];
            }
            unit.GetComponent<ShowRange>().CancleMoveRangeMark(TargetList[0]);
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
            GameUnit.GameUnit unit = null;
            if (BattleMap.BattleMap.Instance().CheckIfHasUnits(target))
            {
                unit = BattleMap.BattleMap.Instance().GetUnitsOnMapBlock(target);
            }
            else
            {
                unit = BeforeMoveGameUnits[0];
            }
            unit.GetComponent<ShowRange>().CancleAttackRangeMark(TargetList[0]);

        }

        /// <summary>
        /// 设置被选中的手牌槽
        /// </summary>
        /// <param name="currentItemUI"></param>
        internal void SelectSlotUnit(UnitSlot currentItemUI)
        {
            selectedSlot = currentItemUI;
        }

        /// <summary>
        /// 单位回收
        /// </summary>
        /// <param name="deadUnit"></param>
        internal void UnitBackPool(GameUnit.GameUnit deadUnit)
        {
            //回收单位
            GameUnitPool.Instance().PushUnit(deadUnit.gameObject);
        }

        /// <summary>
        /// 更新血条HP
        /// </summary>
        /// <param name="attackedUnit">受攻击单位</param>
       internal void UpdateHp(GameUnit.GameUnit attackedUnit)
        {
            float hpDivMaxHp = (float)attackedUnit.hp / attackedUnit.MaxHP * 100;
            var textHp = attackedUnit.transform.GetComponentInChildren<Text>();
            textHp.text = string.Format("Hp: {0}%", Mathf.Ceil(hpDivMaxHp));
        }
    }
}
