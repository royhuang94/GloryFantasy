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
        private BattleMapBlock _battleMapBlock;
        private Trigger _trigger;
        private GlobalReceiver _globalReceiver;

        public override void InitialBuff()
        {
            base.InitialBuff();

            // 清除掉其上的粘滞地块。
            _isViscous = _battleMapBlock.gameObject.GetComponent<BViscous>() != null;
            if (_isViscous)
            {
                GameObject.Destroy(_battleMapBlock.gameObject.GetComponent<BViscous>());
            }
            // 对此时在此上的单位造成1点伤害。
            foreach (GameUnit.GameUnit unit in _battleMapBlock.units_on_me)
            {
                GameplayToolExtend.DealDamage(null, unit, new Damage(1+(_isViscous?1:0)));
            }

            _globalReceiver = new GlobalReceiver();

            // 注册Trigger监听移动消息，如果有单位移动到当前地图块则造成伤害
            _trigger = new TFiring_1(_globalReceiver, _battleMapBlock);
            MsgDispatcher.RegisterMsg(_trigger, "BFiring On Block aftermove");

            // 注册Trigger监听回合开始信息，回合开始时对其上的单位造成伤害
            _trigger = new TFiring_2(_globalReceiver, _battleMapBlock);
            MsgDispatcher.RegisterMsg(_trigger, "BFiring On Block at BP");
        }

        protected override void OnDisappear()
        {
            base.OnDisappear();

            // 移除当前地图块上仍存在单位的debuff脚本
            foreach (GameUnit.GameUnit unit in _battleMapBlock.units_on_me)
            {
                Destroy(unit.GetComponent<BFiring>());
            }

            // 删除receiver，使已注册的trigger被删除
            _globalReceiver = null;

            // 结束执行
            return;
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
            GameplayToolExtend.DealDamage(null, Gameplay.Info.movingUnit, new Damage(1));
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
                GameplayToolExtend.DealDamage(null, unit, new Damage(1));
            }
        }
    }
}