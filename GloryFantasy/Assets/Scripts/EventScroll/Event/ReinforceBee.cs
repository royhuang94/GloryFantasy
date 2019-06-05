using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GamePlay.Event
{

    public class ReinforceBee : Event
    {
        public ReinforceBee()
        {
            //在来源战区随机部署X个迅雷毒蜂（CR Y）
            //从数据库读取属性，id名不能错
            EventDataBase.Instance().GetEventProperty("ReinforceBee", this);
            //实例化该事件的 触发条件 和 效果
            this.Condition = selfCondition;
            this.Action = selfAction;

            //
        }

        bool selfCondition()
        {
            //do nothing

            return true;
        }

        void selfAction()
        {
            int source_type_flag = this.Get_Source_Message();       //函数内部进行信息获取。返回值用于错误控制
            if (source_type_flag == 2)
            {
                //X次效果 最终值为读取的初始值与delta值的加和
                this.amount += delta_x_amount;
                //Y为效果强度 最终值为读取的初始值与delta值的加和
                this.strenth += delta_y_strenth;
                if (this.strenth > 3) this.strenth = 3;
                //根据X和Y的最终值决定召唤结果
                switch (strenth)
                {
                    case 1: SummonMonster_in_Area(this.Source, this.amount, "Bumblebee_1"); break;
                    case 2: SummonMonster_in_Area(this.Source, this.amount, "Bumblebee_2"); break;
                    case 3: SummonMonster_in_Area(this.Source, this.amount, "Bumblebee_3"); break;
                    default: break;
                }
            }
            else
            {
                //output：：源错误
            }
        }

    }
}