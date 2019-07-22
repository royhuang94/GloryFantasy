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
        Unit,
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
        public bool allowCancel;
        public List<EffectTarget> targets;
        public EffectAction action;
        public delegate void SelectionOver();
        public SelectionOver selectionOver;

        public void Excute()
        {
            //先关闭堆叠结算，等待选择结束。
            EffectStack.addLocker();
            Gameplay.Instance().gamePlayInput.OnEffectExcute(this);
        }

        virtual public void Cast()
        {
            // 先执行选择完成的函数
            selectionOver();
            // 将效果句柄压入堆叠
            EffectStack.push(action);
            // 开始堆叠结算
            EffectStack.removeLocker();
        }

        // 取消施放时执行。
        virtual public void cancel()
        {
            EffectStack.removeLocker();
        }
    }

    public class Spell : Effect
    {
        public Variable variable;
        public GameUnit.GameUnit speller;
        public override void Cast()
        {
            // 先执行选择完成的函数
            selectionOver();
            // 将效果句柄压入堆叠
            EffectStack.push(action);
            // 调度卡牌管理器对玩家专注和手牌信息进行操作
            //CardManager.Instance().OnTriggerCurrentCard();
            //HandCardManager.Instance().CurrentSelectingCard
            BaseCard card = Gameplay.Instance().gamePlayInput.InputFSM.selectedCard;
            // 消耗AP值
            Player.Instance().ConsumeAp(card.Cost);
            // 将手牌中的此牌送入待命区
            if (!card.WillDestroy)
                GameplayToolExtend.moveCard(card, CardArea.StandBy);
            else
                HandCardManager.Instance().OperateCard(card, CardArea.Hand, false);
            // 重置当前选中的卡牌
            HandCardManager.Instance().SetSelectCard(-1);
            // 发送施放卡牌的信息
            Gameplay.Info.AbilitySpeller = speller;
            Gameplay.Info.CastingCard = card;
            IMessage.MsgDispatcher.SendMsg((int)IMessage.MessageType.CastCard);
            // 开始堆叠结算
            EffectStack.removeLocker();
        }

        public override void cancel()
        {
            // 取消使用当前的卡牌。
            //CardManager.Instance().CancleUseCurrentCard();
            HandCardManager.Instance().SetSelectCard(-1);
            base.cancel();
        }

        public virtual void init(string spellID)
        {

        }
    }
}
