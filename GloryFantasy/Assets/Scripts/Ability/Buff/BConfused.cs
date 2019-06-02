using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Ability.Buff;

namespace Ability.Buff
{
    public class BConfused : Buff
    {
        //设定Buff的初始化
        public override void InitialBuff()
        {
            //设定Buff的生命周期，两种写法,建议使用第二种，比较直观
            //this.Life = 2f;
            SetLife(2f);

            //Buff要做的事情，可以像Ability一样也写Trigger，也可以只是做一些数值操作。和Ability一样公用一套工具函数库
            GameUnit.GameUnit unit = GetComponent<GameUnit.GameUnit>();
            unit.priSPD -= 2;
        }

        //设定Buff消失时的逆操作
        protected override void OnDisappear()
        {
            GetComponent<GameUnit.GameUnit>().priSPD += 2;
        }
    }
}