using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IMessage;


public class Damage
{

}

//继承Command的基类是为了能够使用Command里的方法
public class DamageRequest : Command
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
    public override void Excute()
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