using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
//using UnityEngine.Events;
using UnityEngine.UI;

namespace NBearUnit
{
    public class CardUI : MonoBehaviour, IPointerDownHandler
    {
        #region UI Component
        private Image m_itemImage;
        private Image ItemImage
        {
            get
            {
                if (m_itemImage == null)
                {
                    m_itemImage = GetComponent<Image>();
                }
                return m_itemImage;
            }
        }
        #endregion


        /// <summary>
        /// 设置PickedItem 不可见
        /// </summary>
        public void Hide()
        {
            gameObject.SetActive(false);
        }
        /// <summary>
        /// 设置PickedItem 可见
        /// </summary>
        public void Show()
        {
            gameObject.SetActive(true);
        }

        /// <summary>
        /// 设置item的当前位置
        /// </summary>
        /// <param name="position">目标位置</param>
        public void SetLocalPosition(Vector3 position)
        {
            transform.localPosition = position;
        }

        public void AttachedAbility(List<string> abilities)
        {
            foreach (var ability in abilities)
            {
                //TODO 测试使用，之后需要整合到一个函数中
                switch (ability)
                {
                    case "Haste":
                        GFGame.UtilityHelper.Log("添加异能" + "Haste" + "成功", GFGame.LogColor.RED);
                        var temp = gameObject.AddComponent<Ability.Haste>();
                        break;
                    case "FirstStrike":
                        GFGame.UtilityHelper.Log("添加异能" + "FirstStrike" + "成功", GFGame.LogColor.RED);
                        //gameObject.AddComponent<Ability.FirstStrike>();
                        break;
                    case "QuickArk":
                        GFGame.UtilityHelper.Log("添加异能" + "QuickArk" + "成功", GFGame.LogColor.RED);
                        //gameObject.AddComponent<Ability.QuickArk>();
                        break;
                    case "Wit":
                        GFGame.UtilityHelper.Log("添加异能" + "Wit" + "成功", GFGame.LogColor.RED);
                        // gameObject.AddComponent<Ability.Wit>();
                        break;
                    case "LastStrike":
                        GFGame.UtilityHelper.Log("添加异能" + "LastStrike" + "成功", GFGame.LogColor.RED);
                        //gameObject.AddComponent<Ability.LastStrike>();
                        break;
                    case "Armor_1":
                        GFGame.UtilityHelper.Log("添加异能" + "Armor1" + "成功", GFGame.LogColor.RED);
                        //gameObject.AddComponent<Ability.Armor>();
                        break;
                    case "Fly":
                        GFGame.UtilityHelper.Log("添加异能" + "Fly" + "成功", GFGame.LogColor.RED);
                        //gameObject.AddComponent<Ability.Fly>();
                        break;
                    case "Regeneration":
                        //gameObject.AddComponent<Ability.Regeneration>();
                        break;
                    case "Heal":
                        GFGame.UtilityHelper.Log("添加异能" + "Heal" + "成功", GFGame.LogColor.RED);
                        //gameObject.AddComponent<Ability.Heal>();
                        break;
                    case "SelfBlasting":
                        GFGame.UtilityHelper.Log("添加异能" + "SelfBlasting" + "成功", GFGame.LogColor.RED);
                        //gameObject.AddComponent<Ability.SelfBlasting>();
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// 设置Unit
        /// </summary>
        public void SetUnit(string heroUnitName, GameUnit.UnitCard unitCard)
        {
            GetComponent<GameUnit.HeroUnit>().unitAttribute = Resources.Load<NBearUnit.UnitAttribute>("ScriptableObjects/BattleAttribuites/" + heroUnitName);

            //传递属性值
            var curUnitCar = gameObject.GetComponent<GameUnit.UnitCard>();
            curUnitCar.id = unitCard.id;

            //TODO 测试挂载异能ability unitAttribute
            UnitManager.Instance.abilitiesID = GetComponent<GameUnit.HeroUnit>().unitAttribute.ability;
            /*AttachedAbility(GetComponent<GameUnit.HeroUnit>().unitAttribute.ability);*/

            //TODO 获取当前Unit信息与image
            //当前为测试代码。。我这边根据resources函数处理的
            ItemImage.sprite = Resources.Load<Sprite>("test");

        }
        public void SetOrderCard()
        {
            //TODO 获取当前Unit信息与image
            //当前为测试代码。。我这边根据resources函数处理的
            ItemImage.sprite = Resources.Load<Sprite>("test");
        }
        public void SetUnit()
        {
            //TODO 获取当前Unit信息与image
            //当前为测试代码。。我这边根据resources函数处理的
            ItemImage.sprite = Resources.Load<Sprite>("test");
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            //TODO 显示单位属性


        }

        // 优化了Update多次激活事件导致的资源浪费，通过UnitMove脚本上的OnPointerDown实现一对一的事件检测
        #region 弃用部分
        //UnityEvent m_MyEvent = new UnityEvent();

        //private void Awake()
        //{
        //    m_MyEvent.AddListener(() =>
        //    {
        //        IMessage.MsgDispatcher.SendMsg((int)MsgTestType.UnitMove);
        //    });
        //}

        //private void Update()
        //{
        //    if(Input.GetMouseButtonDown(0) && UnitManager.Instance.IsInstantiation)
        //    {
        //        Debug.Log("左键击下Unit");
        //        m_MyEvent.Invoke();
        //    }
        //}
        //public void OnPointerDown(PointerEventData eventData)
        //{
        //    if (eventData.button != PointerEventData.InputButton.Left && UnitManager.Instance.IsInstantiation)
        //        return;
        //    Debug.Log("左键击下Unit");
        //    m_MyEvent.Invoke();

        //}
        #endregion


    }
}


