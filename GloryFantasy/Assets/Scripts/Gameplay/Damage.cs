using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using IMessage;
using GamePlay;
using GameUnit;

namespace GamePlay
{

    /// <summary>
    /// 伤害类，存储伤害的信息和伤害公式
    /// </summary>
    public class Damage : GameplayTool
    {

        public int damageValue { get; set; }

        public Damage(int damageValue)
        {
            this.damageValue = damageValue;
        }

        /// <summary>
        /// 获取Damage伤害
        /// </summary>
        /// <param name="unit">当前攻击单位</param>
        /// <returns></returns>
        public static Damage GetDamage(GameUnit.GameUnit unit)
        {
            Damage damage = new Damage(unit.getATK());
            return damage;
        }

        /// <summary>
        /// 单位承受伤害。
        /// </summary>
        /// <param name="unit">承受伤害的单位。</param>
        /// <param name="damage">伤害</param>
        public static void TakeDamage(GameUnit.GameUnit unit, Damage damage)
        {
            Debug.Log(damage.damageValue);
            unit.hp -= damage.damageValue;
            
            // 更新unit的血量
            Gameplay.Instance().gamePlayInput.UpdateHp(unit);
            
            Debug.Log(unit.name + "收到伤害，当前剩余生命值" + unit.hp);
        }

        /// <summary>
        /// 造成伤害的方法。
        /// </summary>
        /// <param name="source">伤害来源单位。可以为空（eg 烧灼地形造成的伤害）。</param>
        /// <param name="taker">伤害承受者。</param>
        /// <param name="damage">伤害。</param>
        public static void DealDamage(GameUnit.GameUnit source, GameUnit.GameUnit taker, Damage damage)
        {
            Damage.TakeDamage(taker, damage);
            damage.SetInjurer(source); damage.SetInjuredUnit(taker);
            if (source != null)
            {
                MsgDispatcher.SendMsg((int)MessageType.Damage);
                Gameplay.Instance().autoController.RecordedHatred(source, taker);
            }
            MsgDispatcher.SendMsg((int)MessageType.BeDamaged);
            if (taker.IsDead())
            {
                UnitManager.Kill(source, taker);
            }
        }
    }

    //继承Command的基类是为了能够使用Command里的方法
    //其实如果把command的方法写成 功能类会更靠谱，因为实际上和Command没有逻辑上的继承关系啊我日
    //所以我把Command的方法提了出来写成了GameplayTool，Command继承GamePlay,依然是采用继承的方式才能调用，因为真的不想增加太多的全局量
    /// <summary>
    /// 伤害请求，一个伤害请求代表一次伤害
    /// </summary>
    public class DamageRequest : GameplayTool
    {
        /// <summary>
        /// 带参构造函数
        /// </summary>
        /// <param name="attacker">攻击者</param>
        /// <param name="attackedUnit">被攻击单位</param>
        /// <param name="priority">优先级</param>
        public DamageRequest(GameUnit.GameUnit attacker, GameUnit.GameUnit attackedUnit, int priority)
        {
            _attacker = attacker;
            _attackedUnit = attackedUnit;
            this.priority = priority;
        }

        /// <summary>
        /// 根据攻击者和被攻击的攻击优先级列表生成对应的伤害请求list
        /// </summary>
        /// <param name="DamageRequestList">伤害请求list</param>
        /// <param name="Attacker">攻击者</param>
        /// <param name="AttackedUnit">被攻击者</param>
        public static List<DamageRequest> CaculateDamageRequestList(GameUnit.GameUnit Attacker, GameUnit.GameUnit AttackedUnit)
        {
            List<DamageRequest> damageRequestList = new List<DamageRequest>();
            Debug.Log(Attacker.priority);

            for (int i = 0; i < AttackedUnit.priority.Count; i++)
            {
                damageRequestList.Add(new DamageRequest(AttackedUnit, Attacker, AttackedUnit.priority[i]));
            }
            for (int i = 0; i < Attacker.priority.Count; i++)
            {
                damageRequestList.Add(new DamageRequest(Attacker, AttackedUnit, Attacker.priority[i]));
            }
            damageRequestList.Sort((a, b) =>
            {
                if (a.priority < b.priority)
                    return 1;
                else if (a.priority == b.priority)
                    return 0;
                else
                    return -1;
            });
            return damageRequestList;
        }

        /// <summary>
        /// 单次伤害请求计算
        /// </summary>
        public void Excute()
        {
            Damage _damage = Damage.GetDamage(_attacker);
            Damage.DealDamage(_attacker, _attackedUnit, _damage);
        }

        /// <summary>
        /// 如果伤害请求优先级相同，则伤害判定流程会特殊一些
        /// </summary>
        public void ExcuteSameTime()
        {
            //CheckWhosTurn(_attacker, _attackedUnit);

            Damage.TakeDamage(_attackedUnit, Damage.GetDamage(_attacker));
            this.SetInjurer(_attackedUnit); this.SetInjuredUnit(_attacker);
            MsgDispatcher.SendMsg((int)MessageType.Damage);
            MsgDispatcher.SendMsg((int)MessageType.BeDamaged);

            Damage.TakeDamage(_attacker, Damage.GetDamage(_attackedUnit));
            this.SetInjurer(_attacker); this.SetInjuredUnit(_attackedUnit);
            MsgDispatcher.SendMsg((int)MessageType.Damage);
            MsgDispatcher.SendMsg((int)MessageType.BeDamaged);

            //两次只有attacker是player的时候触发记录
            Gameplay.Instance().autoController.RecordedHatred(_attacker, _attackedUnit);
            Gameplay.Instance().autoController.RecordedHatred(_attackedUnit, _attacker);
            if (_attacker.IsDead())
            {
                UnitManager.Kill(_attackedUnit, _attacker);
            }
            else
            {
                Gameplay.Instance().gamePlayInput.UpdateHp(_attacker);
                
            }

            if (_attackedUnit.IsDead())
            {
                UnitManager.Kill(_attacker, _attackedUnit);
            }
            else
            {
                Gameplay.Instance().gamePlayInput.UpdateHp(_attackedUnit);
            }
            
        }

        //private Damage _damage;
        public GameUnit.GameUnit _attacker;
        public GameUnit.GameUnit _attackedUnit;
        public int priority;
    }
}