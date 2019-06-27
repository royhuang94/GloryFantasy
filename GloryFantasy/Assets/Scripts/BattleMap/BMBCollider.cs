using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BattleMap;
using Unit = GameUnit.GameUnit;
using IMessage;

public class BMBCollider
{
    //地图块儿的检测范围
    public List<Vector2> colliderRange = new List<Vector2>();
    //进入单位
    public List<Unit> enterUnits = new List<Unit>();
    //离开单位
    public List<Unit> exitUnits = new List<Unit>();
    //驻足单位
    public List<Unit> disposeUnits = new List<Unit>();

    public List<Vector2> _bound = new List<Vector2> { Vector2.zero };
    private bool _isBMA = false; 
    private GameUnit.GameUnit _gameUnit;
    private BattleMapBlock _battleMapBlock;
    private bool _isStatic = false;

    public BMBCollider(GameUnit.GameUnit unit, List<Vector2> bound)
    {
        _gameUnit = unit;
        _bound = bound;
        GamePlay.Gameplay.Instance().bmbColliderManager.InitBMB(this);
        UpdateColliderRange();
        foreach (Vector2 pos in colliderRange)
        {
            GameUnit.GameUnit unitInRange = BattleMap.BattleMap.Instance().GetUnitsOnMapBlock(pos);
            if (unitInRange == null)
                continue;
            disposeUnits.Add(unitInRange);
        }
    }

    public BMBCollider(BattleMapBlock mapBlock, List<Vector2> bound)
    {
        _battleMapBlock = mapBlock;
        _bound = bound;
        GamePlay.Gameplay.Instance().bmbColliderManager.InitBMB(this);
        UpdateColliderRange();
        foreach (Vector2 pos in colliderRange)
        {
            GameUnit.GameUnit unitInRange = BattleMap.BattleMap.Instance().GetUnitsOnMapBlock(pos);
            if (unitInRange == null)
                continue;
            disposeUnits.Add(unitInRange);
        }
    }
    public BMBCollider(GameUnit.GameUnit unit, bool isBMA)
    {
        _gameUnit = unit;
        _isBMA = isBMA;
        GamePlay.Gameplay.Instance().bmbColliderManager.InitBMB(this);
        UpdateColliderRange();
        foreach (Vector2 pos in colliderRange)
        {
            GameUnit.GameUnit unitInRange = BattleMap.BattleMap.Instance().GetUnitsOnMapBlock(pos);
            if (unitInRange == null)
                continue;
            disposeUnits.Add(unitInRange);
        }
    }
    public BMBCollider(List<Vector2> battleArea)
    {
        _isStatic = true;
        _isBMA = true;
        colliderRange = battleArea;
        GamePlay.Gameplay.Instance().bmbColliderManager.InitBMB(this);
        UpdateColliderRange();
        foreach (Vector2 pos in colliderRange)
        {
            GameUnit.GameUnit unitInRange = BattleMap.BattleMap.Instance().GetUnitsOnMapBlock(pos);
            if (unitInRange == null)
                continue;
            disposeUnits.Add(unitInRange);
        }
    }
    /// <summary>
    /// 更新collider的检测范围
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void UpdateColliderRange()
    {
        if (_isStatic)
            return ;
        
        Vector2 pos = Vector2.zero;
        if (_gameUnit != null)
        {
            pos = _gameUnit.CurPos;
            
        }
        else if (_battleMapBlock != null)
        {
            pos = _battleMapBlock.GetCoordinate();
            
        }
        colliderRange.Clear();
        if (_isBMA)
        {
            int _regionID = BattleMap.BattleMap.Instance().GetSpecificMapBlock(pos).area;
            List<Vector2> BA = BattleMap.BattleMap.Instance().battleAreaData.GetBattleAreaAllPosByID(_regionID);
            foreach (Vector2 v in BA)
            {
                colliderRange.Add(v);
            }
        }
        else
        {
            foreach (Vector2 v in _bound)
            {
                colliderRange.Add(new Vector2(pos.x + v.x, pos.y + v.y));
            }
        }
        #region 弃用
        //Vector2 pos = Vector2.zero;
        //if (_gameUnit != null)
        //{
        //    pos = _gameUnit.CurPos;
        //    //检查单位的状态是否正常，例如是否已经死亡离开地图
        //    //if (BattleMap.BattleMap.Instance().GetUnitsOnMapBlock(pos) != _gameUnit)
        //    //    return;
        //}
        //else if (_battleMapBlock != null)
        //{
        //    pos = _battleMapBlock.GetCoordinate();
        //}
        //int _leftTopX = (int)pos.x - (int)bound.x / 2;
        //int _leftTopY = (int)pos.y - (int)bound.y / 2;
        //int _rightButtomX = (int)pos.x + ((int)bound.x - (int)bound.x / 2 - 1);
        //int _rightButtomY = (int)pos.y + ((int)bound.y - (int)bound.y / 2 - 1);
        //colliderRange.Clear();
        //for (int i = _leftTopX; i <= _rightButtomX; i++)
        //{
        //    for (int j = _leftTopY; j <= _rightButtomY; j++)
        //    {
        //        colliderRange.Add(new Vector2(i, j));
        //    }
        //}
        #endregion
    }

