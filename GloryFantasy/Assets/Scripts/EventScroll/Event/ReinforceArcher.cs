using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GamePlay.Event
{
    //     增援：弓手之影
     //   "amount": 1,
     //   "effect": "在来源战区中随机部署X个弓手之影(CR Y)",
     //   "factor": "",
     //   "id": "ReinforceArcher",
     //   "name": "增援：弓手之影",
     //   "source_type": "战区",
     //   "strenth": 1,
     //   "type": "增援",
     //   "weight": 0
    public class ReinforceArcher : Event
    {
        public ReinforceArcher()
        {
            //从数据库读取属性，id名不能错
            EventDataBase.Instance().GetEventProperty("ReinforceArcher", this);
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
            //"在来源战区中随机部署X个弓手之影(CR Y)",

            int source_type_flag = this.Get_Source_Message();       //函数内部进行信息获取。返回值用于错误控制
            if (source_type_flag == 2)
            {
                //X次效果 最终值为读取的初始值与delta值的加和
                this.amount += delta_x_amount;
                //Y为效果强度 最终值为读取的初始值与delta值的加和
                this.strenth += delta_y_strenth;
                if (this.strenth > 3) this.strenth = 3;
                //根据X和Y的最终值决定召唤结果
                BattleMap.BattleArea _area = this.Source as BattleMap.BattleArea;
                if(_area._battleAreaSate == BattleMap.BattleAreaState.Enmey) //若此战区被敌人控制
                {
                    switch (strenth)
                    {
                        case 1: SummonMonster_in_Area(this.Source, this.amount, "WArcher_1"); break;
                        case 2: SummonMonster_in_Area(this.Source, this.amount, "WArcher_2"); break;
                        case 3: SummonMonster_in_Area(this.Source, this.amount, "WArcher_3"); break;
                        default: break;
                    }
                }

            }
            else
            {
                //output：：源错误
            }
        }

    }
}