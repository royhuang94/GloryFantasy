using GameUnit;
using IMessage;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unit = GameUnit.GameUnit;

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

    public class MapNavigator : MsgReceiver
    {
        T IMessage.MsgReceiver.GetUnit<T>(){return this as T;}

        public Dictionary<Vector2, Node> openList;
        public Dictionary<Vector2, Node> closeList;
        List<Node> paths; //最优路径

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
                if (A.H < Mathf.Epsilon)
                {
                    paths = new List<Node>();
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

                    //判断移动力与路径长度
                    if (IsExcessUnitMove())
                        return false;

                    Debug.Log("成功移动到指定目标");
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
                else if (!openList.ContainsKey(B.position))
                {
                    openList.Add(B.position, B);
                    continue;
                }
            }
        }

        /// <summary>
        /// 判断A星算法给出的路径是否超过单位行动力
        /// </summary>
        /// <param name="unit">当前控制单位</param>
        /// <returns></returns>
        private bool IsExcessUnitMove()
        {
            Vector2 startPos = paths[paths.Count - 1].position;
            Unit gameUnit = BattleMap.Instance().GetUnitsOnMapBlock(startPos);

            Debug.Log(gameUnit.name + "行动力为 " + gameUnit.mov);
            if (paths.Count - 1 > gameUnit.mov)
            {
                Debug.Log("超过移动力，无法移动到指定目标");
                return true;
            }

            return false;
        }

        //一格一格移动
        public IEnumerator moveStepByStep(Unit unit)
        {
            //测试一哈
            List<Vector2> vector2s = new List<Vector2>();
            vector2s.Add(new Vector2(0, 0));
            BattleMap.Instance().debuffBM.SetBattleMapBlockBurning(vector2s);
            List<Vector2> vector22s = new List<Vector2>();
            vector22s.Add(new Vector2(1, 1));
            BattleMap.Instance().debuffBM.SetBattleMapBlockRetrad(vector22s);

            Vector2 tempVector;
            BattleMapBlock battleMap;
            for (int i = paths.Count - 2; i >= 0; i--)
            {
                //移除上一步的地图块儿下面的units_on_me
                tempVector = new Vector2((int)paths[i + 1].position.x, (int)paths[i + 1].position.y);
                battleMap = BattleMap.Instance().GetSpecificMapBlock(tempVector);
                battleMap.RemoveUnit(unit);
                //添加当前unit到地图块儿下面的units_on_me内
                tempVector = new Vector2((int)paths[i].position.x, (int)paths[i].position.y);
                battleMap = BattleMap.Instance().GetSpecificMapBlock(tempVector);
                battleMap.AddUnit(unit);
                //unit.gameObject.transform.SetParent(battleMap.transform);
                unit.transform.localPosition = Vector3.zero;
                if (battleMap.blockType == EMapBlockType.Burnning)//如果经过灼烧块
                {
                    BattleMap.Instance().debuffBM.UnitEnterBurning((tempVector));
                }
                else if (battleMap.blockType == EMapBlockType.Retire)//如果经过滞留块
                {
                    i = -1;//结束循环，将单位停在该位置；
                    //TODObug待修护 单位停在了滞留块上而不是鼠标点击位置，导致移动Jude（）的坐标与单位停的坐标不一致，空指针
                }
                unit.nextPos = paths[i].position;

                yield return new WaitForSeconds(0.2f);
                GamePlay.Gameplay.Instance().bmbColliderManager.Fresh(unit);
                MsgDispatcher.SendMsg((int)MessageType.Move);
            }
            MsgDispatcher.SendMsg((int)MessageType.Aftermove);
        }
    }
}



