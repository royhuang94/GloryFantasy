using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using IMessage;
using GamePlay;

namespace Ability
{
    public class Courage : Ability
    {
        Trigger trigger;

//        private void Awake()
//        {
//            //导入Courage异能的参数
//            InitialAbility("Courage");
//        }
//
//        private void Start()
//        {
//            //创建Trigger实例，传入技能的发动者
//            trigger = new TCourage(this.GetCardReceiver(this));
//            //注册Trigger进消息中心
//            MsgDispatcher.RegisterMsg(trigger, "Courage");
//        }

        public override void Init(string abilityId)
        {
            base.Init(abilityId);
            trigger = new TCourage(this.GetCardReceiver(this), AbilityVariable, abilityId);
            MsgDispatcher.RegisterMsg(trigger, abilityId);
        }
    }

    public class TCourage : Trigger
    {
        private int _turns;
        private string _abilityId;
        private AbilityVariable _abilityVariable;
        
        public TCourage(MsgReceiver speller, AbilityVariable abilityVariable, string abilityId)
        {
            _turns = abilityVariable.Turns.Value;
            _abilityId = abilityId;
            _abilityVariable = abilityVariable;
            register = speller;
            //初始化响应时点,为卡片使用时
            msgName = (int)MessageType.CastCard;
            //初始化条件函数和行为函数
            condition = Condition;
            action = Action;
        }

        private bool Condition()
        {
            //判断发动的卡是不是这个技能的注册者，并且这张卡是不是热血律动
            if (this.GetCastingCard().GetMsgReceiver() == register && this.GetCastingCard().ability_id.Contains(_abilityId))
                return true;
            else
                return false;
        }

        private void Action()
        {
            //获取被选中的友军，需要自己根据技能描述强转类型，一旦强转的类型是错的代码会出错
            GameUnit.GameUnit unit = (GameUnit.GameUnit)this.GetSelectingUnits()[0];
            //加buff
            
            unit.gameObject.AddBuff<BCourage>((float)_turns, _abilityVariable);             
            
        }
    }

    public class BCourage : Buff.Buff
    {
        private int _mov;
        private int _deltahp = 4;
        private int _deltaatk = 2;
        //设定Buff的初始化
        public override void InitialBuff()
        {
            _deltaatk = _buffVariable.Amount.Value;
            _deltahp = _buffVariable.Amount.Value + 2;
            GameUnit.GameUnit unit = GetComponent<GameUnit.GameUnit>();
            unit.changeATK(_deltaatk);
            unit.changeMHP(_deltahp);
            unit.hp += _deltahp;
            SetLife(2f);
        }

        public override void setVariable(AbilityVariable variable)
        {
            base.setVariable(variable);
        }

        protected override void OnDisappear()
        {
            GameUnit.GameUnit unit = GetComponent<GameUnit.GameUnit>();
            unit.changeATK(-_deltaatk);
            unit.changeMHP(-_deltahp);
            
        }
    }
}