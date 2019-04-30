using System;
using System.Collections.Generic;
using System.IO;
using IMessage;
using LitJson;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

using GameGUI;

namespace GameCard
{
    public class CardManager : UnitySingleton<CardManager>
    {
        #region 变量
        public GameObject[] tmpCardPrefabs;        // 存放暂用预制卡牌引用的数组
        public GameObject[] unitSlots;            // 卡牌槽物体的引用，仅用于初始化_unitsSlots
        private UnitSlot[] _unitSlots;            // 卡牌存放位置UnitSlot的引用

        private const int AMOUNT_OF_ROW = 6;      //panel每行可放置的卡牌数量，调整在这调
        private const int PANEL_WIDTH = 500;      //panel 宽度，调整在这调
        private const float CARD_WIDTH = 80.5f;      //card 宽度，调整在这调
        
        public GameObject coolDownSlot;        // 冷却牌物体的引用，仅用于初始化_coolDownSlots
        
        public GameObject cardsSetsSlot;        // 牌堆组物体的引用，仅用于初始化_cardsSetsSlots
        
        private List<CoolDownSlot> _coolDownSlots;     //冷却牌存放位置CoolDownSlot的引用
        
        private List<CardsSetsSlot> _cardsSetsSlots;     //牌堆组存放位置CardsSetsSlot的引用

        private GameObject _coolDownPanel;
        private GameObject _cardsSetsPanel;
        
        public int cardsUpperLimit { get; set; }                    // 手牌数量上限
        public int extractCardsUpperLimit { get; set; }            // 抽牌数量上限
        
        private List<GameObject> _cardsInHand;                    // 存储手牌列表，玩家实际持有的牌的预制件在这里面

        public List<GameObject> cardsInHand { get { return _cardsInHand; } }
        public List<GameObject> cardsSets { get; set; }            // 牌组堆，待抽取的卡牌组
        public List<GameObject> cooldownCards { get; set; }        // 临时存储冷却状态中卡牌
        public bool cancelCheck { get; set; }                      // 是否取消抽卡检查，在本行注释存在的情况下请不要修改值

        private Dictionary<string, JsonData> _cardsData;

        private Dictionary<GameObject, CoolDownSlot> _prefabToCoolDownSlots;
        #endregion
        
        private void Awake()
        {
            Init();
            LoadCardsIntoSets();
            cancelCheck = false;
            _coolDownPanel = GameObject.Find("CoolDownCardInfoPanel");
            _cardsSetsPanel = GameObject.Find("CardsSetsInfoPanel");
            _coolDownPanel.SetActive(false);
            _cardsSetsPanel.SetActive(false);
        }

        private void Start()
        {
            ExtractCards();
        }

