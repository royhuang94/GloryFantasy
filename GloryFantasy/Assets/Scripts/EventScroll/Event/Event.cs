using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GamePlay.Event
{
    public class Event : GameplayTool
    {
        //事件的源
        public object Source;

        public string id;
        public int amount;
        public int weight;
        /// <summary>
        /// 事件效果说明
        /// </summary>
        public string effect;
        public string factor;
        /// <summary>
        /// 事件的中文名
        /// </summary>
        public string name;
        public string source_type;
        public int strenth;
        public List<string> type;

        //事件的条件函数和执行函数
        protected Func<bool> Condition;
        protected Action Action;

        //public Event(string _id)
        //{
        //    this.id = _id;
        //}

        /// <summary>
        /// 执行事件
        /// </summary>
        /// <returns></returns>
        public bool Execute()
        {
            if (Condition())
            {
                Action();
                return true;
                ;
            }
            else
            {
                return false;
            }
        }
    }
}
