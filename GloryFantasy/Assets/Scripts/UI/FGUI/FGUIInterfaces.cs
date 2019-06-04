﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GamePlay;
using FairyGUI;
using GameCard;
using IMessage;
using LitJson;


public class FGUIInterfaces : UnitySingleton<FGUIInterfaces>, MsgReceiver
{

	private GComponent _mainUI;
	private GButton _endRoundButton;
	private GButton _cardSetsButton;
	private GTextField _APText;

	private bool _canShowArrow = false;
	private Vector3 _startPos;
	
	#region 卡牌书内变量
	private Window _cardBookWindow;
	private GComponent _cardBookFrame;
	private GButton _closeWindowButton;
	private GList _cardsSetsList;
	private GComponent _cardDisplayer;
	private GTextField _abstractText;
	private GTextField _storyText;
	private GLoader _iconLoader;
	private GLoader _picLoader;
	#endregion
	
	private GList _handcardList;
	
	private GList _cooldownList;

	private GObject lastClicked;

	#region 描述窗内变量
	private Window _cardDescribeWindow;
	private GComponent _cardDescibeFrame;
	private GTextField _title;
	private GTextField _effect;
	private GTextField _value;
	#endregion
	
	#region 现有UGUI遗存
	private GameObject _battleMapBlockAndUnits;
	private GameObject _roundStateText;
	private List<GameObject> _uiToHide;
	#endregion

	#region 资源包定义
	private const string path = "BattleMapFGUIPkg/";
	private const string pkgName = "20190507";
	private const string numsPkg = "cdNums";

	private const string handcardAssets = "handcardFake";
	private const string cooldowncardAssets = "handcardFake";
	private const string cardsetsAssets = "handcardFake";
	private const string cardBookPicAssets = "handcardFake";
	#endregion

	#region 卡牌管理器内列表引用
	private List<string> handcardList;
	private List<string> cardSetsList;
	private List<cdObject> cooldownList;
	private List<GameObject> handcardInstanceList;
	#endregion
	private void Awake()
	{
		GRoot.inst.SetContentScaleFactor(960, 540);
		UIPackage.AddPackage(path + pkgName);
		UIPackage.AddPackage(path + numsPkg);
		UIPackage.AddPackage(path + handcardAssets);
		//UIPackage.AddPackage(path + cooldowncardAssets);
		//UIPackage.AddPackage(path + cardsetsAssets);
		//UIPackage.AddPackage(path + cardBookPicAssets);
		lastClicked = null;
		
		// 战斗场景UI
		_mainUI = UIPackage.CreateObject(pkgName, "battleScene").asCom;
		// 卡牌书界面
		_cardBookFrame = UIPackage.CreateObject(pkgName, "cardBookFrame").asCom;

		// 卡牌描述窗口内容
		_cardDescibeFrame = UIPackage.CreateObject(pkgName, "cardDescribeFrame").asCom;
		
		// 添加主界面UI到游戏场景
		GRoot.inst.AddChild(_mainUI);
		
		#region 卡牌书相关内容初始化
		// 初始化卡牌书窗口
		_cardBookWindow = new Window();
		// 设定卡牌书窗口内容
		_cardBookWindow.contentPane = _cardBookFrame;
		// 获取卡牌书内展示区相关变量
		_cardDisplayer = _cardBookFrame.GetChild("cardDisplayer").asCom;
		_abstractText = _cardDisplayer.GetChild("abstractText").asTextField;
		_storyText = _cardDisplayer.GetChild("storyText").asTextField;
		_iconLoader = _cardDisplayer.GetChild("iconLoader").asLoader;
		_picLoader = _cardDisplayer.GetChild("cardPicLoader").asLoader;
		// 设定卡牌书窗口居中
		_cardBookWindow.CenterOn(GRoot.inst, true);
		#endregion
		
		// 初始化卡牌内容描述窗口
		_cardDescribeWindow = new Window();
		// 设定卡牌内容描述窗口内容
		_cardDescribeWindow.contentPane = _cardDescibeFrame;
		_title = _cardDescibeFrame.GetChild("title").asTextField;
		_effect = _cardDescibeFrame.GetChild("effect").asTextField;
		_value = _cardDescibeFrame.GetChild("values").asTextField;
		
		// 从游戏主场景获得各按钮的引用
		_endRoundButton = _mainUI.GetChild("endRoundButton").asButton;
		_cardSetsButton = _mainUI.GetChild("cardSetsButton").asButton;
		_closeWindowButton = _cardBookFrame.GetChild("closeButton").asButton;
		
		// 从游戏场景获得各list的引用
		_cooldownList = _mainUI.GetChild("cooldownList").asList;
		_handcardList = _mainUI.GetChild("handcardList").asList;
		_cardsSetsList = _cardBookFrame.GetChild("cardList").asList;
		
		// 从游戏主场景获取AP值展示的text
		_APText = _mainUI.GetChild("APDisplayer").asCom.GetChild("APText").asTextField;
	}

