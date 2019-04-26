using System;
using System.Collections;
using System.Collections.Generic;
using BattleMap;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using GamePlay;

namespace GameUnit
{

    /// <summary>
    /// 提供一些与Unit相关的方法
    /// </summary>
    public class UnitManager
    {


        //角色初始化到地图上
        public static void InstantiationUnit(string cardID, OwnerEnum owner, Transform parent)
        {
            //根据卡牌id生成单位
            GameObject temp = GameUnitPool.Instance().GetInst(cardID, owner);
            //TODO:修改连接关系
            //修改单位的父级对象
            temp.transform.SetParent(parent);
            //修改单位的本地坐标系坐标
            temp.transform.localPosition = Vector3.zero;
            //修改单位卡图的射线拦截设置
            temp.GetComponent<Image>().raycastTarget = true;

            var hpTest = temp.transform.GetChild(0);
            //TODO 暂时用Text标识血量，以后改为slider
            hpTest.gameObject.SetActive(true);
            float hp = (temp.GetComponent<GameUnit>().hp = temp.GetComponent<GameUnit>().hp);
            float hpDivMaxHp = hp / temp.GetComponent<GameUnit>().MaxHP * 100;

            hpTest.GetComponent<Text>().text = string.Format("HP: {0}%", hpDivMaxHp);

            //挂载ShowRange脚本
            temp.AddComponent<GameGUI.ShowRange>();

            //获取GameUnit对象
            GameUnit gameUnit = temp.GetComponent<GameUnit>();

            //添加当前实例单位到UnitList中
            BattleMap.BattleMap.Instance().UnitsList.Add(gameUnit);
            //添加当前实例单位的所在地图块儿
            gameUnit.mapBlockBelow = parent.gameObject.GetComponent<BattleMapBlock>();

        }
    }
}