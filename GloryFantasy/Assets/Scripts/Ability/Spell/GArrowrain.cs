using BattleMap;
using GameCard;
using GamePlay;
using GameUnit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ability
{
    class GArrowrain_1 : Spell, GamePlay.GameplayTool
    {
        GameUnit.GameUnit carrier;
        BattleMapBlock block;
        public override void init(string spellID)
        {
            base.init(spellID);
            carrier = this.GetComponent<BaseCard>().carrier;
            variable.Range = 4;
            variable.Damage = 4;
            variable.Area = 3;
            targets = new List<EffectTarget>
            {
                new EffectTarget(TargetType.Block, false, judge_1)
            };
            selectionOver = _SelectionOver;
            action = _action;
        }

        public bool judge_1(object target)
        {
            BattleMapBlock block = (BattleMap.BattleMapBlock)target;
            return GamePlay.GameplayToolExtend.distanceBetween(block, carrier) <= variable.Range;
        }

        public void _SelectionOver()
        {
            block = (BattleMap.BattleMapBlock)this.GetSelecting()[0];
        }

        public void _action()
        {
            if (carrier == null || block == null)
                return;
            List<BattleMapBlock> affectedBlocks =
                GameplayToolExtend.getAreaByPos(variable.Area.Value, block.position);

            foreach (BattleMapBlock affectedBlock in affectedBlocks)
            {
                if (affectedBlock.unit == null || affectedBlock.unit.owner.Equals(OwnerEnum.Player))
                    continue;

                GameplayToolExtend.DealDamage(
                    this.GetAbilitySpeller(),
                    affectedBlock.unit,
                    new Damage(variable.Damage.Value)
                    );
            }
        }
    }
}