	// Use this for initialization
	void Start () {
		#region 处理其他UI
		_battleMapBlockAndUnits = GameObject.Find("MainPanel");
		_roundStateText = GameObject.Find("phaseNameText");
		
		_uiToHide = new List<GameObject>();

		_uiToHide.Add(_battleMapBlockAndUnits);
		_uiToHide.Add(_roundStateText);
		#endregion

		#region 获取卡牌管理器上相应列表引用
		handcardList = CardManager.Instance().cardsInHand;
		cardSetsList = CardManager.Instance().cardsSets;
		cooldownList = CardManager.Instance().cooldownCards;
		handcardInstanceList = CardManager.Instance().handcardsInstance;
		#endregion
		
		// 卡牌书界面内关闭按钮事件监听
		_closeWindowButton.onClick.Add(ShowCardBook);

		// 回合结束按钮添加事件监听
		_endRoundButton.onClick.Add(Gameplay.Instance().switchPhaseHandler);
		
		// 卡牌堆按钮添加事件监听
		_cardSetsButton.onClick.Add(ShowCardBook);

		_handcardList.childrenRenderOrder = ChildrenRenderOrder.Arch;
		
		MsgDispatcher.RegisterMsg(
			this.GetMsgReceiver(),
			(int)MessageType.HandcardChange,
			() => { return true;},
			UpdateHandcards,
			"Hand cards observer"
		);
		
		MsgDispatcher.RegisterMsg(
			this.GetMsgReceiver(),
			(int)MessageType.CardsetChange,
			() => { return true;},
			UpdateCardsSets,
			"Card sets observer"
		);
		
		MsgDispatcher.RegisterMsg(
			this.GetMsgReceiver(),
			(int)MessageType.CooldownlistChange,
			() => { return true;},
			UpdateCooldownList,
			"Cooldown list observer"
		);
		
		UpdateHandcards();
		UpdateCardsSets();
		UpdateCooldownList();
		
		_handcardList.onClickItem.Add(OnClickHandCard);
		
		_cardsSetsList.onClickItem.Add(OnClickCardInCardSets);
		
//		_cooldownList.onClickItem.Add(OnClickCardInCoolDownSets);				// 添加左键点击冷却牌
		_cooldownList.onRightClickItem.Add(OnClickCardInCoolDownSets);			// 添加右键点击冷却牌
	}

	private void FixedUpdate()
	{
//		ArrowManager.Instance().MakeArrowFlow(Input.mousePosition);
//		if(_canShowArrow)
//			ArrowManager.Instance().OnDrag(Input.mousePosition);
		if (_canShowArrow)
		{
			Debug.Log("sp: " + _startPos + " mp: " + Input.mousePosition);
			Vector3 vector3 = GameObject.Find("Main Camera").GetComponent<Camera>().WorldToScreenPoint(_startPos);
			Debug.Log("sp: " + vector3 + " mp: " + Input.mousePosition);
			ArrowMesh.Instance().UpdatePosition(_startPos, Input.mousePosition);
		}

		_APText.text = Player.Instance().ap.ToString();
	}

