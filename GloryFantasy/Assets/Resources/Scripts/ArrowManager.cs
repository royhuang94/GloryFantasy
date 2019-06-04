using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;

public class ArrowManager : UnitySingleton<ArrowManager>
{

    public static ArrowManager instance;
    private GameObject _cameraObject;
    private Camera _camera;

    /// <summary>
    /// ��ͷ�۽���Բ��
    /// </summary>
    Transform _focusRingTfm;

    RectTransform _canvasRect;

    /// <summary>
    /// ����Ļ�ϵ�һ��ͶӰ��RectTransform�ϵ�����ռ�����
    /// </summary>
    Vector3 _worldPosInRect;

    //Transform arrowHeadTfm ;

    /// <summary>
    /// The arrow mask tfm.
    /// </summary>
    Transform _arrowMaskTfm;

    Transform _nodesContainerTfm;

    /// <summary>
    /// ��ͷ���ĸ�������ʼ��
    /// </summary>
    Transform _startTfm;

    /// <summary>
    /// ��ͷ����Ĵӵڼ������ӡ���Node��ʼ
    /// </summary>
    int _initialIndex;

    /// <summary>
    /// ��ʱ��Node�ڵ�
    /// </summary>
    Transform _tempNodeTfm;

    /// <summary>
    /// ��ͷ�Ŀɼ�����
    /// </summary>
    float _visibleLen = 1500f;

    /// <summary>
    /// ��ͷ�������ٶ�
    /// </summary>
    [Range(10f,300f)]
    public float flowSpeed = 15f;

    /// <summary>
    /// ���ּ�ͷNode��RectTransform
    /// </summary>
    RectTransform _maskRect;

    /// <summary>
    /// Rectangle����ק����ʼλ�����������
    /// </summary>
    Vector2 _dragStartPos;

    [Range(30f,120f)]
    public float offset = 50f;

    Vector3 _offsetV = new Vector3 (0f, 50f,0f);

    /// <summary>
    /// Arrow �� Head ���ֵĸ߶�
    /// </summary>
    [Range(40f,120f)]
    const float MinHeight = 80f;

    /// <summary>
    /// ��ͷ��ǰ�Ƿ��ڼ���״̬
    /// </summary>
    bool _mActive = false;

    float _dist;

    protected void Awake ()
    {
        instance = this;
    }

    // Use this for initialization
    void Start ()
    {

    }

//    void FixedUpdate()
//    {
//        Debug.Log("ArrowManager--Update");
////        MakeArrowFlow ();
//    }

    void Initialize()
    {
        //arrowHeadTfm = transform.Find ("ArrowHead");
        Debug.Log("call start in am");
        _mActive = false;
        _cameraObject = GameObject.Find("Main Camera");
        _camera = _cameraObject.GetComponent<Camera>();
        _arrowMaskTfm = transform.GetChild (0);
        Debug.Log("name: " + transform.name);
        _maskRect = _arrowMaskTfm.GetComponent<RectTransform> ();
        _nodesContainerTfm = _arrowMaskTfm.Find ("Container");
        _canvasRect = GameObject.Find ("BattleCanvas").GetComponent<RectTransform> ();
    }

