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
            trigger = new TCourage(this.GetCardReceiver(this), AbilityVariable.Turns.Value, abilityId);
            MsgDispatcher.RegisterMsg(trigger, abilityId);
        }
    }

    public class TCourage : Trigger
    {
        private int _turns;
        private string _abilityId;
        
        public TCourage(MsgReceiver speller, int turns, string abilityId)
        {
            _turns = turns;
            _abilityId = abilityId;
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
            switch (_abilityId)
            {
                case "Courage_1":
                    unit.gameObject.AddBuff<BCourage_1>((float)_turns);
                    break;
                case "Courage_2":
                    unit.gameObject.AddBuff<BCourage_2>((float)_turns);
                    break;
                case "Courage_3":
                    unit.gameObject.AddBuff<BCourage_3>((float)_turns);
                    break;
            }
        }
    }

    public class BCourage_1 : Buff.Buff
    {
        private int _mov;
        //设定Buff的初始化
        protected override void InitialBuff()
        {
            SetLife(2f);

            GameUnit.GameUnit unit = GetComponent<GameUnit.GameUnit>();
            unit.atk += 2;
            unit.MaxHP += 4;
            unit.hp += 4;
        }


        protected override void OnDisappear()
        {
            GameUnit.GameUnit unit = GetComponent<GameUnit.GameUnit>();
            unit.atk -= 2;
            unit.MaxHP -= 4;
            if (unit.hp > unit.MaxHP)
                unit.hp = unit.MaxHP;
        }
    }

    public class BCourage_2 : Buff.Buff
    {
        private int _mov;
        protected override void InitialBuff()
        {
            SetLife(2f);

            GameUnit.GameUnit unit = GetComponent<GameUnit.GameUnit>();
            unit.atk += 3;
            unit.MaxHP += 5;
            unit.hp += 5;
        }


        protected override void OnDisappear()
        {
            GameUnit.GameUnit unit = GetComponent<GameUnit.GameUnit>();
            unit.atk -= 3;
            unit.MaxHP -= 5;
            if (unit.hp > unit.MaxHP)
                unit.hp = unit.MaxHP;
        }
    }

    public class BCourage_3 : Buff.Buff
    {
        private int _mov;
        protected override void InitialBuff()
        {
            SetLife(2f);

            GameUnit.GameUnit unit = GetComponent<GameUnit.GameUnit>();
            unit.atk += 4;
            unit.MaxHP += 6;
            unit.hp += 6;
        }


        protected override void OnDisappear()
        {
            GameUnit.GameUnit unit = GetComponent<GameUnit.GameUnit>();
            unit.atk -= 4;
            unit.MaxHP -= 6;
            if (unit.hp > unit.MaxHP)
                unit.hp = unit.MaxHP;
        }
    }
}