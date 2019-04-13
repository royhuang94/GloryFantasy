﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace NBearUnit
{
    //TODO 处理移动部分
        //1. 算法设计


   //TODO 解决方案
        //1. 贪心算法

   //以上只考虑 行动力情况，暂时不考虑攻击范围等问题



    public class UnitMove : MonoBehaviour, IPointerDownHandler
    {
        private IMessage.MsgReceiver targetReceiver;
        private BattleMapManager.BattleMapBlock mapBlockParent;

        private void Awake()
        {
            mapBlockParent = GetComponentInParent<BattleMapManager.BattleMapBlock>();
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

            UnitManager.Instance.CurUnit = transform.GetComponentInParent<BattleMapManager.BattleMapBlock>().GetCoordinate();
            Debug.Log("左键击下Unit" + UnitManager.Instance.CurUnit);
            Gameplay.GetInstance().gamePlayInput.HandleConfirm(UnitManager.Instance.CurUnit);

            m_MyEvent.Invoke();
            //UnitMoving();

        }

        public void UnitMoving()
        {
            if(mapBlockParent != null)
            {
                Debug.Log("行动力展示");
                
                Gameplay.GetInstance().gamePlayInput.HandleConfirm(mapBlockParent.GetCoordinate());
            }

        }


        UnityEvent m_MyEvent = new UnityEvent();
    }
}


