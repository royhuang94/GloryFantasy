using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Ability;
using Ability.Buff;
using IMessage;
using GameCard;

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
        public static T AddBuff<T>(this GameObject gameObject, float life, AbilityVariable buffVariable = null) where T : Buff
        {
            return addBuff<T>(gameObject, life);
        }

        public static T AddBuff<T>(this Transform transform, float life, AbilityVariable buffVariable = null) where T : Buff
        {
            return addBuff<T>(transform.gameObject, life);
        }
        /// <summary>
        /// 添加buff。参数life不可缺省，buff的持续时间；参数buffVariable可缺省，同异能改变量。
        /// </summary>
        private static T addBuff<T>(GameObject target, float life, AbilityVariable buffVariable = null) where T : Buff
        {
            if (target.GetComponent<T>() != null)
            {
                float rest = target.GetComponent<T>().Life;
                if (rest < life)
                    life = rest;
                GameObject.Destroy(target.GetComponent<T>());
            }
            T temp = target.AddComponent<T>();
            target.GetComponent<T>().SetLife(life);
            if (buffVariable != null)
            {
                target.GetComponent<T>().setVariable(buffVariable);
            } else
            {
                target.GetComponent<T>().InitialBuff();
            }
            return temp;
        }
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


        public static void RelaseLocking(this GameplayTool self)
        {
            Info.locking = false;
            Info.movingUnit = Info.otherMovingUnit;
        }

        /// <summary>
        /// 设置正在移动的单位
        /// </summary>
        /// <param name="self"></param>
        /// <param name="gameUnit"></param>
        public static void SetMovingUnit(this GameplayTool self, GameUnit.GameUnit gameUnit)
        {
            if(Info.locking == false)
            {
                Info.movingUnit = gameUnit;
                Info.locking = true;
            }
            else
            {
                Info.otherMovingUnit = gameUnit;
            }

        }

        /// <summary>
        /// 获取正在移动的单位
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static GameUnit.GameUnit GetMovingUnit(this GameplayTool self)
        {
            //TODO 实现完整的加锁机制
            if(Info.locking == false)
                return Info.otherMovingUnit;
            return Info.movingUnit;
        }

        //public static void SetSelectingUnit(this GameplayTool self, GameUnit.GameUnit unit)
        //{
        //    Info.SelectingUnit = unit;
        //}
        /// <summary>
        /// 获取指令牌发动选择的目标List，对象类型需要自行强转GameUnit或者BattleMapBlock
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static List<object> GetSelectingUnits(this GameplayTool self)
        {

            return Gameplay.Instance().gamePlayInput.SelectingList;
        }

        /// <summary>
        /// 造成伤害的方法。
        /// </summary>
        /// <param name="source">伤害来源单位。可以为空（eg 烧灼地形造成的伤害）。</param>
        /// <param name="taker">伤害承受者。</param>
        /// <param name="damage">伤害。</param>
        public static void DealDamage(GameUnit.GameUnit source, GameUnit.GameUnit taker, Damage damage)
        {
            Damage.DealDamage(source, taker, damage);
        }

        /// <summary>
        /// 重生为某个单位。
        /// </summary>
        /// <param name="name"></param>重生为的单位id。
        /// <param name="position"></param>单位被复活在哪个地格上
        /// <returns></returns>
        public static GameUnit.GameUnit Regenerate(this GameplayTool self, string name, BattleMap.BattleMapBlock block)
        {
            DispositionCommand unitDispose = new DispositionCommand(name, GameUnit.OwnerEnum.Player, block);
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
        }
        /// <summary>
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
        /// <summary>
        /// 传入Monobehaviour脚本，返回该脚本所依附的BaseCard的MsgReceiver
        /// </summary>
        /// <param name="self"></param>
        /// <param name="script"></param>
        /// <returns></returns>
        public static IMessage.MsgReceiver GetCardReceiver(this GameplayTool self, MonoBehaviour script)
        {
            return script.GetComponent<BaseCard>().GetMsgReceiver();
        }

        /// <summary>
        /// 该指令作用：创建一个 直接事件对象 并将它加入到事件系统中
        /// </summary>
        /// <param name="expect_trun">希望此事件在 expect_trun 回合生效</param>
        /// <param name="EventID">该事件的事件ID</param>
        public static int Creat_DirectEvent_to_EventSystem(int expect_trun, string EventID)
        {   //return 值代表插入事件的几种不同结果
            int _turn;
            string _EventID;

            _turn = expect_trun;
            _EventID = EventID;
            GamePlay.Event.DirectEvent _DirectEvent;
            _DirectEvent = new GamePlay.Event.DirectEvent(EventID, expect_trun);    // 生成 直接事件对象 类
            //—————————————以下部分为具体操作执行
            int now_biggest_turn = Gameplay.Instance().eventScroll.nowBigestTurn;   //获取事件轴的最大回合数
                                                                                    //
            int delta_turn = now_biggest_turn - _turn;
            if (delta_turn <= Gameplay.Instance().eventScroll.EventScrollCount - 1 && delta_turn >= 0)  //期望插入的回合已经抽象出了事件队列::此情况下该 直接事件 直接入轴
            {
                Gameplay.Instance().eventScroll.AddDirectEvent_to_Scroll(_DirectEvent);//封装好的：插入 事件轴 函数
                return 1;
            }
            else if (delta_turn < 0)                                                                  //期望插入的回合暂未抽象出事件队列::此情况下 直接事件 进入仲裁器中 直接事件队列 进行等待
            {
                Gameplay.Instance().eventScroll.AddDirectEvent_to_Judge(_DirectEvent);//封装好的：插入 直接事件队列 函数
                return 2;
            }
            else                                                                                       //这种情况是 想要插入已经“过期的事件”导致的 如在最大回合数为100的事件轴中插入第2回合触发的事件
            {
                //todo:加入错误提示
                return 0;
            }
            {

            }
        }

    }
}
