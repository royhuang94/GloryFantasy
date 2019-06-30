using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;
using PlayerCollection;
using GameCard;
using System.Linq;

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
    private string choosedcardid;
    private string CardIconPackage = "BattleMapFGUIPkg/card628";
    private const string cardicons = "card628";

    private int choosencardindex;
    private GList cardlist;
    List<string> cards;
    /// <summary>
    /// 胜利窗口构造函数
    /// </summary>
    /// <param name="bgColor">背景颜色</param>
    public WinWindow(Color bgColor, string pkgName, string resName)
    {
        _bgColor = bgColor;
        _pkgName = pkgName;
        _resName = resName;
    }

    protected override void OnInit()
    {
        UIPackage.AddPackage(CardIconPackage);
        this.modal = true;
        UIConfig.modalLayerColor = _bgColor;
        this.contentPane = UIPackage.CreateObject(_pkgName, _resName).asCom;
        this.CenterOn(GRoot.inst, true);
        
        //_cardList = new List<GButton>();
        //_firstCard = this.contentPane.GetChild("n9").asButton;
        //_cardList.Add(_firstCard);
        //_secondCard = this.contentPane.GetChild("n10").asButton;
        //_cardList.Add(_secondCard);
        //_thirdCard = this.contentPane.GetChild("n11").asButton;
        //_cardList.Add(_thirdCard);
        _continueBtn = this.contentPane.GetChild("continueButton").asButton;
        _continueBtn.onClick.Add(OnContinue);
        LoadVictory();
        //foreach (GButton button in _cardList)
        //{
        //    button.GetController("button").selectedIndex = 0;
        //}
        //foreach (GButton button in _cardList)
        //{
        //    button.onClick.Add(() =>
        //    {
        //        bool isFirst = false;
        //        Controller controller = button.GetController("button");
        //        if (!isFirst || controller.selectedIndex == 0)
        //        {
        //            for (int i = 0; i < _cardList.Count; i++)
        //            {
        //                _cardList[i].GetController("button").selectedIndex = 0;
        //            }

        //            controller.selectedIndex = 1;
        //            isFirst = true;
        //        }
        //        OnChooseCard(_cardList.IndexOf(button));
        //    });
        //}
    }
    public void LoadVictory()
    {
        //cards.Clear();
        cards = CardManager.Instance().GetRandomCards(3);
        choosedcardid = null;
        cardlist = contentPane.GetChild("victorylist").asList;
        cardlist.RemoveChildren();
        foreach (string card in cards)
        {
            GObject item = UIPackage.CreateObject("MainMapUI", "CardSelected");
            item.icon = UIPackage.GetItemURL(cardicons, card.Split('_').First());
            cardlist.AddChild(item);
        }
        cardlist.onClickItem.Add(ChooseWinCard);


    }
    /// <summary>
    /// 卡牌书卡牌点击事件
    /// </summary>
    /// <param name="clickIndex"></param>
    public void ChooseWinCard(EventContext context)
    {
        choosencardindex = cardlist.GetChildIndex(context.data as GObject);
        choosedcardid = cards[choosencardindex];
    }

    /// <summary>
    /// 胜利界面继续按钮点击事件
    /// </summary>
    private void OnContinue()
    {

            if (choosedcardid != null)
            {
                CardCollection.mycollection.Add(choosedcardid);
                this.Hide();
                this.Dispose();
            }

    }

}
