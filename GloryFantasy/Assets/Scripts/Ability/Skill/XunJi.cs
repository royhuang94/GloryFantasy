using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using IMessage;

public class XunJi : Ability
{
    Trigger trigger;

    private void Start()
    {
        //创建Trigger实例，传入技能的发动者
        trigger = new TXunJi(GetComponent<GameUnit.GameUnit>().GetMsgReceiver());
        //注册Trigger进消息中心
        MsgDispatcher.RegisterMsg(trigger, "XunJi");
    }

}

public class TXunJi : Trigger
{
    public TXunJi(MsgReceiver _speller)
    {
        register = _speller;
        msgName = (int)TriggerType.Summon;
        condition = Condition;
        action = Action;
    }

    private bool Condition()
    {
        //获取召唤列表
        List<GameUnit.GameUnit> SummonUnits = GetSummonUnit();
        //循环查询有没有召唤的怪是这个技能的发动者
        for (int i = 0; i < SummonUnits.Count; i++)
        {
            if (SummonUnits[i].GetMsgReceiver() == register)
                return true;
        }
        return false;
    }

    private void Action()
    {
        //获取发动这个技能的怪
        List<GameUnit.GameUnit> SummonUnits = GetSummonUnit();
        GameUnit.GameUnit unit = null;
        for (int i = 0; i < SummonUnits.Count; i++)
        {
            if (SummonUnits[i].GetMsgReceiver() == register)
                unit = SummonUnits[i];

            //让这只怪部署后可以攻击
            unit.disarm = false;
        }
    }
}
