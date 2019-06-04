using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallShaderTest : MonoBehaviour {

	// Use this for initialization
	public Shader selectedShader;
	public Color outterColor;
 
	
	private Color myColor ;
	private Shader myShader;
	private bool Selected = false;
	
	// Use this for initialization
	void Start()
	{
		//保存原来的颜色值和shader,以便鼠标移出时恢复
		myColor = GetComponent<Renderer>().material.color;
		myShader = GetComponent<Renderer>().material.shader;
		//鼠标移入时要使用的shader
		selectedShader = Shader.Find("Sprites/2DOutline");
		if (!selectedShader)
		{
			enabled = false;
			return;
		}
	}

	void OnMouseEnter() 
	{
		//替换Shader
		GetComponent<Renderer>().material.shader = selectedShader;
		//设置边缘光颜色值
		GetComponent<Renderer>().material.SetColor("_RimColor", outterColor);
		//设置纹理颜色值
		GetComponent<Renderer>().material.SetColor("_MainColor", myColor);
	}
	//鼠标移出
	void OnMouseExit()
	{
		//恢复颜色值
		GetComponent<Renderer>().material.color = myColor;
		//恢复shader
		GetComponent<Renderer>().material.shader = myShader;

	}
}
