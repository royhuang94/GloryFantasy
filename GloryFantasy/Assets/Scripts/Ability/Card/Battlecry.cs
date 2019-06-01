using GamePlay;
using IMessage;


using Ability.Buff;
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
            gameObject.AddComponent<BattlecryBuff>();
            //_trigger = new DelayedTrigger(
            //    this.GetCardReceiver(this),
            //    0,
            //    (int)MessageType.MPEnd,
            //    () => { _unit.atk -= 2; }
            //    );
            //MsgDispatcher.RegisterMsg(_trigger, abilityId + "--DT", true);
            //_unit.atk += 2;
        }
        
        
    }

    //要用的Buff
    public class BattlecryBuff : Buff.Buff
    {
        //设定Buff的初始化
        protected override void InitialBuff()
        {
            //设定Buff的生命周期，两种写法,建议使用第二种，比较直观
            SetLife(2f);

            //Buff要做的事情，可以像Ability一样也写Trigger，也可以只是做一些数值操作。和Ability一样公用一套工具函数库
            GameUnit.GameUnit unit = GetComponent<GameUnit.GameUnit>();
            unit.atk += 2;
        }

        //设定Buff CD减少时的操作
        public override void OnSubtractBuffLife()
        {
            //无事可做
        }

        //设定Buff消失时的逆操作
        protected override void OnDisappear()
        {
            GetComponent<GameUnit.GameUnit>().atk -= 2;
        }
    }
}