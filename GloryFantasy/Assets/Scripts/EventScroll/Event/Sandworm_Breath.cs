using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GamePlay.Event
{

    public class Sandworm_Breath : Event
    {
        public Sandworm_Breath()
        {
            //在来源单位周围随机部署2个幽暗触手（CR=Y）
            //从数据库读取属性，id名不能错
            EventDataBase.Instance().GetEventProperty("Sandworm_Breath", this);
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

            int source_type_flag = this.Get_Source_Message();       //函数内部进行信息获取。返回值用于错误控制
                                                                    //X次效果 最终值为读取的初始值与delta值的加和
            this.amount += delta_x_amount;
            //Y为效果强度 最终值为读取的初始值与delta值的加和
            this.strenth += delta_y_strenth;

            GameUnit.GameUnit _Unit = this.Source as GameUnit.GameUnit;
            int _hp = _Unit.hp;
            GameUnit.GameUnit targetUnit = GameplayToolExtend.GetAttackUnit(_Unit, 4, UnityEngine.Random.Range(0, 2));
            GameplayToolExtend.MoveToTargetUnit(_Unit, targetUnit);

            List<BattleMap.BattleMapBlock> battleMapBlocks = GameplayToolExtend.getAreaByBlock(1, BattleMap.BattleMap.Instance().GetSpecificMapBlock(_Unit.CurPos));
            Damage This_Damage = new Damage(this.strenth);
            foreach (BattleMap.BattleMapBlock block in battleMapBlocks)
            {
                if(block.units_on_me.Count > 0)
                {
                    Damage.TakeDamage(block.units_on_me[0], This_Damage);   //使得敌方单位收到Y点伤害
                }

                //block.gameObject.AddBuff<Ability.Debuff.BFiring>(2f);   //使得此地图块儿 灼烧 ,2回合后 消解
            }
        }
    }
}
