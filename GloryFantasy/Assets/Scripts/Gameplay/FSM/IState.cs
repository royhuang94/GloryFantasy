using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GamePlay.FSM
{
    /// <summary>
    /// 该类是一个接口抽象类，主要是定义了状态机的接口。
    /// 包含：OnEnter、OnExit、OnUpdate三个函数。
    /// 分别是进入状态、停止状态、更新状态
    /// </summary>
    public interface IState
    {
        /// <summary>
        /// 进入状态
        /// </summary>
        /// <param name="prevState"></param>
        void OnEnter(string prevState);
        /// <summary>
        /// 停止状态
        /// </summary>
        /// <param name="nextState"></param>
        void OnExit(string nextState);
        /// <summary>
        /// 更新状态
        /// </summary>
        void OnUpdate();
    }
}