	/// <summary>
	/// 响应手牌点击事件的函数
	/// </summary>
	public void OnClickHandCard(EventContext context)
	{
		// 如果不是玩家回合，则无法使用卡牌
		if (!Gameplay.Instance().roundProcessController.IsPlayerRound())
			return;

		GObject item = context.data as GObject;
		_startPos = Input.mousePosition;
//		_canShowArrow = true;
		// 确认当前点击的卡牌和上次点击的不同，此时表明用户想使用这张卡牌
		if (item != lastClicked)
		{
			// 改变记录
			lastClicked = item;
			// 动效
			//DoSpecialEffect(item);
			// 设置当前选中的卡牌
			CardManager.Instance().SetSelectingCard(_handcardList.GetChildIndex(item));
		}
		else // 此时用户点击的牌和上次相同，表示用户想取消使用
		{
			// 恢复原大小
			foreach (GObject litem in _handcardList.GetChildren())
			{
				StartCoroutine(FancyHandCardEffect(litem, 1.0f));
			}
			
			// 重置上次选择项
			lastClicked = null;
			
			// 调用取消使用方法
			CardManager.Instance().CancleUseCurrentCard();
			
			// 结束函数执行，因为用户取消使用
			return;
		}
		
		CardManager.Instance().OnUseCurrentCard();
		
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
		
		// 测试，直接把卡牌放入手牌中
		//CardManager.Instance().InsertIntoHandCard(cardId);
		
		
		// 向数据库查询展示数据
		JsonData data = CardManager.Instance().GetCardJsonData(cardId);

		_abstractText.text = "姓名：" + data["name"] + "\n" + "类型：" + data["type"];

		_storyText.text = "这里本来该有卡牌故事但是现在没有数据\n" + data["effect"];
		
		// TODO: 根据策划案加载icon

		_picLoader.url = UIPackage.GetItemURL(cardBookPicAssets, cardId);
		
	}


	/// <summary>
	/// 响应冷却堆内卡牌右键点击事件的函数
	/// </summary>
	/// <param name="context"></param>
	public void OnClickCardInCoolDownSets(EventContext context)
	{
		// 现在用的展示界面和手牌及已部署单位是同一个界面。
		if (!_cardDescribeWindow.isShowing)
		{
			// 获取冷却列表点击下标
			int index = _cooldownList.GetChildIndex(context.data as GObject);

			// 根据点击下标获取对应冷却牌
			cdObject cooldownCard = cooldownList[index];
			
			// 冷却牌ID
			string cardID = cooldownCard.objectId;

			// 获取展示数据
			JsonData data = CardManager.Instance().GetCardJsonData(cardID);

			_title.text = data["name"].ToString();
			_effect.text = data["effect"].ToString();
			_value.text = "总冷却：" + data["cd"] + "     剩余冷却：" + cooldownCard.leftCd;
		
			_cardDescribeWindow.Show();
		}
		else
		{
			_cardDescribeWindow.Hide();
		}
	}
	
	
	/// <summary>
	/// 临时处理函数，用于协调UGUI和FGUI的显示与隐藏关系
	/// </summary>
	private void ShowCardBook()
	{
		if (!_cardBookWindow.isShowing)
		{
			_cardBookWindow.Show();
		}
		else
		{
			_cardBookWindow.Hide();
		}
		
		for (int i = 0; i < _uiToHide.Count; i++)
		{
			_uiToHide[i].SetActive(!_uiToHide[i].activeSelf);
		}
	}

	/// <summary>
	/// 更新卡牌书内卡牌堆列表，非cardManager调用等于浪费时间
	/// </summary>
	public void UpdateCardsSets()
	{
		// TODO: 完善此方法
		// 从卡牌list中移除所有item，加入新的，虽然暴力，但很简单
		_cardsSetsList.RemoveChildren(0, -1, true);

		foreach (string cardId in cardSetsList)
		{
			GObject item = UIPackage.CreateObject(pkgName, "cardsSetsItem");
			if(!cardId.Contains("#"))
				item.icon = UIPackage.GetItemURL(cardsetsAssets,cardId.Split('_').First());
			else
			{
				// 若带有'#'，则说明此id包含instanceid，需要重新解析
				string nid = cardId.Substring(0, cardId.IndexOf('#'));
				item.icon = UIPackage.GetItemURL(cardsetsAssets,nid.Split('_').First());
			}
			_cardsSetsList.AddChild(item);
		}
	}

