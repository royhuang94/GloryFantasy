using System;
using System.Collections;
using System.Collections.Generic;
using GamePlay.Event;
using IMessage;
using UnityEngine;

namespace GamePlay.Round
{
    //TOOD 使用switch控制new出不同的对象
    public enum RoundInput
    {
        None = -1,

        RestoreApPhase,
        StartPhase,
        ExtractCardsPhase,
        PreparePhase,
        MainPhase,
        DiscardPhase,
        EndPhase,
        AIPhase
    };

    //public static RestoreApPhase RestoreApPhase = new RestoreApPhase();
    //public static StartPhase startPhase = new StartPhase();
    //public static ExtractCardsPhase ExtractCardsPhase = new ExtractCardsPhase();
    //public static PreparePhase PreparePhase = new PreparePhase();
    //public static MainPhase mainPhase = new MainPhase();
    //public static DiscardPhase discardPhase = new DiscardPhase();
    //public static EndPhase endPhase = new EndPhase();
    //public static AIPhase AiPhase = new AIPhase();
    //public static WinState WinState = new WinState();
    //public static LoseState LoseState = new LoseState();

    /// <summary>
    /// 回合流程控制器
    /// </summary>
    public class RoundProcessController
    {
        public RoundProcessController()
        {
            State = RoundState.RestoreApPhase;
            // 初始化状态后手动调用以进行当前状态的工作
            EnteringCurrentState();
        }

        /// <summary>
        /// 切换到下一个状态要做的工作，现暂时使用按钮进行调用，后面由动画队列空消息触发
        /// </summary>
        public void StepIntoNextStateByButton()
        {
            State.Exit(this);
            State.NextState(this);
            EnteringCurrentState();
        }

        /// <summary>
        /// 切换到下一个状态要做的工作，现暂时使用按钮进行调用，后面由动画队列空消息触发
        /// </summary>
        public IEnumerator StepIntoNextState()
        {
            State.Exit(this);
            State.NextState(this);
            EnteringCurrentState();

            yield return null;
        }

        /// <summary>
        /// 进入当前状态需要做的工作
        /// </summary>
        public void EnteringCurrentState()
        {
            State.Enter(this);
            State.Update(this);
        }

        /// <summary>
        /// 对外接口判断，用于检测当前是否为玩家可操作的主要阶段
        /// </summary>
        /// <returns>若为玩家可操作的主要阶段，则返回true，否则返回false</returns>
        public bool IsPlayerRound()
        {
            return (State == RoundState.mainPhase || State == RoundState.discardPhase || State == RoundState.endPhase);
        }


        /// <summary>
        /// 返回当前状态机是否处于结果状态（胜利，失败）
        /// </summary>
        /// <returns>如果不在结果状态，返回false，否则返回true</returns>
        public bool IsResultState()
        {
            if (State != RoundState.WinState || State != RoundState.LoseState)
                return false;

            return true;
        }


        /// <summary>
        /// 对外提供的接口，用于使状态机进入胜利状态
        /// </summary>
        public void Win()
        {
            State = RoundState.WinState;
            State.Enter(this);
        }

        /// <summary>
        /// 对外提供的接口，用于使状态机进入失败的状态
        /// </summary>
        public void Lose()
        {
            State = RoundState.LoseState;
            State.Enter(this);
        }

        /// <summary>
        /// 用于重置状态机当前状态，使进入恢复专注值状态
        /// </summary>
        public void SetDefault()
        {
            State = RoundState.RestoreApPhase;
            EnteringCurrentState();
        }

        public RoundState State;
        public RoundInput roundInput = RoundInput.None;
        public Action<RoundInput, float> action;
    }

    public class RoundState
    {
        protected int _roundCounter = 0;

        public int roundCounter
        {
            set { _roundCounter = value; }
            get { return _roundCounter; }
        }

        public virtual void HandleInput(RoundProcessController roundProcessController, RoundInput input) { }
        public virtual void Update(RoundProcessController roundProcessController) { }
        public virtual void Enter(RoundProcessController roundProcessController) { }
        public virtual void Exit(RoundProcessController roundProcessController) { }
        public virtual void NextState(RoundProcessController roundProcessController)
        {
            Exit(roundProcessController);
        }

