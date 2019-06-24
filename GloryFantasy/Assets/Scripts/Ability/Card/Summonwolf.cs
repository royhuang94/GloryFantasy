using System.Linq;
using GamePlay;
using GamePlay.Input;
using GameUnit;
using IMessage;

namespace Ability
{
    public class Summonwolf : Ability
    {
        private Trigger _trigger;

        /// <summary>
        /// 部署两个幼小/成熟/强大的霜狼。
        /// </summary>
        /// <param name="abilityId"></param>
        public override void Init(string abilityId)
        {
            base.Init(abilityId);
            _trigger = new TSummonWolf(this.GetCardReceiver(this), abilityId);
            MsgDispatcher.RegisterMsg(_trigger, abilityId);
            MyTargetConstraintList[0] = Range_1;
            MyTargetConstraintList[1] = Range_1;
        }

        public bool Range_1(object target)
        {
            // 是mapblock啦
            // 如果目标类型不是GameUnit，直接返回false，为了防止后续强转出错
            if (!target.GetType().ToString().Equals("BattleMap.BattleMapBlock"))
            {
                return false;
            }

            OwnerEnum owner = BattleMap.BattleMap.Instance().GetMapblockBelong((target as BattleMap.BattleMapBlock).position);

            return owner.Equals(OwnerEnum.Player);
        }
    }

    public class TSummonWolf : Trigger
    {
        private string _abilityId;

        public TSummonWolf(MsgReceiver speller, string abilityId)
        {
            _abilityId = abilityId;

            register = speller;
            msgName = (int) MessageType.CastCard;
            condition = Condition;
            action = Action;
        }

        private bool Condition()
        {
            return this.GetCastingCard().GetMsgReceiver() == register &&
                   this.GetCastingCard().ability_id.Contains(_abilityId);
        }

        private void Action()
        {
            string idType = _abilityId.Split('_').Last();

            for (int i = 1; i < 3; i++)
            {
                // 部署对应种类的霜狼
                DispositionCommand unitDispose = new DispositionCommand(
                    "GWinterwolf_" + idType,
                    OwnerEnum.Player,
                    BattleMap.BattleMap.Instance().GetSpecificMapBlock(
                        Gameplay.Instance().gamePlayInput.InputFSM.TargetList[i]
                        )
                    );
                
                unitDispose.Excute();
            }
        }
    }
}