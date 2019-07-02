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
        public bool IsTarget = false;

        public List<string> color;
        public List<string> tag;
        public Func<object,bool> TargetConstrain;
        
        public EffectTarget(TargetType _targetType, bool _isSpeller, bool _isTarget, Func<object, bool> _targetConstrain)
        {
            this.TargetType = _targetType;
            this.IsSpeller = _isSpeller;
            this.IsTarget = _isTarget;
            this.TargetConstrain = _targetConstrain;
        }
    }

    public class Effect : MonoBehaviour, GameplayTool
    {
        
        public List<EffectTarget> abilityTargets;
        public EffectAction action;
        public delegate void SelectionOver();
        SelectionOver selectionOver;
        public bool isSpell;

        public void Excute()
        {
            //先关闭堆叠结算，等待选择结束。
            EffectStack.turnsOff();
            //Gameplay.Instance().gamePlayInput.OnEnterSelectState(this);
        }

        public void Cast()
        {
            // 先执行选择完成的函数
            selectionOver();
            // 将效果句柄压入堆叠
            EffectStack.push(action);
            // 如果这个效果来自于咒语，那么先发送施放咒语的信息
            if (isSpell)
                IMessage.MsgDispatcher.SendMsg((int)IMessage.MessageType.CastCard);
            // 开始堆叠结算
            EffectStack.turnsOn();
        }
    }
}
