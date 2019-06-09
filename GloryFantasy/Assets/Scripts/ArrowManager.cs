using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ArrowManager : UnitySingleton<ArrowManager> 
{
    //���ʱ��
    [FormerlySerializedAs("_FixedTime")] [SerializeField]
    private float fixedTime = 0.05f;

    // �ٶ�  ��֤�ٶȾ��ȣ�·��Խ��ʱ��Խ��
    [FormerlySerializedAs("_Speed")] [SerializeField]
    private float speed = 10;

    private Vector3 offsetPos = new Vector3(-50f, -60f, 0f);
    private Vector3 _startPos;
    private bool _canShowArrow;
    private GameObject _arrowCameraGameObject;
    private GameObject _arrowMesh;
    
    public float Speed
    {
        set
        {
            speed = value;
        }
        get
        {
            return speed;
        }
    }
    //��ͷ�Ŀ��
    [FormerlySerializedAs("_ArrowWidth")] [SerializeField]
    private float arrowWidth = 2.0f;

    private MeshFilter _meshFilter;

    private void Start()
    {
        _canShowArrow = false;
        _meshFilter = GetComponent<MeshFilter>();
        _arrowCameraGameObject = GameObject.Find("ArrowCamera");
        _arrowMesh = GameObject.Find("ArrowMesh");
    }

    private void FixedUpdate()
    {
        if (_canShowArrow)
        {
            UpdateArrow(_startPos, Input.mousePosition);
        }
    }

    /// <summary>
    /// չʾ��ͷ
    /// </summary>
    /// <param name="startPos">��ͷ���</param>
    public void showArrow(Vector3 startPos)
    {
        _startPos = startPos;
        _canShowArrow = true;
        _arrowMesh.SetActive(true);
    }

    /// <summary>
    /// ���ؼ�ͷ
    /// </summary>
    public void hideArrow()
    {
        _startPos = Vector3.zero;
        _canShowArrow = false;
        _meshFilter.mesh = null;
        _arrowMesh.SetActive(false);
    }
    
    /// <summary>
    /// ����������¼�ͷ
    /// </summary>
    /// <param name="startPos"></param>
    /// <param name="endPos"></param>
    public void UpdateArrow(Vector3 startPos, Vector3 endPos)
    {
        if(!_meshFilter)
            _meshFilter = GetComponent<MeshFilter>();
//        endPos += offsetPos;
//        startPos += offsetPos;
        Vector3 startPosInWorld = screenToWorldPos(startPos);
        Vector3 endPosInWorld = screenToWorldPos(endPos);
        List<Vector3> posList = GetRadianPos(startPosInWorld, endPosInWorld);
        CreateMesh(_meshFilter, posList);
    }


    /// <summary>
    /// ����Ļ����ת������������
    /// </summary>
    /// <param name="screenPos">��Ļ����</param>
    /// <returns>��������</returns>
    private Vector3 screenToWorldPos(Vector3 screenPos)
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
    
    #region ����ģ��
    /// <summary>
    /// ����ģ��
    /// </summary>
    /// <param name="meshFilter"></param>
    /// <param name="posList">����֮���ļ���</param>
    void CreateMesh(MeshFilter meshFilter, List<Vector3> posList)
    {
        int num = posList.Count -1;
        if (num < 1)
            return;
        float halfWidth = arrowWidth * 0.5f;
        Vector3 dir = GetDir(posList[0], posList[num]);

        Vector3[] vertices = new Vector3[num * 4];
        Vector2[] uv = new Vector2[num * 4];
        int[] triangle = new int[num * 6];
        for (int i = 0; i < num; i++)
        {
            //���㶥��λ��  
            vertices[i * 4 + 0] = posList[i] + dir* halfWidth;
            vertices[i * 4 + 1] = posList[i + 1] - dir * halfWidth ;
            vertices[i * 4 + 2] = posList[i + 1] + dir * halfWidth ;
            vertices[i * 4 + 3] = posList[i] - dir * halfWidth;

            //����uvλ��  
            uv[i * 4 + 0] = new Vector2(0.0f, 0.0f);
            uv[i * 4 + 1] = new Vector2(1.0f, 1.0f);
            uv[i * 4 + 2] = new Vector2(1.0f, 0.0f);
            uv[i * 4 + 3] = new Vector2(0.0f, 1.0f);
        }

        int verticeIndex = 0;

        for (int i = 0; i < num; i++)
        {
            // ��һ��������  
            triangle[verticeIndex++] = i * 4 + 0;
            triangle[verticeIndex++] = i * 4 + 1;
            triangle[verticeIndex++] = i * 4 + 2;
            // �ڶ���������  
            triangle[verticeIndex++] = i * 4 + 1;
            triangle[verticeIndex++] = i * 4 + 0;
            triangle[verticeIndex++] = i * 4 + 3;
        }
        Mesh newMesh = new Mesh();
        newMesh.vertices = vertices;
        newMesh.uv = uv;
        newMesh.triangles = triangle;
        meshFilter.mesh = newMesh;
    }
    #endregion

    #region ��ȡ��ͷ�Ĵ�ֱ����
    /// <summary>
    /// 
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <returns></returns>
    private Vector3 GetDir(Vector3 start,Vector3 end)
    {
        Vector3 dirValue = (end - start).normalized;
        //��Ϊ����Ҫ����z�����������һ�����������ɵó�Ψһ��ֱ����
        Vector2 dir = new Vector2(Mathf.Abs(dirValue.y),-1.0f * Mathf.Sign(dirValue.x* dirValue.y) * Mathf.Abs(dirValue.x));
        if (dirValue.y < 0)
            dir *= -1.0f;
        return dir;
    }
    #endregion

    #region ��ȡ����֮��ĵ�
    /// <summary>
    /// ��ȡ����֮��ĵ�
    /// </summary>
    /// <param name="startPos">���</param>
    /// <param name="endPos">�յ�</param>
    /// <returns>����֮��ֱ�ߵĵ�ļ���</returns>
    private List<Vector3> GetRadianPos(Vector3 startPos, Vector3 endPos)
    {
        List<Vector3> posList = new List<Vector3>();

        float lifeTime = Vector3.Distance(startPos, endPos) / speed;

        Vector3 startSpeed = (endPos - startPos) / lifeTime;
        for (float moveTime = 0.0f; moveTime <= lifeTime; moveTime += fixedTime)
        {
            if (moveTime > lifeTime)
                moveTime = lifeTime;
            Vector3 tempPos = startPos + startSpeed * moveTime;
            posList.Add(tempPos);
        }
        return posList;
    }
    #endregion
}