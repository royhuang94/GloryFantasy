using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using IMessage;

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
        /// 待显示的直接事件来源列表
        /// </summary>
        private List<DirectEvent> _DirectEventList = new List<DirectEvent>();

        private List<int> _timeScroll = new List<int>();

        /// <summary>
        /// 获取事件模块列表数量
        /// </summary>
        public int EventModuleListCount
        {
            get
            {
                return _eventModuleList.Count;
            }
        }
        /// <summary>
        /// 获取事件集合列表数量
        /// </summary>
        public int EventScrollListCount
        {
            get
            {
                return _eventScroll.Count;
            }
        }
        /// <summary>
        /// 获取待显示的直接事件来源列表数量
        /// </summary>
        public int DirectEventListCount
        {
            get
            {
                return _DirectEventList.Count;
            }
        }
        /// <summary>
        /// 获取事件轴的队列个数，目前默认为5
        /// </summary>
        public int EventScrollCount
        {
            get
            {
                return 5;
            }
        }

        /// <summary>
        /// 添加一个事件模块进入仲裁器
        /// </summary>
        /// <param name="eventModule"></param>
        public void AddEventModule(EventModule eventModule)
        {
            _eventModuleList.Add(eventModule);
        }
        /// <summary>
        /// 添加一个 待显示的直接事件对象 进入 仲裁器
        /// </summary>
        public void AddDirectEvent_to_Judge(DirectEvent _DirectEvent)
        {
            _DirectEventList.Add(_DirectEvent);
        }
        /// <summary>
        /// 添加一个 可显示的直接事件对象 直接进入 事件轴
        /// </summary>
        public void AddDirectEvent_to_Scroll(DirectEvent _DirectEvent)      //在判断到 直接事件对象 应立即入轴时，执行此函数
        {
            //
            int _expect_turn = _DirectEvent.Get_Expect_Turn();
            string _select_event = _DirectEvent.Get_Event_name();
            int No_of_Expect_EventAssembly =(EventScrollListCount-1) - (this.nowBigestTurn - _expect_turn) ;   //计算该 事件 应当加入的 已有事件集合 的编号
                                                                                                               //根据选中的事件的string生成对应的类
            Type tempType = Type.GetType("GamePlay.Event." + _select_event);
            //if (tempType == null)
                //continue;
            Event tempEvent = Activator.CreateInstance(tempType) as Event;
            //设置事件的源
            tempEvent.Source = _DirectEvent.Source;
            //将 事件 加入 相应事件队列
            _eventScroll[No_of_Expect_EventAssembly].Add(tempEvent);
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

            MsgDispatcher.SendMsg((int)MessageType.EventNodeChange);
            //执行完毕后删除事件列表的头
            _eventScroll.RemoveAt(0);
        }

        /// <summary>
        /// 根据已存在事件模块和直接事件对象增加新的事件集合列表：：此函数在每个回合开始时调用,调用此函数会使 事件轴的最大回合计数+1
        /// </summary>
        public void CreateNewEventAssembly()
        {
            EventAssembly newAssembly = new EventAssembly();
            //使用这个方法生成事件集合会让计数+1
            this.nowBigestTurn++;
            //将来源于 事件模块 的 事件 入轴
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
                        break;
                    }
                }
                /*eg: GamePlay.Event.ReinforceArcher*/
                //根据选中的事件的string生成对应的类
                Type tempType = Type.GetType("GamePlay.Event." + selectedEvent);
                if (tempType == null)
                    continue;
                Event tempEvent = Activator.CreateInstance(tempType) as Event;
                //设置事件的源
                tempEvent.Source = module.Source;
                //将生成的事件塞入到新的事件集合里
                newAssembly.Add(tempEvent);
            }
            //将来源于 直接事件对象 的 事件 入轴
            foreach (DirectEvent Dir in _DirectEventList)
            {
                string selectedEvent = "";
                if(Dir.Get_Expect_Turn() == nowBigestTurn)      //若某在 待显示的直接事件列表 中的 直接事件对象代表的事件 应该在此回合入轴
                {
                    selectedEvent = Dir.Get_Event_name();
                    //根据选中的事件的string生成对应的类
                    Type tempType = Type.GetType("GamePlay.Event." + selectedEvent);
                    if (tempType == null)
                        continue;//暂不设错误提示
                    Event tempEvent = Activator.CreateInstance(tempType) as Event;
                    //设置事件的源
                    tempEvent.Source = Dir.Source;
                    //将生成的事件塞入到新的事件集合里
                    newAssembly.Add(tempEvent);
                }

            }
            //所有事件模块判定完后将新的事件集合塞入列表
            //在这之前记得给事件集合打上回合数的的标签
            newAssembly.ExcuteTurn = this.nowBigestTurn;
            _eventScroll.Add(newAssembly);
            //Debug.Log("count: " + EventScrollListCount);
            //添加回合数标签
            //Debug.Log("当前回合数: " + Gameplay.Instance().roundProcessController.State.roundCounter);
            _timeScroll.Add(Gameplay.Instance().roundProcessController.State.roundCounter);     //这个数值暂时没有用到
        }

        /// <summary>
        /// 生成的事件塞入到新的事件集合里
        /// </summary>
        /// <param name="newAssembly"></param>
        /// <param name="eventAssembls"></param>
        private void GetNewAssembly(EventAssembly newAssembly, List<EventModule> eventAssembls)
        {
            foreach (EventModule module in eventAssembls)
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
                /*eg: GamePlay.Event.ReinforceArcher*/
                //根据选中的事件的string生成对应的类
                Type tempType = Type.GetType("GamePlay.Event." + selectedEvent);
                Event tempEvent = Activator.CreateInstance(tempType) as Event;
                //设置事件的源
                tempEvent.Source = module.Source;
                //将生成的事件塞入到新的事件集合里——来源为事件模块
                newAssembly.Add(tempEvent);
                //将事件塞入到新的事件集合里——来源为直接事件对象
                //todo
            }
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

        /// <summary>
        /// 清空事件轴相关
        /// </summary>
        public void Clear()
        {
            if (_eventModuleList != null && _eventModuleList.Count != 0)
            {
                _eventModuleList.Clear();
            }
            if (_eventScroll != null && _eventScroll.Count != 0)
            {
                _eventScroll.Clear();
            }
            if (_timeScroll != null && _timeScroll.Count != 0)
            {
                _timeScroll.Clear();
            }
            if (_DirectEventList != null && _DirectEventList.Count != 0)
            {
                _DirectEventList.Clear();
                MsgDispatcher.SendMsg((int)MessageType.EventNodeChange);
            }

            nowBigestTurn = 0;
        }

        /// <summary>
        /// 以回合数作为坐标添加一个事件模块进入事件轴
        /// 重载+1
        /// </summary>
        /// <param name="round">插入回合数</param>
        /// <param name="eventModule">事件模型</param>
        public void InsertEventByRound(int round, EventModule eventModule)
        {
            List<EventModule> eventModules = new List<EventModule>();
            eventModules.Add(eventModule);
            InsertEventByRound(round, eventModules);
        }
        /// <summary>
        /// 以回合数作为坐标添加一个事件模块进入事件轴
        /// 重载+1
        /// </summary>
        /// <param name="round">插入回合数</param>
        /// <param name="eventModule">事件模型</param>
        public void InsertEventByRound(int round, List<EventModule> eventModule)
        {
            EventAssembly newAssembly = new EventAssembly();
            //使用这个方法生成事件集合会让计数+1
            this.nowBigestTurn++;

            for (int i = 0; i < _eventScroll.Count; i++)
            {
                if (i != round - 1)
                    continue;
                GetNewAssembly(newAssembly, eventModule);
                //所有事件模块判定完后将新的事件集合塞入列表
                //在这之前记得给事件集合打上回合数的的标签
                newAssembly.ExcuteTurn = this.nowBigestTurn;
                _eventScroll.Insert(i, newAssembly);
            }
        }

        /// <summary>
        /// 获取事件集合
        /// </summary>
        /// <returns></returns>
        public List<EventAssembly> GetEventAssemblies()
        {
            return _eventScroll;
        }

        /// <summary>
        /// 获取事件信息集合
        /// </summary>
        /// <param name="i">获取指定的队列，默认为0即第一队列</param>
        /// <returns></returns>
        public List<EventAssembly.EventInfo> GetEventInfos(int i = 0)
        {
            return _eventScroll[i].GetEventInfos;
        }
    }

    /// <summary>
    /// 事件集合
    /// </summary>
    public class EventAssembly
    {
        /// <summary>
        /// 事件信息
        /// </summary>
        public struct EventInfo
        {
            /// <summary>
            /// 事件名
            /// </summary>
            public string EventName;
            /// <summary>
            /// 数量
            /// </summary>
            public int Amount;
            /// <summary>
            /// 强度
            /// </summary>
            public int Strength;
            /// <summary>
            /// 类型
            /// </summary>
            public List<string> Type;
            /// <summary>
            /// 事件源
            /// </summary>
            public object Source;
            /// <summary>
            /// 事件效果描述
            /// </summary>
            public string Effect;

            /// <summary>
            /// 有参构造函数
            /// (重构+1)
            /// </summary>
            /// <param name="_eventName"></param>
            /// <param name="_amount"></param>
            /// <param name="_strength"></param>
            /// <param name="_type"></param>
            /// <param name="_source"></param>
            /// <param name="_effect"></param>
            public EventInfo(string _eventName, int _amount, int _strength, List<string> _type, object _source, string _effect)
            {
                EventName = _eventName;
                Amount = _amount;
                Strength = _strength;
                Type = _type;
                Source = _source;
                Effect = _effect;
            }
            public EventInfo(Event eve)
            {
                EventName = eve.name;
                Amount = eve.amount;
                Strength = eve.strenth;
                Type = eve.type;
                Source = eve.Source;
                Effect = eve.effect;
            }
        }

        /// <summary>
        /// 将在某个回合发动，用以在事件轴中的排序
        /// </summary>
        public int ExcuteTurn;

        /// <summary>
        /// 事件集合里包含的事件
        /// </summary>
        private List<Event> eventList = new List<Event>();
        /// <summary>
        /// 事件信息集合
        /// </summary>
        private List<EventInfo> _eventInfos = new List<EventInfo>();

        /// <summary>
        /// 获取时间集合的长度，即该集合里事件数量
        /// </summary>
        public int eventListCount
        {
            get { return eventList.Count; }
        }

        /// <summary>
        /// 获取事件集合
        /// </summary>
        public List<Event> getEventList
        {
            get { return eventList;  }
        }
        /// <summary>
        /// 获取事件信息集合
        /// </summary>
        public List<EventInfo> GetEventInfos
        {
            get { return _eventInfos; }
        }


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
            //添加事件信息
            _eventInfos.Add(new EventInfo(otherEvent));
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

            foreach (EventInfo eventInfo in _eventInfos)
            {
                if(eventInfo.Source == source)
                    _eventInfos.Remove(eventInfo);
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
        /// <summary>
        /// 事件源的x属性——现在直接由事件向源获取 用处不大了
        /// </summary>
        public int x_of_Source;
        /// <summary>
        /// 事件源的y属性——现在直接由事件向源获取 用处不大了
        /// </summary>
        public int y_of_Source;

        /// <summary>
        /// 事件模块的事件列表，包含事件名，和权重
        /// </summary>
        public List<EventWithWeight> EventList = new List<EventWithWeight>();

        /// <summary>
        /// 事件模块的创建函数
        /// </summary>
        /// <param name="_Source">事件源</param>
        public EventModule(object _Source)
        {
            this.Source = _Source;

            //获取事件源的 强化/弱化 参数 x 和 y
            System.Type tempType = Source.GetType();


        }

        public void AddEvent(List<EventWithWeight> eventWithWeights)
        {
            EventList.AddRange(eventWithWeights);
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
        /// 删除该事件模块
        /// </summary>
        public void DeleteThisModule()
        {
            Gameplay.Instance().eventScroll.DeleteOneModule(this);
        }
    }
    /// <summary>
    /// 直接事件对象
    /// </summary>
    public class DirectEvent
    {

        /// <summary>
        /// 直接事件的事件ID
        /// </summary>
        public string EventName;
        /// <summary>
        /// 直接事件的事件源
        /// </summary>
        public object Source;
        /// <summary>
        /// 事件源的x属性
        /// </summary>
        public int x_of_Source;
        /// <summary>
        /// 事件源的y属性
        /// </summary>
        public int y_of_Source;
        public void get_x_and_y_from_Source()
        {
            //todo：从Source处获取x和y参数的值
            //x_of_Source =
            //y_of_Source =
        }
        /// <summary>
        /// 决定直接事件在哪回合入轴
        /// </summary>
        public int turn_into_EventScroll;
        /// <summary>
        /// 直接事件对象的构造函数
        /// </summary>
        /// <param name="_Source">事件源</param>
        public DirectEvent(string _EventName,int _Expect_Turn, object _Source = null )
        {
            this.EventName = _EventName;
            this.turn_into_EventScroll = _Expect_Turn;
            this.Source = _Source;
            //获取事件源的强化/弱化 参数 x 和 y
            GameUnit.GameUnit Unit_message = this.Source as GameUnit.GameUnit;
            this.x_of_Source = Unit_message.delta_x_amount;
            this.y_of_Source = Unit_message.delta_y_strenth;

        }
        public int Get_Expect_Turn()    //获取事件的入轴回合
        {
            return this.turn_into_EventScroll;
        }
        public string Get_Event_name()  //获取事件的名称
        {
            return this.EventName;
        }

        /// <summary>
        /// 删除该直接事件来源
        /// </summary>
        public void DeleteThisDirectEvent()         //此函数不需要调用，暂留。
        {
           //
        }
    }
}
