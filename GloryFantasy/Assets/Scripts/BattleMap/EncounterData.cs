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
        private Dictionary<string, JsonData> encounterData;
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

        public JsonData GetEncounterDataByID(string encounterID)
        {
            if (encounterID == null)
                return null;

            return encounterData[encounterID];
        }



    }
}