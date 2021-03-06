﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MainMap;
using GameCard;
using UnityEngine.UI;
using LitJson;
using System.IO;
using FairyGUI;
using PlayerCollection;
using StoryDialog;
using System.Linq;
using UnityEngine.EventSystems;

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
        private string CardIconPackage = "BattleMapFGUIPkg/card628";
        private string LibraryPackage = "MainMapFairyGUIPackage/Library";
        private const string path = "MainMapFairyGUIPackage/";
        private const string cardLibraryPackge = "newCardsLibrary";
        #endregion
        #region 大地图的GCompoment 和window
        private GComponent mainmapUI;
        private GComponent libraryUI;
        private GComponent cardcollectUI;
        private GComponent verifyUI;
        private GComponent _winUI;
        private GComponent _loseUI;
        private GComponent _dialogUI;
        private GComponent _aboutBoxUI;
        private Window main_mapUI;
        private Window cardcollect_UI;
        private Window library_UI;
        private Window verify_UI;
        private Window win_UI;
        private Window lose_UI;
        private Window dialog_UI;
        private Window aboutBox_UI;
        
       // private GComponent _cardDisplayer;
        #endregion
        #region 按钮，文本,装载器等

        private GButton _fClueBtn;        // 第一个线索按钮
        private GButton _sClueBtn;        // 第二个线索按钮
        private GButton _tClueBtn;        // 第三个线索按钮
        private GButton ccbtn;
        private GButton buybtn;
        private GButton cancelbtn;
        private GButton cardbookbtn;
        private GLoader _cardloader;
 //       private GList cardlist;
        private GList onsalelist;
        private GList transferlist;

        private GTextField nametext;
        private GTextField typetext;
        private GTextField tagtext;
        private GTextField effecttext;
        private GTextField describetext;
        private GTextField propertytext;
        private GTextField mainmapgoldtext;
        private GTextField librarygoldtext;
        private GLoader picloader;
        #endregion
        private Library choosenlibrary;

        //URL
        private const string cardicons = "card628";
        private const string MapPackage = "MainMapUI";

        //private CardCollectWindow _cardCollectWindow;
        private NewCardCollectWindow _cardCollectWindow;
        private WinWindow _winWindow;
        private DialogWindow _dialogWindowLeft;
        private DialogWindow _dialogWindowRight;
        private LibraryWindow _libraryWindow;

        private GProgressBar _blueSlider;
        private GProgressBar _yellowSlider;
        private GProgressBar _redSlider;

        /// <summary>初始化
        /// 
        /// </summary>
        private void Awake()
        {
            GRoot.inst.SetContentScaleFactor(1920, 1080);
            UIPackage.AddPackage(MainMapUIPackage);
            UIPackage.AddPackage(CardCollectionPackage);
            UIPackage.AddPackage(LibraryPackage);
            UIPackage.AddPackage(path + cardLibraryPackge);
            UIPackage.AddPackage(CardIconPackage);
            mainmapUI = UIPackage.CreateObject("MainMapUI", "MainUI").asCom;
            GRoot.inst.AddChild(mainmapUI);
            cardcollectUI = UIPackage.CreateObject("CardCollection", "CardBook").asCom;
            libraryUI = UIPackage.CreateObject("Shop", "ShopMain").asCom;
            verifyUI = UIPackage.CreateObject("Shop", "ConfirmBuying").asCom;
            _winUI = UIPackage.CreateObject("MainMapUI", "WinMenu").asCom;
            _loseUI = UIPackage.CreateObject("MainMapUI", "LoseMenu").asCom;
            _aboutBoxUI = UIPackage.CreateObject("MainMapUI", "AboutBox").asCom;
            main_mapUI = new Window();
            main_mapUI.contentPane = mainmapUI;
            main_mapUI.CenterOn(GRoot.inst, true);
            main_mapUI.Show();
            cardcollect_UI = new Window();
            cardcollect_UI.contentPane = cardcollectUI;
            cardcollect_UI.CenterOn(GRoot.inst, true);
            library_UI = new Window();
            library_UI.contentPane = libraryUI;
            library_UI.CenterOn(GRoot.inst, true);
            verify_UI = new Window();
            verify_UI.contentPane = verifyUI;
            verify_UI.CenterOn(GRoot.inst, true);
            win_UI = new Window();
            win_UI.contentPane = _winUI;
            win_UI.CenterOn(GRoot.inst, true);
            lose_UI = new Window();
            lose_UI.contentPane = _loseUI;
            lose_UI.CenterOn(GRoot.inst, true);
            aboutBox_UI = new Window();
            aboutBox_UI.contentPane = _aboutBoxUI;
            aboutBox_UI.CenterOn(GRoot.inst, true);
            
            #region 初始化按钮装载器文本等

            _fClueBtn = mainmapUI.GetChild("n25").asButton;
            _sClueBtn = mainmapUI.GetChild("n26").asButton;
            _tClueBtn = mainmapUI.GetChild("n27").asButton;
            _fClueBtn.onClick.Add(() => { ShowClueLog(25);});
            _sClueBtn.onClick.Add(() => { ShowClueLog(26);});
            _tClueBtn.onClick.Add(() => { ShowClueLog(27);});
            
            ccbtn = mainmapUI.GetChild("CardBookButton").asButton;
            _cardCollectWindow = new NewCardCollectWindow(ccbtn);
            //_cardCollectWindow = new CardCollectWindow(CardCollection.mycollection, Color.gray);
            _winWindow = new WinWindow(Color.gray, "MainMapUI", "WinMenu");
            _dialogWindowLeft = new DialogWindow(Color.gray, "MainMapUI", "DialogMessage_left");
            _dialogWindowRight = new DialogWindow(Color.gray, "MainMapUI", "DialogMessage_right");
            _libraryWindow = new LibraryWindow(Color.gray, "Shop", "ShopMain");
            
            //ccbtn.onClick.Add(OpenCardBook);
 //           cardlist = cardcollectUI.GetChild("cardList").asList;
            onsalelist = libraryUI.GetChild("ShopCardList").asList;
//            _cardDisplayer = cardcollectUI.GetChild("cardDisplayer").asCom;
            nametext= cardcollectUI.GetChild("name").asTextField;
            typetext = cardcollectUI.GetChild("type").asTextField;
            tagtext = cardcollectUI.GetChild("tag").asTextField;
            effecttext = cardcollectUI.GetChild("effect").asTextField;
            describetext = cardcollectUI.GetChild("describe").asTextField;
            propertytext = cardcollectUI.GetChild("property").asTextField;
            mainmapgoldtext = mainmapUI.GetChild("playergold").asTextField;
            librarygoldtext = libraryUI.GetChild("PlayerGold").asTextField;
            picloader = cardcollectUI.GetChild("cardPic").asLoader;
            #endregion
            
            
            _blueSlider = mainmapUI.GetChild("ProgressBlue").asProgress;
            _yellowSlider = mainmapUI.GetChild("ProgressYellow").asProgress;
            _redSlider = mainmapUI.GetChild("ProgressRed").asProgress;

            mainmapUI.GetChild("n15").visible = false;
            mainmapUI.GetChild("n16").visible = false;
            Debug.Log("ui初始化");
            
        }
        /// 更新金币数量
        public void UpdateGold(int i)
        {
            Charactor.Instance().charactordata.gold = Charactor.Instance().charactordata.gold + i;
            mainmapgoldtext.text = Charactor.Instance().charactordata.gold.ToString();
            librarygoldtext.text = Charactor.Instance().charactordata.gold.ToString();
        }
        /// <summary>
        /// 更新滑动条材质
        /// </summary>
        public void UpDateSlider(int i)
        {
//            GProgressBar slider = mainmapUI.GetChild("StepShow").asProgress;
//            GObject content = slider.GetChild("content");
//            GObject sliderunder = slider.GetChild("slider");
//            switch (i)
//            {
//                case 0:
//                    content.icon = UIPackage.GetItemURL(MapPackage, "content-full");
//                    sliderunder.icon = UIPackage.GetItemURL(MapPackage, "slider-full");
//                    break;
//                case 1:
//                    content.icon = UIPackage.GetItemURL(MapPackage, "content-half");
//                    sliderunder.icon = UIPackage.GetItemURL(MapPackage, "slider-half");
//                    break;
//                case 2:
//                    content.icon = UIPackage.GetItemURL(MapPackage, "content-less");
//                    sliderunder.icon = UIPackage.GetItemURL(MapPackage, "slider-less");
//                    break;
//            }
            // TODO: 进度条切换待完善
            Charactor charactor = Charactor.Instance();
            double value;
            GTweener gTweener;
            _blueSlider.visible = false;
            _yellowSlider.visible = false;
            _redSlider.visible = false;
            switch (i)
            {
                case 0:
                    _blueSlider.visible = true;
                    _blueSlider.max = charactor.charactordata.maxstep - charactor.iconhalfstep;        // 第一阶段进度条最大值
                    value = getSliderValue(_blueSlider.max, 0.4, charactor.charactordata.maxstep, charactor);    // 获取该阶段每一步对应进度条的值
                    gTweener = _blueSlider.TweenValue(value, 0.5f);                    // 进度条缩短动画
                    while (Math.Abs(value - _blueSlider.max * (1 - 0.4)) < 0.0001 && !gTweener.completed)        // 用于判断阶段交界处进度条的切换
                    {
//                        blueSlider.visible = false;
                        _yellowSlider.visible = true;
                        break;
                    }
                    break;
                case 1:
                    _yellowSlider.visible = true;
                    _yellowSlider.max = charactor.iconhalfstep - charactor.iconlessstep;
                    value = getSliderValue(_yellowSlider.max, 0.583, charactor.iconhalfstep, charactor);
                    gTweener = _yellowSlider.TweenValue(value, 0.5f);
                    while (Math.Abs(value - _yellowSlider.max * (1 - 0.4)) < 0.0001 && gTweener.completed)
                    {
                        _yellowSlider.visible = false;
                        _redSlider.visible = true;
                        break;
                    }
                    break;
                case 2:
                    _redSlider.visible = true;
                    _redSlider.max = charactor.iconlessstep;
                    value = getSliderValue(_redSlider.max, 1, charactor.iconlessstep, charactor);
                    _redSlider.TweenValue(value, 0.5f);
                    break;
            }
        }

        /// <summary>
        /// 获取进度条当前值
        /// </summary>
        /// <param name="sliderMax">进度条最大值</param>
        /// <param name="ratio">每走一步进度条减少值</param>
        /// <param name="max">该阶段最大步数</param>
        /// <param name="charactor">人物实例</param>
        /// <returns></returns>
        private double getSliderValue(double sliderMax, double ratio, double max, Charactor charactor)
        {
            double step = max - charactor.charactordata.step;        // 该阶段最大步数减去当前步数即该阶段走过的步数
            return sliderMax - step * ratio;                         // 进度条最大值减去走过的相应的值，即当前进度条的值
        }
        
        public void HideMain()
        {
            main_mapUI.Hide();
        }
        public void ShowMain()
        {
            main_mapUI.Show();
        }
        private void Start()
        {
//            cardcollectionlist.onClickItem.Add(OnClickCardInCardCollection);
            onsalelist.onClickItem.Add(OnClickCardInLibrary);
        }
        #region 图书馆相关代码
        /// <summary>点击图书馆地格后调用此方法展示图书馆UI并作初始化工作;
        /// 
        /// </summary>
        public void ShowlibraryUI(Library library)
        {

            choosenlibrary = library;
            Library.activelibrarylist.Remove(library);
            GComponent transfermain = libraryUI.GetChild("TransferMain").asCom;
            transferlist = transfermain.GetChild("transferlist").asList;
            transferlist.RemoveChildren();
            foreach (Library i in Library.activelibrarylist)
            {
                GComponent btn = UIPackage.CreateObject("Shop", "Button5").asCom;
                GTextField text = btn.GetChild("text").asTextField;
                text.text = "(" + i.hexVector.Hex_vector.x.ToString() + i.hexVector.Hex_vector.y.ToString() + ")";
                transferlist.AddChild(btn);
            }
            Library.activelibrarylist.Add(library);
            mapcamera.GetComponent<PhysicsRaycaster>().enabled = false;
            library_UI.modal = true;
            UIConfig.modalLayerColor = Color.gray;
            library_UI.Show();
            ShowCard(library);
            GButton closebtn = library_UI.contentPane.GetChild("Close").asButton;
            GButton transbtn = library_UI.contentPane.GetChild("TransferBtn").asButton;
            transbtn.onClick.Add(TransOnClick);
            closebtn.onClick.Add(LeaveOnClick);

        }    
        /// <summary>点击传送时，调用此方法
        /// 
        /// </summary>
        public  void TransOnClick()
        {
            transferlist.onClickItem.Add(trans);
        }
        public void trans(EventContext context)
        {
            int index = transferlist.GetChildIndex(context.data as GObject);
            Library.activelibrarylist[index].transfer();
            library_UI.Hide();
            mapcamera.SetActive(true);

        }
        /// <summary>点击离开时，调用此方法
        /// 
        /// </summary>
        public  void LeaveOnClick()
        {
            library_UI.Hide();
            verify_UI.Hide();
            mapcamera.GetComponent<PhysicsRaycaster>().enabled = true;

        }
        /// <summary>展示三张卡牌，
        /// 
        /// </summary>
        public  void ShowCard(Library library)
        {
            onsalelist.RemoveChildren();
            foreach (string cardID in library.librarylist)
            {
                GObject item = UIPackage.CreateObject("Shop", "CardItem");
                item.icon = UIPackage.GetItemURL(cardicons, cardID.Split('_').First());
                onsalelist.AddChild(item);
            }
        }
        /// <summary>图书馆内卡牌的点击事件
        /// 
        /// </summary>
        /// <param name="context"></param>
        public void OnClickCardInLibrary(EventContext context)
        {
            Debug.Log("click card in library");
            CardCollection.Instance().choosecardindex = onsalelist.GetChildIndex(context.data as GObject);
            CardCollection.Instance().choosecardID = choosenlibrary.librarylist[CardCollection.Instance().choosecardindex];
            verify_UI.Show();
            buybtn = verify_UI.contentPane.GetChild("ConfirmBuying_y").asButton;
            cancelbtn = verify_UI.contentPane.GetChild("ConfirmBuying_n").asButton;
            buybtn.onClick.Add(BuyOnclick);
            cancelbtn.onClick.Add(CancelOnclick);

        }
        /// <summary>购买按钮点击事件
        /// 
        /// </summary>
        /// <param name="cardId"></param>
        /// <param name="index"></param>
        public void BuyOnclick()
        {
            CardCollection.Instance().BuyCard(choosenlibrary);
            onsalelist.RemoveChildAt(CardCollection.Instance().choosecardindex, true);
//            verify_UI.Hide();
            
        }
        /// <summary>取消按钮点击事件
        /// 
        /// </summary>
        public void CancelOnclick()
        {
            Debug.Log("取消购买");
            buybtn.onClick.Remove(BuyOnclick);
//            verify_UI.Hide();
        }
        #endregion
        #region 卡牌书相关代码
