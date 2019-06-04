using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using IMessage;
using GamePlay;

namespace Ability
{
    public class Sandworm_Awake : Ability
    {
        Trigger trigger;

//        private void Awake()
//        {
//            //导入Regeneration异能的参数
//            InitialAbility("Regeneration");
//        }
//
//        private void Start()
//        {
//            //创建Trigger实例，传入技能的发动者
//            trigger = new TRegeneration(GetComponent<GameUnit.GameUnit>().GetMsgReceiver());
//            //注册Trigger进消息中心
//            MsgDispatcher.RegisterMsg(trigger, "Regeneration");
//        }

        public override void Init(string abilityId)
        {
            base.Init(abilityId);
            trigger = new TSandworm_Awake(GetComponent<GameUnit.GameUnit>().GetMsgReceiver());
            //注册Trigger进消息中心
            MsgDispatcher.RegisterMsg(trigger, abilityId);
        }
    }

    public class TSandworm_Awake : Trigger
    {
        public TSandworm_Awake(MsgReceiver speller)
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
            string _CR = deadUnit.id.Substring(deadUnit.id.Length - 1, 1);
            GameUnit.GameUnit newUnit = this.Regenerate("SandwormHead_" + _CR, deadUnit.mapBlockBelow);
            //删除这只怪的复活技能
            this.DeleteUnitAbility(newUnit, "Sandworm_Awake");
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