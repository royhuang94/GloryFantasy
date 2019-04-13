using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GFCharactor;
/// <summary>定义六边形坐标的结构体，并处理坐标转换
/// 
/// </summary>
public struct HexVector
{
    /// <summary>
    /// 
    /// </summary>
    public Vector3 Hex_vector;
    /// <summary>世界坐标
    /// 
    /// </summary>
    public Vector3 Normal_vector;
    public Vector3 ChangeToNormalVect(Vector3 vector)
    {
        //  HexVector hexVector = new HexVector();
        Normal_vector.x = 1.5f * vector.x + 1.5f * vector.z;
        Normal_vector.y = vector.y;
        Normal_vector.z = -vector.x + vector.z;
        vector.x = Normal_vector.x;
        vector = Normal_vector;
        return vector;

    }
    public Vector3 ChangeToHexVect(Vector3 vector)
    {
        Hex_vector.x = vector.x / 3f - 0.5f * vector.z;
        Hex_vector.y = vector.y;
        Hex_vector.z = vector.x / 3f + 0.5f * vector.z;
        vector = Hex_vector;
        return vector;
    }

}
/// <summary>在这个类里读取地图文件并生成地图
/// 
/// </summary>
public class MainMapManager : MonoBehaviour
{
    /// <summary>地格材质，测试用，运行的时候找一个Unity默认的材质加上去就行。
    /// 
    /// </summary>
    public Mesh mesh;
    /// <summary>人物角色实例。
    /// 
    /// </summary>
    public Charactor charactor;
    /// <summary>角色起始所在位置。
    /// 
    /// </summary>
    /// <summary>储存角色周围地格信息的数据结构
    ///
    /// </summary>
    public Dictionary<string, MapUnit> AroundList = new Dictionary<string, MapUnit>();
    /// <summary>初始化，设定
    /// 
    /// </summary>
    void Awake()
    {
        AroundList.Add("0,1", null);
        AroundList.Add("0,-1", null);
        AroundList.Add("1,0", null);
        AroundList.Add("-1,0", null);
        AroundList.Add("-1,1", null);
        AroundList.Add("1,-1", null);
        //测试用
        string[,] simtext = new string[3, 3]
        {
            {"plane","plane","post" },
            { "plane","plane","plane"},
            {"post","plane","plane" },

        };

        for (int i = 0; i < simtext.GetLength(0); i++)
        {
            for (int j = 0; j < simtext.GetLength(1); j++)
            {
                GameObject gameObject = new GameObject("test" + i.ToString() + j.ToString());
                gameObject.transform.parent = GameObject.Find("Map").transform;
                HexVector vector = new HexVector();
                gameObject.AddComponent<Button>();
                gameObject.AddComponent<MapUnit>();
                MeshFilter flit = gameObject.AddComponent<MeshFilter>();
                gameObject.AddComponent<MeshRenderer>();
                gameObject.AddComponent<BoxCollider>();
                flit.mesh = mesh;
                gameObject.transform.position = vector.ChangeToNormalVect(new Vector3(i, 0, j));
                gameObject.GetComponent<MapUnit>().SethexVector();
                switch (simtext[i, j])
                {
                    case "plane":
                        break;
                    case "post":
                        gameObject.AddComponent<Post>();
                        break;
                }
                //   Debug.Log(gameObject.transform.position);
            }

        }




    }
    private void Start()
    {
        charactor = GameObject.Find("Charactor").GetComponent<Charactor>();
        charactor.Initalize();

    }
    /// <summary>移动逻辑合法后重设角色周围地形格写入AroundList
    /// 
    /// </summary>
    /// <param name="onclk"></param>
    /// <returns></returns>
    public Dictionary<string, MapUnit> setaround(GameObject onclk)
    {

        AroundList["0,1"] = SetAround(onclk, 0, 1);
        AroundList["0,-1"] = SetAround(onclk, 0, -1);
        AroundList["1,0"] = SetAround(onclk, 1, 0);
        AroundList["-1,0"] = SetAround(onclk, -1, 0);
        AroundList["-1,1"] = SetAround(onclk, -1, 1);
        AroundList["1,-1"] = SetAround(onclk, 1, -1);

        return AroundList;
    }

    // Debug.Log("playeraround");

