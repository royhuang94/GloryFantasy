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

        //赋值时，position == parentPosition 是起点？
        public Node(Vector2 _position, Vector2 _destination , int _G = int.MaxValue / 2)
        {
            position = _position;
            parentPosition = _position;
            this.H = (int)Vector2.Distance(_position, _destination);
            this.G = _G;
        }

        //parentPosition 是 position的父节点
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
        //是未排序字典，采用哈希表的方式存储的，add是存在key,就更新，不存在就插入
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
            if (BattleMap.Instance().GetSpecificMapBlock((int)startPos.x, (int)startPos.y) == null || BattleMap.Instance().GetSpecificMapBlock((int)endPos.x, (int)endPos.y) == null)
            {
                Debug.Log("In MapNavigator: invalid Pos");
                return false;
            }

            //初始化openList和closeList
            openList = new Dictionary<Vector2, Node>();
            closeList = new Dictionary<Vector2, Node>();
            //加入起点
            openList.Add(startPos, new Node(startPos, endPos, 0));
            do
            {
                //遍历OpenList，寻找F值最小的节点，设为A
                Node A = new Node(startPos, endPos, int.MaxValue / 2);
                //遍历OpenList，寻找F值最小的节点，设为A
                foreach (Node node in openList.Values)
                {
                    if (node.H + node.G < A.H + A.G)
                        A = node;
                }

                AStarSearch(A, startPos, endPos);
                openList.Remove(A.position);
                closeList.Add(A.position, A);
                
                //如果找到了endPos
                //代码自己补
                if (A.H < Mathf.Epsilon)
                {
                    List<Node> paths = new List<Node>();
                    paths.Add(closeList[endPos]);
                    Debug.Log("找到路径");
                    for (int i = 0; i < closeList.Count; i++)
                    {
                        paths.Add(closeList[paths[i].parentPosition]);
                        if (paths[i + 1].G == 0)
                            break;
                    }
                    for (int i = paths.Count - 1; i >= 0; i--)
                    {
                        Debug.Log("最优路径" + paths[i].position);
                    }
                    
                    return true;
                }

                //OpenList是否还有节点
            } while (openList.Count > 0);

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
                    if (B.G > A.G + 1)
                    {
                        B.parentPosition = A.position;
                        B.G = A.G + 1;
                    }
                    continue;
                }
                //如果不在两个列表里
                else if (!openList.ContainsKey(B.position) )
                {
                    openList.Add(B.position, B);
                    continue; 
                }
            }
        }

    }

}



