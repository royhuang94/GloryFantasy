using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

public class WinWindow : Window 
{
    private GButton _continueBtn;
    private GButton _firstCard;
    private GButton _secondCard;
    private GButton _thirdCard;
    private List<GButton> _cardList;
    private Color _bgColor;
    private string _pkgName;
    private string _resName;
    
    /// <summary>
    /// 胜利窗口构造函数
    /// </summary>
    /// <param name="bgColor">背景颜色</param>
    public WinWindow(Color bgColor)
    {
        _bgColor = bgColor;
    }

    protected override void OnInit()
    {
        this.modal = true;
        UIConfig.modalLayerColor = _bgColor;
        this.contentPane = UIPackage.CreateObject("MainMapUI", "WinMenu").asCom;
        this.CenterOn(GRoot.inst, true);
        
        _cardList = new List<GButton>();
        _firstCard = this.contentPane.GetChild("n9").asButton;
        _cardList.Add(_firstCard);
        _secondCard = this.contentPane.GetChild("n10").asButton;
        _cardList.Add(_secondCard);
        _thirdCard = this.contentPane.GetChild("n11").asButton;
        _cardList.Add(_thirdCard);
        _continueBtn = this.contentPane.GetChild("continueButton").asButton;
        _continueBtn.onClick.Add(OnContinue);

        Debug.Log("before: " + _cardList[0].GetController("button").selectedIndex);
        foreach (GButton button in _cardList)
        {
            button.GetController("button").selectedIndex = 0;
        }
        Debug.Log("after: " + _cardList[0].GetController("button").selectedIndex);
        foreach (GButton button in _cardList)
        {
            button.onClick.Add(() =>
            {
                bool isFirst = false;
                Controller controller = button.GetController("button");
                if (!isFirst || controller.selectedIndex == 0)
                {
                    for (int i = 0; i < _cardList.Count; i++)
                    {
                        _cardList[i].GetController("button").selectedIndex = 0;
                    }

                    controller.selectedIndex = 1;
                    isFirst = true;
                }
                OnChooseCard(_cardList.IndexOf(button));
            });
        }
    }

    /// <summary>
    /// 卡牌书卡牌点击事件
    /// </summary>
    /// <param name="clickIndex"></param>
    private void OnChooseCard(int clickIndex)
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
