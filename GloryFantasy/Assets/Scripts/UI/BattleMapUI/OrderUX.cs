using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using Ability;
using BattleMap;
using GameGUI;
using GameUnit;
using Unit = GameUnit.GameUnit;
using UnityEngine;

public class OrderUX : UnitySingleton<OrderUX>
{
    private BattleMap.BattleMap _battleMap;
    private Vector2 _centerPosition;
    private AbilityVariable _abilityVariable;
    private string _abilityName;
    
    // TODO：测试有一点点问题，可能是判断技能范围哪里不对，或者理解错误，明天再看看
    
    // TODO：取消使用效果牌，应该取消效果显示
    
    /// <summary>
    /// 开放接口，点击高亮范围
    /// </summary>
    /// <param name="abilityTarget">发动异能目标</param>
    /// <param name="abilityVariable">发动异能参数</param>
    /// <param name="abilityName">发动异能名</param>
    /// <param name="color">高亮颜色</param>
    /// <param name="position">位置</param>
    public void ClickToHighLight(AbilityTarget abilityTarget, AbilityVariable abilityVariable, string abilityName, Color color, Vector2 position)
    {
        _battleMap = BattleMap.BattleMap.Instance();
        _centerPosition = position;
        _abilityVariable = abilityVariable;
        _abilityName = abilityName;
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
                    ColorUnits(abilityTarget, Color.green);
                    break;
                case TargetType.Enemy:
                    ColorUnits(abilityTarget, Color.green);
                    break;
                case TargetType.Field:
                    ColorBlocks(abilityTarget, new Color(0, 0, 1, 0.5f));
                    break;
                case TargetType.Friendly:
                    ColorUnits(abilityTarget, Color.green);
                    break;
            }
        }
    }

    /// <summary>
    /// 取消高亮
    /// </summary>
    /// <param name="abilityTarget">发动异能目标</param>
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
        
        // 合法单位
        List<Unit> legalUnits = new List<Unit>();
        
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

        // 发动者则不用判断是否在异能范围内，默认全图应该
        // 不是发动者则取战区所有单位和符何的单位的交集
        legalUnits = abilityTarget.IsSpeller ? gameUnits : gameUnits.Intersect(GetLegalUnitList()).ToList<Unit>();
        
        foreach (Unit enemyUnit in legalUnits)
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

    /// <summary>
    /// 按规则给地块着色
    /// </summary>
    /// <param name="abilityTarget">发动异能目标</param>
    /// <param name="color"></param>
    private void ColorBlocks(AbilityTarget abilityTarget, Color color)
    {
        List<Vector2> positions = GetLegalPositionList(abilityTarget);

        _battleMap.ColorMapBlocks(positions, color);
    }

    /// <summary>
    /// 获取合法的地块列表
    /// </summary>
    /// <param name="abilityTarget"></param>
    /// <returns></returns>
    private List<Vector2> GetLegalPositionList(AbilityTarget abilityTarget)
    {
        // 合法地块
        List<Vector2> legalPositions = new List<Vector2>();
        // 战区所有地块
        List<Vector2> warZonePositions = new List<Vector2>();
        switch (abilityTarget.ControllerType)
        {
            case ControllerType.All:
                warZonePositions = PositionsWithoutUnit(_battleMap.battleAreaData.GetAllBlocks());
                break;
            case ControllerType.Enemy:
                warZonePositions = PositionsWithoutUnit(_battleMap.battleAreaData.GetAllBlocksInEnemyBA());
                break;
            case ControllerType.Neutral:
                warZonePositions = PositionsWithoutUnit(_battleMap.battleAreaData.GetAllBlocksInNeutralityBA());
                break;
            case ControllerType.Friendly:
                warZonePositions = PositionsWithoutUnit(_battleMap.battleAreaData.GetAllBlocksInPlayerBA());
                break;
        }

        Debug.Log(_abilityName + ": " + _abilityVariable.Range);
        // 取战区地块和合法地块的交集
        legalPositions = warZonePositions.Intersect(ShowRange.Instance().GetSkillRnage(_centerPosition, (int) _abilityVariable.Range)).ToList<Vector2>();
        return legalPositions;
    }

    /// <summary>
    /// 没有单位的地块坐标列表
    /// </summary>
    /// <param name="blocks">要筛选的地块列表</param>
    /// <returns></returns>
    private List<Vector2> PositionsWithoutUnit(List<BattleMapBlock> blocks)
    {
        List<Vector2> positions = new List<Vector2>();
        foreach (BattleMapBlock block in blocks)
        {
            // 如果该位置有单位，则不加入
            if (_battleMap.CheckIfHasUnits(block.GetCoordinate()))
                continue;
            positions.Add(block.position);
        }

        return positions;
    }
    
    /// <summary>
    /// 获取合法单位
    /// </summary>
    /// <returns></returns>
    private List<Unit> GetLegalUnitList()
    {
        // 念动投掷写死了2
        int range = _abilityName.Equals("念动投掷") ? 2 : (int) _abilityVariable.Range;
        
        List<Unit> legalUnits = new List<Unit>();
        foreach (Vector2 pos in ShowRange.Instance().GetSkillRnage(_centerPosition, range))
        {
            legalUnits.Add(_battleMap.GetUnitsOnMapBlock(pos));
        }
        return legalUnits;
    }
}