    /// <summary>重设地形格字典值的具体逻辑
    /// 
    /// </summary>
    /// <param name="onclk"></param>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public MapUnit SetAround(GameObject onclk, float a, float b)
    {
        MapUnit playeraround;
        float x = onclk.GetComponent<MapUnit>().hexVector.Hex_vector.x + a;
        // float x = onclk.GetComponent<Transform>().position.x+1;
        float z = onclk.GetComponent<MapUnit>().hexVector.Hex_vector.z + b;
        if (GameObject.Find("test" + x.ToString() + z.ToString()) != null)
        {
            playeraround = GameObject.Find("test" + x.ToString() + z.ToString()).GetComponent<MapUnit>();
        }
        else

        {
            Debug.Log("doesn't have this object");
            playeraround = null;
        }
        return playeraround;


    }



}
/// <summary>六边形单元格,每个地格都会挂载这个脚本。
/// 
/// </summary>
public class MapUnit : MonoBehaviour
{
    public static MainMapManager mapManager;
    /// <summary>储存地格坐标的结构体，注意地格自身的世界坐标（Vector3 Normal_vector）也在这个结构里。
    /// 
    /// </summary>
    /// 
    public HexVector hexVector = new HexVector();
    /// <summary>初始化，给所在GameObject添加Button组件，并把自己在两个坐标系的坐标写入HexVector里。
    /// 
    /// </summary>  
    public void Awake()
    {
        mapManager = gameObject.GetComponentInParent<MainMapManager>();
        Button btn = gameObject.GetComponent<Button>();
        btn.onClick.AddListener(CheckAround);

        Debug.Log("instalize mapunit.");

    }
    public HexVector SethexVector()
    {
        hexVector.Normal_vector = this.GetComponent<Transform>().position;
        hexVector.Hex_vector = hexVector.ChangeToHexVect(hexVector.Normal_vector);
        return hexVector;
    }
    /// <summary>检测被点击的地格是否在角色相邻区域，
    /// 
    /// </summary>
    public void CheckAround()
    {
        if (mapManager.AroundList.ContainsValue(this))
        {
            this.DoOnclick();
        }
        else
        {
            Debug.Log("这个格子不在角色相邻区域，无法移动");
        }
    }
    /// <summary>点击合法后，大地图初始化或角色死亡后调用此函数，传到MapManager的SetAround来处理。
    /// 
    /// </summary>
    public void DoOnclick()
    {

        //  hexVector.ChangeToHexVect(hexVector.Normal_vector);
        mapManager.charactor.Move(GetComponent<Transform>().position, -1);
        mapManager.setaround(GameObject.Find("test" + mapManager.charactor.charactorData.PlayerLocate.Hex_vector.x.ToString() + mapManager.charactor.charactorData.PlayerLocate.Hex_vector.z.ToString()));
        Debug.Log("Onclick" + hexVector.Normal_vector.x.ToString() + hexVector.Normal_vector.z.ToString());

    }


}
public class Post : MapUnit
{
    private bool isActive = false;
    private static bool ReadyToTrans = false;
    public new void Awake()
    {
        Debug.Log("驿站初始化");
        Button btn = gameObject.GetComponent<Button>();
        btn.onClick.AddListener(Onclick);
    }
    public void Onclick()
    {
        if (mapManager.charactor.charactorData.PlayerLocate.Normal_vector == GetComponent<Transform>().position && ReadyToTrans == false)
        {
            isActive = true;
            Debug.Log("驿站已激活");
            Debug.Log("进入驿站");
            ReadyToTrans = true;
            Debug.Log("准备传送");
            foreach (MapUnit unit in mapManager.AroundList.Values)
            {

                if (unit != null)
                {
                    unit.gameObject.GetComponent<Button>().onClick.AddListener(CancelTrans);
                   
                }
                else
                {
                    
                }
            }
        }
        else if (ReadyToTrans == true)
        {

            if (isActive == false)
            {
                Debug.Log("所选驿站未激活。");
            }
            else
            {
                Debug.Log("指令合法，开始传送");
                transfer();
                ReadyToTrans = false;
                Debug.Log("传送完成");
            }
        }
        else
        {
            Debug.Log("你不在这个驿站");
        }
    }
    public void transfer()
    {
        mapManager.charactor.Move(GetComponent<Transform>().position, -2);
        mapManager.setaround(GameObject.Find("test" + mapManager.charactor.charactorData.PlayerLocate.Hex_vector.x.ToString() + mapManager.charactor.charactorData.PlayerLocate.Hex_vector.z.ToString()));
        ReadyToTrans = false;
    }
    public void CancelTrans()
    {
        ReadyToTrans = false;
    }
}