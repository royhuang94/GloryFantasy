using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace GamePlay.Event
{
    /// <summary>
    /// 事件轴
    /// </summary>
    public class EventScroll
    {
        /// <summary>
        /// 当前自然生成的事件集合的最大触发回合数
        /// </summary>
        public int nowBigestTurn = 0;

        /// <summary>
        /// 事件集合列表
        /// </summary>
        private List<EventAssembly> _eventScroll = new List<EventAssembly>();

        /// <summary>
        /// 事件模块列表
        /// </summary>
        private List<EventModule> _eventModuleList = new List<EventModule>();

        /// <summary>
        /// 添加一个事件模块进入随机池
        /// </summary>
        /// <param name="eventModule"></param>
        public void AddEventModule(EventModule eventModule)
        {
            _eventModuleList.Add(eventModule);
        }

        /// <summary>
        /// 执行事件列表的头
        /// </summary>
        public void ProcessFirstEventModule()
        {
            if (_eventScroll.Count > 0)
            {
                _eventScroll[0].Execute();
            }
            //执行完毕后删除事件列表的头
            _eventScroll.RemoveAt(0);
        }

        /// <summary>
        /// 根据已存在事件模块增加新的事件集合列表
        /// </summary>
        public void CreateNewEventAssembly()
        {
            EventAssembly newAssembly = new EventAssembly();
            //使用这个方法生成事件集合会让计数+1
            this.nowBigestTurn++;

            foreach (EventModule module in _eventModuleList)
            {
                //按照权重随机选择事件
                string selectedEvent = "";
                //预计算权重的总值
                int sum = 0;
                for (int i = 0; i < module.EventList.Count; i++)
                {
                    sum += module.EventList[i].EventWeight;
                }
                var r = UnityEngine.Random.Range(0, sum) + 1;
                int temp = 0; 
                for (int i = 0; i < module.EventList.Count; i++)
                {
                    temp += module.EventList[i].EventWeight;
                    if (r <= temp)
                    {
                        selectedEvent = module.EventList[i].EventName;
                    }
                }
                //根据选中的事件的string生成对应的类
                Type tempType = Type.GetType("GamePlay.Event." + selectedEvent);
                Event tempEvent = Activator.CreateInstance(tempType) as Event;
                //设置事件的源
                tempEvent.Source = module.Source;
                //将生成的事件塞入到新的事件集合里
                newAssembly.Add(tempEvent);
            }
            //所有事件模块判定完后将新的事件集合塞入列表
            //在这之前记得给事件集合打上回合数的的标签
            newAssembly.ExcuteTurn = this.nowBigestTurn;
            _eventScroll.Add(newAssembly);
        }

        /// <summary>
        /// 传入一个事件模块，将与之相关的事件全部清除
        /// </summary>
        /// <param name="assembly"></param>
        public void DeleteOneModule(EventModule module)
        {
            _eventModuleList.Remove(module);

            object source = module.Source;
            foreach (EventAssembly assembly in _eventScroll)
            {
                assembly.DeleteEvent(source);
            }
        }
        
    }

    /// <summary>
    /// 事件集合
    /// </summary>
    public class EventAssembly
    {
        /// <summary>
        /// 将在某个回合发动，用以在事件轴中的排序
        /// </summary>
        public int ExcuteTurn;

        /// <summary>
        /// 事件集合里包含的事件
        /// </summary>
        private List<Event> eventList = new List<Event>();

        /// <summary>
        /// 执行该事件集合里的所有事件
        /// </summary>
        public void Execute()
        {
            foreach (Event i in eventList)
            {
                i.Execute();
            }
        }

        /// <summary>
        /// 将一个事件加入到该事件集合
        /// </summary>
        /// <param name="otherEvent"></param>
        public void Add(Event otherEvent)
        {
            eventList.Add(otherEvent);
        }

        /// <summary>
        /// 传入源，删除这个源的事件
        /// </summary>
        /// <param name="source"></param>
        public void DeleteEvent(object source)
        {
            foreach (Event oneEvent in eventList)
            {
                if (oneEvent.Source == source)
                {
                    eventList.Remove(oneEvent);
                }
            }
        }
    }

    /// <summary>
    /// 事件模块
    /// </summary>
    public class EventModule
    {
        public struct EventWithWeight
        {
            /// <summary>
            /// 事件名
            /// </summary>
            public string EventName;
            /// <summary>
            /// 事件权重
            /// </summary>
            public int EventWeight;

            public EventWithWeight(string _eventName, int _eventWeight)
            {
                EventName = _eventName;
                EventWeight = _eventWeight;
            }
        }

        /// <summary>
        /// 事件的事件源
        /// </summary>
        public object Source;
        public List<EventWithWeight> EventList = new List<EventWithWeight>();

        /// <summary>
        /// 事件模块的创建函数
        /// </summary>
        /// <param name="_Source">事件源</param>
        public EventModule(object _Source)
        {
            this.Source = _Source;
        }

        /// <summary>
        /// 向事件模块加入事件id以及该事件的权重
        /// </summary>
        /// <param name="EventName"></param>
        /// <param name="EventWeight"></param>
        public void AddEvent(string EventName, int EventWeight)
        {
            EventList.Add(new EventWithWeight(EventName, EventWeight));
        }

        /// <summary>
        /// 删除该事件集合
        /// </summary>
        public void DeleteThisAssembly()
        {
            Gameplay.Instance().eventScroll.DeleteOneModule(this);
        }
    }
}