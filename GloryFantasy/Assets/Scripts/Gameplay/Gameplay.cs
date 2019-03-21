using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum TriggerType
{
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
    Move
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
///查询环境变量的方法类
/// </summary>
public class GameplayTool
{
    public GameplayTool()
    {
        Info = Gameplay.Info;
    }

    protected void SetRoundOwned(Player player)
    {
        Gameplay.Info.RoundOwned = player;
    }
    protected Player GetRoundOwned()
    {
        return Gameplay.Info.RoundOwned;
    }

    protected void SetCaster(Player player)
    {
        Gameplay.Info.Caster = player;
    }
    protected Player GetCaster()
    {
        return Gameplay.Info.Caster;
    }

    protected void SetCastingCard(Card card)
    {
        Info.CastingCard = card;
    }
    protected Card GetCastingCard()
    {
        return Info.CastingCard;
    }

    protected void SetSummonersController(Player player)
    {
        Info.SummonersController = player;
    }
    protected Player GetSummonersController()
    {
        return Info.SummonersController;
    }

    protected void SetSummonUnit(List<GameUnit.GameUnit> units)
    {
        Info.SummonUnit = units;
    }
    protected List<GameUnit.GameUnit> GetSummonUnit()
    {
        return Info.SummonUnit;
    }

    protected void SetDrawer(Player player)
    {
        Info.Drawer = player;
    }
    protected Player GetDrawer()
    {
        return Info.Drawer;
    }

    protected void SetCaughtCard(List<Card> cards)
    {
        Info.CaughtCard = cards;
    }
    protected List<Card> GetCaughtCard()
    {
        return Info.CaughtCard;
    }

    protected void SetHandAdder(Player player)
    {
        Info.HandAdder = player;
    }
    protected Player GetHandAdder()
    {
        return Info.HandAdder;
    }

    protected void SetAddingCard(List<Card> cards)
    {
        Info.AddingCard = cards;
    }
    protected List<Card> GetAddingCard()
    {
        return Info.AddingCard;
    }

    protected void SetAttacker(GameUnit.GameUnit unit)
    {
        Gameplay.Info.Attacker = unit;
    }
    protected GameUnit.GameUnit GetAttacker()
    {
        return Gameplay.Info.Attacker;
    }

    protected void SetAttackedUnit(GameUnit.GameUnit unit)
    {
        Gameplay.Info.AttackedUnit = unit;
    }
    protected GameUnit.GameUnit GetAttackedUnit()
    {
        return Gameplay.Info.AttackedUnit;
    }

    protected void SetAbilitySpeller(GameUnit.GameUnit speller)
    {
        Info.AbilitySpeller = speller;
    }
    protected GameUnit.GameUnit GetAbilitySpeller()
    {
        return Info.AbilitySpeller;
    }



    protected void SetInjurer(GameUnit.GameUnit unit)
    {
        Gameplay.Info.Injurer = unit;
    }
    protected void SetInjuredUnit(GameUnit.GameUnit unit)
    {
        Gameplay.Info.InjuredUnit = unit;
    }

    protected void SetKiller(GameUnit.GameUnit killer)
    {
        Gameplay.Info.Killer = killer;
    }
    protected void SetKilledAndDeadUnit(GameUnit.GameUnit killedUnit)
    {
        Gameplay.Info.KilledUnit = killedUnit;
        Gameplay.Info.Dead = killedUnit;
    }

    protected GameUnit.GameUnit GetSelectingUnit()
    {
        return Gameplay.Info.SelectingUnit;
    }

    private Info Info;
}


public class Gameplay
{
    public static Info Info;
    public RoundProcessController roundProcessController;

    public void HandleInput() { }
}
