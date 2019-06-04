using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using IMessage;
using GamePlay;

namespace Ability
{
    /// <summary>
    /// 此单位溃退时转化为Gargoyle_0。
    /// </summary>
    public class Gargole_sleep : Ability
    {
        Trigger trigger;

        public override void Init(string abilityId)
        {
            base.Init(abilityId);
            trigger = new TGargole_sleep(GetComponent<GameUnit.GameUnit>().GetMsgReceiver());
            //注册Trigger进消息中心
            MsgDispatcher.RegisterMsg(trigger, abilityId);
        }
    }

    public class TGargole_sleep : Trigger
    {
        public TGargole_sleep(MsgReceiver speller)
        {
            register = speller;
            //初始化响应时点
            msgName = (int)MessageType.Dead;
            //初始化条件函数和行为函数
            condition = Condition;
            action = Action;
        }
        private void Action()
        {
            //保存死掉的怪
            GameUnit.GameUnit deadUnit = this.GetDead();
            //复活死掉的怪并保存
            GameUnit.GameUnit newUnit = this.Regenerate("Gargoyle_0", deadUnit.mapBlockBelow);
            //删除这只怪的复活技能
            this.DeleteUnitAbility(newUnit, "Gargole_sleep");
        }

        private bool Condition()
        {
            //判断死掉的怪是不是这个复活技能的注册者
            if (this.GetDead().GetMsgReceiver() == register)
                return true;
            else
                return false;
        }
    }
}