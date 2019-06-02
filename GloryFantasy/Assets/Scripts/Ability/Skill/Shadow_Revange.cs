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
        private GameUnit.GameUnit _unit;
        private string _abilityId;

        public TShadow_Revange(MsgReceiver speller, int amount, GameUnit.GameUnit unit, string abilityId)
        {
            _amount = amount;
            _unit = unit;
            _abilityId = abilityId;
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
            _unit.gameObject.AddBuff<BShadow_Revenge>(1f);
        }
    }

    public class BShadow_Revenge : Buff.Buff
    {
        private int _deltamov;
        //设定Buff的初始化
        public override void InitialBuff()
        {
            //设定Buff的生命周期，两种写法,建议使用第二种，比较直观
            SetLife(2f);

            //Buff要做的事情，可以像Ability一样也写Trigger，也可以只是做一些数值操作。和Ability一样公用一套工具函数库
            GameUnit.GameUnit unit = GetComponent<GameUnit.GameUnit>();
            _deltamov = unit.mov - 4;
            unit.mov -= _deltamov;
        }

        
        //设定Buff消失时的逆操作
        protected override void OnDisappear()
        {
            GameUnit.GameUnit unit = GetComponent<GameUnit.GameUnit>();
            unit.mov += _deltamov;
        }
    }
}