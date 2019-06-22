using System;
using System.Collections.Generic;
using FairyGUI;
using GamePlay;
using GamePlay.Event;
using IMessage;
using UnityEngine;

namespace UI.FGUI
{
	public class FGUIInterfaces : UnitySingleton<FGUIInterfaces>, MsgReceiver, IComponent
	{

		private GComponent _mainUI;
		private GButton _endRoundButton;
		private GButton _cardSetsButton;

		/// <summary>
		/// 存储所有component的字典
		/// </summary>
		private Dictionary<string, IComponent> _components;
		
		#region 描述窗内变量
		public Window cardDescribeWindow;
		private GComponent _cardDescibeFrame;
	
		public GTextField title;
		public GTextField effect;
		public GTextField value;
		#endregion

		#region 资源包定义
		private const string path = "BattleMapFGUIPkg/";
		private const string cardBook = "cardBook";
		private const string pkgName = "newBattlemap";
		private const string numsPkg = "newCdNums";
		private const string handcardAssets = "fakeHandcard";
		private const string cooldowncardAssets = "fakeHandcard";
		#endregion

		private void Awake()
		{
			GRoot.inst.SetContentScaleFactor(1920, 1080);
			UIPackage.AddPackage(path + cardBook);
			UIObjectFactory.SetPackageItemExtension("ui://cardBook/Book", typeof(FairyBook));
			UIObjectFactory.SetPackageItemExtension("ui://cardBook/Page", typeof(TestBookPage));
			UIPackage.AddPackage(path + pkgName);
			UIPackage.AddPackage(path + numsPkg);
			UIPackage.AddPackage(path + handcardAssets);
			//UIPackage.AddPackage(path + cooldowncardAssets);
			//UIPackage.AddPackage(path + cardsetsAssets);
			//UIPackage.AddPackage(path + cardBookPicAssets);
			// 战斗场景UI
			_mainUI = UIPackage.CreateObject(pkgName, "BattleScene").asCom;
			// 卡牌描述窗口内容
			_cardDescibeFrame = UIPackage.CreateObject(pkgName, "cardDescribeFrame").asCom;
		
			// 添加主界面UI到游戏场景
			GRoot.inst.AddChild(_mainUI);

			// 初始化卡牌内容描述窗口
			cardDescribeWindow = new Window();
			// 设定卡牌内容描述窗口内容
			cardDescribeWindow.contentPane = _cardDescibeFrame;
			title = _cardDescibeFrame.GetChild("title").asTextField;
			effect = _cardDescibeFrame.GetChild("effect").asTextField;
			value = _cardDescibeFrame.GetChild("values").asTextField;
			
			cardDescribeWindow.SetXY(1900f - cardDescribeWindow._width, 20);
		
			// 从游戏主场景获得各按钮的引用
			_endRoundButton = _mainUI.GetChild("endRoundButton").asButton;
			_cardSetsButton = _mainUI.GetChild("cardSetsButton").asButton;

		}

		// Use this for initialization
		void Start () {

			_components = new Dictionary<string, IComponent>();
			// 添加卡牌书组件
			Add(new NewCardBookComponent(cardBook, "newCardBookFrame"));
			// 添加手牌组件
			HandCardComponent component = gameObject.AddComponent<HandCardComponent>();
			component.Init(_mainUI.GetChild("handcardList").asList, pkgName);
			Add(component);
			// 添加冷却池组件
			Add(new CoolDownListComponent(_mainUI.GetChild("cooldownList").asList, pkgName));
			// 添加AP值组件
			Add(new APDisplayerComponent(_mainUI.GetChild("APDisplayer").asCom));
			// 添加回合信息展示组件
			Add(new RoundInfoComponent(_mainUI.GetChild("roundText").asTextField));
			// 添加事件轴组件
			//Add(new EventScrollComponent(pkgName, "eventDescribeFrame", _mainUI.GetChild("eventScrollList").asList));
			// 回合结束按钮添加事件监听
			_endRoundButton.onClick.Add(Gameplay.Instance().switchPhaseHandler);
		
			// 卡牌堆按钮添加事件监听
			_cardSetsButton.onClick.Add(ShowCardBook);
		}

		/// <summary>
		/// 临时处理函数，用于协调UGUI和FGUI的显示与隐藏关系
		/// </summary>
		private void ShowCardBook()
		{
			// 获取窗口子组件，执行操作
			GetChild("CardBookWindow").Operation();
		}


		/// <summary>
		/// 设置左上角卡牌描述框内容的接口，该窗口不会自动根据内容调整大小，自己注意文字长度
		/// </summary>
		/// <param name="title">标题文字</param>
		/// <param name="middle">中间文字</param>
		/// <param name="end">末端文字</param>
		public void SetDescribeWindowContentText(string title, string middle, string end)
		{
			this.title.text = title;
			effect.text = middle;
			value.text = end;
		}

		/// <summary>
		/// 设置卡牌描述框显示
		/// </summary>
		public void SetDescribeWindowShow()
		{
			cardDescribeWindow.Show();
		}

		/// <summary>
		/// 设置卡牌描述框隐藏
		/// </summary>
		public void SetDescribeWindowHide()
		{
			cardDescribeWindow.Hide();
		}
	
		/// <summary>
		/// 仿照主程写的写的接口
		/// </summary>
		T MsgReceiver.GetUnit<T>()
		{
			return this as T;
		}
	
		/// <summary>
		/// 关闭所有UI
		/// </summary>
		public void CloseAll()
		{
			_mainUI.Dispose();
		}


		/// <summary>
		/// 根组件无Operation操作，调用则报错
		/// </summary>
		/// <exception cref="NotImplementedException"></exception>
		public void Operation()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// 添加组件到管理包
		/// </summary>
		/// <param name="component">要添加的组件</param>
		public void Add(IComponent component)
		{
			if (_components.ContainsKey(component.ToString()))
				return;
			_components.Add(component.ToString(), component);
		}

		/// <summary>
		/// 获取指定id对应的组件，非法id返回null
		/// </summary>
		/// <param name="comId">想获取的组件的id</param>
		/// <returns>对应的组件或者Null</returns>
		public IComponent GetChild(string comId)
		{
			if (_components.ContainsKey(comId))
				return _components[comId];
			return null;
		}

	}
}

