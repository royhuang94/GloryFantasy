using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GamePlay.Event
{

    public class Hostling : Event
    {
        public Hostling()
        {
            //从数据库读取属性，id名不能错
            EventDataBase.Instance().GetEventProperty("Hostling", this);
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
            //"我方立即抽一张牌。",

            int source_type_flag = this.Get_Source_Message();       //函数内部进行信息获取。返回值用于错误控制
                                                                    //X次效果 最终值为读取的初始值与delta值的加和
            this.amount += delta_x_amount;
            //Y为效果强度 最终值为读取的初始值与delta值的加和
            this.strenth += delta_y_strenth;
            IMessage.MsgDispatcher.SendMsg((int)IMessage.MessageType.DrawCard);     //发送抽卡指令

        }
    }
}