	/// <summary>
	/// 更新手牌，保持与CardManager的同步
	/// </summary>
	public void UpdateHandcards()
	{
		// TODO: 完善此方法
		_handcardList.RemoveChildren(0, -1, true);
		foreach (string cardId in handcardList)
		{
			GObject item = UIPackage.CreateObject(pkgName, "handcardItem2");
			String nid;
			if (cardId.Contains("#"))
			{
				nid = cardId.Substring(0, cardId.IndexOf('#'));
			}
			else
			{
				nid = cardId;
			}

			item.icon = UIPackage.GetItemURL(handcardAssets, nid.Split('_').First());
			item.SetPivot(0.5f, 1f);
			_handcardList.AddChild(item);
			string id = string.Copy(nid);
			item.onRollOver.Add(() =>
			{
				// 切换当前鼠标防治上的卡牌最最上
				_handcardList.apexIndex = _handcardList.GetChildIndex(item);
				// 获取并展示数据
				JsonData data = CardManager.Instance().GetCardJsonData(id);
				_title.text = data["name"].ToString();
				_effect.text = data["effect"].ToString();
				_value.text = "冷却：" + data["cd"] + "    " + "专注值：" + data["cost"] + "\n" + data["type"];
				int costAp = int.Parse(""+data["cost"]);
				if (Player.Instance().CanConsumeAp(costAp))			// 可使用单位才放大
				{
					StartCoroutine(FancyHandCardEffect(item, 1.3f));
				}
				
				_cardDescribeWindow.Show();
			});

			item.onRollOut.Add(() =>
			{
				StartCoroutine(FancyHandCardEffect(item, 1.0f));
				_cardDescribeWindow.Hide();
			});
		}
	}

	/// <summary>
	/// 设置左上角卡牌描述框内容的接口，该窗口不会自动根据内容调整大小，自己注意文字长度
	/// </summary>
	/// <param name="title">标题文字</param>
	/// <param name="middle">中间文字</param>
	/// <param name="end">末端文字</param>
	public void setDescribeWindowContentText(string title, string middle, string end)
	{
		_title.text = title;
		_effect.text = middle;
		_value.text = end;
	}

	/// <summary>
	/// 设置卡牌描述框显示
	/// </summary>
	public void setDescribeWindowShow()
	{
		_cardDescribeWindow.Show();
	}

	/// <summary>
	/// 设置卡牌描述框隐藏
	/// </summary>
	public void setDescribeWindowHide()
	{
		_cardDescribeWindow.Hide();
	}

	private void DoSpecialEffect(GObject item)
	{
//		int index = _handcardList.GetChildIndex(item);
//		for (int i = 0; i < _handcardList.numChildren; i++)
//		{  
//			if (i == index)
//			{
//				StartCoroutine(FancyHandCardEffect(item, 1.3f));
//				continue;
//			}
//			
//			GObject childItem = _handcardList.GetChildAt(i);
//			
//			float distance = Mathf.Abs(i - index);
//
//			float distanceRange = 1.25f - 0.5f / 6.0f * distance;
//			
//			StartCoroutine(FancyHandCardEffect(childItem, distanceRange));
//
//		}
		StartCoroutine(FancyHandCardEffect(item, 1.3f));

	}


	private IEnumerator FancyHandCardEffect(GObject item, float finalScale)
	{
		int frameCount = 15;
		
		float range = item.scaleX;

		float step = (range - finalScale) / frameCount;

//		float judge = Mathf.Abs(step / 2 + step);
		const float judge = 0.0001f;        // 用于判断是否满足规定大小，不能用MinValue, 会越来越大或者小到不见了，谁用谁知道
		while (Mathf.Abs(range - finalScale) > judge)
		{
			range -= step;
			item.SetScale(range, range);
			yield return null;
		}
	}

	/// <summary>
	/// 更新冷却区卡牌
	/// </summary>
	public void UpdateCooldownList()
	{
		// TODO: 完善此方法
		_cooldownList.RemoveChildren(0, -1, true);
		foreach (cdObject cooldownCard in cooldownList)
		{
			GObject item = UIPackage.CreateObject(pkgName, "cooldownItem");
			item.icon = UIPackage.GetItemURL(numsPkg, "cdNum" + cooldownCard.leftCd);
			item.asCom.GetChild("n2").asLoader.url = UIPackage.GetItemURL(cooldowncardAssets,
				cooldownCard.objectId.Split('_').First());
			_cooldownList.AddChild(item);
		}
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
	
}

