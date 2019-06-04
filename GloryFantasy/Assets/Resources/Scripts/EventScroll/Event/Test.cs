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
    public class Test : Event
    {
        public Test()
        {
            //从数据库读取属性，id名不能错
            EventDataBase.Instance().GetEventProperty("ReinforceArcher", this);
            //初始化条件函数和行动函数
            this.Condition = selfCondition ;
            this.Action = selfAction;
        }

        bool selfCondition()
        {
            //do nothing

            return true;
        }

        void selfAction()
        {

            Debug.Log("测试事件触发");
            //"在来源战区中随机部署X个弓手之影(CR Y)",
            
            //来源
            //this.Source as GameUnit.GameUnit

            //X个
            //this.amount

            //Y
            //this.strenth
        }
    }
}