using GamePlay;
using IMessage;
using Mediator;

namespace Ability
{
    
    /// <summary>
    /// 此单位会于部署后{Turns}消解。
    /// </summary>
    public class Crash : Buff.Buff
    {
        private Trigger _trigger;

//        public override void Init(string abilityId)
//        {
//            base.Init(abilityId);
//            string id = GetComponent<GameUnit.GameUnit>().id;
//
//            //_trigger = new DelayedTrigger(
//            //    this.GetUnitReceiver(this),
//            //    AbilityVariable.Turns.Value,
//            //    (int)MessageType.MPEnd,
//            //    () =>
//            //    {
//            //        AbilityMediator.Instance().SendUnitToDeath(id);
//            //    }
//            //    );
//            //    MsgDispatcher.RegisterMsg(_trigger, abilityId + "--DT", true);
//            int turn = GameplayToolExtend.getTurnNum();
//            
//
//            
//        }
        
    }
}