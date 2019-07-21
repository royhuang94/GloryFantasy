using System;
using System.Collections.Generic;
using System.Linq;
using Cards;
using LitJson;
using UnityEngine;

namespace GameCard
{
    public class DeckController : UnitySingleton<DeckController>
    {
        #region 变量
        
        /// <summary>
        /// 所有卡牌的list
        /// </summary>
        private List<BaseCard> _allCards;
        /// <summary>
        /// 单位牌的list
        /// </summary>
        private List<BaseCard> _unitCards;
        /// <summary>
        /// 效果牌的list
        /// </summary>
        private List<BaseCard> _orderCards;
        /// <summary>
        /// 存储战技映射关系的字典
        /// CardInstance(_NewBaseCard) --> List{ _NewOrderCard...}
        /// </summary>
        private Dictionary<BaseCard, List<OrderCard>> _exskillRecorder;
        
        
        #endregion
        private void Start()
        {
            _allCards = new List<BaseCard>();
            _unitCards = new List<BaseCard>();
            _orderCards = new List<BaseCard>();
            _exskillRecorder = new Dictionary<BaseCard, List<OrderCard>>();
            
            
            // TODO: 添加初始手牌
        }

        
        /// <summary>
        /// 通过卡牌ID向牌堆内放入卡牌，在内部进行卡牌ID合法性检查
        /// </summary>
        /// <param name="cardId">要放入的卡牌的ID</param>
        /// <exception cref="NotImplementedException">若卡牌ID不存在则抛出此异常</exception>
        public void InsertCardById(string cardId)
        {
            // 调用函数创建卡牌实例
            BaseCard card = CardDataBase.Instance().GetCardInstanceById(cardId);
            
            // 根据卡牌类型收纳
            if(card.Type.Equals("Order"))
                _orderCards.Add(card);
            else if(card.Type.Equals("Unit"))
                _unitCards.Add(card);
            
            // 不管什么类型都会添加到总list内
            _allCards.Add(card);
        }

        /// <summary>
        /// 从牌堆中移除指定卡牌，移除规则为首张ID一致的单张牌
        /// </summary>
        /// <param name="cardId"></param>
        public void RemoveCardById(string cardId)
        {
            BaseCard card = null;
            for (int i = 0; i < _allCards.Count; i++)
            {
                if (_allCards[i].Id.Equals(cardId))
                {
                    card = _allCards[i];
                    break;
                }
            }
            
            // 说明不存在，那就直接返回
            if (card == null)
                return;

            // 从对应的卡牌List内移除
            _allCards.Remove(card);
            
            if (card.Type.Equals("Order"))
                _orderCards.Remove(card);
            else if (card.Type.Equals("Unit"))
                _unitCards.Remove(card);
        }
        
        /// <summary>
        /// 获取牌堆，从类内信息整理构造Deck类实例，目前没实现
        /// </summary>
        /// <returns></returns>
        public Deck GetDeck()
        {
            // TODO : 构造Deck实例
            return null;
        }
    }
}