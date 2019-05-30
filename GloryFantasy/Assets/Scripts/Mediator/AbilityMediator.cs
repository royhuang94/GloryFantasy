using System.Collections.Generic;
using BattleMap;
using GameCard;
using GameUnit;
using UnityEngine;

namespace Mediator
{
    public class AbilityMediator : UnitySingleton<AbilityMediator>, AbilityMediatorInterface
    {
        public bool CheckUnitDeathById(string unitId)
        {
            return GameUnitPool.Instance().CheckDeathByID(unitId);
        }

        public void ReduceSpecificCardCd(string unitId, int amount)
        {
            CardManager.Instance().HandleCooldownEvent(unitId, amount);
        }

        public List<BattleMapBlock> GetMapBlocksWithinRange(Vector2 cordinate, int range)
        {
            // TODO: 完成实现
            return null;
        }
        
        public List<GameUnit.GameUnit> GetGameUnitsWithinRange(Vector2 cordinate, int range)
        {
            List<BattleMapBlock> blocks = GetMapBlocksWithinRange(cordinate, range);
            if (blocks == null)
                return null;
            List<GameUnit.GameUnit> units = new List<GameUnit.GameUnit>();

            foreach (BattleMapBlock mapBlock in blocks)
            {
                if(mapBlock.units_on_me.Count > 0)
                    units.AddRange(mapBlock.units_on_me);
            }
            return units;
        }

        public void CaseDamageToEnemy(string unitId, int damage)
        {
            // TODO: 完成实现，并将这个接口集成到应有的位置
        }
    }
}