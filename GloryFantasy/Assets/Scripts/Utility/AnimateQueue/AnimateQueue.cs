using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameUtility.Animate
{
    public interface IPlay
    {
        /// <summary>
        /// 提供一个空委托供动画队列封装
        /// </summary>
        Action PlayNext
        {
            get;set;
        }

        /// <summary>
        /// 实现启动方法
        /// </summary>
        void Start();
    }

    public class AnimateQueue : UnitySingleton<AnimateQueue>
    {
        public Queue<IPlay> playQueue = new Queue<IPlay>();

        private int index = 0;

        /// <summary>
        /// 向队列末尾增加动画请求
        /// </summary>
        /// <param name="play"></param>
        public void AddPlay(IPlay play)
        {
            if (play == null)
                return;
            //给动画文件的结束委托封装播放下一个动画的方法
            play.PlayNext += playNext;
            playQueue.Enqueue(play);
            //如果队列没有播放动画就开始播放，这是唯一的启动队列方法
            if (IsAnimating == false)
            {
                Debug.Log(Time.time + " Start play");
                _isAnimating = true;
                //将队头弹出并播放
                playQueue.Dequeue().Start();
            }
        }

        private void playNext()
        {
            //Debug.Log(Time.time + ":Now playQueue count is " + playQueue.Count);
            //如果队列仍然有动画就继续播放
            if (playQueue.Count > 0)
            {
                _isAnimating = true;
                playQueue.Dequeue().Start();
            }
            else
            //如果队列已空
            {
                _isAnimating = false;
            }
        }

        bool _isAnimating = false;
        public bool IsAnimating
        {
            get { return _isAnimating; }
        }

    }
}