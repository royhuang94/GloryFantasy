using UnityEngine;
using System;
using System.Collections.Generic;
using Unit = GameUnit.GameUnit;
using UnityEngine.EventSystems;




//TODO 通过 自身 this.transform.position与第一块儿地图块儿坐标(298.8)的差，的几倍关系得到具体为(0, 0) -> (7, 7)得物体坐标

namespace BattleMap
{
    public enum AStarState
    {
        free,
        isOpenList,
        isCloseList
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
        
        
        //public MapBlock(int area, string[] data)
        //{
        //    this.area = area;
        //    this.data = data;
        //}

        //public MapBlock()
        //{
        //    this.area = 0;
        //    this.data = null;
        //}

        public BattleMapBlock() { }
        public BattleMapBlock(int x,int y)
        {
            this.x = x;
            this.y = y;
        }


        //public List<Unit> GetGameUnits()
        //{
        //    Debug.Log("getunitList");
        //    this.units_on_me = MapManager.getInstance().UnitsList;
        //    return this.units_on_me;
        //}

        public void AddUnit(Unit unit)
        {
            Debug.Log("MapBlocks--Added unit:" + unit.ToString());
            units_on_me.Add(unit);
        }

        public void AddUnits(Unit[] units)
        {
            Debug.Log("MapBlocks--Adding Units");
            foreach (Unit gameUnit in units)
            {
                units_on_me.Add(gameUnit);
            }
        }

        public void RemoveUnit(Unit unit)
        {
            Debug.Log("MapBlocks--Removed unit:" + unit.ToString());
            units_on_me.Remove(unit);
        }

        public Vector3 GetCoordinate()
        {
            return new Vector3(this.x, this.y, 0f);
        }

        //获取该地图块自身的位置
        public Vector3 GetSelfPosition()
        {
            //return this.transform.position;
            return coordinate;
        }       


        //处理地图块点击事件
        public void OnPointerDown(PointerEventData eventData)
        {
            if (UnitManager.Instance.IsPickedUnit)
            {
                //检测到地图，可实例化棋子
                if (!BattleMap.getInstance().WarZoneBelong(GetSelfPosition())) return;
                UnitManager.Instance.CouldInstantiation(true,this.transform);

            }

            /*GameObject go = */
            //MapManager.getInstance().OnClickInstantiateUnit();
            //go.transform.SetParent(this.transform);
            //go.transform.localScale = GetCoordinate();
            //go.transform.localPosition = Vector3.zero;

            BattleMap.getInstance().curMapPos = GetSelfPosition();
            Debug.Log(GetSelfPosition());
        }

        private void Awake()
        {
            setMapBlackPosition();
        }

        //处理地图块的坐标
        private void setMapBlackPosition()
        {

            coordinate = new Vector3((int)transform.position.x, (int)transform.position.y, 0.0f);
            //Debug.Log(coordinate);
            //var minusValue = Mathf.Abs(this.transform.position.x - coordinateSub);
            //Debug.Log("Position: " + this.transform.position + "/(0, " + minusValue / coordinateDivisor + ")");

        }

        public int Distance(BattleMapBlock target)
        {
            //TODO 计算两点间的距离

           
            return 0;
        }

        //显示战区
        public void OnPointerEnter(PointerEventData eventData)
        {
            BattleMap.getInstance().ShowBattleZooe(GetSelfPosition());
        }
        //隐藏战区
        public void OnPointerExit(PointerEventData eventData)
        {
            BattleMap.getInstance().HideBattleZooe(GetSelfPosition());
        }




        private Vector3 coordinate;

        public int area { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public string[] data { get; set; }
        public string type { get; set; }
        public int tokenCount;
        public List<Unit> units_on_me = new List<Unit>();
        public BattleMapBlock[] neighbourBlocke { get; set; }
        public AStarState aStarState { get; set; }
        public EMapBlockType blockType { get; set; }

        //指向obj类型的通用临时使用的指针
        public System.Object tempRef;
    }
}

