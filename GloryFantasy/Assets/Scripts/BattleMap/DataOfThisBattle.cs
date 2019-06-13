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
        private int deathPage;//死页数
        private int ap;//专注值
        private List<Unit> leaders;//“领导者们"
        #endregion

        public int DeathPage { get { return deathPage; } }//死页数
        public int AP { get { return ap; } }//专注值，计算方式为：默认专注值 + 增益专注值
        public List<Unit> Leaders { get { return leaders; } }//"领导者们",这里面的单位都是"领导"

        /// <summary>
        /// 初始数据
        /// </summary>
        public void InitData()
        {
            deathPage = EncouterData.Instance().GetEncounter().deathPage;
            ap = Player.Instance().ap;
            GetLeaders();
        }

        /// <summary>
        /// 增加死页
        /// </summary>
        /// <param name="amount"></param>
        public void AddDeathPage(int amount)
        {
            ap += amount;
        }

        /// <summary>
        /// 增加ap
        /// </summary>
        /// <param name="ap"></param>
        public void AddAP(int ap)
        {
            Player.Instance().AddAp(ap);
        }

        /// <summary>
        /// 获取"领导者"
        /// </summary>
        private void GetLeaders()
        {
            leaders = new List<Unit>();
            foreach(Unit unit in BattleMap.BattleMap.Instance().UnitsList)
            {
                //if(unit.isLeader == 1)//TODO等待单位初始isLeader属性
                //{
                //    leaders.Add(unit);
                //}
            }
        }
    }

}
