using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using IMessage;
using GamePlay;
using GamePlay.Input;
using GamePlay.FSM;

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
                new TFling(this.GetCardReceiver(this), AbilityVariable.Damage.Value, AbilityVariable.Range.Value, AbilityVariable.Area.Value, abilityId);
            MsgDispatcher.RegisterMsg(trigger, abilityId);

            MyTargetConstraintList[1] = Range_1;
            MyTargetConstraintList[2] = Range_2;

        }

        public bool Range_1(object target)
        {
            Vector2 userPos = GamePlay.Gameplay.Instance().gamePlayInput.InputFSM.TargetList[0];

            if (GameplayToolExtend.distanceBetween(userPos, target) <= 2)
                return true;
            return false;
        }

        public bool Range_2(object target)
        {
            Vector2 userPos = GamePlay.Gameplay.Instance().gamePlayInput.InputFSM.TargetList[0];
            BattleMap.BattleMapBlock block = (BattleMap.BattleMapBlock)target;
            GameUnit.GameUnit unit = BattleMap.BattleMap.Instance().GetUnitsOnMapBlock(new Vector2(block.position.x, block.position.y));
            if (GameplayToolExtend.distanceBetween(userPos, target) <= AbilityVariable.Range.Value && unit == null)
                return true;
            return false;
        }
    }

    public class TFling : Trigger
    {
        private int _damage;
        private int _range;
        private string _abilityId;
        private int _area;
        
        public TFling(MsgReceiver speller, int damage, int range, int area, string abilityId)
        {
            _damage = damage;
            _range = range;
            _abilityId = abilityId;
            _area = area;
            
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
            GameUnit.GameUnit user = (GameUnit.GameUnit)this.GetSelectingUnits()[0];
            GameUnit.GameUnit unit = (GameUnit.GameUnit)this.GetSelectingUnits()[1];
            BattleMap.BattleMapBlock battleMapBlock = (BattleMap.BattleMapBlock)this.GetSelectingUnits()[2];


            //将该单位移动到三格内一格
            SkillJumpCommand skillJumpCommand = new SkillJumpCommand(unit, battleMapBlock.position, _range);
            skillJumpCommand.Excute();
            if (_abilityId.Split('_')[1] == "2")
            {
                GameplayToolExtend.DealDamage(user, unit, new Damage(_damage));
            }
            else if (_abilityId.Split('_')[1] == "3")
            {
                Vector2 v = new Vector2(battleMapBlock.position.x, battleMapBlock.position.y);
                foreach(GameUnit.GameUnit suffer in GameplayToolExtend.GetUnitsByBlocks(GameplayToolExtend.getAreaByBlock(_area, battleMapBlock)))
                {
                    GameplayToolExtend.DealDamage(user, suffer, new Damage(_damage));
                }
            }
        }
    }
}