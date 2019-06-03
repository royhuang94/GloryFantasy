using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Vectrosity;

namespace BattleMap
{

    public class DrawBattleArea
    {
        #region 画的方式显示战区框，弃用qwq

        //战斗地图块边长
        //int side = (int)BattleMap.Instance().battlePanel.GetComponent<GridLayoutGroup>().cellSize.x;
        int side = (int)BattleMap.Instance().flat.GetComponent<SpriteRenderer>().size.x;
        public Dictionary<int, List<Grid>> battleGridDic = new Dictionary<int, List<Grid>>();
        Dictionary<int, VectorLine> LinesDic = new Dictionary<int, VectorLine>();//所有战区外边框
        int offx = 0;
        int offy = 0;

        public class Grid
        {
            public Vector2 LeftTop = new Vector2();
            public Vector2 RightTop = new Vector2();
            public Vector2 LeftBottom = new Vector2();
            public Vector2 RightBottom = new Vector2();

            public Grid(Vector2 LT, Vector2 RT, Vector2 LB, Vector2 RB)
            {
                LeftTop = LT;
                RightTop = RT;
                LeftBottom = LB;
                RightBottom = RB;
            }
        }

        /// <summary>
        /// 获取每个格子的每个顶点，生成格子对象，保存在字典中
        /// </summary>
        public void GetBattleBlockAllPosiotn()
        {
            foreach (int k in BattleMap.Instance().battleAreaData.BattleAreaDic.Keys)
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
                    LeftBottom = new Vector2((int)lis[i].x * side + offx, ((int)lis[i].y + 1) * -side + offy);
                    RightBottom = new Vector2(((int)lis[i].x + 1) * side + offx, ((int)lis[i].y + 1) * -side + offy);
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

            for (int i = 0; i < lis.Count; i++)
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
        /// 画战区外边框
        /// </summary>
        public void DrawLine()
        {
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
            for (int i = 0; i < list.Count; i++)
            {
                list[i] = new Vector2((int)list[i].x * side + offx, (int)list[i].y * -side + offy);
            }
        }

        /// <summary>
        /// 判断该点是否是交点坐标
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="targgleList"></param>
        /// <returns></returns>
        public bool IsNodicalPosition(Vector2 pos, List<Vector2> targgleList)
        {
            foreach (Vector2 k in targgleList)
            {
                if (k == pos)
                {
                    return true;
                }
            }
            return false;
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
        #endregion

        public Dictionary<int, List<SpriteRenderer>> BattleAreaRenderDic = new Dictionary<int, List<SpriteRenderer>>();
        private bool isBattleAreaShow = true;

        /// <summary>
        /// 获取格子边框图片，不是必要的情况下，请不要点开，我自己都要吐了
        /// </summary>
        public void GetBattleAreaBorder()//1
        {
            Vector2 TopOff = new Vector2(0, -1);
            Vector2 BottomOff = new Vector2(0, 1);
            Vector2 LeftOff = new Vector2(-1, 0);
            Vector2 RightOff = new Vector2(1, 0);
            int padding = 0;

            foreach (int k in BattleMap.Instance().battleAreaData.BattleAreaDic.Keys)
            {
                List<Vector2> battleAreas = null;
                BattleMap.Instance().battleAreaData.BattleAreaDic.TryGetValue(k, out battleAreas);

                List<SpriteRenderer> images = new List<SpriteRenderer>();
                for (int i = 0; i < battleAreas.Count; i++)
                {
                    BattleMapBlock battleMapBlock = BattleMap.Instance().GetSpecificMapBlock(battleAreas[i]);


                    //判断它周围还有没有格子，有格子就不用管了
                    if (HasBorderUpon(k, battleAreas[i] + TopOff))//上面没有格子
                    {
                        //RectTransform transform = battleMapBlock.transform.Find("Top").GetComponent<RectTransform>();
                        //transform.anchoredPosition += new Vector2(0, -padding);
                        SpriteRenderer image = battleMapBlock.transform.Find("Top").GetComponent<SpriteRenderer>();
                        images.Add(image);
                    }
                    if (HasBorderUpon(k, battleAreas[i] + BottomOff))//下面没有格子
                    {
                        //RectTransform transform = battleMapBlock.transform.Find("Bottom").GetComponent<RectTransform>();
                        //transform.anchoredPosition += new Vector2(0, padding);
                        SpriteRenderer image = battleMapBlock.transform.Find("Bottom").GetComponent<SpriteRenderer>();
                        images.Add(image);
                    }
                    if (HasBorderUpon(k, battleAreas[i] + LeftOff))//左边没有格子
                    {
                        //RectTransform transform = battleMapBlock.transform.Find("Left").GetComponent<RectTransform>();
                        //transform.anchoredPosition += new Vector2(padding, 0);
                        SpriteRenderer image = battleMapBlock.transform.Find("Left").GetComponent<SpriteRenderer>();
                        images.Add(image);
                    }
                    if (HasBorderUpon(k, battleAreas[i] + RightOff))//右边没有格子
                    {
                        //RectTransform transform = battleMapBlock.transform.Find("Right").GetComponent<RectTransform>();
                        //transform.anchoredPosition += new Vector2(-padding, 0);
                        SpriteRenderer image = battleMapBlock.transform.Find("Right").GetComponent<SpriteRenderer>();
                        images.Add(image);
                    }

                    if (!HasBorderUpon(k, battleAreas[i] + TopOff) && HasBorderUpon(k, battleAreas[i] + BottomOff))//如果上面有格子,下面没有格子
                    {
                        if (!HasBorderUpon(k, battleAreas[i] + LeftOff) && HasBorderUpon(k, battleAreas[i] + LeftOff + TopOff))//如果左边有格子,左上角还不能有格子，我自己都要吐了 呕！
                        {
                            //RectTransform transform = battleMapBlock.transform.Find("LeftTopRangle").GetComponent<RectTransform>();
                            //transform.anchoredPosition += new Vector2(padding, -padding);
                            SpriteRenderer image = battleMapBlock.transform.Find("LeftTopRangle").GetComponent<SpriteRenderer>();
                            images.Add(image);
                        }
                        else if (!HasBorderUpon(k, battleAreas[i] + RightOff) && HasBorderUpon(k, battleAreas[i] + RightOff + TopOff))//右边有格子
                        {
                            //RectTransform transform = battleMapBlock.transform.Find("RightopRangle").GetComponent<RectTransform>();
                            //transform.anchoredPosition += new Vector2(-padding, -padding);
                            SpriteRenderer image = battleMapBlock.transform.Find("RightopRangle").GetComponent<SpriteRenderer>();
                            images.Add(image);
                        }
                    }

                    if (HasBorderUpon(k, battleAreas[i] + TopOff) && !HasBorderUpon(k, battleAreas[i] + BottomOff))//如果上面没有格子,下面有格子
                    {
                        if (!HasBorderUpon(k, battleAreas[i] + LeftOff) && HasBorderUpon(k, battleAreas[i] + LeftOff + BottomOff))//如果左边有格子，右上角没有格子
                        {
                            //RectTransform transform = battleMapBlock.transform.Find("LeftBottomRangle").GetComponent<RectTransform>();
                            //transform.anchoredPosition += new Vector2(padding, padding);
                            SpriteRenderer image = battleMapBlock.transform.Find("LeftBottomRangle").GetComponent<SpriteRenderer>();
                            images.Add(image);
                        }
                        else if (!HasBorderUpon(k, battleAreas[i] + RightOff) && HasBorderUpon(k, battleAreas[i] + RightOff + BottomOff))//右边有格子
                        {
                            //RectTransform transform = battleMapBlock.transform.Find("RightBottomRangle").GetComponent<RectTransform>();
                            //transform.anchoredPosition += new Vector2(-padding, padding);
                            SpriteRenderer image = battleMapBlock.transform.Find("RightBottomRangle").GetComponent<SpriteRenderer>();
                            images.Add(image);
                        }
                    }
                }
                BattleAreaRenderDic.Add(k, images);
            }
        }

        /// <summary>
        /// 判断周围有无相邻格子
        /// </summary>
        /// <param name="reginID"></param>
        /// <param name="vector2"></param>
        /// <returns></returns>
        private bool HasBorderUpon(int reginID,Vector2 vector2)
        {
            List<Vector2> battleAreas = new List<Vector2>();
            BattleMap.Instance().battleAreaData.BattleAreaDic.TryGetValue(reginID, out battleAreas);
            for(int a = 0;a <battleAreas.Count;a++)
            {
                if (vector2 == battleAreas[a])
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 显示战区内边框
        /// </summary>
        public void ShowAndUpdateBattleArea()//2
        {
            foreach (int id in BattleMap.Instance().battleAreaData.BattleAreaDic.Keys)
            {
                List<SpriteRenderer> images = new List<SpriteRenderer>();
                BattleAreaRenderDic.TryGetValue(id, out images);
                if (BattleMap.Instance().battleAreaData.WarZoneBelong(id) == BattleAreaSate.Enmey)
                {
                    for (int i = 0; i < images.Count; i++)
                        images[i].color = new Color(255, 0, 0, 255);
                }
                else if (BattleMap.Instance().battleAreaData.WarZoneBelong(id) == BattleAreaSate.Battle)
                {
                    for (int i = 0; i < images.Count; i++)
                        images[i].color = new Color(255, 125, 0, 255);
                }
                else if (BattleMap.Instance().battleAreaData.WarZoneBelong(id) == BattleAreaSate.Player)
                {
                    for (int i = 0; i < images.Count; i++)
                        images[i].color = new Color(0, 255, 0, 255);
                }
                else if (isBattleAreaShow && BattleMap.Instance().battleAreaData.WarZoneBelong(id) == BattleAreaSate.Neutrality)
                {
                    for (int i = 0; i < images.Count; i++)
                    {
                        images[i].color = new Color(255, 255, 255, 255);
                    }
                }
            }
            isBattleAreaShow = false;
        }

        /// <summary>
        /// 隐藏战区
        /// </summary>
        public void HideBattleArea()
        {
            foreach (int id in BattleMap.Instance().battleAreaData.BattleAreaDic.Keys)
            {
                List<SpriteRenderer> images = new List<SpriteRenderer>();
                BattleAreaRenderDic.TryGetValue(id, out images);
                for (int i = 0; i < images.Count; i++)
                {
                    images[i].color = new Color(255, 255, 255, 0);
                }
            }
        }
    }
}


