using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using IMessage;
using GamePlay;
using GamePlay.Input;

namespace Ability
{
    public class Throw : Ability
    {
        Trigger trigger;

//        private void Awake()
//        {
//            //导入Jump异能的参数
//            InitialAbility("Fling");
//        }
//
//        private void Start()
//        {
//            //创建Trigger实例，传入技能的发动者
//            trigger = new TFling(this.GetCardReceiver(this));
//            //注册Trigger进消息中心
//            MsgDispatcher.RegisterMsg(trigger, "Fling");
//        }

        public override void Init(string abilityId)
        {
            base.Init(abilityId);
            trigger =
                new TFling(this.GetCardReceiver(this), AbilityVariable.Damage.Value, AbilityVariable.Range.Value, abilityId);
            MsgDispatcher.RegisterMsg(trigger, abilityId);
        }
    }

    public class TFling : Trigger
    {
        private int _damage;
        private int _range;
        private string _abilityId;
        
        public TFling(MsgReceiver speller, int damage, int range, string abilityId)
        {
            _damage = damage;
            _range = range;
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
            //获取被选中的敌军，需要自己根据技能描述强转类型，一旦强转的类型是错的代码会出错
            GameUnit.GameUnit source = (GameUnit.GameUnit)this.GetSelectingUnits()[0];
            GameUnit.GameUnit unit = (GameUnit.GameUnit)this.GetSelectingUnits()[1];
            BattleMap.BattleMapBlock battleMapBlock = (BattleMap.BattleMapBlock)this.GetSelectingUnits()[2];


            //将该单位移动到三格内一格
            SkillJumpCommand skillJumpCommand = new SkillJumpCommand(unit, battleMapBlock.position, _range);
            skillJumpCommand.Excute();
            //if (_abilityId.Split('_')[1] == "2")
            //{
            //    GameplayToolExtend.DealDamage()
            //}
        }
    }
}