using UnityEngine;
using System;
using System.Collections.Generic;
using Unit = GameUnit.GameUnit;
using UnityEngine.EventSystems;

using GamePlay;


//TODO 通过 自身 this.transform.position与第一块儿地图块儿坐标(298.8)的差，的几倍关系得到具体为(0, 0) -> (7, 7)得物体坐标

namespace BattleMap
{
    public enum AStarState
    {
        free,
        isInOpenList,
        isInCloseList
    }


    public enum EMapBlockType
    {
        normal,   //普通地图块儿
        burnning, //灼烧块儿
        Retire,   //滞留块儿
        aStarPath   //A星路径
    }


    public class BattleMapBlock : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
    {
        private void Awake()
        {
            setMapBlackPosition();
        }

        /// <summary>
        /// 向方块上增加GameUnit
        /// </summary>
        /// <param name="unit"></param>
        public void AddUnit(Unit unit)
        {
            //Debug.Log("MapBlocks--Added unit:" + unit.ToString());
            units_on_me.Add(unit);
            //在Hierarchy中，还需要把单位添加到Block下
            //修改单位的父级对象
            unit.gameObject.transform.SetParent(this.transform);
        }
        /// <summary>
        /// 向方块上增加GameUnit/ 疑问为啥一个地图块儿阔能出现多只单位？
        /// </summary>
        /// <param name="units"></param>
        public void AddUnits(Unit[] units)
        {
            //Debug.Log("MapBlocks--Adding Units");
            foreach (Unit gameUnit in units)
            {
                units_on_me.Add(gameUnit);
                //在Hierarchy中，还需要把单位添加到Block下
                //修改单位的父级对象
                gameUnit.gameObject.transform.SetParent(this.transform);
            }
        }
        /// <summary>
        /// 方块上移除GameUnit
        /// </summary>
        /// <param name="unit"></param>
        public void RemoveUnit(Unit unit)
        {
            //Debug.Log("MapBlocks--Removed unit:" + unit.ToString());
            units_on_me.Remove(unit);
        }
        /// <summary>
        /// 获得这个方块的虚拟坐标
        /// </summary>
        /// <returns></returns>
        public Vector3 GetCoordinate()
        {
            //对于新添加的单位以及移动后的单位，需要对其进行更新
            return new Vector3(this.x, this.y, 0f);
        }
        /// <summary>
        /// 获取该地图块自身的世界系坐标
        /// </summary>
        /// <returns></returns>
        public Vector3 GetSelfPosition()
        {
            return coordinate;
        }
        /// <summary>
        /// update地图块的世界系坐标
        /// </summary>
        private void setMapBlackPosition()
        {
            coordinate = new Vector3((int)transform.position.x, (int)transform.position.y, 0.0f);
        }

        //处理地图块点击事件
        public void OnPointerDown(PointerEventData eventData)
        {
            Gameplay.Instance().gamePlayInput.OnPointerDown(this, eventData);
        }

        //显示战区
        public void OnPointerEnter(PointerEventData eventData)
        {
            Gameplay.Instance().gamePlayInput.OnPointerEnter(this, eventData);
        }
        //隐藏战区
        public void OnPointerExit(PointerEventData eventData)
        {
            Gameplay.Instance().gamePlayInput.OnPointerExit(this, eventData);
        }



        private Vector3 coordinate;

        public int area { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public string[] data { get; set; }
        public string type { get; set; }
        public int tokenCount;
        public List<Unit> units_on_me = new List<Unit>();
        public EMapBlockType blockType { get; set; }
        public Vector2 position
        {
            get { return new Vector2(x, y); } 
        }

        ////这些变量为什么会放在这里？
        ///neighbourBlock这个成员非常危险
        //public BattleMapBlock[] neighbourBlock = new BattleMapBlock[4];
        //public BattleMapBlock parentBlock = null;
        //public AStarState aStarState { get; set; }

        ////最优路径计算
        //public float F = 0;
        //public float G = 0;
        //public float H = 0;

    }
}

