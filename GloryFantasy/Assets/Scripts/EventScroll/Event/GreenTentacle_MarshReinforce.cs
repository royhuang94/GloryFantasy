using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GamePlay.Event
{
    //在来源单位周围部署2个深沼泥人（CR Y）。
    public class GreenTentacle_MarshReinforce : Event
    {
        public GreenTentacle_MarshReinforce()
        {
            //从数据库读取属性，id名不能错
            EventDataBase.Instance().GetEventProperty("GreenTentacle_MarshReinforce", this);
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
            switch (strenth)
            {
                case 1: SummonMonster_in_Unit_Around(this.Source, this.amount, "Ooze_1"); break;
                case 2: SummonMonster_in_Unit_Around(this.Source, this.amount, "Ooze_2"); break;
                case 3: SummonMonster_in_Unit_Around(this.Source, this.amount, "Ooze_3"); break;
                default: break;
            }


        }
    }
}