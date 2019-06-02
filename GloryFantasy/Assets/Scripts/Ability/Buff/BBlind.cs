using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Ability.Buff;

namespace Ability.Buff
{
    public class BBlind : Buff
    {
        private int _deltarng;
        private int _deltamov;
        //设定Buff的初始化
        public override void InitialBuff()
        {
            //设定Buff的生命周期，两种写法,建议使用第二种，比较直观
            //this.Life = 2f;
            SetLife(2f);

            //Buff要做的事情，可以像Ability一样也写Trigger，也可以只是做一些数值操作。和Ability一样公用一套工具函数库
            GameUnit.GameUnit unit = GetComponent<GameUnit.GameUnit>();

            _deltarng = unit.rng - 1;
            _deltamov = unit.mov - 1;

            unit.rng -= _deltarng;
            unit.mov -= _deltamov;
        }

        //设定Buff消失时的逆操作
        protected override void OnDisappear()
        {
            GameUnit.GameUnit unit = GetComponent<GameUnit.GameUnit>();
            unit.rng += _deltarng;
            unit.mov += _deltamov;
        }
    }
}