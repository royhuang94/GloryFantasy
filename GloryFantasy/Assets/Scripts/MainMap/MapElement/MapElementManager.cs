using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameGUI;
using System.IO;
using UnityEngine.EventSystems;
using UnityEditor;
using System.Linq;
using PlayerCollection;

namespace MainMap
{

    /// <summary>管理所有地格上元素，在这里初始化并生成地格上元素（怪物，事件，道具等）
    /// 
    /// </summary>
    public class MapElementManager : UnitySingleton<MapElementManager>
    {
        /// <summary>
        /// 正在处理的地格上层元素
        /// </summary>
        public static GameObject DuringTheProcess;
        /// <summary>根据传入的参数生成地图上层元素
        /// 
        /// </summary>
        /// <param name="elementdetail"></param>
        /// <param name="mapunit"></param>
        public void InstalizeElement(string[] elementdetail, GameObject mapunit)
        {
            switch (elementdetail[1])
            {
                case "monster":
                    //Debug.Log("生成怪物");
                    GameObject monster = (GameObject)Instantiate(Resources.Load("MMtestPrefab/monster", typeof(GameObject)));
                    ElementSet(monster, mapunit);
                    Monster m = monster.AddComponent<Monster>();
                    m.SetID(elementdetail[2]);
                    m.SetTexture();

                    break;
                case "randomevent":
                    //Debug.Log("生成随机事件");
                    GameObject randomevent = (GameObject)Instantiate(Resources.Load("MMtestPrefab/randomevent", typeof(GameObject)));
                    ElementSet(randomevent, mapunit);
                    randomevent.AddComponent<RandomEvent>();
                    break;
                case "treasure":
                    //Debug.Log("生成宝箱");
                    GameObject treasure = (GameObject)Instantiate(Resources.Load("MMtestPrefab/treasure", typeof(GameObject)));
                    ElementSet(treasure, mapunit);
                    treasure.AddComponent<Treasure>();
                    break;
                default:
                    Debug.Log("地格上层元素读取错误");
                    break;
            }
        }
        /// <summary>设置传入的地图上层元素的父节点
        /// 
        /// </summary>
        /// <param name="mapelement"></param>
        /// <param name="mapunit"></param>
        public void ElementSet(GameObject mapelement, GameObject mapunit)
        {
            mapelement.transform.parent = mapunit.transform;
            mapelement.transform.position = mapunit.transform.position + new Vector3(0, 0, -0.05f);
            mapelement.layer = 10;
        }
        /// <summary>
        /// 处理结束后摧毁地格上层元素
        /// </summary>
        public static void DestroyElement()
        {
            GameObject.Destroy(DuringTheProcess);
            Debug.Log("摧毁元素");
        }
    }
    /// <summary>地图元素抽象类
    /// 
    /// </summary>
    public abstract class MapElement : MonoBehaviour
    {
        protected virtual void Awake()
        {

        }
        /// <summary>
        /// 把此元素设为正在处理的地格上层元素，并处理点击细节
        /// </summary>
        public virtual void OnElementClick()
        {
            MapElementManager.DuringTheProcess = this.gameObject;
            OnClickDetail();
        }
        public abstract void OnClickDetail();
    }
    /// <summary>怪物
    /// 
    /// </summary>
    public class Monster : MapElement
    {
        //遭遇id
        private string encounterid;
        //这个是测试用的id！只用来更换素材
        private string testid = "id_1";
        private int level;
        private static List<Monster> monsterlist = new List<Monster>();
        private string BattleMapSceneName = "BattleMapTest";          // 战斗地图场景名，在此修改
        private string MainMapSceneName = "MainMapTest1";
        protected override void Awake()
        {
            //Debug.Log("怪物初始化");
            monsterlist.Add(this);
        }
        public override void OnClickDetail()
        {
            // Debug.Log("怪物被点击");
            //            MainMapUI.Instance().HideMain();
            InToBattle();

        }
        public void InToBattle()
        {
            SceneSwitchController.Instance().SetData(encounterid, null);
            SceneSwitchController.Instance().Switch(MainMapSceneName, BattleMapSceneName);
            SceneSwitchController.Instance().GetDeckFormMainMapK(CardCollection.Instance().deck);
        }
        /// <summary>
        /// 设置怪物遭遇id和等级信息
        /// </summary>
        /// <param name="id"></param>
        public void SetID(string id)
        {
            encounterid = id;
            level = int.Parse(id.Split('_').Last());
            Debug.Log(encounterid);
        }
        public void SetTexture()
        {
            //这行测试用，素材统一了就删掉,
            Sprite newsprite = (Sprite)Resources.Load("MMtesttexture/Monster/" + encounterid, typeof(Sprite));
            Sprite defaultsprite = (Sprite)Resources.Load("MMtesttexture/Monster/" + testid, typeof(Sprite));
            if (newsprite != null)
            {
                this.GetComponent<SpriteRenderer>().sprite = newsprite;
            }
            else if (defaultsprite != null)
            {
                this.GetComponent<SpriteRenderer>().sprite = defaultsprite;
            }
            //下面那行是要用的，别删
            //this.GetComponent<SpriteRenderer>().sprite = (Sprite)Resources.Load("MMtesttexture/Monster/" + encounterid, typeof(Sprite));
        }
        /// <summary>
        /// 升级所有怪物
        /// </summary>
        public static void UpDateAllMonsters(int i)
        {
            
            foreach (Monster m in monsterlist)
            {
                if(m.encounterid == "Devil_1" || m.encounterid == "Devil_2")
                {
                    //不升级
                }
                else
                {
                    string[] newid = m.encounterid.Split('_');
                    m.SetID(newid[0] + "_" + i.ToString());
                    string[] newtest = m.testid.Split('_');
                    Debug.Log(m.encounterid);
                    //测试用
                    m.testid = newtest[0] + "_" + i.ToString();
                    Debug.Log(m.testid);
                    //
                }
                m.SetTexture();

            }
        }
        /// <summary>
        /// 战斗结束后调用，把战斗结果什么的传回来
        /// </summary>
        public static void AfterBattle()
        {
//            MainMapUI.Instance().ShowMain();
            if (true)//胜利，把怪物毁掉
            {
                MapElementManager.DestroyElement();
            }
            else
            {
                //我也不知道要干嘛
            }

        }
    }
    /// <summary>随机事件
    /// 
    /// </summary>
    public class RandomEvent : MapElement
    {
        protected override void Awake()
        {
           // Debug.Log("随机事件初始化");
        }
        public override void OnClickDetail()
        {
            Debug.Log("随机事件被点击");
            MapElementManager.DestroyElement();
        }
    }
    public class Treasure : MapElement
    {
        protected override void Awake()
        {
           // Debug.Log("宝箱初始化");
        }
        public override void OnClickDetail()
        {
            Debug.Log("宝箱被点击");
            MapElementManager.DestroyElement();
        }
    }


}

