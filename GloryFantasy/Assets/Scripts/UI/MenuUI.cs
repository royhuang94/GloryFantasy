using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;
using UnityEngine.SceneManagement;

public class MenuUI : UnitySingleton<MenuUI>
{
	private GComponent _menuMainUI;
	private const string MenuUIPath= "MenuFairyGUIPackage/";
	private const string MenuUIPackageName = "Mainmenu";

	private GButton _beginBtn;
	private GButton _exitBtn;
	
	private string _mainMapSceneName = "MainMapTest1";
	private string _mainMenuSceneName = "MenuScene";
	private void Awake()
	{
		GRoot.inst.SetContentScaleFactor(1920, 1080);
		UIPackage.AddPackage(MenuUIPath + MenuUIPackageName);
		_menuMainUI = UIPackage.CreateObject(MenuUIPackageName, "Mainmenu").asCom;
		GRoot.inst.AddChild(_menuMainUI);

		// 开始按钮
		_beginBtn = _menuMainUI.GetChild("beginButton").asButton;
		_beginBtn.onClick.Add(BeginFantasy);

		// 退出按钮
		_exitBtn = _menuMainUI.GetChild("exitButton").asButton;
		_exitBtn.onClick.Add(ExitFantasy);

	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void CloseAll()
	{
		Debug.Log("close menu");
		_menuMainUI.Dispose();
//		GRoot.inst.RemoveChild(_menuMainUI, true);
	}
	
	private void BeginFantasy()
	{
		Debug.Log("begin fantasy");
		SceneSwitchController.Instance().Switch(_mainMenuSceneName, _mainMapSceneName);
	}
	
	private void ExitFantasy()
	{
		Debug.Log("exit fantasy");
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
	}
}
