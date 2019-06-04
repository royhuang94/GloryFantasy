﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System.IO;
using GameCard;

namespace PlayerCollection
{
    /// <summary>
    /// 管理大地图卡牌相关资源
    /// 
    /// 
    /// </summary>
    public class CardCollection : UnitySingleton<CardCollection>
    {
        /// <summary>
        /// 角色的卡牌收藏
        /// </summary>
        public List<string> mycollection = new List<string>();
        /// <summary>
        /// 英雄单位的战技牌，规则为英雄字符串-卡牌字符串
        /// </summary>
        public Dictionary<string, string> battleskill = new Dictionary<string, string>();
        /// <summary>图书馆正销售的卡牌链表
        /// 
        /// </summary>
        public List<string> librarylist = new List<string>();
        /// <summary>图书馆里被选中的卡牌0.0
        /// 
        /// </summary>
        public string choosecardID;
        public int choosecardindex;
        /// <summary>
        /// 通过卡牌ID向收藏中添加卡牌时调用，添加成功返回true
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool AddCard(string ID)
        {
            mycollection.Add(ID);
            return true;
        }
        /// <summary>图书馆购买卡牌时调用，移出商店并添加进卡牌收藏
        /// 
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public bool BuyCard()
        {
            mycollection.Add(choosecardID);
            Debug.Log("购买成功！");
            librarylist.Remove(choosecardID);
            return true;
        }
        /// <summary>获取三张卡牌信息,并写入librarylist;
        /// 
        /// </summary>
        public void GetCards()
        {
            
            // TODO : 这个json文件被删掉了，尽快查看新文件，并更新代码
            JsonData cardsJsonData =
            JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/Resources/Scripts/Cards/cardSample.1.json"));
            int dataAmount = cardsJsonData.Count;
            int num1 = Random.Range(0, dataAmount);
            int num2 = Random.Range(0, dataAmount);
            int num3 = Random.Range(0, dataAmount);
            librarylist.Add(cardsJsonData[num1]["id"].ToString());
            librarylist.Add(cardsJsonData[num2]["id"].ToString());
            librarylist.Add(cardsJsonData[num3]["id"].ToString());
        }
    }
}

