using System.Collections;
using System.Collections.Generic;
using GameCard;
using UnityEngine;

using IMessage;
using GamePlay;

namespace Ability
{
    /// <summary>
    /// 使用者：携带有战技的友方单位。创建使用者携带的 1/1/2 张战技的临时复制置入你的手中（临时复制使用后销毁）
    /// </summary>
    public class InstantIdea : Ability
    {
        Trigger trigger;

//        private void Awake()
//        {
//            //导入InstantIdea异能的参数
//            InitialAbility("InstantIdea");
//        }
//
//        private void Start()
//        {
//            //创建Trigger实例，传入技能的发动者
//            trigger = new TInstantIdea(this.GetCardReceiver(this));
//            //注册Trigger进消息中心
//            MsgDispatcher.RegisterMsg(trigger, "InstantIdea");
//        }

        public override void Init(string abilityId)
        {
            base.Init(abilityId);
            trigger = new TInstantIdea(this.GetCardReceiver(this), abilityId);
            MsgDispatcher.RegisterMsg(trigger, abilityId);
        }
    }

    public class TInstantIdea : Trigger
    {
        private GameUnit.GameUnit unit;
        private string _abilityId;
        public TInstantIdea(MsgReceiver speller, string abilityId)
        {
            _abilityId = abilityId;
            register = speller;
            //初始化响应时点,为卡片使用时
            msgName = (int)MessageType.CastCard;
            //初始化条件函数和行为函数
            condition = Condition;
            action = Action;
        }

        private bool Condition()
        {
            //判断发动的卡是不是这个技能的注册者，并且这张卡是不是瞬发幻想
            if (this.GetCastingCard().GetMsgReceiver() == register && this.GetCastingCard().ability_id.Contains(_abilityId))
            {
                unit = (GameUnit.GameUnit) this.GetSelectingUnits()[0];
                if (unit.tag.Contains("英雄"))
                    return true;
            }
            
            return false;
        }

        private void Action()
        {
            //获取被选中的友军，需要自己根据技能描述强转类型，一旦强转的类型是错的代码会出错
            //复制被选中友军的一次性战技入手牌
            List<string> exCardId = CardManager.Instance().unitsExSkillCardDataBase[unit.name];
            foreach (string id in exCardId)
            {
                CardManager.Instance().ArrangeExSkillCard(id, unit.gameObject.GetInstanceID(), true);
                
                // 如果是是InstantIdea_3，就再复制一张
                if (_abilityId.Contains("_3"))
                {
                    CardManager.Instance().ArrangeExSkillCard(id, unit.gameObject.GetInstanceID(), true);
                }
            }
        }
    }
}