﻿using System.Collections;
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
            Gameplay.Instance().gamePlayInput.OnPointerDown(this, eventData);
        }
    }
}

