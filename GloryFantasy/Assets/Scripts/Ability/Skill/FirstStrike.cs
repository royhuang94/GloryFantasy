using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using IMessage;
using GamePlay;

namespace Ability
{
    public class FirstStrike : Ability
    {
        GameUnit.GameUnit _unit;
        public override void Init(string abilityId)
        {
            base.Init(abilityId);
            _unit = gameObject.GetComponent<GameUnit.GameUnit>();
            _unit.priSPD += 2;
        }

    }
}
