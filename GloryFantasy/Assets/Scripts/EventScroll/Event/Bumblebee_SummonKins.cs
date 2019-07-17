using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GamePlay.Event
{
    //升级来源单位同一战区的迅雷毒蜂。随后在来源单位周围部署Y个迅雷毒蜂（CR 1）。
    public class Bumblebee_SummonKins : Event
    {
        public Bumblebee_SummonKins()
        {
            //从数据库读取属性，id名不能错
            EventDataBase.Instance().GetEventProperty("Bumblebee_SummonKins", this);
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
            //升级来源单位同一战区的迅雷毒蜂
            GameUnit.GameUnit _Unit = this.Source as GameUnit.GameUnit;
            List<GameUnit.GameUnit> Unit_in_Source_Area = GameplayToolExtend.getUnitsInRegion(GameplayToolExtend.GetRegion(_Unit));
            foreach (GameUnit.GameUnit unit in Unit_in_Source_Area)
            {
                //升级明细：
                if (unit.Name == "Bumblebee_1")
                {
                    GameUnit.UnitManager.Kill(null, this.Source as GameUnit.GameUnit);
                    GameUnit.GameUnit newUnit;
                    newUnit = this.Regenerate("Bumblebee_2", unit.mapBlockBelow,_Unit.owner);
                    this.Source = newUnit;
                }
                if (unit.Name == "Bumblebee_2")
                {
                    GameUnit.UnitManager.Kill(null, this.Source as GameUnit.GameUnit);
                    GameUnit.GameUnit newUnit;
                    newUnit = this.Regenerate("Bumblebee_3", unit.mapBlockBelow , _Unit.owner);
                    this.Source = newUnit;
                }


                //随后在来源单位周围部署Y个迅雷毒蜂（CR 1）。
                SummonMonster_in_Unit_Around(this.Source, this.strenth, "Bumblebee_1");

            }
        }

    }
}
