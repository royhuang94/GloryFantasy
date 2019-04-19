using System.Collections;
using System.Collections.Generic;
using GameUnit;
using LitJson;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NBearUnit
{
    public class UnitSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
    {
        private GameObject _cardPrefab = null;
        private GameObject _cardInstance = null;
        private bool _canShowMsg = false;
        private bool _alreadyShowButton = false;
        private GameObject _gameObject = null;
        
        /// <summary>
        /// 用于鼠标移动将单位放回slot，存储Unit到slot下
        /// </summary>
        /// <param name="unit"></param>
        public void StoreItem(UnitUI unit)
        {
            _cardPrefab = unit.gameObject;

            //GameObject itemGameObject = Instantiate(_cardPrefab, transform, true) as GameObject;
            //itemGameObject.transform.localPosition = Vector3.zero;
            //BattleMap.BattleMap.getInstance().IsColor = false;

            //itemGameObject.GetComponent<UnitUI>().SetUnit();

            _cardInstance = Instantiate(_cardPrefab, transform, true) as GameObject;
            _cardInstance.transform.localPosition = Vector3.zero;
            BattleMap.BattleMap.getInstance().IsColor = false;

            _cardInstance.GetComponent<UnitUI>().SetUnit();

            Debug.Log("StoreItem");
            
        }

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
        /// <param name="notNotify">默认为false，请勿修改</param>
        public void RemoveItem(bool notNotify = false)
        {
            // 如果有Button存在，则销毁按钮
            Destroy(_gameObject);
            
            // 销毁卡牌实例
            Destroy(_cardInstance);
            _cardInstance = null;
            
            if(!notNotify)
                // 向CardManager发送通知
                CardManager.GetInstance().RemoveCard(_cardPrefab);
            
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
                /*
                GUIStyle style1= new GUIStyle();
                style1.fontSize = 30;
                style1.normal.textColor = Color.red;
                GUI.Label(
                    new Rect(Input.mousePosition.x, Screen.height - Input.mousePosition.y, 400, 50),
                    "Cube",
                    style1);*/
                UnitUI currentItemUI = gameObject.GetComponentInChildren<UnitUI>();
                if (currentItemUI == null)
                {
                    return;
                }


                Debug.Log(_cardInstance);

                BaseCard card = _cardInstance.GetComponent<BaseCard>();
                JsonData jsonData = CardManager.GetInstance().GetCardJsonData(card.id);
                int tagCount = jsonData["tag"].Count;
                string tagInToal = "";
                for (int i = 0; i < tagCount; i++)
                {
                    tagInToal += jsonData["tag"][i].ToString();
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
                GUILayout.TextField(jsonData["name"].ToString());
                GUILayout.TextField(jsonData["effect"].ToString());
                GUILayout.TextField(jsonData["cd"].ToString());
                GUILayout.TextField(tagInToal);
                GUILayout.TextField(jsonData["type"].ToString());
                
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
                GUILayout.EndArea();
            }
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
            //Debug.Log("鼠标");
            if (eventData.button != PointerEventData.InputButton.Left)
                return;
            //Debug.Log("鼠标左键");
            if (transform.childCount > 0)
            {
                // 若点击的卡牌类型为效果牌
                if (_cardInstance.GetComponent<BaseCard>() is OrderCard)
                {
                    
                    // 检测当前使用按钮的展示状态，若没有展示
                    if (!_alreadyShowButton)
                    {
                        OrderCard cardPreference = _cardInstance.GetComponent<OrderCard>();

                        // 实例化按钮预制件
                        _gameObject = Instantiate(cardPreference.buttonPrefab,
                            GameObject.Find("OrderCardCanvas").transform, true);

                        // 设定按钮位置
                        Button btn = _gameObject.GetComponentInChildren<Button>();
                        var position = gameObject.transform.position;
                        btn.transform.position = new Vector3(position.x,
                            position.y + 40, position.z);

                        // 动态添加按钮响应函数
                        btn.onClick.AddListener(delegate
                        {
                            // 调用效果牌中的使用函数
                            bool useSucceed = cardPreference.Use();

                            // 若成功使用
                            if (useSucceed)
                            {
                                // 从slot中移除当前卡牌
                                RemoveItem();
                            }

                            // 不论成功使用与否，都销毁按钮
                            Destroy(_gameObject);
                        });
                    }
                    else
                    {
                        // 若未点击使用按钮，则此动作未取消使用卡牌，销毁按钮
                        Destroy(_gameObject);
                        _gameObject = null;
                    }

                    _alreadyShowButton = !_alreadyShowButton;

                }
                else
                {
                    //TODO 自身不为空
                    //获取当前自身slot下的Unit
                    UnitUI currentItemUI = transform.GetChild(0).GetComponent<UnitUI>();
                    if (UnitManager.Instance.IsPickedUnit == false)
                    {
                        UnitManager.Instance.PickedUpUnit(currentItemUI); //调用此函数用于鼠标"捡起"当前slot下的unit
                        BattleMap.BattleMap.getInstance().IsColor = true;
                        //Destroy(currentItemUI.gameObject); //摧毁slot下已经被鼠标"捡起"的unit

                        RemoveItem();
                    }
                    else
                    {
                        //TODO 自身不为空， 当前slot下的unit.id != pickedUnit.id， pickedUnit与当前物品交换
                    }
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


