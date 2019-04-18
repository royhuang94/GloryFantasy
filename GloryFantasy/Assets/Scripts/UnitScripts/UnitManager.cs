using System.Collections;
using System.Collections.Generic;
using BattleMap;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//TODO 实现简单的点击事件，处理单位实例化

public class UnitManager: 
    GFGame.MonoBehaviourSingleton<UnitManager>
{
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

    /// <summary>
    /// 标记是否在释放行动牌
    /// </summary>
    public bool IsCasting
    {
        get;
        private set;
    }
    /// <summary>
    /// 正在释放的行动牌的异能的引用
    /// </summary>
    private Ability.Ability CastingCard;
    /// <summary>
    /// 用来存储释放异能的选择目标
    /// </summary>
    private List<Vector2> SelectingList = new List<Vector2>();

    private Transform goTransform;
    public void CouldInstantiation(bool coudInstantiation, Transform parent)
    {
        IsInstantiation = coudInstantiation;
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
        GameObject temp = GameObject.Instantiate(pickedUnit.gameObject, goTransform) as GameObject;
        //temp.transform.parent = Panel;
        //Debug.Log(temp.name);
        temp.transform.localPosition = new Vector3(6.8f, 7f, 0.0f);
        temp.GetComponent<Image>().raycastTarget = true;
        
        //添加
        //脚本到实例化Unit上
        //AddComponent
        temp.gameObject.AddComponent<NBearUnit.UnitMove>();

        //AttachHpOnUnit(temp);

        var hpTest = temp.transform.GetChild(0);
        pickedUnit.Hide();
        //TODO 暂时用Text标识血量，以后改为slider
        hpTest.gameObject.SetActive(true);
        float hp = temp.GetComponent<GameUnit.GameUnit>().unitAttribute.HP;
        float hpDivMaxHp = hp / temp.GetComponent<GameUnit.GameUnit>().unitAttribute.MaxHp * 100;

        hpTest.GetComponent<Text>().text = string.Format("HP: {0}%", hpDivMaxHp);

    }
    #endregion

    public bool IsClicked { get; set; }
    public List<Vector2> TargetList { get; set; }
    private Canvas canvas;

    #region 单位行为
    public bool isMoving = false;
    public bool canMoving = false;
    public bool canAttack = false;
    #endregion

    private void Start()
    {
        TargetList = new List<Vector2>();
        mapBlockParent = GetComponentInParent<BattleMap.BattleMapBlock>();
        canvas = GameObject.Find("UnitUI").GetComponent<Canvas>();
        pickedUnit = GameObject.Find("PickedUnit").GetComponent<NBearUnit.UnitUI>();
        pickedUnit.Hide();
        IsInstantiation = false;
        IsClicked = false;
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
        if (isMoving)
        {
            GFGame.UtilityHelper.Log("高亮地图显示", GFGame.LogColor.RED);
            UnitMoving();
        }

    }

    private BattleMap.BattleMapBlock mapBlockParent;
    /// <summary>
    /// 管理单位移动
    /// </summary>
    public void UnitMoving()
    {
        if (BattleMap.MapNavigator._Instantce.PathSearch(UnitManager.Instance.CurUnit, BattleMap.BattleMap.getInstance().curMapPos))
        {

            BattleMap.BattleMapBlock startBlock = BattleMap.BattleMap.getInstance().GetSpecificMapBlock((int)UnitManager.Instance.CurUnit.x, (int)UnitManager.Instance.CurUnit.y);
            BattleMap.BattleMapBlock destBlock = BattleMap.BattleMap.getInstance().GetSpecificMapBlock((int)BattleMap.BattleMap.getInstance().curMapPos.x, (int)BattleMap.BattleMap.getInstance().curMapPos.y);

            startBlock.transform.GetChild(0).SetParent(destBlock.transform);
            destBlock.transform.GetChild(0).localPosition = new Vector3(6.0f, 4.0f, 0.0f);

            canMoving = false;
        }

        isMoving = false;
        //TODO 待优化
        BattleMap.MapNavigator._Instantce.RestIsInCloseListBlock();
    }

    /// <summary>
    /// 释放行动牌
    /// </summary>
    /// <param name="ability">被释放的行动牌上的异能</param>
    public void CastCard(Ability.Ability ability)
    {
        IsCasting = true;
        this.CastingCard = ability;
        //初始化对象列表
        this.SelectingList = new List<Vector2>();
    }
    /// <summary>
    /// 释放指令牌时输入选择目标
    /// </summary>
    /// <param name="target"></param>
    public void InputTarget(Vector2 target)
    {
        int count = SelectingList.Count;
        Ability.AbilityTarget abilityTarget = CastingCard.AbilityTargetList[count];
        //TODU: 修改敌军友军的区分方式
        if ((abilityTarget.TargetType == Ability.TargetType.Enemy && BattleMap.BattleMap.getInstance().GetUnitsOnMapBlock(target).owner == "Enemy") ||
            (abilityTarget.TargetType == Ability.TargetType.Friendly && BattleMap.BattleMap.getInstance().GetUnitsOnMapBlock(target).owner == "Frendly") ||
             abilityTarget.TargetType == Ability.TargetType.Field && !BattleMap.BattleMap.getInstance().CheckIfHasUnits(target))
        {
            if (abilityTarget.IsSpeller)
            {
                Gameplay.Info.AbilitySpeller = BattleMap.BattleMap.getInstance().GetUnitsOnMapBlock(target);
                CastingCard.Speller = BattleMap.BattleMap.getInstance().GetUnitsOnMapBlock(target);
            }
            if (abilityTarget.IsTarget)
            {
                CastingCard.TargetList.Add(target);
                SelectingList.Add(target);
            }

            //如果这是最后一个目标
            if (count + 1 == CastingCard.AbilityTargetList.Count)
            {
                Gameplay.Info.SpellingAbility = CastingCard;
                IMessage.MsgDispatcher.SendMsg((int)IMessage.MessageType.ActiveAbility);
            }
        }
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
    public Vector3 EnemyCurUnit { get; set; }
}
