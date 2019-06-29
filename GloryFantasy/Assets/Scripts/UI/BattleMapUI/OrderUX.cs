using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using Ability;
using BattleMap;
using GameUnit;
using Unit = GameUnit.GameUnit;
using UnityEngine;

public class OrderUX : UnitySingleton<OrderUX>
{
    private BattleMap.BattleMap _battleMap;
    
    public void ClickCardToHighLight(AbilityTarget abilityTarget, Color color)
    {
        _battleMap = BattleMap.BattleMap.Instance();
        // 是释放者
        if (abilityTarget.IsSpeller)
        {        
            // 释放者是友军
            if (abilityTarget.TargetType == TargetType.Friendly)
            {
                ColorUnits(abilityTarget, color);
            }
            // 释放者是敌军
            // 现在好像还没有这种情况，先写着吧
            else
            {
                ColorUnits(abilityTarget, color);
            }
        }
        // 不是释放者，即目标，一般是地格
        else
        {
            switch (abilityTarget.TargetType)
            {
                case TargetType.All:
                    // 合并地图上所有友方单位和敌方单位
                    ColorUnits(abilityTarget, Color.green);
                    break;
                case TargetType.Enemy:
                    ColorUnits(abilityTarget, Color.green);
                    break;
                case TargetType.Field:
                    ColorBlocks(abilityTarget, new Color(0, 1, 1, 0.8f));
                    break;
                case TargetType.Friendly:
                    ColorUnits(abilityTarget, Color.green);
                    break;
            }
        }
    }

    public void CancelColorAll(AbilityTarget abilityTarget)
    {
        ColorUnits(abilityTarget, Color.white);
        ColorBlocks(abilityTarget, Color.white);
    }
    /// <summary>
    /// 按order牌规则给单位着色
    /// </summary>
    /// <param name="abilityTarget">异能目标 1，一般是释放者</param>
    /// <param name="color">颜色</param>
    private void ColorUnits(AbilityTarget abilityTarget, Color color)
    {
        List<Unit> gameUnits;
        switch (abilityTarget.TargetType)
        {
            case TargetType.All:
                // 合并地图上所有友方单位和敌方单位
                gameUnits = _battleMap.GetFriendlyUnitsList().Union(_battleMap.GetEnemyUnitsList()).ToList<Unit>();
                break;
            case TargetType.Enemy:
                gameUnits = _battleMap.GetEnemyUnitsList();
                break;
            case TargetType.Friendly:
                gameUnits = _battleMap.GetFriendlyUnitsList();
                break;
            default:
                gameUnits = _battleMap.GetFriendlyUnitsList();
                break;
        }
        foreach (Unit enemyUnit in gameUnits)
        {
            // 有颜色限制
            if (abilityTarget.color != null && abilityTarget.color.Count > 0)
            {
                // 单位颜色不在需要的颜色里
                if(!abilityTarget.color.Contains(enemyUnit.Color))
                    continue;
                UnitManager.ColorUnitOnBlock(enemyUnit.mapBlockBelow.position, color);
            }
            // 没有颜色限制
            else
            {
                UnitManager.ColorUnitOnBlock(enemyUnit.mapBlockBelow.position, color);
            }
        }
    }

    private void ColorBlocks(AbilityTarget abilityTarget, Color color)
    {
        List<Vector2> positions = GetLegalPositionList(abilityTarget);

        _battleMap.ColorMapBlocks(positions, color);
    }

    private List<Vector2> GetLegalPositionList(AbilityTarget abilityTarget)
    {
        List<Vector2> legalPositions = new List<Vector2>();
        switch (abilityTarget.ControllerType)
        {
            case ControllerType.All:
                legalPositions = PositionsWithoutUnit(_battleMap.battleAreaData.GetAllBlocks());
                break;
            case ControllerType.Enemy:
                legalPositions = PositionsWithoutUnit(_battleMap.battleAreaData.GetAllBlocksInEnemyBA());
                break;
            case ControllerType.Neutral:
                legalPositions = PositionsWithoutUnit(_battleMap.battleAreaData.GetAllBlocksInNeutralityBA());
                break;
            case ControllerType.Friendly:
                legalPositions = PositionsWithoutUnit(_battleMap.battleAreaData.GetAllBlocksInPlayerBA());
                break;
        }
        return legalPositions;
    }

    private List<Vector2> PositionsWithoutUnit(List<BattleMapBlock> blocks)
    {
        List<Vector2> positions = new List<Vector2>();
        foreach (BattleMapBlock block in blocks)
        {
            // 如果该位置有单位，则不高亮
            if (_battleMap.CheckIfHasUnits(block.GetCoordinate()))
                continue;
            positions.Add(block.position);
        }

        return positions;
    }
    
    private List<Unit> GetLegalUnitList()
    {
        List<Unit> legalUnits = new List<Unit>();
        return legalUnits;
    }
}
