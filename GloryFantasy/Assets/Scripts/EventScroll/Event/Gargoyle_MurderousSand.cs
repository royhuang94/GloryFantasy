using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GamePlay.Event
{
    //来源单位所在战区的敌方单位受到Y点伤害并获得滞击，2回合后消解。
    public class Gargoyle_MurderousSand : Event
    {
        public Gargoyle_MurderousSand()
        {
            //从数据库读取属性，id名不能错
            EventDataBase.Instance().GetEventProperty("Gargoyle_MurderousSand", this);
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
            List<GameUnit.GameUnit> Unit_in_Source_Area = GameplayToolExtend.getUnitsInRegion(GameplayToolExtend.GetRegion(_Unit));
            foreach (GameUnit.GameUnit unit in Unit_in_Source_Area)
            {
                if(unit.owner != _Unit.owner)   //使得敌方单位 受到Y点伤害并获得滞击，2回合后消解。
                {
                    Damage This_Damage = new Damage(this.strenth);
                    //Damage.TakeDamage(unit, This_Damage);   //使得敌方单位收到Y点伤害
                   // unit.gameObject.AddBuff<Ability.Debuff.BDisarm>(2f);   //使得此单位获得 滞击 ,2回合后 消解
                }
            }


        }
    }
}
