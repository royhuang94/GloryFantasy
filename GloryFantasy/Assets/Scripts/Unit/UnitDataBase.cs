using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using LitJson;
using Unit = GameUnit.GameUnit;
using Random = UnityEngine.Random;

using Ability;

namespace GameUnit
{
    public class UnitDataBase : UnitySingleton<UnitDataBase>
    {
        private void Awake()
        {
            InitDictionary();
        }

        #region 变量
       [SerializeField]private string DataBasePath;
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
            JsonData unitsTemplate = JsonMapper.ToObject(File.ReadAllText(Application.dataPath + DataBasePath));
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
        /// 对GameUnit进行数值初始化
        /// </summary>
        /// <param name="unit">被初始化的持有GameUnit的GameObject</param>
        /// <param name="id">要初始化的Unit的数据的ID</param>
        /// <param name="damage">这个角色是否出场就受伤</param>
        public void InitGameUnit(GameObject unit, string id, OwnerEnum owner, int damage = 0)
        {
            if (unit.GetComponent<Unit>() != null)
            {
                //把GameUnit清除掉，等于把trigger洗掉
                Destroy(unit.GetComponent<Unit>());

                InitGameUnit(unit.AddComponent<Unit>(), id, owner, damage);
            }
            else
            {
                InitGameUnit(unit.AddComponent<Unit>(), id, owner, damage);
                //Debug.Log("In UnitDataBase: " + unit.name + " doesn't have GameUnit.Can;t be Initial.");
            }
        }

        /// <summary>
        /// 对GameUnit进行数值初始化
        /// </summary>
        /// <param name="unit">被初始化的GameUnit引用</param>
        /// <param name="unitID">要初始化的Unit的数据的ID</param>
        /// <param name="damage">这个角色是否出场就受伤</param>
        public void InitGameUnit(Unit unit, string unitID, OwnerEnum owner, int damage = 0)
        {
            if (!_unitsData.ContainsKey(unitID))
            {
                Debug.Log("UnitDataBase is not contant " + unitID);
                return;
            }

            JsonData data = _unitsData[unitID];

            //先删除异能再初始化数值
            //初始化数值,记得和GameUnit的成员保持一致
            unit.owner = owner;
            unit.setATK(int.Parse(data["Atk"].ToString()));
            unit.id = data["CardID"].ToString();
            unit.Color = data["Color"][0].ToString();
            unit.Effort = data["Effort"].ToString();
            unit.CD = int.Parse(data["HasCD"].ToString());
            unit.setMHP(int.Parse(data["Hp"].ToString())); unit.hp = unit.getMHP() - damage;
            unit.id = data["ID"].ToString();
            unit.setMOV(int.Parse(data["Mov"].ToString()));
            unit.name = data["Name"].ToString();
            unit.priority = new List<int>();
            unit.priority.Add(int.Parse(data["Prt"].ToString()));
            unit.setRNG(int.Parse(data["Rng"].ToString()));
            unit.tag = new List<string>();
            for (int i = 0; i < data["Tag"].Count; i++)
            {
                unit.tag.Add(data["Tag"][i].ToString());
            }

            unit.priSPD = 0;
            unit.priDS = 0;
            //unit.damaged = "" //不知道这什么玩意儿
            unit.canNotAttack = true;
            unit.canNotMove = true;
            unit.armorRestore = 0;
            unit.armor = 0;

            //最后初始化新异能
            AddGameUnitAbility(unit, data);
            //最后初始化单位绑定的事件信息
           bool canInitEventModule =  AddGameUnitEventsInfo(unit, data);
            if(canInitEventModule)
            {
                unit.EventModule = new GamePlay.Event.EventModule(unit);
                unit.EventModule.AddEvent(unit.eventsInfo);
            }

        }

        /// <summary>
        /// 将给定GameUnit的异能脚本全部删除
        /// </summary>
        /// <param name="unit"></param>
        public void DeleteGameUnitAbility(Unit unit)
        {
            //删除所有异能脚本
            foreach (Ability.Ability ability in unit.gameObject.GetComponents<Ability.Ability>())
            {
                Destroy(ability);
            }
        }

        /// <summary>
        /// 根据给定UnitID，添加GameUnit的异能脚本
        /// </summary>
        public void AddGameUnitAbility(Unit unit, JsonData unitJsonData)
        {
            unit.abilities = new List<string>();
            for (int i = 0; i < unitJsonData["Ability"].Count; i++)
            {
                if (unitJsonData["Ability"][i].ToString() == "") continue;
                unit.abilities.Add(unitJsonData["Ability"][i].ToString());
                Component ability =
                    unit.gameObject.AddComponent(
                        System.Type.GetType("Ability." + unitJsonData["Ability"][i].ToString().Split('_').First()));
                if (ability != null)
                {
                    GameUtility.UtilityHelper.Log("添加异能 " + unitJsonData["Ability"][i].ToString() + " 成功",
                        GameUtility.LogColor.RED);
                    Ability.Ability a = ability as Ability.Ability;
                    a.Init(unitJsonData["Ability"][i].ToString());
                }
                else
                {
                    GameUtility.UtilityHelper.Log("添加异能 " + unitJsonData["Ability"][i].ToString() + " 失败",
                        GameUtility.LogColor.RED);
                }
            }
            
        }
        /// <summary>
        /// 根据给定UnitID，添加GameUnit的异能脚本
        /// </summary>
        public bool AddGameUnitEventsInfo(Unit unit, JsonData unitJsonData)
        {
            if (unitJsonData["Event"].Count <= 0)
                return false;

            unit.eventsInfo = new List<GamePlay.Event.EventModule.EventWithWeight>();
            Dictionary<string, int> temp = GameUtility.UtilityHelper.JsonToDictionary(unitJsonData["Event"]);
            foreach (string key in temp.Keys)
            {
                unit.eventsInfo.Add(new GamePlay.Event.EventModule.EventWithWeight(key, temp[key]));
            }

            return true;
        }
    }
}