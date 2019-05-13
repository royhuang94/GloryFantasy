using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 1. FSEvent类实现了FiniteStateMachine中三个委托的定义。
/// 2. 该类主要用于事件的处理。
/// 3. 定义一个枚举用于处理有限状态机的状态，以及使用System的Func类。
/// 4. 核心功能是处理FSState的Enter、Push、Pop。
/// 5. 通过调用FSEvent的Execute对Enter、Push、Pop状态的切换。
/// </summary>
namespace GamePlay.FSM
{
    public class FSEvent
    {
        protected FiniteStateMachine.EnterState mEnterDelegate;
        protected FiniteStateMachine.PushState mPushDelegate;
        protected FiniteStateMachine.PopState mPopDelegate;

        /// <summary>
        /// 通过调用FSEvent的Execute对Enter、Push、Pop状态的切换
        /// </summary>
        protected enum EventType { NONE, ENTER, PUSH, POP};

        //当前事件名
        protected string mEventName;
        //目标状态名
        protected string mTargetState;
        //拥有者
        protected FiniteStateMachine mOwner;
        //当前事件的状态拥有者
        protected FSState mStateOwner;
        protected EventType eType;

        /// <summary>
        /// FSEvent构造函数
        /// </summary>
        /// <param name="eventName">事件名</param>
        /// <param name="targetState">目标状态名</param>
        /// <param name="stateOwner">当前事件的状态拥有者</param>
        /// <param name="owner">拥有者</param>
        /// <param name="enter">FiniteStateMachine中的委托函数：状态的进入</param>
        /// <param name="push">FiniteStateMachine中的委托函数：状态的入栈</param>
        /// <param name="pop">FiniteStateMachine中的委托函数：状态的出栈</param>
        public FSEvent(string eventName, string targetState, 
            FSState stateOwner, FiniteStateMachine owner, 
            FiniteStateMachine.EnterState enter, FiniteStateMachine.PushState push, FiniteStateMachine.PopState pop)
        {
            mEventName = eventName;
            mTargetState = targetState;
            mStateOwner = stateOwner;
            mOwner = owner;
            mEnterDelegate = enter;
            mPushDelegate = push;
            mPopDelegate = pop;
            eType = EventType.NONE;
        }

        /// <summary>
        /// 对进入目标状态的设置
        /// </summary>
        /// <param name="targetState"></param>
        /// <returns></returns>
        public FSState Enter(string targetState)
        {
            eType = EventType.PUSH;
            mTargetState = targetState;
            return mStateOwner;
        }
        /// <summary>
        /// 对进入目标状态的设置
        /// </summary>
        /// <param name="targetState"></param>
        /// <returns></returns>
        public FSState Push(string targetState)
        {
            eType = EventType.PUSH;
            mTargetState = targetState;
            return mStateOwner;
        }
        /// <summary>
        /// 退出目标状态的设置
        /// </summary>
        public void Pop()
        {
            eType = EventType.POP;
        }

        /// <summary>
        /// 对Enter、Push、Pop状态的切换
        /// </summary>
        public void Execute()
        {
            if (eType == EventType.POP)
                mPopDelegate();
            else if (eType == EventType.PUSH)
                mPushDelegate(mTargetState, mOwner.CurrentState.StateName);
            else if (eType == EventType.ENTER)
                mEnterDelegate(mTargetState);
        }
    }
}

