using LitJson;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace GamePlay.Event
{
    public class EventVariable
    {
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


        public EventVariable(string _id)
        {
            id = _id;
        }
    }

    /// <summary>
    /// 事件数据库
    /// </summary>
    public class EventDataBase : UnitySingleton<EventDataBase>
    {
        //事件表存储对象
        private Dictionary<string, EventVariable> _eventData;
        //Json文件的路径
        public string JsonFilePath = "/Scripts/EventScroll/EventDataBase.json";

        private void Awake()
        {
            InitEventDatabase();
        }

        /// <summary>
        /// 初始化事件数据库
        /// </summary>
        private void InitEventDatabase()
        {
            _eventData = new Dictionary<string, EventVariable>();

            JsonData abilitiesJsonData =
                JsonMapper.ToObject(File.ReadAllText(Application.dataPath + JsonFilePath));

            for (int i = 0; i < abilitiesJsonData.Count; i++)
            {
                //如果数据库里还没有这个事件的数据
                if (!_eventData.ContainsKey(abilitiesJsonData[i]["ID"].ToString()))
                {
                    JsonData tmp = abilitiesJsonData[i];
                    EventVariable newEvent = new EventVariable(tmp["ID"].ToString());

                    newEvent.amount = (int)tmp["amount"];
                    newEvent.effect = tmp["effect"].ToString();
                    //newEvent.factor = tmp["factor"].ToString();
                    newEvent.name = tmp["name"].ToString();
                    newEvent.source_type = tmp["source_type"].ToString();
                    newEvent.strenth = (int)tmp["strenth"];
                    //newEvent.type = JsonMapper.ToObject<List<string>>(tmp["type"].ToJson());
                    //newEvent.weight = (int)tmp["weight"];
                    newEvent.type = new List<string>();
                    for (int j = 0; j < tmp["type"].Count; j++)
                    {
                        newEvent.type.Add(tmp["type"][j].ToString());
                    }


                    //事件数据库中加入这个事件
                    _eventData.Add(tmp["ID"].ToString(), newEvent);
                }
                else
                {
                    //如果已经有这个异能，那还做什么屁，策划写错了呗……
                }
            }
        }

        /// <summary>
        /// 从事件数据库读入对应事件的各种属性包括id
        /// </summary>
        /// <param name="eventID"></param>
        /// <param name="selfEvent"></param>
        public void GetEventProperty(string eventID, Event selfEvent)
        {
            selfEvent.id = eventID;

            selfEvent.amount = _eventData[eventID].amount;
            selfEvent.effect = _eventData[eventID].effect;
            selfEvent.factor = _eventData[eventID].factor;
            selfEvent.name = _eventData[eventID].name;
            selfEvent.source_type = _eventData[eventID].source_type;
            selfEvent.strenth = _eventData[eventID].strenth;
            selfEvent.type = new List<string>(_eventData[eventID].type);
            selfEvent.weight = _eventData[eventID].weight;
        }
    }
}