        private void Init()
        {
            Debug.Log("in init");
            _cardsInHand = new List<GameObject>();
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
                JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/Scripts/Cards/cardSample.1.json"));

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
            Debug.Log("init_unit: " + unitSlots.Length);
            for (int i = 0; i < unitSlots.Length; i++)
            {
                _unitSlots[i] = unitSlots[i].GetComponent<UnitSlot>();

                
            }
            _coolDownSlots = new List<CoolDownSlot>();
            _cardsSetsSlots = new List<CardsSetsSlot>();

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

        /// <summary>
        /// 返回给定卡牌ID对应的卡牌ability id
        /// </summary>
        /// <param name="cardID">卡牌预制件内BaseCard存的ID</param>
        /// <returns>若存在此ID则返回对用的List，若不存在则返回null，若abilityid总数为0，则返回list为空</returns>
        public List<string> GetCardAbilityIDs(string cardID)
        {
            if (_cardsData.ContainsKey(cardID))
            {
                JsonData abilitys = _cardsData[cardID]["ability_id"];
                List<string> abilityIDs = new List<string>();
                for (int i = 0; i < abilitys.Count; i++)
                {
                    abilityIDs.Add(abilitys[i].ToString());
                }

                return abilityIDs;
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
            Debug.Log("点击抽卡");
            // 若手牌数量大于或等于手牌上限，直接返回（取消检查的话则此判定永false）
            if (_cardsInHand.Count >= cardsUpperLimit && !cancelCheck)
            {
                return;
            }

            // 计算应该抽取的卡牌数，计算规则：不检查=抽三张， 检查=不超出手牌数量上限的，最多三张牌
            int extractAmount = this.cancelCheck
                    ? this.extractCardsUpperLimit
                    : (this.cardsUpperLimit - this._cardsInHand.Count > extractCardsUpperLimit
                        ? extractCardsUpperLimit
                        : this.cardsUpperLimit - this._cardsInHand.Count);
            
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
                _unitSlots[_cardsInHand.Count].InsertItem(cardPrefab);
                
                // 更新手牌list
                this._cardsInHand.Add(cardPrefab);
            }
            Debug.Log("cardset: " + cardsSets.Count);
            UpdateCardsSetsInfo();
        }

        /// <summary>
        /// 从手牌中移除卡牌，用于外界通知CardManager手牌发生变动
        /// </summary>
        /// <param name="cardPrefab"></param>
        public void RemoveCard(GameObject cardPrefab)
        {
            // 从手牌列表中移除预制
            this._cardsInHand.Remove(cardPrefab);
            
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
            //Debug.Log(cardPrefab);
            // 读取数据库得到冷却回合数
            int roundAmount= (int)GetCardJsonData(cardPrefab.GetComponent<BaseCard>().id)["cd"];
            
            // 初始化BaseCard脚本内剩余回合数的counter
            cardPrefab.GetComponent<BaseCard>().cooldownRounds = roundAmount;
            
            //TODO: 不用加入冷却堆的卡牌不加入冷却list
            // 加入冷却list
            this.cooldownCards.Add(cardPrefab);
            Debug.Log("cool: " + cooldownCards.Count);
            PutInCoolDown(cardPrefab);                //每处理一个进入冷却状态的卡牌就放进冷却堆中
        }

        
        /// <summary>
        /// 重新调整卡牌槽中卡牌位置，消除空闲卡牌位
        /// </summary>
        public void ResortCardsInSlots()
        {
            int i = 0;
            // 先遍历前手牌数量个槽位
            for (; i < _cardsInHand.Count; i++)
            {
                // 若存在空栏
                if (_unitSlots[i].IsEmpty())
                {
                    // 跳出循环
                    break;
                }
            }
            
            // 如果i比手牌数量小，则一定存在空闲卡牌位
            if(i < _cardsInHand.Count)
            {
                // 将所有卡牌位清空
                for (int j = 0; j < _unitSlots.Length; j++)
                {
                    _unitSlots[j].RemoveItem(true);
                }

                // 再按手牌数量放入卡牌
                for (i = 0; i < _cardsInHand.Count; i++)
                {
                    _unitSlots[i].InsertItem(_cardsInHand[i]);
                }
            }
        }


        // 处理冷却牌堆，将冷却完毕的卡牌放回牌组中
        public void HandleCooldownEvent()
        {
            List<GameObject> prefabToRemove = new List<GameObject>();
            for (int i = 0; i < cooldownCards.Count; i++)
            {
                int leftRounds = cooldownCards[i].GetComponent<BaseCard>().cooldownRounds -= 1;
                if (leftRounds == 0)
                {
                    prefabToRemove.Add(cooldownCards[i]);
                }
            }

            for (int i = 0; i < prefabToRemove.Count; i++)
            {
                cardsSets.Add(prefabToRemove[i]);
                cooldownCards.Remove(prefabToRemove[i]);
                _coolDownSlots.Remove(_prefabToCoolDownSlots[prefabToRemove[i]]);
                
                _prefabToCoolDownSlots[prefabToRemove[i]].RemoveItem();
                
                Destroy(_prefabToCoolDownSlots[prefabToRemove[i]]);
                
                _prefabToCoolDownSlots.Remove(prefabToRemove[i]);

            }
            if (cardsSets.Count > _cardsSetsSlots.Count)
            {
                for (int j = 0; j < cardsSets.Count - _cardsSetsSlots.Count; j++)
                {
                    GameObject newCooldownSlot = Instantiate(cardsSetsSlot, _cardsSetsPanel.transform);
                    _cardsSetsSlots.Add(newCooldownSlot.GetComponent<CardsSetsSlot>());
                }
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



        
        /// <summary>
        /// 冷却堆按钮绑定方法，点击显示冷却堆卡牌信息界面
        /// </summary>
        public void ShowCoolDownCardInfo()
        {
            
            if (!_coolDownPanel.activeInHierarchy)
            {
                Debug.Log("点击冷却牌_显示");
                
                _cardsSetsPanel.SetActive(false);
                
                _coolDownPanel.SetActive(true);
            }
            else
            {
                Debug.Log("点击冷却牌_不显示");
                _coolDownPanel.SetActive(false);
            }
            
            
        }

        /// <summary>
        /// 冷却堆按钮绑定方法，点击显示牌组堆卡牌信息界面
        /// </summary>
        public void ShowCardsSetsInfo()
        {
            if (!_cardsSetsPanel.activeInHierarchy)
            {
                Debug.Log("点击牌堆组_显示");
                
                _coolDownPanel.SetActive(false);
                _cardsSetsPanel.SetActive(true);
            }
            else
            {
                Debug.Log("点击牌堆组_不显示");
                _cardsSetsPanel.SetActive(false);
            }
            
            
        }

        /// <summary>
        /// 负责把进入冷却的卡牌放进冷却堆中
        /// </summary>
        public void PutInCoolDown(GameObject cardPrefab)
        {
            setPanelSize(_coolDownPanel, cooldownCards.Count);
            
            if (_coolDownSlots.Count < cooldownCards.Count)        //slot中比冷却卡牌的个数小，说明有新的进入冷却状态的卡牌
            {
                int slotIndex = _coolDownSlots.Count;              //新加入的下标
                for (int i = 0; i < cooldownCards.Count - slotIndex; i++)
                {
                    GameObject newCooldownSlot = Instantiate(coolDownSlot, _coolDownPanel.transform);
                    _coolDownSlots.Add(newCooldownSlot.GetComponent<CoolDownSlot>());
                
                }
                for (int i = 0; i < cooldownCards.Count - slotIndex; i++)
                {
                    _prefabToCoolDownSlots.Add(cardPrefab, _coolDownSlots[slotIndex + i]);
                    Debug.Log("panel:" + _coolDownPanel.transform); 
                    _coolDownSlots[slotIndex + i].InsertItem(cooldownCards[slotIndex + i]);       //插入到冷却slot中
                    //_coolDownSlots[slotIndex + i].init(_coolDownPanel.transform); 
                }
            }
        }
        
        /// <summary>
        /// 根据卡牌数量设置对应panel的size
        /// </summary>
        /// <param name="panel">冷却堆或者牌组堆对应 panel</param>
        /// <param name="amount">冷却堆卡牌数组或者牌组堆数组对应卡牌数量</param>
        private void setPanelSize(GameObject panel, int amount)
        {
            int height = (amount + AMOUNT_OF_ROW - 1) / AMOUNT_OF_ROW;
            
            RectTransform rectTransform = panel.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(PANEL_WIDTH, height * CARD_WIDTH);
        }
        
        
        /// <summary>
        /// 更新牌组堆信息，用于 panel 显示
        /// </summary>
        public void UpdateCardsSetsInfo()
        {
            setPanelSize(_cardsSetsPanel, cardsSets.Count);

            for (int i = 0; i < _cardsSetsSlots.Count; i++)     //删除slot里面原有的所有元素
            {
                _cardsSetsSlots[i].RemoveItem();
                Destroy(_cardsSetsSlots[i].gameObject);        
            }
            _cardsSetsSlots.Clear();
            for (int i = 0; i < cardsSets.Count; i++)        //重新添加进去，以实现卡牌堆紧凑
            {
                GameObject newCooldownSlot = Instantiate(cardsSetsSlot, _cardsSetsPanel.transform);
                _cardsSetsSlots.Add(newCooldownSlot.GetComponent<CardsSetsSlot>());
            }

            for (int i = 0; i < cardsSets.Count; i++)
            {
                _cardsSetsSlots[i].InsertItem(cardsSets[i]);
                
            }
        }
    }
}