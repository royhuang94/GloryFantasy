using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System.IO;
using GameCard;
using System.Linq;
using MainMap;
using Mediator;
using GameGUI;

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
        /// <summary>
        /// 存储所有的卡牌Json数据
        /// </summary>
        private JsonData _cardsJsonData;
        /// <summary>
        /// 存储所有的单位Json数据
        /// </summary>
        private JsonData _unitsJsonData;
        /// <summary>
        /// 存有从卡牌ID到对应数据的字典
        /// </summary>
        private Dictionary<string, JsonData> _cardData;
        /// <summary>
        /// 存有从单位ID到对应数据的字典
        /// </summary>
        private Dictionary<string, JsonData> _unitData;
        /// <summary>
        /// 角色的卡牌收藏
        /// </summary>
        public static List<string> mycollection = new List<string>();
        public Deck deck = new Deck(mycollection, "HElf_1");
        /// <summary>
        /// 英雄单位的战技牌，规则为英雄字符串-卡牌字符串
        /// </summary>
        public Dictionary<string, string> battleskill = new Dictionary<string, string>();
        /// <summary>图书馆里被选中的卡牌0.0
        /// 
        /// </summary>
        public string choosecardID;
        #region 弃用的变量
        ///// <summary>
        ///// 图书馆展示的最大卡牌张数
        ///// </summary>
        //private static int librarylength = 3;
        #endregion
        public int choosecardindex;
        #endregion
        private void Awake()
        {
            // 获取卡牌库和单位库数据
            json = Resources.Load<TextAsset>("DatabaseJsonFiles/CardDatabase");
            _cardsJsonData = JsonMapper.ToObject(json.text);
            json = Resources.Load<TextAsset>("DatabaseJsonFiles/UnitDatabase");
            _unitsJsonData = JsonMapper.ToObject(json.text);
            
            // 初始化卡牌库及单位库数据
            _cardData = new Dictionary<string, JsonData>();
            for (int i = 0; i < _cardsJsonData.Count; i++)
            {
                _cardData.Add(_cardsJsonData[i]["ID"].ToString(), _cardsJsonData[i]);
            }

            _unitData = new Dictionary<string, JsonData>();
            for (int i = 0; i < _unitsJsonData.Count; i++)
            {
                _unitData.Add(_unitsJsonData[i]["ID"].ToString(), _unitsJsonData[i]);
            }
        }
        
        /// <summary>
        /// 获得随机的指定张数的卡牌
        /// </summary>
        /// <param name="controlNum">控制需要的牌的张数，默认为3</param>
        /// <returns>返回List</returns>
        public List<string> GetRandomCards(int controlNum = 3)
        {
            List<string> randomlyPickedCards = new List<string>();

            int count = 0;
            do
            {
                // 随机生成数
                int index = Random.Range(0, _cardsJsonData.Count);
                bool isHeroUnit = false;
                
                // 确保不含有英雄字段
                for (int i = 0; i < _cardsJsonData[index]["tag"].Count; i++)
                {
                    if (_cardsJsonData[index]["tag"][i].ToString().Equals("英雄"))
                    {
                        isHeroUnit = true;
                        break;
                    }
                }
                // 如果是蔻蔻这种英雄卡牌，不添加
                if(isHeroUnit)
                    continue;
                
                // 添加普通卡牌
                randomlyPickedCards.Add(_cardsJsonData[index]["ID"].ToString());
                count++;
            } while (count < controlNum);

            return randomlyPickedCards;
        }
        /// <summary>
        /// 通过卡牌ID向收藏中添加卡牌时调用，添加成功返回true
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool AddCard(string ID)
        {
            string newID;
            if (int.Parse(ID.Split('_')[1]) > 1)
            {
                newID = ID.Split('_')[0] + "_1";
            }
            else
            {
                newID = ID;
            }
            mycollection.Add(newID);
            return true;
        }
        /// <summary>图书馆购买卡牌时调用，移出商店并添加进卡牌收藏
        /// 
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public bool BuyCard(Library library)
        {
            string newID;
            if (int.Parse(choosecardID.Split('_')[1]) > 1)
            {
                newID = choosecardID.Split('_')[0] + "_1";
            }
            else
            {
                newID = choosecardID;
            }
            mycollection.Add(newID);
            deck.FreshDeck(mycollection);
            Debug.Log("购买成功！");
            Debug.Log(choosecardID);
            Debug.Log(newID);
            MainMapUI.Instance().UpdateGold(-1);
            library.librarylist.Remove(choosecardID);
            return true;
        }
# region 弃用的获取卡牌代码
        ///// <summary>获取三张卡牌信息,并写入librarylist;
        ///// 
        ///// </summary>
        //public void GetCards(Library library)
        //{
        //    int dataAmount = cardsJsonData.Count;
        //    for(int i=0; i<librarylength;i++)
        //    {
        //        library.librarylist.Add(GetCardID(Random.Range(0, dataAmount)));
        //    }
        //}
#endregion
        /// <summary>
        /// 根据传入的jsoncount得到卡牌id
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        private string GetCardID(int num)
        {
            return _cardsJsonData[num]["ID"].ToString();

        }

        /// <summary>
        /// 获取指定卡牌ID的卡牌数据，为JsonData格式
        /// </summary>
        /// <param name="cardId">想获取详细信息的卡牌ID</param>
        /// <returns>若无记录，则返回null，否则返回对应的jsonData数据</returns>
        public JsonData GetCardData(string cardId)
        {
            if (_cardData.ContainsKey(cardId))
                return _cardData[cardId];
            return null;
        }

        /// <summary>
        /// 获取指定id的单位数据，为JsonData格式
        /// </summary>
        /// <param name="id">想获取详细信息的单位ID</param>
        /// <returns>若无记录，则返回NULL，否则返回对应的数据</returns>
        public JsonData GetUnitData(string id)
        {
            if (_unitData.ContainsKey(id))
                return _unitData[id];
            return null;
        }
        public void CardCollectBeZero()
        {
            mycollection.Clear();
            mycollection.Add("HElf_1");
            deck.FreshDeck(mycollection);
        }
    }
}

