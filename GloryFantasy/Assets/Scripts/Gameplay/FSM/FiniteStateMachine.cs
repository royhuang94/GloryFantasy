using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GamePlay.FSM
{
    /// <summary>
    /// 1. 该类是对外开放的，主要用于外界的调用，独立封装，不继承Mono。
    /// 2. 该类定义一个栈，用于存放FSState，通过Update进行状态的切换。
    /// 3. 对栈的管理及Pop和Push操作、状态机的注册。
    /// 4. 该类声明了三个重要的委托，并在FSState类中定义。
    /// </summary>
    public class FiniteStateMachine
    {
        #region 委托的声明，这三个委托主要用于状态的转换。
        public delegate void EnterState(string stateName);
        public delegate void PushState(string stateName, string lastStateName);
        public delegate void PopState();
        #endregion

        //用于注册后的状态字典
        protected Dictionary<string, FSState> mStates;
        //状态入口
        protected string mEntryPoint;

        /// <summary>
        /// 栈用于状态机的注册，对Stack的操作主要是Pop和Push函数
        /// </summary>
        protected Stack<FSState> mStateStack;

        /// <summary>
        /// 返回当前状态
        /// 状态栈如果总数为0，则返回NULL
        /// 否在返回栈顶的状态
        /// </summary>
        public FSState CurrentState
        {
            get
            {
                if(mStateStack.Count == 0)
                {
                    return null;
                }
                //返回栈顶的状态
                return mStateStack.Peek();
            }
        }
        /// <summary>
        /// FiniteStateMachine构造函数
        /// </summary>
        public FiniteStateMachine()
        {
            mStates = new Dictionary<string, FSState>();
            mStateStack = new Stack<FSState>();
            mEntryPoint = null;
        }

        public void Update()
        {
            if(CurrentState == null)
            {
                //如果当前状态为null，则把注册字典中，的入口状态压栈
                mStateStack.Push(mStates[mEntryPoint]);
                CurrentState.StateObject.OnEnter(null);
            }
            CurrentState.StateObject.OnUpdate();//如果不为null，则持续更新
        }

        public void Register(string stateName, IState stateObject)
        {
            if (mStates.Count == 0)
                mEntryPoint = stateName;
            mStates.Add(stateName, new FSState(stateObject, this, stateName, Enter, Push, Pop));
        }

        /// <summary>
        /// 状态进栈
        /// </summary>
        /// <param name="stateName"></param>
        public void Enter(string stateName)
        {
            Push(stateName, Pop(stateName));
        }

        /// <summary>
        /// Push 重载(+1)
        /// 状态进栈
        /// </summary>
        /// <param name="newState"></param>
        public void Push(string newState)
        {
            string lastName = null;
            if(mStateStack.Count > 1)
            {
                lastName = mStateStack.Peek().StateName;
            }

            Push(newState, lastName);
        }
        public void Push(string stateName, string lastStateName)
        {
            mStateStack.Push(mStates[stateName]);
            mStateStack.Peek().StateObject.OnEnter(lastStateName);
        }

        /// <summary>
        /// Pop 重载 (+1)
        /// 状态弹栈
        /// </summary>
        public void Pop()
        {
            Pop(null);
        }

        public string Pop(string newName)
        {
            //TODO 为啥要区分 newState == null，与不为null的情况
            FSState lastState = mStateStack.Peek();
            string newState = null;
            if (newName == null && mStateStack.Count > 1)
            {
                int index = 0;
                foreach (FSState item in mStateStack)
                {
                    if (index++ == mStateStack.Count - 2)
                        newState = item.StateName;
                }
            }
            else
                newState = newName;

            string lastStateName = null;
            if(lastState != null)
            {
                lastStateName = lastState.StateName;
                lastState.StateObject.OnExit(newState);
            }
            mStateStack.Pop();
            return lastStateName;
        }

        /// <summary>
        /// 从字典中返回对应键值的状态
        /// </summary>
        /// <param name="stateName"></param>
        /// <returns></returns>
        public FSState GetState(string stateName)
        {
            return mStates[stateName];
        }

        /// <summary>
        /// 设置入口状态名称
        /// </summary>
        /// <param name="stateName"></param>
        public void EntryPoint(string stateName)
        {
            mEntryPoint = stateName;
        }

        /// <summary>
        /// 事件的触发
        /// </summary>
        /// <param name="eventName"></param>
        public void Trigger(string eventName)
        {
            CurrentState.Trigger(eventName);
        }
    }

}
