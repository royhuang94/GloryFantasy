using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using IMessage;

public class Damage
{

    public int damageValue { get; set; }

    public Damage(int damageValue)
    {
        this.damageValue = damageValue;
    }

    public static Damage GetDamage(GameUnit.GameUnit unit)
    {
        Damage damage = new Damage(unit.unitAttribute.atk);
        return damage;
    }

    public static void TakeDamage(GameUnit.GameUnit unit, Damage damage)
    {
        unit.hp -= damage.damageValue;
    }
}

//继承Command的基类是为了能够使用Command里的方法
//其实如果把command的方法写成 功能类会更靠谱，因为实际上和Command没有逻辑上的继承关系啊我日
//所以我把Command的方法提了出来写成了GameplayTool，Command继承GamePlay,依然是采用继承的方式才能调用，因为真的不想增加太多的全局量
public class DamageRequest : GameplayTool
{
    public DamageRequest(GameUnit.GameUnit attacker, GameUnit.GameUnit attackedUnit, int priority)
    {
        _attacker = attacker;
        _attackedUnit = attackedUnit;
        this.priority = priority;
    }

    /// <summary>
    /// 根据攻击者和被攻击的攻击优先级列表生成对应的伤害请求list
    /// </summary>
    /// <param name="DamageRequestList">被填充的伤害请求list</param>
    /// <param name="Attacker">攻击者</param>
    /// <param name="AttackedUnit">被攻击者</param>
    public static List<DamageRequest> CaculateDamageRequestList(GameUnit.GameUnit Attacker, GameUnit.GameUnit AttackedUnit)
    {
        List<DamageRequest> damageRequestList = new List<DamageRequest>();
        for (int i = 0; i < Attacker.priority.Count; i++)
        {
            damageRequestList.Add(new DamageRequest(Attacker, AttackedUnit, Attacker.priority[i]));
        }
        for (int i = 0; i < AttackedUnit.priority.Count; i++)
        {
            damageRequestList.Add(new DamageRequest(AttackedUnit, Attacker, AttackedUnit.priority[i]));
        }
        damageRequestList.Sort((a, b) => 
        {
            if (a.priority < b.priority)
                return 1;
            else if (a.priority == b.priority)
                return 0;
            else
                return -1;
        });
        return damageRequestList;
    }

    /// <summary>
    /// 单次伤害请求计算
    /// </summary>
    public void Excute()
    {
        Damage.TakeDamage(_attackedUnit, Damage.GetDamage(_attacker));
        this.SetInjurer(_attacker); this.SetInjuredUnit(_attackedUnit);
        MsgDispatcher.SendMsg((int)TriggerType.Damage);
        MsgDispatcher.SendMsg((int)TriggerType.BeDamaged);

        if (_attackedUnit.IsDead())
        {
            this.SetKiller(_attacker); this.SetKilledAndDeadUnit(_attackedUnit);
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

        Damage.TakeDamage(_attackedUnit, Damage.GetDamage(_attacker));
        this.SetInjurer(_attackedUnit); this.SetInjuredUnit(_attacker);
        MsgDispatcher.SendMsg((int)TriggerType.Damage);
        MsgDispatcher.SendMsg((int)TriggerType.BeDamaged);

        Damage.TakeDamage(_attacker, Damage.GetDamage(_attackedUnit));
        this.SetInjurer(_attacker); this.SetInjuredUnit(_attackedUnit);
        MsgDispatcher.SendMsg((int)TriggerType.Damage);
        MsgDispatcher.SendMsg((int)TriggerType.BeDamaged);

        if (_attacker.IsDead())
        {
            this.SetKiller(_attackedUnit); this.SetKilledAndDeadUnit(_attacker);
            MsgDispatcher.SendMsg((int)TriggerType.Kill);
            MsgDispatcher.SendMsg((int)TriggerType.Dead);
        }

        if (_attackedUnit.IsDead())
        {
            this.SetKiller(_attacker); this.SetKilledAndDeadUnit(_attackedUnit);
            MsgDispatcher.SendMsg((int)TriggerType.Kill);
            MsgDispatcher.SendMsg((int)TriggerType.Dead);
        }
    }

    //private Damage _damage;
    public GameUnit.GameUnit _attacker;
    public GameUnit.GameUnit _attackedUnit;
    public int priority;
}