using System.Collections;
using System.Collections.Generic;
using GameUnit;
using LitJson;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using GameCard;

namespace GameGUI
{
    /// <summary>
    /// 手牌槽
    /// </summary>
    public class CardsSetsSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private GameObject _cardPrefab = null;   
        private GameObject _cardInstance = null; //卡牌的实例
        private bool _canShowMsg = false;

        /// <summary>
        /// 用于向UnitSlot中放入卡牌
        /// </summary>
        /// <param name="cardPrefab">要实例化的卡牌</param>
        public void InsertItem(GameObject cardPrefab)
        {
            _cardInstance = Instantiate(cardPrefab, gameObject.transform, true);
            _cardInstance.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
            this._cardPrefab = cardPrefab;
        }

        /// <summary>
        /// 移除当前slot中的卡牌,并通知CardManager手牌栏位发生变化
        /// </summary>
        public void RemoveItem()
        {
            // 销毁卡牌实例
            Destroy(_cardInstance);
            _cardInstance = null;
            
            
            //_cardPrefab = null;
        }

        /// <summary>
        /// 确认当前栏位是否为空，空是指slot内是否有卡牌存在
        /// </summary>
        /// <returns>若为空，则返回true</returns>
        public bool IsEmpty()
        {
            // 若保存的预制件引用为空，则意味着本栏位已空
            return _cardInstance == null;
        }

        /// <summary>
        /// 当鼠标移入slot槽时
        /// 如果当前slot槽中包含Unit(单位)，及显示提示面板并修改其内容
        /// </summary>
        public void OnPointerEnter(PointerEventData eventData)
        {
            //TODO 显示Unit属性
            //Debug.Log("鼠标进入");
            _canShowMsg = true;
        }

        /// <summary>
        /// 当鼠标移除slot槽时
        /// 如果当前slot槽中包含Unit(单位)时，直接隐藏提示面板
        /// </summary>
        public void OnPointerExit(PointerEventData eventData)
        {
            //Debug.Log("鼠标退出");
            _canShowMsg = false;
        }

        private void OnGUI()
        {
            if (_canShowMsg)
            {
                CardUI currentItemUI = gameObject.GetComponentInChildren<CardUI>();
                if (currentItemUI == null)
                {
                    return;
                }

                BaseCard card = _cardInstance.GetComponent<BaseCard>();

                string tagInToal = "";
                if (card.tag.Count != 0)
                {
                    for (int i = 0; i < card.tag.Count; i++)
                    {
                        tagInToal += card.tag[i];
                    }
                }       
                //GUILayout.BeginArea(new Rect(Input.mousePosition.x, Screen.height - Input.mousePosition.y, 300, 350));
                GUILayout.BeginArea(new Rect(0, 0, 300, 500));
                GUILayout.BeginHorizontal("Box");
                GUILayout.BeginVertical(GUILayout.Width(40));
                GUILayout.Label("name:");
                GUILayout.Label("effect:");
                GUILayout.Label("cd:");
                GUILayout.Label("tag:");
                GUILayout.Label("type:");
                GUILayout.EndVertical();
                
                GUILayout.BeginVertical("Box", GUILayout.Width(500));
                GUILayout.TextField(card.name);
                GUILayout.TextField(card.effect);
                GUILayout.TextField(card.cd.ToString());
                GUILayout.TextField(tagInToal);
                GUILayout.TextField(card.type);
                
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
                GUILayout.EndArea();
            }
        }

    }
}


