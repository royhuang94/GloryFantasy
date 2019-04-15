using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using IMessage;

//护甲1
public class HuJia1 : Ability
{
    Trigger trigger;
    public int armor = 1;

    private void Start()
    {
        //获得这个技能的持有者，并修改其护甲恢复值
        GetComponent<GameUnit.GameUnit>().armorRestore = armor;
        //创建Trigger实例，传入技能的发动者和护甲恢复值
        trigger = new THuJia(GetComponent<GameUnit.GameUnit>().GetMsgReceiver());
        //注册Trigger进消息中心
        MsgDispatcher.RegisterMsg(trigger, "Hujia");
    }

}

public class THuJia : Trigger
{

    public THuJia(MsgReceiver _speller)
    {
        register = _speller;
        msgName = (int)TriggerType.BP;
        condition = Condition;
        action = Action;
    }

    private bool Condition()
    {
        return true;
    }

    private void Action()
    {
        //获取发动这个技能的怪
        GameUnit.GameUnit unit= register.GetGameUnit();
        //用护甲恢复值去修正护甲值
        if (unit.armor < unit.armorRestore)
            unit.armor = unit.armorRestore;
    }
}
