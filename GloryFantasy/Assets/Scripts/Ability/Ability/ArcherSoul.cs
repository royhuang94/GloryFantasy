using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ability
{
    /// <summary>
    /// 弓手之魂：此单位获得“弓手”Tag，RNG变为3，SPD下降2。
    /// </summary>
    class ArcherSoul: Ability
    {
        private GameUnit.GameUnit _unit;
        public override void Init(string abilityId)
        {
            base.Init(abilityId);
            _unit = this.GetComponent<GameUnit.GameUnit>();
            if (_unit == null)
                return;
            _unit.tag.Add("弓手");
            _unit.setRNG(3);
            _unit.changeSPD(-2);
        }
    }
}
