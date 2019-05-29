using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Vectrosity;

namespace BattleMap
{
    public class Grid
    {
        public Vector2 LeftTop = new Vector2();
        public Vector2 RightTop = new Vector2();
        public Vector2 LeftBottom = new Vector2();
        public Vector2 RightBottom = new Vector2();

        public Grid(Vector2 LT,Vector2 RT,Vector2 LB,Vector2 RB)
        {
            LeftTop = LT;
            RightTop = RT;
            LeftBottom = LB;
            RightBottom = RB;
        }
    }

    public class DrawBattleArea
    {
        //战斗地图块边长
        int side = (int)BattleMap.Instance().battlePanel.GetComponent<GridLayoutGroup>().cellSize.x;
        public Dictionary<int, List<Grid>> battleGridDic = new Dictionary<int, List<Grid>>();
        Dictionary<int, VectorLine> LinesDic = new Dictionary<int, VectorLine>();//所有战区外边框
        int offx = 430;
        int offy = 500;

        /// <summary>
        /// 获取每个格子的每个顶点，生成格子对象，保存在字典中
        /// </summary>
        public void GetBattleBlockAllPosiotn()
        {
            foreach(int k in BattleMap.Instance().battleAreaData.BattleAreaDic.Keys)
            {
                List<Vector2> lis = new List<Vector2>();
                BattleMap.Instance().battleAreaData.BattleAreaDic.TryGetValue(k, out lis);

                Vector2 LeftTop = new Vector2();
                Vector2 RightTop = new Vector2();
                Vector2 LeftBottom = new Vector2();
                Vector2 RightBottom = new Vector2();
                List<Grid> grids = new List<Grid>();
                for (int i = 0; i < lis.Count; i++)
                {
                    LeftTop = new Vector2((int)lis[i].x * side + offx, (int)lis[i].y * -side + offy);
                    RightTop = new Vector2(((int)lis[i].x + 1) * side + offx, (int)lis[i].y * -side + offy);
                    LeftBottom = new Vector2((int)lis[i].x  * side + offx, ((int)lis[i].y +1) * -side + offy);
                    RightBottom = new Vector2(((int)lis[i].x + 1) * side + offx, ((int)lis[i].y+1) * -side + offy);
                    Grid grid = new Grid(LeftTop, RightTop, LeftBottom, RightBottom);
                    grids.Add(grid);
                }
                battleGridDic.Add(k, grids);
            }
        }

        /// <summary>
        /// 获取每个格子所有顶点坐标
        /// </summary>
        /// <param name="lis"></param>
        public List<Vector2> GetBattleBlockAllPosiotn(List<Vector2> lis)
        {
            Vector2 LeftTop = new Vector2();
            Vector2 RightTop = new Vector2();
            Vector2 LeftBottom = new Vector2();
            Vector2 RightBottom = new Vector2();

            List<Vector2> allPositons = new List<Vector2>();

            for (int i = 0;i< lis.Count; i++)
            {
                LeftTop = lis[i];
                RightTop = lis[i] + new Vector2(1, 0);
                LeftBottom = lis[i] + new Vector2(0, 1);
                RightBottom = lis[i] + new Vector2(1, 1);
                allPositons.Add(LeftTop);
                allPositons.Add(RightTop);
                allPositons.Add(LeftBottom);
                allPositons.Add(RightBottom);
            }
            DealPoiston(allPositons);
            return allPositons;
        }

        /// <summary>
        /// 移除并获取交点坐标
        /// </summary>
        /// <param name="reslist"></param>
        public List<Vector2> RemovetAndGetNodicals(List<Vector2> reslist)
        {
            List<Vector2> nodicalPositons = new List<Vector2>();
            for (int i = 0; i < reslist.Count; i++)
            {
                for (int j = reslist.Count - 1; j > i; j--)
                {
                    if (reslist[i] == reslist[j])
                    {
                        nodicalPositons.Add(reslist[i]);
                        reslist.RemoveAt(j);    
                    }
                }
            }
            return nodicalPositons;
        }

        /// <summary>
        /// 坐标调整一下
        /// </summary>
        /// <param name="list"></param>
        private void DealPoiston(List<Vector2> list)
        {
            for(int i = 0; i< list.Count; i++)
            {
                list[i] = new Vector2((int)list[i].x * side + offx, (int)list[i].y * -side  +offy);
            }
        }

