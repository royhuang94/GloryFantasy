using GamePlay;
using IMessage;
using Mediator;
using UnityEngine;

namespace Ability
{
    
    /// <summary>
    /// 此单位受到伤害时，MOV变为{Amount}直到下一个你的回合结束。
    /// </summary>
    public class Shadow_Revange : Ability
    {
        
        private Trigger _trigger;
        private GameUnit.GameUnit _unit;
        
        public override void Init(string abilityId)
        {
            base.Init(abilityId);
            _trigger = new TShadow_Revange(
                this.GetUnitReceiver(this),
                AbilityVariable.Amount.Value,
                GetComponent<GameUnit.GameUnit>(),
                abilityId);
            
            MsgDispatcher.RegisterMsg(_trigger, abilityId);
        }
    }

    public class TShadow_Revange : Trigger
    {
        private int _amount;
        private int _mov;
        private GameUnit.GameUnit _unit;
        private string _abilityId;

        public TShadow_Revange(MsgReceiver speller, int amount, GameUnit.GameUnit unit, string abilityId)
        {
            _amount = amount;
            _unit = unit;
            _abilityId = abilityId;
            _mov = unit.mov;
            register = speller;
            msgName = (int) MessageType.BeDamaged;
            condition = Condition;
            action = Action;
        }

        /// <summary>
        ///  检查单位有没有死
        /// </summary>
        /// <returns></returns>
        private bool Condition()
        {
            return !AbilityMediator.Instance().CheckUnitDeathById(_unit.id);
        }

        private void Action()
        {
            Trigger dt = new DelayedTrigger(
                register,
                1,
                (int)MessageType.MPEnd,
                () => { _unit.mov = _mov; });
            
            MsgDispatcher.RegisterMsg(dt, _abilityId + "--DT", true);

            _unit.mov = _amount;
        }
    }
}