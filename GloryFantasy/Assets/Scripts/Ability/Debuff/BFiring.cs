using BattleMap;
using GamePlay;
using IMessage;
using UnityEngine;

namespace Ability.Debuff
{
    /// <summary>
    /// 烧灼地形，会对其上的单位造成伤害，并且会清除掉粘滞地形。
    /// </summary>
    public class BFiring : Buff.Buff
    {
        private bool _isViscous;
        private bool _isOnUnit;
        private GameUnit.GameUnit _unit;
        private BattleMapBlock _battleMapBlock;
        private Trigger _trigger;
        private GlobalReceiver _globalReceiver;

        public override void InitialBuff()
        {
            base.InitialBuff();

            _isOnUnit = (_unit = GetComponent<GameUnit.GameUnit>()) != null;
            
            //如果被挂在了单位上
            if (_isOnUnit)
            {
                GameplayToolExtend.DealDamage(null, _unit, new Damage(1));
                // 弄完伤害后自我毁灭
                GameObject.Destroy(this);
            }
            else
            {
                _battleMapBlock = GetComponent<BattleMapBlock>();
                // 清除掉其上的粘滞地块。
                _isViscous = _battleMapBlock.gameObject.GetComponent<BViscous>() != null;
                if (_isViscous)
                {
                    GameObject.Destroy(_battleMapBlock.gameObject.GetComponent<BViscous>());
                }
                DebuffBattleMapBlock debuffBattleMapBlock = new DebuffBattleMapBlock();
                debuffBattleMapBlock.SetBattleMapBlockBurning(_battleMapBlock);
                // 对此时在此上的单位造成1点伤害。
                foreach (GameUnit.GameUnit unit in _battleMapBlock.units_on_me)
                {
                    // 通过向单位上挂载buff的方式去造成伤害
                    unit.gameObject.AddBuff<BFiring>(0.5f);
                }

                _globalReceiver = new GlobalReceiver();

                // 注册Trigger监听移动消息，如果有单位移动到当前地图块则造成伤害
                _trigger = new TFiring_1(_globalReceiver, _battleMapBlock);
                MsgDispatcher.RegisterMsg(_trigger, "BFiring On Block aftermove");

                // 注册Trigger监听回合开始信息，回合开始时对其上的单位造成伤害
                _trigger = new TFiring_2(_globalReceiver, _battleMapBlock);
                MsgDispatcher.RegisterMsg(_trigger, "BFiring On Block at BP");
            }
        }
        
        protected override void OnDisappear()
        {
            base.OnDisappear();

            if (!_isOnUnit)
            {
                if (_battleMapBlock != null)
                {
                    DebuffBattleMapBlock debuffBattleMapBlock = new DebuffBattleMapBlock();
                    debuffBattleMapBlock.SetBattleMapBlockNormal(_battleMapBlock);
                }
                // 删除receiver，使已注册的trigger被删除
                _globalReceiver = null;

            }

        }
    }

    public class TFiring_1 : Trigger
    {
        private BattleMapBlock _specificBlock;
        
        public TFiring_1(MsgReceiver speller, BattleMapBlock battleMapBlock)
        {
            _specificBlock = battleMapBlock;
            register = speller;
            msgName = (int) MessageType.Aftermove;
            condition = Condition;
            action = Action;
        }

        private bool Condition()
        {
            // 判断afterMove前指定的正在移动的单位是不是走到了当前的地图块上
            return Gameplay.Info.movingUnit.mapBlockBelow == _specificBlock;
        }

        private void Action()
        {
            // 给移动到当前地图块上的单位造成1点伤害。
            Gameplay.Info.movingUnit.gameObject.AddBuff<BFiring>(0.5f);
        }
    }

    public class TFiring_2 : Trigger
    {
        private BattleMapBlock _specificBlock;

        public TFiring_2(MsgReceiver speller, BattleMapBlock battleMapBlock)
        {
            _specificBlock = battleMapBlock;
            register = speller;
            msgName = (int)MessageType.BP;
            condition = Condition;
            action = Action;
        }

        private bool Condition()
        {
            // 没什么判断
            return true;
        }

        private void Action()
        {
            // 给移动到当前地图块上的单位造成1点伤害。
            foreach (GameUnit.GameUnit unit in _specificBlock.units_on_me)
            {
                unit.gameObject.AddBuff<BFiring>(0.5f);
                //GameplayToolExtend.DealDamage(null, unit, new Damage(1));
            }
        }
    }
}