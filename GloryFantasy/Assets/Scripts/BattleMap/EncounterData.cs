using System.Collections.Generic;
using System.IO;
using UnityEngine;
using LitJson;

namespace Encounter
{
    public class EncounterData
    {
        #region 变量
        //初始化遭遇
        [SerializeField] private string EncounterPath = "/Scripts/BattleMap/encounter.json";//遭遇事件文件路径
        private Dictionary<string, JsonData> encounterData;//遭遇id和对用的Jsondata对象
        public Dictionary<int, JsonData> battlefieldMessageData;//该战区的id和对应的事件
        #endregion


        ///<summary>
        /// 初始并存储遭遇
        ///</summary>
        public void InitEncounter()
        {
            encounterData = new Dictionary<string, JsonData>();
            JsonData data = JsonMapper.ToObject(File.ReadAllText(Application.dataPath + EncounterPath));
            int encouterCount = data.Count;
            for (int i = 0; i < encouterCount; i++)
            {
                string encounterID = data[i]["EncounterID"].ToString();
                this.encounterData.Add(encounterID, data[i]);
            }
        }

        /// <summary>
        /// 获取遭遇id，给大地图的接口
        /// </summary>
        /// <param name="encounterID">遭遇id</param>
        public void InitEncounter(string encounterID)
        {
            JsonData jsonData = null;
            encounterData.TryGetValue(encounterID, out jsonData);
            BattleMap.BattleMap.Instance().InitBattleMapPath(jsonData["MapID"].ToString());
        }

        /// <summary>
        /// 通过遭遇ID获取对应的遭遇data
        /// </summary>
        /// <param name="encounterID">遭遇id</param>
        public JsonData GetEncounterDataByID(string encounterID)
        {
            if (encounterID == null)
                return null;

            return encounterData[encounterID];
        }

        /// <summary>
        /// 初始战区事件
        /// </summary>
        /// <param name="encounterID"></param>
        public void InitBattlefield(string encounterID)
        {
            battlefieldMessageData = new Dictionary<int, JsonData>();
            JsonData jsonData = null;//整个遭遇Json对象
            encounterData.TryGetValue(encounterID, out jsonData);

            JsonData battleFieldData = jsonData["BattlefieldMessage"];//战区事件的data
            for(int i =0;i < battleFieldData.Count; i++)
            {
                int regionID = (int)battleFieldData[i]["RegionID"]; //该战区在地图文件中的“战区区分编号”
                battlefieldMessageData.Add(regionID, battleFieldData[i]);
            }

        }

    }
}