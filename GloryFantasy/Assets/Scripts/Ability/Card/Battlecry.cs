using GamePlay;
using IMessage;

namespace Ability
{
    public class Battlecry : Ability
    {
        private Trigger _trigger;
        private GameUnit.GameUnit _unit;

        public override void Init(string abilityId)
        {
            base.Init(abilityId);
            
            _unit = GetComponent<GameUnit.GameUnit>();
            _trigger = new DelayedTrigger(
                this.GetCardReceiver(this),
                0,
                (int)MessageType.MPEnd,
                () => { _unit.atk -= 2; }
                );
            MsgDispatcher.RegisterMsg(_trigger, abilityId + "--DT", true);
            _unit.atk += 2;
            
        }
        
        
    }
}