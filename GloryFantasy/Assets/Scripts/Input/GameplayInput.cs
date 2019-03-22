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
            if (target ==  Vector2.zero)
            {
                TargetList.Add(target);
            }
        }
       else
       if (TargetList.Count == 1)
        {
            if (target == Vector2.zero)
            {
                GameUnit.GameUnit unit1 = null, unit2 = null;
                UnitAttackCommand attackCommand = new UnitAttackCommand(unit1, unit2);
                attackCommand.Excute();
            }
            else
            if (target == Vector2.one)
            {
                GameUnit.GameUnit unit1 = null;
                Vector2 unit2 = Vector2.zero;
                UnitMoveCommand moveCommand = new UnitMoveCommand(unit1, unit2);
                moveCommand.Excute();
            }
        }
    }
}
