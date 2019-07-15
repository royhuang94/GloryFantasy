using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ability
{
    class Haste : Ability
    {
        private GameUnit.GameUnit _unit;
        public override void Init(string abilityId)
        {
            base.Init(abilityId);
            _unit = this.GetComponent<GameUnit.GameUnit>();
            if (_unit == null)
                return;
            _unit.AT = 1;
            _unit.MT = 1;
        }
    }
}
