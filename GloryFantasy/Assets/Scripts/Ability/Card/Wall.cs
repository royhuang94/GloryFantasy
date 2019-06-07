using System.Linq;
using GamePlay;
using GamePlay.Input;
using GameUnit;
using IMessage;
using UnityEngine;

namespace Ability
{
    public class Wall : Ability
    {
        private Trigger _trigger;

        public override void Init(string abilityId)
        {
            base.Init(abilityId);
            _trigger = new TWall(this.GetCardReceiver(this), AbilityVariable.Turns.Value, abilityId);
            MsgDispatcher.RegisterMsg(_trigger, abilityId);
            
            // 选择三个约束相同的地面
            MyTargetConstraintList[1] = Range_1;
            MyTargetConstraintList[2] = Range_1;
            MyTargetConstraintList[3] = Range_1;
        }
        
        public bool Range_1(object target)
        {
            Vector2 userPos = Gameplay.Instance().gamePlayInput.InputFSM.TargetList[0];

            return GameplayToolExtend.distanceBetween(userPos, target) <= AbilityVariable.Range.Value;
        }
    }

    public class TWall : Trigger
    {
        private int _turn;
        private string _abilityId;

        public TWall(MsgReceiver speller, int turn, string abilityId)
        {
            _turn = turn;
            _abilityId = abilityId;

            register = speller;
            msgName = (int) MessageType.CastCard;
            condition = Condition;
            action = Action;
        }

        private bool Condition()
        {
            if (this.GetCastingCard().GetMsgReceiver() == register &&
                this.GetCastingCard().ability_id.Contains(_abilityId))
            {
                return true;
            }

            return false;
        }

        private void Action()
        {
            string idType =_abilityId.Split('_').Last();

            for (int i = 1; i < 4; i++)
            {
                // 部署墙体
                DispositionCommand unitDispose = new DispositionCommand(
                    "GWall_" + idType,
                    OwnerEnum.Player,
                    BattleMap.BattleMap.Instance().GetSpecificMapBlock(
                        Gameplay.Instance().gamePlayInput.InputFSM.TargetList[i]
                        )
                    );
                unitDispose.Excute();
                
                // 修改crash生命周期
                BattleMap.BattleMap.Instance().GetUnitsOnMapBlock(
                    Gameplay.Instance().gamePlayInput.InputFSM.TargetList[i]
                ).GetComponent<Crash>().SetLife(_turn);
            }
        }
    }
}