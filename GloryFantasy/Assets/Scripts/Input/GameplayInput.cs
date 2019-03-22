using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MapManager;

public class GameplayInput
{
    List<Vector2> TargetList;

    public void HandleInput(Vector2 target)
    {
        MapManager.MapManager mapManager = MapManager.MapManager.getInstance();
       if (TargetList.Count == 0)
        {
            if (mapManager.CheckIfHasUnits(target))
            {
                TargetList.Add(target);
            }
        }
       else
       if (TargetList.Count == 1)
        {
            if (mapManager.CheckIfHasUnits(target))
            {
                TargetList.Add(target);
                GameUnit.GameUnit unit1 = mapManager.GetUnitsOnMapBlock(TargetList[0])[0];
                GameUnit.GameUnit unit2 = mapManager.GetUnitsOnMapBlock(TargetList[1])[0];
                UnitAttackCommand attackCommand = new UnitAttackCommand(unit1, unit2);
                attackCommand.Excute();
            }
            else
            {
                TargetList.Add(target);
                GameUnit.GameUnit unit1 = mapManager.GetUnitsOnMapBlock(TargetList[0])[0];
                Vector2 unit2 = target;
                UnitMoveCommand moveCommand = new UnitMoveCommand(unit1, unit2);
                moveCommand.Excute();
            }
        }
    }
}
