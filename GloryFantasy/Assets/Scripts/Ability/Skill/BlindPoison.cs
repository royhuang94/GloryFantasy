using IMessage;
using GamePlay;
using Ability.Debuff;

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
            GameUnit.GameUnit Injurer = this.GetInjurer();
            //查询伤害者是否为此生物
            if (Injurer.GetMsgReceiver() == register)
                    return true;
            return false;
        }

        private void Action()
        {
            //获取发动受伤者
            GameUnit.GameUnit InjuredUnit = this.GetInjuredUnit();
            InjuredUnit.gameObject.AddBuff<BBlind>(1f);
        }
    }
}
