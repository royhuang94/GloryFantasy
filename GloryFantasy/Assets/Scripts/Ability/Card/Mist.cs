using System;
using GamePlay;
using IMessage;
using UnityEngine; 

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
            _trigger = new TMist(this.GetCardReceiver(this), AbilityVariable.Turns.Value, gameObject, abilityId);
            MsgDispatcher.RegisterMsg(_trigger, abilityId);
        }
    }

    public class TMist : Trigger
    {
        private int _turns;
        private string _abilityId;
        private GameObject _obj;
        public TMist(MsgReceiver speller, int turns, GameObject obj, string abilityId)
        {
            _turns = turns;
            _abilityId = abilityId;
            _obj = obj;

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
            if (_abilityId.Contains("_3"))
            {
                Ability a = _obj.AddComponent<Blind>();
                a.Init("Blind");
                
                Trigger dt = new DelayedTrigger(
                    register,
                    _turns,
                    (int)MessageType.MPEnd,
                    () =>
                    {
                        GameObject.Destroy(_obj.GetComponent<Blind>());
                    });
                
                MsgDispatcher.RegisterMsg(dt, _abilityId, true);
            }
            else
            {
                Ability a = _obj.AddComponent<LastStrike>();
                a.Init("LastStrike");

                Trigger dt = new DelayedTrigger(
                    register,
                    _turns,
                    (int) MessageType.MPEnd,
                    ()=>
                    {
                        GameObject.Destroy(_obj.GetComponent<LastStrike>());
                    });
                
                MsgDispatcher.RegisterMsg(dt, _abilityId, true);
            }
        }
    }
}