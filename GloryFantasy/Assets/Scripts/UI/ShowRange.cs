using UnityEngine;
using System.Collections.Generic;
using Unit = GameUnit.GameUnit;

using GameUnit;

namespace GameGUI
{

    public class ShowRange : MonoBehaviour
    {
        private int rows;
        private int columns;
        private Unit unit;
        private bool unitMove;
        private void Awake()
        {
            this.unit = gameObject.GetComponent<Unit>();
        }

        private void Start()
        {
            
            rows = BattleMap.BattleMap.Instance().Rows;
            columns = BattleMap.BattleMap.Instance().Columns;
        }

        public List<Vector2> GetPositionsWithinCertainMd(Vector2 position, int ManhattanDistance)
        {
            List<Vector2> reslist = new List<Vector2>();
            if (unitMove)
            {
                RecrusiveBody((int)position.x, (int)position.y, ManhattanDistance, reslist);
                RemoveMapBlokHasUnit(reslist);
            }
            else
            {
                RecrusiveBody((int)position.x, (int)position.y, ManhattanDistance, reslist);               
           }
            return reslist;

        }

        private void RecrusiveBody(int x, int y, int leftManhattanDistance, List<Vector2> reslist)
        {
            if (x < 0 || y < 0 || x >= rows || y >= columns) return;
            reslist.Add(new Vector2(x, y));
            if (leftManhattanDistance == 0)
                return;
            RecrusiveBody(x + 1, y, leftManhattanDistance - 1, reslist);
            RecrusiveBody(x - 1, y, leftManhattanDistance - 1, reslist);
            RecrusiveBody(x, y + 1, leftManhattanDistance - 1, reslist);
            RecrusiveBody(x, y - 1, leftManhattanDistance - 1, reslist);
        }

        //移动范围不显示有单位的地图块
        //不显示无法到达的地图块
        public void RemoveMapBlokHasUnit(List<Vector2> reslist)
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
            for (int i = 0; i < reslist.Count; i++)
            {
                if (BattleMap.BattleMap.Instance().CheckIfHasUnits(reslist[i]))
                {
                    reslist.Remove(reslist[i]);
                }
            }
        }

        public void MarkMoveRange(Vector2 target)
        {
            unitMove = true;
            BattleMap.BattleMap.Instance().ColorMapBlocks(
                GetPositionsWithinCertainMd(target, unit.mov), Color.green);
        }

        public void MarkAttackRange(Vector2 target)
        {
            BattleMap.BattleMap.Instance().ColorMapBlocks(
                GetPositionsWithinCertainMd(target, unit.rng), Color.red);
        }

        public void CancleMoveRangeMark(Vector2 target)
        {
            unitMove = false;
            BattleMap.BattleMap.Instance().ColorMapBlocks(
                 GetPositionsWithinCertainMd(target, unit.mov), Color.white);
        }

        public void CancleAttackRangeMark(Vector2 target)
        {
            BattleMap.BattleMap.Instance().ColorMapBlocks(
                 GetPositionsWithinCertainMd(target, unit.rng), Color.white);
        }

        //技能释放范围
        public void MarkSkillRange(Vector2 target, int range)
        {
            BattleMap.BattleMap.Instance().ColorMapBlocks(
                GetPositionsWithinCertainMd(target, range), Color.red);
        }

        public void CancleSkillRangeMark(Vector2 target,int range)
        {
            BattleMap.BattleMap.Instance().ColorMapBlocks(
                GetPositionsWithinCertainMd(target, range), Color.white);
        }
    }
}