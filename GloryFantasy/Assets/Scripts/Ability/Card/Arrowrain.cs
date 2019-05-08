using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using IMessage;
using GamePlay;

namespace Ability
{
    public class Arrowrain : Ability
    {
        Trigger trigger;

        private void Awake()
        {
            //导入Arrowrain异能的参数
            InitialAbility("Arrowrain");
        }

        private void Start()
        {
            //创建Trigger实例，传入技能的发动者
            trigger = new TArrowrain(this.GetCardReceiver(this));
            //注册Trigger进消息中心
            MsgDispatcher.RegisterMsg(trigger, "Arrowrains");
        }

    }

    public class TArrowrain : Trigger
    {
        public TArrowrain(MsgReceiver speller)
        {
            register = speller;
            //初始化响应时点,为卡片使用时
            msgName = (int)MessageType.CastCard;
            //初始化条件函数和行为函数
            condition = Condition;
            action = Action;
        }

        private bool Condition()
        {
            //判断发动的卡是不是这个技能的注册者，并且这张卡是不是箭雨
            if (this.GetCastingCard().GetMsgReceiver() == register && this.GetCastingCard().id == "WArrowrain_1")
                return true;
            else
                return false;
        }

        private void Action()
        {

        }
    }
}
