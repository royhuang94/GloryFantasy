using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GamePlay.Event
{
    //增援：弓手之影
    public class ReinforceArcher : Event
    {
        public ReinforceArcher()
        {
            EventDataBase.Instance().GetEventProperty("ReinforceArcher", this);
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
            //"在来源战区中随机部署X个弓手之影(CR Y)",
            
            //来源
            //this.Source

            //X个
            //this.amount

            //Y
            //this.strenth
        }
    }
}