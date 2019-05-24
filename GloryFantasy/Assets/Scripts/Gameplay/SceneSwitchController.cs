using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class SceneSwitchController : UnitySingleton<SceneSwitchController> {

	// Use this for initialization
	private string _currentScene;
	private string _targetScene;
	private AsyncOperation _asyncOperation;
	private static GameObject _MMapCameraObject;		// 大地图
	private static GameObject _BMapCameraObject;		// 战斗地图
	private string _mainMapSceneName = "MainMapTest1";
	
	void Start () {
		SceneManager.sceneLoaded += this.OnSceneLoader;
		SceneManager.sceneUnloaded += this.OnSceneUnloader;
		SceneManager.activeSceneChanged += this.OnSceneChanged;
		_MMapCameraObject =GameObject.Find("Main Camera");
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
	/// 传遭遇ID给战斗地图
	/// </summary>
	/// <param name="ecounterID">遭遇ID</param>
	public void transferToBmap(string ecounterID)
	{
		BattleMap.BattleMap.Instance().GetEncounterIDFromMainMap(ecounterID);
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
		BattleMap.BattleMap.Instance().RegisterMSG();
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
//		SceneManager.SetActiveScene()
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
		if(_MMapCameraObject.activeInHierarchy)
			_MMapCameraObject.SetActive(false);
		else
		{
			_MMapCameraObject.SetActive(true);
		}
//		if (mainMapCamera != null)
//		{
//			mainMapCamera.enabled = !mainMapCamera.enabled;			// 主相机得关掉，不然大地图会躺在战斗地图下面
//			AudioListener audioListener = mainMapCamera.GetComponent<AudioListener>();
//			audioListener.enabled = !audioListener.enabled;		// 这个也得关，不然会弹上千条提示，bulabula的
//		}
	}
}
