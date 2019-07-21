using System;
using System.Collections.Generic;
using System.Linq;
using GamePlay;
using IMessage;
using Mediator;
using UnityEngine;

namespace GameCard
{
    /// <summary>
    /// 用于指定操作的卡牌类型
    /// </summary>
    public enum CardDesignation
    {
        /// <summary>
        /// 手牌内的牌
        /// </summary>
        HandCard,
        /// <summary>
        /// 牌堆内的牌
        /// </summary>
        DeckCard,
        /// <summary>
        /// 待命区的牌
        /// </summary>
        StandByCard
    }
    public class HandCardManager : UnitySingleton<HandCardManager>
    {
        #region 变量

        /// <summary>
        /// 手牌上线数
        /// </summary>
        private int _cardsUpperLimit;
        /// <summary>
        /// 抽牌上限数
        /// </summary>
        private int _extractCardSUpperLimit;

        /// <summary>
        /// 记录手牌组当前操作，true表示被锁定，无法使用
        /// </summary>
        private bool _lockStatus;

        /// <summary>
        /// 手牌实例对象
        /// </summary>
        private List<_NewBaseCard> _handCards;

        /// <summary>
        /// 待命区
        /// </summary>
        private List<_NewBaseCard> _standBy;
        /// <summary>
        /// 每回合的卡牌堆对象引用
        /// </summary>
        private Deck _deck;
        
        

        /// <summary>
        /// 当前选中的卡牌
        /// </summary>
        private _NewBaseCard _currentSelectingCard;
        /// <summary>
        /// 当前选中的卡牌的位置
        /// </summary>
        private int _currentSelectingPos;

        #endregion

        
        #region 变量可见性定义

        /// <summary>
        /// 当前选中的卡牌，若未选中则会返回空值
        /// </summary>
        public _NewBaseCard CurrentSelectingCard
        {
            get
            {
                if (_currentSelectingCard == null || _currentSelectingPos < 0)
                    return null;
                return _currentSelectingCard;
            }
        }
        
        /// <summary>
        /// 是否处于锁定状态
        /// </summary>
        public bool LockStatus
        {
            get { return _lockStatus; }
            set { _lockStatus = value; }
        }

        #endregion

        private void Start()
        {
            // 默认锁定
            _lockStatus = true;

            _cardsUpperLimit = 7;
            _extractCardSUpperLimit = 1;
            _currentSelectingPos = -1;
            _currentSelectingCard = null;
            
            // 从牌堆管理中请求牌堆
            _deck = DeckController.Instance().GetDeck();
            
            _handCards = new List<_NewBaseCard>();
            _standBy = new List<_NewBaseCard>();
        }

        /// <summary>
        /// 设置当前选中的卡牌
        /// </summary>
        /// <param name="selectPos"></param>
        public void SetSelectCard(int selectPos)
        {
            // 当手牌数量大于selectPos时，可以正常设置卡牌位置，因为此时Count是上界，永远不等于手牌最大位置下标
            if (selectPos >= 0 && _handCards.Count > selectPos)
            {
                _currentSelectingPos = selectPos;
                _currentSelectingCard = _handCards[selectPos];
            }
            // 若手牌数量小于selectPos则越界，若手牌数量与selectPos相等
            // 则要么有手牌，此时selectPos越上界，要么无手牌，此时0下标也越界
            else
            {
                _currentSelectingPos = -1;
                _currentSelectingCard = null;
            }
        }

        /// <summary>
        /// 用于对接手牌点击到FSM的中间函数，仅在参数正确，且未锁定时会起到传递作用
        /// </summary>
        /// <param name="selectPos">点击的手牌下标，默认-1表示不做反应</param>
        public void OnClickCard(int selectPos = -1)
        {
            // 如果处于锁定状态，直接返回
            if (_lockStatus)
                return;
            
            // 调用函数完成_currentSelectingCard对象的设置及对应的检查
            SetSelectCard(selectPos);

            // 传入的参数非法，那设置完之后_currentSelectingCard就会为null，一旦发生，直接返回
            if (_currentSelectingCard == null)
                return;

            // 点击后动作交由FSM处理，TODO: 改成新的类型_NewBaseCard
            //Gameplay.Instance().gamePlayInput.OnPointerDownCard(_currentSelectingCard);
        }

        
        /// <summary>
        /// 从牌组中抽取卡牌到手牌中
        /// </summary>
        /// <param name="cardAmount">抽取卡牌数量，默认为一</param>
        public void ExtractCards(int cardAmount = 1, bool cancelCheck = false)
        {
            // 若手牌数量大于或等于手牌上限，直接返回（取消检查的话则此判定永false）
            if (_handCards.Count >= _cardsUpperLimit && !cancelCheck)
            {
                return;
            }

            // 根据参数确定抽取卡牌的数量,若和抽牌上限一致则使用抽牌上限，否则使用给定参数
            int extractAmount = (cardAmount >= _extractCardSUpperLimit) ? _extractCardSUpperLimit : cardAmount;

            // 计算应该抽取的卡牌数，计算规则：不检查=抽cardAmount张， 检查=不超出手牌数量上限的，最多cardAmount张牌
            extractAmount = cancelCheck ? cardAmount : 
                (_cardsUpperLimit - _handCards.Count > extractAmount ? 
                    extractAmount : _cardsUpperLimit - _handCards.Count);
            
            // 如果剩余牌量不足，有多少抽多少（几乎不可能）
            if (extractAmount > _deck._deck.Count)
                extractAmount = _deck._deck.Count;
            
            
            // 按照序列抽取卡牌
            for (int i = 0; i < extractAmount; i++)
            {
                // 获得对应卡牌的id
                _NewBaseCard card= _deck._deck[i];
                
                // 将其从卡牌堆中移除
                _deck._deck.Remove(card);
                
                // 向手牌中插入卡牌操作
                _handCards.Add(card);
            }
            
            // 发送手牌变动消息
            MsgDispatcher.SendMsg((int)MessageType.HandcardChange);
            // 发送牌堆变化消息
            MsgDispatcher.SendMsg((int)MessageType.CardsetChange);
            // 发送卡牌抽取消息
            MsgDispatcher.SendMsg((int)MessageType.DrawCard);
        }

