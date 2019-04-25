using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BattleMap
{
    /// <summary>
    /// 寻路结点
    /// </summary>
    public class Node
    {
        /// <summary>
        /// 该点的坐标
        /// </summary>
        public Vector2 position;
        /// <summary>
        /// 该点上一个点的坐标
        /// </summary>
        public Vector2 parentPosition;
        /// <summary>
        /// 从起点走到该点的代价
        /// </summary>
        public int G;
        /// <summary>
        /// 从该点走到终点的理想预测代价
        /// </summary>
        public int H;

        public Node(Vector2 _position, Vector2 _destination , int _G = int.MaxValue / 2)
        {
            position = _position;
            parentPosition = _position;
            this.H = (int)Vector2.Distance(_position, _destination);
            this.G = _G;
        }

        public Node(Vector2 _position, Vector2 _fatherPosition, Vector2 _destination, int _G = int.MaxValue / 2)
        {
            position = _position;
            parentPosition = _fatherPosition;
            this.H = (int)Vector2.Distance(_position, _destination);
            this.G = _G;
        }
    }

    public class MapNavigator
    {
        //这里有什么必要用单例？让BattleMap持有引用不就行了？
        //private static MapNavigator _instance;
        //public static MapNavigator _Instantce
        //{
        //    get
        //    {
        //        if (_instance == null)
        //            _instance = new MapNavigator();
        //        return _instance;
        //    }
        //}

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

        public Dictionary<Vector2, Node> openList;
        public Dictionary<Vector2, Node> closeList;

        public Vector2 globalEndPos;

        //TODO 用于保存最优路径
        /// <summary>
        /// 路径
        /// </summary>
        public List<BattleMapBlock> paths = new List<BattleMapBlock>();

        //寻路入口
        public bool PathSearch(Vector2 startPos, Vector2 endPos)
        {
            //单位起点与终点 是否包含在字典内
            //为什么要用这个字典?
            //if (!BattleMap.Instance().mapBlockDict.ContainsKey(startPos) || !BattleMap.Instance().mapBlockDict.ContainsKey(endPos))
            //{
            //    Debug.Log("invalid para");
            //    return false;
            //}

            if (BattleMap.Instance().GetSpecificMapBlock((int)startPos.x, (int)startPos.y) == null || BattleMap.Instance().GetSpecificMapBlock((int)endPos.x, (int)endPos.y) == null)
            {
                Debug.Log("In MapNavigator: invalid Pos");
                return false;
            }

            //初始化openList和closeList
            openList = new Dictionary<Vector2, Node>();
            closeList = new Dictionary<Vector2, Node>();

            //算法开始

            ////起点为A
            //BattleMapBlock A = BattleMap.Instance().GetSpecificMapBlock((int)startPos.x, (int)startPos.y);
            //A.G = 0;
            //A.H = Mathf.Abs(globalEndPos.x - startPos.x) + Mathf.Abs(globalEndPos.y - startPos.y);  //Vector2.Distance(A.pos, globalEndPos);
            //A.F = A.G + A.H;
            //A.parentBlock = null; //父地图块儿设置为null，因为此处是起点

            //加入起点
            openList.Add(startPos, new Node(startPos, endPos, 0));

            ////此时起点A已经设置完毕，可以不用再考虑此地图块儿了
            //CloseList.Add(A);
            //A.aStarState = AStarState.isInCloseList;

            do
            {
                //遍历OpenList，寻找F值最小的节点，设为A
                Node A = new Node(startPos, endPos, int.MaxValue / 2);
                foreach (Node node in openList.Values)
                {
                    if (node.H + node.G < A.H + A.G)
                        A = node;
                }

                AStarSearch(A, startPos, endPos);
                #region 自己看懂了删掉
                //if (path != null)
                //{
                //    //do
                //    //{
                //    //    //设置为AStarPath
                //    //    //会不会出现问题，此处修改了EMapBlockType，后面遍历时，会不会出问题
                //    //    path.blockType = EMapBlockType.aStarPath;
                //    //    //不懂为什么要放在BattleMap里
                //    //    //BattleMap.Instance().aStarPath.Add(path);
                //    //    paths.Add(path);
                //    //    //Debug.Log("aStarPath " + path.GetSelfPosition());

                //    //    if (path.parentBlock == null)
                //    //        path = null;
                //    //    else
                //    //        path = path.parentBlock;
                //    //} while (path != null);

                //    RestIsInOpenListBlock();

                //    //TODO 测试移动
                //    var mov = BattleMap.Instance().GetUnitsOnMapBlock(startPos).UnitAttribute.Mov;
                //    if (paths.Count - 1 > mov)
                //    {
                //        GFGame.UtilityHelper.Log("超出移动力范围，当前移动力" + mov, GFGame.LogColor.RED);
                //        Debug.Log((paths.Count - 1));
                //        return false;
                //    }
                //    return true;
                //}
                #endregion
                openList.Remove(A.position);
                closeList.Add(A.position, A);

                //如果找到了endPos
                //代码自己补

                //OpenList是否还有节点
            } while (OpenList.Count > 0);

            return false;
        }

        //查找周边方块
        private void AStarSearch(Node A, Vector2 startPos, Vector2 endPos)
        {
            //获得A的周边MapBlock
            List<BattleMapBlock> neighbourBlock = BattleMap.Instance().GetNeighbourBlock(A);
            //将MapBlock转为Node格式
            List<Node> neighourNode = new List<Node>();
            foreach (BattleMapBlock mapBlock in neighbourBlock)
            {
                neighourNode.Add(new Node(mapBlock.position, A.position, endPos, A.G + 1));
            }
            //遍历A的周边Node
            for (int i = 0; i < neighourNode.Count; i++)
            {
                Node B = neighourNode[i];
                //如果在关闭列表中直接忽略跳到下一个
                if (closeList.ContainsKey(B.position))
                    continue;
                //如果在开放列表中
                if (openList.ContainsKey(B.position))
                {
                    //A到B的G值+A.G>B.G
                    //TODO 未加入H，会不会对最优结果产生影响
                    //废话，当然不会，还有这个distance是什么鬼，是这么用的吗我的天
                    //float curG = Vector2.Distance(A.GetSelfPosition(), B.GetSelfPosition());
                    //if (B.G > curG + A.G)
                    //{
                    //    //更新B的父节点为A，并相应更新B.G,B.H
                    //    B.parentBlock = A;
                    //    B.G = curG + A.G;
                    //    B.F = B.H + B.G;
                    //}
                    if (B.G > A.G + 1)
                    {
                        B.parentPosition = A.position;
                        B.G = A.G + 1;
                    }
                    continue;
                }
                //如果不在两个列表里
                else if (!openList.ContainsKey(B.position))
                {
                    //更新B的父节点为A，并相应更新B.G; 计算B.F,B.H; B加入OpenList
                    //B.parentBlock = A;
                    //B.G = Vector2.Distance(A.GetSelfPosition(), B.GetSelfPosition()) + A.G;
                    //B.H = Mathf.Abs(B.GetSelfPosition().x - globalEndPos.x) + Mathf.Abs(B.GetSelfPosition().y - globalEndPos.y);
                    //B.F = B.G + B.H;
                    //OpenList.Add(B);
                    //B.aStarState = AStarState.isInOpenList;
                    openList.Add(B.position, B);

                    //当B.H接近0时
                    //if (B.H < Mathf.Epsilon)
                    //    //B的所有父节点既是路径
                    //    return B;
                    //else
                        //继续遍历
                        continue;
                }
            }
        }

        //地图具象化（并对Openlist，Closelist内节点上色）
        public void InstantiateMapBeta(BattleMapBlock[,] mapBlocks)
        {

        }



        //TODO 待优化
        //重置Block的aStarState
        //public void RestIsInCloseListBlock()
        //{
        //    if(CloseList != null)
        //    {
        //        foreach (var block in CloseList)
        //        {
        //            block.aStarState = AStarState.free;
        //            //Debug.Log(block.aStarState);
        //        }

        //    }

        //}

        //public void RestIsInOpenListBlock()
        //{
        //    if (OpenList != null)
        //    {
        //        foreach (var block in OpenList)
        //        {
        //            block.aStarState = AStarState.free;
        //        }
        //    }
        //}
    }

}



