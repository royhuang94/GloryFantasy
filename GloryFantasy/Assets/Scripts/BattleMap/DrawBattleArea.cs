using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Vectrosity;

namespace BattleMap
{

    public class DrawBattleArea
    {
        public Dictionary<int, List<SpriteRenderer>> BattleAreaRenderDic = new Dictionary<int, List<SpriteRenderer>>();
        private bool isBattleAreaShow = true;

        /// <summary>
        /// 获取格子边框图片，不是必要的情况下，请不要点开，我自己都要吐了
        /// </summary>
        public void GetBattleAreaBorder()
        {
            Vector2 TopOff = new Vector2(0, -1);
            Vector2 BottomOff = new Vector2(0, 1);
            Vector2 LeftOff = new Vector2(-1, 0);
            Vector2 RightOff = new Vector2(1, 0);
            //int padding = 0;

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
                        SpriteRenderer image = battleMapBlock.transform.Find("Top").GetComponent<SpriteRenderer>();
                        images.Add(image);
                    }
                    if (HasBorderUpon(k, battleAreas[i] + BottomOff))//下面没有格子
                    {
                        SpriteRenderer image = battleMapBlock.transform.Find("Bottom").GetComponent<SpriteRenderer>();
                        images.Add(image);
                    }
                    if (HasBorderUpon(k, battleAreas[i] + LeftOff))//左边没有格子
                    {
                        SpriteRenderer image = battleMapBlock.transform.Find("Left").GetComponent<SpriteRenderer>();
                        images.Add(image);
                    }
                    if (HasBorderUpon(k, battleAreas[i] + RightOff))//右边没有格子
                    {
                        SpriteRenderer image = battleMapBlock.transform.Find("Right").GetComponent<SpriteRenderer>();
                        images.Add(image);
                    }

                    if (!HasBorderUpon(k, battleAreas[i] + TopOff) && HasBorderUpon(k, battleAreas[i] + BottomOff))//如果上面有格子,下面没有格子
                    {
                        if (!HasBorderUpon(k, battleAreas[i] + LeftOff) && HasBorderUpon(k, battleAreas[i] + LeftOff + TopOff))//如果左边有格子,左上角还不能有格子，我自己都要吐了 呕！
                        {
                            SpriteRenderer image = battleMapBlock.transform.Find("LeftTopRangle").GetComponent<SpriteRenderer>();
                            images.Add(image);
                        }
                        else if (!HasBorderUpon(k, battleAreas[i] + RightOff) && HasBorderUpon(k, battleAreas[i] + RightOff + TopOff))//右边有格子
                        {
                            SpriteRenderer image = battleMapBlock.transform.Find("RightopRangle").GetComponent<SpriteRenderer>();
                            images.Add(image);
                        }
                    }

                    if (HasBorderUpon(k, battleAreas[i] + TopOff) && !HasBorderUpon(k, battleAreas[i] + BottomOff))//如果上面没有格子,下面有格子
                    {
                        if (!HasBorderUpon(k, battleAreas[i] + LeftOff) && HasBorderUpon(k, battleAreas[i] + LeftOff + BottomOff))//如果左边有格子，右上角没有格子
                        {
                            SpriteRenderer image = battleMapBlock.transform.Find("LeftBottomRangle").GetComponent<SpriteRenderer>();
                            images.Add(image);
                        }
                        else if (!HasBorderUpon(k, battleAreas[i] + RightOff) && HasBorderUpon(k, battleAreas[i] + RightOff + BottomOff))//右边有格子
                        {
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
        public void ShowAndUpdateBattleArea()
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
                //else if (BattleMap.Instance().battleAreaData.WarZoneBelong(id) == BattleAreaSate.Battle)
                //{
                //    for (int i = 0; i < images.Count; i++)
                //        images[i].color = new Color(255, 125, 0, 255);
                //}
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


