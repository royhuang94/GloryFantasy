using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BattleMap;
using Unit = GameUnit.GameUnit;

public class BMBCollider
{
    //地图块儿的检测范围
    List<Vector2> colliderRange = new List<Vector2>();
    //进入单位
    List<Unit> enterUnits = new List<Unit>();
    //离开单位
    List<Unit> exitUnits = new List<Unit>();
    //驻足单位
    List<Unit> disposeUnits = new List<Unit>();


    //-1 离开 / 0 进入 / 1 驻足
    public int state = -1; //-2(默认值)

    /// <summary>
    /// 初始化检测范围
    /// </summary>
    /// <param name="battleMapBlock"></param>
    public void init(BattleMapBlock battleMapBlock)
    {
        for(int i = 0; i < 2; i++)
        {
            float x = battleMapBlock.position.x + i;
            float y = battleMapBlock.position.y + i;
            colliderRange.Add(new Vector2(x, y));
        }
    }

    /// <summary>
    /// 单位进入，并更新enterUnits，state
    /// </summary>
    /// <param name="unit">当前操作单位</param>
    public void OnUnitEnter(Unit unit)
    {
        state = 0;
        enterUnits.Add(unit);

        unit.CurPos = unit.nextPos;
        Debug.Log("坐标：" + colliderRange[0] + " 地图块儿检测到单位进入");
    }
    /// <summary>
    /// 单位退出，并更新enterUnits/disposeUnits，state
    /// </summary>
    /// <param name="unit">当前操作单位</param>
    public void OnUnitExit(Unit unit)
    {
        if (state == 0)
            enterUnits.Remove(unit);
        else
            disposeUnits.Remove(unit);

        state = -1;
        exitUnits.Add(unit);

        unit.CurPos = unit.nextPos;
        Debug.Log("坐标：" + colliderRange[0] + " 地图块儿检测到单位离开");
    }
    /// <summary>
    /// 单位驻足，并更新disposeUnits，state
    /// </summary>
    /// <param name="unit">当前操作单位</param>
    public void OnUnitDispose(Unit unit)
    {
        state = 1;
        disposeUnits.Add(unit);
        Debug.Log("坐标：" + colliderRange[0] + " 地图块儿检测到单位驻足");
    }
}
