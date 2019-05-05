﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unit = GameUnit.GameUnit;

namespace AI
{
    public class HatredItem
    {
        /// <summary>
        /// 被仇恨的单位
        /// </summary>
        public Unit battleUnit;
        /// <summary>
        /// 仇恨值
        /// </summary>
        public int hatred;
    }


    public class HatredRecorder
        : IComparer<HatredItem>
    {
        /// <summary>
        /// 当前仇恨列表的拥有者
        /// </summary>
        private Unit host;
        /// <summary>
        /// 仇恨列表
        /// </summary>
        private List<HatredItem> hatredList = new List<HatredItem>(5);


        /// <summary>
        /// 重置仇恨列表
        /// </summary>
        /// <param name="hostUnit">仇恨列表的拥有者</param>
        /// <param name="enemyTeam">敌人队伍列表</param>
        public void Reset(Unit hostUnit, List<Unit> enemyTeam)
        {
            Clean();

            if(hostUnit == null || enemyTeam == null)
            {
                Debug.Log("重置仇恨列表失败");
                return;
            }

            host = hostUnit;
            for (int i = 0; i < enemyTeam.Count; i++) //遍历仇恨列表
            {
                if(i >= hatredList.Count)
                    hatredList.Add(new HatredItem()); //当敌人数量大于优先设置的仇恨列表总容量时，动态增加列表，以防数组越界

                hatredList[i].battleUnit = enemyTeam[i];
                hatredList[i].hatred = 0; //gui 000.....
            }

            if (hatredList.Count > enemyTeam.Count)
                hatredList.RemoveRange(enemyTeam.Count, hatredList.Count - enemyTeam.Count); //删除多余部分
        }

        /// <summary>
        /// 清空仇恨列表
        /// </summary>
        public void Clean()
        {
            host = null;
            for(int i= 0; i <hatredList.Count; i++)
            {
                hatredList[i].battleUnit = null;
                hatredList[i].hatred = 0;
            }

        }

        /// <summary>
        /// 排序仇恨列表
        /// </summary>
        private void SortHatred()
        {
            //简单的排序
            hatredList.Sort(this);
        }

        /// <summary>
        ///记录仇恨列表
        ///对仇恨值做加法
        /// </summary>
        /// <param name="id">单位ID</param>
        ///id 因为Json获取下来的值类型为string
        /// <param name="hatredIncrease">仇恨增加值</param>
        public void RecordedHatred(string id, int hatredIncrease)
        {
            for(int i = 0; i < hatredList.Count; i++)
            {
                if(hatredList[i].battleUnit.id == id)
                {
                    //原始仇恨值
                    int originHatred = hatredList[i].hatred;
                    originHatred += hatredIncrease;
                    //仇恨值不能 ＜ 0
                    if (originHatred < 0)
                        originHatred = 0;
                    //记录新的仇恨值
                    hatredList[i].hatred = originHatred;
                    return; 
                }
            }
        }

        /// <summary>
        /// 记录仇恨的数量
        /// </summary>
        public int HatredCount
        {
            get
            {
                return hatredList.Count;
            }
        }

        /// <summary>
        /// 根据索引获得仇恨列表中的战斗单位
        /// </summary>
        /// <param name="index">仇恨列表的索引</param>
        /// <param name="isSort">是否需要排序</param>
        /// <returns></returns>
        public Unit GetHatredByIndex(int index, bool isSort)
        {
            if (hatredList.Count == 0)
                return null;

            if (isSort)
                SortHatred();

            //防止越界
            return hatredList[index > hatredList.Count - 1 ? hatredList.Count : index].battleUnit;
        }

        public int Compare(HatredItem x, HatredItem y)
        {
            return -1;
        }
    }

}


