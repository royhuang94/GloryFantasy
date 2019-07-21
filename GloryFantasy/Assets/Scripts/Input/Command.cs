﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using IMessage;
using BattleMap;
using GameUnit;

namespace GamePlay.Input
{
    //指令类，在UI下面真正执行游戏逻辑的是这些Command类
    //也就是说 玩家输入鼠标 → GUI → Command → GamePlay，BattaleMap等游戏类

    public class Command : GameplayTool
    {
        virtual public void Excute() { }
    }

    public class SelectUnitCommand : Command
    {
        public SelectUnitCommand(GameUnit.GameUnit unit)
        {
            _unit = unit;
        }

        public override void Excute()
        {
            Gameplay.Info.SelectingUnit = _unit;
        }

        private GameUnit.GameUnit _unit;
    }

    /// <summary>
    /// 部署指令。
    /// </summary>
    /// <param name="unitID">要部署的单位的ID。</param>
    /// <param name="owner">部属单位的操控者。</param>
    /// <param name="battleMapBlock">部署在的地格。</param>
    /// <param name="post">是否需要发送部署消息，可缺省，默认为是。请在特殊情况下才设置为否。</param>

    public class DispositionCommand : Command
    {
        public DispositionCommand(string unitID, OwnerEnum owner, BattleMapBlock battleMapBlock, GameCard.BaseCard card = null)  //构造函数
        {
            _unitID = unitID;
            _owner = owner;
            _battleMapBlock = battleMapBlock;
            if (card != null)
                _card = card as GameCard.UnitCard;
        }

        public void set(string unitID, OwnerEnum owner, BattleMapBlock battleMapBlock, GameCard.BaseCard card = null)    //创建对象后通过此方法修改参数
        {
            _unitID = unitID;
            _owner = owner;
            _battleMapBlock = battleMapBlock;
            if (card != null)
                _card = card as GameCard.UnitCard;
        }

        public bool Judge()
        {
            if (_battleMapBlock.units_on_me.Count == 0)
                return true;
            return false;
        }

        public override void Excute()
        {
            //更新本此召唤的怪物（覆盖方式）
            UnitManager.InstantiationUnit(_unitID, _owner, _battleMapBlock);

            //Debug.Log(_battleMapBlock.units_on_me.ToString());
            GameUnit.GameUnit unit = this.GetGeneratingUnit();
            this.SetSummonUnit(new List<GameUnit.GameUnit> { unit });
            if (_card != null)
            {
                unit.card = _card;
                _card.SetUnit(unit);
                _card.cardArea = GameCard.CardArea.Field;
            }
            MsgDispatcher.SendMsg((int)MessageType.Summon);

            //更新仇恨列表
            if (!unit.IsDead && unit.owner != OwnerEnum.Enemy)
                Gameplay.Instance().autoController.UpdateAllHatredList(null, new List<GameUnit.GameUnit> { unit });
            else if (!unit.IsDead)
                Gameplay.Instance().autoController.AddAIController(unit);
        }

        private string _unitID;
        private OwnerEnum _owner;
        private BattleMapBlock _battleMapBlock;
        private GameCard.UnitCard _card;
    }


    public class SkillJumpCommand : Command
    {
        public SkillJumpCommand(GameUnit.GameUnit unit, Vector2 targetPos, int maxJumpPower)
        {
            _unit = unit;
            _targetPos = targetPos;
            _maxJumpPower = maxJumpPower;
        }

        public bool Judge()
        {
            Vector2 unit1 = _unit.CurPos;
            Vector2 unit2 = _targetPos;
            int MAN_HA_DUN = Mathf.Abs((int)unit1.x - (int)unit2.x) + Mathf.Abs((int)unit1.y - (int)unit2.y);
            if (MAN_HA_DUN <= _maxJumpPower)
                return true;
            //BattleMap.BattleMap.Instance().MapNavigator
            return false;
        }

        public override void Excute()
        {
            //Gameplay.Instance().gamePlayInput.HandleSkillConfim(_unit.CurPos, _maxJumpPower);

            if (Judge())
            {
                BattleMap.BattleMap.Instance().MoveUnitToCoordinate(_targetPos, _unit);
                this.SetMovingUnit(_unit);
                MsgDispatcher.SendMsg((int)MessageType.Moved);
            }

        }

        private GameUnit.GameUnit _unit;
        private Vector2 _targetPos;
        private int _maxJumpPower;
    }

    public class UnitMoveAICommand : Command
    {
        /// <summary>
        /// AI移动类的构造函数
        /// </summary>
        /// <param name="unit">单位</param>
        /// <param name="_toTargetPath">最优路径</param>
        /// <param name="_callback">攻击回调</param>
        public UnitMoveAICommand(GameUnit.GameUnit unit, List<Vector2> _toTargetPath, System.Action _callback)
        {
            _unit = unit;
            toTargetPath = _toTargetPath;
            callback = _callback;
        }

        public override void Excute()
        {
            //Debug.Log("Moving Command excusing");
            this.SetMovingUnit(_unit);
            BattleMap.BattleMap.Instance().AIMoveCondition(_unit, toTargetPath, callback);
            //BattleMap.BattleMap.Instance().AIMoveUnitToCoordinate(_unit, toTargetPath, callback);
        }


