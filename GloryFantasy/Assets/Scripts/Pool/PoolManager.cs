using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager:
    GFGame.LiteSingleton<PoolManager>
{

    //static 静态变量
    private static string poolConfigPathPrefix = "Assets/Resources/ScriptableObjects/GameObjectPool/";
    //const常量
    private const string poolConfigPathMiddle = "gameobjectpool";
    private const string poolConfigPathPostfix = ".asset";

    public static string PoolConfigPath
    {
        get
        {
            return poolConfigPathPrefix + poolConfigPathMiddle + poolConfigPathPostfix;
        }
    }

    //字典
    private Dictionary<string, GameObjectPool> poolDic;


    /// <summary>
    /// 警告：此处不能直接使用new来创建对象，应该用单例
    /// </summary>
    public PoolManager()
    {
        //初始化
        GameObjectPoolList poolList = Resources.Load<GameObjectPoolList>("ScriptableObjects/GameObjectPool/gameobjectpool");

        poolDic = new Dictionary<string, GameObjectPool>();
        foreach (GameObjectPool pool in poolList.poolList)
        {
            //Debug.Log(pool.ID);
            poolDic.Add(pool.ID, pool);
        }
    }

    public void Init()
    {
        //Do Nothing...
    }

    //TODO 实现返回GameObject的函数，GetInst(string name)
    //TODO 实现返回多个GameObject的话，就设置GetInst的参数为(string name, int num)
    public GameObject GetInst(string id)
    {
        GameObjectPool pool;
        if(poolDic.TryGetValue(id, out pool))
        {
            return pool.GetInst();
        }
        Debug.LogWarning("Pool: " + id + "is not exist!!");
        return null;
    }
    
}
