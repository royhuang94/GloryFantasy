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
	private static GameObject cameraObject;
	
	void Start () {
		SceneManager.sceneLoaded += this.OnSceneLoader;
		SceneManager.sceneUnloaded += this.OnSceneUnloader;
		cameraObject =GameObject.Find("Main Camera");
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
		Debug.Log("Load scene: " + scene);
	}

	
	/// <summary>
	/// 委托 —— 卸载场景时收到通知，执行操作
	/// </summary>
	/// <param name="scene">卸载的场景</param>
	private void OnSceneUnloader(Scene scene)
	{
		Debug.Log("Hello Gary~");
	}
	

	/// <summary>
	/// 开放接口，场景切换时调用此方法
	/// </summary>
	/// <param name="targetScene">要切换的目标场景</param>
	/// <param name="ecounterID">遭遇ID</param>
	public void Switch(string currentScene, string targetScene)
	{
		if (currentScene.Equals("MainMapTest1"))
		{
			StartCoroutine(LoadScene(targetScene)); // 异步加载场景
		}
		else
		{
			StopCoroutine(UnloadScene(targetScene));
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
		setComponent();
	}


	private IEnumerator UnloadScene(string targetScene)
	{
		_asyncOperation = SceneManager.UnloadSceneAsync(targetScene);
		yield return _asyncOperation;
		setComponent();
	}

	private void setComponent()
	{
		Camera mainCamera = cameraObject.GetComponent<Camera>();
		if (mainCamera != null)
		{
			mainCamera.enabled = !mainCamera.enabled;			// 主相机得关掉，不然大地图会躺在战斗地图下面
			AudioListener audioListener = mainCamera.GetComponent<AudioListener>();
			audioListener.enabled = !audioListener.enabled;		// 这个也得关，不然会弹上千条提示，bulabula的
		}
	}
}
