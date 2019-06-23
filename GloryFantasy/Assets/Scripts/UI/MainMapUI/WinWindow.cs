using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

public class WinWindow : Window 
{
    private List<string> _playerCardList;

    private GButton _continueBtn;
    private GButton _firstCard;
    private GButton _secondCard;
    private GButton _thirdCard;
    private List<GButton> _cardList;
    public WinWindow(List<string> playerCardList)
    {
        Debug.Log("construct");
        _playerCardList = playerCardList;
    }

    protected override void OnInit()
    {
        Debug.Log("init");
        this.contentPane = UIPackage.CreateObject("MainMapUI", "WinMenu").asCom;
        _cardList = new List<GButton>();
        _firstCard = this.contentPane.GetChild("n9").asButton;
        _cardList.Add(_firstCard);
        _secondCard = this.contentPane.GetChild("n10").asButton;
        _cardList.Add(_secondCard);
        _thirdCard = this.contentPane.GetChild("n11").asButton;
        _cardList.Add(_thirdCard);
        _continueBtn = this.contentPane.GetChild("continueButton").asButton;
        _continueBtn.onClick.Add(OnContinue);

    }

    /// <summary>
    /// 卡牌书卡牌点击事件
    /// </summary>
    /// <param name="context"></param>
    private void OnChooseCard(EventContext context)
    {
        
    }

    /// <summary>
    /// 胜利界面继续按钮点击事件
    /// </summary>
    private void OnContinue()
    {
        this.Hide();
    }

}
