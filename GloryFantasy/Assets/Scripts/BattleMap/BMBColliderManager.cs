using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BattleMap;
using IMessage;
using Unit = GameUnit.GameUnit;

public class BMBColliderManager : MonoBehaviour, MsgReceiver
{
    //保存了所有的地图块儿
    private BattleMapBlock[,] mapBlocks;
    //当前操控的单位
    private Unit curUnit;

    /// <summary>
    /// 当前处于移动的单位
    /// 需要随时修改他的单位值，并且修改值后要SendMessage
    /// </summary>
    /// <param name="unit"></param>
    public void SetCurUnit(Unit unit)
    {
        curUnit = unit;
    }


    public void InitBMB(BattleMapBlock[,] _mapBlocks)
    {
        mapBlocks = _mapBlocks;
    }

    public void Start()
    {
        MsgDispatcher.RegisterMsg(
           this.GetMsgReceiver(),
           (int)MessageType.UnitEnter,
           isUnitEnter,
           UnitEnter,
           "Unit Enter Trigger"
           );
        MsgDispatcher.RegisterMsg(
           this.GetMsgReceiver(),
           (int)MessageType.UnitExit,
           isUnitExit,
           UnitExit,
           "Unit Exit Trigger"
           );
        MsgDispatcher.RegisterMsg(
           this.GetMsgReceiver(),
           (int)MessageType.UnitDispose,
           isUnitDispose,
           UnitDispose,
           "Unit Dispose Trigger"
           );
    }

    /// <summary>
    /// 单位是否进入对应的地图块儿
    /// </summary>
    /// <returns>检测到进入返回true，反之false</returns>
    public bool isUnitEnter()
    {
        if (curUnit != null && curUnit.mapBlockBelow.position == curUnit.nextPos)
            return true;

        return false;
    }
    /// <summary>
    /// 单位是否退出对应的地图块儿
    /// </summary>
    /// <returns>检测到退出返回true，反之false</returns>
    public bool isUnitExit()
    {
        if (curUnit != null && curUnit.mapBlockBelow.position != curUnit.nextPos)
            return true;

        return false;
    }
    /// <summary>
    /// 单位是否驻足对应的地图块儿
    /// </summary>
    /// <returns>检测到停留返回true，反之false</returns>
    public bool isUnitDispose()
    {
        if(curUnit != null && curUnit.CurPos == curUnit .nextPos)
        {
            int x = (int)curUnit.nextPos.x;
            int y = (int)curUnit.nextPos.y;
            mapBlocks[x, y].bmbCollider.OnUnitDispose(curUnit);
        }

        return false;
    }

    /// <summary>
    /// 产生进入信息，并更新对应变量
    /// </summary>
    public void UnitEnter()
    {
        int x = (int)curUnit.nextPos.x;
        int y = (int)curUnit.nextPos.y;

        //调用对应地图块儿的函数
        mapBlocks[x, y].bmbCollider.OnUnitEnter(curUnit);
        MsgDispatcher.SendMsg((int)MessageType.UnitExit);
    }
    /// <summary>
    /// 产生退出信息，并更新对应变量
    /// </summary>
    public void UnitExit()
    {
        int x = (int)curUnit.CurPos.x;
        int y = (int)curUnit.CurPos.y;

        //调用对应地图块儿的函数
        mapBlocks[x, y].bmbCollider.OnUnitExit(curUnit);
        MsgDispatcher.SendMsg((int)MessageType.UnitEnter);
    }
    /// <summary>
    /// 产生驻足信息，并更新对应变量
    /// </summary>
    public void UnitDispose()
    {
        int x = (int)curUnit.nextPos.x;
        int y = (int)curUnit.nextPos.y;

        //调用对应地图块儿的函数
        mapBlocks[x, y].bmbCollider.OnUnitDispose(curUnit);
    }


    /// <summary>
    /// 仿照主程写的写的接口
    /// </summary>
    T IMessage.MsgReceiver.GetUnit<T>()
    {
        return this as T;
    }
}