        /// <summary>
        /// 获取指定卡牌的所有卡牌图片ID，UI那边拿去直接可以装载图片的
        /// </summary>
        /// <param name="cardDesignation">要获取的卡牌</param>
        /// <param name="container">用于容纳卡牌ID的容器，会被清空再装入新东西</param>
        public void GetCardImageIds(CardDesignation cardDesignation, List<string> container)
        {
            List<_NewBaseCard> targetCard = null;

            // 根据指定类型，获取指定的卡牌区
            switch (cardDesignation)
            {
                case CardDesignation.DeckCard:    // 牌堆
                    targetCard = _deck._deck;
                    break;
                case CardDesignation.HandCard:    // 手牌
                    targetCard = _handCards;
                    break;
                case CardDesignation.StandByCard:    // 待命区
                    targetCard = _standBy;
                    break;
                default:    // dnmd遇到奇葩值直接结束
                    return;
            }
            
            // 清空容器
            container.Clear();
            
            // 挨个解析对应去内的卡牌ID,基本就是去掉_以及其后的下标数字
            foreach (_NewBaseCard newBaseCard in targetCard)
            {
                container.Add(newBaseCard.Id.Split('_').First());
            }
            
            if(cardDesignation.Equals(CardDesignation.DeckCard))
            {
                container.Sort();
            }
        }

        /// <summary>
        /// 指定卡牌，卡牌所属区，行为对卡牌进行从指定区域移除或添加到指定区域
        /// 以上操作均在函数内进行合法性检验，若不合法则直接结束函数执行
        /// </summary>
        /// <param name="cardObject">要操作的卡牌实例</param>
        /// <param name="cardDesignation">对应的卡牌区域</param>
        /// <param name="insert">true表示插入操作，false表示移除</param>
        public void OperateCard(_NewBaseCard cardObject, CardDesignation cardDesignation, bool insert = true)
        {
            List<_NewBaseCard> targetCardSets = null;
            
            // 根据指定类型，获取指定的卡牌区
            switch (cardDesignation)
            {
                case CardDesignation.DeckCard:
                    targetCardSets = _deck._deck;    // 牌堆
                    break;
                case CardDesignation.HandCard:
                    targetCardSets = _handCards;    // 手牌
                    break;
                case CardDesignation.StandByCard:
                    targetCardSets = _standBy;        // 待命区
                    break;
                default:
                    return;
            }

            // 根据insert值进行操作
            if (insert)    // 插入操作，默认尾插，有需要再改咯
                targetCardSets.Add(cardObject);
            else if ( ! targetCardSets.Remove(cardObject))    // 进行移除再根据移除结果进行判断
            {
                return;    // 移除失败，那就直接返回，毕竟没变动
            }

            int message = 0;
            
            // 根据选择的卡牌区，设置不同信号
            switch (cardDesignation)
            {
                case CardDesignation.DeckCard:
                    // TODO: 以后应该要给改个名，现在牌堆叫deck
                    message = (int) MessageType.CardsetChange;
                    break;
                case CardDesignation.HandCard:
                    message = (int) MessageType.HandcardChange;
                    break;
                case CardDesignation.StandByCard:
                    // TODO: 添加新的卡牌变动信号
                    //message = (int) MessageType.StandByChange;
                    break;
            }
            
            // 发出对应的变动信号
            MsgDispatcher.SendMsg(message);
        }

        
        /// <summary>
        /// 特别专用的函数，就是把待命区的卡牌移动回牌堆里面，仅此而已
        /// </summary>
        public void MoveCardsInStandByToDeck()
        {
            // 如果是空的，那就什么也不用做咯
            if (_standBy.Count == 0)
                return;

            // 把每一张卡牌放入牌堆内
            foreach (_NewBaseCard newBaseCard in _standBy)
            {
                _deck._deck.Add(newBaseCard);
            }
            
            // 清空待命区
            _standBy.Clear();
        }
        
    }
}