using System;
using System.Collections.Generic;
using System.Linq;
using FairyGUI;
using LitJson;
using MainMap;
using PlayerCollection;
using UnityEngine;

namespace GameGUI
{
    public class NewCardCollectWindow 
    {
        #region 变量
        /// <summary>
        /// 外部按钮
        /// </summary>
        private GButton _gButton;
        /// <summary>
        /// 卡牌书窗口
        /// </summary>
        private Window _window;
        /// <summary>
        /// 关闭窗口按钮
        /// </summary>
        private GButton _closeButton;

        /// <summary>
        /// 窗体内容
        /// </summary>
        private GComponent _cardCollectFrame;

        private FairyBook _book;

        /// <summary>
        ///  总资源包定义
        /// </summary>
        private string _pkgName;

        /// <summary>
        /// 卡牌素材
        /// </summary>
        private string _cardAssets;

        #endregion
        public NewCardCollectWindow(string pkgName, string resName, GButton button)
        {
            
            _cardCollectFrame = UIPackage.CreateObject("mainMapCardBook", "newCardBookFrame").asCom;

            if (_cardCollectFrame == null) return;
            _pkgName = "mainMapCardBook";
            Init();
            
            _book = (FairyBook)_cardCollectFrame.GetChild("book");
            _book.SetSoftShadowResource("ui://" + _pkgName + "/shadow_soft");
            _book.pageRenderer = RenderPage;
            _book.pageCount = 100;
            _book.currentPage = 0;
            _book.ShowCover(FairyBook.CoverType.Front, false);
            _book.onTurnComplete.Add(OnTurnComplete);
            
            GearBase.disableAllTweenEffect = true;
            _cardCollectFrame.GetController("bookPos").selectedIndex = 1;
            GearBase.disableAllTweenEffect = false;

            _gButton = button;
            _gButton.onClick.Add(() =>
            {
                if (_window.isShowing)
                {
                    _window.Hide();
                }
                else
                {
                    _window.Show();
                }
            });
            _closeButton.onClick.Add(() =>
            {
                _window.Hide();
            });
        }
        
        void OnTurnComplete()
        {
            if (_book.isCoverShowing(FairyBook.CoverType.Front))
                _cardCollectFrame.GetController("bookPos").selectedIndex = 1;		// 位置控制器调用，改变卡牌书位置
            else if (_book.isCoverShowing(FairyBook.CoverType.Back))
                _cardCollectFrame.GetController("bookPos").selectedIndex = 2;
            else
                _cardCollectFrame.GetController("bookPos").selectedIndex = 0;
        }

        void RenderPage(int index, GComponent page)
        {
            ((CardCollectPage)page).render(index);
        }

        private void Init()
        {
            _window = new Window();
            _window.contentPane = _cardCollectFrame;
            // 给窗口按钮绑定事件
            _closeButton = _cardCollectFrame.GetChild("closeButton").asButton;
            _closeButton.onClick.Add(()=>
            {
                if (!_window.isShowing)
                {
                    _window.Show();
                }
                else
                {
                    _window.Hide();
                }
            });
            // 设置界面居中
            _window.CenterOn(GRoot.inst, true);
            _cardAssets = "card628";
            // 设置展示后的背景虚化
            _window.modal = true;
            UIConfig.modalLayerColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        }
    }
    
    public class CardCollectPage : GComponent
    {
        private Controller _style;
        private GObject _pageNumber;

        private static CardCollectPage _describePage;
        private static CardCollectPage _cardPage;
        private static int _pos;

        private const string _cardAssets = "card628";

        /// <summary>
        /// 卡牌书左边页面内的卡牌列表
        /// </summary>
        private GList _cardList;

        private GTextField _name;
        private GTextField _type;
        private GTextField _tag;
        private GTextField _effect;
        private GTextField _describe;
        private GTextField _property;
        /// <summary>
        /// 背景图装载器
        /// </summary>
        private GLoader _cardPicLoader;
        /// <summary>
        /// 升级按钮
        /// </summary>
        private GButton _upgradeButton;
        /// <summary>
        /// 进化按钮
        /// </summary>
        private GButton _evolveButton;


