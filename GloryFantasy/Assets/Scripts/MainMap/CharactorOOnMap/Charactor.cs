using UnityEngine;
using System;


//2019.4.12版
namespace GFCharactor
{ 

public class Charactor : MonoBehaviour
{
    public Vector3 locate;
    /// <summary>开放给策划用于在Unity的Inspector里直接调整数值，代码内需要修改角色信息请修改CharacorData.
    /// 
    /// </summary>
    public int HP;//人物血量
    public int Step;//当前剩余步数，貌似不需要在外部修改，写在外面只是为了看见它。
    public int MaxStep;//最大步数
    /// <summary>储存角色信息的数据结构，考虑到未来可能有需求需要返回全部的数值，就写了这么个玩意。
    /// 
    /// </summary>
    public struct CharactorData
    {
        public int HP;
        public int MaxStep;
        public int Step;
        public object[,] Bag;
        public HexVector PlayerLocate;
        public GameObject UnderFeet;
    }
    public MainMapManager mapManager;
    public CharactorData charactorData;
    /// <summary>设定角色初始位置
    /// 
    /// </summary>
    /// <param name="locate"></param>
    /// <returns></returns>
    public Vector3 SetCharactorLocate(Vector3 locate)
    {

        Debug.Log("初始化角色位置");
        charactorData.PlayerLocate.Normal_vector = locate;
        Debug.Log("Initialize complete!" + locate);
        return charactorData.PlayerLocate.Normal_vector;
    }
    /// <summary>初始化角色信息
    /// 
    /// </summary>
    public void Initalize()
    {
        this.SetCharactorLocate(locate);
        this.SetMessage(HP, MaxStep);
        GetComponent<Transform>().position = locate;
        charactorData.PlayerLocate.Normal_vector = transform.GetComponent<Transform>().position;
        charactorData.UnderFeet = GameObject.Find("test" + charactorData.PlayerLocate.ChangeToHexVect(charactorData.PlayerLocate.Normal_vector).x.ToString() + charactorData.PlayerLocate.ChangeToHexVect(charactorData.PlayerLocate.Normal_vector).z.ToString());
        mapManager.setaround(charactorData.UnderFeet);
        Debug.Log("角色初始化完成");

    }
    /// <summary>设定初始步数和血量
    /// 
    /// </summary>
    /// <param name="hp"></param>
    /// <param name="maxstep"></param>
    /// <returns></returns>
    public CharactorData SetMessage(int hp, int maxstep)
    {
        Debug.Log("初始化角色最大步数和血量");
        charactorData.HP = hp;
        charactorData.MaxStep = maxstep;
        charactorData.Step = charactorData.MaxStep;
        Step = charactorData.Step;
        Debug.Log("Initialize complete HP:" + charactorData.HP + " Maxstep:" + charactorData.Step);
        return charactorData;
    }
    /// <summary>如果移动合法，调用这个函数改变角色坐标。
    /// 
    /// </summary>
    /// <param name="newtransform"></param>
    /// <returns></returns>
    public Vector3 Move(Vector3 newtransform, int step)
    {
        this.charactorData.PlayerLocate.Normal_vector = newtransform;
        this.GetComponent<Transform>().position = charactorData.PlayerLocate.Normal_vector;
        charactorData.UnderFeet = GameObject.Find("test" + charactorData.PlayerLocate.ChangeToHexVect(charactorData.PlayerLocate.Normal_vector).x.ToString() + charactorData.PlayerLocate.ChangeToHexVect(charactorData.PlayerLocate.Normal_vector).z.ToString());
        this.ChangeStep(step);
        return newtransform;
    }

    /// <summary>改变角色步数时调用， 并返回剩余步数。
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public int ChangeStep(int value)
    {
        if (charactorData.Step + value <= charactorData.MaxStep)
        {

            charactorData.Step = charactorData.Step + value;
            Step = charactorData.Step;
            if (charactorData.Step < 0)
            {
                HasDead();
            }
        }
        else
        {
            Debug.Log("超出最大步数");
            charactorData.Step = charactorData.MaxStep;
            Step = charactorData.Step;

        }

        return charactorData.Step;
    }
    /// <summary>角色死亡时调用。
    /// 
    /// </summary>
    public void HasDead()
    {

        Debug.Log("步数为负，角色累死了，返回起点");
        Initalize();

    }
    // Use this for initialization
    void Awake()
    {

        mapManager = GameObject.Find("Map").GetComponent<MainMapManager>();
    }
}
}