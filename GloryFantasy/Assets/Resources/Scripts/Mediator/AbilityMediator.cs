using System.Collections.Generic;
using BattleMap;
using GameCard;
using GameUnit;
using UnityEngine;

namespace Mediator
{
    public class AbilityMediator : UnitySingleton<AbilityMediator>, AbilityMediatorInterface
    {
        /// <summary>
        /// 移除重复元素
        /// </summary>
        /// <param name="reslist"></param>
        private void RemoveRepeat(List<Vector2> reslist)
        {
            for (int i = 0; i < reslist.Count; i++)
            {
                for (int j = reslist.Count - 1; j > i; j--)
                {
                    if (reslist[i] == reslist[j])
                    {
                        reslist.RemoveAt(j);
                    }
                }
            }
        }

        //public bool CheckUnitDeathById(string unitId)
        //{
        //    return GameUnitPool.Instance().CheckDeathByID(unitId);
        //}

        public void ReduceSpecificCardCd(string unitId, int amount)
        {
            CardManager.Instance().HandleCooldownEvent(unitId, amount);
        }

        public List<BattleMapBlock> GetMapBlocksWithinRange(Vector2 cordinate, int range)
        {
            List<BattleMapBlock> battleMapBlocks = new List<BattleMapBlock>();
            BattleMapBlock battleMapBlock = null;

            List<Vector2> vector2s = GameGUI.ShowRange.Instance().GetSkillRnage(cordinate, range);
            RemoveRepeat(vector2s);//去重
            for (int i = 0; i < vector2s.Count; i++)
            {
                battleMapBlock = BattleMap.BattleMap.Instance().GetSpecificMapBlock(vector2s[i]);
                if (battleMapBlock != null)
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
            //获取战区id
            BattleMapBlock _mapBlock = BattleMap.BattleMap.Instance().GetSpecificMapBlock(pos);
            int area = _mapBlock.area;

            BattleMapBlock mapBlock = null;
            List<BattleMapBlock> battleMapBlocks = new List<BattleMapBlock>();

            List<Vector2> vector2s = BattleMap.BattleMap.Instance().battleAreaData.GetBattleAreaAllPosByID(area);
            foreach(Vector2 v in vector2s)
            {
                mapBlock = BattleMap.BattleMap.Instance().GetSpecificMapBlock(v);
                battleMapBlocks.Add(mapBlock);
            }
            return battleMapBlocks;
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

        /// <summary>
        /// 治疗某一个单位特定的血量（负值治疗量不会生效）。
        /// </summary>
        /// <param name="unit">被治疗的单位</param>
        /// <param name="amount">治疗量</param>
        public void RecoverUnitsHp(GameUnit.GameUnit unit, int amount)
        {
            if (amount > 0)
                unit.changeHP(amount);
        }

        public void SendUnitToDeath(string unitId)
        {
            // TODO: 完成实现
        }
    }
}