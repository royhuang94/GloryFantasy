using GamePlay;
using IMessage;
using Mediator;
using System.Collections.Generic;

namespace Ability
{
    public class Crash : Ability
    {
        GameUnit.GameUnit _unit;
        public override void Init(string abilityId)
        {
            base.Init(abilityId);
            _unit = gameObject.GetComponent<GameUnit.GameUnit>();
            _unit.gameObject.AddBuff<BCrash>(AbilityVariable.Turns.Value);
        }
    }
    /// <summary>
    /// 此单位会于部署后{Turns}消解。
    /// </summary>
    public class BCrash : Buff.Buff
    {
        public override void InitialBuff()
        {
            //设定Buff的生命周期，两种写法,建议使用第二种，比较直观
            //this.Life = 2f;
            SetLife(2f);

            //Buff要做的事情，可以像Ability一样也写Trigger，也可以只是做一些数值操作。和Ability一样公用一套工具函数库
            
            //unit.MaxHP += 2;
            //unit.hp += 2;
        }

        //设定Buff消失时的逆操作
        public override void OnComplete()
        {
            GameUnit.GameUnit unit = GetComponent<GameUnit.GameUnit>();
            GameUnit.UnitManager.Kill(null, unit);
        }
    }
}