        public static RestoreApPhase RestoreApPhase = new RestoreApPhase();
        public static StartPhase startPhase = new StartPhase();
        public static ExtractCardsPhase ExtractCardsPhase = new ExtractCardsPhase();
        public static PreparePhase PreparePhase = new PreparePhase();
        public static MainPhase mainPhase = new MainPhase();
        public static DiscardPhase discardPhase = new DiscardPhase();
        public static EndPhase endPhase = new EndPhase();
        public static AIPhase AiPhase = new AIPhase();
        public static WinState WinState = new WinState();
        public static LoseState LoseState = new LoseState();
    }

    /// <summary>
    /// 恢复费用阶段
    /// </summary>
    public class RestoreApPhase : RoundState
    {
        public override void NextState(RoundProcessController roundProcessController)
        {
            base.NextState(roundProcessController);
            roundProcessController.State = RoundState.startPhase;
            //roundProcessController.roundInput = RoundInput.StartPhase;
            roundProcessController.action(RoundInput.StartPhase, 1.8f);
        }

        public override void Enter(RoundProcessController roundProcessController)
        {
            base.Enter(roundProcessController);
            // 发送更新资源点消息
            MsgDispatcher.SendMsg((int)MessageType.UpdateSource);
        }

        public override string ToString()
        {
            return "恢复专注值阶段";
        }
    }

    /// <summary>
    /// 开始阶段
    /// </summary>
    public class StartPhase : RoundState
    {
        bool isFirstEnter = true;
        public override void NextState(RoundProcessController roundProcessController)
        {
            base.NextState(roundProcessController);
            roundProcessController.State = ExtractCardsPhase;
            //roundProcessController.roundInput = RoundInput.ExtractCardsPhase;
            roundProcessController.action(RoundInput.ExtractCardsPhase, 3.0f);
            roundProcessController.State.Enter(roundProcessController);
        }

        public override void Enter(RoundProcessController roundProcessController)
        {
            base.Enter(roundProcessController);
            if (isFirstEnter)
            {
                for (int i = 0; i < Gameplay.Instance().eventScroll.EventScrollCount; i++)
                {
                    Gameplay.Instance().eventScroll.CreateNewEventAssembly();
                    isFirstEnter = false;
                }
            }

            _roundCounter++;
            Gameplay.Instance().eventScroll.CreateNewEventAssembly();
            Gameplay.Instance().eventScroll.ProcessFirstEventModule();
            MsgDispatcher.SendMsg((int)MessageType.BP);
        }
        public override string ToString()
        {
            return "开始阶段";
        }
    }


    /// <summary>
    /// 抽牌阶段
    /// </summary>
    public class ExtractCardsPhase : RoundState
    {
        public override void NextState(RoundProcessController roundProcessController)
        {
            base.NextState(roundProcessController);
            roundProcessController.State = RoundState.PreparePhase;
            //roundProcessController.roundInput = RoundInput.PreparePhase;
            roundProcessController.action(RoundInput.PreparePhase, 2.0f);
        }

        public override void Enter(RoundProcessController roundProcessController)
        {
            base.Enter(roundProcessController);
            MsgDispatcher.SendMsg((int)MessageType.DrawCard);
        }

        public override string ToString()
        {
            return "抽牌阶段";
        }
    }

    /// <summary>
    /// 准备阶段
    /// </summary>
    public class PreparePhase : RoundState
    {
        public override void NextState(RoundProcessController roundProcessController)
        {
            base.NextState(roundProcessController);
            roundProcessController.State = RoundState.mainPhase;
            //roundProcessController.roundInput = RoundInput.MainPhase;
            roundProcessController.action(RoundInput.MainPhase, 1.8f);
            roundProcessController.State.Enter(roundProcessController);
        }

        public override void Enter(RoundProcessController roundProcessController)
        {
            base.Enter(roundProcessController);
            MsgDispatcher.SendMsg((int)MessageType.Prepare);

            foreach (GameUnit.GameUnit unit in BattleMap.BattleMap.Instance().UnitsList)
            {
                if (unit.owner == GameUnit.OwnerEnum.Player)
                {
                    unit.canNotMove = false;
                    unit.canNotAttack = false;
                }
            }
        }

