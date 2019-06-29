using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Ability;
using Ability.Buff;
using IMessage;
using GamePlay;
using GameCard;
using GamePlay.Input;
using GamePlay.Round;
using GamePlay.Event;

namespace IMessage
{
    //TODO 关于Info的问题
    //每次攻击都会更新他的ATK部分吗？
    //会不会因为在同一回合中攻击太频繁导致出现BUG

    //更啊，又不会同时发生攻击，怕什么bug

    public class Info
    {
        public PlayerEnum RoundOwned; //回合所属
        public PlayerEnum Caster; //施放者
        public BaseCard CastingCard; //施放的牌
        public PlayerEnum SummonersController; //召唤单位的控制者
        public List<GameUnit.GameUnit> SummonUnit = new List<GameUnit.GameUnit>(); //召唤的单位
        public PlayerEnum Drawer; //抓牌者
        public List<BaseCard> CaughtCard; //抓的牌
        public PlayerEnum HandAdder; //加手者
        public List<BaseCard> AddingCard; //加手的牌
        public GameUnit.GameUnit AbilitySpeller; //发动异能者

        public BattleMap.BattleArea changedBA; //所有权发生变更的战区
        public BattleMap.BattleAreaState newOwner; //发生变更的战区的新的所有者
        public BattleMap.BattleAreaState exOwner; //发生变更的战区的上一任所有者

        #region ATK部分
        public Ability.Ability SpellingAbility; //发动的异能
        public GameUnit.GameUnit Attacker; //宣言攻击者
        public GameUnit.GameUnit AttackedUnit; //被攻击者
        public List<GameUnit.GameUnit> Injurer; //伤害者
        public List<GameUnit.GameUnit> InjuredUnit; //被伤害者
        public List<Damage> damage; //伤害
        public GameUnit.GameUnit Killer; //击杀者
        public GameUnit.GameUnit KilledUnit; //被杀者
        public GameUnit.GameUnit Dead; //死者
        public int ATKDistance; //攻击者与被攻击者之间的距离
        #endregion

        public GameUnit.GameUnit GeneratingUnit;
        public bool locking = false;
        public GameUnit.GameUnit movingUnit; //正在移动的单位
        public GameUnit.GameUnit otherMovingUnit; //正在移动的单位

        /// <summary>
        /// 被UI选定的Unit单位
        /// </summary>
        public GameUnit.GameUnit SelectingUnit;

        public Info Clone()
        {
            Info other = new Info();
            other.RoundOwned = this.RoundOwned;
            other.Caster = this.Caster;
            other.CastingCard = this.CastingCard;
            other.SummonersController = this.SummonersController;
            other.SummonUnit = this.SummonUnit;
            other.GeneratingUnit = this.GeneratingUnit;
            other.Drawer = this.Drawer;
            other.CaughtCard = this.CaughtCard;
            other.HandAdder = this.HandAdder;
            other.AddingCard = this.AddingCard;
            other.Attacker = this.Attacker;
            other.AttackedUnit = this.AttackedUnit;
            other.AbilitySpeller = this.AbilitySpeller;
            other.SpellingAbility = this.SpellingAbility;
            other.Injurer = this.Injurer;
            other.InjuredUnit = this.InjuredUnit;
            other.damage = this.damage;
            other.Killer = this.Killer;
            other.KilledUnit = this.KilledUnit;
            other.Dead = this.Dead;

            return other;
        }
    }
}

namespace GamePlay
{
    

    public class Gameplay : UnitySingleton<Gameplay>
    {

        public void Awake()
        {
            roundProcessController = new RoundProcessController();
            gamePlayInput = new GameplayInput();
            bmbColliderManager = new BMBColliderManager();
            buffManager = new BuffManager();
            autoController = new AI.AutoController();
            singleBattle = new AI.BattleField();
            eventScroll = new EventScroll();
        }
        //private void Update()
        //{
        //    if(roundProcessController.roundInput != RoundInput.None)
        //    {
        //        IEnumerator coroutine;
        //        coroutine = WaitAndPrint(8.0f);
        //        StartCoroutine(coroutine);
        //    }
        //}

