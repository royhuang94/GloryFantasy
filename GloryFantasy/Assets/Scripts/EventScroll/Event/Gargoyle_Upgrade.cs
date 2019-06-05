using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GamePlay.Event
{
    //来源单位提升为高一级的形态（保留他受到的伤害）。
    public class Gargoyle_Upgrade : Event
    {
        public Gargoyle_Upgrade()
        {
            //从数据库读取属性，id名不能错
            EventDataBase.Instance().GetEventProperty("Gargoyle_Upgrade", this);
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

            GameUnit.GameUnit Unit = this.Source as GameUnit.GameUnit;
            int _hp = Unit.hp;
            string _CR = Unit.id.Substring(Unit.id.Length - 1, 1);
            string Unit_Type = Unit.id.Substring(0, Unit.id.Length - 2);
            if(Convert.ToInt32(_CR) <= 2)
            {
                _CR = Convert.ToString(Convert.ToInt32(_CR) + 1);   //_CR的值+1
                GameUnit.UnitManager.Kill(null, this.Source as GameUnit.GameUnit);
                GameUnit.GameUnit newUnit = this.Regenerate("SandwormHead_" + _CR, Unit.mapBlockBelow, Unit.owner);
                newUnit.hp = _hp;
            }
            else
            {
                //等级已经到头了，啥都不发生
            }


        }
    }
}