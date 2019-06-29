using System.Linq;
using FairyGUI;
using GameCard;
using GamePlay;
using IMessage;
using LitJson;

namespace UI.FGUI
{
    public class TEST_ONLY_cardListComponent : IComponent
    {

        private Window _cardListWindow;
        private GComponent _cardListFrame;
        private GButton _clearAll;
        private GList _cardList;

        private string _pkgName;
        private const string _cardSetsAssets = "card628";
        private const string _cardItem = "Button2";
        
        private TEST_ONLY_cardListComponent()
        {
            
        }

        public TEST_ONLY_cardListComponent(string pkgName, string resName)
        {
            _cardListFrame = UIPackage.CreateObject(pkgName, resName).asCom;

            if (_cardListFrame == null) return;
            _pkgName = pkgName;
            _cardList = _cardListFrame.GetChild("n4").asList;
            _clearAll = _cardListFrame.GetChild("n5").asButton;
            _cardListWindow = new Window();
            _cardListWindow.contentPane = _cardListFrame;
            _cardListWindow.CenterOn(GRoot.inst, true);
            _clearAll.onClick.Add(() =>
            {
                CardManager.Instance().SendAllHandcardToCd();
            });
            LoadCards();
        }

        public void LoadCards()
        {
            foreach (string cardId in CardManager.Instance().cardsData.Keys)
            {
                GButton cardItem = UIPackage.CreateObject(_pkgName, _cardItem).asButton;
                cardItem.icon = UIPackage.GetItemURL(_cardSetsAssets, cardId.Split('_').First());
                
                cardItem.onRollOver.Add(() =>
                {
                    JsonData data = CardManager.Instance().GetCardJsonData(cardId);
                    FGUIInterfaces.Instance().title.text = data["name"].ToString();
                    FGUIInterfaces.Instance().effect.text = data["effect"].ToString();
                    FGUIInterfaces.Instance().value.text = "冷却：" + data["cd"] + "    " + "专注值：" + data["cost"] + "\n" + data["type"];
                    FGUIInterfaces.Instance().cardDescribeWindow.Show();
                });
                
                cardItem.onRollOut.Add(() =>
                {
                    FGUIInterfaces.Instance().cardDescribeWindow.Hide();
                });
                
                cardItem.onClick.Add(() =>
                {
                    CardManager.Instance().InsertIntoHandCard(cardId);
                });
                
                _cardList.AddChild(cardItem);
            }
        }

        public override string ToString()
        {
            return "cardListComponent";
        }
        
        public void Operation()
        {
            if (!_cardListWindow.isShowing)
            {
                GRoot.inst.ShowPopup(_cardListWindow);
            }
        }

        #region 不用的函数

        public void Add(IComponent component)
        {
            throw new System.NotImplementedException();
        }

        public IComponent GetChild(string comId)
        {
            throw new System.NotImplementedException();
        }
        #endregion
    }
}