        public override void ConstructFromXML(FairyGUI.Utils.XML xml)
        {
            base.ConstructFromXML(xml);
            
            #region 绑定变量

            _name = GetChild("name").asTextField;
            _type = GetChild("type").asTextField;
            _tag = GetChild("tag").asTextField;
            _effect = GetChild("effect").asTextField;
            _describe = GetChild("describe").asTextField;
            _property = GetChild("property").asTextField;
            _cardPicLoader = GetChild("cardPic").asLoader;
            _upgradeButton = GetChild("upgradeButton").asButton;
            _evolveButton = GetChild("evolveButton").asButton;
            _cardList = GetChild("cardList").asList;
            
            _style = GetController("style");

            _pageNumber = GetChild("pn");

            #endregion
        }

        public void render(int pageIndex)
        {

            _pageNumber.text = (pageIndex + 1).ToString();
            
            if (pageIndex == 0 || pageIndex == 99)
            {
                _style.selectedIndex = 2;    // 宣渲染成白页
            } else if (pageIndex % 2 == 0)
            {
                _describePage = this;
                _style.selectedIndex = 1;    // 渲染成说明页
            }
            else
            {
                _cardPage = this;
                _cardList.RemoveChildren(0, -1, true);
                _style.selectedIndex = 0;    // 渲染成卡牌页
                int pos = 9 * (pageIndex -1) / 2;
                string initialCard = CardCollection.mycollection[pos];
                int count = 0;
                while (count < 9 && CardCollection.mycollection.Count > pos)
                {
                    GButton newItem = UIPackage.CreateObject("mainMapCardBook", "cardsSetsItem").asButton;
                    newItem.icon = UIPackage.GetItemURL(_cardAssets, CardCollection.mycollection[pos].Split('_').First());
                    newItem.onClick.Add(() =>
                    {
                        _pos = pos;
                        _describePage.Refresh(CardCollection.mycollection[pos]);
                    });
                    count++;
                    pos++;
                }

                if (count < 9)
                {
                    while (count < 9)
                    {
                        GButton newItem = UIPackage.CreateObject("mainMapCardBook", "cardsSetsItem").asButton;
                        newItem.icon = UIPackage.GetItemURL(_cardAssets, "empty");
                        count++;
                    }
                }

                _describePage.Refresh(initialCard);
            }
        }

        public void Refresh(string cardId)
        {
            JsonData cardData = MainMapManager.Instance().CardData[cardId];
            _name.text = cardData["name"].ToString();
            _tag.text = "";
            for (int i = 0; i < cardData["tag"].Count; i++)
            {
                _tag.text += "/" + cardData["tag"][i];
            }

            _type.text = cardData["type"].ToString();
            _effect.text = cardData["effect"].ToString();
            if (_type.text.Equals("Order"))
            {
                _property.text = string.Format("{0,-6}：{1,3}{2,4}{3,-6}：{4,3}", "专注力消耗", cardData["cost"], " ", "冷却时间",
                    cardData["cd"]);
            } else if (_type.text.Equals("Unit"))
            {
                _property.text = "";
                _property.text += string.Format("{0,-6}：{1, 3}{2,4}", "生命值", 6," ");
                _property.text += string.Format("{0,-6}：{1,3}\r\n", "攻击力", 6);
                _property.text += string.Format("{0,-6}：{1, 3}{2,4}", "攻击范围", 6," ");
                _property.text += string.Format("{0,-6}：{1,3}\r\n", "冷却时间", 6);
                _property.text += string.Format("{0,-6}：{1, 3}{2,4}", "移动范围", 6," ");
                _property.text += string.Format("{0,-6}：{1,3}\r\n", "移动次数", 6);
                _property.text += string.Format("{0,-6}：{1, 3}{2,4}", "专注力消耗", 6," ");
                _property.text += string.Format("{0,-6}：{1,3}", "伤害优先级", 6);
            }

            _cardPicLoader.icon = UIPackage.GetItemURL(_cardAssets, cardId.Split('_').First());
            
        }
    }
}