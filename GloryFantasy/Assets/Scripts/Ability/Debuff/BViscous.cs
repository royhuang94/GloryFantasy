using BattleMap;
using GamePlay;
using IMessage;
using UnityEngine;

namespace Ability.Debuff
{
    public class BViscous : Buff.Buff
    {
        private bool _isOnBlock;
        private BattleMapBlock _battleMapBlock;
        private Trigger _trigger;
        private GlobalReceiver _globalReceiver;

        public override void InitialBuff()
        {
            base.InitialBuff();
            
            //判断是否被挂在了地图块
            _isOnBlock = (_battleMapBlock = GetComponent<BattleMapBlock>()) != null;

            // 如果被挂在了地图块上
            if (_isOnBlock)
            {
                // 先将处理已经存在本地图块上的单位
                foreach (GameUnit.GameUnit unit in _battleMapBlock.units_on_me)
                {
                    // 挂载本脚本
                    unit.gameObject.AddBuff<BViscous>(-1);
                }
                
                _globalReceiver = new GlobalReceiver();

                // 注册Trigger监听移动消息，如果有单位移动到当前地图块则处理之
                _trigger = new TViscousTrigger(_globalReceiver, _battleMapBlock);
                MsgDispatcher.RegisterMsg(_trigger, "BViscous On Block");
                
                return;
            }

            GetComponent<GameUnit.GameUnit>().canNotMove = true;
        }

        protected override void OnDisappear()
        {
            base.OnDisappear();

            // 如果本脚本被挂在了地图块上
            if (_isOnBlock)
            {
                // 移除当前地图块上仍存在单位的debuff脚本
                foreach (GameUnit.GameUnit unit in _battleMapBlock.units_on_me)
                {
                    Destroy(unit.GetComponent<BViscous>());
                }

                // 删除receiver，使已注册的trigger被删除
                _globalReceiver = null;

                // 结束执行
                return;
            }

            GetComponent<GameUnit.GameUnit>().canNotMove = false;
        }
    }
    
    public class TViscousTrigger : Trigger
    {
        private BattleMapBlock _specificBlock;
        
        public TViscousTrigger(MsgReceiver speller, BattleMapBlock battleMapBlock)
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
            // 给移动到当前地图块上的单位挂载本debuff
            Gameplay.Info.movingUnit.gameObject.AddBuff<BViscous>(-1);
        }
    }
}