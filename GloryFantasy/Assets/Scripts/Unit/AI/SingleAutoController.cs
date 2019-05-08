using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unit = GameUnit.GameUnit;
using GamePlay.Input;
using BattleMap;

namespace AI
{
    public class SingleController
    {

        public SingleController(Unit _gameUnit)
        {
            battleUnit = _gameUnit;
            targetBattleUnit = null;
            hatredRecorder = new HatredRecorder();
            toTargetPath = new List<Vector2>();
        }
        //所控制棋子
        private Unit battleUnit;
        public Unit BattleUnit
        {
            get
            {
                return battleUnit;
            }
        }

        //目标单位
        private Unit targetBattleUnit;
        //仇恨列表记录器
        public HatredRecorder hatredRecorder;
        //移动到目标的路径
        private List<Vector2> toTargetPath;

        /// <summary>
        /// 路径长度
        /// </summary>
        private int PathCount
        {
            get
            {
                return toTargetPath.Count;
            }
        }

        /// <summary>
        /// 起点
        /// </summary>
        private Vector2 StartPos
        {
            get
            {
                return toTargetPath[PathCount - 1];
            }
        }

        /// <summary>
        /// 终点
        /// </summary>
        private Vector2 EndPos
        {
            get
            {
                return toTargetPath[0];
            }
        }

        /// <summary>
        ///技能相关
        ///获取技能的停止移动距离
        /// </summary>
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
        public GameUnit.HeroActionState AutoAction()
        {
            //自动选取目标
            AutoSelectTarget();

            //找不到目标单位
            if (targetBattleUnit == null && battleUnit != null)
            {
                return GameUnit.HeroActionState.Warn; ;
            }

            UnitMoveAICommand unitMove;
            //需要移动
            if (battleUnit != null && PathCount > 0)
            {
                unitMove = new UnitMoveAICommand(battleUnit, toTargetPath, AutoUseAtk);
                Debug.Log("AI StartPos: " + StartPos + " EndPos: " + EndPos);
                unitMove.Excute(); //已经判断过距离
            }

            //TODO 战斗结束
            if(false)
                return GameUnit.HeroActionState.BattleEnd;
            else
                return GameUnit.HeroActionState.Normal;

        }

        /// <summary>
        /// 自动选择目标
        /// </summary>
        /// <param name="battleUnitAction"></param>
        private void AutoSelectTarget()
        {
            int stopDistance = AtkStopDistance();
            //从仇恨列表中确定目标
            Unit hatredUnit = null;
            //地图导航
            BattleMap.MapNavigator mapNavigator = BattleMap.BattleMap.Instance().MapNavigator;

            for (int i = 0; i < hatredRecorder.HatredCount; i++)
            {
                hatredUnit = hatredRecorder.GetHatredByIndex(i, i == 0);
                if (hatredUnit.IsDead())
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
                    targetBattleUnit = hatredUnit;
                    AutoUseAtk();
                    catched = true;
                }
                else
                {
                    //if (catched = mapNavigator.PathSearch(battleUnit.CurPos, new Vector2(hatredUnit.CurPos.x, hatredUnit.CurPos.y + 1)))
                    //    toTargetPath = mapNavigator.Paths;
                    //TODO 把被仇恨单位作为起点
                    //遍历4个相邻地图块儿，把对于当前单位最近的地图块儿作为终点
                    Node nodeStart = new Node(hatredUnit.CurPos, hatredUnit.CurPos);
                    //获得A的周边MapBlock
                    List<BattleMapBlock> neighbourBlock = BattleMap.BattleMap.Instance().GetNeighbourBlock(nodeStart);
                    int prevPathCount = int.MaxValue;
                    BattleMapBlock preBattleMapBlock = null;
                    foreach (BattleMapBlock battleMapBlock in neighbourBlock)
                    {
                        if (mapNavigator.PathSearch(battleUnit.CurPos, battleMapBlock.position))
                        {
                            //找到对于ai单位的最短路径
                            if (prevPathCount > mapNavigator.Paths.Count)
                            {
                                toTargetPath = mapNavigator.Paths;
                                /* prevPathCount = mapNavigator.Paths.Count*/
                                
                                if (preBattleMapBlock != null)
                                    preBattleMapBlock.RemoveUnit(battleUnit);
                                battleMapBlock.units_on_me.Add(battleUnit);
                                preBattleMapBlock = battleMapBlock;
                                catched = true;
                            }
                        }   
                    }

                }

                //寻路不可达
                if (!catched)
                {
                    hatredUnit = null;
                    continue;
                }
                else //找到了
                {
                    break;
                }
            }

            //没有目标
            if (hatredUnit == null)
            {
                targetBattleUnit = null;
                return;
            }

            if (battleUnit != null && !hatredUnit.Equals(targetBattleUnit))
            {
                targetBattleUnit = hatredUnit;
            }

        }

        /// <summary>
        /// 自动攻击
        /// </summary>
        private void AutoUseAtk()
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
        private int AtkStopDistance()
        {
            return battleUnit.rng;
        }

        private int Distance(Unit unit1, Unit unit2)
        {
            return Mathf.Abs((int)unit1.CurPos.x - (int)unit2.CurPos.x) + Mathf.Abs((int)unit1.CurPos.y - (int)unit2.CurPos.y);
        }
    }
}


