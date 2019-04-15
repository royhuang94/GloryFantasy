using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace NBearUnit
{
    //TODO 处理移动部分
        //1. 算法设计


   //TODO 解决方案
        //1. A*算法

   //以上只考虑 行动力情况，暂时不考虑攻击范围等问题



    public class UnitMove : MonoBehaviour, IPointerDownHandler
    {
        private IMessage.MsgReceiver targetReceiver;


        private void Awake()
        { 
            targetReceiver = GameObject.Find("ReceiverTest").GetComponent<MsgTestReceiver>();
            m_MyEvent.AddListener(() =>
            {
                IMessage.MsgDispatcher.SendMsg((int)MsgTestType.UnitMoving);
            });
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left && UnitManager.Instance.IsInstantiation)
                return;

            UnitManager.Instance.CurUnit = transform.GetComponentInParent<BattleMap.BattleMapBlock>().GetCoordinate();
            //Debug.Log("左键击下Unit" + UnitManager.Instance.CurUnit);
            //染色相关
            if (!UnitManager.Instance.IsClicked)
            {
                BattleMap.BattleMap.getInstance().IsColor = false;
                BattleMap.BattleMap.getInstance().HideBattleZooe(UnitManager.Instance.CurUnit);
                UnitManager.Instance.IsClicked = !UnitManager.Instance.IsClicked;
            }
            else
            {
                BattleMap.BattleMap.getInstance().IsColor = true;
                UnitManager.Instance.IsClicked = !UnitManager.Instance.IsClicked;
            }
            Gameplay.GetInstance().gamePlayInput.HandleConfirm(UnitManager.Instance.CurUnit);

            m_MyEvent.Invoke();
            UnitManager.Instance.canMoving = true;
            //UnitMoving();
        }


        UnityEvent m_MyEvent = new UnityEvent();
    }
}



