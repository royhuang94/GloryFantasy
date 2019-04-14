using System.Collections.Generic;
using System.IO;
using UnityEngine;
using LitJson;
using Unit = GameUnit.GameUnit;
using Random = UnityEngine.Random;

namespace BattleMap
{
    public class BmuScriptsHandler : MonoBehaviour
    {
        #region 简单粗暴单例模式
        private static BmuScriptsHandler _instance = null;

        private void Awake()
        {
            _instance = this;
            InitDictionary();
            //GameObject prefabInstance = Instantiate(prefabs);
            //prefabInstance.transform.SetParent(GameObject.Find("UnitUI/Panel").gameObject.transform);
        }
        
        public static BmuScriptsHandler GetInstance()
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
                JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/Scripts/textToken2.json"));

            // 获取总模板数量
            int dataAmount = unitsTemplate.Count;
            // 依次添加数据到相应数据集中
            for (int i = 0; i < dataAmount; i++)
            {
                string id = unitsTemplate[i]["id"].ToString();
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
        public void AddScripts(GameObject gameObject)
        {
            // TODO: 添加必须添加的脚本
        }

        /// <summary>
        /// 给已有GameUnit脚本的GameObject，初始化其他属性
        /// </summary>
        /// <param name="unit">需要其中id和owner字段都已初始化</param>
        public void InitGameUnit(Unit unit)
        {
            JsonData data = _unitsData[unit.id];
            unit.Name = data["name"].ToString();
            unit.atk = (int)data["atk"];
            unit.hp = (int) data["hp"];
            unit.mov = (int) data["mov"];
            unit.rng = (int) data["rng"];
            unit.priority = new List<int>();
            unit.priority.Add((int) data["priority"]);
            int tagCount = data["tag"].Count;
            if (tagCount > 0)
            {
                unit.tag = new string[tagCount];
                for (int i = 0; i < tagCount; i++)
                {
                    unit.tag[i] = data["tag"][i].ToString();
                }
            }
        }

        /// <summary>
        /// 随机设定GameUnit脚本中的数值，仅能生成玩家方单位
        /// </summary>
        /// <param name="unit">无要求</param>
        public void InitGameUnitRandomly(Unit unit)
        {
            string randomID = this._unitsDataIDs[Random.Range(0, _unitsDataIDs.Count)];
            unit.id = randomID;
            unit.owner = "self";
            InitGameUnit(unit);
        }
        
    }
}