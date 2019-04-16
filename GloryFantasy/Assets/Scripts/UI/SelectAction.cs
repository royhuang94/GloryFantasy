using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectAction : MonoBehaviour {

    /// <summary>
    /// 在这处理战斗行为；
    /// </summary>

    // Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Show()
    {
        BattleMap.BattleMap.getInstance().selectAction.SetActive(true);
    }

    public void Hide()
    {
        BattleMap.BattleMap.getInstance().selectAction.SetActive(false);
    }
    //攻击接口
    public void Attack()
    {
        Vector2 tempPosition = BattleMap.BattleMap.getInstance().curMapPos;
        Gameplay.GetInstance().gamePlayInput.HandleAtkConfirm(tempPosition);
        Hide();
    }

    //防御接口
    public void Defense()
    {
        Hide();
    }
}
