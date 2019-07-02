using GameCard;
using GamePlay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Ability
{
    /// <summary>
    /// 效果对象枚举类型
    /// </summary>
    public enum TargetType
    {
        EnemyUnit,
        FriendlyUnit,
        NeutralUnit,
        Block,
        Card,
        CDHero,
        Event,
        Empty
    }
    /// <summary>
    /// 效果对象类
    /// </summary>
    public class EffectTarget
    {
        public TargetType TargetType = TargetType.Empty;
        public bool IsSpeller = false;

        public List<string> color;
        public List<string> tag;
        public Func<object,bool> TargetConstrain;
        
        public EffectTarget(TargetType _targetType, bool _isSpeller, Func<object, bool> _targetConstrain)
        {
            this.TargetType = _targetType;
            this.IsSpeller = _isSpeller;
            this.TargetConstrain = _targetConstrain;
        }
    }

    public class Effect : MonoBehaviour, GameplayTool
    {
        
        public List<EffectTarget> abilityTargets;
        public EffectAction action;
        public delegate void SelectionOver();
        public SelectionOver selectionOver;

        public void Excute()
        {
            //先关闭堆叠结算，等待选择结束。
            EffectStack.turnsOff();
            //Gameplay.Instance().gamePlayInput.OnEnterSelectState(this);
        }

        virtual public void Cast()
        {
            // 先执行选择完成的函数
            selectionOver();
            // 将效果句柄压入堆叠
            EffectStack.push(action);
            // 开始堆叠结算
            EffectStack.turnsOn();
        }
    }

    public class Spell : Effect
    {
        public AbilityVariable abilityVariable;
        public GameUnit.GameUnit speller;
        public override void Cast()
        {
            // 先执行选择完成的函数
            selectionOver();
            // 将效果句柄压入堆叠
            EffectStack.push(action);
            // 调度卡牌管理器对玩家专注和手牌信息进行操作
            CardManager.Instance().OnTriggerCurrentCard();
            // 发送施放卡牌的信息
            Gameplay.Info.AbilitySpeller = speller;
            Gameplay.Info.CastingCard = this.GetComponent<OrderCard>();
            IMessage.MsgDispatcher.SendMsg((int)IMessage.MessageType.CastCard);
            // 开始堆叠结算
            EffectStack.turnsOn();
        }
    }
}
