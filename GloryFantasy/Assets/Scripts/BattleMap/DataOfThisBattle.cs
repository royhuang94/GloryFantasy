using GamePlay.Encounter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GamePlay
{
    public class DataOfThisBattle : MonoBehaviour
    {
        #region 变量
        private int deathPage;//死页数
        private int ap;//专注值
        #endregion

        public int DeathPage { get { return deathPage; } }//死页数
        public int AP { get { return ap; } }//专注值，计算方式为：默认专注值 + 增益专注值

        /// <summary>
        /// 初始数据
        /// </summary>
        public void InitData()
        {
            deathPage = EncouterData.Instance().GetEncounter().deathPage;
            ap = Player.Instance().ap;
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
    }

}
