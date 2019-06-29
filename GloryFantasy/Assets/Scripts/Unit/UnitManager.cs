using System;
using System.Collections;
using System.Collections.Generic;
using BattleMap;
using GameCard;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using GamePlay.Encounter;

using GamePlay;
using IMessage;
using LitJson;
using FairyGUI;

namespace GameUnit
{
    public enum HeroActionState
    {
        Normal,                  //正常
        WaitForPlayerChoose,     //等待玩家操作
        BattleEnd,               //战斗结束
        Error,                   //错误
        Warn,                    //警告(测试用)
    }

    /// <summary>
    /// 提供一些与Unit相关的方法
    /// </summary>
    public class UnitManager : GameplayTool
    {
      
        /// <summary>
        /// 击杀单位。击杀者为空时单表示单位死亡。
        /// </summary>
        /// <param name="killer">击杀者，可以为null。</param>
        /// <param name="beKilled">死亡者。</param>
        public static void Kill(GameUnit killer, GameUnit beKilled)
        {
            UnitManager _unitManager = new UnitManager();
            _unitManager.SetKiller(killer);
            _unitManager.SetKilledAndDeadUnit(beKilled);
            //死亡单位回收到对象池
            GameUnitPool.Instance().UnitBackPool(beKilled);

            //删除对应controller中的死亡单位
            Gameplay.Instance().autoController.UpdateAllHatredList();

            //删除对应的事件模型
            if (beKilled.EventModule != null)
            {
                beKilled.EventModule.DeleteThisModule();
                beKilled.EventModule = null;
                beKilled.eventsInfo.Clear();
            }
            if (killer != null)
                MsgDispatcher.SendMsg((int)MessageType.Kill);
            MsgDispatcher.SendMsg((int)MessageType.Dead);
            MsgDispatcher.SendMsg((int)MessageType.BattleSate);//单位死亡更新战区所属状态
        }
        //角色从卡牌初始化到地图上
        public static void InstantiationUnit(string cardID, OwnerEnum owner, BattleMapBlock battleMapBlock)
        {
            //根据卡牌id生成单位
            GameObject temp = GameUnitPool.Instance().GetInst(cardID, owner);
            Debug.Log("temp: " + temp);
            //获取GameUnit对象
            GameUnit gameUnit = temp.GetComponent<GameUnit>(); ;

            //添加当前实例单位到UnitList中
            BattleMap.BattleMap.Instance().UnitsList.Add(gameUnit);

            Debug.Log("gameUnit " + gameUnit);
            //添加当前实例单位的所在地图块儿
            gameUnit.mapBlockBelow = battleMapBlock;

            //添加gameUnit到units_on_me上，且修改单位的父级对象
            battleMapBlock.AddUnit(gameUnit);
            //修改单位的本地坐标系坐标
            temp.transform.localPosition = Vector3.zero;
            //修改单位卡图的射线拦截设置

            //TODO 暂时用Text标识血量，以后改为slider
            SetHpInfo(gameUnit);

            //单位部署相当于单位驻足地图块儿
            gameUnit.nextPos = gameUnit.CurPos;

            //部署成功
            Gameplay.Instance().bmbColliderManager.Fresh();
            AddEventModule(gameUnit);
            //Debug.LogFormat("EventModuleListCount: {0}", Gameplay.Instance().eventScroll.EventModuleListCount);

            //if(gameUnit.owner == OwnerEnum.Enemy)
            //{
            //    AI.SingleController controller;
            //    //初始化AI控制器与携带的仇恨列表
            //    if (BattleMap.BattleMap.Instance().UnitsList.Count %2 != 0)
            //        controller = new AI.SingleAutoControllerAtker(gameUnit); //无脑型
            //    else
            //        controller = new AI.SingleAutoControllerDefender(gameUnit);//防守型
            //    controller.hatredRecorder.Reset(gameUnit);
            //    GamePlay.Gameplay.Instance().autoController.singleControllers.Add(controller);
            //}
            Gameplay.Info.GeneratingUnit = gameUnit;
            MsgDispatcher.SendMsg((int)MessageType.GenerateUnit);
//            if (gameUnit.tag.Contains("英雄"))
//                temp.AddComponent<ESSlot>();
        }

