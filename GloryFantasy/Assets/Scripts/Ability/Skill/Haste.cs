using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using IMessage;
using GamePlay;

namespace Ability
{
    public class Haste : Ability
    {
        GameUnit.GameUnit _unit;
        public override void Init(string abilityId)
        {
            base.Init(abilityId);
            _unit = gameObject.GetComponent<GameUnit.GameUnit>();
            if (_unit == null)
                return;
            _unit.canNotAttack = false;
            _unit.canNotMove = false;
        }

        #region 用Trigger的写法
        //Trigger trigger;
        //
        //    public override void Init(string abilityId)
        //    {
        //        base.Init(abilityId);
        //        trigger = new THaste(GetComponent<GameUnit.GameUnit>().GetMsgReceiver());
        //        MsgDispatcher.RegisterMsg(trigger, abilityId);
        //    }
        //}

        //public class THaste : Trigger
        //{
        //    public THaste(MsgReceiver _speller)
        //    {
        //        register = _speller;
        //        msgName = (int)MessageType.GenerateUnit;
        //        condition = Condition;
        //        action = Action;
        //    }

        //    private bool Condition()
        //    {
        //        ////获取召唤列表
        //        //List<GameUnit.GameUnit> SummonUnits = this.GetSummonUnit();
        //        ////循环查询有没有召唤的怪是这个技能的发动者
        //        //for (int i = 0; i < SummonUnits.Count; i++)
        //        //{
        //        //    if (SummonUnits[i].GetMsgReceiver() == register)
        //        //        return true;
        //        //}
        //        //return false;
        //        GameUnit.GameUnit generatingUnit = this.GetGeneratingUnit();
        //        return generatingUnit.GetMsgReceiver() == register;
        //    }

        //    private void Action()
        //    {
        //        //获取发动这个技能的怪
        //        //List<GameUnit.GameUnit> SummonUnits = this.GetSummonUnit();
        //        //GameUnit.GameUnit unit = null;
        //        //for (int i = 0; i < SummonUnits.Count; i++)
        //        //{
        //        //    if (SummonUnits[i].GetMsgReceiver() == register)
        //        //        unit = SummonUnits[i];

        //        //    //让这只怪部署后可以移动
        //        //    unit.canNotMove = false;
        //        //    //让这只怪部署后可以攻击
        //        //    unit.canNotAttack = false;
        //        //}
        //        GameUnit.GameUnit unit = this.GetGeneratingUnit();
        //        unit.canNotAttack = false;
        //        unit.canNotMove = false;
        //    }
        //}
        #endregion
    }
}