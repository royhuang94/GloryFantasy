using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager
{
    private static PoolManager _instance;  //单列模式

    public static PoolManager Instance  //单列模式
    {
        get
        {
            if (_instance == null)
                _instance = new PoolManager();
            return _instance;
        }
    }

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

    private PoolManager()
    {
        //初始化
        GameObjectPoolList poolList = Resources.Load<GameObjectPoolList>("ScriptableObjects/GameObjectPool/gameobjectpool");

        poolDic = new Dictionary<string, GameObjectPool>();
        foreach (GameObjectPool pool in poolList.poolList)
        {
            Debug.Log(pool.ID);
            poolDic.Add(pool.ID, pool);
        }



    }

    public void Init()
    {
        //Do Nothing...
        //用于初始化该类的实例对象
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
