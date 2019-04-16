using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour , IMessage.MsgReceiver{

    public string id { get; set; }

    T IMessage.MsgReceiver.GetUnit<T>()
    {
        return this as T;
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
