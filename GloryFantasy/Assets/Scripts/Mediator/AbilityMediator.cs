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
            List<BattleMapBlock> battleMapBlocks = new List<BattleMapBlock>();
            BattleMapBlock battleMapBlock = null;
            List<Vector2> vector2s = GameGUI.ShowRange.Instance().GetSkillRnage(cordinate, range);
            for (int i = 0; i < vector2s.Count; i++)
            {
                battleMapBlock = BattleMap.BattleMap.Instance().GetSpecificMapBlock(vector2s[i]);
                battleMapBlocks.Add(battleMapBlock);
            }
            return battleMapBlocks;
        }

        public List<GameUnit.GameUnit> GetGameUnitsWithinRange(Vector2 cordinate, int range)
        {
            List<BattleMapBlock> blocks = GetMapBlocksWithinRange(cordinate, range);
            if (blocks == null)
                return null;
            List<GameUnit.GameUnit> units = new List<GameUnit.GameUnit>();

            foreach (BattleMapBlock mapBlock in blocks)
            {
                if (mapBlock.units_on_me.Count > 0)
                    units.AddRange(mapBlock.units_on_me);
            }
            return units;
        }

        public void CaseDamageToEnemy(string unitId, int damage)
        {
            foreach (GameUnit.GameUnit unit in BattleMap.BattleMap.Instance().UnitsList)
            {
                if (unit.id == unitId)
                {
                    GamePlay.Damage.TakeDamage(unit, GamePlay.Damage.GetDamage(unit));
                    GamePlay.Gameplay.Instance().gamePlayInput.UpdateHp(unit);
                }
            }
        }

        public List<BattleMapBlock> GetMapBlockInSpecificBattleArea(Vector2 pos)
        {
            // TODO: 完成实现
            return null;
        }

        public List<GameUnit.GameUnit> GetGameUnitsInBattleArea(Vector2 pos)
        {
            List<BattleMapBlock> blocks = GetMapBlockInSpecificBattleArea(pos);
            if (blocks == null)
            {
                return null;
            }
            List<GameUnit.GameUnit> units = new List<GameUnit.GameUnit>();

            foreach (BattleMapBlock mapBlock in blocks)
            {
                if (mapBlock.units_on_me.Count > 0)
                    units.AddRange(mapBlock.units_on_me);
            }
            return units;
        }

        public void RecoverUnitsHp(string unitId, int amount)
        {
            // TODO : 完成实现
        }

        public void SendUnitToDeath(string unitId)
        {
            // TODO: 完成实现
        }
    }
}