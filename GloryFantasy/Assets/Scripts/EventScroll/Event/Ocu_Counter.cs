using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GamePlay.Event
{

    public class Ocu_Counter : Event
    {
        public Ocu_Counter()
        {
            //从数据库读取属性，id名不能错
            EventDataBase.Instance().GetEventProperty("Ocu_Counter", this);
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
            //"敌方事件部署量+1，强度+1。",

            int source_type_flag = this.Get_Source_Message();       //函数内部进行信息获取。返回值用于错误控制
                                                                    //X次效果 最终值为读取的初始值与delta值的加和
            this.amount += delta_x_amount;
            //Y为效果强度 最终值为读取的初始值与delta值的加和
            this.strenth += delta_y_strenth;

            foreach (GameUnit.GameUnit unit in BattleMap.BattleMap.Instance().UnitsList) //从地图获取单位列表
            {
                if (unit.owner == GameUnit.OwnerEnum.Enemy) //若该单位拥有者为敌方则x与y属性+1
                {
                    unit.delta_x_amount = unit.delta_x_amount + 1;
                    unit.delta_y_strenth = unit.delta_y_strenth + 1;
                }
            }

        }
    }
}
