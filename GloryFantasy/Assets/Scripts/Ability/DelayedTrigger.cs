using System;
using IMessage;
using Action = IMessage.Action;

namespace Ability
{
    /// <summary>
    /// 通过回合计数的触发器，应该只触发一次
    /// 用于指定回合后进行指定的action操作
    /// </summary>
    public class DelayedTrigger : Trigger
    {
        private int _currentRound;
        private int _rounds;
        
        public DelayedTrigger(MsgReceiver speller, int rounds, int messageType, Action _action)
        {
            register = speller;
            msgName = messageType;
            
            _currentRound = GamePlay.Gameplay.Instance().roundProcessController.State.roundCounter;

            _rounds = _currentRound + rounds;
            
            condition = Condition;
            action = _action;
        }

        private bool Condition()
        {
            return _currentRound == _rounds;
        }
    }
}