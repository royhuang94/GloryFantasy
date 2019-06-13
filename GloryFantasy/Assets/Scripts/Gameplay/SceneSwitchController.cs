using System.Collections;
using System.Collections.Generic;
using GameGUI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitchController : UnitySingleton<SceneSwitchController> {

	// Use this for initialization
	private string _currentScene;
	private string _targetScene;
	private AsyncOperation _asyncOperation;
	private static GameObject _MMapCameraObject;		// 大地图
	private static GameObject _BMapCameraObject;		// 战斗地图
	private string _mainMapSceneName = "MainMapTest1";
	private string _encounterID;
	private List<string> _cardList;

	public string encounterId
	{
		get { return _encounterID; }
	}

	public List<string> cardList
	{
		get { return _cardList; }
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
//		Debug.Log("Load scene: " + scene.name);
	}

	
	/// <summary>
	/// 委托 —— 卸载场景时收到通知，执行操作
	/// </summary>
	/// <param name="scene">卸载的场景</param>
	private void OnSceneUnloader(Scene scene)
	{
//		Debug.Log("Unload scene: " + scene.name);
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
	public void SetData(string encounterId, List<string> cardList)
	{
		_encounterID = encounterId;
		_cardList = cardList;
	}

	/// <summary>
	/// 获取遭遇ID，一般供战斗地图调用
	/// </summary>
	/// <returns>_encounterID</returns>
	public string GetEncounterId()
	{
		return _encounterID;
	}

	/// <summary>
	/// 获取卡组，一般卡牌管理器调用
	/// </summary>
	/// <returns>_cardList</returns>
	public List<string> GetCardList()
	{
		return _cardList;
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
		if (_mainMapSceneName.Equals(currentScene))
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
		_asyncOperation = SceneManager.LoadSceneAsync(targetScene, LoadSceneMode.Additive);
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
	}

	
	/// <summary>
	/// 设置大地图相机的开关
	/// </summary>
	private void SwitchMMapCamera()
	{
		Camera mainMapCamera = _MMapCameraObject.GetComponent<Camera>();
		
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
}
