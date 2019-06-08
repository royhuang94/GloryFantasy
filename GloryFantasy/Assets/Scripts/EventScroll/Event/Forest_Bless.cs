using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GamePlay.Event
{
    //此区域的单位获得4点治疗。
    public class Forest_Bless : Event
    {
        public Forest_Bless()
        {
            //从数据库读取属性，id名不能错
            EventDataBase.Instance().GetEventProperty("Forest_Bless", this);
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

            List<GameUnit.GameUnit> Unit_in_Source_Area = GameplayToolExtend.getUnitsInRegion(this.Source as BattleMap.BattleArea);
            foreach (GameUnit.GameUnit unit in Unit_in_Source_Area)
            {
                unit.changeHP(4);   //治疗 4点生命值
            }


        }
    }
}
