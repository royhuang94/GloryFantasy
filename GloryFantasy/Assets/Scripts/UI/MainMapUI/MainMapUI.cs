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
{/// <summary>
/// 
/// </summary>
    public class MainMapUI : UnitySingleton<MainMapUI>
    {
        /// <summary>测试用
        /// 
        /// </summary>
        public GameObject mapcamera;
        GameObject TestUI;
        #region 大地图FairyGUI素材包
        private string MainMapUIPackage = "MainMapFairyGUIPackage/MainMapUI";
        private string CardCollectionPackage = "MainMapFairyGUIPackage/CardCollection";
        private string CardIconPackage = "MainMapFairyGUIPackage/CardIcon";
        private string LibraryPackage = "MainMapFairyGUIPackage/Library";
        #endregion
        #region 大地图的GCompoment 和window
        private GComponent mainmapUI;
        private GComponent libraryUI;
        private GComponent cardcollectUI;
        private Window cardcollect_UI;
        private Window library_UI;
        private GComponent _cardDisplayer;
        #endregion
        #region 按钮，文本,装载器等
        private GButton ccbtn;
        //private GButton closebtn;
        private GLoader _cardloader;
        private GList cardcollectionlist;
        private GList onsalelist;
        private GTextField _abstractText;
        private GTextField _storyText;
        private GLoader _iconLoader;
        private GLoader _picLoader;
        #endregion
        /// <summary>卡牌收藏链表，对CardCollection的引用
        /// //Todo:为啥这么写呢。。？
        /// 
        /// </summary>
        private List<string> playercardlist;
        /// <summary>图书馆正销售的卡牌链表
        /// 
        /// </summary>
        private List<string> librarylist = new List<string>();
        //URL
        private const string cardicons = "CardIcon";
        /// <summary>初始化
        /// 
        /// </summary>
        private void Awake()
        {
            GRoot.inst.SetContentScaleFactor(960, 540);
            UIPackage.AddPackage(MainMapUIPackage);
            UIPackage.AddPackage(CardCollectionPackage);
            UIPackage.AddPackage(CardIconPackage);
            UIPackage.AddPackage(LibraryPackage);
            mainmapUI = UIPackage.CreateObject("MainMapUI", "Component2").asCom;
            cardcollectUI = UIPackage.CreateObject("CardCollection", "CardBookMain").asCom;
            cardcollect_UI = new Window();
            cardcollect_UI.contentPane = cardcollectUI;
            cardcollect_UI.CenterOn(GRoot.inst, true);
            libraryUI = UIPackage.CreateObject("Library", "LibraryMain").asCom;
            library_UI = new Window();
            library_UI.contentPane = libraryUI;
            library_UI.CenterOn(GRoot.inst, true);
            GRoot.inst.AddChild(mainmapUI);
            #region 初始化按钮装载器文本等
            ccbtn = mainmapUI.GetChild("CardCollectionBtn").asButton;
            ccbtn.onClick.Add(() => ShowCardCollect());
            cardcollectionlist = cardcollectUI.GetChild("cardList").asList;
            onsalelist = libraryUI.GetChild("LibraryCardList").asList;
            _cardDisplayer = cardcollectUI.GetChild("cardDisplayer").asCom;
            _abstractText = _cardDisplayer.GetChild("abstractText").asTextField;
            _storyText = _cardDisplayer.GetChild("storyText").asTextField;
            _iconLoader = _cardDisplayer.GetChild("iconLoader").asLoader;
            _picLoader = _cardDisplayer.GetChild("cardPicLoader").asLoader;
            #endregion
            Debug.Log("ui初始化");
            
        }
        private void Start()
        {
            playercardlist = CardCollection.Instance().mycollection;
            cardcollectionlist.onClickItem.Add(OnClickCardInCardCollection);
            onsalelist.onClickItem.Add(OnClickCardInLibrary);
            GetCards();
        }
        #region 图书馆相关代码
        /// <summary>点击图书馆地格后调用此方法展示图书馆UI并作初始化工作;
        /// 
        /// </summary>
        public void ShowlibraryUI(Library library)
        {
            mapcamera.SetActive(false);
            library_UI.Show();
            ShowCard();
            GButton closebtn = library_UI.contentPane.GetChild("Close").asButton;
            closebtn.onClick.Add(() => LeaveOnClick());
        }
        public  void DealOnClick()
        {
            GetCards();
        }
        /// <summary>获取三张卡牌信息,并写入librarylist;
        /// 
        /// </summary>
        public  void GetCards()
        {
            JsonData cardsJsonData =
            JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/Scripts/Cards/cardSample.1.json"));
            int dataAmount = cardsJsonData.Count;
            int num1 = Random.Range(0, dataAmount);
            int num2 = Random.Range(0, dataAmount);
            int num3 = Random.Range(0, dataAmount);
            librarylist.Add(cardsJsonData[num1]["id"].ToString());
            librarylist.Add(cardsJsonData[num2]["id"].ToString());
            librarylist.Add(cardsJsonData[num3]["id"].ToString());
        }
        /// <summary>点击传送时，调用此方法
        /// 
        /// </summary>
        public  void TransOnClick()
        {
            Library.PrepareTrans();
        }
        /// <summary>点击离开时，调用此方法
        /// 
        /// </summary>
        public  void LeaveOnClick()
        {
            library_UI.Hide();
            mapcamera.SetActive(true);
        }
        /// <summary>展示三张卡牌，
        /// 
        /// </summary>
        public  void ShowCard()
        {
            onsalelist.RemoveChildren();
            foreach (string cardID in librarylist)
            {
                GObject item = UIPackage.CreateObject("Library", "CardItem");
                item.icon = UIPackage.GetItemURL(cardicons, cardID);
                onsalelist.AddChild(item);
            }
        }
        /// <summary>图书馆内卡牌的点击事件
        /// 
        /// </summary>
        /// <param name="context"></param>
        public void OnClickCardInLibrary(EventContext context)
        {
            int index = onsalelist.GetChildIndex(context.data as GObject);
            string cardId = librarylist[index];
            if (CardCollection.Instance().AddCard(cardId))
            {
                Debug.Log("购买成功！");
                librarylist.Remove(cardId);
            }

            onsalelist.RemoveChildAt(index, true);


        }
        #endregion
        #region 卡牌书相关代码
        /// <summary>展示卡牌收藏
        /// 
        /// </summary>
        public void ShowCardCollect()
        {
            cardcollect_UI.Show();
            Debug.Log("展示卡牌收藏");
            //TODO：隐藏地格渲染
            mapcamera.SetActive(false);
            cardcollectionlist.RemoveChildren(0, -1, true);

            foreach (string cardId in playercardlist)
            {
                GObject item = UIPackage.CreateObject("CardCollection", "cardsSetsItem");
                item.icon = UIPackage.GetItemURL(cardicons, cardId);
                cardcollectionlist.AddChild(item);
            }
           GButton closebtn = cardcollect_UI.contentPane.GetChild("Close").asButton;
           closebtn.onClick.Add(() => CloseCardCollect());

        }
        /// <summary>关闭卡牌收藏
        /// 
        /// </summary>
        public void CloseCardCollect()
        {
            cardcollect_UI.Hide();
            mapcamera.SetActive(true);
            //TODO：显示地格渲染
            Debug.Log("卡牌收藏关闭");
        }
        /// <summary>
        /// 响应卡牌书内卡牌点击事件
        /// </summary>
        /// <param name="context"></param>
        public void OnClickCardInCardCollection(EventContext context)
        {
            // 先获取到点击的下标
            int index = cardcollectionlist.GetChildIndex(context.data as GObject);

            // 通过下标获取到id
            string cardId = playercardlist[index];
            // 向数据库查询展示数据
            JsonData data = CardManager.Instance().GetCardJsonData(cardId);

            _abstractText.text = "姓名：" + data["name"] + "\n" + "类型：" + data["type"];

            _storyText.text = "这里本来该有卡牌故事但是现在没有数据\n" + data["effect"];

            // TODO: 根据策划案加载icon

            _picLoader.url = UIPackage.GetItemURL(cardicons, cardId);

        }
        #endregion
    }
}

