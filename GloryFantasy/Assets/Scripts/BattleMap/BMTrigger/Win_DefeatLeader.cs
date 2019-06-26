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
            //如果死掉的单位在地方指挥官列表中，即胜利
            if (EncouterData.Instance().dataOfThisBattle.GetLeaders().Contains(this.GetDead()))
                return true;

            return false;
        }

        private void Action()
        {
            Debug.Log("You win!");
            MsgDispatcher.SendMsg((int)MessageType.WIN);
            Gameplay.Instance().roundProcessController.Win();
        }
    }
}
