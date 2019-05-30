using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using IMessage;
using GamePlay;

namespace Ability
{
    public class Blind : Ability
    {
        Trigger trigger;
        private int _rng;
        private int _mov;
        private GameUnit.GameUnit _unit;
        
        public override void Init(string abilityId)
        {
            base.Init(abilityId);
            _unit = GetComponent<GameUnit.GameUnit>();
            _rng = _unit.rng;
            _mov = _unit.mov;

            _unit.rng = 1;
            _unit.mov = 1;
        }

        //这个技能被删除时要做反向操作
        //准确来说，应该是trigger启动即召唤之后删除技能才需要反向操作
        //不过因为怪被summon之后才能被玩家看见，所以技能被删除时直接反向也没差
        private void OnDestroy()
        {
            _unit.rng = _rng;
            _unit.mov = _mov;
        }

    }

}
