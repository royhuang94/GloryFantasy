using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BattleMap
{
    public class MapNavigator
    {
        private static MapNavigator _instance;
        public static MapNavigator _Instantce
        {
            get
            {
                if (_instance == null)
                    _instance = new MapNavigator();
                return _instance;
            }
        }

        //TODO AStarPath
        //1. 设置 BlockType 枚举
        //moveable， bar， boundary， aStarPath
        //2. 设置 AStarState 枚举
        //free， isInOpenList， isInCloseList
        //3. MapBlock
        //字段： BlockType、AStarState、neighbourBlock、parentBlock、F、G、H
        //4. List<MapBlock> OpenList //可探索的Block

        //5 List<MapBlock> CloseList  //已选过的Block

        //6. Vector2 globalEndPos

        //7. bool PathSearch(Vector2 startPos,Vector2 endPos)

        //8  MapBlock AStarSearch(MapBlock A)

        //9 InstantiateMapBeta //地图具象化(颜色改变)


        //TODO 放在什么地方调用这个函数

        public List<BattleMapBlock> OpenList; //所有被考虑来寻找最短路径的地图块儿
        public List<BattleMapBlock> CloseList; //不会再被考虑的地图块儿
        public Vector2 globalEndPos;


        //寻路入口
        public bool PathSearch(Vector2 startPos, Vector2 endPos)
        {
            //单位起点与终点 是否包含在字典内
            if (!BattleMap.getInstance().mapBlockDict.ContainsKey(startPos) || !BattleMap.getInstance().mapBlockDict.ContainsKey(endPos))
            {
                Debug.Log("invalid para");
                return false;
            }

            //为OpenList && CloseList 初始化空间
            OpenList = new List<BattleMapBlock>();
            CloseList = new List<BattleMapBlock>();
            globalEndPos = endPos;
            //算法开始
            //起点为A
            BattleMapBlock A = BattleMap.getInstance().GetSpecificMapBlock((int)startPos.x, (int)startPos.y);
            A.G = 0;
            A.H = Mathf.Abs(globalEndPos.x - startPos.x) + Mathf.Abs(globalEndPos.y - startPos.y);  //Vector2.Distance(A.pos, globalEndPos);
            A.F = A.G + A.H;
            A.parentBlock = null; //父地图块儿设置为null，因为此处是起点

            //此时起点A已经设置完毕，可以不用再考虑此地图块儿了
            CloseList.Add(A);
            A.aStarState = AStarState.isInCloseList;
            do
            {
                //遍历OpenList，寻找F值最小的节点，设为A
                if (OpenList.Count > 0)
                    A = OpenList[0];
                for (int i = 0; i < OpenList.Count; i++)
                {
                    //冒泡
                    if (OpenList[i].F < A.F)
                        A = OpenList[i];
                }

                BattleMapBlock path = AStarSearch(A);
                if (path != null)
                {
                    Debug.Log("Path Found");
                    do
                    {
                        //设置为AStarPath
                        //TODO 会不会出现问题，此处修改了EMapBlockType，后面遍历时，会不会出问题
                        path.blockType = EMapBlockType.aStarPath;
                        BattleMap.getInstance().aStarPath.Add(path);
                        Debug.Log("aStarPath " + path.GetSelfPosition());
                        path.GetComponent<Image>().color = Color.blue;
                        if (path.parentBlock == null)
                            path = null;
                        else
                            path = path.parentBlock;
                    } while (path != null);

                    return true;
                }
                OpenList.Remove(A);
                CloseList.Add(A);
                A.aStarState = AStarState.isInCloseList;
                //OpenList是否还有节点
            } while (OpenList.Count > 0);

            Debug.Log("Path Not Found");
            return false;
        }

        //寻找最优Block
        private BattleMapBlock AStarSearch(BattleMapBlock A)
        {
            //遍历A的周边节点，当前处理地图块儿为B
            BattleMapBlock B;
            for (int i = 0; i < A.neighbourBlock.Length; i++)
            {
                if (A.neighbourBlock[i] == null)
                    continue;
                B = A.neighbourBlock[i];
                //是否在开放列表中
                if (B.aStarState == AStarState.isInOpenList)
                {
                    //A到B的G值+A.G>B.G
                    //TODO 未加入H，会不会对最优结果产生影响
                    float curG = Vector2.Distance(A.GetSelfPosition(), B.GetSelfPosition());
                    if (B.G > curG + A.G)
                    {
                        //更新B的父节点为A，并相应更新B.G,B.H
                        B.parentBlock = A;
                        B.G = curG + A.G;
                        B.F = B.H + B.G;
                    }
                    continue;
                }
                else if (B.aStarState == AStarState.free)
                {
                    //更新B的父节点为A，并相应更新B.G; 计算B.F,B.H; B加入OpenList
                    B.parentBlock = A;
                    B.G = Vector2.Distance(A.GetSelfPosition(), B.GetSelfPosition()) + A.G;
                    B.H = Mathf.Abs(B.GetSelfPosition().x - globalEndPos.x) + Mathf.Abs(B.GetSelfPosition().y - globalEndPos.y);
                    B.F = B.G + B.H;
                    OpenList.Add(B);
                    B.aStarState = AStarState.isInOpenList;

                    //当B.H接近0时
                    if (B.H < Mathf.Epsilon)
                        //B的所有父节点既是路径
                        return B;
                    else
                        //继续遍历
                        continue;
                }
                else
                    continue;
            }
            return null;
        }

        //地图具象化（并对Openlist，Closelist内节点上色）
        public void InstantiateMapBeta(BattleMapBlock[,] mapBlocks)
        {

        }
    }

}



