using GameCard;
using GamePlay;
using IMessage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ability
{
    /// <summary>
    /// 狼伴：此单位攻击时，创建一张临时的一级霜狼置入你的手中。
    /// </summary>
    class WolfCompanion: Ability
    {
        private GameUnit.GameUnit _unit;
        private int level;
        public class EWolfCompanion: Effect
        {
            private int level;
            public EWolfCompanion(GameUnit.GameUnit unit, int level)
            {
                this.action = act;
                this.level = level;
            }

            public void act()
            {
                string id = "GWinterWolf_" + level.ToString();
                BaseCard card = CardDataBase.Instance().GetCardInstanceById(id);
                card.setWillDestroy(true);
                GameplayToolExtend.moveCard(card, CardArea.Hand);
            }
        }

        public bool condition()
        {
            if (this.GetAttacker() == _unit)
                return true;
            return false;
        }

        public override void Init(string abilityId)
        {
            base.Init(abilityId);
            level = Convert.ToInt32(abilityId.Split('_').Last());
            _unit = this.GetComponent<GameUnit.GameUnit>();
            EWolfCompanion eWolfCompanion = new EWolfCompanion(_unit, level);
            MsgDispatcher.RegisterMsg(
                _unit.GetMsgReceiver(),
                (int)MessageType.AnnounceAttack,
                condition,
                eWolfCompanion.Excute,
                "Trigger WolfCompanion"
            );
        }
    }
}
