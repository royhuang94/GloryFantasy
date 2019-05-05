using GamePlay.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unit = GameUnit.GameUnit;

namespace AI
{
    public class AutoController
    {
        //TODO 需要写啥基类吗？
        //a. 比如，远程/魔法 ai 与近战 ai 不同？ 相同的类处理
        //b. 他们攻击方式，移动方式不同？
        //TODO 在这个函数进行监听消息？
        //TODO 在这个函数进行AutoAction -> AutoSelect -> AutoATK ?

        //目标单位
        private Unit targetBattleUnit;
        //仇恨列表记录器
        public HatredRecorder hatredRecorder = new HatredRecorder();
        //移动到目标的路径
        private List<Vector2> toTargetPath = new List<Vector2>();
        //路径长度
        private int PathCount
        {
            get
            {
                return toTargetPath.Count;
            }
        }
        //起点
        private Vector2 StartPos
        {
            get
            {
                return toTargetPath[PathCount - 1];
            }
        }       
        //终点
        private Vector2 EndPos
        {
            get
            {
                return toTargetPath[0];
            }
        }
        //技能相关
        //获取技能的停止移动距离
        private int SkillStopDistance
        {
            get
            {
                return -1; //目前还未使用异能
            }
        }

        /// <summary>
        /// 自动行动
        /// </summary>
        /// <param name="battleUnit"></param>
        private void AutoAction(Unit battleUnit)
        {
            //自动选取目标
            AutoSelectTarget(battleUnit);

            //找不到目标单位
            if (targetBattleUnit != null && battleUnit == null)
            {
                return;
            }

            UnitMoveCommand unitMove;
            //需要移动
            if (battleUnit != null && PathCount > 0)
            {
                unitMove = new UnitMoveCommand(battleUnit, StartPos, EndPos, Vector2.zero);
                unitMove.Excute(); //已经判断过距离
            }

            //自动攻击(搓招)
            AutoUseAtk(battleUnit);

        }

        /// <summary>
        /// 自动选择目标
        /// </summary>
        /// <param name="battleUnitAction"></param>
        private void AutoSelectTarget(Unit battleUnit)
        {
            int stopDistance = AtkStopDistance(battleUnit);
            //从仇恨列表中确定目标
            Unit hatredUnit = null;
            //地图导航
            BattleMap.MapNavigator mapNavigator = BattleMap.BattleMap.Instance().MapNavigator;

            for (int i = 0; i < hatredRecorder.HatredCount; i++)
            {
                hatredUnit = hatredRecorder.GetHatredByIndex(i, i == 0);
                if(hatredUnit.IsDead())
                {
                    //已经排序过，且无法找到还能够行动的单位，就表示场上没有存活的敌方单位了
                    hatredUnit = null;
                    break;
                }

                //判断这个单位是否可以到达
                bool catched = false;

                //如果这个单位就在攻击范围内，即身边
                if (Distance(battleUnit, hatredUnit) <= stopDistance)
                {
                    toTargetPath.Clear();
                    catched = true;
                }
                else
                {
                    catched = mapNavigator.PathSearch(battleUnit.CurPos, hatredUnit.CurPos);
                }

                //寻路不可达
                if(!catched)
                {
                    hatredUnit = null;
                    continue;
                }
                else //找到了
                {
                    toTargetPath = mapNavigator.Paths;
                    break;
                }
            }

            //没有目标
            if (hatredUnit == null)
            {
                targetBattleUnit = null;
                return;
            }

            if(battleUnit != null && !hatredUnit.Equals(targetBattleUnit))
            {
                targetBattleUnit = hatredUnit;
            }

        }

        /// <summary>
        /// 自动攻击
        /// </summary>
        private void AutoUseAtk(Unit battleUnit)
        {
            //TODO 异能引入后进行修改

            //异能为引入前版本
            //获取攻击者和被攻击者
            GameUnit.GameUnit Attacker = battleUnit;
            GameUnit.GameUnit AttackedUnit = targetBattleUnit;
            //创建攻击指令
            UnitAttackCommand unitAtk = new UnitAttackCommand(Attacker, AttackedUnit);

            unitAtk.Excute();//已经判断过距离，放心攻击
        }

        /// <summary>
        /// 获取攻击时的停止距离，近战，远程不同
        /// </summary>
        /// <returns>攻击范围</returns>
        private int AtkStopDistance(Unit battleUnit)
        {
            return battleUnit.rng;
        }

        private int Distance(Unit unit1, Unit unit2)
        {
            return Mathf.Abs((int)unit1.CurPos.x - (int)unit2.CurPos.x) + Mathf.Abs((int)unit1.CurPos.y - (int)unit2.CurPos.y);
        }
    }

}

