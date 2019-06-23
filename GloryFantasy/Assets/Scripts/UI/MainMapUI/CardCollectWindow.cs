using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;
using GameCard;
using LitJson;

public class CardCollectWindow : Window
{
    private GList _cardList;
    private GTextField _name;
    private GTextField _type;
    private GTextField _tag;
    private GTextField _effect;
    private GTextField _describe;
    private GTextField _property;
    private GLoader _cardPicLoader;
    private GButton _upgradeBtn;
    private GButton _evolveBtn;
    private GButton _closeBtn;

    private List<string> _playerCardList;

    public CardCollectWindow(List<string> playerCardList)
    {
        Debug.Log("construct");
        _playerCardList = playerCardList;
    }

    protected override void OnInit()
    {
        Debug.Log("init");
        this.contentPane = UIPackage.CreateObject("CardCollection", "CardBook").asCom;

        _cardList = this.contentPane.GetChild("cardList").asList;
        
        _name = this.contentPane.GetChild("name").asTextField;
        _type = this.contentPane.GetChild("type").asTextField;
        _tag = this.contentPane.GetChild("tag").asTextField;
        _effect = this.contentPane.GetChild("effect").asTextField;
        _describe = this.contentPane.GetChild("describe").asTextField;
        _property = this.contentPane.GetChild("property").asTextField;
        _name = this.contentPane.GetChild("name").asTextField;

        _cardPicLoader = this.contentPane.GetChild("cardPic").asLoader;

        _upgradeBtn = this.contentPane.GetChild("upgradeButton").asButton;
        _evolveBtn = this.contentPane.GetChild("evolveButton").asButton;
        _closeBtn = this.contentPane.GetChild("closeButton").asButton;
        
        _closeBtn.onClick.Add(this.Hide);
        _cardList.onClickItem.Add(OnClickCardItem);
    }

    /// <summary>
    /// 卡牌书卡牌点击事件
    /// </summary>
    /// <param name="context"></param>
    private void OnClickCardItem(EventContext context)
    {
        // 获取点击卡牌下标
        int index = _cardList.GetChildIndex(context.data as GObject);

        // 根据下标获得卡牌ID
        string cardId = _playerCardList[index];

        // 获取卡牌信息
        JsonData data = CardManager.Instance().GetCardJsonData(cardId);

        // TODO: 卡牌详细信息的显示及图片加载
        _name.text = data["name"].ToString();
        _type.text = data["type"].ToString();
        _tag.text = data["tag"].ToString();
        
//        _cardPicLoader.url = UIPackage.GetItemURL()
    }
    
    /// <summary>
    /// 卡牌书升级按钮绑定事件
    /// </summary>
    private void OnUpgradeCard()
    {
        
    }

    /// <summary>
    /// 卡牌书铭转按钮绑定事件
    /// </summary>
    private void OnEvolveCard()
    {
        
    }
    
    
}