        private System.Action callback;
        private GameUnit.GameUnit _unit;
        //移动到目标的路径
        private List<Vector2> toTargetPath;
    }

    public class UnitMoveCommand : Command
    {
        public UnitMoveCommand(GameUnit.GameUnit unit, Vector2 unitPositon, Vector2 targetPosion, Vector2 destination)
        {
            _unit = unit;
            _unitPosition = unitPositon;
            _targetPosition = targetPosion;
            _destination = destination;
        }

        public bool Judge()
        {
            Vector2 unit1 = _unitPosition;
            Vector2 unit2 = _targetPosition;
            int MAN_HA_DUN = Mathf.Abs((int)unit1.x - (int)unit2.x) + Mathf.Abs((int)unit1.y - (int)unit2.y);
            if (MAN_HA_DUN <= _unit.getMOV())
                return true;
            //BattleMap.BattleMap.Instance().MapNavigator
            return false;
        }

        public override void Excute()
        {
            //Debug.Log("Moving Command excusing");
            if (BattleMap.BattleMap.Instance().MapNavigator.PathSearch(_unitPosition, _targetPosition))
            {
                this.SetMovingUnit(_unit);
                //TODO 产生移动变化，检测
                BattleMap.BattleMap.Instance().MoveUnitToCoordinate(_unit, _targetPosition);
            }
        }

        private GameUnit.GameUnit _unit;
        private Vector2 _destination;
        private Vector2 _unitPosition;
        private Vector2 _targetPosition;
    }

    public class UnitAttackCommand : Command
    {
        public UnitAttackCommand(GameUnit.GameUnit Attacker, GameUnit.GameUnit AttackedUnit)
        {
            _Attacker = Attacker; this.SetAttacker(Attacker);
            _AttackedUnit = AttackedUnit; this.SetAttackedUnit(AttackedUnit);
        }

        /// <summary>
        /// 计算攻击距离是否大于曼哈顿
        /// </summary>
        /// <returns></returns>
        public bool Judge()
        {
            Vector2 unit1 = BattleMap.BattleMap.Instance().GetUnitCoordinate(_Attacker);
            Vector2 unit2 = BattleMap.BattleMap.Instance().GetUnitCoordinate(_AttackedUnit);
            int MAN_HA_DUN = Mathf.Abs((int)unit1.x - (int)unit2.x) + Mathf.Abs((int)unit1.y - (int)unit2.y);
            if (MAN_HA_DUN <= _Attacker.getRNG())
                return true;

            return false;
        }
        /// <summary>
        /// 计算反击距离是否大于曼哈顿
        /// </summary>
        public bool JudgeStrikeBack(int rng)
        {
            Vector2 unit1 = BattleMap.BattleMap.Instance().GetUnitCoordinate(_AttackedUnit);
            Vector2 unit2 = BattleMap.BattleMap.Instance().GetUnitCoordinate(_Attacker);
            int MAN_HA_DUN = Mathf.Abs((int)unit1.x - (int)unit2.x) + Mathf.Abs((int)unit1.y - (int)unit2.y);
            if (MAN_HA_DUN <= rng)
                return true;

            return false;
        }

        public override void Excute()
        {

            //根据伤害优先级对伤害请求排序
            GamePlay.Gameplay.Instance().damageManager.CaculateDamageRequestList(_Attacker, _AttackedUnit);
            MsgDispatcher.SendMsg((int)MessageType.AnnounceAttack);
            DamageRequestList = GamePlay.Gameplay.Instance().damageManager.damageRequestList;
            for (i = 0; i < DamageRequestList.Count; i++)
            {
                Ability.EffectStack.push(excuteDamageAtSameTime);
                Ability.EffectStack.turnsOn();
                if (_Attacker.IsDead || _AttackedUnit.IsDead)
                    break;
            }
        }

        public void excuteDamageAtSameTime()
        {
            for (; i + 1 < DamageRequestList.Count && (DamageRequestList[i].priority == DamageRequestList[i + 1].priority); i++)
            {
                DamageRequestList[i].Excute();
            }
        }

        //TODO 攻击制作
        //1. 通过变量_Attacker _AttackedUnit 保存宣言攻击者和被攻击者
        //2. 通过DamageRequestList  —> Damange类中
        //3. 通过Damage类与Command类来执行攻击环节，注意细节修改
        
        private GameUnit.GameUnit _Attacker; //宣言攻击者
        private GameUnit.GameUnit _AttackedUnit; //被攻击者
        private List<DamageRequest> DamageRequestList;
        private int i;
    }

    public class ReleaseSkillCommand : Command
    {
        public ReleaseSkillCommand(GameUnit.GameUnit skillMaker, Vector2 makerPosition, Vector2 targetPosition)
        {
            _skillMaker = skillMaker;
            _targetPosition = targetPosition;
            _makerPosition = makerPosition;
        }
        

        public override void Excute()
        {

        }
        private GameUnit.GameUnit _skillMaker;//技能释放者
        private Vector2 _targetPosition;//释放技能的目标点(中心点)
        private Vector2 _makerPosition;//释放者坐标
    }

}