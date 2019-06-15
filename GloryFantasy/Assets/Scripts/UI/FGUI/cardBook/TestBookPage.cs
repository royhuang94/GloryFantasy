using UnityEngine;
using FairyGUI;

class TestBookPage : GComponent
{
	Controller _style;
	GObject _pageNumber;

	public override void ConstructFromXML(FairyGUI.Utils.XML xml)
	{
		base.ConstructFromXML(xml);
		
		_style = GetController("style");

		_pageNumber = GetChild("pn");
	}

	public void render(int pageIndex)
	{
		_pageNumber.text = (pageIndex + 1).ToString();

		if (pageIndex == 0)
			_style.selectedIndex = 0; // 空白页
		else if (pageIndex == 2)
		{
			// TODO: 应该在这里调用函数载入数据
			_style.selectedIndex = 2; // 卡牌说明页
		}
		else
			_style.selectedIndex = 1; // 卡牌说明页
	}
}
