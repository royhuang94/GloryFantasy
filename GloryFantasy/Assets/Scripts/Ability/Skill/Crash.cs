using GamePlay;
using IMessage;
using Mediator;
using System.Collections.Generic;

namespace Ability
{
    public class Crash : Ability
    {
        Trigger trigger;
        public override void Init(string abilityId)
        {
            base.Init(abilityId);
            trigger = new TCrash(GetComponent<GameUnit.GameUnit>().GetMsgReceiver());
            MsgDispatcher.RegisterMsg(trigger, abilityId);
        }
    }

    public class TCrash : Trigger
    {
        public TCrash(MsgReceiver _speller)
        {
            register = _speller;
            msgName = (int)MessageType.GenerateUnit;
            condition = Condition;
            action = Action;
        }

        private bool Condition()
        {
            ////获取召唤列表
            //List<GameUnit.GameUnit> SummonUnits = this.GetSummonUnit();
            ////循环查询有没有召唤的怪是这个技能的发动者
            //for (int i = 0; i < SummonUnits.Count; i++)
            //{
            //    if (SummonUnits[i].GetMsgReceiver() == register)
            //        return true;
            //}
            //return false;
            GameUnit.GameUnit generatingUnit = this.GetGeneratingUnit();
            return generatingUnit.GetMsgReceiver() == register;
        }

        private void Action()
        {
            //获取发动这个技能的怪
            //List<GameUnit.GameUnit> SummonUnits = this.GetSummonUnit();
            //GameUnit.GameUnit unit = null;
            //for (int i = 0; i < SummonUnits.Count; i++)
            //{
            //    if (SummonUnits[i].GetMsgReceiver() == register)
            //        unit = SummonUnits[i];
            //}
            GameUnit.GameUnit unit = this.GetGeneratingUnit();
            unit.gameObject.AddBuff<BCrash>(2f);
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