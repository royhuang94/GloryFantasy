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
    /// <summary>传入六边形坐标数值转换成世界坐标
    /// 
    /// </summary>
    /// <param name="vector"></param>
    /// <returns></returns>
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
    /// <summary>传入世界坐标数值转换成六边形坐标
    /// 
    /// </summary>
    /// <param name="vector"></param>
    /// <returns></returns>
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

    public TextAsset textAsset;
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
        //通过读取文件里的字符串转换成对应的地格生成地图
        System.StringSplitOptions option = System.StringSplitOptions.RemoveEmptyEntries;
        string[] lines = textAsset.text.Split(new char[] { '\r', '\n' }, option);
        for (int i = 0; i < lines.Length; i++)
        {
            string[] element = lines[i].Split(',');
            for (int j = 0; j < element.Length; j++)
            {
                if (element[j] != "null")//如果字符串不为null,则生成地格挂载脚本。
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
                    switch (element[j])
                    {
                        case "plane":
                            break;
                        case "post":
                            gameObject.AddComponent<Post>();
                            break;

                    }
                }
              
               
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
/// <summary>六边形单元格,每个地格都会挂载这个脚本，负责处理角色移动，地格坐标信息也都在这里
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
    /// <summary>点击合法后，传到MapManager的SetAround来处理。
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
/// <summary>每个驿站都会挂这个脚本，负责控制传送逻辑
/// 
/// </summary>
public class Post : MapUnit
{
    /// <summary>驿站是否激活
    /// 
    /// </summary>
    private bool isActive = false;
    /// <summary>角色踩在驿站上会把ReadyToTrans设置为true
    /// 
    /// </summary>
    private static bool ReadyToTrans = false;
    public new void Awake()
    {
        Debug.Log("驿站初始化");
        Button btn = gameObject.GetComponent<Button>();
        btn.onClick.AddListener(Onclick);
    }
    /// <summary>点击驿站格子后触发的事件
    /// 
    /// </summary>
    public void Onclick()
    {
        if (mapManager.charactor.charactorData.PlayerLocate.Normal_vector == GetComponent<Transform>().position && ReadyToTrans == false)
        {
            isActive = true;
            Debug.Log("驿站已激活");
            Debug.Log("进入驿站");
            ReadyToTrans = true;
            Debug.Log("准备传送");
            //如果放弃传送移动到驿站相邻格子会重新把readytotrans设置为false,这里实现的很蠢，等结合UI就可以通过按钮监听canceltrans了0.0
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
    /// <summary>传送的具体方法
    /// 
    /// </summary>
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