        public static Info Info = new Info();
        public RoundProcessController roundProcessController; 
        public GameplayInput gamePlayInput;
        public BMBColliderManager bmbColliderManager;
        public BuffManager buffManager;
        public  AI.BattleField singleBattle;
        public AI.AutoController autoController;
        public EventScroll eventScroll;

        private void Update()
        {
            gamePlayInput.Update();
        }

        /// <summary>
        /// 提供给场景中阶段切换的按钮
        /// </summary>
        public void switchPhaseHandler()
        {
            roundProcessController.StepIntoNextStateByButton();
        }

        #region 回合自流动
        /// <summary>
        /// 启动回合自流动控制器
        /// </summary>
        private void Start()
        {
            roundProcessController.action = BackUpdateRound;
            //StartCoroutine(RoundUpdate());
            BackUpdateRound(RoundInput.RestoreApPhase);
        }
        /// <summary>
        /// 提供给场景中阶段切换的按钮
        /// </summary>
        public IEnumerator SwitchPhaseHandler()
        {
            yield return StartCoroutine(roundProcessController.StepIntoNextState());
        }
        /// <summary>
        /// 回调函数
        /// </summary>
        private void BackUpdateRound(RoundInput roundInput, float waitTime = 0.0f)
        {
            if(waitTime <= 0.0f)
                StartCoroutine(WaitAndPrint(roundInput));
            else
                StartCoroutine(WaitAndPrint(roundInput, waitTime));
        }


        private IEnumerator WaitAndPrint(RoundInput roundInput, float waitTime = 1.8f)
        {
            if (roundProcessController.roundInput != roundInput)
            {
                yield return new WaitForSeconds(waitTime);
                roundProcessController.roundInput = roundInput;
                StartCoroutine(RoundUpdate());
            }
        }

        private IEnumerator RoundUpdate()
        {
            switch (GamePlay.Gameplay.Instance().roundProcessController.roundInput)
            {
                case GamePlay.Round.RoundInput.None:
                    Debug.Log("None");
                    yield return null;
                    break;
                case GamePlay.Round.RoundInput.RestoreApPhase:
                    Debug.Log("RestoreApPhase");
                    yield return StartCoroutine(SwitchPhaseHandler());
                    break;
                case GamePlay.Round.RoundInput.StartPhase:
                    Debug.Log("StartPhase");
                    yield return StartCoroutine(SwitchPhaseHandler());
                    break;
                case GamePlay.Round.RoundInput.ExtractCardsPhase:
                    Debug.Log("ExtractCardsPhase");
                    yield return StartCoroutine(SwitchPhaseHandler());
                    break;
                case GamePlay.Round.RoundInput.PreparePhase:
                    Debug.Log("PreparePhase");
                    yield return StartCoroutine(SwitchPhaseHandler());
                    break;
                case GamePlay.Round.RoundInput.MainPhase:
                    Debug.Log("MainPhase");
                    break;
                case GamePlay.Round.RoundInput.DiscardPhase:
                    Debug.Log("DiscardPhase");
                    yield return StartCoroutine(SwitchPhaseHandler());
                    break;
                case GamePlay.Round.RoundInput.EndPhase:
                    yield return StartCoroutine(SwitchPhaseHandler());
                    Debug.Log("EndPhase");
                    break;
                case GamePlay.Round.RoundInput.AIPhase:
                    Debug.Log("AIPhase");
                    break;
                default:
                    Debug.Log("None");
                    yield return null;
                    break;
            }

        }
        #endregion

        /// <summary>
        /// 当单位移动范围显示的时候，点击卡牌，取消移动范围显示,防止被箭头覆盖
        /// </summary>
        public void CancleMoveRangeMark()
        {
            if (BattleMap.BattleMap.Instance().IsMoveColor == true)
            {
                GameGUI.ShowRange.Instance().CancleMoveRangeMark();
                gamePlayInput.InputFSM.PushState(new FSM.InputFSMIdleState(gamePlayInput.InputFSM));
            }
        }
    }
}