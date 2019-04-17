using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using IMessage;

namespace Ability
{
    public class FuHuo : Ability
    {
        Trigger trigger;

        private void Start()
        {
            //创建Trigger实例，传入技能的发动者
            trigger = new TFuHuo(GetComponent<GameUnit.GameUnit>().GetMsgReceiver());
            //注册Trigger进消息中心
            MsgDispatcher.RegisterMsg(trigger, "FuHuo");
        }

    }

    public class TFuHuo : Trigger
    {
        public TFuHuo(MsgReceiver speller)
        {
            register = speller;
            //初始化响应时点
            msgName = (int)TriggerType.Dead;
            //初始化条件函数和行为函数
            condition = Condition;
            action = Action;
        }

        private bool Condition()
        {
            //判断死掉的怪是不是这个复活技能的注册者
            if (this.GetDead().GetMsgReceiver() == register)
                return true;
            else
                return false;
        }

        private void Action()
        {
            //保存死掉的怪
            GameUnit.GameUnit deadUnit = this.GetDead();
            //复活死掉的怪并保存
            GameUnit.GameUnit newUnit = this.Regenerate(deadUnit.Name, this.GetUnitPosition(deadUnit));
            //修改这只怪的血量
            newUnit.hp -= newUnit.hp / 2;
            //删除这只怪的复活技能
            this.DeleteUnitAbility(newUnit, "FuHuo");
        }
    }
}