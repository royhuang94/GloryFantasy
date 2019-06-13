using IMessage;
using System;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BattleMap
{
    class MainBF_Friendly : Trigger
    {
        private BattleArea _battleArea;
        public MainBF_Friendly(MsgReceiver battleArea)
        {
            register = battleArea;
            //初始化响应时点,为战区状态改变
            msgName = (int)MessageType.AfterColliderChange;
            //初始化条件函数和行为函数
            condition = Condition;
            action = Action;
            _battleArea = (BattleArea) battleArea;
        }

        private bool Condition()
        {
            return _battleArea._battleAreaSate == BattleAreaState.Enmey;
        }

        private void Action()
        {
            Debug.Log("You lose!");
            MsgDispatcher.SendMsg((int)MessageType.LOSE);
        }
    }
}
