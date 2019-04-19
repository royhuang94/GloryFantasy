using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectAction : MonoBehaviour{

    /// <summary>
    /// 在这处理战斗行为；
    /// </summary>

    //好吧，还是单例方便
    private static SelectAction instance;
    public static SelectAction Instance
    {
        get
        {
            return instance;
        }
    }
     
    // Use this for initialization
	void Start () {
        instance = this;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ShowSeclectActionUI ()
    {
        BattleMap.BattleMap.getInstance().selectAction.SetActive(true);
        BattleMap.BattleMap.getInstance().selectAction.transform.position = Input.mousePosition;
    }

    public void HideSeclectActionUI()
    {
        BattleMap.BattleMap.getInstance().selectAction.SetActive(false);
    }
    //攻击接口
    public void Attack()
    {
        Vector2 tempPosition = BattleMap.BattleMap.getInstance().curMapPos;
        Gameplay.GetInstance().gamePlayInput.HandleAtkConfirm(tempPosition);
        BattleMap.BattleMap.getInstance().selcetAction_Cancel.SetActive(true);
        BattleMap.BattleMap.getInstance().selcetAction_Cancel.transform.position = Input.mousePosition;
        HideSeclectActionUI();
        UnitManager.Instance.canAttack = true;
    }

    //取消当前行动返回选择面板
    public void Cancel()
    {
        BattleMap.BattleMap.getInstance().selcetAction_Cancel.SetActive(false);
        Gameplay.GetInstance().gamePlayInput.HandleAtkCancel(unitPositon());
        BattleMap.BattleMap.getInstance().selectAction.SetActive(true);
    }

    //防御接口
    public void Defense()
    {
        HideSeclectActionUI();
    }

    private Vector2 unitPositon()
    {
        if (BattleMap.BattleMap.getInstance().CheckIfHasUnits(UnitManager.Instance.CurUnit))
        {
            return UnitManager.Instance.CurUnit;
        }
        else
        {
            return BattleMap.BattleMap.getInstance().curMapPos;
        }
    }
}
