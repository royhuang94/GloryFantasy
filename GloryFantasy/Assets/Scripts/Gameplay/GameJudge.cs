using System;
using GamePlay.Round;
using IMessage;
using System.Collections.Generic;
using GameUnit;
using UnityEngine;

namespace GamePlay
{
    public class GameJudge : UnitySingleton<GameJudge>, MsgReceiver
    {
        private Dictionary<int, List<Vector2>> _battleAreaDictionary;
        
        private void Start()
        {
            _battleAreaDictionary =
                BattleMap.BattleMap.Instance().battleArea.BattleAreaDic;
            
            MsgDispatcher.RegisterMsg(
                this.GetMsgReceiver(),
                (int)MessageType.Dead,
                NeedToDoJudge,
                CountEnemyControllingArea,
                "Game Judge CBA trigger"
            );
        }

        /// <summary>
        /// 用于判断是否需要进行judge的工作，若已处于结果状态则不需要再响应处理请求
        /// </summary>
        /// <returns>若已经在结果状态返回false，否则返回true</returns>
        public bool NeedToDoJudge()
        {
            if (Gameplay.Instance().roundProcessController.IsResultState())
                return false;
            return true;
        }

        /// <summary>
        /// 本函数本意是根据敌方控制战区数量情况确定胜负
        /// 若敌方控制所有战区，判定失败，敌方无控制战区，判定胜利
        /// </summary>
        public void CountEnemyControllingArea()
        {
            int enemyCountrolAreaAmount = 0;
            int allBattleAreaAmount = 0;
            
            foreach (int areaID in _battleAreaDictionary.Keys)
            {
                int enemyAmount = 0;
                int playerUnitAmount = 0;
                allBattleAreaAmount++;
                foreach (Vector2 pos in _battleAreaDictionary[areaID])
                {
                    OwnerEnum ownerEnum = BattleMap.BattleMap.Instance().GetMapblockBelong(pos);
                    if (ownerEnum == OwnerEnum.Player)
                    {
                        playerUnitAmount++;
                    } else if (ownerEnum == OwnerEnum.Enemy)
                    {
                        enemyAmount++;
                    }
                }
                
                // 若战区内敌我单位数量一致
                if (playerUnitAmount == enemyAmount)
                {
                    // 若都为0，那就是中立战区，不影响战区所属判定，继续下一个判断
                    if(enemyAmount == 0)
                        continue;
                    
                    // 若均不为0，说明处于争夺状态，则该战区属于敌方
                    enemyCountrolAreaAmount++;
                }
                // 我方单位数量比敌方少，该战区属于敌方单位
                else if (playerUnitAmount < enemyAmount)
                {
                    enemyCountrolAreaAmount++;
                }
                
                // 剩下的情况不影响结果，不检测了
                // if playerUnitAmount > enemyAmount
            }
            
            // 若敌方控制所有战区，则游戏失败，通知状态机
            if (allBattleAreaAmount == enemyCountrolAreaAmount)
            {
                Gameplay.Instance().roundProcessController.Lose();
                return;
            }
                
            // 若敌方无控制战区，则游戏胜利，通知状态机
            if(enemyCountrolAreaAmount == 0)
                Gameplay.Instance().roundProcessController.Win();
        }
        
        
        /// <summary>
        /// 仿照主程写的写的接口
        /// </summary>
        T MsgReceiver.GetUnit<T>()
        {
            return this as T;
        }
    }
}