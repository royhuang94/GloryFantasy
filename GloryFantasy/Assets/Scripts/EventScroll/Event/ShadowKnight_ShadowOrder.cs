using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GamePlay.Event
{

    public class ShadowKnight_ShadowOrder : Event
    {
        public ShadowKnight_ShadowOrder()
        {
            //在来源单位周围部署Y个影之迅骑的复制，他们有一半最大生命值，且没有复活能力。
            //从数据库读取属性，id名不能错
            EventDataBase.Instance().GetEventProperty("ShadowKnight_ShadowOrder", this);
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
            SummonMonster_in_Unit_Around(this.Source, this.strenth, "ShadowKnight_2");  //暂时先采用这个方法，等待regenerat更改

        }
    }
}
