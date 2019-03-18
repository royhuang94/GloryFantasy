using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IMessage;
using GameUnit;

public class Command
{
    virtual public void excute() { }

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

    GameUnit.GameUnit GetSelectingUnit()
    {
        return Gameplay.Info.SelectingUnit;
    }
}

public class SelectUnitCommand : Command
{
    public override void excute()
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

    public override void excute()
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

    public override void excute()
    {

        MsgDispatcher.SendMsg((int)TriggerType.AnnounceAttack);
    }

    //private DamageRequestList
    private GameUnit.GameUnit _Attacker; //宣言攻击者
    private GameUnit.GameUnit _AttackedUnit; //被攻击者
}