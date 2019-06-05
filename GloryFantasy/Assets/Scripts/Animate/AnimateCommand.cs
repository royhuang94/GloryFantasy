using GameUtility.Animate;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Animate
{
    public class AnimateCommand : IPlay
    {
        protected Action _playNext;
        Action IPlay.PlayNext
        {
            get
            {
                return _playNext;
            }

            set
            {
                _playNext = value;
            }
        }

        void IPlay.Start()
        {
            StartPlayAnimate();
        }

        //模型动画播放对象的引用
        protected AnimateReceiver _animateReceiver;

        //开始播放动画的虚方法
        protected virtual void StartPlayAnimate()
        {
        }

        //动画指令结束的回调
        protected void OnFinished()
        {
            _playNext();
        }
    }

    public class AttackAnimate : AnimateCommand
    {

        public AttackAnimate(GameUnit.GameUnit attacker, GameUnit.GameUnit beAttacked)
        {
            _animateReceiver = attacker.GetComponentInChildren<AnimateReceiver>();
        }

        //开始播放动画，并且将结束回调委托与模型动画的结束挂钩
        protected override void StartPlayAnimate()
        {
            base.StartPlayAnimate();
            _animateReceiver.OnFinishAnimate = OnFinished;
            //播放动画
            _animateReceiver.Attack();
            //可能还有位移等需要处理
        }
    }

    public class StateUp1Animate : AnimateCommand
    {
        public StateUp1Animate(GameUnit.GameUnit _speller)
        {
            _animateReceiver = _speller.GetComponentInChildren<AnimateReceiver>();
        }

        //开始播放动画，并且将结束回调委托与模型动画的结束挂钩
        protected override void StartPlayAnimate()
        {
            base.StartPlayAnimate();
            _animateReceiver.OnFinishAnimate = OnFinished;
            //播放动画
            _animateReceiver.StateUp();
        }
    }
}