﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using IMessage;
using GamePlay;
using GamePlay.Input;
using Random = System.Random;

namespace Ability
{
    public class Jump : Ability
    {
        Trigger trigger;

//        private void Awake()
//        {
//            //导入Jump异能的参数
//            InitialAbility("Jump");
//        }
//
//        private void Start()
//        {
//            //创建Trigger实例，传入技能的发动者
//            trigger = new TJump(this.GetCardReceiver(this));
//            //注册Trigger进消息中心
//            MsgDispatcher.RegisterMsg(trigger, "Jump");
//        }

        public override void Init(string abilityId)
        {
            base.Init(abilityId);
            trigger = new TJump(this.GetCardReceiver(this), AbilityVariable.Range.Value, abilityId);
            MsgDispatcher.RegisterMsg(trigger, abilityId);

            MyTargetConstraintList[1] = Range_1;
        }

        public bool Range_1(object target)
        {
            return GameplayToolExtend.distanceBetween(
                       Gameplay.Instance().gamePlayInput.InputFSM.TargetList[0],
                       target) <= AbilityVariable.Range.Value;
        }

    }

    public class TJump : Trigger
    {
        private int _jumpRange;
        private string _abilityId;
        public TJump(MsgReceiver speller, int range, string abilityId)
        {
            _jumpRange = range;
            _abilityId = abilityId;
            register = speller;
            //初始化响应时点,为卡片使用时
            msgName = (int)MessageType.CastCard;
            //初始化条件函数和行为函数
            condition = Condition;
            action = Action;    
        }

        private bool Condition()
        {
            //判断发动的卡是不是这个技能的注册者，并且这张卡是不是轻身飞跃
            if (this.GetCastingCard().GetMsgReceiver() == register && this.GetCastingCard().ability_id.Contains(_abilityId))
                return true;
            else
                return false;
        }

        private void Action()
        {
            //获取被选中的友军，需要自己根据技能描述强转类型，一旦强转的类型是错的代码会出错
            GameUnit.GameUnit unit = (GameUnit.GameUnit)this.GetSelectingUnits()[0];
            BattleMap.BattleMapBlock battleMapBlock = (BattleMap.BattleMapBlock)this.GetSelectingUnits()[1];
            

            //将该单位移动到三格内一格
            SkillJumpCommand skillJumpCommand = new SkillJumpCommand(unit, battleMapBlock.position, _jumpRange);
            skillJumpCommand.Excute();
        }
    }
}