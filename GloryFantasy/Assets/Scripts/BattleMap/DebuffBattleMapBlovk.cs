using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unit = GameUnit.GameUnit;

/// <summary>
/// 处理异常地图块，例如灼烧
/// </summary>
namespace BattleMap
{
    public class DebuffBattleMapBlovk : UnitySingleton<DebuffBattleMapBlovk>
    {
        //灼烧
        public void BattleMapBlockBurning(Vector2 position)
        {
            Unit unit = GetUnitOnMapBolock(position);
            unit.hp -= 1;
        }

        //滞留
        public void BattleMapBlockRetrad(Vector2 position)
        {
            Unit unit = GetUnitOnMapBolock(position);
            unit.transform.position = position;
        }

        
        private Unit GetUnitOnMapBolock(Vector2 position)
        {
            Unit unit = null;
            if (BattleMap.Instance().CheckIfHasUnits(position))
            {
               return unit = BattleMap.Instance().GetUnitsOnMapBlock(position);
            }
            else
            {
                return null;
            }
        }
    }
}

