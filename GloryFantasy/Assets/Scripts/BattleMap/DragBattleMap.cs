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

        Vector3 pos = Vector3.zero;

        // Use this for initialization
        void Start()
        {
            isEnter = false;
            _recoverScale = transform.localScale.x;
        }

        // Update is called once per frame
        void Update()
        {
            if (isEnter)
            {
                StartCoroutine(OnMouseDown());
                ScaleBattleMap();
            }
            //A键还原
            if (Input.GetKey(KeyCode.A))
            {
                _scale = 1;
                transform.localScale = new Vector3(_recoverScale, _recoverScale, _recoverScale);
                transform.localPosition = new Vector3(0f, -3.6f, 0f);
            }
        }

        //拖动地图
        IEnumerator OnMouseDown()
        {
            //将物体由世界坐标系转换为屏幕坐标系
            Vector3 screenSpace = Camera.main.WorldToScreenPoint(transform.position);

            Vector3 offset = transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenSpace.z));
            while (Input.GetMouseButton(0))
            {
                //得到现在鼠标的2维坐标系位置
                Vector3 curScreenSpace = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenSpace.z);
                //将当前鼠标的2维位置转换成3维位置，再加上鼠标的移动量
                Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenSpace) + offset;
                //curPosition就是物体应该的移动向量赋给transform的position属性
                transform.position = curPosition;
                yield return new WaitForFixedUpdate(); //这个很重要，循环执行
            }
        }
        //缩放地图
        private void ScaleBattleMap()
        {
            if (Input.GetAxis("Mouse ScrollWheel") != 0)
            {
                _scale += Input.GetAxis("Mouse ScrollWheel");
                if (_scale >= 0)
                {
                    transform.localScale = new Vector3(_scale, _scale, _scale);
                }
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            isEnter = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isEnter = false;
        }
    }
}

