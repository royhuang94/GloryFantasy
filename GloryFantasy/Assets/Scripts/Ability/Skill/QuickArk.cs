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

        private void Awake()
        {
            InitialAbility("QuickArk");
            _trigger = new TQuickArk(this.GetCardReceiver(this), gameObject);
            MsgDispatcher.RegisterMsg(_trigger, "QuickArk");
        }

//        private void Start()
//        {
//            
//        }
    }

    public class TQuickArk : Trigger
    {
        private GameObject unit;
        private ESSlot _esSlot;

        public TQuickArk(MsgReceiver speller, GameObject unit)
        {
            this.unit = unit;

            register = speller;
            msgName = (int) MessageType.Summon;

            action = Action;
            condition = Condition;
        }

        /// <summary>
        /// 这个脚本就是把挂在单位身上的战技牌放到手牌里，如果有的话
        /// </summary>
        /// <returns></returns>
        private bool Condition()
        {
            if (_esSlot == null)
                _esSlot = unit.GetComponent<ESSlot>();
            return  _esSlot.ExSkillCards.Count != 0;
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