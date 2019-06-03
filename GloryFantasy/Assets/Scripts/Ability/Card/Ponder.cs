using System;
using System.Collections;
using System.Collections.Generic;
using GameCard;
using UnityEngine;

using IMessage;
using GamePlay;

namespace Ability
{
    public class Ponder : Ability
    {
        Trigger trigger;

//        private void Awake()
//        {
//            //导入Ponder异能的参数
//            InitialAbility("Ponder");
//        }
//
//        private void Start()
//        {
//            //创建Trigger实例，传入技能的发动者
//            trigger = new TPonder(this.GetCardReceiver(this));
//            //注册Trigger进消息中心
//            MsgDispatcher.RegisterMsg(trigger, "Ponder");
//        }

        public override void Init(string abilityId)
        {
            base.Init(abilityId);
            trigger = new TPonder(this.GetCardReceiver(this), AbilityVariable.Amount.Value, AbilityVariable.Draws.Value, abilityId);
            MsgDispatcher.RegisterMsg(trigger, abilityId);
        }
    }

    public class TPonder : Trigger
    {
        private int _amount;
        private int _draws;
        private string _abilityId;
        
        public TPonder(MsgReceiver speller, int amount, int draws, string abilityId)
        {
            _amount = 2;
            _draws = draws;
            _abilityId = abilityId;
            register = speller;
            //初始化响应时点,为卡片使用时
            msgName = (int)MessageType.CastCard;
            //初始化条件函数和行为函数
            action = Action;
            condition = Condition;
        }

        private bool Condition()
        {
            if (this.GetCastingCard().GetMsgReceiver() == register && this.GetCastingCard().ability_id.Contains(_abilityId))
                return true;
            else
                return false;
        }

        private void Action()
        {
            //TODO抽两张牌
            CardManager.Instance().ExtractCards(_draws);
            //TODO选择手牌
            CardManager.Instance().selectingMode = true;
            CardManager.Instance().cb = OnSlectionOver;
        }

        private void OnSlectionOver()
        {
            BaseCard card = Gameplay.Instance().gamePlayInput.InputFSM.selectedCard;
            CardManager.Instance().selectingMode = false;

            if (_abilityId.Contains("_1"))
            {
                //将选择的冷却两回合
                CardManager.Instance().RemoveCardToCd(card.gameObject, _amount);
            }
            else
            {
                // 将选择的卡牌放入牌库
                CardManager.Instance().MoveBackToCardSets(card.gameObject);
            }
            
        }
    }
}