        public override string ToString()
        {
            return "准备阶段";
        }
    }


    /// <summary>
    /// 主要阶段
    /// </summary>
    public class MainPhase : RoundState
    {
        public override void NextState(RoundProcessController roundProcessController)
        {
            base.NextState(roundProcessController);
            roundProcessController.State = RoundState.discardPhase;
            roundProcessController.action(RoundInput.DiscardPhase, 1.0f);
            //roundProcessController.roundInput = RoundInput.None;
        }

        /// <summary>
        /// 进入状态时发送主要阶段开始消息
        /// </summary>
        /// <param name="roundProcessController"></param>
        public override void Enter(RoundProcessController roundProcessController)
        {
            base.Enter(roundProcessController);
            MsgDispatcher.SendMsg((int)MessageType.MPBegin);
        }

        /// <summary>
        /// 退出状态时发送主要阶段结束消息
        /// </summary>
        /// <param name="roundProcessController"></param>
        public override void Exit(RoundProcessController roundProcessController)
        {
            base.Exit(roundProcessController);
            MsgDispatcher.SendMsg((int)MessageType.MPEnd);
        }

        public override string ToString()
        {
            return "我方回合/主要阶段";
        }
    }


    /// <summary>
    /// 弃牌阶段
    /// </summary>
    public class DiscardPhase : RoundState
    {
        public override void NextState(RoundProcessController roundProcessController)
        {
            base.NextState(roundProcessController);
            roundProcessController.State = RoundState.endPhase;
            roundProcessController.action(RoundInput.EndPhase, 1.0f);
        }

        public override void Enter(RoundProcessController roundProcessController)
        {
            base.Enter(roundProcessController);
            MsgDispatcher.SendMsg((int)MessageType.Discard);


        }

        public override string ToString()
        {
            return "我方回合/弃牌阶段";
        }
    }


    /// <summary>
    /// 结束阶段
    /// </summary>
    public class EndPhase : RoundState
    {
        public override void NextState(RoundProcessController roundProcessController)
        {
            base.NextState(roundProcessController);
            roundProcessController.State = RoundState.AiPhase;
            //roundProcessController.roundInput = RoundInput.AIPhase;
            roundProcessController.action(RoundInput.AIPhase, 2.8f);
        }

        public override void Enter(RoundProcessController roundProcessController)
        {
            base.Enter(roundProcessController);
            MsgDispatcher.SendMsg((int)MessageType.EP);
        }

        public override string ToString()
        {
            return "结束阶段";
        }
    }

    public class AIPhase : RoundState
    {
        public override void NextState(RoundProcessController roundProcessController)
        {
            base.NextState(roundProcessController);
            roundProcessController.State = RoundState.RestoreApPhase;
            //roundProcessController.roundInput = RoundInput.RestoreApPhase;
            roundProcessController.action(RoundInput.RestoreApPhase, 1.8f);
            base.roundCounter += 1;
        }

        public override void Enter(RoundProcessController roundProcessController)
        {
            base.Enter(roundProcessController);
            MsgDispatcher.SendMsg((int)MessageType.AI);

            Gameplay.Instance().singleBattle.battleState = AI.BattleState.Prepare;
            Gameplay.Instance().singleBattle.Run();
        }
        public override string ToString()
        {
            return "AI阶段";
        }
    }

    public class WinState : RoundState
    {
        public override void Enter(RoundProcessController roundProcessController)
        {
            base.Enter(roundProcessController);
            MsgDispatcher.SendMsg((int)MessageType.WIN);
        }

        public override string ToString()
        {
            return "获胜";
        }
    }

    public class LoseState : RoundState
    {
        public override void Enter(RoundProcessController roundProcessController)
        {
            base.Enter(roundProcessController);
            MsgDispatcher.SendMsg((int)MessageType.LOSE);
        }

        public override string ToString()
        {
            return "败北";
        }
    }

}