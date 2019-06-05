using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GamePlay.Event
{

    public class Devil_DashSwing : Event
    {
        public Devil_DashSwing()
        {
            //在来源单位周围随机部署2个幽暗触手（CR=Y）
            //从数据库读取属性，id名不能错
            EventDataBase.Instance().GetEventProperty("Devil_DashSwing", this);
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
            //在源单位的周围放置制定怪物
            SummonMonster_in_Unit_Around(this.Source, this.strenth, "Tentacle_1");

        }
    }
}
