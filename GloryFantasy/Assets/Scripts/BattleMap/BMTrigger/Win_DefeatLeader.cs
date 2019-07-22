using IMessage;
using System;
using System.Linq;
using System.Text;
using UnityEngine;
using GamePlay.Encounter;
using GamePlay;

namespace BattleMap
{
    class Win_DefeatLeader : Trigger
    {
        private BattleArea _battleArea;
        public Win_DefeatLeader()
        {
            //初始化响应时点,为战区状态改变
            msgName = (int)MessageType.Dead;
            //初始化条件函数和行为函数
            condition = Condition;
            action = Action;
        }

        private bool Condition()
        {
            bool c = true;
            //foreach(GameUnit.GameUnit unit in Player.Instance().leaders)

            return false;
        }

        private void Action()
        {
            Debug.Log("You win!");
            Gameplay.Instance().roundProcessController.Win();
        }
    }
}
