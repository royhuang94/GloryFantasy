using System.Collections.Generic;
using UnityEngine;
using LitJson;
using EventModel = GamePlay.Event.EventModule.EventWithWeight;

namespace GamePlay.Encounter
{
    public class Encounter
    {
        public string encounterID;//遭遇id
        public string mapID;//战斗地图id（文件名）
        public List<UnitMessage> unitMessageList;//本场遭遇里需要初始的单位
        public List<BattlefieldMessage> battleFieldMessageList;//本场遭遇里需要初始的战区事件
        public string[] globalTrigger;//判断胜负的全局Trigger
        public int deathPage;//死页数
        public Encounter(string id)
        {
            encounterID = id;
        }
    }

    /// <summary>
    /// 遭遇中的单位的结构体
    /// </summary>
    public class UnitMessage
    {
        public string unitID;
        public int pos_X;
        public int pos_Y;
        public int unitControler;
        public int isLeader;

        public UnitMessage(string id,int x,int y,int controler,int leader)
        {
            unitID = id;
            pos_X = x;
            pos_Y = y;
            unitControler = controler;
            isLeader = leader;
        }
    }

    /// <summary>
    /// 遭遇中战区的结构体
    /// </summary>
    public class BattlefieldMessage
    {
        public int regionID;
        public Dictionary<string, int> eventDic;//战区事件，key为事件id（名字），value为权重
        public string[] buff;
        public string[] triggers;
        public int Delta_X;
        public int Delta_Y;
        public string Owner;

        public BattlefieldMessage(int id, Dictionary<string, int> dic, string[] _buff,string[] _triggers,int dx,int dy,string owner)
        {
            regionID = id;
            eventDic = dic;
            buff = _buff;
            triggers = _triggers;
            Delta_X = dx;
            Delta_Y = dy;
            Owner = owner;
        }
    }

    /// <summary>
    /// 读取遭遇文件
    /// </summary>
    public class EncouterData : UnitySingleton<EncouterData>
    {
        //public string EncounterPath = "/Scripts/BattleMap/BattleMapData/encounter.json";//遭遇事件文件路径
        public Dictionary<string, Encounter> _encounterData;//遭遇对象
        public Dictionary<int, List<EventModel>> battleAreaEventsDic = new Dictionary<int, List<EventModel>>();//遭遇中文件中战区事件
        public DataOfThisBattle dataOfThisBattle = new DataOfThisBattle();
        

        /// <summary>
        /// 读取遭遇文件
        /// </summary>
        public void InitEncounter()
        {
            //JsonData data = JsonMapper.ToObject(File.ReadAllText(Application.dataPath + EncounterPath));
            string path = "EncounterDatabase/" + BattleMap.BattleMap.Instance().GetEncounterID();
            TextAsset json = Resources.Load<TextAsset>(path);
            JsonData data = JsonMapper.ToObject(json.text);
            
            _encounterData = new Dictionary<string, Encounter>();

            int dataCount = data.Count;
            for(int i = 0;i<dataCount; i++)
            {
                JsonData tem = data[i];
                Encounter encounter = new Encounter(tem["EncounterID"].ToString());
                string encounterID = tem["EncounterID"].ToString();
                encounter.mapID = tem["MapID"].ToString();
                JsonData triggerData = tem["GlobalTrigger"];
                encounter.globalTrigger = JsonToArray(triggerData);
                encounter.deathPage = (int)tem["DeathPage"];
                encounter.unitMessageList = new List<UnitMessage>();
                encounter.battleFieldMessageList = new List<BattlefieldMessage>();

                JsonData unitData = tem["UnitMessage"];
                UnitMessage unitMessage;
                for(int j = 0; j< unitData.Count; j++)
                {
                    string unitID = unitData[j]["UnitID"].ToString();
                    int x = (int)unitData[j]["Pos_X"];
                    int y = (int)unitData[j]["Pos_Y"];
                    int controler = (int)unitData[i]["UnitControler"];
                    int isLeader = (int)unitData[i]["IsLeader"];
                    unitMessage = new UnitMessage(unitID, x, y, controler,isLeader);
                    encounter.unitMessageList.Add(unitMessage);
                }

                JsonData battleFieldData = tem["BattlefieldMessage"];
                BattlefieldMessage battlefieldMessage;
                for (int j = 0; j < battleFieldData.Count; j++)
                {
                    int regionID = (int)battleFieldData[j]["RegionID"];
                    JsonData eventData = battleFieldData[j]["EventList"];
                    JsonData buffData = battleFieldData[j]["Buff"];
                    JsonData triggersData = battleFieldData[j]["Trigger"];
                    Dictionary<string, int> messageDic = new Dictionary<string, int>();
                    messageDic = JsonToDictionary(eventData);
                    string[] buff = null;
                    string[] triggers = null;
                    buff = JsonToArray(buffData);
                    triggers = JsonToArray(triggersData);
                    int dx = (int)battleFieldData[j]["Delta_X"];
                    int dy = (int)battleFieldData[j]["Delta_Y"];
                    string owner = battleFieldData[j]["Owner"].ToString();
                    battlefieldMessage = new BattlefieldMessage(regionID, messageDic, buff,triggers,dx,dy,owner);
                    encounter.battleFieldMessageList.Add(battlefieldMessage);
                }

                _encounterData.Add(encounterID, encounter); ;
            }
        }

