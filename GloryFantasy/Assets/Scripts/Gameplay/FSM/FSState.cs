using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GamePlay.FSM
{
    /// <summary>
    /// 该类主要是状态的基本操作以及事件的添加
    /// 定义了FiniteStateMachine的三个委托
    /// </summary>
    public class FSState
    {
        #region FiniteStateMachine中三个委托的定义
        protected FiniteStateMachine.EnterState mEnterDelegate;
        protected FiniteStateMachine.PushState mPushDelegate;
        protected FiniteStateMachine.PopState mPopDelegate;
        #endregion

        //状态拥有者
        protected FiniteStateMachine mOwner;
        //字典，包含状态切换节点
        protected Dictionary<string, FSEvent> mTranslationEvents;
        //继承自IState的状态对象
        protected IState mStateObject;
        public IState StateObject
        {
            get
            {
                return mStateObject;
            }
        }
        //状态名
        protected string mStateName;
        public string StateName
        {
            get { return mStateName; }
        }

        /// <summary>
        /// FSState构造函数
        /// 定义了FiniteStateMachine的三个委托
        /// </summary>
        /// <param name="obj">状态对象</param>
        /// <param name="owner">状态拥有者</param>
        /// <param name="stateName">状态名</param>
        /// <param name="enter">FiniteStateMachine中的委托函数：状态的进入</param>
        /// <param name="pop">FiniteStateMachine中的委托函数：状态的出栈</param>
        /// <param name="push">FiniteStateMachine中的委托函数：状态的入栈</param>
        public FSState(IState obj, FiniteStateMachine owner, string stateName, FiniteStateMachine.EnterState enter, FiniteStateMachine.PushState push, FiniteStateMachine.PopState pop)
        {
            mStateObject = obj;
            mOwner = owner;
            mStateName = stateName;
            mEnterDelegate = enter;
            mPushDelegate = push;
            mPopDelegate = pop;
            mTranslationEvents = new Dictionary<string, FSEvent>();
        }

        #region On(+1 重载)
        /// <summary>
        /// On(+6 重载)
        /// 该方法用于将事件的名字和事件加入到Dictionary中
        /// </summary>
        /// <param name="eventName"></param>
        /// <returns></returns>
        public FSEvent On(string eventName)
        {
            //创建对应的event事件
            FSEvent newEvent = new FSEvent(eventName, null, this, mOwner, mEnterDelegate, mPushDelegate, mPopDelegate);
            mTranslationEvents.Add(eventName, newEvent);
            return newEvent;
        }
        public FSEvent On<T>(string eventName, Action<T> action)
        {
            //创建对应的event事件
            FSEvent newEvent = new FSEvent(eventName, null, this, mOwner, mEnterDelegate, mPushDelegate, mPopDelegate);
            mTranslationEvents.Add(eventName, newEvent);
            return newEvent;
        }
        #endregion

        #region Trigger
        /// <summary>
        /// Trigger
        /// 该方法用于从Dictionary中取出事件的触发函数，去执行相应的操作。
        /// </summary>
        /// <param name="name"></param>
        public void Trigger(string name)
        {
            mTranslationEvents[name].Execute();
        }

        #endregion

    }
}


