using System;
using GameCard;
using IMessage;
using GamePlay;
using GameUnit;

namespace Ability
{
    public class Wit : Ability
    {
        private Trigger _trigger;

        private void Awake()
        {
            InitialAbility("Wit");
        }

        private void Start()
        {
            String targetId = gameObject.GetComponent<GameUnit.GameUnit>().id;
            _trigger = new TWit(this.GetCardReceiver(this), targetId);
            MsgDispatcher.RegisterMsg(_trigger, "Wit");
        }
    }

    public class TWit : Trigger
    {
        private AbilityVariable _abilityVariable;
        private string _targetId;

        public TWit(MsgReceiver speller, string targetId)
        {
            _abilityVariable = AbilityDatabase.GetInstance().GetAbilityVariable("Wit");
            if (_abilityVariable == null)
                throw new NotImplementedException();
            _targetId = targetId;
            
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
            return !GameUnitPool.Instance().CheckDeathByID(_targetId);
        }

        private void Action()
        {
            // 填充参数调用接口，完成指定用户使用卡牌cd减少指定回合cd功能
            CardManager.Instance().HandleCooldownEvent(_targetId, _abilityVariable.Amount.Value);
        }
    }
}