    public void MakeArrowFlow()
    {
        Debug.Log("");
        if (!_mActive)
            return;
        //�ı��ͷǰ�˵�͸����
        for(int i =0;i<_nodesContainerTfm.childCount;i++)
        {
            _tempNodeTfm = _nodesContainerTfm.GetChild(i);
            _tempNodeTfm.localPosition = new Vector3(0f,_tempNodeTfm.localPosition.y+Time.fixedDeltaTime*flowSpeed,0f);
            //�ı��ͷ����͸����
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

    public bool ready(Vector3 position)
    {
        Initialize ();
        Debug.Log("ready to draw from " + position);
        _mActive = true;
        transform.localScale = Vector3.one;
        Vector3 startObjPos = position;
        WorldPointInRectangle (_canvasRect, startObjPos, _camera, out _worldPosInRect);
        transform.position = _worldPosInRect;
        _dragStartPos = _worldPosInRect;
        return true;
    }

    /// <summary>
    /// ������ռ���һ��ͶӰ��Ŀ��Rectangle�ϣ��õ�ͶӰ��������ռ��е�����
    /// </summary>
    /// <param name="rect">Rect.</param>
    /// <param name="worldPos">World position.</param>
    /// <param name="camera">Camera.</param>
    /// <param name="worldPosInRect">World position in rect.</param>
    void WorldPointInRectangle(RectTransform rect,Vector3 worldPos,Camera camera, out Vector3 worldPosInRect)
    {
        Vector3 screenPos = _camera.WorldToScreenPoint (worldPos);
        RectTransformUtility.ScreenPointToWorldPointInRectangle (rect, screenPos, camera, out worldPosInRect);
    }

    public void OnDrag(Vector3 position)
    {
        //transform.position = eventData.pointerDrag.gameObject.transform.position;
        RectTransformUtility.ScreenPointToWorldPointInRectangle (_canvasRect, position, _camera, out _worldPosInRect);
        Debug.Log("onDrag: " + transform.name);
        transform.position = _worldPosInRect;
        transform.rotation = CaculateRotation (_worldPosInRect,_dragStartPos);
        //dist = Vector2.Distance (worldPosInRect, dragStartPos) + offset;
        CaculateVisibleLen(_worldPosInRect);
        _dist = _visibleLen + offset;
        _dist = _dist >= MinHeight ? _dist : MinHeight;
        _maskRect.sizeDelta = new Vector2 (100f, _dist);
        MakeArrowFlow();
//        RayCastCheck ();
    }

    void RayCastCheck()
    {
        Ray ray  =_camera.ScreenPointToRay(Input.mousePosition);
        //Debug.DrawLine (ray.origin, ray.origin + 10000 * ray.direction, Color.red);
        RaycastHit hit;
        if (Physics.Raycast (ray, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer ("BattleUnit")))
        {
            Transform hitTfm = hit.collider.transform;
            if (hitTfm != _startTfm)
            {
                ShowFocusRing (hitTfm);
                RotateFocusRing (_worldPosInRect);
            }
        }
        else
        {
            HideFocusRing ();
        }
    }

    void ShowFocusRing(Transform focusTargetTfm)
    {
        //focusRingTfm.localScale = Vector3.one;
        _focusRingTfm.gameObject.SetActive(true);
        _focusRingTfm.position = focusTargetTfm.position;
    }

    /// <summary>
    /// Rotates the focus ring.
    /// </summary>
    /// <param name="currentPos">��ǰ���ͶӰ��CanvasRect�ϵ���������.</param>
    void RotateFocusRing(Vector3 currentPos)
    {
        Vector3 focusRingPosInRect;
        WorldPointInRectangle (_canvasRect, _focusRingTfm.position, _camera, out focusRingPosInRect);
        _focusRingTfm.rotation = CaculateRotation (currentPos, focusRingPosInRect);
    }

    void HideFocusRing()
    {
        //focusRingTfm.localScale = Vector3.zero;
        _focusRingTfm.gameObject.SetActive(false);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.localScale = Vector3.zero;
        this._startTfm = null;
        _mActive = false;
        HideFocusRing ();
    }

    /// <summary>
    /// �����ͷ����Ŀɼ�����
    /// </summary>
    /// <param name="currentPos">Current position.</param>
    void CaculateVisibleLen(Vector2 currentPos )
    {
        Vector2 dirVector = currentPos - _dragStartPos;
        //��ΪArrow�����Ǵ���Canvas�ϵģ�Arrow�ĳ��Ȼ��ܸ�����Ӱ��
        //���Ƚ���һ���ȳߴ绹ԭ������ռ��µĳߴ磬Ȼ�󱻸���������Ӱ�죬�õ���ȷ�ߴ�
        _visibleLen = dirVector.magnitude / _canvasRect.localScale.x;
    }

    /// <summary>
    /// ���뵱ǰ��קλ�ã���ü�ͷ����ȷת�򡪡�rotation
    /// </summary>
    /// <returns>The rotation.</returns>
    /// <param name="currentPos">Current position.</param>
    /// <param name="middlePos"></param>
    Quaternion CaculateRotation(Vector2 currentPos,Vector2 middlePos)
    {
        Vector2 fromVector = Vector2.up;
        Vector2 toVector = currentPos - middlePos;
        //��Ȼ�βε����ƺ����ǻ��з�������
        //�����ĸ��������ĸ�����
        //Ȼ��ʵ���в������֣���ֻ�᷵����������֮�����С�Ǹ����н�
        float angle = Vector2.Angle (fromVector, toVector);
        //��x��������0ʱ��Vector2.Angle �����õ��ĽǶ�Ϊ��z��˳ʱ�����
        if (toVector.x > 0)
        {
            angle = 360f - angle;
        }
        //��ϵõ�ŷ����
        Vector3 diff = new Vector3 (0f, 0f, angle);
        //��ŷ����ת��Ϊ��Ԫ��
        Quaternion rotation = Quaternion.Euler (diff);
        return rotation;
    }
}
