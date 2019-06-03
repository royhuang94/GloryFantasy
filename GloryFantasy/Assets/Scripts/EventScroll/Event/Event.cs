using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GamePlay.Event
{
    public class Event : GameplayTool
    {
        /// <summary>
        /// 事件源
        /// </summary>
        public object Source;           
        /// <summary>
        /// 事件ID
        /// </summary>
        public string id;           
        /// <summary>
        /// 事件所包含的子效果生效的次数
        /// </summary>
        public int amount;
        /// <summary>
        /// 事件所包含的子效果的强度
        /// </summary>
        public int strenth;
        /// <summary>
        /// 参数x——事件源带来的 事件具体效果实施的次数 强化/弱化
        /// </summary>
        public int delta_x_amount;
        /// <summary>
        /// 参数y——事件源带来的 事件具体效果的强度 强化/弱化
        /// </summary>
        public int delta_y_strenth;
   
        /// <summary>
        /// 事件权重
        /// </summary>
        public int weight;          
        /// <summary>
        /// 事件效果说明——文字
        /// </summary>
        public string effect;           
        public string factor;               
        /// <summary>
        /// 事件的中文名
        /// </summary>  
        public string name;
        /// <summary>
        /// 事件源名称
        /// </summary>         
        public string source_type;      //事件源名称
        /// <summary>
        /// ？？？
        /// </summary>  
        public List<string> type;
        /// <summary>
        /// 事件的条件函数
        /// </summary>  
        protected Func<bool> Condition;
        /// <summary>
        /// 事件的执行函数
        /// </summary>  
        protected Action Action;
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
