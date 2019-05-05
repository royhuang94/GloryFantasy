using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GamePlay;
using FairyGUI;
using GameCard;


public class FGUIInterfaces : MonoBehaviour
{

	private GComponent _mainUI;
	private GButton _endRoundButton;
	private GButton _cardSetsButton;
	private GTextField _APText;
	
	private GList _listInCardBook;
	private GComponent _cardBookFrame;
	private Window _cardBookWindow;
	private GButton _closeWindowButton;

	private GameObject _battleMapBlockAndUnits;
	private GameObject _cardsSlots;
	private GameObject _summonButton;
	private GameObject _cooldownButton;
	private GameObject _roundStateText;
	
	private List<GameObject> _uiToHide;
	
	// Use this for initialization
	void Start () {
		GRoot.inst.SetContentScaleFactor(960, 540);
		UIPackage.AddPackage("BattleMapFGUIPkg/20190504");
		_mainUI = UIPackage.CreateObject("20190504", "battleScene").asCom;
		_cardBookFrame = UIPackage.CreateObject("20190504", "cardBookFrame").asCom;
		
		GRoot.inst.AddChild(_mainUI);
		
		_cardBookWindow = new Window();
		_cardBookWindow.contentPane = _cardBookFrame;
		
		_endRoundButton = _mainUI.GetChild("endRoundButton").asButton;
		_cardSetsButton = _mainUI.GetChild("cardSetsButton").asButton;
		_APText = _mainUI.GetChild("APDisplayer").asCom.GetChild("APText").asTextField;
		_closeWindowButton = _cardBookFrame.GetChild("closeButton").asButton;
		_listInCardBook = _cardBookFrame.GetChild("cardList").asList;
		
		
		#region 处理其他UI
		_battleMapBlockAndUnits = GameObject.Find("MainPanel_1212");
		_cardsSlots = GameObject.Find("Panel");
		_summonButton = GameObject.Find("SummonButton");
		_cooldownButton = GameObject.Find("CoolDownButton");
		_roundStateText = GameObject.Find("phaseNameText");
		
		_uiToHide = new List<GameObject>();

		_uiToHide.Add(_battleMapBlockAndUnits);
		_uiToHide.Add(_cardsSlots);
		_uiToHide.Add(_summonButton);
		_uiToHide.Add(_cooldownButton);
		_uiToHide.Add(_roundStateText);
		#endregion

		foreach (GComponent item in _listInCardBook.GetChildren())
		{
			item.icon = UIPackage.GetItemURL("20190504", "emptyCardSlot");
		}
		
		// 卡牌书界面内关闭按钮事件监听
		_closeWindowButton.onClick.Add(ShowCardBook);

		// 回合结束按钮添加事件监听
		_endRoundButton.onClick.Add(Gameplay.Instance().switchPhaseHandler);
		
		// 卡牌堆按钮添加事件监听
		_cardSetsButton.onClick.Add(ShowCardBook);
		
	}

	private void LateUpdate()
	{
		_APText.text = Player.Instance().ap.ToString();
	}

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
}
