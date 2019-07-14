using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GamePlay.Event
{
    //此区域的单位获得目盲，一回合后消解。
    public class Desert_Storm : Event
    {
        public Desert_Storm()
        {
            //从数据库读取属性，id名不能错
            EventDataBase.Instance().GetEventProperty("Desert_Storm", this);
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
                //unit.gameObject.AddBuff<Ability.Debuff.BBlind>(1f);   //使得此单位获得 目盲 ,1回合后 消解
            }


        }
    }
}
