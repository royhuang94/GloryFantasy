using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unit = GameUnit.GameUnit;

namespace AI
{
    //战斗状态
    public enum BattleState
    {
        Prepare,        //准备中
        Ready,          //准备就绪
        Fighting,       //战斗中
        WaitForPlayer,  //等待玩家
        End,            //战斗结束
        Exception,      //战斗状态异常
    }

    public class BattleField
    {
        public BattleState battleState = BattleState.Prepare;

        /// <summary>
        /// 准备战斗
        /// </summary>
        private void Prepare()
        {
            battleState = BattleState.Ready;
            AIBattleController.Instance().PlayBattle(AIBattleController.Instance().AIFightConditon);
        }

        /// <summary>
        /// 开始战斗
        /// </summary>
        private void Fight()
        {
            battleState = BattleState.Fighting;
            AIBattleController.Instance().PlayBattle(AIBattleController.Instance().AIFightConditon);
        }

        /// <summary>
        /// 运行
        /// </summary>
        public void Run()
        {
            switch (battleState)
            {
                case BattleState.Prepare:
                    Prepare();
                    break;

                case BattleState.Ready:
                    Fight();
                    break;

                case BattleState.Fighting:
                    Fight();
                    break;

                case BattleState.WaitForPlayer:
                    break;

                case BattleState.End:
                    break;

                case BattleState.Exception:
                    break;

                default:
                    break;
            }
        }
    }
}


