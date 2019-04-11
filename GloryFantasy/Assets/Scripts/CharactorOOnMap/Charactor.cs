using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//2019.4.10  13：50版

public class Charactor : MonoBehaviour {
    public Vector3 locate;
    public int HP;//人物血量
    public int MaxStep;//最大步数
    public Charactor(int hp,int maxstep)
        {
        this.HP = hp;
        this.MaxStep = maxstep;
        }
    public struct CharactorData
    {
        public  int HP ;
        public  int MaxStep ;
        public  object[,] Bag ;
        public Transform mytransform ;
       
    }
    public CharactorData charactorData= new CharactorData();
    public Vector3 Initialize(Vector3 locate)
    {
        Debug.Log("初始化角色坐标");
       this.locate.x = locate.x;
       this.locate.y = locate.y;
       this.locate.z = locate.z;
       charactorData.mytransform.position = locate;
        Debug.Log("Initialize complete!" + locate);
        return charactorData.mytransform.position;
    }
    public CharactorData Initialize(int hp, int maxstep)
    {
        Debug.Log("初始化角色最大步数与血量");
        charactorData.HP = hp;
        charactorData.MaxStep = maxstep;
        Debug.Log("Initialize complete HP:" + charactorData.HP + " Maxstep:" + charactorData.MaxStep);
        return charactorData;
    }
    public Vector3 Move(Vector3 locate)
    {
        this.charactorData.mytransform.position = locate;
        return locate;
    }
	// Use this for initialization
	void Start () {
        
        charactorData.mytransform = transform.GetComponent<Transform>();
        this.Initialize(locate);
        this.Initialize(HP, MaxStep);
       
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
