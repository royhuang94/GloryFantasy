using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GamePlay;
using FairyGUI;
using GameCard;


public class FGUIInterfaces : MonoBehaviour {

	// Use this for initialization
	void Start () {
		GRoot.inst.SetContentScaleFactor(960, 540);
		UIPackage.AddPackage("Package2");
		GComponent mainUI = UIPackage.CreateObject("Package1", "Component5").asCom;
		GRoot.inst.AddChild(mainUI); 
		
		// 回合结束按钮添加事件监听
		mainUI.GetChild("n5").onClick.Add(Gameplay.Instance().switchPhaseHandler);
		
		// 卡牌堆按钮添加事件监听
		mainUI.GetChild("n1").onClick.Add(CardManager.Instance().ShowCardsSetsInfo);
		
	}
	
	
}
