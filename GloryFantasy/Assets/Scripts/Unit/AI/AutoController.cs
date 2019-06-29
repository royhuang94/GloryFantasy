﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unit = GameUnit.GameUnit;

namespace AI
{
    public class AutoController
    {
        //管理所有的ai的controllers
        public List<SingleController> singleControllers = new List<SingleController>();

        public void AddAIController(Unit unit)
        {
            if (unit != null)
            {
                SingleController controller;
                //初始化AI控制器与携带的仇恨列表
                if (BattleMap.BattleMap.Instance().UnitsList.Count % 2 != 0)
                    controller = new SingleAutoControllerAtker(unit); //无脑型
                else
                    controller = new SingleAutoControllerDefender(unit);//防守型
                controller.hatredRecorder.Reset(unit);
                GamePlay.Gameplay.Instance().autoController.singleControllers.Add(controller);
            }
        }

        /// <summary>
        /// 更新所有Controller的仇恨列表
        /// </summary>
        /// <param name="enemyUnit"></param>
        /// <param name="enemyUnits"></param>
        public void UpdateAllHatredList(Unit enemyUnit = null, List<Unit> enemyUnits = null)
        {
            if (singleControllers.Count <= 0)
                return;

            foreach (SingleController controller in singleControllers)
            {
                if (enemyUnit != null)
                {
                    controller.hatredRecorder.AddHatred(enemyUnit);
                    continue;
                }
                else if(enemyUnits != null)
                    controller.hatredRecorder.AddHatredUnits(enemyUnits);

                //Debug.Log("仇恨列表数量: " + controller.hatredRecorder.HatredCount);
            }

        }

        /// <summary>
        /// 通过ID
        /// 获取对应的singleController
        /// </summary>
        /// <param name="id">想要获取单位的controller的ID</param>
        /// <returns></returns>
        public SingleController GetSingleControllerByID(Vector2 pos)
        {
            foreach (SingleController controller in singleControllers)
            {
                if (controller.BattleUnit.CurPos == pos)
                    return controller;
            }
            return null;
        }


        /// <summary>
        ///记录仇恨列表
        ///对仇恨值做加法
        /// </summary>
        /// <param name="enemyUnit"></param>
        /// <param name="hostUnit"></param>
        public void RecordedHatred(Unit enemyUnit, Unit hostUnit)
        {
            //当敌方单位是玩家单位时，触发更新
            if(enemyUnit.owner == GameUnit.OwnerEnum.Player && hostUnit.owner == GameUnit.OwnerEnum.Enemy)
            {
                GetSingleControllerByID(hostUnit.CurPos).hatredRecorder.RecordedHatred(enemyUnit.id, enemyUnit.getATK());
            }
        }
    }
}

