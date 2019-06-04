using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BattleMap;
using IMessage;
using Unit = GameUnit.GameUnit;
using GamePlay;

public class BMBColliderManager : MsgReceiver, GameplayTool
{
    private List<BMBCollider> bmbColliders;

    public void InitBMB(BMBCollider collider)
    {
        bmbColliders.Add(collider);
    }

    public BMBColliderManager()
    {
        bmbColliders = new List<BMBCollider>();

        //// 注册函数相应移动消息
        //MsgDispatcher.RegisterMsg(
        //    this.GetMsgReceiver(),
        //    (int)MessageType.Move,
        //    CanFreshCollider,
        //    FreshCollider,
        //    "Fresh Collider Trigger"
        //);
        // 注册函数相应移动消息
        MsgDispatcher.RegisterMsg(
            this.GetMsgReceiver(),
            (int)MessageType.ColliderChange,
            CanFreshCollider,
            FreshCollider,
            "Fresh Collider Trigger"
        );
        //// 注册函数相应移动消息
        //MsgDispatcher.RegisterMsg(
        //    this.GetMsgReceiver(),
        //    (int)MessageType.Moved,
        //    CanFreshCollider,
        //    FreshDispose,
        //    "Fresh Collider Trigger"
        //);
    }

    #region 监听Move与afterMove消息
    /// <summary>
    /// 判断是否可以触发更新函数
    /// </summary>
    /// <returns></returns>
    public bool CanFreshCollider()
    {
        return true;
    }

    /// <summary>
    /// 触发Fresh函数
    /// </summary>
    public void FreshCollider()
    {
        Fresh();
    }
    #endregion



    public void Fresh()
    {
        foreach (BMBCollider collider in bmbColliders)
        {
            //1. 单位发生移动，召唤，死亡后更新
            //2. 如何更新？
                //a. 遍历collider，获取当前collider的范围值
                //b. 上一次更新保存好的在自己范围内的单位的引用，再搜一遍自己的范围
                //c. 最后对比出哪个Exit，哪个Enter

            
            List<Unit> oldList = collider.disposeUnits;
            //更新collider的范围
            collider.UpdateColliderRange();
            //更新collider的三个列表
            collider.fresh(oldList);
        }
    }

    #region 主程看不懂
    ///// <summary>
    ///// 单位坐标发生变化后，管理更新所有Collider
    ///// </summary>
    //public void Fresh(Unit unit, List<Unit> units = null)
    //{
    //    //1. 单位发生移动，召唤，死亡后更新
    //    //2. 如何更新？
    //        //a. 遍历collider，获取当前collider的范围值
    //        //b. 上一次更新保存好的在自己范围内的单位的引用，再搜一遍自己的范围
    //        //c. 最后对比出哪个Exit，哪个Enter
    //    if(units == null)
    //    {
    //        units = new List<Unit>();
    //        units.Add(unit);
    //    }

    //    foreach(BMBCollider collider in bmbColliders)
    //    {
    //        //获取当前地图块儿 （collider.colliderRange[0] 地图块儿地址)
    //        BattleMapBlock battleMapBlock = BattleMap.BattleMap.Instance().GetSpecificMapBlock(collider.colliderRange[0]);
    //        //先确认上一次自己范围内是否保存得有单位的引用
    //        if (collider.enterUnits.Count != 0 || collider.disposeUnits.Count != 0)
    //        {
    //            if(battleMapBlock.units_on_me.Count == 0)
    //            {
    //                //单位坐标发生变化
    //                collider.OnUnitExit();
    //            }
    //        }
    //        else
    //        {
    //            if (battleMapBlock.units_on_me.Count != 0)
    //                collider.OnUnitEnter(units);
    //        }
    //    }
    //}
    #endregion

    /// <summary>
    /// 仿照主程写的写的接口
    /// </summary>
    T IMessage.MsgReceiver.GetUnit<T>()
    {
        return this as T;
    }

    #region 弃用
    //当前操控的单位
    //private Unit curUnit;

    /// <summary>
    /// 当前处于移动的单位
    /// 需要随时修改他的单位值，并且修改值后要SendMessage
    /// </summary>
    /// <param name="unit"></param>
    //public void SetCurUnit(Unit unit)
    //{
    //    curUnit = unit;
    //}


    //public BMBColliderManager()
    //{
    //MsgDispatcher.RegisterMsg(
    //   this.GetMsgReceiver(),
    //   (int)MessageType.UnitEnter,
    //   isUnitEnter,
    //   UnitEnter,
    //   "Unit Enter Trigger"
    //   );
    //MsgDispatcher.RegisterMsg(
    //   this.GetMsgReceiver(),
    //   (int)MessageType.UnitExit,
    //   isUnitExit,
    //   UnitExit,
    //   "Unit Exit Trigger"
    //   );
    //MsgDispatcher.RegisterMsg(
    //   this.GetMsgReceiver(),
    //   (int)MessageType.UnitDispose,
    //   isUnitDispose,
    //   UnitDispose,
    //   "Unit Dispose Trigger"
    //   );
    //}

    ///// <summary>
    ///// 单位是否进入对应的地图块儿
    ///// </summary>
    ///// <returns>检测到进入返回true，反之false</returns>
    //public bool isUnitEnter()
    //{
    //    if (curUnit != null && curUnit.mapBlockBelow.position == curUnit.nextPos)
    //        return true;

    //    return false;
    //}
    ///// <summary>
    ///// 单位是否退出对应的地图块儿
    ///// </summary>
    ///// <returns>检测到退出返回true，反之false</returns>
    //public bool isUnitExit()
    //{
    //    //if (curUnit != null && curUnit.mapBlockBelow.position != curUnit.nextPos)
    //    //    return true;

    //    //return false;

    //    return true;
    //}
    ///// <summary>
    ///// 单位是否驻足对应的地图块儿
    ///// </summary>
    ///// <returns>检测到停留返回true，反之false</returns>
    //public bool isUnitDispose()
    //{
    //    if(curUnit != null && curUnit.CurPos == curUnit .nextPos)
    //    {
    //        int x = (int)curUnit.nextPos.x;
    //        int y = (int)curUnit.nextPos.y;
    //        mapBlocks[x, y].bmbCollider.OnUnitDispose(curUnit);
    //    }

    //    return false;
    //}

    ///// <summary>
    ///// 产生进入信息，并更新对应变量
    ///// </summary>
    //public void UnitEnter()
    //{
    //    int x = (int)curUnit.nextPos.x;
    //    int y = (int)curUnit.nextPos.y;

    //    //调用对应地图块儿的函数
    //    mapBlocks[x, y].bmbCollider.OnUnitEnter(curUnit);
    //    MsgDispatcher.SendMsg((int)MessageType.UnitExit);
    //}
    ///// <summary>
    ///// 产生退出信息，并更新对应变量
    ///// </summary>
    //public void UnitExit()
    //{
    //    int x = (int)curUnit.CurPos.x;
    //    int y = (int)curUnit.CurPos.y;

    //    //调用对应地图块儿的函数
    //    mapBlocks[x, y].bmbCollider.OnUnitExit(curUnit);
    //    MsgDispatcher.SendMsg((int)MessageType.UnitEnter);
    //}
    ///// <summary>
    ///// 产生驻足信息，并更新对应变量
    ///// </summary>
    //public void UnitDispose()
    //{
    //    int x = (int)curUnit.nextPos.x;
    //    int y = (int)curUnit.nextPos.y;

    //    //调用对应地图块儿的函数
    //    mapBlocks[x, y].bmbCollider.OnUnitDispose(curUnit);
    //}

    #endregion
}
