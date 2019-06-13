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
                // 如果已有相同脚本，且其生命周期为永久，则不添加新buff
                if (rest < 0)
                    return target.GetComponent<T>();

                if (rest < life)
                    life = rest;
                GameObject.Destroy(target.GetComponent<T>());
            }
            T temp = target.AddComponent<T>();
            target.GetComponent<T>().SetLife(life);
            if (buffVariable != null)
            {
                target.GetComponent<T>().setVariable(buffVariable);
            }
            else
            {
                target.GetComponent<T>().InitialBuff();
            }
            return temp;
        }
        /// <summary>
        /// 获取战区内所有的单位。
        /// </summary>
        /// <param name="ba">战区</param>
        /// <returns>单位组成的List。</returns>
        public static List<GameUnit.GameUnit> getUnitsInRegion(BattleMap.BattleArea ba)
        {
            return ba._collider.disposeUnits;
        }
        /// <summary>
        /// 获取战区内所有的单位。
        /// </summary>
        /// <param name="regionID">战区ID</param>
        /// <returns>单位组成的List。</returns>
        public static List<GameUnit.GameUnit> getUnitsInRegion(int regionID)
        {
            BattleMap.BattleArea ba = BattleMap.BattleMap.Instance().battleAreaData.GetBattleAreaByID(regionID);
            if (ba == null)
                return null;
            return ba._collider.disposeUnits;
        }
        public static void SetChangedBA(this GameplayTool self, BattleMap.BattleArea _changedBA)
        {
            Gameplay.Info.changedBA = _changedBA;
        }
        public static BattleMap.BattleArea GetChangedBA(this GameplayTool self)
        {
            return Gameplay.Info.changedBA;
        }
        public static void SetNewOwner(this GameplayTool self, BattleMap.BattleAreaState _newOwner)
        {
            Gameplay.Info.newOwner = _newOwner;
        }
        public static BattleMap.BattleAreaState GetNewOwner(this GameplayTool self)
        {
            return Gameplay.Info.newOwner;
        }
        public static void SetExOwner(this GameplayTool self, BattleMap.BattleAreaState _exOwner)
        {
            Gameplay.Info.exOwner = _exOwner;
        }
        public static BattleMap.BattleAreaState GetExOwner(this GameplayTool self)
        {
            return Gameplay.Info.exOwner;
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
            if (Info.locking == false)
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
            if (Info.locking == false)
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
        /// <param name="name">重生为的单位id。</param>
        /// <param name="position">单位被复活在哪个地格上</param>
        /// <param name="Owner">单位所属</param>
        /// <returns></returns>
        public static GameUnit.GameUnit Regenerate(this GameplayTool self, string name, BattleMap.BattleMapBlock block, GameUnit.OwnerEnum Owner)
        {
            DispositionCommand unitDispose = new DispositionCommand(name, Owner, block);
            unitDispose.Excute();
            if (block.units_on_me.Count < 1)
                return null;
            GameUnit.GameUnit unit = block.units_on_me[0];
            if (unit.id != name)
                return null;
            return unit;
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
        /// <returns>插入事件的几种不同结果</returns>
        /// <summary>
        /// 该指令作用：创建一个 直接事件对象 并将它加入到事件系统中
        /// </summary>
        /// <param name="expect_trun">希望此事件在 expect_trun 回合生效</param>
        /// <param name="EventID">该事件的事件ID</param>
        /// <returns>插入事件的几种不同结果</returns>
        /// <param name="Source">该事件的事件源</param>
        public static int creatDirectEventToEventSystem(int expect_trun, string EventID, object Source)
        {
            int _turn;
            string _EventID;

            _turn = expect_trun;
            _EventID = EventID;
            GamePlay.Event.DirectEvent _DirectEvent;
            _DirectEvent = new GamePlay.Event.DirectEvent(EventID, expect_trun);    // 生成 直接事件对象 类
            _DirectEvent.Source = Source; //设置直接事件对象的源
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
        }
        /// <summary>
        /// 该指令作用：获取 当前回合数
        /// </summary>
        public static int getTurnNum()
        {
            return (Gameplay.Instance().eventScroll.nowBigestTurn - Gameplay.Instance().eventScroll.EventScrollCount + 1);//当前事件模块的最大触发回合数 - 事件队列个数 +1
        }
        /// <summary>
        /// 该指令作用：抽 amount 张卡
        /// </summary>
        /// <param name="amount">抽牌数量</param>
        public static void drawCards(int amount)
        {
            int i = amount;
            while (i >= 1)
            {
                IMessage.MsgDispatcher.SendMsg((int)IMessage.MessageType.DrawCard);     //发送抽卡消息
                i--;
            }

        }
        /// <summary>
        /// 指定中心块获得以其为中心的爆发区域。
        /// </summary>
        /// <param name="level">爆发区域等级</param>
        /// <param name="center">中心块地格</param>
        /// <returns>地格列表</returns>
        public static List<BattleMap.BattleMapBlock> getAreaByBlock(int level, BattleMap.BattleMapBlock center)
        {
            return getAreaByPos(level, new Vector2(center.position.x, center.position.y));
        }

        /// <summary>
        /// 指定中心块获得以其为中心的爆发区域。
        /// </summary>
        /// <param name="level">爆发区域等级</param>
        /// <param name="pos">中心块坐标</param>
        /// <returns>地格列表</returns>
        public static List<BattleMap.BattleMapBlock> getAreaByPos(int level, Vector2 pos)
        {
            if (level < 0)
                level = 0;
            if (level > 6)
                level = 6;

            return getBlocksByBound(pos, Area[level]);
        }
        /// <summary>
        /// 获取以pos为中心，偏移量约束为bound的地格列表。
        /// </summary>
        /// <param name="pos">中心坐标</param>
        /// <param name="bound">偏移量向量列表</param>
        /// <returns>地格列表</returns>
        public static List<BattleMap.BattleMapBlock> getBlocksByBound(Vector2 pos, List<Vector2> bound)
        {
            List<BattleMap.BattleMapBlock> res = new List<BattleMap.BattleMapBlock>();
            foreach (Vector2 v in bound)
            {
                res.Add(BattleMap.BattleMap.Instance().GetSpecificMapBlock(new Vector2(pos.x + v.x, pos.y + v.y)));
            }

            res.RemoveAll(it => it == null);
            return res;
        }
        /// <summary>
        /// 获取一个矩形区域，返回一个以左上角的点为中心的偏移量列表
        /// </summary>
        /// <param name="x_len">x轴跨度</param>
        /// <param name="y_len">y轴跨度</param>
        /// <returns>偏移量列表</returns>
        public static List<Vector2> generateSquare(int x_len, int y_len)
        {
            List<Vector2> res = new List<Vector2>();
            for (int i = (int)0; i < x_len; i++)
            {
                for (int j = (int)0; j < y_len; j++)
                {
                    res.Add(new Vector2(i, j));
                }
            }
            return res;
        }
        public static bool checkDeath(GameUnit.GameUnit unit)
        {
            return GameUnit.GameUnitPool.Instance().CheckDeath(unit);
        }
        /// <summary>
        /// 各等级的爆发区域。偏移量列表的列表。如Area[2]为2级爆发区域的偏移量列表。
        /// </summary>
        public static List<List<Vector2>> Area = new List<List<Vector2>>
        {
            new List<Vector2>
            {
                new Vector2(0, 0)
            },
            new List<Vector2>
            {
                new Vector2(0, 0), new Vector2(0, 1), new Vector2(0, -1), new Vector2(1, 0), new Vector2(-1, 0)
            },
            new List<Vector2>
            {
                new Vector2(0, 0), new Vector2(0, 1), new Vector2(0, -1), new Vector2(1, 0), new Vector2(-1, 0),
                new Vector2(1, 1), new Vector2(-1, -1), new Vector2(1, -1), new Vector2(-1, 1)
            },
            new List<Vector2>
            {
                new Vector2(0, 0), new Vector2(0, 1), new Vector2(0, -1), new Vector2(1, 0), new Vector2(-1, 0),
                new Vector2(0, 2), new Vector2(0, -2), new Vector2(2, 0), new Vector2(-2, 0),
                new Vector2(1, 1), new Vector2(-1, -1), new Vector2(1, -1), new Vector2(-1, 1)
            },
            new List<Vector2>
            {
                new Vector2(0, 0), new Vector2(0, 1), new Vector2(0, -1), new Vector2(1, 0), new Vector2(-1, 0),
                new Vector2(0, 2), new Vector2(0, -2), new Vector2(2, 0), new Vector2(-2, 0),
                new Vector2(1, 2), new Vector2(1, -2), new Vector2(2, 1), new Vector2(-2, 1),
                new Vector2(-1, 2), new Vector2(-1, -2), new Vector2(2, -1), new Vector2(-2, -1),
                new Vector2(1, 1), new Vector2(-1, -1), new Vector2(1, -1), new Vector2(-1, 1)
            },
            new List<Vector2>
            {
                new Vector2(0, 0), new Vector2(0, 1), new Vector2(0, -1), new Vector2(1, 0), new Vector2(-1, 0),
                new Vector2(0, 2), new Vector2(0, -2), new Vector2(2, 0), new Vector2(-2, 0),
                new Vector2(1, 2), new Vector2(1, -2), new Vector2(2, 1), new Vector2(-2, 1),
                new Vector2(-1, 2), new Vector2(-1, -2), new Vector2(2, -1), new Vector2(-2, -1),
                new Vector2(1, 1), new Vector2(-1, -1), new Vector2(1, -1), new Vector2(-1, 1),
                new Vector2(2, 2), new Vector2(-2, -2), new Vector2(2, -2), new Vector2(-2, 2)
            }
        };
        public static int distanceBetween(object a, object b)
        {
            Vector2 aPos = toVector2(a);
            Vector2 bPos = toVector2(b);

            return (int)(Mathf.Abs(aPos.x - bPos.y) + Mathf.Abs(aPos.y - bPos.y));
        }
        private static Vector2 toVector2(object a)
        {
            Vector2 aPos;
            if (a.GetType().ToString() == "GameUnit.GameUnit")
            {
                GameUnit.GameUnit aTemp = (GameUnit.GameUnit)a;
                aPos = new Vector2(aTemp.mapBlockBelow.position.x, aTemp.mapBlockBelow.position.y);
            }
            else if (a.GetType().ToString() == "BattleMap.BattleMapBlock")
            {
                BattleMap.BattleMapBlock aTemp = (BattleMap.BattleMapBlock)a;
                aPos = new Vector2(aTemp.position.x, aTemp.position.y);
            }
            else if (a.GetType().ToString() == "Vector2")
            {
                aPos = (Vector2)a;
            }
            else
                aPos = Vector2.zero;
            return aPos;
        }
        /// <summary>
        /// 该指令作用：设置一个 事件源 的X与Y属性（这两个参数影响事件强度）
        /// </summary>
        /// <param name="x_amount">设置该事件源的X属性</param>
        /// <param name="y_strength">设置该事件源的Y属性</param>
        /// <param name="Source">该事件源：object类型</param>
        public static void setUnitOrAReaXandY(int x_amount, int y_strength, ref object Source)
        {//!!!需要验证下在使用引用参数时会不会出现指针错误的情况，应该是没问题


            /*eg: GamePlay.Event.ReinforceArcher*/
            //根据选中的事件的string生成对应的类
            System.Type tempType = Source.GetType();
            if (tempType.ToString() == "GameUnit.GameUnit")     //若此 源 是一个单位
            {
                GameUnit.GameUnit Unit = Source as GameUnit.GameUnit;
                Unit.delta_x_amount = x_amount;
                Unit.delta_y_strenth = y_strength;
                Source = Unit;
            }
            if (tempType.ToString() == "BattleMap.BattleArea")   //若此 源 是一个战区
            {
                BattleMap.BattleArea battleArea = Source as BattleMap.BattleArea;
                battleArea.delta_x_amount = x_amount;
                battleArea.delta_y_strenth = y_strength;
                Source = battleArea;
            }
        }
        /// <summary>
        /// 移除重复元素
        /// </summary>
        /// <param name="reslist"></param>
        private static void RemoveRepeat(List<Vector2> reslist)
        {
            for (int i = 0; i < reslist.Count; i++)
            {
                for (int j = reslist.Count - 1; j > i; j--)
                {
                    if (reslist[i] == reslist[j])
                    {
                        reslist.RemoveAt(j);
                    }
                }
            }
        }
        /// <summary>
        /// 依照unitid减少冷却中的牌的cd。
        /// </summary>
        /// <param name="unitId"></param>
        /// <param name="amount"></param>
        public static void ReduceSpecificCardCd(string unitId, int amount)
        {
            CardManager.Instance().HandleCooldownEvent(unitId, amount);
        }

        /// <summary>
        /// 获取指定中心和range的所有格子。返回偏移量列表。
        /// </summary>
        /// <param name="cordinate"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        public static List<Vector2> GetBoundWithinRange(Vector2 center, int range)
        {

            List<Vector2> vector2s = GameGUI.ShowRange.Instance().GetSkillRnage(center, range);
            List<Vector2> res = new List<Vector2>();
            RemoveRepeat(vector2s);//去重
            foreach (Vector2 v in vector2s)
            {
                res.Add(new Vector2(v.x - center.x, v.y - center.y));
            }
            return res;
        }
        /// <summary>
        /// 获取指定地格的所属战区。
        /// </summary>
        /// <param name="pos">地格坐标</param>
        /// <returns></returns>
        public static int GetRegion(Vector2 pos)
        {
            //获取战区id
            BattleMap.BattleMapBlock _mapBlock = BattleMap.BattleMap.Instance().GetSpecificMapBlock(pos);
            return _mapBlock.area;
        }
        /// <summary>
        /// 获取指定地格的所属战区。
        /// </summary>
        /// <param name="block">地格</param>
        /// <returns></returns>
        public static int GetRegion(BattleMap.BattleMapBlock block)
        {
            return block.area;
        }
        /// <summary>
        /// 获取指定单位的所属战区。
        /// </summary>
        /// <param name="unit">单位</param>
        /// <returns></returns>
        public static int GetRegion(GameUnit.GameUnit unit)
        {
            return unit.mapBlockBelow.area;
        }
        /// <summary>
        /// 获取战区内所有的地格
        /// </summary>
        /// <param name="RegionId">战区id</param>
        /// <returns></returns>
        public static List<BattleMap.BattleMapBlock> GetBlocksInRegion(int RegionId)
        {
            BattleMap.BattleArea battleArea = BattleMap.BattleMap.Instance().battleAreaData.GetBattleAreaByID(RegionId);
            if (battleArea == null)
                return null;
            return GetBlocksInRegion(battleArea);
        }
        /// <summary>
        /// 获取战区内所有的地格
        /// </summary>
        /// <param name="ba">战区/param>
        /// <returns></returns>
        public static List<BattleMap.BattleMapBlock> GetBlocksInRegion(BattleMap.BattleArea ba)
        {
            List<BattleMap.BattleMapBlock> res = new List<BattleMap.BattleMapBlock>();
            foreach (Vector2 v in ba._battleAreas)
            {
                res.Add(BattleMap.BattleMap.Instance().GetSpecificMapBlock(v));
            }
            return res;
        }
        public static List<GameUnit.GameUnit> GetUnitsByBlocks(List<BattleMap.BattleMapBlock> blocks)
        {
            List<GameUnit.GameUnit> gameUnits = new List<GameUnit.GameUnit>();
            foreach (BattleMap.BattleMapBlock block in blocks)
            {
                GameUnit.GameUnit unit = BattleMap.BattleMap.Instance().GetUnitsOnMapBlock(new Vector2(block.position.x, block.position.y));
                if (unit != null)
                    gameUnits.Add(unit);
            }
            return gameUnits;
        }

        /// <summary>
        /// 索敌接口
        /// 返回值会返回NULL， 使用后请判断返回值是否为空以防止空异常
        /// </summary>
        /// <param name="gameUnit"></param>
        /// <param name="range"></param>
        /// <param name="attackMethod"></param>
        /// <returns>返回值会返回NULL， 使用后请判断返回值是否为空以防止空异常</returns>
        public static GameUnit.GameUnit GetAttackUnit(GameUnit.GameUnit gameUnit, int range, int attackMethod)
        {
            List<GameUnit.GameUnit> gameUnits;
            List<BattleMap.BattleMapBlock> battleMapBlocks = getBlocksByBound(gameUnit.CurPos, Area[range]); //获取 以单位为中心，range大小范围的地图块儿列表

            if(gameUnit.owner == GameUnit.OwnerEnum.Enemy)
                gameUnits = BattleMap.BattleMap.Instance().GetFriendlyUnitsList(); //获取我方所有单位
            else
                gameUnits = BattleMap.BattleMap.Instance().GetEnemyUnitsList(); //获取敌方所有单位

            //防止出现数组越界
            if (gameUnits.Count <= 0)//范围内没有敌对单位，事件触发失效
                return null;


            GameUnit.GameUnit tempUnit = gameUnits[0];
            //移除未包含再范围内的敌对单位，且排序为距离又近到远的顺序
            foreach (GameUnit.GameUnit unit in gameUnits)
            {
                if (!battleMapBlocks.Contains(BattleMap.BattleMap.Instance().GetSpecificMapBlock(gameUnit.CurPos)))
                {
                    gameUnits.Remove(unit);
                    continue;
                }

                int a = Mathf.Abs((int)unit.CurPos.x - (int)gameUnit.CurPos.x) + Mathf.Abs((int)unit.CurPos.y - (int)gameUnit.CurPos.y);
                int b = Mathf.Abs((int)gameUnit.CurPos.x - (int)tempUnit.CurPos.x) + Mathf.Abs((int)gameUnit.CurPos.y - (int)tempUnit.CurPos.y);
                if (a < b) //代表当前unit离gameunit更近
                {
                    gameUnits.Remove(unit);
                    gameUnits.Insert(0, unit);
                }
                tempUnit = unit;
            }


            switch (attackMethod)
            {
                case 0:
                    //TODO 如何搜寻近的敌人 根据距离判断我方最近
                    return gameUnits[0];
                case 1:
                    //TODO 如何搜寻远的敌人 根据距离判断我方最远
                    return gameUnits[gameUnits.Count - 1];
                default:
                    //TODO 如何随机的敌人，通过UnitList获取我方敌人随机
                    return gameUnits[Random.Range(0, gameUnits.Count - 1)];
            }
        }

        /// <summary>
        /// 索敌成功后的移动
        /// </summary>
        public static void MoveToTargetUnit(GameUnit.GameUnit gameUnit, GameUnit.GameUnit targetUnit)
        {
            //地图导航
            BattleMap.MapNavigator mapNavigator = BattleMap.BattleMap.Instance().MapNavigator;

            //获得A的周边MapBlock
            List<BattleMap.BattleMapBlock> neighbourBlocks = BattleMap.BattleMap.Instance().GetNeighbourBlock(BattleMap.BattleMap.Instance().GetSpecificMapBlock(gameUnit.CurPos));
            int prevPathCount = int.MaxValue;
            BattleMap.BattleMapBlock neighbourBlock = neighbourBlocks[0];
            
            foreach (BattleMap.BattleMapBlock battleMapBlock in neighbourBlocks)
            {
                if (mapNavigator.PathSearch(gameUnit.CurPos, battleMapBlock.position))
                {
                    //找到对于ai单位的最短路径
                    if (prevPathCount > mapNavigator.Paths.Count)
                    {
                        //更新最优路径
                        neighbourBlock = battleMapBlock;
                        prevPathCount = mapNavigator.Paths.Count;
                    }
                }
            }
            
            //创建移动命令
            UnitMoveCommand unitMove = new UnitMoveCommand(gameUnit, gameUnit.CurPos, neighbourBlock.position, Vector2.zero);
            unitMove.Excute();
        }


    }
}