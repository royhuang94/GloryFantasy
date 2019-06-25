using GamePlay;
using GamePlay.Input;
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
        public override void Init(string abilityId)
        {
            base.Init(abilityId);
            MsgDispatcher.RegisterMsg(
                new THeal(this.GetUnitReceiver(this), AbilityVariable.Curing.Value, this),
                abilityId,
                true);
        }
    }

    public class THeal : Trigger
    {
        private int _curing;
        private bool _ifHeal;
        private Ability _srcAbility;
        
        public THeal(MsgReceiver speller, int curing, Ability srcAbility)
        {
            _curing = curing;
            _srcAbility = srcAbility;
            register = speller;
            msgName = (int) MessageType.Summon;
            condition = Condition;
            action = Action;
            _ifHeal = false;
        }

        private bool Condition()
        {
            // 应该没有啥条件限制了，只执行一次
            return true;
        }

        private void Action()
        {
            // 如果是被Summon消息触发
            if (!_ifHeal)
            {
                // 改变当前trigger的响应方式
                msgName = (int) MessageType.SelectionOver;
                // 修改状态以改变下次响应的行为
                _ifHeal = true;
                // 重新注册，保持单次执行
                MsgDispatcher.RegisterMsg(this, "Heal subMsgSender", true);
                
                // 调整输入状态到选择模式
                Gameplay.Instance().gamePlayInput.OnEnterSelectState(_srcAbility);
                
                // 直接结束执行
                return;
            }
            
            // 以下是被SelectionOver消息触发后的动作
            GameUnit.GameUnit gameUnit = (GameUnit.GameUnit) this.GetSelectingUnits()[0];
            if (gameUnit == null)
            {
                Debug.Log("要治愈的单位指定错误，请检查");
                return;
            }
            
            AbilityMediator.Instance().RecoverUnitsHp(gameUnit, _curing);
        }
    }
}