using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using GamePlay;

namespace GameUnit
{
    public class EnemyUnit : GameUnit, IPointerDownHandler
    {

        /// <summary>
        /// 鼠标点击敌人Unit
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerDown(PointerEventData eventData)
        {
            if (Gameplay.Instance().roundProcessController.IsPlayerRound())
            {
                if (Gameplay.Instance().gamePlayInput.InputFSM.selectedCard != null)
                    Debug.Log("该地格已有敌方单位，不可部署");
                //Gameplay.Instance().gamePlayInput.OnPointerDownEnemy(this, eventData);
                Gameplay.Instance().gamePlayInput.OnPointerDownEnemy(this, eventData);
            }
        }
    }
}


