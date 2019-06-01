using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ArrowMesh : UnitySingleton<ArrowMesh> {

    // 重力加速度
    [SerializeField]
    private Vector3 G = new Vector3(0.0f, 0.0f, 9.8f);
    //间隔时间
    [FormerlySerializedAs("_FixedTime")] [SerializeField]
    private float fixedTime = 0.05f;

    // 速度  保证速度均匀，路径越长时间越长
    [FormerlySerializedAs("_Speed")] [SerializeField]
    private float speed = 10;

    private Vector3 pos;
    private Camera _camera;
    private RectTransform _canvas;
    private Vector3 _worldStartPosInRect;
    private Vector3 _worldEndPosInRect;
    private Vector3 offsetPos = new Vector3(-50f, -60f, 0f); 

    private bool canShow = false;
    
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
    //箭头的宽度
    [FormerlySerializedAs("_ArrowWidth")] [SerializeField]
    private float arrowWidth = 2.0f;

    private MeshFilter _meshFilter;

    private void Start()
    {
        _meshFilter = GetComponent<MeshFilter>();
        _camera = GameObject.Find("Main Camera").GetComponent<Camera>();
    }

    public void UpdatePosition(Vector3 startPos, Vector3 endPos)
    {
        if(!_meshFilter)
            _meshFilter = GetComponent<MeshFilter>();
        endPos += offsetPos;
        startPos += offsetPos;
        List<Vector3> posList = GetRadianPos(startPos, endPos);
        CreateMesh(_meshFilter, posList);
    }

    
    #region 创建模型
    void CreateMesh(MeshFilter meshFilter, List<Vector3> pos)
    {
        Debug.Log("mesh: " + meshFilter);
        int num = pos.Count -1;
        if (num < 1)
            return;
        float halfWidth = arrowWidth * 0.5f;
        Vector3 dir = GetDir(pos[0], pos[num]);

        //Vector3[] _vertices = new Vector3[_num*4+8];
        //Vector2[] _uv = new Vector2[_num * 4 + 8];
        //int[] _triangle = new int[_num * 6 + 12];
        Vector3[] vertices = new Vector3[num * 4];
        Vector2[] uv = new Vector2[num * 4];
        int[] triangle = new int[num * 6];
        for (int i = 0; i < num; i++)
        {
            //计算顶点位置  
            vertices[i * 4 + 0] = pos[i] + dir* halfWidth;
            vertices[i * 4 + 1] = pos[i + 1] - dir * halfWidth ;
            vertices[i * 4 + 2] = pos[i + 1] + dir * halfWidth ;
            vertices[i * 4 + 3] = pos[i] - dir * halfWidth;

            //计算uv位置  
            uv[i * 4 + 0] = new Vector2(0.0f, 0.0f);
            uv[i * 4 + 1] = new Vector2(1.0f, 1.0f);
            uv[i * 4 + 2] = new Vector2(1.0f, 0.0f);
            uv[i * 4 + 3] = new Vector2(0.0f, 1.0f);
        }

        int verticeIndex = 0;

        for (int i = 0; i < num; i++)
        {
            // 第一个三角形  
            triangle[verticeIndex++] = i * 4 + 0;
            triangle[verticeIndex++] = i * 4 + 1;
            triangle[verticeIndex++] = i * 4 + 2;
            // 第二个三角形  
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

    #region 获取箭头的垂直向量
    private Vector3 GetDir(Vector3 start,Vector3 end)
    {
        Vector3 dirValue = (end - start).normalized;
        //因为不需要考虑z轴的向量，加一个条件，即可得出唯一垂直向量
        Vector2 dir = new Vector2(Mathf.Abs(dirValue.y),-1.0f * Mathf.Sign(dirValue.x* dirValue.y) * Mathf.Abs(dirValue.x));
        if (dirValue.y < 0)
            dir *= -1.0f;
        return dir;
    }
    #endregion

    #region 获取两点之间的点
    private List<Vector3> GetRadianPos(Vector3 startPos, Vector3 endPos)
    {
        List<Vector3> posList = new List<Vector3>();

        float lifeTime = Vector3.Distance(startPos, endPos) / speed;

        Vector3 startSpeed = (endPos - startPos) / lifeTime;
        for (float moveTime = 0.0f; moveTime <= lifeTime; moveTime+= fixedTime)
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