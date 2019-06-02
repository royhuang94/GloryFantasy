using System;
using Ability.Buff;
using GamePlay;
using GameUnit;
using IMessage;
using Mediator;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Ability
{
    /// <summary>
    /// 使用者所在区域的敌方单位获得 滞击 / 滞击 / 目盲，{Turns}回合后消解
    /// </summary>
    public class Mist : Ability
    {
        private Trigger _trigger;

        public override void Init(string abilityId)
        {
            base.Init(abilityId);
            _trigger = new TMist(
                this.GetCardReceiver(this),
                AbilityVariable.Turns.Value, 
                GetComponent<GameUnit.GameUnit>().CurPos,
                abilityId);
            MsgDispatcher.RegisterMsg(_trigger, abilityId);
        }
    }

    public class TMist : Trigger
    {
        private int _turns;
        private string _abilityId;
        private Vector2 _currentPos;
        
        public TMist(MsgReceiver speller, int turns, Vector2 pos, string abilityId)
        {
            _turns = turns;
            _abilityId = abilityId;
            _currentPos = pos;
            register = speller;
            msgName = (int) MessageType.CastCard;
            condition = Condition;
            action = Action;
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
            foreach (GameUnit.GameUnit unit in AbilityMediator.Instance().GetGameUnitsInBattleArea(_currentPos))
            {
                if (unit.owner == OwnerEnum.Enemy)
                {
                    unit.gameObject.AddBuff<BConfused>((float)_turns);
                    if (_abilityId.Contains("_3"))
                    {
                        unit.gameObject.AddBuff<BBlind>((float)_turns);
                    }
                }
            }
            
        }
    }
}