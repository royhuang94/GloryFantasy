using IMessage;
using GamePlay;
using Ability.Debuff;
using System.Collections.Generic;

namespace Ability
{
    /// <summary>
    /// 受到此单位伤害的生物获得目盲，一回合后消解。
    /// </summary>
    public class BlindPoison : Ability
    {
        Trigger trigger;

        public override void Init(string abilityId)
        {
            base.Init(abilityId);
            trigger = new TBlindPoison(GetComponent<GameUnit.GameUnit>().GetMsgReceiver());
            MsgDispatcher.RegisterMsg(trigger, abilityId);

        }

    }

    public class TBlindPoison : Trigger
    {
        public TBlindPoison(MsgReceiver _speller)
        {
            register = _speller;
            //造成伤害时触发
            msgName = (int)MessageType.Damage;
            condition = Condition;
            action = Action;
        }

        private bool Condition()
        {
            //获取召唤列表
            List<GameUnit.GameUnit> Injurers = this.GetInjurer();
            //查询伤害者是否为此生物
            foreach(GameUnit.GameUnit Injurer in Injurers)
                if (Injurer.GetMsgReceiver() == register)
                       return true;
            return false;
        }

        private void Action()
        {
            List<GameUnit.GameUnit> Injurers = this.GetInjurer();
            //找到伤害者为此生物的受伤生物
            for (int i = 0; i < Injurers.Count; i++){
                GameUnit.GameUnit Injurer = Injurers[i];
                if (Injurer.GetMsgReceiver() == register)
                {
                    GameUnit.GameUnit InjuredUnit = this.GetInjuredUnit()[i];
                    InjuredUnit.gameObject.AddBuff<BBlind>(1f);
                }
            }
        }
    }
}
