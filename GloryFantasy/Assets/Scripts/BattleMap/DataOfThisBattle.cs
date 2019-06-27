using GamePlay.Encounter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unit = GameUnit.GameUnit;

/// <summary>
/// 该次遭遇的一些临时数据
/// </summary>
namespace GamePlay
{
    public class DataOfThisBattle
    {
        #region 变量
        private int deathPage;//死页数（上限）
        private int curDeathPage;//当前死页数
        private int ap;//专注值
        private List<Unit> leaders;//“领导者们"
        #endregion

        public int DeathPage { get { return deathPage; } }//死页数（上限）
        public int CurDeathPage { get { return curDeathPage; } }//当前死页数

        /// <summary>
        /// 初始数据
        /// </summary>
        public void InitData(string encounterID)
        {
            deathPage = EncouterData.Instance()._encounterData[encounterID].deathPage;
            curDeathPage = 0;
        }

        /// <summary>
        /// 增加死页
        /// </summary>
        /// <param name="amount"></param>
        public void AddDeathPage()
        {
            curDeathPage++;
        }

        /// <summary>
        /// 获取"领导者"
        /// </summary>
        public List<Unit> GetLeaders()
        {
            List<Unit> leaders = new List<Unit>();
            foreach(Unit unit in BattleMap.BattleMap.Instance().UnitsList)
            {
                if (unit.isLeader == 1)
                {
                    leaders.Add(unit);
                }
            }
            return leaders;
        }
    }

}
