using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IMessage;
using GameUnit;

public class Command
{
    virtual public void Excute() { }

    protected void SetAttacker(GameUnit.GameUnit unit)
    {
        Gameplay.Info.Attacker = unit;
    }
    protected GameUnit.GameUnit GetAttacker()
    {
        return Gameplay.Info.Attacker;
    }

    protected void SetAttackedUnit(GameUnit.GameUnit unit)
    {
        Gameplay.Info.AttackedUnit = unit;
    }
    protected GameUnit.GameUnit GetAttackedUnit()
    {
        return Gameplay.Info.AttackedUnit;
    }

    protected void SetInjurer(GameUnit.GameUnit unit)
    {
        Gameplay.Info.Injurer = unit;
    }
    protected void SetInjuredUnit(GameUnit.GameUnit unit)
    {
        Gameplay.Info.InjuredUnit = unit;
    }

    protected void SetKiller(GameUnit.GameUnit killer)
    {
        Gameplay.Info.Killer = killer;
    }
    protected void SetKilledAndDeadUnit(GameUnit.GameUnit killedUnit)
    {
        Gameplay.Info.KilledUnit = killedUnit;
        Gameplay.Info.Dead = killedUnit;
    }

    protected GameUnit.GameUnit GetSelectingUnit()
    {
        return Gameplay.Info.SelectingUnit;
    }
}

public class SelectUnitCommand : Command
{
    public override void Excute()
    {
        Gameplay.Info.SelectingUnit = _unit;
    }

    private GameUnit.GameUnit _unit;
}

public class UnitMoveCommand :Command
{
    UnitMoveCommand(GameUnit.GameUnit unit)
    {
        _unit = unit;
    }

    public override void Excute()
    {
    }

    private GameUnit.GameUnit _unit;
}

public class UnitAttackCommand :Command
{
    UnitAttackCommand(GameUnit.GameUnit Attacker, GameUnit.GameUnit AttackedUnit)
    {
        _Attacker = Attacker; SetAttacker(Attacker);
        _AttackedUnit = AttackedUnit; SetAttackedUnit(AttackedUnit);
    }

    public override void Excute()
    {
        MsgDispatcher.SendMsg((int)TriggerType.AnnounceAttack);

        DamageRequest.CaculateDamageRequestList(DamageRequestList, _Attacker, _AttackedUnit);

        for (int i = 0; i < DamageRequestList.Count; i++)
        {

        }
    }

    private List<DamageRequest> DamageRequestList;
    private GameUnit.GameUnit _Attacker; //宣言攻击者
    private GameUnit.GameUnit _AttackedUnit; //被攻击者
}