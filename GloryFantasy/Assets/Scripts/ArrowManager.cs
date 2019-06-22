using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;

public class ArrowManager : UnitySingleton<ArrowManager>
{

    private GameObject _cameraObject;

    /// <summary>
    /// 遮罩
    /// </summary>
    private Transform _arrowMaskTfm;

    /// <summary>
    /// 节点容器
    /// </summary>
    private Transform _nodesContainerTfm;

    /// <summary>
    /// 起始点位置
    /// </summary>
    private int _initialIndex;

    private Transform _tempNodeTfm;

    /// <summary>
    /// 箭头可见长度
    /// </summary>
    private float _visibleLen = 15f;

    /// <summary>
    /// 箭头流动速度
    /// </summary>
    [Range(1f,300f)]
    public float flowSpeed = 15f;

    private RectTransform _maskRect;

    /// <summary>
    /// 起始点坐标
    /// </summary>
    private Vector2 _startPos;

    [Range(0f,12f)]
    public float offset = 0.5f;

    public GameObject arrowNode;

    /// <summary>
    /// 箭头高度
    /// </summary>
    [Range(0f,12f)]
    private const float MinHeight = 1f;

    private bool _canShowArrow = false;

    private float _dist;

    // Use this for initialization
    void Start ()
    {
        this.gameObject.GetComponent<SpriteRenderer>().enabled = false;
        Initialize();
    }

    void FixedUpdate()
    {
        if (_canShowArrow && Input.GetMouseButton(1))        // 右键取消箭头显示
        {
            HideArrow();
        }

        if (_canShowArrow)
        {
            UpdateArrow(Input.mousePosition);
        }
    }

    /// <summary>
    /// 初始化各组件
    /// </summary>
    void Initialize()
    {
        _canShowArrow = false;
        _cameraObject = GameObject.Find("Main Camera");
        _arrowMaskTfm = transform.GetChild (0);
        _maskRect = _arrowMaskTfm.GetComponent<RectTransform> ();
        _nodesContainerTfm = _arrowMaskTfm.Find ("Container");
    }

    /// <summary>
    /// 箭身流动效果
    /// 先不实现了，因为效果不好，等之后有时间再看
    /// </summary>
    /// <param name="position"></param>
    public void MakeArrowFlow(Vector3 position)
    {
        if (!this.gameObject.GetComponent<SpriteRenderer>().enabled)
        {
            ShowArrow();
        }
//       if (!_mActive)
//           return;
//       // 改变箭头前端透明度
//       for(int i =0;i<_nodesContainerTfm.childCount;i++)
//       {
//           _tempNodeTfm = _nodesContainerTfm.GetChild(i);
//           _tempNodeTfm.localPosition = new Vector3(0f,_tempNodeTfm.localPosition.y+Time.fixedDeltaTime*flowSpeed,0f);
//           // 改变箭头起点透明度
//           _initialIndex = (int)(_visibleLen/1f);
//           if (i <= 2)
//           {
//               _tempNodeTfm.GetComponent<SpriteRenderer> ().color = Color.Lerp (_tempNodeTfm.GetComponent<SpriteRenderer> ().color, new Color (1, 1, 1, (60 * i + 60) / 255f), Time.fixedDeltaTime * 5f);
//           }
//           else if (i <= (_initialIndex + 3) && i >= (_initialIndex - 3))
//           {
//               int diff = i - (_initialIndex - 3);
//               _tempNodeTfm.GetComponent<SpriteRenderer> ().color = Color.Lerp (_tempNodeTfm.GetComponent<SpriteRenderer> ().color, new Color (1, 1, 1, (255f - 40f * diff) / 255f), Time.fixedDeltaTime * 5f);
//           }
//           else if (i > (_initialIndex + 3))
//           {
//               _tempNodeTfm.GetComponent<SpriteRenderer> ().color = new Color (1, 1, 1, 0);
//           }
//           else
//           {
//               _tempNodeTfm.GetComponent<SpriteRenderer> ().color = Color.white;
//           }
//
//           if (_tempNodeTfm.localPosition.y > -100f)
//           {
//               _tempNodeTfm.GetComponent<SpriteRenderer>().color = Color.white;
//               _tempNodeTfm.localPosition = new Vector3(0f,-100 + _nodesContainerTfm.GetChild(_nodesContainerTfm.childCount-1).localPosition.y,0f);
//               _tempNodeTfm.SetAsLastSibling();
//           }
//
//       }

//        if (!_mActive || Mathf.Abs((position - _lastPos).magnitude) < 0.001f)
//            return;
        //改变箭头前端的透明度
        if(!_canShowArrow)
            return;
        int nodeCount = _nodesContainerTfm.childCount;    // 当前箭身节点个数
        int len = (int) _visibleLen;                        // 可显示长度
        _nodesContainerTfm.GetComponent<RectTransform>().sizeDelta = new Vector2(3f, len);
        // 显示长度大，添加节点
        if (nodeCount < len)
        {
            for (int j = nodeCount; j < len; j++)
            {
                GameObject arrowNodeInstance = Instantiate(arrowNode, new Vector3(1.5f, -j, 0), new Quaternion(0f, 0f, 0f, 0f), _nodesContainerTfm);
                arrowNodeInstance.transform.localScale = new Vector3(1f, 1.3f, 1f);
            }
        }
        // 否则删除多余节点
        else
        {
            for (int j = len; j < nodeCount; j++)
            {
                Destroy(_nodesContainerTfm.GetChild(j).gameObject);
            }
        }
    }

