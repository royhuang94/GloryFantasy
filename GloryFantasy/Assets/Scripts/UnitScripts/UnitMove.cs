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
        private List<Vector2> targetList;


        private void Awake()
        {
            targetList = UnitManager.Instance.TargetList;
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
            //行动染色
            if (!UnitManager.Instance.IsClicked)
            {
                targetList.Clear();
                targetList.Add(UnitManager.Instance.CurUnit);
                Gameplay.GetInstance().gamePlayInput.HandleMovConfirm(UnitManager.Instance.CurUnit);
                UnitManager.Instance.IsClicked = true;
                UnitManager.Instance.canMoving = true;
            }
            else
            {
                Vector2 target2 = UnitManager.Instance.CurUnit;
                if (targetList[0] != target2)
                {
                    if (targetList.Count >= 2)
                    {
                        if (targetList[1] != target2)
                        {
                            targetList.Remove(targetList[1]);
                            targetList.Add(target2);
                        }
                    }
                    else
                    {
                        targetList.Add(target2);
                    }
                    if (BattleMap.BattleMap.getInstance().CheckIfHasUnits(targetList[0]))
                    {
                        Gameplay.GetInstance().gamePlayInput.HandleMovCancel(targetList[0]);
                    }
                    Gameplay.GetInstance().gamePlayInput.HandleMovConfirm(targetList[1]);
                    Vector2 temVector = target2;
                    targetList[1] = targetList[0];
                    targetList[0] = temVector;
                    UnitManager.Instance.IsClicked = true;
                    UnitManager.Instance.canMoving = true;
                }
                else
                {
                    Gameplay.GetInstance().gamePlayInput.HandleMovCancel(UnitManager.Instance.CurUnit);
                    //BattleMap.BattleMap.getInstance().selectAction.SetActive(true);
                    BattleMap.BattleMap.getInstance().selectAction.transform.position = UnitManager.Instance.CurUnit;
                    UnitManager.Instance.IsClicked = false;
                    UnitManager.Instance.canMoving = false;
                    targetList.Clear();
                }
            }

            m_MyEvent.Invoke();
            //TODO 待解决Bug，canMoving应当在染色区关闭时设置为false
            GFGame.UtilityHelper.Log(UnitManager.Instance.canMoving, GFGame.LogColor.RED);
        }


        UnityEvent m_MyEvent = new UnityEvent();
    }
}



