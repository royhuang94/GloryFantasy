﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using IMessage;

namespace Ability
{
    public class ShunFaChuangXiang : Ability
    {
        Trigger trigger;

        private void Start()
        {
            //创建Trigger实例，传入技能的发动者
            trigger = new TShunFaChuangXiang(this.GetUnitReceiver(this));
            //注册Trigger进消息中心
            MsgDispatcher.RegisterMsg(trigger, "ShunFaChuangXiang");
        }

    }

    public class TShunFaChuangXiang : Trigger
    {
        public TShunFaChuangXiang(MsgReceiver speller)
        {
            register = speller;
            //初始化响应时点,为卡片使用时
            msgName = (int)TriggerType.CastCard;
            //初始化条件函数和行为函数
            condition = Condition;
            action = Action;
        }

        private bool Condition()
        {
            //判断发动的卡是不是这个技能的注册者，并且这张卡是不是瞬发幻想
            if (this.GetCastingCard().GetMsgReceiver() == register && this.GetCastingCard().id == "WInstant_1")
                return true;
            else
                return false;
        }

        private void Action()
        {
            //获取被选中的友军
            GameUnit.GameUnit unit = this.GetSelectingUnit();
            //复制被选中友军的一次性战技入手牌
        }
    }
}