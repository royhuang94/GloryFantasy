using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

using GamePlay;
using GamePlay.Input;

namespace GameUnit
{
    public class FriendlyUnit : GameUnit, IPointerDownHandler
    {

        public void OnPointerDown(PointerEventData eventData)
        {
            if (Gameplay.Instance().gamePlayInput.InputFSM.selectedCard != null)
                Debug.Log("该地格已有友方单位，不可部署");
            //Gameplay.Instance().gamePlayInput.OnPointerDownFriendly(this, eventData);
            Gameplay.Instance().gamePlayInput.OnPointerDownFriendly(this, eventData);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            Gameplay.Instance().gamePlayInput.OnPointerEnter(this.mapBlockBelow, eventData);
        }

    }
}


