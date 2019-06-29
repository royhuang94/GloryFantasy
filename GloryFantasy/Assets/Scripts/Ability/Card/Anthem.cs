using GamePlay;
using IMessage;
using Mediator;
using System.Collections.Generic;
using UnityEngine;

namespace Ability
{

    /// <summary>
    /// 使用者获得“4级范围以内的其他友方单位获得+{amount}攻击力和+1最大生命上限。”和移动力-1，持续{Turns}回合。
    /// </summary>
    public class Anthem : Ability
    {
        
        private Trigger _trigger;
        
        public override void Init(string abilityId)
        {
            base.Init(abilityId);
            _trigger = new TAnthem(
                this.GetCardReceiver(this),
                AbilityVariable.Turns.Value,
                abilityId);
            
            MsgDispatcher.RegisterMsg(_trigger, abilityId);
        }
    }

    public class TAnthem : Trigger
    {
        private string _abilityId;
        private int _turns;

        public TAnthem(MsgReceiver speller, int turns, string abilityId)
        {
            // 从GamePlayTool中获取使用者
            _turns = turns;
            _abilityId = abilityId;
            register = speller;
            msgName = (int) MessageType.CastCard;
            condition = Condition;
            action = Action;
        }
        
        private bool Condition()
        {
            if (this.GetCastingCard().GetMsgReceiver() == register && this.GetCastingCard().ability_id.Contains(_abilityId))
                return true;
            else
                return false;
        }

        private void Action()
        {
            this.GetAbilitySpeller().gameObject.AddBuff<HAnthem>((float)_turns);
        }
    }

    public class HAnthem : Buff.Buff
    {
        //设定Buff的初始化
        private GameUnit.GameUnit _source;
        private BMBCollider halo;
        private Trigger _trigger;
        public override void InitialBuff()
        {
            SetLife(-1f);
            _source = GetComponent<GameUnit.GameUnit>();
            _source.changeMOV(-1);
            halo = new BMBCollider(_source, GameplayToolExtend.Area[4]);
            foreach(GameUnit.GameUnit unit in halo.disposeUnits)
            {
                if (judge(unit, _source))
                    unit.gameObject.AddBuff<BAnthem>(-1f);
            }
            _trigger = new FHAnthem(
                this.GetUnitReceiver(this),
                _source,
                judge,
                halo
                );

            MsgDispatcher.RegisterMsg(_trigger, "Fresh Halo Anthem");
        }

        private static bool judge(GameUnit.GameUnit unit, GameUnit.GameUnit _unit)
        {
            if (unit.owner == _unit.owner)
                return true;
            return false;
        }

        public class BAnthem : Buff.Buff
        {
            public override void InitialBuff()
            {
                //设定Buff的生命周期，两种写法,建议使用第二种，比较直观
                SetLife(-1f);
                GetComponent<GameUnit.GameUnit>().changeATK(AbilityDatabase.GetInstance().GetAbilityVariable(this.GetSpellingAbility().AbilityID).Amount.Value);
                GetComponent<GameUnit.GameUnit>().changeMHP(1);
            }

            protected override void OnDisappear()
            {
                GetComponent<GameUnit.GameUnit>().changeATK(AbilityDatabase.GetInstance().GetAbilityVariable(this.GetSpellingAbility().AbilityID).Amount.Value);
                GetComponent<GameUnit.GameUnit>().changeMHP(-1);
            }
        }

        //设定Buff消失时的逆操作
        protected override void OnDisappear()
        {
            _source.changeMOV(1);
            foreach (GameUnit.GameUnit unit in halo.disposeUnits)
            {
                GameObject.Destroy(unit.gameObject.GetComponent<BAnthem>());
            }
        }

        //刷新光环用的Trigger
        public class FHAnthem : Trigger
        {
            private BMBCollider _halo;
            public delegate bool Judge(GameUnit.GameUnit unit, GameUnit.GameUnit _unit);
            private Judge _judge;
            private GameUnit.GameUnit _source;

            public FHAnthem(MsgReceiver speller, GameUnit.GameUnit source, Judge judge, BMBCollider halo)
            {
                _halo = halo;
                register = speller;
                msgName = (int)MessageType.ColliderChanged;
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
                    GameObject.Destroy(unit.gameObject.GetComponent<BAnthem>());
                }
                foreach (GameUnit.GameUnit unit in _halo.enterUnits)
                {
                    if (_judge(unit, _source))
                        unit.gameObject.AddBuff<BAnthem>(-1f);

                }
            }
        }
    }
    
}