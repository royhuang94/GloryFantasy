using UnityEngine;
using System.Collections.Generic;
using Unit = GameUnit.GameUnit;

public class ShowRange : MonoBehaviour
{
    public int rows = 8;
    public int columns = 8;
    public Unit unit;
    private void Awake()
    {
        this.unit = gameObject.GetComponent<Unit>();
    }

    public List<Vector2> GetPositionsWithinCertainMd(Vector2 position, int ManhattanDistance)
    {
        List<Vector2> reslist = new List<Vector2>();
        RecrusiveBody((int)position.x, (int)position.y, ManhattanDistance, reslist);
        return reslist;
    }

    public void RecrusiveBody(int x, int y, int leftManhattanDistance, List<Vector2> reslist)
    {
        if (x < 0 || y < 0 || x > rows || y > columns) return;
        reslist.Add(new Vector2(x, y));
        if (leftManhattanDistance == 0)
            return;

        RecrusiveBody(x + 1, y, leftManhattanDistance - 1, reslist);
        RecrusiveBody(x - 1, y, leftManhattanDistance - 1, reslist);
        RecrusiveBody(x, y + 1, leftManhattanDistance - 1, reslist);
        RecrusiveBody(x, y - 1, leftManhattanDistance - 1, reslist);
    }
    
    public void MarkMoveRange()
    {
        MapManager.MapManager.getInstance().ColorMapBlocks(
            GetPositionsWithinCertainMd(unit.mapBlockBelow.GetCoordinate(), unit.mov), Color.green);
    }

    public void MarkAttackRange()
    {
        MapManager.MapManager.getInstance().ColorMapBlocks(
            GetPositionsWithinCertainMd(unit.mapBlockBelow.GetCoordinate(), unit.rng), Color.red);
    }

    public void CancleMoveRangeMark()
    {
        MapManager.MapManager.getInstance().ColorMapBlocks(
            GetPositionsWithinCertainMd(unit.mapBlockBelow.GetCoordinate(), unit.mov), Color.white);
    }

    public void CancleAttackRangeMark()
    {
        MapManager.MapManager.getInstance().ColorMapBlocks(
            GetPositionsWithinCertainMd(unit.mapBlockBelow.GetCoordinate(), unit.rng), Color.white);
    }
}