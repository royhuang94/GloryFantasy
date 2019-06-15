using UnityEngine;
using FairyGUI;

public class TurnPageMain : MonoBehaviour
{
	GComponent _mainView;
	FairyBook _book;
	GSlider _slider;

	void Awake()
	{
		UIPackage.AddPackage("UI/TurnPage");
		// 调用此函数后，从FGUI对象到自定义的类强转不再报错
		UIObjectFactory.SetPackageItemExtension("ui://TurnPage/testBook", typeof(FairyBook));
		UIObjectFactory.SetPackageItemExtension("ui://TurnPage/testPage", typeof(TestBookPage));
	}

	void Start()
	{
		Application.targetFrameRate = 60;
		Stage.inst.onKeyDown.Add(OnKeyDown);

		_mainView = this.GetComponent<UIPanel>().ui;

		_book = (FairyBook)_mainView.GetChild("book");
		_book.SetSoftShadowResource("ui://TurnPage/shadow_soft");
		_book.pageRenderer = RenderPage;
		_book.pageCount = 20;
		_book.currentPage = 0;
		_book.ShowCover(FairyBook.CoverType.Front, false);
		_book.onTurnComplete.Add(OnTurnComplete);

		// 关闭了卡牌书位移动画
		GearBase.disableAllTweenEffect = true;
		_mainView.GetController("bookPos").selectedIndex = 1;	// 位置控制
		GearBase.disableAllTweenEffect = false;

		#region 卡牌书以外其他按钮交互逻辑
		
		_mainView.GetChild("btnNext").onClick.Add(() =>
		{
			_book.TurnNext();
		});
		_mainView.GetChild("btnPrev").onClick.Add(() =>
		{
			_book.TurnPrevious();
		});

		_slider = _mainView.GetChild("pageSlide").asSlider;
		_slider.max = _book.pageCount - 1;
		_slider.value = 0;
		_slider.onGripTouchEnd.Add(() =>
		{
			_book.TurnTo((int)_slider.value);
		});
		
		#endregion
	}

	void OnTurnComplete()
	{
		_slider.value = _book.currentPage;

		if (_book.isCoverShowing(FairyBook.CoverType.Front))
			_mainView.GetController("bookPos").selectedIndex = 1;		// 位置控制器调用，改变卡牌书位置
		else if (_book.isCoverShowing(FairyBook.CoverType.Back))
			_mainView.GetController("bookPos").selectedIndex = 2;
		else
			_mainView.GetController("bookPos").selectedIndex = 0;
	}

	void RenderPage(int index, GComponent page)
	{
		((TestBookPage)page).render(index);
	}

	void OnKeyDown(EventContext context)
	{
		if (context.inputEvent.keyCode == KeyCode.Escape)
		{
			Application.Quit();
		}
	}
}