        /// <summary>
        /// 判断该点是否是交点坐标
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="targgleList"></param>
        /// <returns></returns>
        public bool IsNodicalPosition(Vector2 pos,List<Vector2> targgleList)
        {
            foreach(Vector2 k in targgleList)
            {
                if(k == pos)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 画战区外边框
        /// </summary>
        public void DrawLine()
        {
            Debug.Log("fdaf");
            GetBattleBlockAllPosiotn();
            Dictionary<int, List<Grid>> gridsDic = battleGridDic;
            DrawBattleArea drawBattleArea = BattleMap.Instance().drawBattleArea;

            foreach (int i in gridsDic.Keys)
            {
                List<Grid> grids = null;
                gridsDic.TryGetValue(i, out grids);

                List<Vector2> battleAreas = null;
                BattleMap.Instance().battleAreaData.BattleAreaDic.TryGetValue(i, out battleAreas);
                //获取每个格子的顶点坐标
                List<Vector2> vertexs = BattleMap.Instance().drawBattleArea.GetBattleBlockAllPosiotn(battleAreas);
                //    //移除重复并保存交点坐标
                List<Vector2> nodicals = BattleMap.Instance().drawBattleArea.RemovetAndGetNodicals(vertexs);

                List<Vector2> drawR = new List<Vector2>();
                List<Vector2> drawL = new List<Vector2>();
                bool isNewLine = false;
                for (int j = 0; j <= grids.Count - 1; j++)
                {
                    int end = grids.Count - 1;

                    if (j == 0)//第一块特殊处理
                    {
                        Vector2 vectorTT = new Vector2(grids[0].LeftTop.x + 4, grids[0].LeftTop.y + 4);
                        Vector2 vectorTB = new Vector2(grids[0].LeftTop.x + 4, grids[0].LeftTop.y - 4);
                        Vector2 vectorBT = new Vector2(grids[0].LeftBottom.x + 4, grids[0].LeftBottom.y + 4);
                        Vector2 vectorBB = new Vector2(grids[0].LeftBottom.x + 4, grids[0].LeftBottom.y - 4);
                        if (IsNodicalPosition(grids[0].LeftTop, nodicals))//该点是不是交点坐标
                        {
                            drawL.Add(vectorTT);
                            drawL.Add(vectorTB);
                        }
                        else
                        {
                            drawL.Add(vectorTB);
                        }
                        if (IsNodicalPosition(grids[0].LeftBottom, nodicals))
                        {
                            drawL.Add(vectorBT);
                            drawL.Add(vectorBB);
                        }
                        else
                        {
                            drawL.Add(vectorBT);
                        }
                    }
                    if (j == grids.Count - 1)//最后一个
                    {
                        Vector2 vectorTT = new Vector2(grids[end].RightTop.x - 4, grids[j].RightTop.y + 4);
                        Vector2 vectorTB = new Vector2(grids[end].RightTop.x - 4, grids[j].RightTop.y - 4);
                        Vector2 vectorBT = new Vector2(grids[end].RightBottom.x - 4, grids[j].RightBottom.y + 4);
                        Vector2 vectorBB = new Vector2(grids[end].RightBottom.x - 4, grids[j].RightBottom.y - 4);
                        if (IsNodicalPosition(grids[end].RightTop, nodicals))//该点是不是交点坐标
                        {
                            drawR.Add(vectorTT);
                            drawR.Add(vectorTB);
                        }
                        else
                        {
                            drawR.Add(vectorTB);
                        }
                        if (IsNodicalPosition(grids[end].RightBottom, nodicals))
                        {
                            drawR.Add(vectorBT);
                            drawR.Add(vectorBB);
                        }
                        else
                        {
                            drawR.Add(vectorBT);
                        }
                    }
                    if (j + 1 < grids.Count && grids[j].LeftTop.y != grids[j + 1].LeftTop.y)//该行最后一个
                    {
                        Vector2 vectorTT = new Vector2(grids[j].RightTop.x - 4, grids[j].RightTop.y + 4);
                        Vector2 vectorTB = new Vector2(grids[j].RightTop.x - 4, grids[j].RightTop.y - 4);
                        Vector2 vectorBT = new Vector2(grids[j].RightBottom.x - 4, grids[j].RightBottom.y + 4);
                        Vector2 vectorBB = new Vector2(grids[j].RightBottom.x - 4, grids[j].RightBottom.y - 4);
                        if (IsNodicalPosition(grids[j].RightTop, nodicals))//该点是不是交点坐标
                        {
                            drawR.Add(vectorTT);
                            drawR.Add(vectorTB);
                        }
                        else
                        {
                            drawR.Add(vectorTB);
                        }
                        if (IsNodicalPosition(grids[j].RightBottom, nodicals))
                        {
                            drawR.Add(vectorBT);
                            drawR.Add(vectorBB);
                        }
                        else
                        {
                            drawR.Add(vectorBT);
                        }
                        isNewLine = true;
                    }
                    else if (isNewLine)//该行第一个
                    {
                        Vector2 vectorTT = new Vector2(grids[j].LeftTop.x + 4, grids[j].LeftTop.y + 4);
                        Vector2 vectorTB = new Vector2(grids[j].LeftTop.x + 4, grids[j].LeftTop.y - 4);
                        Vector2 vectorBT = new Vector2(grids[j].LeftBottom.x + 4, grids[j].LeftBottom.y + 4);
                        Vector2 vectorBB = new Vector2(grids[j].LeftBottom.x + 4, grids[j].LeftBottom.y - 4);
                        if (IsNodicalPosition(grids[j].LeftTop, nodicals))
                        {
                            drawL.Add(vectorTT);
                            drawL.Add(vectorTB);
                        }
                        else
                        {
                            drawL.Add(vectorTB);
                        }
                        if (IsNodicalPosition(grids[j].LeftBottom, nodicals))
                        {
                            drawL.Add(vectorBT);
                            drawL.Add(vectorBB);
                        }
                        else
                        {
                            drawL.Add(vectorBT);
                        }
                        isNewLine = false;
                    }
                }

                for (int d = drawL.Count - 1; d >= 0; d--)
                {
                    drawR.Add(drawL[d]);
                }
                drawR.Add(drawR[0]);

                VectorLine myLine = new VectorLine("Line" + i.ToString(), drawR, 2.0f, LineType.Continuous);
                LinesDic.Add(i, myLine);
                myLine.lineWidth = 4.0f;
                myLine.Draw();
                myLine.SetColor(new Color(255, 255, 255, 0));
            }
        }

        /// <summary>
        /// 通过战区id获取战区边框；
        /// </summary>
        /// <param name="reginID"></param>
        /// <returns></returns>
        public VectorLine GetLineByID(int reginID)
        {
            VectorLine vectorLine = null;
            if (LinesDic.ContainsKey(reginID))
            {
                LinesDic.TryGetValue(reginID, out vectorLine);
                return vectorLine;
            }
            else
            {
                Debug.Log("该战区不存在");
                return null;
            }
        }
    }
}


