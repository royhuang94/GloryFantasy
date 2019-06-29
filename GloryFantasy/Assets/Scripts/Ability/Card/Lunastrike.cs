using System.Linq;
using GameCard;
using GamePlay;
using IMessage;

namespace Ability
{
    public class Lunastrike : Ability
    {
        private Trigger _trigger;
        /// <summary>
        /// 要求携带者：红色或蓝色单位。携带者MOV-1。使用者：携带者。使用者对目标格子上的单位造成5点伤害。
        /// </summary>
        /// <param name="abilityId"></param>
        public override void Init(string abilityId)
        {
            base.Init(abilityId);
            _trigger = new TLunastrike(this.GetCardReceiver(this), AbilityVariable.Damage.Value, abilityId);
            MsgDispatcher.RegisterMsg(_trigger, abilityId);
//            MyTargetConstraintList[0] = Range_0;
        }

//        public bool Range_0(object target)
//        {
//            // 如果目标类型不是GameUnit，直接返回false，为了防止后续强转出错
//            if (!target.GetType().ToString().Equals("GameUnit.FriendlyUnit"))
//            {
//                return false;
//            }
//
//            GameUnit.GameUnit _unit = (target as GameUnit.GameUnit);
//            // TODO: GameUnit.GameUnit未考虑多重颜色的问题，需要改成list，以下代码也需做对应修改
//            return _unit.Color.Equals("R") || _unit.Color.Equals("U");
//        }
    }


    public class TLunastrike : Trigger
    {
        private string _abilityId;
        private int _damage;

        public TLunastrike(MsgReceiver speller, int damage, string abilityId)
        {
            _abilityId = abilityId;
            _damage = damage;
            
            register = speller;
            msgName = (int) MessageType.CastCard;
            condition = Condition;
            action = Action;
        }

        private bool Condition()
        {
            return this.GetCastingCard().GetMsgReceiver() == register &&
                   this.GetCastingCard().ability_id.Contains(_abilityId);
        }

        private void Action()
        {
            // TODO: 消耗使用者动作次数

//            // 如果选择了空地格，当作已经使用
//            if (!BattleMap.BattleMap.Instance()
//                .CheckIfHasUnits(Gameplay.Instance().gamePlayInput.InputFSM.TargetList[1]))
//                return;
            
            // 造成伤害
            GameplayToolExtend.DealDamage(
                this.GetAbilitySpeller(),
                BattleMap.BattleMap.Instance().GetUnitsOnMapBlock(
                    Gameplay.Instance().gamePlayInput.InputFSM.TargetList[1]
                ),
                new Damage(_damage)
            );
        }
    }
}