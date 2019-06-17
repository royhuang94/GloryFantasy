 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unit = GameUnit.GameUnit;

namespace AI
{
    public class AIBattleController
      : UnitySingleton<AIBattleController>
    {

        public IEnumerator PlayBattleByCoroutine(System.Action callback)
        {
            foreach (Unit unit in BattleMap.BattleMap.Instance().UnitsList)
            {
                if (unit.owner != GameUnit.OwnerEnum.Enemy)
                    yield return null; //TODO 调用人物行为对应的函数
            }

            if (callback != null)
                callback();

            yield return null;
        }

        /// <summary>
        /// 用于启动同步协程Fight
        /// </summary>
        internal void AIFightConditon()
        {
            StartCoroutine(Fight());
        }
        /// <summary>
        /// 开始战斗
        /// </summary>
        private IEnumerator Fight()
        {
            GamePlay.Gameplay.Instance().singleBattle.battleState = BattleState.Fighting;
            foreach (Unit unit in BattleMap.BattleMap.Instance().UnitsList)
            {
                //只获取敌人
                if (unit.owner != GameUnit.OwnerEnum.Enemy && GamePlay.Gameplay.Instance().singleBattle.battleState == BattleState.End)
                    break;
                if (!unit.IsDead())
                {
                    AI.SingleController controller = GamePlay.Gameplay.Instance().autoController.GetSingleControllerByID(unit.CurPos);
                    if (controller != null)
                    {
                        GameUnit.HeroActionState state = controller.AutoAction();

                        //TODO 状态切换
                        //目前没有切换，之后添加
                        switch (state)
                        {
                            case GameUnit.HeroActionState.WaitForPlayerChoose:
                                GamePlay.Gameplay.Instance().singleBattle.battleState = BattleState.WaitForPlayer;
                                break;
                            case GameUnit.HeroActionState.BattleEnd:
                                GamePlay.Gameplay.Instance().singleBattle.battleState = BattleState.End;
                                break;
                            case GameUnit.HeroActionState.Error:
                                GamePlay.Gameplay.Instance().singleBattle.battleState = BattleState.Exception;
                                break;
                            case GameUnit.HeroActionState.Warn:
                                GamePlay.Gameplay.Instance().singleBattle.battleState = BattleState.Exception;
                                break;
                            default:
                                break;
                        }
                    }
                }

                yield return new WaitForSeconds(0.8f);
            }

            //AI回合结束，切换回恢复阶段
            yield return StartCoroutine(GamePlay.Gameplay.Instance().SwitchPhaseHandler());  
        }

        //播放战斗(异步的方式)
        public void PlayBattle(System.Action callback)
        {
            StartCoroutine(PlayBattleByCoroutine(callback));
        }
    }
}



