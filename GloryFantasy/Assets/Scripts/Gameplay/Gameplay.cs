using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Ability;
using Ability.Buff;
using IMessage;
using GamePlay;
using GameCard;

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
        public List<GameUnit.GameUnit> SummonUnit; //召唤的单位
        public PlayerEnum Drawer; //抓牌者
        public List<BaseCard> CaughtCard; //抓的牌
        public PlayerEnum HandAdder; //加手者
        public List<BaseCard> AddingCard; //加手的牌
        public GameUnit.GameUnit AbilitySpeller; //发动异能者

        #region ATK部分
        public Ability.Ability SpellingAbility; //发动的异能
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
    using GamePlay.Input;
    using Round;

    /// <summary>
    ///查询环境变量和控制游戏的方法类
    /// </summary>
    public interface GameplayTool
    {

    }

    public static class GameplayToolExtend
    {
        private static Info Info = Gameplay.Info;


        public static void SetRoundOwned(this GameplayTool self, PlayerEnum player)
        {
            Gameplay.Info.RoundOwned = player;
        }
        public static PlayerEnum GetRoundOwned(this GameplayTool self)
        {
            return Gameplay.Info.RoundOwned;
        }

        public static void SetCaster(this GameplayTool self, PlayerEnum player)
        {
            Gameplay.Info.Caster = player;
        }
        public static PlayerEnum GetCaster(this GameplayTool self)
        {
            return Gameplay.Info.Caster;
        }

        public static void SetCastingCard(this GameplayTool self, BaseCard card)
        {
            Info.CastingCard = card;
        }
        public static BaseCard GetCastingCard(this GameplayTool self)
        {
            return Info.CastingCard;
        }

        public static void SetSummonersController(this GameplayTool self, PlayerEnum player)
        {
            Info.SummonersController = player;
        }
        public static PlayerEnum GetSummonersController(this GameplayTool self)
        {
            return Info.SummonersController;
        }

        public static void SetSummonUnit(this GameplayTool self, List<GameUnit.GameUnit> units)
        {
            Info.SummonUnit = units;
        }
        public static List<GameUnit.GameUnit> GetSummonUnit(this GameplayTool self)
        {
            return Info.SummonUnit;
        }

        public static void SetDrawer(this GameplayTool self, PlayerEnum player)
        {
            Info.Drawer = player;
        }
        public static PlayerEnum GetDrawer(this GameplayTool self)
        {
            return Info.Drawer;
        }

        public static void SetCaughtCard(this GameplayTool self, List<BaseCard> cards)
        {
            Info.CaughtCard = cards;
        }
        public static List<BaseCard> GetCaughtCard(this GameplayTool self)
        {
            return Info.CaughtCard;
        }

        public static void SetHandAdder(this GameplayTool self, PlayerEnum player)
        {
            Info.HandAdder = player;
        }
        public static PlayerEnum GetHandAdder(this GameplayTool self)
        {
            return Info.HandAdder;
        }

        public static void SetAddingCard(this GameplayTool self, List<BaseCard> cards)
        {
            Info.AddingCard = cards;
        }
        public static List<BaseCard> GetAddingCard(this GameplayTool self)
        {
            return Info.AddingCard;
        }

        #region ATK部分
        /// <summary>
        /// 设置宣言攻击者
        /// </summary>
        /// <param name="self">GameplayTool 自身或者子类</param>
        /// <param name="unit">宣言攻击者</param>
        public static void SetAttacker(this GameplayTool self, GameUnit.GameUnit unit)
        {
            Gameplay.Info.Attacker = unit;
        }
        /// <summary>
        /// 获取攻击宣言者
        /// </summary>
        /// <param name="self">GameplayTool 自身或者子类</param>
        /// <returns></returns>
        public static GameUnit.GameUnit GetAttacker(this GameplayTool self)
        {
            return Gameplay.Info.Attacker;
        }
        /// <summary>
        /// 设置被攻击者
        /// </summary>
        /// <param name="self">GameplayTool 自身或者子类</param>
        /// <param name="unit">被攻击者</param>
        public static void SetAttackedUnit(this GameplayTool self, GameUnit.GameUnit unit)
        {
            Gameplay.Info.AttackedUnit = unit;
        }
        /// <summary>
        /// 获取被攻击者
        /// </summary>
        /// <param name="self">GameplayTool 自身或者子类</param>
        /// <returns></returns>
        public static GameUnit.GameUnit GetAttackedUnit(this GameplayTool self)
        {
            return Gameplay.Info.AttackedUnit;
        }
        #endregion

        public static void SetAbilitySpeller(this GameplayTool self, GameUnit.GameUnit speller)
        {
            Info.AbilitySpeller = speller;
        }
        /// <summary>
        /// 获得当前技能的发动者
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static GameUnit.GameUnit GetAbilitySpeller(this GameplayTool self)
        {
            return Info.AbilitySpeller;
        }
        /// <summary>
        /// 设置当前发动的技能
        /// </summary>
        /// <param name="self"></param>
        /// <param name="ability"></param>
        public static void SetSpellingAbility(this GameplayTool self, Ability.Ability ability)
        {
            Info.SpellingAbility = ability;
        }
        /// <summary>
        /// 获得当前发动的技能
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static Ability.Ability GetSpellingAbility(this GameplayTool self)
        {
            return Info.SpellingAbility;
        }

        #region ATK部分
        /// <summary>
        /// 设置伤害者
        /// </summary>
        /// <param name="self">GameplayTool 自身或者子类</param>
        /// <param name="unit">触发伤害者</param>
        public static void SetInjurer(this GameplayTool self, GameUnit.GameUnit unit)
        {
            Gameplay.Info.Injurer = unit;
        }
        /// <summary>
        /// 获取伤害者
        /// </summary>
        /// <param name="self">GameplayTool 自身或者子类</param>
        /// <returns></returns>
        public static GameUnit.GameUnit GetInjurer(this GameplayTool self)
        {
            return Info.Injurer;
        }
        /// <summary>
        /// 设置被伤害者
        /// </summary>
        /// <param name="self">GameplayTool 自身或者子类</param>
        /// <param name="unit">被触发伤害者</param>
        public static void SetInjuredUnit(this GameplayTool self, GameUnit.GameUnit unit)
        {
            Gameplay.Info.InjuredUnit = unit;
        }
        /// <summary>
        /// 获取被伤害者
        /// </summary>
        /// <param name="self">GameplayTool 自身或者子类</param>
        /// <returns></returns>
        public static GameUnit.GameUnit GetInjuredUnit(this GameplayTool self)
        {
            return Info.InjuredUnit;
        }
        /// <summary>
        /// 设置伤害数值大小
        /// </summary>
        /// <param name="self">GameplayTool 自身或者子类</param>
        /// <param name="damage">当前伤害数值</param>
        public static void SetDamage(this GameplayTool self, Damage damage)
        {
            Info.damage = damage;
        }
        /// <summary>
        /// 获取当前伤害数值
        /// </summary>
        /// <param name="self">GameplayTool 自身或者子类</param>
        /// <returns></returns>
        public static Damage GetDamage(this GameplayTool self)
        {
            return Info.damage;
        }
        /// <summary>
        /// 设置击杀者
        /// </summary>
        /// <param name="self">GameplayTool 自身或者子类</param>
        /// <param name="killer">击杀者</param>
        public static void SetKiller(this GameplayTool self, GameUnit.GameUnit killer)
        {
            Gameplay.Info.Killer = killer;
        }
        /// <summary>
        /// 获取击杀者
        /// </summary>
        /// <param name="self">GameplayTool 自身或者子类</param>
        /// <returns></returns>
        public static GameUnit.GameUnit GetKiller(this GameplayTool self)
        {
            return Info.Killer;
        }
        /// <summary>
        /// 设置被击杀者和死者
        /// </summary>
        /// <param name="self">GameplayTool 自身或者子类</param>
        /// <param name="killedUnit">被击杀者</param>
        public static void SetKilledAndDeadUnit(this GameplayTool self, GameUnit.GameUnit killedUnit)
        {
            Gameplay.Info.KilledUnit = killedUnit;
            Gameplay.Info.Dead = killedUnit;
        }
        /// <summary>
        /// 获取被击杀者
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static GameUnit.GameUnit GetKilledUnit(this GameplayTool self)
        {
            return Info.KilledUnit;
        }
        /// <summary>
        /// 获取死亡单位
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static GameUnit.GameUnit GetDead(this GameplayTool self)
        {
            return Info.Dead;
        }
        #endregion

        public static void SetSelectingUnit(this GameplayTool self, GameUnit.GameUnit unit)
        {
            Info.SelectingUnit = unit;
        }
        public static GameUnit.GameUnit GetSelectingUnit(this GameplayTool self)
        {
            return Gameplay.Info.SelectingUnit;
        }

        /// <summary>
        /// 复活某个单位
        /// </summary>
        /// <param name="name"></param>被复活单位的名字
        /// <param name="position"></param>单位被复活在哪个地形上
        /// <returns></returns>
        public static GameUnit.GameUnit Regenerate(this GameplayTool self, string name, Vector2 position)
        {
            //TODO:没写完的复活功能
            return null;
        }
        /// <summary>
        /// 获取某个单位的坐标
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        public static Vector2 GetUnitPosition(this GameplayTool self, GameUnit.GameUnit unit)
        {
            return BattleMap.BattleMap.Instance().GetUnitCoordinate(unit);
        }/// <summary>
         /// 删除某个单位的某个技能
         /// </summary>
         /// <param name="unit"></param>被删除技能的单位
         /// <param name="ability"></param>被删除的技能
        public static void DeleteUnitAbility(this GameplayTool self, GameUnit.GameUnit unit, string ability)
        {
            GameObject.Destroy(unit.GetComponent(ability));
        }
        /// <summary>
        /// 传入Monobehaviour脚本，返回该脚本所依附的GameUnit
        /// </summary>
        /// <param name="self"></param>
        /// <param name="ability"></param>
        /// <returns></returns>
        public static GameUnit.GameUnit GetUnit(this GameplayTool self, MonoBehaviour script)
        {
            return script.GetComponent<GameUnit.GameUnit>();
        }
        /// <summary>
        /// 传入Monobehaviour脚本，返回该脚本所依附的GameUnit的MsgReceiver
        /// </summary>
        /// <param name="self"></param>
        /// <param name="script"></param>
        /// <returns></returns>
        public static IMessage.MsgReceiver GetUnitReceiver(this GameplayTool self, MonoBehaviour script)
        {
            return script.GetComponent<GameUnit.GameUnit>().GetMsgReceiver();
        }
    }

    public class Gameplay : UnitySingleton<Gameplay>
    {

        public void Awake()
        {
            roundProcessController = new RoundProcessController();
            gamePlayInput = new GameplayInput();
            bmbColliderManager = new BMBColliderManager();
            buffManager = new BuffManager();

            _phaseNameText = GameObject.Find("phaseNameText").GetComponentInChildren<Text>();
            _phaseNameText.color = Color.red;
            _phaseNameText.text = roundProcessController.State.ToString();
        }

        public static Info Info = new Info();
        public RoundProcessController roundProcessController;
        public GameplayInput gamePlayInput;
        public BMBColliderManager bmbColliderManager;
        public BuffManager buffManager;

        private Text _phaseNameText;
        
        /// <summary>
        /// 提供给场景中阶段切换的按钮
        /// </summary>
        public void switchPhaseHandler()
        {
            roundProcessController.StepIntoNextState();
            _phaseNameText.text = roundProcessController.State.ToString();
        }
    }
}