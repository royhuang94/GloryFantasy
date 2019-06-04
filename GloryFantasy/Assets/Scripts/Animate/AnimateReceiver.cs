using GameUtility.Animate;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Animate
{
    public class AnimateReceiver : MonoBehaviour
    {
        private Animator _animator;
        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        //播放攻击动画
        public void Attack()
        {
            _animator.SetTrigger("attack");
        }

        public void StateUp()
        {
            _animator.SetTrigger("stateUp1");
        }

        //暴露给动画指令的回调句柄，一个Receiver一次只能使用一个句柄
        public Action OnFinishAnimate;

        //Animator的动画结束的回调
        public void OnFinish()
        {
            Debug.Log("Animate play completly.");
            OnFinishAnimate();
        }
    }
}