    /// <summary>
    /// 画箭头，外部调用接口
    /// </summary>
    /// <param name="startPos"></param>
    /// <param name="endPos"></param>
    /// <returns></returns>
    public void DrawArrow(Vector3 startPos, Vector3 endPos)
    {
//        Initialize();
        _canShowArrow = true;
        transform.localScale = Vector3.one;
        transform.position = ScreenPosToWorldPos(endPos);
        _startPos = ScreenPosToWorldPos(startPos);
    }

    /// <summary>
    /// 把屏幕上的点转换成世界空间
    /// </summary>
    /// <param name="screenPos"></param>
    /// <returns></returns>
    private Vector3 ScreenPosToWorldPos(Vector3 screenPos)
    {
        screenPos.z = 5.0f;
        Vector3 p1 = _cameraObject.GetComponent<Camera>().ScreenToWorldPoint(screenPos);
        Vector3 p0 = _cameraObject.GetComponent<Camera>().transform.position;
        float h2 = 0.0f - p0.z;
        float h1 = p1.z - p0.z;
        Vector3 dir = p1 - p0;
        dir.z = 0.0f;
        float d1 = dir.magnitude;
        float d2 = d1 * h2 / h1;
        Vector3 p2 = p0 + dir.normalized * d2;
        p2.z = 0;
        return p2;
    }

    /// <summary>
    /// 显示箭头
    /// </summary>
    public void ShowArrow()
    {
        this.gameObject.GetComponent<SpriteRenderer>().enabled = true;
    }

    /// <summary>
    /// 隐藏箭头
    /// </summary>
    public void HideArrow()
    {
        for (int i = 0; i < _nodesContainerTfm.childCount; i++)
        {
            Destroy(_nodesContainerTfm.GetChild(i).gameObject);        // 删除各箭身节点
        }
        this.gameObject.GetComponent<SpriteRenderer>().enabled = false;    // 隐藏箭头
        _canShowArrow = false;
    }

    /// <summary>
    /// 更新箭头显示
    /// </summary>
    /// <param name="position">当前鼠标位置</param>
    public void UpdateArrow(Vector3 position)
    {
        Vector3 mousePos = ScreenPosToWorldPos(position);
        transform.position = mousePos;
        transform.rotation = CaculateRotation (mousePos,_startPos);
        CaculateVisibleLen(mousePos);
        _dist = _visibleLen + offset;
        _dist = _dist >= MinHeight ? _dist : MinHeight;
        _maskRect.sizeDelta = new Vector2 (1f, _dist);
        MakeArrowFlow(position);
    }


    /// <summary>
    /// 计算可见长度
    /// </summary>
    /// <param name="currentPos">Current position.</param>
    void CaculateVisibleLen(Vector2 currentPos)
    {
        Vector2 dirVector = currentPos - _startPos;
        _visibleLen = dirVector.magnitude;
    }

    /// <summary>
    /// 输入当前位置，获得正确转向
    /// </summary>
    /// <returns>The rotation.</returns>
    /// <param name="currentPos">Current position.</param>
    /// <param name="middlePos"></param>
    Quaternion CaculateRotation(Vector2 currentPos,Vector2 middlePos)
    {
        Vector2 fromVector = Vector2.up;
        Vector2 toVector = currentPos - middlePos;
        float angle = Vector2.Angle (fromVector, toVector);
        if (toVector.x > 0)
        {
            angle = 360f - angle;
        }
        // 组合得到欧拉角
        Vector3 diff = new Vector3 (0f, 0f, angle);
        // 将欧拉角转换成四元数
        Quaternion rotation = Quaternion.Euler (diff);
        return rotation;
    }
}
