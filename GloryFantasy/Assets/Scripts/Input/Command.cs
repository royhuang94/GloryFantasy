using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IMessage;
using GameUnit;

//指令操作的基类
public class Command : GameplayTool
{
    virtual public void Excute() { }
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
    public UnitMoveCommand(GameUnit.GameUnit unit, Vector2 destination)
    {
        _unit = unit;
        _destination = destination;
    }

    public bool Judge()
    {
        Vector2 unit1 = BattleMapManager.BattleMapManager.GetInstance().GetUnitCoordinate(_unit);
        Vector2 unit2 = _destination;
        int MAN_HA_DUN = Mathf.Abs((int)unit1.x - (int)unit2.x) + Mathf.Abs((int)unit1.y - (int)unit2.y);
        if (MAN_HA_DUN <= _unit.mov)
            return true;

        return false;
    }

    public override void Excute()
    {
        Debug.Log("Moving Command excusing");
        BattleMapManager.BattleMapManager.GetInstance().MoveUnitToCoordinate(_unit, _destination);
    }

    private GameUnit.GameUnit _unit;
    private Vector2 _destination;
}

public class UnitAttackCommand :Command
{
    public UnitAttackCommand(GameUnit.GameUnit Attacker, GameUnit.GameUnit AttackedUnit)
    {
        _Attacker = Attacker; SetAttacker(Attacker);
        _AttackedUnit = AttackedUnit; SetAttackedUnit(AttackedUnit);
    }

    //计算攻击距离是否大于曼哈顿
    public bool Judge()
    {
        Vector2 unit1 = BattleMapManager.BattleMapManager.GetInstance().GetUnitCoordinate(_Attacker);
        Vector2 unit2 = BattleMapManager.BattleMapManager.GetInstance().GetUnitCoordinate(_AttackedUnit);
        int MAN_HA_DUN = Mathf.Abs((int)unit1.x - (int)unit2.x) + Mathf.Abs((int)unit1.y - (int)unit2.y);
        if (MAN_HA_DUN <= _Attacker.rng)
            return true;

        return false;
    }

    public override void Excute()
    {
        MsgDispatcher.SendMsg((int)TriggerType.AnnounceAttack);
        //根据伤害优先级对伤害请求排序
        DamageRequestList = DamageRequest.CaculateDamageRequestList(_Attacker, _AttackedUnit);

        for (int i = 0; i < DamageRequestList.Count; i++)
        {
            //优先级相同并且两方互打的伤害请求作为同时处理
            if (i != DamageRequestList.Count - 1 && DamageRequestList[i].priority == DamageRequestList[i + 1].priority
                && DamageRequestList[i]._attacker == DamageRequestList[i + 1]._attackedUnit
                && DamageRequestList[i]._attackedUnit == DamageRequestList[i + 1]._attacker)
            {
                DamageRequestList[i].ExcuteSameTime();
                i++;
            }
            else
            {
                DamageRequestList[i].Excute();
            }
        }
    }

    private List<DamageRequest> DamageRequestList;
    private GameUnit.GameUnit _Attacker; //宣言攻击者
    private GameUnit.GameUnit _AttackedUnit; //被攻击者
}