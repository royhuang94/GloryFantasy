using System;
using GameCard;
using GamePlay;
using IMessage;
using UnityEngine;

namespace Ability
{
    public class QuickArk : Ability
    {
        private Trigger _trigger;

//        private void Awake()
//        {
//            InitialAbility("QuickArk");
//            _trigger = new TQuickArk(this.GetUnitReceiver(this), gameObject);
//            MsgDispatcher.RegisterMsg(_trigger, "QuickArk");
//            //_trigger = new TQuickArk(this.GetCardReceiver(this), gameObject);
//            
//        }

        public override void Init(string abilityId)
        {
            base.Init(abilityId);
            if(gameObject != null)
            {
                _trigger = new TQuickArk(this.GetUnitReceiver(this), gameObject);
                MsgDispatcher.RegisterMsg(_trigger, abilityId);
            }           
        }
    }

    public class TQuickArk : Trigger
    {
        private GameObject unit;
        private ESSlot _esSlot;

        public TQuickArk(MsgReceiver speller, GameObject unit)
        {
            this.unit = unit;

            register = speller;
            msgName = (int) MessageType.GenerateUnit;

            action = Action;
            condition = Condition;
        }

        /// <summary>
        /// 这个脚本就是把挂在单位身上的战技牌放到手牌里，如果有的话
        /// </summary>
        /// <returns></returns>
        private bool Condition()
        {
            if (_esSlot == null && unit != null)
                _esSlot = unit.GetComponent<ESSlot>();
            //_esSlot还是null
            if(_esSlot != null)
                return  _esSlot.ExSkillCards.Count != 0;
            return false; //_esSlot为null，返回false
        }

        private void Action()
        {
            for (int i = 0; i < _esSlot.ExSkillCards.Count; i++)
            {
                _esSlot.SettleESCard(i, true);
            }
        }
    }
}