using GamePlay;
using GameUnit;
using IMessage;
using Mediator;
using UnityEngine;

namespace Ability
{
    /// <summary>
    /// 此单位部署时，其治疗同一战区内的目标友方单位{Curing}点生命。
    /// </summary>
    public class Heal : Ability
    {
        private Trigger _trigger;

        public override void Init(string abilityId)
        {
            base.Init(abilityId);
            _trigger = new THeal(
                this.GetUnitReceiver(this), 
                AbilityVariable.Curing.Value, 
                gameObject.GetComponent<GameUnit.GameUnit>().CurPos);
            MsgDispatcher.RegisterMsg(_trigger, abilityId, true);
        }
    }

    public class THeal : Trigger
    {
        private int _curing;
        private Vector2 _currentPos;

        public THeal(MsgReceiver speller, int curing, Vector2 currentPos)
        {
            _curing = curing;
            _currentPos = currentPos;

            register = speller;
            msgName = (int) MessageType.Summon;
            condition = Condition;
            action = Action;
        }

        private bool Condition()
        {
            // 应该没有啥条件限制了，只执行一次
            return true;
        }

        private void Action()
        {
            foreach (GameUnit.GameUnit gameUnit in AbilityMediator.Instance().GetGameUnitsInBattleArea(_currentPos))
            {
                if(gameUnit.owner == OwnerEnum.Player)
                    AbilityMediator.Instance().RecoverUnitsHp(gameUnit, _curing);
            }
        }
    }
}