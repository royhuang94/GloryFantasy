using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IMessage;
using GameUnit;

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
    public UnitMoveCommand(GameUnit.GameUnit unit, Vector2 unitPositon,Vector2 targetPosion)
    {
        _unit = unit;
        _unitPosition = unitPositon;
        _targetPosition = targetPosion;
    }

    public bool Judge()
    {
        Vector2 unit1 = _unitPosition;
        Vector2 unit2 = _targetPosition;
        int MAN_HA_DUN = Mathf.Abs((int)unit1.x - (int)unit2.x) + Mathf.Abs((int)unit1.y - (int)unit2.y);
        if (MAN_HA_DUN <= _unit.unitAttribute.Mov)
            return true;

        return false;
    }

    public override void Excute()
    {
        Debug.Log("Moving Command excusing");
        BattleMap.BattleMap.getInstance().MoveUnitToCoordinate(_unit, _destination);
    }

    private GameUnit.GameUnit _unit;
    private Vector2 _destination;
    private Vector2 _unitPosition;
    private Vector2 _targetPosition;
}

public class UnitAttackCommand :Command
{
    public UnitAttackCommand(GameUnit.GameUnit Attacker, GameUnit.GameUnit AttackedUnit)
    {
        _Attacker = Attacker; this.SetAttacker(Attacker);
        _AttackedUnit = AttackedUnit; this.SetAttackedUnit(AttackedUnit);
    }


    //TODO 如何使用
    //计算攻击距离是否大于曼哈顿
    public bool Judge()
    {
        //Vector2 unit1 = BattleMap.BattleMap.getInstance().GetUnitCoordinate(_Attacker);
        //Vector2 unit2 = BattleMap.BattleMap.getInstance().GetUnitCoordinate(_AttackedUnit);
        Vector2 unit1;
        if (BattleMap.BattleMap.getInstance().CheckIfHasUnits(UnitManager.Instance.CurUnit))
        {
            unit1 = UnitManager.Instance.CurUnit;
        }
        else
        {
            unit1 = BattleMap.BattleMap.getInstance().curMapPos;
        }
        Vector2 unit2 = UnitManager.Instance.EnemyCurUnit;
        int MAN_HA_DUN = Mathf.Abs((int)unit1.x - (int)unit2.x) + Mathf.Abs((int)unit1.y - (int)unit2.y);
        if (MAN_HA_DUN <= _Attacker.unitAttribute.rng)
            return true;

        return false;
    }

    public override void Excute()
    {
        MsgDispatcher.SendMsg((int)MessageType.AnnounceAttack);
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
            else if (!_AttackedUnit.IsDead() && !_Attacker.IsDead())
            {
                DamageRequestList[i].Excute();
            }
        }
    }

    //TODO 攻击制作
    //1. 通过变量_Attacker _AttackedUnit 保存宣言攻击者和被攻击者
    //2. 通过DamageRequestList  —> Damange类中
    //3. 通过Damage类与Command类来执行攻击环节，注意细节修改

    private List<DamageRequest> DamageRequestList;
    private GameUnit.GameUnit _Attacker; //宣言攻击者
    private GameUnit.GameUnit _AttackedUnit; //被攻击者

}