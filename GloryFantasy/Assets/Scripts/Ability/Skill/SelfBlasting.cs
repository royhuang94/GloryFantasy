using System.Collections.Generic;
using Ability.Debuff;
using GamePlay;
using GameUnit;
using IMessage;
using Mediator;

namespace Ability
{
    /// <summary>
    /// 你的回合结束时，此单位烧灼其周围{Area}级范围，并对其上的单位造成{Damage}点伤害。
    /// </summary>
    public class SelfBlasting : Ability
    {
        private Trigger _trigger;
        
        public override void Init(string abilityId)
        {
            base.Init(abilityId);
            GameUnit.GameUnit unit = gameObject.GetComponent<GameUnit.GameUnit>();

            _trigger = new TSelfBlasting(
                unit,
                AbilityVariable.Area.Value,
                AbilityVariable.Damage.Value
            );
            MsgDispatcher.RegisterMsg(_trigger, abilityId);
        }
    }

    public class TSelfBlasting : Trigger
    {
        private int _area;
        private int _damage;
        private string _unitId;
        private GameUnit.GameUnit _unit;

        public TSelfBlasting(GameUnit.GameUnit unit, int area, int damage)
        {
            register = unit.GetMsgReceiver();
            _area = area;
            _damage = damage;
            _unit = unit;
            _unitId = unit.id;
            msgName = unit.owner == OwnerEnum.Player ? (int) MessageType.MPEnd : (int) MessageType.AIEnd;
            condition = Condition;
            action = Action;
        }

        /// <summary>
        /// 查询当前id是否还活着，如果活着就返回true
        /// </summary>
        /// <returns></returns>
        private bool Condition()
        {
            return !GameplayToolExtend.checkDeath(_unit);
        }

        private void Action()
        {
            List<BattleMap.BattleMapBlock> blocks = GamePlay.GameplayToolExtend.getAreaByPos(_area, _unit.CurPos);
            foreach(BattleMap.BattleMapBlock block in blocks)
            {
                int i = 0;
                while(block.units_on_me.Count > i)
                {
                    int count = block.units_on_me.Count;
                    GameUnit.GameUnit unit = block.units_on_me[i];
                    if (unit != _unit)
                        GameplayToolExtend.DealDamage(_unit, unit, new Damage(_damage));
                    if (count == block.units_on_me.Count)
                        i++;
                }
                block.gameObject.AddBuff<BFiring>(2f);
            }
            //List<GameUnit.GameUnit> units = AbilityMediator.Instance().GetGameUnitsWithinRange(_unit.CurPos, _area);
            //if (units == null)
            //    return;
            //
            //foreach (GameUnit.GameUnit unit in units)
            //{
            //    AbilityMediator.Instance().CaseDamageToEnemy(unit.id, _damage);
            //}

        }
    }
}