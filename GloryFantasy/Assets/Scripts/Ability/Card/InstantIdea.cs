using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameCard;
using UnityEngine;

using IMessage;
using GamePlay;

namespace Ability
{
    /// <summary>
    /// 使用者：携带有战技的友方单位。创建使用者携带的 1/1/2 张战技的临时复制置入你的手中（临时复制使用后销毁）
    /// </summary>
    public class InstantIdea : Ability
    {
        Trigger trigger;

//        private void Awake()
//        {
//            //导入InstantIdea异能的参数
//            InitialAbility("InstantIdea");
//        }
//
//        private void Start()
//        {
//            //创建Trigger实例，传入技能的发动者
//            trigger = new TInstantIdea(this.GetCardReceiver(this));
//            //注册Trigger进消息中心
//            MsgDispatcher.RegisterMsg(trigger, "InstantIdea");
//        }

        public override void Init(string abilityId)
        {
            base.Init(abilityId);
            trigger = new TInstantIdea(this.GetCardReceiver(this), abilityId);
            MsgDispatcher.RegisterMsg(trigger, abilityId);
        }
    }

    public class TInstantIdea : Trigger
    {
        private GameUnit.GameUnit unit;
        private string _abilityId;
        public TInstantIdea(MsgReceiver speller, string abilityId)
        {
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
            //判断发动的卡是不是这个技能的注册者，并且这张卡是不是瞬发幻想
            if (this.GetCastingCard().GetMsgReceiver() == register && this.GetCastingCard().ability_id.Contains(_abilityId))
            {
                    return true;
            }
            
            return false;
        }

        private void Action()
        {
            unit = (GameUnit.GameUnit)this.GetSelectingUnits()[0];
            unit.gameObject.AddBuff<Btemp>(2f);

        }
    }

    public class Btemp : Buff.Buff
    {
        int deltaATK = 2;
        //设定Buff的初始化
        public override void InitialBuff()
        {
            //设定Buff的生命周期，两种写法,建议使用第二种，比较直观
            //this.Life = 2f;
            SetLife(2f);

            deltaATK = _buffVariable.Amount.Value;
            //Buff要做的事情，可以像Ability一样也写Trigger，也可以只是做一些数值操作。和Ability一样公用一套工具函数库
            GameUnit.GameUnit unit = GetComponent<GameUnit.GameUnit>();

            unit.changeATK( deltaATK );
            unit.priSPD += 2;
        }

        //设定Buff消失时的逆操作
        protected override void OnDisappear()
        {
            GameUnit.GameUnit unit = GetComponent<GameUnit.GameUnit>();
            unit.changeATK( - deltaATK );
            unit.priSPD -= 2;
        }
    }
}