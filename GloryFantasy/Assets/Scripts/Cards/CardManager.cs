using System;
using System.Collections.Generic;
using System.IO;
using LitJson;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace GameUnit
{
    public class CardManager : MonoBehaviour
    {
        private static CardManager _instance = null;
        
        // 存放暂用预制卡牌引用的数组
        public GameObject[] tmpCardPrefabs;
        // 手牌数量上限
        public int cardsUpperLimit { get; set; }
        // 抽牌数量上限
        public int extractCardsUpperLimit { get; set; }
        // 存储手牌实例列表，玩家实际持有的牌在这里面
        public List<GameObject> cardsInstancesInHand { get; set; }
        // 弃牌堆，所有用过消失的，丢弃的牌在这里面
        public List<GameObject> removedCards { get; set; }
        // 牌组堆，待抽取的卡牌组
        public List<GameObject> cardsSets { get; set; }
        // 临时存储冷却状态中卡牌
        public List<GameObject> cooldownCards { get; set; }
        
        // 是否取消抽卡检查
        public bool cancelCheck { get; set; }

        private Dictionary<string, JsonData> _cardsData;

        private GameObject panel;
        
        private CardManager()
        {
            
        }
        
        public static CardManager GetInstance()
        {
            return _instance;
        }

        private void Awake()
        {
            _instance = this;
            Init();
            LoadCardsIntoSets();
            cancelCheck = false;
        }

        private void Start()
        {
            panel = GameObject.Find("UnitUI/Panel").gameObject;
            ExtractCards();
        }

        private void Init()
        {
            cardsInstancesInHand = new List<GameObject>();
            removedCards = new List<GameObject>();
            cardsSets = new List<GameObject>();
            cooldownCards = new List<GameObject>();

            cardsUpperLimit = 10;
            extractCardsUpperLimit = 3;
            
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

        // 从牌组中抽取卡牌到手牌中， 只有手牌中保存卡牌实例，其他list中均是预制件的引用
        public void ExtractCards()
        {
            if (this.cardsInstancesInHand.Count >= this.cardsUpperLimit && !this.cancelCheck)
            {
                return;
            }

            int extractAmount = this.cancelCheck
                    ? this.extractCardsUpperLimit
                    : (this.cardsUpperLimit - this.cardsInstancesInHand.Count > extractCardsUpperLimit
                        ? extractCardsUpperLimit
                        : this.cardsUpperLimit - this.cardsInstancesInHand.Count);
            
            
            //Debug.Log(String.Format("Extracting {0} card", extractAmount));
            if (extractAmount > this.cardsSets.Count)
                extractAmount = this.cardsSets.Count;
            
            for (int i = 0; i < extractAmount; i++)
            {
                int extractPos = Random.Range(0, cardsSets.Count);
                GameObject toInstantiate = this.cardsSets[extractPos];
                this.cardsSets.RemoveAt(extractPos);
                GameObject newCard = Instantiate(toInstantiate);
                newCard.transform.SetParent(panel.transform);
                newCard.GetComponent<BaseCard>().cardPrefabs = toInstantiate;
                this.cardsInstancesInHand.Add(newCard);
            }
        }

        public void RemoveCard(GameObject cardInstance)
        {
            this.cardsInstancesInHand.Remove(cardInstance);
            GameObject cardPrefab = cardInstance.GetComponent<BaseCard>().cardPrefabs;
            this.removedCards.Add(cardPrefab);
            Destroy(cardInstance);
        }

        // 将给定手牌实例销毁，并将预存的预制件放入冷却列表中
        public void CooldownCard(GameObject cardInstance, int roundAmount)
        {
            this.cardsInstancesInHand.Remove(cardInstance);
            GameObject cardPrefab = cardInstance.GetComponent<BaseCard>().cardPrefabs;
            cardPrefab.GetComponent<BaseCard>().cooldownRounds = roundAmount;
            this.cooldownCards.Add(cardPrefab);
            Destroy(cardInstance);
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