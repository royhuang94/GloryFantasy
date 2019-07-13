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
using GamePlay.Encounter;
using BattleMap;
using GameUnit;

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
        public Ability.Effect SelectingEffect; //正在为什么效果选取对象
        public GameUnit.GameUnit Attacker; //宣言攻击者
        public GameUnit.GameUnit AttackedUnit; //被攻击者
        public GameUnit.GameUnit Injurer; //伤害者
        public GameUnit.GameUnit InjuredUnit; //被伤害者
        public Damage damage; //伤害
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
            other.SelectingEffect = this.SelectingEffect;
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
        public AI.BattleField singleBattle;
        public AI.AutoController autoController;
        public EventScroll eventScroll;
        private string _encouterID;//该次遭遇的遭遇id;
        public string EncouterID { get { return _encouterID; } }

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
            InitMap(GetEncounterID(), GetDeck());
        }

        public void InitMap(string encouterId, Mediator.Deck deck)
        {
            if (deck == null)
            {
                deck = defaultDeck;
            }
            //下面的初始顺序不能变
            this._encouterID = encouterId;
            //_unitsList = new List<Unit>();//放在这里为了每次从遭遇选择器切换地图后，清空之前的
            //_quickplat = new List<string>(deck._unitsWithQuickPlat);
            CardManager.Instance().LoadCardsIntoSets(deck, deck._unitsWithQuickPlat);
            //读取并存储遭遇
            EncouterData.Instance().InitEncounter(encouterId);
            //初始化地图
            BattleMap.BattleMap.Instance().InitAndInstantiateMapBlocks(encouterId);
            //初始战区事件
            EncouterData.Instance().InitBattleFieldEvent(encouterId);
            //初始战区状态，战区对象并添加事件模块进入仲裁器；
            BattleMap.BattleMap.Instance().battleAreaData.InitBattleArea(encouterId);
            //初始战斗地图上的单位 
            UnitManager.InitAndInstantiateGameUnit(encouterId);
            //该次遭遇中的一些临时数值
            EncouterData.Instance().dataOfThisBattle.InitData(encouterId);
            //设置回合为第一回合
            GamePlay.Gameplay.Instance().roundProcessController.SetFristRound();
            //一直显示战区所属
            BattleMap.BattleMap.Instance().drawBattleArea.ShowAndUpdateBattleArea();

            BattleMap.BattleMap.Instance().ScaleBattleMap();
        }

        private Mediator.Deck defaultDeck = new Mediator.Deck(new List<string>(), "HElf_1");

        /// <summary>
        /// 重新根据遭遇文件生成新的战斗地图
        /// </summary>
        /// <param name="encouterID"></param>
        public void RestatInitMap(string encouterID, Mediator.Deck deck)
        {
            if (deck == null)
            {
                deck = defaultDeck;
            }
            GamePlay.Gameplay.Instance().eventScroll.Clear();
            //初始一个遭遇id，供其他地方使用
            _encouterID = encouterID;
            //删除之前的地图
            BattleMap.BattleMap.Instance().DestroyMap();
            //重新生成
            InitMap(encouterID, deck);
        }

        /// <summary>
        /// 获取遭遇id
        /// </summary>
        /// <returns></returns>
        private string GetEncounterID()
        {
            if (SceneSwitchController.Instance().encounterId == null)//如果直接从战斗场景运行，默认初始一场遭遇
                return "planeshadow_1";
            Debug.Log("front id: " + SceneSwitchController.Instance().encounterId);
            string temp_id = SceneSwitchController.Instance().encounterId;
            string temp_id_front = temp_id.Split('_')[0];
            //if (temp_id_front == "sandworm")
            //    return "sandworm_1";
            //if (temp_id_front == "chomper")
            //    return "chomper_1";
            //if (temp_id_front == "Devil")
            //    return "Devil_1";
            if (temp_id == "hunter_3")
                return "hunter_2";
            if (temp_id == "dk_3")
                return "dk_2";
            return SceneSwitchController.Instance().encounterId;
            //return "Plain_Shadow_1";
        }


        private Mediator.Deck GetDeck()
        {
            return SceneSwitchController.Instance().deck;
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

            if(BattleMap.BattleMap.Instance().IsAtkColor == true)
            {
                GameGUI.ShowRange.Instance().CancleAttackRangeMark();
                gamePlayInput.InputFSM.PushState(new FSM.InputFSMAttackState(gamePlayInput.InputFSM));
            }
        }

        /// <summary>
        /// 当单位移动范围显示的时候，点击卡牌，取消移动范围显示,防止被箭头覆盖
        /// </summary>
        public void CancleRangeMark()
        {
            if (BattleMap.BattleMap.Instance().IsMoveColor == true)
            {
                BattleMap.BattleMap.Instance().IsMoveColor = false;
                GameGUI.ShowRange.Instance().CancleMoveRangeMark();
                gamePlayInput.InputFSM.PushState(new FSM.InputFSMIdleState(gamePlayInput.InputFSM));
            }

            if (BattleMap.BattleMap.Instance().IsAtkColor == true)
            {
                BattleMap.BattleMap.Instance().IsAtkColor = false;
                GameGUI.ShowRange.Instance().CancleAttackRangeMark();
                gamePlayInput.InputFSM.PushState(new FSM.InputFSMIdleState(gamePlayInput.InputFSM));
            }
        }
    }
}