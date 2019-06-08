﻿using IMessage;
using System;
using System.Linq;
using System.Text;
using UnityEngine;
using GamePlay.Encounter;

namespace BattleMap
{
    class Failed_DeathPage : Trigger
    {
        private BattleArea _battleArea;
        public Failed_DeathPage()
        {
            //初始化响应时点,为战区状态改变
            msgName = (int)MessageType.DeathPageIncrease;
            //初始化条件函数和行为函数
            condition = Condition;
            action = Action;
        }

        private bool Condition()
        {
            //if (BattleMap.Instance().encounter.Deathpage >=  BattleMap.Instance().Deathpage)
                return true;

            return false;
        }

        private void Action()
        {
            Debug.Log("You lose!");
            MsgDispatcher.SendMsg((int)MessageType.LOSE);
        }
    }
}