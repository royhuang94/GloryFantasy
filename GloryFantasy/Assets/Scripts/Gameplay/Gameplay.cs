using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum TriggerType
{
    UpdateSource,
    BP,
    MPBegin,
    MPEnd,
    EP,
    CastCard,
    Summon,
    DrawCard,
    AddInHand,
    AnnounceAttack,
    ActiveAbility,
    BeAttacked,
    Damage,
    BeDamaged,
    Kill,
    BeKilled,
    Dead,
    ToBeKilled,
    Move,
    UnitMoving
};

public class Info
{
    public Player RoundOwned; //回合所属
    public Player Caster; //施放者
    public Card CastingCard; //施放的牌
    public Player SummonersController; //召唤单位的控制者
    public List<GameUnit.GameUnit> SummonUnit; //召唤的单位
    public Player Drawer; //抓牌者
    public List<Card> CaughtCard; //抓的牌
    public Player HandAdder; //加手者
    public List<Card> AddingCard; //加手的牌
    public GameUnit.GameUnit Attacker; //宣言攻击者
    public GameUnit.GameUnit AttackedUnit; //被攻击者
    public GameUnit.GameUnit AbilitySpeller; //发动异能者
    public Ability SpellingAbility; //发动的异能
    public GameUnit.GameUnit Injurer; //伤害者
    public GameUnit.GameUnit InjuredUnit; //被伤害者
    public Damage damage; //伤害
    public GameUnit.GameUnit Killer; //击杀者
    public GameUnit.GameUnit KilledUnit; //被杀者
    public GameUnit.GameUnit Dead; //死者

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

/// <summary>
///查询环境变量和控制游戏的方法类
/// </summary>
public interface GameplayTool
{

}

public static class GameplayToolExtend
{
    private static Info Info = Gameplay.Info;


    public static void SetRoundOwned(this GameplayTool self, Player player)
    {
        Gameplay.Info.RoundOwned = player;
    }
    public static Player GetRoundOwned(this GameplayTool self)
    {
        return Gameplay.Info.RoundOwned;
    }

    public static void SetCaster(this GameplayTool self, Player player)
    {
        Gameplay.Info.Caster = player;
    }
    public static Player GetCaster(this GameplayTool self)
    {
        return Gameplay.Info.Caster;
    }

    public static void SetCastingCard(this GameplayTool self, Card card)
    {
        Info.CastingCard = card;
    }
    public static Card GetCastingCard(this GameplayTool self)
    {
        return Info.CastingCard;
    }

    public static void SetSummonersController(this GameplayTool self, Player player)
    {
        Info.SummonersController = player;
    }
    public static Player GetSummonersController(this GameplayTool self)
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

    public static void SetDrawer(this GameplayTool self, Player player)
    {
        Info.Drawer = player;
    }
    public static Player GetDrawer(this GameplayTool self)
    {
        return Info.Drawer;
    }

    public static void SetCaughtCard(this GameplayTool self, List<Card> cards)
    {
        Info.CaughtCard = cards;
    }
    public static List<Card> GetCaughtCard(this GameplayTool self)
    {
        return Info.CaughtCard;
    }

    public static void SetHandAdder(this GameplayTool self, Player player)
    {
        Info.HandAdder = player;
    }
    public static Player GetHandAdder(this GameplayTool self)
    {
        return Info.HandAdder;
    }

    public static void SetAddingCard(this GameplayTool self, List<Card> cards)
    {
        Info.AddingCard = cards;
    }
    public static List<Card> GetAddingCard(this GameplayTool self)
    {
        return Info.AddingCard;
    }

    public static void SetAttacker(this GameplayTool self, GameUnit.GameUnit unit)
    {
        Gameplay.Info.Attacker = unit;
    }
    public static GameUnit.GameUnit GetAttacker(this GameplayTool self)
    {
        return Gameplay.Info.Attacker;
    }

    public static void SetAttackedUnit(this GameplayTool self, GameUnit.GameUnit unit)
    {
        Gameplay.Info.AttackedUnit = unit;
    }
    public static GameUnit.GameUnit GetAttackedUnit(this GameplayTool self)
    {
        return Gameplay.Info.AttackedUnit;
    }

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
    public static void SetSpellingAbility(this GameplayTool self, Ability ability)
    {
        Info.SpellingAbility = ability;
    }
    /// <summary>
    /// 获得当前发动的技能
    /// </summary>
    /// <param name="self"></param>
    /// <returns></returns>
    public static Ability GetSpellingAbility(this GameplayTool self)
    {
        return Info.SpellingAbility;
    }

    public static void SetInjurer(this GameplayTool self, GameUnit.GameUnit unit)
    {
        Gameplay.Info.Injurer = unit;
    }
    public static GameUnit.GameUnit GetInjurer(this GameplayTool self)
    {
        return Info.Injurer;
    }

    public static void SetInjuredUnit(this GameplayTool self, GameUnit.GameUnit unit)
    {
        Gameplay.Info.InjuredUnit = unit;
    }
    public static GameUnit.GameUnit GetInjuredUnit(this GameplayTool self)
    {
        return Info.InjuredUnit;
    }

    public static void SetDamage(this GameplayTool self, Damage damage)
    {
        Info.damage = damage;
    }
    public static Damage GetDamage(this GameplayTool self)
    {
        return Info.damage;
    }

    public static void SetKiller(this GameplayTool self, GameUnit.GameUnit killer)
    {
        Gameplay.Info.Killer = killer;
    }
    public static GameUnit.GameUnit GetKiller(this GameplayTool self)
    {
        return Info.Killer;
    }

    public static void SetKilledAndDeadUnit(this GameplayTool self, GameUnit.GameUnit killedUnit)
    {
        Gameplay.Info.KilledUnit = killedUnit;
        Gameplay.Info.Dead = killedUnit;
    }
    public static GameUnit.GameUnit GetKilledUnit(this GameplayTool self)
    {
        return Info.KilledUnit;
    }
    public static GameUnit.GameUnit GetDead(this GameplayTool self)
    {
        return Info.Dead;
    }

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
        return BattleMap.BattleMap.getInstance().GetUnitCoordinate(unit);
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
    /// 传入Ability，返回该技能所在的GameUnit
    /// </summary>
    /// <param name="self"></param>
    /// <param name="ability"></param>
    /// <returns></returns>
    public static GameUnit.GameUnit GetAbilitysUnit(this GameplayTool self, Ability ability)
    {
        return ability.GetComponent<GameUnit.GameUnit>();
    }
}

public static class testClassExtend
{
    public static void testFunction(this string self)
    {

    }
}

public class Gameplay : MonoBehaviour
{
    private Gameplay() { }
    private static Gameplay instance = null;

    public static Gameplay GetInstance()
    {
        if (instance == null)
        {
            instance = GameObject.FindObjectOfType(typeof(Gameplay)) as Gameplay;
        }
        return instance;
    }

    public void Awake()
    {
        roundProcessController = new RoundProcessController();
        gamePlayInput = new GameplayInput();
    }

    public static Info Info = new Info();
    public RoundProcessController roundProcessController;
    public GameplayInput gamePlayInput;
}
