using System.Collections.Generic;
using UnityEngine;
using Vectrosity;
public class Draw : MonoBehaviour
{
    void Start()
    {
        Debug.Log("dfsa");
        //定义屏幕上的6个点，左下角为（0,0）
        var linePoints = new List<Vector2>() { new Vector2(200, 500),
        new Vector2(200,600), new Vector2(300, 600), new Vector2(300, 500),
        new Vector2(400,500), new Vector2(400, 600) };

        //定义对象：myline 
        var myline = new VectorLine("Line", linePoints, 2.0f,LineType.Continuous);
        myline.lineWidth = 4.0f;    //定义线宽 
        myline.Draw();
        //初始化颜色对象
        var myColors = new List<Color32>();
        myColors.Add(Color.red);
        //myColors.Add(Color.green);
        //myColors.Add(Color.white);
        //myColors.Add(Color.blue);
        //myColors.Add(Color.yellow);

        myline.smoothColor = true;  //设置线条颜色平滑
        myline.SetColor(Color.red);
    }

    void Update()
    {

    }
}