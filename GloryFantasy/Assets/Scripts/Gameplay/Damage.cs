using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IMessage;


public class Damage
{
    public int damageCount { get; set; }
}

//继承Command的基类是为了能够使用Command里的方法
//其实如果把command的方法写成 功能类会更靠谱，因为实际上和Command没有逻辑上的继承关系啊我日
//所以我把Command的方法提了出来写成了GameplayTool，Command继承GamePlay,依然是采用继承的方式才能调用，因为真的不想增加太多的全局量
public class DamageRequest : GameplayTool
{
    /// <summary>
    /// 根据攻击者和被攻击的攻击优先级列表生成对应的伤害请求list
    /// </summary>
    /// <param name="DamageRequestList">被填充的伤害请求list</param>
    /// <param name="Attacker">攻击者</param>
    /// <param name="AttackedUnit">被攻击者</param>
    public static void CaculateDamageRequestList(List<DamageRequest> DamageRequestList, GameUnit.GameUnit Attacker, GameUnit.GameUnit AttackedUnit)
    {

    }

    /// <summary>
    /// 单次伤害请求计算
    /// </summary>
    public void Excute()
    {
        //_attackedUnit.TakeDamage(_damage);
        SetInjurer(_attacker); SetInjuredUnit(_attackedUnit);
        MsgDispatcher.SendMsg((int)TriggerType.Damage);
        MsgDispatcher.SendMsg((int)TriggerType.BeDamaged);

        //if _attackedUnit.IsDead()
        {
            SetKiller(_attacker); SetKilledAndDeadUnit(_attackedUnit);
            MsgDispatcher.SendMsg((int)TriggerType.Kill);
            MsgDispatcher.SendMsg((int)TriggerType.Dead);
        }
    }

    /// <summary>
    /// 如果伤害请求优先级相同，则伤害判定流程会特殊一些
    /// </summary>
    public void ExcuteSameTime()
    {
        //CheckWhosTurn(_attacker, _attackedUnit);

        //_attacker.TakeDamage(_damage);
        SetInjurer(_attackedUnit); SetInjuredUnit(_attacker);
        MsgDispatcher.SendMsg((int)TriggerType.Damage);
        MsgDispatcher.SendMsg((int)TriggerType.BeDamaged);

        //_attackedUnit.TakeDamage(_damage);
        SetInjurer(_attacker); SetInjuredUnit(_attackedUnit);
        MsgDispatcher.SendMsg((int)TriggerType.Damage);
        MsgDispatcher.SendMsg((int)TriggerType.BeDamaged);

        //if _attacker.IsDead()
        {
            SetKiller(_attackedUnit); SetKilledAndDeadUnit(_attacker);
            MsgDispatcher.SendMsg((int)TriggerType.Kill);
            MsgDispatcher.SendMsg((int)TriggerType.Dead);
        }

        //if _attackedUnit.IsDead()
        {
            SetKiller(_attacker); SetKilledAndDeadUnit(_attackedUnit);
            MsgDispatcher.SendMsg((int)TriggerType.Kill);
            MsgDispatcher.SendMsg((int)TriggerType.Dead);
        }
    }

    private Damage damage;
    private GameUnit.GameUnit _attacker;
    private GameUnit.GameUnit _attackedUnit;
}