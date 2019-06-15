using System;
using System.Collections.Generic;
using System.Linq;
using FairyGUI;
using GameCard;
using IMessage;
using LitJson;
using UnityEngine;

namespace UI.FGUI
{
    public class NewCardBookComponent : IComponent, MsgReceiver
    {
        
        #region 变量

        private FairyBook _book;

        /// <summary>
        /// 卡牌书窗口变量
        /// </summary>
        private Window _cardBookWindow;
        /// <summary>
        /// 卡牌书窗口内所有组件的根节点
        /// </summary>
        private GComponent _cardBookFrame;
        /// <summary>
        /// 卡牌书窗口关闭窗口按钮
        /// </summary>
        private GButton _closeWindowButton;
        
        
        /// <summary>
        /// 卡牌管理器内卡牌列表引用, 可能以后会修改
        /// </summary>
        private List<string> __cardSetsList;
        /// <summary>
        /// 总资源包定义
        /// </summary>
        private string _pkgName;
        /// <summary>
        /// 卡牌堆资源包定义
        /// </summary>
        private string _cardSetsAssets;
        #endregion
        
        /// <summary>
        /// 不给调用
        /// </summary>
        private NewCardBookComponent()
        {
            // 不给调用
        }

        /// <summary>
        /// 初始化函数，需要提供卡牌书窗口的资源包及对应资源名
        /// 不对资源进行检查，无对应资源则初始化失败
        /// </summary>
        /// <param name="pkgName">资源包名</param>
        /// <param name="resName">资源名</param>
        public NewCardBookComponent(string pkgName, string resName)
        {
            // 加载卡牌书组件资源
            _cardBookFrame = UIPackage.CreateObject(pkgName, resName).asCom;
            
            if (_cardBookFrame == null) return;
            _pkgName = pkgName;
            Init();
            _book = (FairyBook)_cardBookFrame.GetChild("book");
            _book.SetSoftShadowResource("ui://" + pkgName + "/shadow_soft");
            _book.pageRenderer = RenderPage;
            int count = (int) Math.Ceiling(__cardSetsList.Count / 9.0f);
            if (count % 2 != 0) count += 1;
            _book.pageCount = count;
            _book.currentPage = 0;
            _book.ShowCover(FairyBook.CoverType.Front, false);
            _book.onTurnComplete.Add(OnTurnComplete);

            GearBase.disableAllTweenEffect = true;
            _cardBookFrame.GetController("bookPos").selectedIndex = 1;
            GearBase.disableAllTweenEffect = false;
            
        }
        
        void OnTurnComplete()
        {
            if (_book.isCoverShowing(FairyBook.CoverType.Front))
                _cardBookFrame.GetController("bookPos").selectedIndex = 1;		// 位置控制器调用，改变卡牌书位置
            else if (_book.isCoverShowing(FairyBook.CoverType.Back))
                _cardBookFrame.GetController("bookPos").selectedIndex = 2;
            else
                _cardBookFrame.GetController("bookPos").selectedIndex = 0;
        }

        /// <summary>
        /// 卡牌书页面渲染函数
        /// </summary>
        /// <param name="index"></param>
        /// <param name="page"></param>
        void RenderPage(int index, GComponent page)
        {
            ((TestBookPage)page).render(index);
        }

        /// <summary>
        /// 初始化卡牌变量函数
        /// </summary>
        private void Init()
        {
            _cardBookWindow = new Window();
            // 设定窗口组件
            _cardBookWindow.contentPane = _cardBookFrame;

            _closeWindowButton = _cardBookFrame.GetChild("closeButton").asButton;
            
            _closeWindowButton.onClick.Add(Operation);

            // 设定卡牌书窗口居中
            _cardBookWindow.CenterOn(GRoot.inst, true);
            
            // 获取卡牌引用
            __cardSetsList = CardManager.Instance().cardsSets;

            // 卡牌资源定义
            _cardSetsAssets = "fakeHandcard";
            
//            // 添加卡牌点击事件
//            _cardSetsList.onClickItem.Add(OnClickCardSetsItem);
        }

        /// <summary>
        /// 仿照主程写的写的接口
        /// </summary>
        T MsgReceiver.GetUnit<T>()
        {
            return this as T;
        }
        

        /// <summary>
        /// 响应卡牌书内item点击事件
        /// </summary>
        /// <param name="context"></param>
        public void OnClickCardSetsItem(EventContext context)
        {
//            // 先获取到点击的下标
//            int index = _cardSetsList.GetChildIndex(context.data as GObject);
//		
//            // 通过下标获取到id
//            string cardId = __cardSetsList[index];
//		
//            // 测试，直接把卡牌放入手牌中
//            CardManager.Instance().InsertIntoHandCard(cardId);
//		
//		
//            // 向数据库查询展示数据
//            JsonData data = CardManager.Instance().GetCardJsonData(cardId);
//
//            _abstractText.text = "姓名：" + data["name"] + "\n" + "类型：" + data["type"];
//
//            _storyText.text = "这里本来该有卡牌故事但是现在没有数据\n" + data["effect"];
//		
//            // TODO: 根据策划案加载icon
//
//            _picLoader.url = UIPackage.GetItemURL(_cardSetsAssets, cardId.Split('_').First());

        }
        
        /// <summary>
        /// 窗口的Operation就是调用进行展示或者关闭
        /// </summary>
        public void Operation()
        {
            if (!_cardBookWindow.isShowing)
            {
                _cardBookWindow.Show();
            }
            else
            {
                _cardBookWindow.Hide();
            }
        }

        public override string ToString()
        {
            return "CardBookWindow";
        }

        #region 叶子节点停用的函数
        /// <summary>
        /// 卡牌书组件作为叶子节点，不包含此方法
        /// </summary>
        /// <param name="component"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void Add(IComponent component)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// 卡牌书组件作为叶子节点，不包含此方法
        /// </summary>
        /// <param name="component"></param>
        /// <exception cref="NotImplementedException"></exception>
        public IComponent GetChild(string comId)
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}