using IMessage;
using System;
using System.Linq;
using System.Text;
using UnityEngine;
using GamePlay;

namespace BattleMap
{
    class MainBF_Enemy : Trigger
    {
        private BattleArea _battleArea;
        public MainBF_Enemy(MsgReceiver battleArea)
        {
            register = battleArea;
            //初始化响应时点,为战区状态改变
            msgName = (int)MessageType.RegionChange;
            //初始化条件函数和行为函数
            condition = Condition;
            action = Action;
            _battleArea = (BattleArea)battleArea;
        }

        private bool Condition()
        {
            
            return _battleArea._battleAreaSate == BattleAreaState.Player;
        }

        private void Action()
        {
            Debug.Log("You win!");
            MsgDispatcher.SendMsg((int)MessageType.WIN);
            Gameplay.Instance().roundProcessController.Win();
        }
    }
}