        /// <summary>
        /// 根据遭遇ID,获取相应的地图文件
        /// </summary>
        /// <param name="encounterID">遭遇id</param>
        public void InitEncounter(string encounterID)
        {
            Encounter encounter = null;
            _encounterData.TryGetValue(encounterID, out encounter);
            BattleMap.BattleMap.Instance().InitBattleMapPath(encounter.mapID);
        }

        /// <summary>
        /// 读取遭遇中的战区事件,
        /// </summary>
        /// <param name="encounterID">遭遇id</param>
        public void InitBattleFieldEvent(string encounterID)
        {
            Encounter encounter = null;
            _encounterData.TryGetValue(encounterID, out encounter);
            List<BattlefieldMessage> battlefieldMessages = encounter.battleFieldMessageList;
            for(int i = 0; i < battlefieldMessages.Count; i++)
            {
                
                BattlefieldMessage battlefieldMessage = battlefieldMessages[i];
                int reginID = battlefieldMessage.regionID;
                List<EventModel> eventModes = new List<EventModel>();
                foreach (string eventID in battlefieldMessage.eventDic.Keys)
                {
                    int weight = 0;
                    battlefieldMessage.eventDic.TryGetValue(eventID, out weight);
                    EventModel _eventModel = new EventModel(eventID, weight);
                    eventModes.Add(_eventModel);                   
                }
                battleAreaEventsDic.Add(reginID, eventModes);
            }
            
        }

        /// <summary>
        /// 通过战区id获取战区里的事件Model
        /// </summary>
        /// <param name="regionID"></param>
        public List<EventModel> GetBattleFieldEvent(int regionID)
        {
            List<EventModel> models = null;
            if (battleAreaEventsDic.ContainsKey(regionID))
            {
                battleAreaEventsDic.TryGetValue(regionID, out models);
            }
            else
            {
                Debug.Log(string.Format("战区{0}：该战区没有事件",regionID));
                return null;
            }
            return models;
        }

        /// <summary>
        /// 通过战区id获取战区里的trigger
        /// </summary>
        /// <param name="regionID"></param>
        /// <returns></returns>
        public string[] GetBattleAreaTriggerByRegionID(int regionID)
        {
            Encounter encounter = null;
            string[] triggers = null;
            _encounterData.TryGetValue(BattleMap.BattleMap.Instance().GetEncounterID(), out encounter);
            for(int i =0;i<encounter.battleFieldMessageList.Count;i++)
            {
                BattlefieldMessage battlefieldMessage = encounter.battleFieldMessageList[i];
                if(regionID == battlefieldMessage.regionID)
                {
                    triggers = battlefieldMessage.triggers;
                }
            }
            if(triggers != null)
                return triggers;
            return null;
        }

        /// <summary>
        /// 通过战区id获取遭遇文件里面的BattlefieldMessage模块
        /// </summary>
        /// <param name="regionID"></param>
        /// <returns></returns>
        public BattlefieldMessage GetBattlefieldMessagebyID(int regionID)
        {
            Encounter encounter = null;
            BattlefieldMessage battlefieldMessage = null;
            _encounterData.TryGetValue(BattleMap.BattleMap.Instance().GetEncounterID(), out encounter);
            for (int i = 0; i < encounter.battleFieldMessageList.Count; i++)
            {
                battlefieldMessage = encounter.battleFieldMessageList[i];
                if (regionID == battlefieldMessage.regionID)
                {
                    return battlefieldMessage;
                }
            }
            Debug.Log("null");
            return null;
        }

        /// <summary>
        /// 获取遭遇对象
        /// </summary>
        /// <returns></returns>
        public Encounter GetEncounter()
        {
            return _encounterData[BattleMap.BattleMap.Instance().GetEncounterID()];
        }

        /// <summary>
        /// 获取遭遇文件中的战区初始状态；
        /// </summary>
        /// <param name="reginID"></param>
        /// <returns></returns>
        public string GetInitBattleAreaState(int reginID)
        {
            int count = _encounterData[BattleMap.BattleMap.Instance().GetEncounterID()].battleFieldMessageList.Count;
            for (int i = 0; i < count; i++)
            {
                if(reginID == _encounterData[BattleMap.BattleMap.Instance().GetEncounterID()].battleFieldMessageList[i].regionID)
                {
                    return _encounterData[BattleMap.BattleMap.Instance().GetEncounterID()].battleFieldMessageList[i].Owner;
                }
            }
            Debug.Log("该战区不存在");
            return null;
        }
        #region
        /// <summary>
        /// Json转字典，只支持{"a":1,"b":1,"c":1}格式,其中a为key，1为value
        /// </summary>
        /// <param name="a"></param>
        private Dictionary<string, int> JsonToDictionary(JsonData a)
        {
            Dictionary<string, int> dic = new Dictionary<string, int>();
            string b = a.ToJson(); //转成字符串
            string c = b.Replace("{", "").Replace("}", "").Replace("\"", "");//"去掉",{等字符
            string[] d = c.Split(',');//分割
            string[][] e = new string[d.Length][];
            for (int i = 0; i < e.Length; i++)//再次分割
            {
                e[i] = d[i].Split(':');
            }
            for (int i = 0; i < e.Length; i++)
            {
                dic.Add(e[i][0], int.Parse(e[i][1]));
            }
            return dic;
        }

        private string[] JsonToArray(JsonData a)
        {
            string[] array = new string[a.Count];
            string b = a.ToJson();
            string c = b.Replace("[", "").Replace("]", "").Replace("\"", "");
            string[] d = c.Split(',');
            return d;
        }
        #endregion
    }
}