using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayInput
{
    List<Vector2> TargetList = new List<Vector2>();

    public void HandleInput()
    {

    }

    public void HandleCancle()
    {
        if (TargetList.Count == 1)
        {
            GameUnit.GameUnit unit = BattleMapManager.BattleMapManager.GetInstance().GetUnitsOnMapBlock(TargetList[0])[0];
            unit.GetComponent<ShowRange>().CancleAttackRangeMark();
            unit.GetComponent<ShowRange>().CancleMoveRangeMark();
            TargetList.Clear();
        }
    }

    public void HandleConfirm(Vector2 target)
    {
        BattleMapManager.BattleMapManager mapManager = BattleMapManager.BattleMapManager.GetInstance();
       if (TargetList.Count == 0)
        {
            if (mapManager.CheckIfHasUnits(target))
            {
                TargetList.Add(target);
                GameUnit.GameUnit unit = BattleMapManager.BattleMapManager.GetInstance().GetUnitsOnMapBlock(TargetList[0])[0];
                unit.GetComponent<ShowRange>().MarkMoveRange();
                unit.GetComponent<ShowRange>().MarkAttackRange();
            }
        }
       else
       if (TargetList.Count == 1)
        {
            if (mapManager.CheckIfHasUnits(target))
            {
                //TargetList.Add(target);
                GameUnit.GameUnit unit1 = mapManager.GetUnitsOnMapBlock(TargetList[0])[0];
                //GameUnit.GameUnit unit2 = mapManager.GetUnitsOnMapBlock(TargetList[1])[0];
                GameUnit.GameUnit unit2 = mapManager.GetUnitsOnMapBlock(target)[0];
                UnitAttackCommand attackCommand = new UnitAttackCommand(unit1, unit2);
                if (attackCommand.Judge())
                {
                    //关闭染色
                    unit1.GetComponent<ShowRange>().CancleAttackRangeMark();
                    unit1.GetComponent<ShowRange>().CancleMoveRangeMark();

                    attackCommand.Excute();
                    TargetList.Clear();
                }
            }
            else
            {
                //TargetList.Add(target);
                GameUnit.GameUnit unit1 = mapManager.GetUnitsOnMapBlock(TargetList[0])[0];
                Vector2 unit2 = target;
                UnitMoveCommand moveCommand = new UnitMoveCommand(unit1, unit2);
                if (moveCommand.Judge())
                {
                    //关闭染色
                    unit1.GetComponent<ShowRange>().CancleAttackRangeMark();
                    unit1.GetComponent<ShowRange>().CancleMoveRangeMark();

                    moveCommand.Excute();
                    TargetList.Clear();
                }
            }
        }
    }
}
