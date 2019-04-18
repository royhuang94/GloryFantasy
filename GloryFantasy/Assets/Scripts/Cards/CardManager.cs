using System;
using System.Collections.Generic;
using System.IO;
using LitJson;
using NBearUnit;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace GameUnit
{
    public class CardManager : MonoBehaviour
    {
        #region 变量
        public GameObject[] tmpCardPrefabs;        // 存放暂用预制卡牌引用的数组
        public GameObject[] unitSlots;            // 卡牌槽物体的引用，仅用于初始化_unitsSlots
        private UnitSlot[] _unitSlots;            // 卡牌存放位置UnitSlot的引用
        
        public int cardsUpperLimit { get; set; }                    // 手牌数量上限
        public int extractCardsUpperLimit { get; set; }            // 抽牌数量上限
        public List<GameObject> cardsInstancesInHand { get; set; }// 存储手牌列表，玩家实际持有的牌的预制件在这里面
        public List<GameObject> cardsSets { get; set; }            // 牌组堆，待抽取的卡牌组
        public List<GameObject> cooldownCards { get; set; }        // 临时存储冷却状态中卡牌
        public bool cancelCheck { get; set; }                      // 是否取消抽卡检查，在本行注释存在的情况下请不要修改值

        private Dictionary<string, JsonData> _cardsData;
        #endregion
        
        #region 简单单例模式
        private static CardManager _instance = null;
        private CardManager()
        {
            
        }
        
        public static CardManager GetInstance()
        {
            return _instance;
        }
        #endregion
        private void Awake()
        {
            _instance = this;
            Init();
            LoadCardsIntoSets();
            cancelCheck = false;
        }

        private void Start()
        {
            ExtractCards();
        }

        private void Init()
        {
            cardsInstancesInHand = new List<GameObject>();
            cardsSets = new List<GameObject>();
            cooldownCards = new List<GameObject>();

            cardsUpperLimit = 10;
            extractCardsUpperLimit = 3;
            
            InitUnitSlots();
            InitCardsData();
        }

        /// <summary>
        /// 初始化存储所有卡牌的数据字典
        /// id -> json
        /// </summary>
        private void InitCardsData()
        {
            _cardsData = new Dictionary<string, JsonData>();

            JsonData cardsJsonData =
                JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/Scripts/Cards/cardSample.json"));

            int dataAmount = cardsJsonData.Count;
            for (int i = 0; i < dataAmount; i++)
            {
                _cardsData.Add(cardsJsonData[i]["id"].ToString(), cardsJsonData[i]);
            }
        }

        
        /// <summary>
        /// 初始化_unitSlots引用数组
        /// </summary>
        private void InitUnitSlots()
        {
            _unitSlots = new UnitSlot[unitSlots.Length];
            for (int i = 0; i < unitSlots.Length; i++)
            {
                _unitSlots[i] = unitSlots[i].GetComponent<UnitSlot>();
                
            }
        }


        /// <summary>
        /// 返回给定的ID对应的Json数据
        /// </summary>
        /// <param name="cardID">卡牌预制件保存的UnitCard内存储的ID</param>
        /// <returns>若存在此ID则返回对应的Json数据，若不存在则返回null</returns>
        public JsonData GetCardJsonData(string cardID)
        {
            if (_cardsData.ContainsKey(cardID))
            {
                return _cardsData[cardID];
            }

            return null;
        }

        private void LoadCardsIntoSets()
        {
            // TODO: 根据策划案修改此函数，以下仅用于demo
            for (int i = 0; i < 24; i++)
            {
                // 随机把prefab放入牌组中，实际应按照策划案或玩家数据
                cardsSets.Add(tmpCardPrefabs[Random.Range(0, tmpCardPrefabs.Length)]);
            }
        }

        /// <summary>
        /// 从牌组中抽取卡牌到手牌中
        /// </summary>
        public void ExtractCards()
        {
            // 若手牌数量大于或等于手牌上限，直接返回（取消检查的话则此判定永false）
            if (cardsInstancesInHand.Count >= cardsUpperLimit && !cancelCheck)
            {
                return;
            }

            // 计算应该抽取的卡牌数，计算规则：不检查=抽三张， 检查=不超出手牌数量上限的，最多三张牌
            int extractAmount = this.cancelCheck
                    ? this.extractCardsUpperLimit
                    : (this.cardsUpperLimit - this.cardsInstancesInHand.Count > extractCardsUpperLimit
                        ? extractCardsUpperLimit
                        : this.cardsUpperLimit - this.cardsInstancesInHand.Count);
            
            // 如果剩余牌量不足，有多少抽多少（几乎不可能）
            if (extractAmount > this.cardsSets.Count)
                extractAmount = this.cardsSets.Count;
            
            
            for (int i = 0; i < extractAmount; i++)
            {
                // 随机抽取卡牌
                int extractPos = Random.Range(0, cardsSets.Count);
                GameObject cardPrefab = this.cardsSets[extractPos];
                
                // 确定是哪张后就将其从卡牌堆中移除
                this.cardsSets.RemoveAt(extractPos);
                
                // 将卡牌预制件放入栏位中
                _unitSlots[cardsInstancesInHand.Count].InsertItem(cardPrefab);
                
                // 更新手牌list
                this.cardsInstancesInHand.Add(cardPrefab);
            }
        }

        /// <summary>
        /// 从手牌中移除卡牌，用于外界通知CardManager手牌发生变动
        /// </summary>
        /// <param name="cardPrefab"></param>
        public void RemoveCard(GameObject cardPrefab)
        {
            // 从手牌列表中移除预制
            this.cardsInstancesInHand.Remove(cardPrefab);
            
            // 将其加入冷却列表进行冷却
            this.CooldownCard(cardPrefab);
            
            // 重设手牌位置
            ResortCardsInSlots();
        }

        /// <summary>
        /// 将预存的预制件放入冷却列表中，仅CardManager内部使用
        /// </summary>
        /// <param name="cardPrefab">要冷却的卡牌的预制件引用</param>
        private void CooldownCard(GameObject cardPrefab)
        {
            // 读取数据库得到冷却回合数
            int roundAmount= (int)GetCardJsonData(cardPrefab.GetComponent<BaseCard>().id)["cd"];
            
            // 初始化BaseCard脚本内剩余回合数的counter
            cardPrefab.GetComponent<BaseCard>().cooldownRounds = roundAmount;
            
            // 加入冷却list
            this.cooldownCards.Add(cardPrefab);
        }

        
        /// <summary>
        /// 重新调整卡牌槽中卡牌位置，消除空闲卡牌位
        /// </summary>
        public void ResortCardsInSlots()
        {
            int i = 0;
            // 先遍历前手牌数量个槽位
            for (; i < cardsInstancesInHand.Count; i++)
            {
                // 若存在空栏
                if (_unitSlots[i].IsEmpty())
                {
                    // 跳出循环
                    break;
                }
            }
            
            // 如果i比手牌数量小，则一定存在空闲卡牌位
            if(i < cardsInstancesInHand.Count)
            {
                // 将所有卡牌位清空
                foreach (UnitSlot slot in _unitSlots)
                {
                    slot.RemoveItem();
                }

                // 再按手牌数量放入卡牌
                for (i = 0; i < cardsInstancesInHand.Count; i++)
                {
                    _unitSlots[i].InsertItem(cardsInstancesInHand[i]);
                }
            }
        }


        // 处理冷却牌堆，将冷却完毕的卡牌放回牌组中
        public void HandleCooldownEvent()
        {
            List<int> pos = new List<int>();
            for (int i = 0; i < cooldownCards.Count; i++)
            {
                int leftRounds = cooldownCards[i].GetComponent<BaseCard>().cooldownRounds -= 1;
                if (leftRounds == 0)
                {
                    pos.Add(i);
                }
            }

            for (int i = 0; i < pos.Count; i++)
            {
                cardsSets.Add(cooldownCards[pos[i]]);
                cooldownCards.RemoveAt(pos[i]);
            }
        }

        // 随机交换牌组中位置实现洗牌
        public void Shuffle()
        {
            int size = cardsSets.Count;
            
            for (int i = 0; i < size; i++)
            {
                int pos = Random.Range(0, size);
                GameObject temp = cardsSets[i];
                cardsSets[i] = cardsSets[pos];
                cardsSets[pos] = temp;
            }
        }
        
        // 回合结束被调用的接口，处理回合结束应处理的事务
        public void OnNotifyRoundEnd()
        {
            HandleCooldownEvent();
        }

        public void OnNotifyRoundStart()
        {
            
        }

        
    }
}