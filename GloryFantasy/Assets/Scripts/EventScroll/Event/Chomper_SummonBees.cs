using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GamePlay.Event
{

    public class Chomper_SummonBees : Event
    {
        public Chomper_SummonBees()
        {
            //从数据库读取属性，id名不能错
            EventDataBase.Instance().GetEventProperty("Chomper_SummonBees", this);
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
            //"在来源单位周围部署Y个迅雷毒蜂（CR 1）。

            int source_type_flag = this.Get_Source_Message();       //函数内部进行信息获取。返回值用于错误控制
                                                                    //X次效果 最终值为读取的初始值与delta值的加和
            this.amount += delta_x_amount;
            //Y为效果强度 最终值为读取的初始值与delta值的加和
            this.strenth += delta_y_strenth;
            //在源单位的周围放置制定怪物
            SummonMonster_in_Unit_Around(this.Source, this.strenth, "Bumblebee_1");

        }
    }
}