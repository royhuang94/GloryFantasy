using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System.IO;
using GameCard;
using System.Linq;
using MainMap;

namespace PlayerCollection
{
    /// <summary>
    /// 管理大地图卡牌相关资源
    /// 
    /// 
    /// </summary>
    public class CardCollection : UnitySingleton<CardCollection>
    {
        #region 一堆变量和引用
        TextAsset json;
        JsonData cardsJsonData;
        /// <summary>
        /// 角色的卡牌收藏
        /// </summary>
        public List<string> mycollection = new List<string>();
        /// <summary>
        /// 英雄单位的战技牌，规则为英雄字符串-卡牌字符串
        /// </summary>
        public Dictionary<string, string> battleskill = new Dictionary<string, string>();
        /// <summary>图书馆里被选中的卡牌0.0
        /// 
        /// </summary>
        public string choosecardID;
        /// <summary>
        /// 图书馆展示的最大卡牌张数
        /// </summary>
        private static int librarylength = 3;
        public int choosecardindex;
        #endregion
        private void Awake()
        {
            json = Resources.Load<TextAsset>("DatabaseJsonFiles/CardDatabase");
            cardsJsonData = JsonMapper.ToObject(json.text);
        }
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
        public bool BuyCard(Library library)
        {
            mycollection.Add(choosecardID);
            Debug.Log("购买成功！");
            library.librarylist.Remove(choosecardID);
            return true;
        }
        /// <summary>获取三张卡牌信息,并写入librarylist;
        /// 
        /// </summary>
        public void GetCards(Library library)
        {
            int dataAmount = cardsJsonData.Count;
            for(int i=0; i<librarylength;i++)
            {
                library.librarylist.Add(GetCardID(Random.Range(0, dataAmount)));
            }
        }
        /// <summary>
        /// 根据传入的jsoncount得到卡牌id
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        private string GetCardID(int num)
        {
            return cardsJsonData[num]["ID"].ToString().Split('_').First();
        }
    }
}

