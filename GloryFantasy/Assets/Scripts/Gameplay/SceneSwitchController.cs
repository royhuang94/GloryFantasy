using System.Collections;
using System.Collections.Generic;
using System.Threading;
using FairyGUI;
using GameGUI;
using UI.FGUI;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mediator;

public class SceneSwitchController : UnitySingleton<SceneSwitchController> {

    

    // Use this for initialization
    private string _currentScene;
	private string _targetScene;
	private AsyncOperation _asyncOperation;
	private static GameObject _MMapCameraObject;		// 大地图
	private static GameObject _BMapCameraObject;		// 战斗地图
	private string _mainMapSceneName = "MainMapTest1";
	private string _mainMenuSceneName = "MenuScene";
	private string _battleMapSceneName = "BattleMapTest";
	private string _encounterID;
	private Deck _deck;
	private bool _win;			// 是否胜利

	private GComponent _menuComponent;

	public bool win
	{
		set { _win = value; }
	}

	/// <summary>
	/// 获取遭遇ID
	/// </summary>
	public string encounterId
	{
		get { return _encounterID; }
	}

	/// <summary>
	/// 获取卡组列表
	/// </summary>
	public Deck deck
	{
		get { return _deck; }
	}

	void Start () 
	{
		SceneManager.sceneLoaded += this.OnSceneLoader;
		SceneManager.sceneUnloaded += this.OnSceneUnloader;
		SceneManager.activeSceneChanged += this.OnSceneChanged;
		_MMapCameraObject = GameObject.Find("Main Camera");
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	/// <summary>
	/// 委托 —— 加载场景时收到通知，执行操作
	/// </summary>
	/// <param name="scene">加载的场景</param>
	/// <param name="mode">加载模式</param>
	private void OnSceneLoader(Scene scene, LoadSceneMode mode)
	{
		// 大地图场景加载
		if (_mainMapSceneName.Equals(scene.name))
		{
			GRoot.inst.RemoveChild(_menuComponent);
		}

	}

	public void setMenuComponent(GComponent component)
	{
		_menuComponent = component;
	}
	
	/// <summary>
	/// 委托 —— 卸载场景时收到通知，执行操作
	/// </summary>
	/// <param name="scene">卸载的场景</param>
	private void OnSceneUnloader(Scene scene)
	{
	}


	/// <summary>
	/// 委托 —— 切换场景时收到通知，执行操作
	/// </summary>
	/// <param name="oldScene">旧场景</param>
	/// <param name="newScene">新场景</param>
	private void OnSceneChanged(Scene oldScene, Scene newScene)
	{
//		Debug.Log("change: old-" + oldScene.name + " new-" + newScene.name);
//		if (oldScene.name.Equals("BattleMapTest"))
//		{
//			foreach (var go in newScene.GetRootGameObjects())
//			{
//				Debug.Log("go: " + go.name);
//				if(go.name.Equals("BattleMap.BattleMap") || go.name.Equals("GameCard.CardManager"))
//					Destroy(go);
//			}
//		}
	}

	/// <summary>
	/// 设置数据，一般供大地图调用
	/// </summary>
	/// <param name="encounterId">遭遇ID</param>
	/// <param name="cardList">卡组list</param>
	public void SetData(string encounterId, Deck cardList)
	{
		_encounterID = encounterId;
		_deck = cardList;
	}

	
	/// <summary>
	/// 开放接口，场景切换时调用此方法
	/// </summary>
	/// <param name="currentScene">当前场景</param>
	/// <param name="targetScene">要切换的目标场景</param>
	public void Switch(string currentScene, string targetScene)
	{
		Debug.Log("current: " + currentScene);
		
		// 如果当前场景是大地图，则加载战斗场景，否则卸载战斗场景
		if (_mainMapSceneName.Equals(currentScene) || _mainMenuSceneName.Equals(currentScene))
		{
			StartCoroutine(LoadScene(targetScene));			 // 异步加载场景
		}
		else
		{
			StartCoroutine(UnloadScene(targetScene));		 // 异步卸载场景
		}
	}

	
	/// <summary>
	/// 异步加载场景
	/// </summary>
	/// <param name="targetScene">目标场景</param>
	/// <returns></returns>
	private IEnumerator LoadScene(string targetScene)
	{
		if (_battleMapSceneName.Equals(targetScene))
		{
			_asyncOperation = SceneManager.LoadSceneAsync(targetScene, LoadSceneMode.Additive);
		}
		else
		{
			_asyncOperation = SceneManager.LoadSceneAsync(targetScene, LoadSceneMode.Single);
		}

		yield return _asyncOperation;
		SceneManager.SetActiveScene(SceneManager.GetSceneByName(targetScene));
		SwitchMMapCamera();
	}


	/// <summary>
	/// 异步卸载场景
	/// </summary>
	/// <param name="targetScene">要卸载的目标场景，一般是战斗地图</param>
	/// <returns></returns>
	private IEnumerator UnloadScene(string targetScene)
	{
		Debug.Log("start to exit");
		FGUIInterfaces.Instance().CloseAll();
		_asyncOperation = SceneManager.UnloadSceneAsync(targetScene);
		yield return _asyncOperation;
		Resources.UnloadUnusedAssets();				// 删掉战斗地图所有未使用的资源，应该能减少一点资源使用
		SceneManager.SetActiveScene(SceneManager.GetSceneByName("MainMapTest1"));
		SwitchMMapCamera();
		
		if(_win)
			MainMapUI.Instance().ShowVictory();				// 胜利展示胜利界面
		else
			MainMapUI.Instance().ShowDefeat();				// 失败展示失败界面
	}

	
	/// <summary>
	/// 设置大地图相机的开关
	/// </summary>
	private void SwitchMMapCamera()
	{
		Debug.Log("switch camera");
		if (_MMapCameraObject.activeInHierarchy)		// 隐藏大地图显示
		{
			MainMapUI.Instance().HideMain();
			_MMapCameraObject.SetActive(false);
		}
		else
		{
			MainMapUI.Instance().ShowMain();
			_MMapCameraObject.SetActive(true);
		}
	}

    public void GetDeckFormMainMapK(Deck deck)
    {
        this._deck = deck;
    }
}
