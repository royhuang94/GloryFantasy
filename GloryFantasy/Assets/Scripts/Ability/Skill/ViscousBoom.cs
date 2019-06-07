using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using IMessage;
using GamePlay;
using Ability.Debuff;

namespace Ability
{
    /// <summary>
    /// 此单位溃退时在地上留一滩粘滞地形。
    /// </summary>
    public class ViscousBoom : Ability
    {
        Trigger trigger;

        public override void Init(string abilityId)
        {
            base.Init(abilityId);
            trigger = new TViscousBoom(GetComponent<GameUnit.GameUnit>().GetMsgReceiver(), AbilityVariable.Area.Value, AbilityVariable.Turns.Value);
            //注册Trigger进消息中心
            MsgDispatcher.RegisterMsg(trigger, abilityId);
        }
    }

    public class TViscousBoom : Trigger
    {
        private int _area;
        private int _turns;
        public TViscousBoom(MsgReceiver speller, int area, int turns)
        {
            register = speller;
            //初始化响应时点
            msgName = (int)MessageType.Dead;
            _area = area;
            _turns = turns;
            //初始化条件函数和行为函数
            condition = Condition;
            action = Action;
        }
        private void Action()
        {
            GameUnit.GameUnit deadUnit = this.GetDead();
            List<BattleMap.BattleMapBlock> effectBlocks = GameplayToolExtend.getAreaByBlock(_area, deadUnit.mapBlockBelow);
            foreach(BattleMap.BattleMapBlock block in effectBlocks)
            {
                block.gameObject.AddBuff<BViscous>((float)_turns);
            }
        }

        private bool Condition()
        {
            
            if (this.GetDead().GetMsgReceiver() == register)
                return true;
            else
                return false;
        }
    }
}