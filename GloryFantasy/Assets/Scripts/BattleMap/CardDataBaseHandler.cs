using System.Collections.Generic;
using System.IO;
using UnityEngine;
using LitJson;
using Unit = GameUnit.GameUnit;
using Random = UnityEngine.Random;

namespace BattleMap
{
    public class CardDataBaseHandler : MonoBehaviour
    {
        #region 简单粗暴单例模式
        private static CardDataBaseHandler _instance = null;

        private void Awake()
        {
            _instance = this;
            InitDictionary();
        }
        
        public static CardDataBaseHandler GetInstance()
        {
            return _instance;
        }
        #endregion

        #region 变量

        private Dictionary<string, JsonData> _unitsData;
        private List<string> _unitsDataIDs;

        #endregion
        
        ///<summary>
        /// 初始化存储模板数据字典以及模板数据id列表
        ///</summary>
        private void InitDictionary()
        {
            this._unitsData = new Dictionary<string, JsonData>();
            this._unitsDataIDs = new List<string>();

            // 从制定路劲加载json文件并映射成字典
            JsonData unitsTemplate =
                JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/Scripts/UnitScripts/textToken.json"));
            // 获取总模板数量
            int dataAmount = unitsTemplate.Count;
            // 依次添加数据到相应数据集中
            for (int i = 0; i < dataAmount; i++)
            {
                string id = unitsTemplate[i]["ID"].ToString();
                _unitsData.Add(id, unitsTemplate[i]);
                _unitsDataIDs.Add(id);
            }
        }

        /// <summary>
        /// 获取所有模板数据的ID列表
        /// </summary>
        /// <returns>string形式List</returns>
        public List<string> GetUnitsDataIDs()
        {
            return _unitsDataIDs;
        }

        
        /// <summary>
        /// 挂载所有在战斗地图中必须的脚本
        /// </summary>
        /// <param name="gameObject">需要挂载脚本的GameObject</param>
        public void AddScripts(GameObject _gameObject)
        {
            // TODO: 添加必须添加的脚本
        }

        public void InitGameUnit(Unit unit,string id)
        {
            JsonData data = _unitsData[id];
            unit.unitAttribute.atk = int.Parse(data["Atk"].ToString());
            unit.unitAttribute.CardID = data["CardID"].ToString();
            unit.unitAttribute.Color = data["Color"].ToString();
            unit.unitAttribute.Effort = data["Effort"].ToString();
            unit.unitAttribute.HasCD = int.Parse(data["HasCD"].ToString());
            unit.unitAttribute.HP = int.Parse(data["Hp"].ToString());
            unit.unitAttribute.ID = data["ID"].ToString();
            unit.unitAttribute.Mov = int.Parse(data["Mov"].ToString());
            unit.unitAttribute.name = data["Name"].ToString();
            unit.unitAttribute.Prt = int.Parse(data["Prt"].ToString());
            unit.unitAttribute.rng = int.Parse(data["Rng"].ToString());
            int tagCount = data["Tag"].Count;
            if(tagCount > 0)
            {
                unit.unitAttribute.tag = new List<string>();
                for (int i = 0; i < tagCount; i++)
                {
                    unit.unitAttribute.tag.Add(data["Tag"][i].ToString());
                }
            }
        }
        
    }
}