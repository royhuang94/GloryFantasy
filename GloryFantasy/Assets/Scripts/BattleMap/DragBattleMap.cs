using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BattleMap
{
    public class DragBattleMap : MonoBehaviour, IPointerEnterHandler,IPointerExitHandler
    {
        bool isEnter;

        private bool one_click = false;
        private float time;
        float _scale = 1.0f;
        float _recoverScale;
        float _moveX;
        float _moveY;
        Vector3 starPos = Vector3.zero;
        Vector3 endPos = Vector3.zero;
        Vector3 temPos = Vector3.zero;

        Vector3 pos = Vector3.zero;

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
                StartCoroutine(OnMouseDown());
            }
            //A键还原
            if (Input.GetKey(KeyCode.A))
            {
                _scale = 1;
                transform.localScale = new Vector3(_recoverScale, _recoverScale, _recoverScale);
                transform.localPosition = new Vector3(0f, 1.5f, 0f);
            }
            ScaleBattleMap();
        }

        //拖动地图
        IEnumerator OnMouseDown()
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

            //一次移动一格的距离
            if (Input.GetMouseButtonDown(0))
            {
                starPos = Input.mousePosition;
            }
            if (Input.GetMouseButtonUp(0))
            {
                endPos = Input.mousePosition;
                Vector3 offset = starPos - endPos;
                if (System.Math.Abs(offset.x) >= System.Math.Abs(offset.y) && System.Math.Abs(offset.x) >= 10)
                {
                    if (offset.x >= 0)
                        _moveX -= 2 * _recoverScale;
                    else
                        _moveX += 2 * _recoverScale;
                    transform.localPosition = new Vector3(_moveX, transform.localPosition.y);
                }
                if(System.Math.Abs(offset.x) < System.Math.Abs(offset.y) && System.Math.Abs(offset.y) >= 10)
                {
                    if (offset.y >= 0)
                        _moveY -= 2 * _recoverScale;
                    else
                        _moveY += 2 * _recoverScale;
                    transform.localPosition = new Vector3(transform.localPosition.x, _moveY);
                }
                isEnter = false;
            }
            yield return new WaitForFixedUpdate();

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
            isEnter = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
        }
    }
}

