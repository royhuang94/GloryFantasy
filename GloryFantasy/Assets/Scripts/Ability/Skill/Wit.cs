using System;
using GameCard;
using IMessage;
using GamePlay;
using GameUnit;
using Mediator;

namespace Ability
{
    public class Wit : Ability
    {
        private Trigger _trigger;

//        private void Awake()
//        {
//            InitialAbility("Wit");
//        }
//
//        private void Start()
//        {
//            String targetId = gameObject.GetComponent<GameUnit.GameUnit>().id;
//            _trigger = new TWit(this.GetCardReceiver(this), targetId);
//            MsgDispatcher.RegisterMsg(_trigger, "Wit");
//        }

        public override void Init(string abilityId)
        {
            base.Init(abilityId);
            GameUnit.GameUnit unit = gameObject.GetComponent<GameUnit.GameUnit>();
            _trigger = new TWit(this.GetUnitReceiver(this), GetInstanceID(), AbilityVariable.Amount.Value);
            MsgDispatcher.RegisterMsg(_trigger, abilityId);
        }
    }

    public class TWit : Trigger
    {
        private int _amount;
        private int _binderInstanceId;

        public TWit(MsgReceiver speller, int binderInstanceId, int amount)
        {
            _amount = amount;
            _binderInstanceId = binderInstanceId;
            register = speller;
            //初始化响应时点,为卡片使用时
            msgName = (int)MessageType.CastCard;
            //初始化条件函数和行为函数
            action = Action;
            condition = Condition;
        }

        /// <summary>
        /// 确定绑定此异能的单位没有死亡
        /// </summary>
        /// <returns>没有死亡就是true</returns>
        private bool Condition()
        {
            if (Gameplay.Instance().gamePlayInput.InputFSM.TargetList.Count == 0)
                return false;
            
            return BattleMap.BattleMap.Instance().GetUnitsOnMapBlock(
                       Gameplay.Instance().gamePlayInput.InputFSM.TargetList[0]
                       ).GetInstanceID() == _binderInstanceId;
        }

        private void Action()
        {
            // 完成指定用户使用卡牌cd减少指定回合cd功能
            AbilityMediator.Instance().ReduceSpecificCardCd(_binderInstanceId.ToString(), _amount);
        }
    }
}