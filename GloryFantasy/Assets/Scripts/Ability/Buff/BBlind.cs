using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Ability.Buff;

namespace Ability.Buff
{
    public class BBlind : Buff
    {
        private int _rng;
        private int _mov;
        //设定Buff的初始化
        protected override void InitialBuff()
        {
            //设定Buff的生命周期，两种写法,建议使用第二种，比较直观
            //this.Life = 2f;
            SetLife(2f);

            //Buff要做的事情，可以像Ability一样也写Trigger，也可以只是做一些数值操作。和Ability一样公用一套工具函数库
            GameUnit.GameUnit unit = GetComponent<GameUnit.GameUnit>();
            
            _rng = unit.rng;
            _mov = unit.mov;

            unit.rng = 1;
            unit.mov = 1;
        }

        //设定Buff消失时的逆操作
        protected override void OnDisappear()
        {
            GameUnit.GameUnit unit = GetComponent<GameUnit.GameUnit>();
            unit.rng = _rng;
            unit.mov = _mov;
        }
    }
}