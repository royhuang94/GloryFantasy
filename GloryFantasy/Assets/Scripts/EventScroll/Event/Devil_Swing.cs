using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GamePlay.Event
{

    public class Devil_Swing : Event
    {
        public Devil_Swing()
        {
            //在来源单位周围随机部署2个幽暗触手（CR=Y）
            //从数据库读取属性，id名不能错
            EventDataBase.Instance().GetEventProperty("Devil_Swing", this);
            //实例化该事件的 触发条件 和 效果
            this.Condition = selfCondition;
            this.Action = selfAction;
        }

        bool selfCondition()
        {
            //do nothing

            return true;
        }

        void selfAction()
        {

            this.amount += delta_x_amount;
            //Y为效果强度 最终值为读取的初始值与delta值的加和
            this.strenth += delta_y_strenth;

            GameUnit.GameUnit _Unit = this.Source as GameUnit.GameUnit;
            int _hp = _Unit.hp;
            GameUnit.GameUnit targetUnit = GameplayToolExtend.GetAttackUnit(_Unit, 4, UnityEngine.Random.Range(0, 2));
            GameplayToolExtend.MoveToTargetUnit(_Unit, targetUnit);

            Damage This_Damage = new Damage(7);
            Damage.TakeDamage(targetUnit, This_Damage);   //使得敌方单位收到Y点伤害
            //targetUnit.gameObject.AddBuff<Ability.Debuff.BDizziness>(2f);   //使得此单位获得 眩晕 ,2回合后 消解

        }
    }
}
