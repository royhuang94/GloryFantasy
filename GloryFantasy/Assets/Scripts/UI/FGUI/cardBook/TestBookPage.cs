using System.Linq;
using UnityEngine;
using FairyGUI;
using GameCard;

class TestBookPage : GComponent
{
	private Controller _style;
	private GObject _pageNumber;
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
	/// 总资源包定义
	/// </summary>
	private const string _pkgName = "20190603";
	/// <summary>
	/// 卡牌堆资源包定义
	/// </summary>
	private const string _cardSetsAssets = "fakeHandcard";

	public override void ConstructFromXML(FairyGUI.Utils.XML xml)
	{
		base.ConstructFromXML(xml);
		
		_style = GetController("style");

		_cardSetsList = GetChild("cardList").asList;

		_cardDisplayer = GetChild("cardDisplayer").asCom;
		
		// 获取卡牌书内展示区相关变量
		_abstractText = _cardDisplayer.GetChild("abstractText").asTextField;
		_storyText = _cardDisplayer.GetChild("storyText").asTextField;
		_iconLoader = _cardDisplayer.GetChild("iconLoader").asLoader;
		_picLoader = _cardDisplayer.GetChild("cardPicLoader").asLoader;

		_pageNumber = GetChild("pn");
	}

	public void render(int pageIndex)
	{
		_pageNumber.text = (pageIndex + 1).ToString();

		if (pageIndex == 0)
			_style.selectedIndex = 0; // 空白页
		else if (pageIndex % 2 == 0)
		{
			_style.selectedIndex = 2; // 卡牌说明页
		}
		else
		{
			_cardSetsList.RemoveChildren(0,-1,true);
			_style.selectedIndex = 1; // 卡牌页
			int pos = (pageIndex - 1) * 9;
			int count = 0;
			while (count < 9 && CardManager.Instance().cardsSets.Count > pos)
			{
				string cardId = CardManager.Instance().cardsSets[pos];
				GObject item = UIPackage.CreateObject(_pkgName, "cardsSetsItem");
				if(!cardId.Contains("#"))
					item.icon = UIPackage.GetItemURL(_cardSetsAssets,cardId.Split('_').First());
				else
				{
					// 若带有'#'，则说明此id包含instanceid，需要重新解析
					string nid = cardId.Substring(0, cardId.IndexOf('#'));
					item.icon = UIPackage.GetItemURL(_cardSetsAssets,nid.Split('_').First());
				}
				
				item.onClick.Add(() =>
				{
					CardManager.Instance().InsertIntoHandCard(cardId);
				});

				_cardSetsList.AddChild(item);
				count++;
				pos++;
			}
		}
	}
}
