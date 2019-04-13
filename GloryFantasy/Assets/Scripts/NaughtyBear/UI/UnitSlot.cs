using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace NBearUnit
{
    public class UnitSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
    {
        public GameObject unitPrefab;
        /// <summary>
        /// 存储Unit到slot下
        /// </summary>
        /// <param name="unit"></param>
        public void StoreItem(UnitUI unit)
        {
            GameObject itemGameObject = Instantiate(unitPrefab) as GameObject;
            itemGameObject.transform.SetParent(transform);
            itemGameObject.transform.localPosition = new Vector3(0.0f, -21.0f, 0.0f);
            
            itemGameObject.GetComponent<UnitUI>().SetUnit(/*unit*/); //设置Item
            Debug.Log("StoreItem");
        }




        /// <summary>
        /// 当鼠标移入slot槽时
        /// 如果当前slot槽中包含Unit(单位)，及显示提示面板并修改其内容
        /// </summary>
        public void OnPointerEnter(PointerEventData eventData)
        {
            //TODO 显示Unit属性
            Debug.Log("鼠标进入");
        }

        /// <summary>
        /// 当鼠标移除slot槽时
        /// 如果当前slot槽中包含Unit(单位)时，直接隐藏提示面板
        /// </summary>
        public void OnPointerExit(PointerEventData eventData)
        {
            Debug.Log("鼠标退出");
        }



        /// <summary>
        /// 自身slot为空
        /// 1. isPickedUnit != false       直接放在这个空的slot槽下
        /// 2. isPickedUnit == false     不做任何处理
        /// 
        /// 自身slot不为空
        ///  1. isPickedUnit != false
        ///       ①  当前slot下的unit.id == pickedUnit.id，不做任何处理
        ///       ②  当前slot下的unit.id != pickedUnit.id， pickedUnit与当前物品交换
        /// 2. isPickedUnit == false 把当前物品槽下的Unit放到鼠标下
        /// </summary>
        public void OnPointerDown(PointerEventData eventData)
        {
            Debug.Log("鼠标");
            if (eventData.button != PointerEventData.InputButton.Left)
                return;
            Debug.Log("鼠标左键");
            if (transform.childCount > 0)
            {
                //TODO 自身不为空
                //获取当前自身slot下的Unit
                UnitUI currentItemUI = transform.GetChild(0).GetComponent<UnitUI>();
                if (UnitManager.Instance.IsPickedUnit == false)
                {
                    UnitManager.Instance.PickedUpUnit(currentItemUI); //调用此函数用于鼠标"捡起"当前slot下的unit
                    Destroy(currentItemUI.gameObject); //摧毁slot下已经被鼠标"捡起"的unit
                }
                else
                {
                    //TODO 自身不为空， 当前slot下的unit.id != pickedUnit.id， pickedUnit与当前物品交换
                }

            }
            else
            {
                //TODO 自身为空
                if(UnitManager.Instance.IsPickedUnit == true)
                {
                    StoreItem(UnitManager.Instance.PickedUnit);
                    UnitManager.Instance.RemoveAllItem();
                }

            }
        }

    }
}


