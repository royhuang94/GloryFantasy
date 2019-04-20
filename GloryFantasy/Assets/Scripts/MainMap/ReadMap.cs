using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>这个脚本是我（张骑）测试读取文件的时候乱写的，跟游戏无关！
/// 
/// </summary>
public class ReadMap : MonoBehaviour {
    public TextAsset textAsset;
	// Use this for initialization
	void Start () {
        System.StringSplitOptions option = System.StringSplitOptions.RemoveEmptyEntries;
        string[] lines = textAsset.text.Split(new char[] { '\r', '\n' }, option);
      for(int i=0;i<lines.Length; i++)
        {
            Debug.Log(lines[i]);
        }
        for(int i=0;i<3;i++)
        {
            string[] element = lines[i].Split(',');
            for(int j=0;j<element.Length; j++)
            {
                Debug.Log(element[j]);
            }
           
        }

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
