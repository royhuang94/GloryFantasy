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
        private string CardIconPackage = "BattleMapFGUIPkg/fakeHandcard";
        private string LibraryPackage = "MainMapFairyGUIPackage/Library";
        #endregion
        #region 大地图的GCompoment 和window
        private GComponent mainmapUI;
        private GComponent libraryUI;
//        private GComponent cardcollectUI;
        private GComponent verifyUI;
        private GComponent _winUI;
        private GComponent _loseUI;
        private GComponent _dialogUI;
        private Window main_mapUI;
        private Window cardcollect_UI;
        private Window library_UI;
        private Window verify_UI;
        private Window win_UI;
        private Window lose_UI;
        private Window dialog_UI;
        
        private GComponent _cardDisplayer;
        #endregion
        #region 按钮，文本,装载器等

        private GButton _fClueBtn;        // 第一个线索按钮
        private GButton _sClueBtn;        // 第二个线索按钮
        private GButton _tClueBtn;        // 第三个线索按钮
        private GButton ccbtn;
        private GButton buybtn;
        private GButton cancelbtn;
        private GLoader _cardloader;
        private GList cardcollectionlist;
        private GList onsalelist;
        private GList transferlist;
        private GTextField _abstractText;
        private GTextField _storyText;
        private GLoader _picLoader;
        #endregion
        /// <summary>对CardCollection的引用
        /// //Todo:为啥这么写呢。。？
        /// 
        /// </summary>
        private List<string> playercardlist;
        private Library choosenlibrary;
        //URL
        private const string cardicons = "fakeHandcard";
        private const string MapPackage = "MainMapUI";

        private CardCollectWindow _cardCollectWindow;
        private WinWindow _winWindow;

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
            UIPackage.AddPackage(CardIconPackage);
            UIPackage.AddPackage(LibraryPackage);
            mainmapUI = UIPackage.CreateObject("MainMapUI", "MainUI").asCom;
            GRoot.inst.AddChild(mainmapUI);
//            cardcollectUI = UIPackage.CreateObject("CardCollection", "CardBook").asCom;
            libraryUI = UIPackage.CreateObject("Library", "LibraryMain").asCom;
            verifyUI = UIPackage.CreateObject("Library", "ConfirmBuying").asCom;
            _winUI = UIPackage.CreateObject("MainMapUI", "WinMenu").asCom;
            _loseUI = UIPackage.CreateObject("MainMapUI", "LoseMenu").asCom;
            main_mapUI = new Window();
            main_mapUI.contentPane = mainmapUI;
            main_mapUI.CenterOn(GRoot.inst, true);
            main_mapUI.Show();
            cardcollect_UI = new Window();
//            cardcollect_UI.contentPane = cardcollectUI;
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
            #region 初始化按钮装载器文本等

            _fClueBtn = mainmapUI.GetChild("n25").asButton;
            _sClueBtn = mainmapUI.GetChild("n26").asButton;
            _tClueBtn = mainmapUI.GetChild("n27").asButton;
            _fClueBtn.onClick.Add(ShowVictory);
            _sClueBtn.onClick.Add(ShowDefeat);
            _tClueBtn.onClick.Add(ShowDialog);
            ccbtn = mainmapUI.GetChild("CardBookButton").asButton;
            _cardCollectWindow = new CardCollectWindow(playercardlist);
            _winWindow = new WinWindow(playercardlist);
//            ccbtn.onClick.Add(() => ShowCardCollect());
            ccbtn.onClick.Add(() => { _cardCollectWindow.Show();});
//            cardcollectionlist = cardcollectUI.GetChild("cardList").asList;
            onsalelist = libraryUI.GetChild("LibraryCardList").asList;
//            _cardDisplayer = cardcollectUI.GetChild("cardDisplayer").asCom;
//            _abstractText = _cardDisplayer.GetChild("abstractText").asTextField;
//            _storyText = _cardDisplayer.GetChild("storyText").asTextField;
//            _picLoader = _cardDisplayer.GetChild("cardPicLoader").asLoader;
            #endregion
            
            
            _blueSlider = mainmapUI.GetChild("ProgressBlue").asProgress;
            _yellowSlider = mainmapUI.GetChild("ProgressYellow").asProgress;
            _redSlider = mainmapUI.GetChild("ProgressRed").asProgress;

            mainmapUI.GetChild("n15").visible = false;
            mainmapUI.GetChild("n16").visible = false;
            Debug.Log("ui初始化");
            
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
            playercardlist = CardCollection.Instance().mycollection;
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
                GComponent btn = UIPackage.CreateObject("Library", "btn").asCom;
                GTextField text = btn.GetChild("text").asTextField;
                text.text = "(" + i.hexVector.Hex_vector.x.ToString() + i.hexVector.Hex_vector.y.ToString() + ")";
                transferlist.AddChild(btn);
            }
            Library.activelibrarylist.Add(library);
            mapcamera.SetActive(false);
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
//            verify_UI.Hide();
            mapcamera.SetActive(true);
        }
        /// <summary>展示三张卡牌，
        /// 
        /// </summary>
        public  void ShowCard(Library library)
        {
            onsalelist.RemoveChildren();
            foreach (string cardID in library.librarylist)
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
            CardCollection.Instance().choosecardindex = onsalelist.GetChildIndex(context.data as GObject);
            CardCollection.Instance().choosecardID = choosenlibrary.librarylist[CardCollection.Instance().choosecardindex];
            verify_UI.Show();
            buybtn = verify_UI.contentPane.GetChild("buybtn").asButton;
            cancelbtn = verify_UI.contentPane.GetChild("cancelbtn").asButton;
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
        /// <summary>展示卡牌收藏
        /// 
        /// </summary>
        public void ShowCardCollect()
        {
            MapUnit.CleanList();
            cardcollect_UI.Show();
            Debug.Log("展示卡牌收藏");
//            mapcamera.SetActive(false);
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

            _picLoader.url = UIPackage.GetItemURL(cardicons, cardId);

        }
        #endregion

        #region 三个线索按钮点击代码
        public void ShowVictory()
        {
            Debug.Log("click first clue -- show victory");
            _winWindow.Show();
        }

        public void ShowDefeat()
        {
            Debug.Log("click second clue -- show defeat");
            lose_UI.Show();
            GButton continueBtn = lose_UI.contentPane.GetChild("loseReturnButton").asButton;
            continueBtn.onClick.Add(CloseLoseWindow);
        }

        private void CloseLoseWindow()
        {
            lose_UI.Hide();
        }
        
        public void ShowDialog()
        {
            Debug.Log("click third clue -- show dialog");
            
        }
        #endregion
    }
}

