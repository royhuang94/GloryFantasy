using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//TODO 实现简单的点击事件，处理单位实例化

public class UnitManager : MonoBehaviour {

    #region 单列模式
    private static UnitManager _instance;
    public static UnitManager Instance
    {
        get
        {
            if (_instance == null)
            {
                //由于GameObject.Find函数太容易消耗资源，所以我们此处只进行一次
                _instance = GameObject.Find("UnitManager").GetComponent<UnitManager>();
            }
            return _instance;
        }
    }
    #endregion

    #region 拖拽Unit
    private bool isPickedUnit = false;
    private NBearUnit.UnitUI pickedUnit; //被鼠标选中的物体 
    private Vector2 offsetPosition = new Vector2(-6.0f, -6.0f);

    public NBearUnit.UnitUI PickedUnit
    {
        get
        {
            return pickedUnit; //pickedUnit永远不会为空，因为是从prefab中复制出去的
        }
    }
    public bool IsPickedUnit
    {
        get
        {
            return isPickedUnit;
        }
    }
    #endregion

    #region 实例化战旗
    public bool IsInstantiation
    {
        get;
        private set;
    }

    private string goName;
    private Transform goTransform;
    public void CouldInstantiation(bool coudInstantiation,Transform parent ,string goName="")
    {
        IsInstantiation = coudInstantiation;
        this.goName = goName;
        goTransform = parent;

        //当 IsInstantiation 为true时，isPickedUnit 必定为false
        if (IsInstantiation)
        {
            isPickedUnit = false;
            ClonePickedUnit();
            return;
        }
        pickedUnit.Hide();

    }

    //TODO 实例化函数，clone pickedUnit
    private void ClonePickedUnit()
    {
        //TODO 需要修改，，，代码框架不变
        //TODO 遇到问题，此处应该放在Unit单位下，而不是直接加在panel下
        Debug.Log(goName);
        //var Panel = GameObject.Find("UnitUI/MainPanel_88/" + goName).transform;
        GameObject temp = GameObject.Instantiate(pickedUnit.gameObject, goTransform) as GameObject;
        //temp.transform.parent = Panel;
        //Debug.Log(temp.name);
        temp.transform.localPosition = new Vector3(6.8f, 7f, 0.0f);
        temp.GetComponent<Image>().raycastTarget = true;

        //添加
        //脚本到实例化Unit上
        //AddComponent
        temp.gameObject.AddComponent<NBearUnit.UnitMove>();

        pickedUnit.Hide();
        //Debug.Log(goTransform.position);
    }

    #endregion


    private Canvas canvas;

    private void Start()
    {
        canvas = GameObject.Find("UnitUI").GetComponent<Canvas>();
        pickedUnit = GameObject.Find("PickedUnit").GetComponent<NBearUnit.UnitUI>();
        pickedUnit.Hide();
        IsInstantiation = false;
    }

    private void LateUpdate()
    {
        if (isPickedUnit)
        {
            //TODO 拖拽Unit
            Vector2 position;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, Input.mousePosition, null, out position);
            pickedUnit.SetLocalPosition(position + offsetPosition);
        }
        
        //TODO 丢弃BUG 
        //if(isPickedUnit && Input.GetMouseButtonDown(0) && EventSystem.current.IsPointerOverGameObject(-1) == false)
        //{
        //    //TODO 判断是否是地图，是地图则实例化
        //    if(!IsInstantiation)
        //    {
        //        isPickedUnit = false;
        //        pickedUnit.Hide();
        //    }

        //}
    }



    /// <summary>
    /// “抓起” Unit
    /// </summary>
    /// <param name="unitUI">目标拾起的单位，用于传递信息</param>
    public void PickedUpUnit(NBearUnit.UnitUI unitUI)
    {
        //TODO “拾起” 单位
        pickedUnit.SetUnit();
        isPickedUnit = true;
        pickedUnit.Show();
    }

    /// <summary>
    /// 移除鼠标上的Unit
    /// </summary>
    public void RemoveAllItem()
    {
        isPickedUnit = false;
        PickedUnit.Hide();
    }

    public Vector3 CurUnit { get; set; }
}
