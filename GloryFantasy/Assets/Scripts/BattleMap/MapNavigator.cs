using BattleMapManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MapManager
{

    public class MapNavigator
    {
        private static MapNavigator _instance;
        public static MapNavigator Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new MapNavigator();
                return _instance;
            }
        }


        public class NavigationData
        {
            //TODO 实现俩函数
            //1. GetEmptyNavigationData
            //2. Navigate

            //TODO 解决掉以上俩函数，实现移动
            public bool open = true;

            public int F;
            public int G;
            public int H;

            public BattleMapBlock thisBlock;
            public NavigationData preBlock;

            public NavigationData()
            {

            }

            public void Reset()
            {
                open = true;

                F = 0;
                G = 0;
                H = 0;

                //清空关联
                if (thisBlock != null)
                {
                    thisBlock.tempRef = null;
                    thisBlock = null;
                }

                preBlock = null;
            }

            //池
            private int curUsedIdx = 0;
            private List<NavigationData> navigationDataPool = null;

            private NavigationData GetEmptyNavigationData(BattleMapBlock _thisBlock, NavigationData _preBlock, int _G, int _H)
            {
                //优先从池子里取出
                NavigationData nd = null;
                if (curUsedIdx < navigationDataPool.Count)
                {
                    nd = navigationDataPool[curUsedIdx];
                }
                else
                {
                    nd = new NavigationData();
                    navigationDataPool.Add(nd);
                }

                ++curUsedIdx;

                nd.thisBlock = _thisBlock;
                nd.preBlock = _preBlock;
                nd.G = _G;
                nd.H = _H;
                nd.F = _G + _H;
                nd.open = true;
                nd.thisBlock.tempRef = nd;

                return nd;
            }

            private void ResetPool()
            {
                for (int i = 0; i < curUsedIdx; ++i)
                {
                    navigationDataPool[i].Reset();
                }
                curUsedIdx = 0;
            }

            public bool Navigate(
                BattleMapBlock from,
                BattleMapBlock to,
                List<BattleMapBlock> path,
                List<BattleMapBlock> searched = null,
                int mobility = -1,
                int stopDistance = 0)
            {
                if (BattleMapManager.BattleMapManager.getInstance() == null)
                    return false;

                if (path != null)
                    path.Clear();

                if (searched != null)
                    searched.Clear();

                //这种情况基本上也就不用寻路了吧...
                if (stopDistance <= 1 && from.Distance(to) > 1)
                    return false;

                //本来就在停止距离内
                if (from.Distance(to) <= stopDistance)
                    return true;

                int tryTimes = BattleMapManager.BattleMapManager.getInstance().BlockCount;

                //添加一个openPath 到list中
                List<NavigationData> opening = new List<NavigationData>();
                opening.Add(GetEmptyNavigationData(from, null, 0, from.Distance(to)));

                int retry = 0;
                bool catched = false;

                //当前探索方向
                int curDir = 0;
                //上次探索方向
                int lastDir = 0;
                //每次检测方向的次数
                int checkTimes = 0;

                //判断是否需要遍历open列表
                NavigationData gift = null;

                //距离最近的格子(接下来要移动的)
                NavigationData next_0 = null;
                //距离次近的格子
                NavigationData next_1 = null;

                int minStep = /*Mathf.Infinity*/ 99999;

                while (retry <= tryTimes && !catched)
                {
                    ++retry;
                    //从open中查找最近的节点
                    if (gift != null)
                    {
                        next_0 = next_1;
                        gift = null;
                    }
                    else if (next_1 != null)
                    {
                        next_0 = next_1;
                        next_1 = null;
                    }
                    else
                    {
                        minStep = 99999;
                        if (opening.Count == 0)
                            break;


                        for (int i = opening.Count - 1; i >= 0; --i)
                        {
                            if (!opening[i].open)
                            {
                                opening.RemoveAt(i);
                            }
                            else if (opening[i].F < minStep)
                            {
                                next_0 = opening[i];
                                minStep = next_0.F;
                            }
                            else if (next_1 == null && next_0 != null && opening[i].F == next_0.F)
                            {
                                next_1 = opening[i];
                            }
                        }
                    }

                    //标志为已关闭
                    next_0.open = false;

                    //放入已搜索中
                    if (searched != null)
                    {
                        searched.Add(next_0.thisBlock);
                    }

                    checkTimes = 4;  //此处因为是四边形所以会有4次检测
                    curDir = lastDir;
                    //遍历最近节点的周围6个节点，依次放入close中
                    //TODO 实现NavigationPassable get set构造器
                    //int roads = next_0.thisBlock.NavigationPassable ? 63 : next_0.thisGrid.roadPasses;

                    while (checkTimes > 0)
                    {
                        //沿着当前探索方向继续探索


                    }


                }

                return false;
            }




        }
    }
}

