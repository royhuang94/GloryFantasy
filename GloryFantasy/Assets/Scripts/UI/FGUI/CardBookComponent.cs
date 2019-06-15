using System;
using System.Collections.Generic;
using System.Linq;
using FairyGUI;
using GameCard;
using IMessage;
using LitJson;

namespace UI.FGUI
{
    public class CardBookComponent : IComponent, MsgReceiver
    {
        
        #region 变量

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
        /// 卡牌书内卡牌列表的list
        /// </summary>
        private GList _cardSetsList;
        /// <summary>
        /// 卡牌书内右侧内容展示子组件引用
        /// </summary>
        private GComponent _cardDisplayer;
        /// <summary>
        /// _cardDisplayer组件内简述文本组件引用
        /// </summary>
        private GTextField _abstractText;
        /// <summary>
        /// _cardDisplayer组件内故事描述文本组件引用
        /// </summary>
        private GTextField _storyText;
        /// <summary>
        /// _cardDisplayer组件内icon装载器的引用
        /// </summary>
        private GLoader _iconLoader;
        /// <summary>
        /// _cardDisplayer组件内图片装载器的引用
        /// </summary>
        private GLoader _picLoader;
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
        private CardBookComponent()
        {
            // 不给调用
        }

        /// <summary>
        /// 初始化函数，需要提供卡牌书窗口的资源包及对应资源名
        /// 不对资源进行检查，无对应资源则初始化失败
        /// </summary>
        /// <param name="pkgName">资源包名</param>
        /// <param name="resName">资源名</param>
        public CardBookComponent(string pkgName, string resName)
        {
            // 加载卡牌书组件资源
            _cardBookFrame = UIPackage.CreateObject(pkgName, resName).asCom;
            
            if (_cardBookFrame == null) return;
            _pkgName = pkgName;
            
            Init();
            
            MsgDispatcher.RegisterMsg(
                this.GetMsgReceiver(),
                (int)MessageType.CardsetChange,
                () => { return true;},
                UpdateCardSets,
                "Card sets observer"
            );
            
            UpdateCardSets();
        }

        /// <summary>
        /// 初始化卡牌变量函数
        /// </summary>
        private void Init()
        {
            _cardBookWindow = new Window();
            // 设定窗口组件
            _cardBookWindow.contentPane = _cardBookFrame;
            
            // 获取卡牌书内展示区相关变量
            _cardDisplayer = _cardBookFrame.GetChild("cardDisplayer").asCom;
            _abstractText = _cardDisplayer.GetChild("abstractText").asTextField;
            _storyText = _cardDisplayer.GetChild("storyText").asTextField;
            _iconLoader = _cardDisplayer.GetChild("iconLoader").asLoader;
            _picLoader = _cardDisplayer.GetChild("cardPicLoader").asLoader;
            
            _closeWindowButton = _cardBookFrame.GetChild("closeButton").asButton;
            
            _closeWindowButton.onClick.Add(Operation);

            _cardSetsList = _cardBookFrame.GetChild("cardList").asList;

            
            // 设定卡牌书窗口居中
            _cardBookWindow.CenterOn(GRoot.inst, true);
            
            // 获取卡牌引用
            __cardSetsList = CardManager.Instance().cardsSets;

            // 卡牌资源定义
            _cardSetsAssets = "fakeHandcard";
            
            // 添加卡牌点击事件
            _cardSetsList.onClickItem.Add(OnClickCardSetsItem);
        }

        /// <summary>
        /// 仿照主程写的写的接口
        /// </summary>
        T MsgReceiver.GetUnit<T>()
        {
            return this as T;
        }

        /// <summary>
        /// 更新卡牌书内卡牌堆列表
        /// </summary>
        public void UpdateCardSets()
        {
            // 从卡牌List中移除所有item，加入新的，暴力但是简单
            _cardSetsList.RemoveChildren(0, -1, true);

            foreach (string cardId in __cardSetsList)
            {
                GObject item = UIPackage.CreateObject(_pkgName, "cardsSetsItem");
                if(!cardId.Contains("#"))
                    item.icon = UIPackage.GetItemURL(_cardSetsAssets,cardId.Split('_').First());
                else
                {
                    // 若带有'#'，则说明此id包含instanceid，需要重新解析
                    string nid = cardId.Substring(0, cardId.IndexOf('#'));
                    item.icon = UIPackage.GetItemURL(_cardSetsAssets,nid.Split('_').First());
                }
                _cardSetsList.AddChild(item);
            }
        }

        /// <summary>
        /// 响应卡牌书内item点击事件
        /// </summary>
        /// <param name="context"></param>
        public void OnClickCardSetsItem(EventContext context)
        {
            // 先获取到点击的下标
            int index = _cardSetsList.GetChildIndex(context.data as GObject);
		
            // 通过下标获取到id
            string cardId = __cardSetsList[index];
		
            // 测试，直接把卡牌放入手牌中
            CardManager.Instance().InsertIntoHandCard(cardId);
		
		
            // 向数据库查询展示数据
            JsonData data = CardManager.Instance().GetCardJsonData(cardId);

            _abstractText.text = "姓名：" + data["name"] + "\n" + "类型：" + data["type"];

            _storyText.text = "这里本来该有卡牌故事但是现在没有数据\n" + data["effect"];
		
            // TODO: 根据策划案加载icon

            _picLoader.url = UIPackage.GetItemURL(_cardSetsAssets, cardId.Split('_').First());

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