        /// <summary>
        /// 初始战斗地图上的单位
        /// </summary>
        /// <param name="encounterID">遭遇id</param>
        public static void InitAndInstantiateGameUnit(string encounterID, BattleMapBlock[,] _mapBlocks)
        {
            Encounter encounter = null;
            EncouterData.Instance()._encounterData.TryGetValue(encounterID, out encounter);
            if (encounter == null)
                return;

            OwnerEnum owner;
            GameObject _object;
            for (int i = 0; i < encounter.unitMessageList.Count; i++)
            {
                UnitMessage unitMessage = encounter.unitMessageList[i];
                int x = unitMessage.pos_X;
                int y = unitMessage.pos_Y;
                int isLeader = unitMessage.isLeader;
                //单位控制者:0为玩家，1为敌方AI_1,2为敌方AI_2，...
                switch (unitMessage.unitControler.ToString())
                {
                    case ("0"):
                        owner = OwnerEnum.Player; break;
                    case ("1"):
                        owner = OwnerEnum.Enemy; break;
                    default:
                        owner = OwnerEnum.Enemy; break;
                }
                //从对象池获取单位
                _object = GameUnitPool.Instance().GetInst(unitMessage.unitID, owner,isLeader);

                GameUnit unit = _object.GetComponent<GameUnit>();
                //修改单位对象的父级为地图方块
                Debug.Log(string.Format("Add Unit on Map:({0},{1})", x, y));
                _mapBlocks[x, y].AddUnit(unit);

                List<GameUnit> _unitsList = BattleMap.BattleMap.Instance().UnitsList;
                _unitsList.Add(unit);
                unit.mapBlockBelow = _mapBlocks[x, y];

                AI.SingleController controller;
                //初始化AI控制器与携带的仇恨列表
                if (_unitsList.Count == 1 || _unitsList.Count == 3 || _unitsList.Count == 5)
                    controller = new AI.SingleAutoControllerAtker(unit); //无脑型
                else
                    controller = new AI.SingleAutoControllerDefender(unit);//防守型
                controller.hatredRecorder.Reset(unit);
                GamePlay.Gameplay.Instance().autoController.singleControllers.Add(controller);

                //单位部署相当于单位驻足地图块儿
                unit.nextPos = unit.CurPos;
                //部署成功
                Gameplay.Instance().bmbColliderManager.Fresh();



                //UIPanel panel = _object.transform.Find("HeadBar").GetComponent<UIPanel>();
                //panel.ui.GetChild("name").text = unit.name;
                //float hp = unit.hp/* - Random.Range(2, 6)*/;
                //float maxHp = unit.getMHP();
                //float hpDivMaxHp = hp / maxHp * 100;
                //panel.ui.GetChild("blood").asProgress.value = hpDivMaxHp;
                //panel.ui.GetChild("sign").asLoader.url = "ui://HeadBar/none";

                SetHpInfo(unit);
                AddEventModule(unit);
            }
            Debug.LogFormat("EventModuleListCount: {0}", Gameplay.Instance().eventScroll.EventModuleListCount);
        }

        /// <summary>
        /// 设置血条信息
        /// </summary>
        /// <param name="unit"></param>
        public static void SetHpInfo(GameUnit unit)
        {
            if (unit == null)
                return;

            UIPanel panel = unit.gameObject.transform.Find("HeadBar").GetComponent<UIPanel>();
            panel.ui.GetChild("name").text = null;
            float hp = unit.hp/* - Random.Range(2, 6)*/;
            float maxHp = unit.getMHP();
            float hpDivMaxHp = hp / maxHp * 100;
            if (hpDivMaxHp > 100)
                hpDivMaxHp = 100;

            panel.ui.GetChild("blood").asProgress.value = hpDivMaxHp;
            panel.ui.GetChild("sign").asLoader.url = "ui://HeadBar/none";
        }

        /// <summary>
        /// 获取地图块儿上的list 单位
        /// </summary>
        /// <param name="position">单位位置</param>
        /// <returns></returns>
        public static List<GameUnit> GetUnitFromBattleMapBlock(Vector2 position)
        {
            BattleMap.BattleMapBlock mapBlock = BattleMap.BattleMap.Instance().GetSpecificMapBlock(position);
            return mapBlock.units_on_me;
        }


        /// <summary>
        /// 读取每个事件模块中的信息
        /// </summary>
        private static void AddEventModule(GameUnit unit)
        {
            if (unit.EventModule != null)
            {
                Gameplay.Instance().eventScroll.AddEventModule(unit.EventModule);
            }
        }

        public static void ColorUnitOnBlock(Vector3 position, Color color)
        {
            GameUnit unit = BattleMap.BattleMap.Instance().GetUnitsOnMapBlock(position);

            if (unit != null)
            {
                unit.GetComponent<SpriteRenderer>().color = color;
                unit.transform.Find(unit.owner.ToString()).GetComponent<SpriteRenderer>().color = color;
            }
        }

        /// <summary>
        /// 更新棋子头像
        /// </summary>
        /// <param name="id">棋子name，name与头像名相同</param>
        public static void UpdateChessImg(string name, GameUnit gameUnit)
        {
            SpriteRenderer spr = GameObject.Find("name").GetComponent<SpriteRenderer>(); ;

            Texture2D texture2d = (Texture2D)Resources.Load("BattleMapUnitAssets/" + name);
            Sprite sp = Sprite.Create(texture2d, spr.sprite.textureRect, new Vector2(0.5f, 0.5f));
            spr.sprite = sp;
        }
    }
}