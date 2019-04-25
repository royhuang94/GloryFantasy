using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GamePlay.Round
{

    public enum RoundInput
    {

    };

    /// <summary>
    /// 回合流程控制器
    /// </summary>
    public class RoundProcessController
    {
        public RoundProcessController()
        {
            _state = RoundState.crystalPhase;
        }

        public bool IsPlayerRound()
        {
            return this.isPlayerRound;
        }

        public bool IsCpuRound()
        {
            return !this.isPlayerRound;
        }

        public void EnterPlayerRound()
        {
            this.isPlayerRound = true;
            // TODO : 进入玩家回合的操作
        }

        public void QuitPlayerRound()
        {
            this.isPlayerRound = false;
            // TODO : 退出玩家回合的操作
        }

        public void EnterCpuRound()
        {
            this.isPlayerRound = false;
            // TODO : 进入AI回合的操作
        }

        public void QuitCpuRound()
        {
            // TODO : 退出CPU回合的操作
        }

        private bool isPlayerRound = false;

        public RoundState _state;
    }

    public class RoundState
    {
        virtual public void HandleInput(RoundProcessController roundProcessController, RoundInput input) { }
        virtual public void Update(RoundProcessController roundProcessController) { }
        virtual public void Enter(RoundProcessController roundProcessController) { }
        virtual public void Exit(RoundProcessController roundProcessController) { }
        virtual public void NextState(RoundProcessController roundProcessController)
        {
            Exit(roundProcessController);
        }

        public static CrystalPhase crystalPhase;
        public static StartPhase startPhase;
        public static DrawPhase drawPhase;
        public static ReadyPhase readyPhase;
        public static MainPhase mainPhase;
        public static DiscardPhase discardPhase;
        public static EndPhase endPhase;
    }

    public class CrystalPhase : RoundState
    {
        public override void NextState(RoundProcessController roundProcessController)
        {
            base.NextState(roundProcessController);
            roundProcessController._state = RoundState.startPhase;
        }
    }

    public class StartPhase : RoundState
    {
        public override void NextState(RoundProcessController roundProcessController)
        {
            base.NextState(roundProcessController);
            roundProcessController._state = RoundState.drawPhase;
        }
    }

    public class DrawPhase : RoundState
    {
        public override void NextState(RoundProcessController roundProcessController)
        {
            base.NextState(roundProcessController);
            roundProcessController._state = RoundState.readyPhase;
        }
    }

    public class ReadyPhase : RoundState
    {
        public override void NextState(RoundProcessController roundProcessController)
        {
            base.NextState(roundProcessController);
            roundProcessController._state = RoundState.mainPhase;
        }
    }

    public class MainPhase : RoundState
    {
        public override void NextState(RoundProcessController roundProcessController)
        {
            base.NextState(roundProcessController);
            roundProcessController._state = RoundState.discardPhase;
        }
    }

    public class DiscardPhase : RoundState
    {
        public override void NextState(RoundProcessController roundProcessController)
        {
            base.NextState(roundProcessController);
            roundProcessController._state = RoundState.endPhase;
        }
    }

    public class EndPhase : RoundState
    {
        public override void NextState(RoundProcessController roundProcessController)
        {
            base.NextState(roundProcessController);
            roundProcessController._state = RoundState.crystalPhase;
        }
    }
}