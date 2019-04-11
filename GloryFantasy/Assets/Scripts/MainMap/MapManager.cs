using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>定义六边形坐标的结构体，并处理坐标转换
/// 
/// </summary>
public struct HexVector
{
    public float Hex_x;
    public float Hex_y;
    public float Hex_z;
    public Vector3 Hex_vector;
    public Vector3 ChangeToVect(Vector3 vector)
    {
        //  HexVector hexVector = new HexVector();
        this.Hex_x = 1.5f * vector.x + 1.5f * vector.z;
        this.Hex_y = vector.y;
        this.Hex_z = -vector.x + vector.z;
        vector.x = this.Hex_x;
        vector.y = this.Hex_y;
        vector.z = this.Hex_z;
        return vector;

    }


}
/// <summary>在这个类里读取地图文件并生成地图
/// 
/// </summary>
public class MapManager : MonoBehaviour {
    public Mesh mesh;
    public MapUnit playeraround;
    // Use this for initialization
    void Start () {
        //测试用
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                GameObject gameObject = new GameObject("test"+i.ToString()+j.ToString());
                gameObject.transform.parent = GameObject.Find("Plane").transform;
                HexVector vector = new HexVector();
                Button btn = gameObject.AddComponent<Button>();
                MapUnit mapunit = gameObject.AddComponent<MapUnit>();
                MeshFilter flit = gameObject.AddComponent<MeshFilter>();
                MeshRenderer renderer = gameObject.AddComponent<MeshRenderer>();
                gameObject.AddComponent<BoxCollider>();
                flit.mesh = mesh;
                gameObject.transform.position = vector.ChangeToVect(new Vector3(i, 0, j));

             //   Debug.Log(gameObject.transform.position);
            }

        }
       // setaround(GameObject.Find("test00"));
       // Debug.Log("clicktest00");
        
    }
    public MapUnit setaround(GameObject onclk)
    {
        float x = onclk.GetComponent<MapUnit>().hexVector.Hex_x+1;
       // float x = onclk.GetComponent<Transform>().position.x+1;
        float z = onclk.GetComponent<MapUnit>().hexVector.Hex_z;
        playeraround = GameObject.Find("test" + x.ToString()+z.ToString()).GetComponent<MapUnit>();
        Debug.Log("setaround"+x +z);
       // Debug.Log("playeraround");
        return playeraround;
    }
   

	

}
/// <summary>六边形单元格的基类，
/// 
/// </summary>
public class MapUnit:MonoBehaviour
{
    public Vector3 vector;
    public HexVector hexVector;
    //public MapUnit around;
    public void Start()
    {
        Button btn = gameObject.GetComponent<Button>();
        hexVector.ChangeToVect(vector);
        btn.onClick.AddListener(Onclick);
        vector = transform.GetComponent<Transform>().position;
        Debug.Log("instalize mapunit.");

    }

    public void Onclick()
    {
        MapManager mapManager = gameObject.GetComponentInParent<MapManager>();
        mapManager.setaround(GameObject.Find("test" + vector.x.ToString() + vector.z.ToString()));
        Debug.Log("Onclick" + vector.x.ToString() + vector.z.ToString());
       // return clickunit.around;
    }


}