using System.Linq;
using UnityEngine;
using FairyGUI;
using GameCard;

class TestBookPage : GComponent
{
	private Controller _style;
	private GObject _pageNumber;
	/// <summary>
	/// 卡牌堆资源包定义
	/// </summary>
	private const string _cardSetsAssets = "fakeHandcard";

	/// <summary>
	/// 卡牌书页面内左上角卡牌
	/// </summary>
	private GButton _cardItem1;
	
	/// <summary>
	/// 卡牌书页面内右上角卡牌
	/// </summary>
	private GButton _cardItem2;
	
	/// <summary>
	/// 卡牌书页面内左下角卡牌
	/// </summary>
	private GButton _cardItem3;
	
	/// <summary>
	/// 卡牌书页面内右下角卡牌
	/// </summary>
	private GButton _cardItem4;

	public override void ConstructFromXML(FairyGUI.Utils.XML xml)
	{
		base.ConstructFromXML(xml);

		_cardItem1 = GetChild("cardItem1").asButton;
		_cardItem2 = GetChild("cardItem2").asButton;
		_cardItem3 = GetChild("cardItem3").asButton;
		_cardItem4 = GetChild("cardItem4").asButton;
		
		_style = GetController("style");

		_pageNumber = GetChild("pn");
	}

	public void render(int pageIndex)
	{
		_pageNumber.text = (pageIndex + 1).ToString();

		_style.selectedIndex = 1; // 卡牌页
		int pos = pageIndex * 4;
		int count = 0;
		while (count < 4 && CardManager.Instance().cardsSets.Count > pos)
		{
			string cardId = CardManager.Instance().cardsSets[pos];
			GButton item = null;

			switch (count)
			{
				case 0:
					item = _cardItem1;
					break;
				case 1:
					item = _cardItem2;
					break;
				case 2:
					item = _cardItem3;
					break;
				case 3:
					item = _cardItem4;
					break;
				default:
					break;
			}

			if (!cardId.Contains("#"))
				item.icon = UIPackage.GetItemURL(_cardSetsAssets, cardId.Split('_').First());
			else
			{
				// 若带有'#'，则说明此id包含instanceid，需要重新解析
				string nid = cardId.Substring(0, cardId.IndexOf('#'));
				item.icon = UIPackage.GetItemURL(_cardSetsAssets, nid.Split('_').First());
			}

			item.onClick.Clear();
			item.onClick.Add(() => { CardManager.Instance().InsertIntoHandCard(cardId); });
			count++;
			pos++;
		}

		if (count < 4)
		{
			while (count < 4)
			{
				GButton item = null;
				switch (count)
				{
					case 0:
						item = _cardItem1;
						break;
					case 1:
						item = _cardItem2;
						break;
					case 2:
						item = _cardItem3;
						break;
					case 3:
						item = _cardItem4;
						break;
					default:
						break;
				}

				item.icon = UIPackage.GetItemURL(_cardSetsAssets, "empty");
				count++;
			}
		}
	}
}
