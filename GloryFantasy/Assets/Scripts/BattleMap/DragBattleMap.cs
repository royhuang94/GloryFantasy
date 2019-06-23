using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BattleMap
{
    public class DragBattleMap : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        #region 变量
        bool isEnter = false;//鼠标是否进入
        private bool isMove = false;//是否可以移动
        float _recoverScale;//战斗地图的初始的缩放大小
        float _moveX;//x方向移动距离
        float _moveY;//y方向移动距离
        Vector3 starPos = Vector3.zero;//鼠标按下时的鼠标坐标
        Vector3 endPos = Vector3.zero;//鼠标抬起时的鼠标坐标
        Vector3 temPos = Vector3.zero;//记录战斗地图缩放为全视野时前的位置
        int rows;//战斗地图的格子行数
        int columns;//战斗地图的格子列数
        #endregion

        // Use this for initialization
        void Start()
        {
            isEnter = false;
            _moveX = transform.localPosition.x;
            _moveY = transform.localPosition.y;
            _recoverScale = transform.localScale.x;
        }

        // Update is called once per frame
        void Update()
        {
            if (isEnter)
            {
                OnMouseDown();
            }
            //A键还原
            if (Input.GetKey(KeyCode.A))
            {
                transform.localScale = new Vector3(_recoverScale, _recoverScale, _recoverScale);
                transform.localPosition = new Vector3(0f, 1.5f, 0f);
            }
            ScaleBattleMap();
        }

        //拖动地图
        void OnMouseDown()
        {
            //将物体由世界坐标系转换为屏幕坐标系

            #region 跟随鼠标移动
            //Vector3 offset = transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenSpace.z));
            //Vector3 screenSpace = Camera.main.WorldToScreenPoint(transform.position);
            //Debug.Log(offset.y);
            //while (Input.GetMouseButton(0))
            //{
            //    ////得到现在鼠标的2维坐标系位置
            //    //Vector3 curScreenSpace = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenSpace.z);
            //    ////将当前鼠标的2维位置转换成3维位置，再加上鼠标的移动量
            //    //Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenSpace) + offset;
            //    ////curPosition就是物体应该的移动向量赋给transform的position属性
            //    //transform.position = curPosition;
            //    yield return new WaitForFixedUpdate(); //这个很重要，循环执行
            //
            #endregion
            rows = BattleMap.Instance().Rows;
            columns = BattleMap.Instance().Columns;
            //一次移动一格的距离
            if (Input.GetMouseButtonDown(0))
            {
                isMove = true;
                starPos = Input.mousePosition;
            }
            if (Input.GetMouseButtonUp(0))
            {
                endPos = Input.mousePosition;
                Vector3 offset = starPos - endPos;
                if (System.Math.Abs(offset.x) >= System.Math.Abs(offset.y) && System.Math.Abs(offset.x) >= 5 && rows > 6)
                {
                    if (offset.x >= 0)
                        _moveX -= 2 * _recoverScale;
                    else
                        _moveX += 2 * _recoverScale;
                    transform.localPosition = new Vector3(_moveX, transform.localPosition.y);
                }
                if (System.Math.Abs(offset.x) < System.Math.Abs(offset.y) && System.Math.Abs(offset.y) >= 5 && columns > 10)
                {
                    if (offset.y >= 0)
                        _moveY -= 2 * _recoverScale;
                    else
                        _moveY += 2 * _recoverScale;
                    transform.localPosition = new Vector3(transform.localPosition.x, _moveY);
                }
                isMove = false;
            }
        }
        //缩放地图
        private void ScaleBattleMap()
        {
            if (Input.GetMouseButtonDown(2))//缩小整个地图到视野内
            {
                temPos = transform.localPosition;
                transform.localScale = new Vector3(0.4f, 0.4f, 0f);
                transform.localPosition = new Vector3(0f, 1.5f, 0f);
            }
            if (Input.GetMouseButtonUp(2))
            {
                transform.localScale = new Vector3(_recoverScale, _recoverScale, 0f);
                transform.localPosition = temPos;
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            starPos = Input.mousePosition;
            isEnter = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (isMove == false)
            {
                isEnter = false;
            }
        }
    }
}


