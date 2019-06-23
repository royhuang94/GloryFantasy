using GamePlay;
using IMessage;
using GameUnit;
using Mediator;
using Ability.Buff;
using UnityEngine;

namespace Ability
{
    /// <summary>
    /// 使用者所在区域的友方单位获得+2攻击力直到回合结束。
    /// </summary>
    public class Battlecry : Ability
    {
        private Trigger _trigger;

        public override void Init(string abilityId)
        {
            base.Init(abilityId);
            _trigger = new TBattlecry(
                this.GetCardReceiver(this),
                GetComponent<GameUnit.GameUnit>().CurPos,
                abilityId);
            MsgDispatcher.RegisterMsg(_trigger, abilityId);
        }
    }

    public class TBattlecry : Trigger
    {
        private int _turns;
        private string _abilityId;
        private Vector2 _currentPos;
        
        public TBattlecry(MsgReceiver speller, Vector2 pos, string abilityId)
        {
            _abilityId = abilityId;
            _currentPos = pos;
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
            foreach (GameUnit.GameUnit unit in AbilityMediator.Instance().GetGameUnitsInBattleArea(_currentPos))
            {
                if (unit.owner == OwnerEnum.Player)
                {
                    unit.gameObject.AddBuff<BBattlecry>(0.5f);
                }
            }
            
        }
    }

    //要用的Buff
    public class BBattlecry : Buff.Buff
    {
        //设定Buff的初始化
        public override void InitialBuff()
        {
            //设定Buff的生命周期，两种写法,建议使用第二种，比较直观
            SetLife(2f);

            //Buff要做的事情，可以像Ability一样也写Trigger，也可以只是做一些数值操作。和Ability一样公用一套工具函数库
            GameUnit.GameUnit unit = GetComponent<GameUnit.GameUnit>();
            unit.changeATK(2);
        }

        //设定Buff 持续回合减少时的操作
        public override void OnSubtractBuffLife()
        {
            //无事可做
        }

        //设定Buff消失时的逆操作
        protected override void OnDisappear()
        {
            GetComponent<GameUnit.GameUnit>().changeATK(-2);
        }
    }
}