    public void fresh(List<Unit> oldDisposeUnits)
    {
        //先清空三个列表
        enterUnits.Clear();
        exitUnits.Clear();
        disposeUnits.Clear();

        foreach (Vector2 pos in colliderRange)
        {
            GameUnit.GameUnit unit = BattleMap.BattleMap.Instance().GetUnitsOnMapBlock(pos);
            if (unit == null)
                continue;

            //如果老列表没有，代表是新进入的
            if (!oldDisposeUnits.Contains(unit))
            {
                disposeUnits.Add(unit);
                enterUnits.Add(unit);
            }
            //如果老列表有，代表驻留
            else if (oldDisposeUnits.Contains(unit))
            {
                disposeUnits.Add(unit);
            }
        }
        //老列表有的，新列表没有，那就是离开了
        foreach (Unit unit in oldDisposeUnits)
        {
            if (!disposeUnits.Contains(unit))
            {
                exitUnits.Add(unit);
            }
        }
        MsgDispatcher.SendMsg((int)MessageType.ColliderChanged);
    }

    #region 主程看不懂的老代码
    ////-1 离开 / 0 进入 / 1 驻足
    //public int state = -1; //-2(默认值)

    ///// <summary>
    ///// 单位进入，并更新enterUnits，state
    ///// </summary>
    ///// <param name="unit">当前操作单位</param>
    //public void OnUnitEnter(List<Unit> units)
    //{
    //    enterUnits = units; //进入记录
    //    state = 0;
    //    Debug.Log("坐标：" + colliderRange[0] + " 地图块儿检测到单位进入");
    //    if (units[0].nextPos == units[0].CurPos)
    //        OnUnitDispose();
    //}

    ///// <summary>
    ///// 单位退出，并更新enterUnits/disposeUnits，state
    ///// </summary>
    ///// <param name="unit">当前操作单位</param>
    //public void OnUnitExit()
    //{
    //    exitUnits = enterUnits; //退出记录
    //    enterUnits = new List<Unit>(); //覆盖

    //    if (state == 1)
    //        disposeUnits = new List<Unit>();//覆盖
    //    state = -1;
    //    Debug.Log("坐标：" + colliderRange[0] + " 地图块儿检测到单位离开");
    //    MsgDispatcher.SendMsg((int)MessageType.BattleSate);

    //}

    ///// <summary>
    ///// 单位驻足，并更新disposeUnits，state
    ///// </summary>
    ///// <param name="unit">当前操作单位</param>
    //public void OnUnitDispose()
    //{
    //    state = 1;
    //    disposeUnits = enterUnits; //驻足记录
    //    Debug.Log("坐标：" + colliderRange[0] + " 地图块儿检测到单位驻足");
    //    MsgDispatcher.SendMsg((int)MessageType.BattleSate);
    //    if (GamePlay.Gameplay.Instance().gamePlayInput.InputFSM.CurrentState is GamePlay.FSM.InputFSMSummonState)
    //        return;
    //    if(enterUnits[0].owner != GameUnit.OwnerEnum.Enemy)
    //        GamePlay.Gameplay.Instance().gamePlayInput.InputFSM.HandleAtkConfirm(colliderRange[0],BattleMap.BattleMap.Instance().GetUnitsOnMapBlock(colliderRange[0])); 
    //}
    #endregion
}
