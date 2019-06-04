using GamePlay;
using IMessage;
using Mediator;
using System.Collections.Generic;
using UnityEngine;

namespace Ability
{
    
    /// <summary>
    /// 与此单位处于同一战区的友方单位获得+1攻击力和+3生命值。
    /// </summary>
    public class Shadow_Courage : Ability
    {
        
        private Trigger _trigger;
        private GameUnit.GameUnit _unit;
        
        public override void Init(string abilityId)
        {
            base.Init(abilityId);
            _trigger = new TShadow_Courage(
                this.GetUnitReceiver(this),
                GetComponent<GameUnit.GameUnit>(),
                abilityId);
            
            MsgDispatcher.RegisterMsg(_trigger, abilityId);
        }
    }

    public class TShadow_Courage : Trigger
    {
        private GameUnit.GameUnit _unit;
        private string _abilityId;

        public TShadow_Courage(MsgReceiver speller, GameUnit.GameUnit unit, string abilityId)
        {
            _unit = unit;
            _abilityId = abilityId;
            register = speller;
            msgName = (int) MessageType.Summon;
            condition = Condition;
            action = Action;
        }
        
        private bool Condition()
        {
            //获取召唤列表
            List<GameUnit.GameUnit> SummonUnits = this.GetSummonUnit();
            //循环查询有没有召唤的怪是这个技能的发动者
            for (int i = 0; i < SummonUnits.Count; i++)
            {
                if (SummonUnits[i].GetMsgReceiver() == register)
                    return true;
            }
            return false;
        }

        private void Action()
        {
            //获取发动这个技能的怪
            List<GameUnit.GameUnit> SummonUnits = this.GetSummonUnit();
            GameUnit.GameUnit unit = null;
            for (int i = 0; i < SummonUnits.Count; i++)
            {
                if (SummonUnits[i].GetMsgReceiver() == register)
                    unit = SummonUnits[i];
            }
            unit.gameObject.AddBuff<HShadow_Courage>(-1f);
        }
    }

    public class HShadow_Courage : Buff.Buff
    {
        //设定Buff的初始化
        private GameUnit.GameUnit _source;
        private BMBCollider halo;
        private Trigger _trigger;
        public override void InitialBuff()
        {
            SetLife(-1f);
            _source = GetComponent<GameUnit.GameUnit>();
            halo = new BMBCollider(_source, true);
            foreach(GameUnit.GameUnit unit in halo.disposeUnits)
            {
                if (judge(unit, _source))
                    unit.gameObject.AddBuff<BShadow_Courage>(-1f);
            }
            _trigger = new FHShadow_Courage(
                this.GetUnitReceiver(this),
                _source,
                judge,
                halo
                );

            MsgDispatcher.RegisterMsg(_trigger, "Fresh Halo Shadow_Courage");
        }

        private static bool judge(GameUnit.GameUnit unit, GameUnit.GameUnit _unit)
        {
            if (unit.owner == _unit.owner)
                return true;
            return false;
        }

        public class BShadow_Courage : Buff.Buff
        {
            public override void InitialBuff()
            {
                //设定Buff的生命周期，两种写法,建议使用第二种，比较直观
                SetLife(-1f);
                GetComponent<GameUnit.GameUnit>().changeATK(1);
                GetComponent<GameUnit.GameUnit>().changeMHP(3);
            }

            protected override void OnDisappear()
            {
                GetComponent<GameUnit.GameUnit>().changeATK(-1);
                GetComponent<GameUnit.GameUnit>().changeMHP(-3);
            }
        }

        //设定Buff消失时的逆操作
        protected override void OnDisappear()
        {
            foreach (GameUnit.GameUnit unit in halo.disposeUnits)
            {
                GameObject.Destroy(unit.gameObject.GetComponent<BShadow_Courage>());
            }
        }

        //刷新光环用的Trigger
        public class FHShadow_Courage : Trigger
        {
            private BMBCollider _halo;
            public delegate bool Judge(GameUnit.GameUnit unit, GameUnit.GameUnit _unit);
            private Judge _judge;
            private GameUnit.GameUnit _source;

            public FHShadow_Courage(MsgReceiver speller, GameUnit.GameUnit source, Judge judge, BMBCollider halo)
            {
                _halo = halo;
                register = speller;
                msgName = (int)MessageType.AfterColliderChange;
                condition = Condition;
                action = Action;
                _judge = judge;
                _source = source;
            }

            private bool Condition()
            {
                return true;
            }

            private void Action()
            {
                foreach (GameUnit.GameUnit unit in _halo.exitUnits)
                {
                    GameObject.Destroy(unit.gameObject.GetComponent<BShadow_Courage>());
                }
                foreach (GameUnit.GameUnit unit in _halo.enterUnits)
                {
                    if (_judge(unit, _source))
                        unit.gameObject.AddBuff<BShadow_Courage>(-1f);

                }
            }
        }
    }
    
}