//        private void OpenCardBook()
//        {
//            _cardCollectWindow.Show();
//            _cardCollectWindow.UpdateCardBook();
//        }
        # region 已弃用
        /// <summary>
        /// 响应卡牌书内卡牌点击事件
        /// </summary>
        /// <param name="context"></param>
        //public void OnClickCardInCardCollection(EventContext context)
        //{
        //    // 先获取到点击的下标
        //    int index = cardlist.GetChildIndex(context.data as GObject);

        //    // 通过下标获取到id
        //    string cardId = playercardlist[index];
        //    // 向数据库查询展示数据
        //    JsonData data = CardManager.Instance().GetCardJsonData(cardId);
        //    nametext.text = data["name"].ToString();
        //    typetext.text = data["type"].ToString();
        //    tagtext.text = data["tag"].ToString();
        //    effecttext.text = data["effect"].ToString();
        //    describetext.text = data["describe"].ToString();
        //    propertytext.text = data["property"].ToString();


        //    picloader.url = UIPackage.GetItemURL(cardicons, cardId);

        //}
        #endregion
        #endregion
        
        /// <summary>
        /// 展示胜利界面
        /// </summary>
        /// <param name="encounterID">遭遇ID</param>
        public void ShowVictory(string encounterID)
        {
            MainMapManager.Instance().Source.Play();
            if (Monster.IsBoss(encounterID))
            {
                DialogManager.Instance().RequestDialog(this, "test");
                switch(encounterID.Split('_').First())
                {
                    case "sandworm":
                        CardCollection.mycollection.Add("HKnight");
                        DialogManager.Instance().RequestDialog(this, "aftersandworm");
                        break;
                    case "chomper":
                        CardCollection.mycollection.Add("HLunamage");
                        DialogManager.Instance().RequestDialog(this, "afterchomper");
                        break;
                    case "Devil":
                        DialogManager.Instance().RequestDialog(this, "finalwin");
                        break;


                    default:
                        break;
                }
            }
            else
            {
                Debug.Log("click first clue -- show victory");
                _winWindow = new WinWindow(Color.gray, "MainMapUI", "WinMenu");
                _winWindow.Show();
            }

        }



        
        /// <summary>
        /// 展示失败界面
        /// </summary>
        /// <param name="encounterID">遭遇ID</param>
        public void ShowDefeat(string encounterID)
        {
            MainMapManager.Instance().Source.Play();
            Debug.Log("click second clue -- show defeat");
            lose_UI.modal = true;
            UIConfig.modalLayerColor = Color.gray;
            lose_UI.Show();
            GButton continueBtn = lose_UI.contentPane.GetChild("loseReturnButton").asButton;
            continueBtn.onClick.Add(CloseLoseWindow);
        }

        private void CloseLoseWindow()
        {
            lose_UI.Hide();
            Charactor.Instance().HasDead();
        }
        
        public void ShowDialog()
        {
            Debug.Log("click third clue -- show dialog");
            DialogManager.Instance().ShowDialog("test");
        }

        #region 三个线索按钮点击代码，以后换成其他地方调用

        private void ShowClueLog(int selectedClue)
        {
            string logTitle;
            string logContent;
            switch (selectedClue)
            {
                case 25:
                    logTitle = "诵咒者 - 英勇之魂";
                    logContent = "来自某个歌颂勇敢与坚毅的种族的战士，一位坚定地守护弱者的守护者。" +
                                 "他们的种族正面对着同胞失去理性的互相攻伐，他来到莱波利亚寻找答案。他似乎正在地图西侧的沙漠之中踟蹰前行。";
                    break;
                case 26:
                    logTitle = "诵咒者 - 聪慧之魂";
                    logContent = "来自某个歌颂智慧与创造的种族的学者，一位不倦地钻研神秘的探寻者。" +
                                 "他们的种族正面对着同胞离奇消失的巨大异变，她来到莱波利亚寻找答案。她似乎正在地图北侧的沼地之中迷惘前行。";
                    break;
                case 27:
                    logTitle = "世界之恶 - 邪念无歇";
                    logContent = "世界上所有知识附着的邪恶凝结的巨大意志，正在一点点膨胀着改变世界的法则。" +
                                 "不加遏制的话将会把世界所撕碎。核心位于莱波利亚的中央，在地图的东南方一个废弃的大书库。";
                    break;
                default:
                    logTitle = "这是题目";
                    logContent = "这是内容";
                    break;
            }

            GTextField title = _aboutBoxUI.GetChild("title").asTextField;
            title.text = logTitle;
            GTextField content = _aboutBoxUI.GetChild("content").asTextField;
            content.text = logContent;
            aboutBox_UI.modal = true;
            UIConfig.modalLayerColor = Color.gray;
            aboutBox_UI.Show();
            GButton closeBtn = _aboutBoxUI.GetChild("close").asButton;
            closeBtn.onClick.Add(() => { aboutBox_UI.Hide();});
        }
        #endregion

        private void Update()
        {
        }
    }
}

