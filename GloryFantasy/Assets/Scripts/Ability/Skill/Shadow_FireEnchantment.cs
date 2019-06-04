using System.Collections.Generic;
using Ability.Debuff;
using BattleMap;
using GamePlay;
using IMessage;

namespace Ability
{
    /// <summary>
    /// 攻击时会烧灼周围{Area}级区域的格子，一回合后消解，并对其上的单位造成{Damage}的伤害。
    /// </summary>
    public class Shadow_FireEnchantment : Ability
    {
        private Trigger _trigger;
        public override void Init(string abilityId)
        {
            base.Init(abilityId);
            _trigger = new TShadow_FireEnchantment(
                this.GetUnitReceiver(this),
                AbilityVariable.Damage.Value,
                AbilityVariable.Area.Value,
                GetComponent<GameUnit.GameUnit>());
            MsgDispatcher.RegisterMsg(_trigger, abilityId);
        }
    }

    public class TShadow_FireEnchantment : Trigger
    {
        private int _damage;
        private int _area;
        private GameUnit.GameUnit _unit;
        
        public TShadow_FireEnchantment(MsgReceiver owner, int damage, int area, GameUnit.GameUnit unit)
        {
            _damage = damage;
            _area = area;
            _unit = unit;
            register = owner;
            msgName = (int) MessageType.Damage;
            condition = Condition;
            action = Action;
        }

        private bool Condition()
        {
            // 检测单位是否还活着了
            return _unit.gameObject.activeSelf;
        }

        private void Action()
        {
            List<BattleMapBlock> blocks = GameplayToolExtend.getAreaByPos(_area, _unit.CurPos);

            foreach (BattleMapBlock block in blocks)
            {
                block.gameObject.AddBuff<BFiring>(1f);
                foreach (GameUnit.GameUnit unit in block.units_on_me)
                {
                    GameplayToolExtend.DealDamage(_unit, unit, new Damage(_damage));
                }
            }
        }
    }
}