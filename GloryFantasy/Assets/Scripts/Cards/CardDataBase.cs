using System;
using System.Collections.Generic;
using LitJson;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GameCard
{
    public class CardDataBase : UnitySingleton<CardDataBase>
    {
        /// <summary>
        /// 所有卡牌数据的总和，生肉
        /// </summary>
        private JsonData _rawData;
        /// <summary>
        /// 存储卡牌映射数据的字典
        /// </summary>
        private Dictionary<string, JsonData> _cardsData;

        private void Start()
        {
            // 初始化字典变量
            _cardsData = new Dictionary<string, JsonData>();
            
            // 从Resources/DatabaseJsonFiles/CardDataBase.json文件读入数据
            TextAsset json = Resources.Load<TextAsset>("DatabaseJsonFiles/CardDatabase");
            // 转换成映射字典
            JsonData cardsJsonData = JsonMapper.ToObject(json.text);
            // 传递元数据引用
            _rawData = cardsJsonData;
            // 用数据填充字典
            for (int i = 0; i < cardsJsonData.Count; i++)
            {
                _cardsData.Add(cardsJsonData[i]["ID"].ToString(), cardsJsonData[i]);
            }
        }

        /// <summary>
        /// 查找并返回给定的卡牌ID对应的Json数据，也可以用于判断卡牌ID是否存在
        /// 存在，则返回jsonData数据; 不存在则返回null
        /// </summary>
        /// <param name="cardId">要查询的卡牌ID</param>
        /// <returns>若ID存在，则返回对应的JsonData数据，否则返回null</returns>
        public JsonData GetCardJsonData(string cardId)
        {
            // 检查key
            if (_cardsData.ContainsKey(cardId))
                return _cardsData[cardId];
            
            // 不存在给定的ID，则返回null
            return null;
        }

        /// <summary>
        /// 传入卡牌ID，并由此生成卡牌实例并返回，可能造成内存泄漏，慎用
        /// 会对卡牌进行检查
        /// </summary>
        /// <param name="cardId">要获取的卡牌实例的ID</param>
        /// <returns>返回对应的实例</returns>
        /// <exception cref="NotImplementedException">若卡牌ID不存在则抛出此异常</exception>
        public BaseCard GetCardInstanceById(string cardId)
        {
            JsonData jsonData = GetCardJsonData(cardId);
            
            // 检查获取的卡牌ID是否存在
            if (jsonData == null)
                throw new NotImplementedException();

            BaseCard newcard;
            
            // 根据卡牌类型创建新卡牌对象
            if (jsonData["Type"].ToString().Equals("Order"))
            {
                newcard = new OrderCard();
            }
            else if (jsonData["Type"].ToString().Equals("Unit"))
            {
                newcard = new UnitCard();
            }
            else
                throw new NotImplementedException();
            
            // 进行数值初始化
            newcard.Init(cardId, jsonData);
            
            return newcard;
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
                int index = Random.Range(0, _rawData.Count);
                bool isHeroUnit = false;
                
                // 确保不含有英雄字段
                for (int i = 0; i < _rawData[index]["Tag"].Count; i++)
                {
                    if (_rawData[index]["Tag"][i].ToString().Equals("英雄"))
                    {
                        isHeroUnit = true;
                        break;
                    }
                }
                // 如果是蔻蔻这种英雄卡牌，不添加
                if(isHeroUnit)
                    continue;
                
                // 添加普通卡牌
                randomlyPickedCards.Add(_rawData[index]["ID"].ToString());
                count++;
            } while (count < controlNum);

            return randomlyPickedCards;
        }
    }
}