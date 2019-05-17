using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MainMap;
using GameCard;
using UnityEngine.UI;
using LitJson;
using System.IO;
using FairyGUI;

namespace GameGUI
{/// <summary>以下所有代码都是为了测试和演示，所以会略草率= = 
/// 
/// </summary>
    public class MainMapUI : UnitySingleton<MainMapUI>
    {
        GameObject TestUI;
        private string MainMapUIPackage = "MainMapFairyGUIPackage/MainMapUI";
        private string CardCollectionPackage = "MainMapFairyGUIPackage/CardCollection";
        private string CardIconPackage = "MainMapFairyGUIPackage/CardIcon";
        private const string cardicons = "CardIcon";
        public GComponent mainmapUI;
        private GComponent libraryUI;
        private GComponent cardcollectUI;
        private Window cardcollect_UI;
        private GameObject postUI;
        //Todo: delete this
        public GameObject Map;
        private GButton ccbtn;
        private GButton closebtn;
        private GLoader _cardloader;
        private GList _cardsSetsList;
        private List<string> cardSetsList;
        private GTextField _abstractText;
        private GTextField _storyText;
        private GLoader _iconLoader;
        private GLoader _picLoader;
        private GComponent _cardDisplayer;

        private void Awake()
        {
            GRoot.inst.SetContentScaleFactor(960, 540);
            UIPackage.AddPackage(MainMapUIPackage);
            UIPackage.AddPackage(CardCollectionPackage);
            UIPackage.AddPackage(CardIconPackage);
            mainmapUI = UIPackage.CreateObject("MainMapUI", "Component2").asCom;
            cardcollectUI = UIPackage.CreateObject("CardCollection", "CardBookMain").asCom;
            GRoot.inst.AddChild(mainmapUI);
            ccbtn = mainmapUI.GetChild("CardCollectionBtn").asButton;
            ccbtn.onClick.Add(() => ShowCardCollect());
            Map = GameObject.FindGameObjectWithTag("Map");
            cardcollect_UI = new Window();
            cardcollect_UI.contentPane=cardcollectUI;
            cardcollect_UI.CenterOn(GRoot.inst, true);
            _cardsSetsList = cardcollectUI.GetChild("cardList").asList;
            _cardDisplayer = cardcollectUI.GetChild("cardDisplayer").asCom;
            _abstractText = _cardDisplayer.GetChild("abstractText").asTextField;
            _storyText = _cardDisplayer.GetChild("storyText").asTextField;
            _iconLoader = _cardDisplayer.GetChild("iconLoader").asLoader;
            _picLoader = _cardDisplayer.GetChild("cardPicLoader").asLoader;
            Debug.Log("ui初始化");
            
        }
        private void Start()
        {
            cardSetsList = CardCollection.Instance().mycollection;

            _cardsSetsList.onClickItem.Add(OnClickCardInCardSets);
            GetCards();
        }
        /// <summary>点击驿站地格后调用此方法展示驿站UI;
        /// 
        /// </summary>
        public  void ShowPostUI(Post post)
        {
            postUI.SetActive(true);
        }
        public  void DealOnClick()
        {
            GetCards();
        }
        /// <summary>获取三张卡牌信息
        /// 
        /// </summary>
        public  void GetCards()
        {
            JsonData cardsJsonData =
            JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/Scripts/Cards/cardSample.1.json"));
            //因为赶时间写的超烂，以后会改0.0
            int dataAmount = cardsJsonData.Count;
            int num1 = Random.Range(0, dataAmount);
            int num2 = Random.Range(0, dataAmount);
            int num3 = Random.Range(0, dataAmount);
          //  JsonData Card1 = CardManager.Instance().GetCardJsonData(cardsJsonData[num1]["id"].ToString());
          //  JsonData Card2 = CardManager.Instance().GetCardJsonData(cardsJsonData[num2]["id"].ToString());
          //  JsonData Card3 = CardManager.Instance().GetCardJsonData(cardsJsonData[num3]["id"].ToString());
            ShowCard(cardsJsonData[num1]["id"].ToString(), cardsJsonData[num2]["id"].ToString(), cardsJsonData[num3]["id"].ToString());
            CardCollection.Instance().mycollection.Add(cardsJsonData[num1]["id"].ToString());
            CardCollection.Instance().mycollection.Add(cardsJsonData[num2]["id"].ToString());
            CardCollection.Instance().mycollection.Add(cardsJsonData[num3]["id"].ToString());
        }
        /// <summary>点击传送时，调用此方法
        /// 
        /// </summary>
        public  void TransOnClick()
        {
            Post.PrepareTrans();
        }
        /// <summary>点击离开时，调用此方法
        /// 
        /// </summary>
        public  void LeaveOnClick()
        {
            postUI.SetActive(false);
        }
        /// <summary>展示三张卡牌，
        /// 
        /// </summary>
        /// <param name="card1"></param>
        /// <param name="card2"></param>
        /// <param name="card3"></param>
        public  void ShowCard(string card1,string card2,string card3)
        {
            //TODO:在ui上显示三张卡牌的信息
            Debug.Log("ThreeCards:" + card1 + "," + card2 + "," + card3);


        }
        /// <summary>展示卡牌收藏
        /// 
        /// </summary>
        public void ShowCardCollect()
        {
            cardcollect_UI.Show();
            Debug.Log("展示卡牌收藏");
            //TODO：ui调用setactive会导致性能问题，后期要改其他实现方式！！2019.5.8
            Map.SetActive(false);
            _cardsSetsList.RemoveChildren(0, -1, true);

            foreach (string cardId in cardSetsList)
            {
                GObject item = UIPackage.CreateObject("CardCollection", "cardsSetsItem");
                item.icon = UIPackage.GetItemURL(cardicons, cardId);
                _cardsSetsList.AddChild(item);
            }
            closebtn = cardcollect_UI.contentPane.GetChild("Close").asButton;
            closebtn.onClick.Add(() => CloseCardCollect());

        }
        /// <summary>关闭卡牌收藏
        /// 
        /// </summary>
        public void CloseCardCollect()
        {

            Map.SetActive(true);
            cardcollect_UI.Hide();
            //TODO：ui调用setactive会导致性能问题，后期要改其他实现方式！！2019.5.8
            Debug.Log("卡牌收藏关闭");
        }
        /// <summary>
        /// 响应卡牌堆内卡牌点击事件的函数
        /// </summary>
        /// <param name="context"></param>
        public void OnClickCardInCardSets(EventContext context)
        {
            // 先获取到点击的下标
            int index = _cardsSetsList.GetChildIndex(context.data as GObject);

            // 通过下标获取到id
            string cardId = cardSetsList[index];
            // 向数据库查询展示数据
            JsonData data = CardManager.Instance().GetCardJsonData(cardId);

            _abstractText.text = "姓名：" + data["name"] + "\n" + "类型：" + data["type"];

            _storyText.text = "这里本来该有卡牌故事但是现在没有数据\n" + data["effect"];

            // TODO: 根据策划案加载icon

            _picLoader.url = UIPackage.GetItemURL(cardicons, cardId);

        }
    }
}

