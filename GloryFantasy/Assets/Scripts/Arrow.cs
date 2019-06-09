using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;

public class Arrow : UnitySingleton<Arrow>
{

    private GameObject _cameraObject;
    private Camera _camera;

    RectTransform _canvasRect;


    /// <summary>
    /// The arrow mask tfm.
    /// </summary>
    Transform _arrowMaskTfm;

    Transform _nodesContainerTfm;

    Transform _startTfm;

    int _initialIndex;

    Transform _tempNodeTfm;

    /// <summary>
    /// 箭头可见长度
    /// </summary>
    float _visibleLen = 1500f;

    /// <summary>
    /// 箭头流动速度
    /// </summary>
    [Range(10f,300f)]
    public float flowSpeed = 15f;

    RectTransform _maskRect;

    /// <summary>
    /// 起始点坐标
    /// </summary>
    Vector2 _startPos;

    [Range(30f,120f)]
    public float offset = 50f;

    Vector3 _offsetV = new Vector3 (0f, 50f,0f);

    /// <summary>
    /// 箭头高度
    /// </summary>
    [Range(40f,120f)]
    const float MinHeight = 80f;

    bool _mActive = false;

    float _dist;

    // Use this for initialization
    void Start ()
    {
        Initialize();
    }

    void FixedUpdate()
    {
        if(_mActive)
            draw(Input.mousePosition);
    }

    void Initialize()
    {
        _mActive = false;
        _cameraObject = GameObject.Find("Main Camera");
        _camera = _cameraObject.GetComponent<Camera>();
        _arrowMaskTfm = transform.GetChild (0);
        _maskRect = _arrowMaskTfm.GetComponent<RectTransform> ();
        _nodesContainerTfm = _arrowMaskTfm.Find ("Container");
    }

    public void MakeArrowFlow()
    {
        Debug.Log("start to flow");
        if (!_mActive)
            return;
        // 改变箭头前端透明度
        for(int i =0;i<_nodesContainerTfm.childCount;i++)
        {
            _tempNodeTfm = _nodesContainerTfm.GetChild(i);
            _tempNodeTfm.localPosition = new Vector3(0f,_tempNodeTfm.localPosition.y+flowSpeed * 0.0000000000001f,0f);
            // 改变箭头起点透明度
            _initialIndex = (int)(_visibleLen/100f);
            if (i <= 2)
            {
                _tempNodeTfm.GetComponent<SpriteRenderer> ().color = Color.Lerp (_tempNodeTfm.GetComponent<SpriteRenderer> ().color, new Color (1, 1, 1, (60 * i + 60) / 255f), Time.fixedDeltaTime * 5f);
            }
            else if (i <= (_initialIndex + 3) && i >= (_initialIndex - 3))
            {
                int diff = i - (_initialIndex - 3);
                _tempNodeTfm.GetComponent<SpriteRenderer> ().color = Color.Lerp (_tempNodeTfm.GetComponent<SpriteRenderer> ().color, new Color (1, 1, 1, (255f - 40f * diff) / 255f), Time.fixedDeltaTime * 5f);
            }
            else if (i > (_initialIndex + 3))
            {
                _tempNodeTfm.GetComponent<SpriteRenderer> ().color = new Color (1, 1, 1, 0);
            }
            else
            {
                _tempNodeTfm.GetComponent<SpriteRenderer> ().color = Color.white;
            }

            if (_tempNodeTfm.localPosition.y > -100f)
            {
                _tempNodeTfm.GetComponent<SpriteRenderer>().color = Color.white;
                _tempNodeTfm.localPosition = new Vector3(0f,-100 + _nodesContainerTfm.GetChild(_nodesContainerTfm.childCount-1).localPosition.y,0f);
                _tempNodeTfm.SetAsLastSibling();
            }
        }
    }

    public bool ready(Vector3 startPos, Vector3 endPos)
    {
        Debug.Log("ready to draw from " + startPos);
        _mActive = true;
        transform.localScale = Vector3.one;
        transform.position = getPos(endPos);
        _startPos = getPos(startPos);
        MakeArrowFlow();
        return true;
    }

    private Vector3 getPos(Vector3 screenPos)
    {
        screenPos.z = 5.0f;
        Vector3 p1 = GameObject.Find("Main Camera").GetComponent<Camera>().ScreenToWorldPoint(screenPos);
        Vector3 p0 = GameObject.Find("Main Camera").GetComponent<Camera>().transform.position;
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

    public void draw(Vector3 position)
    {
        Vector3 mouse = getPos(Input.mousePosition);
        transform.position = mouse;
        transform.rotation = CaculateRotation (mouse,_startPos);
        CaculateVisibleLen(mouse);
        _dist = _visibleLen + offset;
        _dist = _dist >= MinHeight ? _dist : MinHeight;
        _maskRect.sizeDelta = new Vector2 (100f, _dist);
        MakeArrowFlow();
    }


    /// <summary>
    /// 计算可见长度
    /// </summary>
    /// <param name="currentPos">Current position.</param>
    void CaculateVisibleLen(Vector2 currentPos )
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
