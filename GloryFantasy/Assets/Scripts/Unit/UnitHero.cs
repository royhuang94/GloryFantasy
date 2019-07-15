using Ability.Buff;
using BattleMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IMessage;
using UnityEngine;
using GamePlay;

namespace GameUnit
{
    public class HeroManager
    {
        private List<UnitHero> unitHeroes;
        // TODO:英雄CD池
    }
    public class UnitHero: GameUnit
    {
        int revorRate;
        List<string> extraAbilities;

        public void dispose(BattleMapBlock mapBlock)
        {
            // 清除triggers、Buff和异能。
            MsgDispatcher.clearHandlers(this.GetMsgReceiver());
            foreach (Buff _buff in this.GetComponents<Buff>())
            {
                Destroy(_buff);
            }
            foreach (Ability.Ability _ability in this.GetComponents<Ability.Ability>())
            {
                Destroy(_ability);
            }
            UnitDataBase.Instance().InitGameUnit(this, id, this.owner, this.isLeader);

            // 添加额外异能。
            for (int i = 0; i < extraAbilities.Count; i++)
            {
                if (extraAbilities[i].ToString() == "") continue;
                this.abilities.Add(extraAbilities[i].ToString());
                Component ability =
                    this.gameObject.AddComponent(
                        System.Type.GetType("Ability." + extraAbilities[i].ToString().Split('_').First()));
                if (ability != null)
                {
                    GameUtility.UtilityHelper.Log("添加异能 " + extraAbilities[i].ToString() + " 成功",
                        GameUtility.LogColor.RED);
                    Ability.Ability a = ability as Ability.Ability;
                    a.Init(extraAbilities[i].ToString());
                }
                else
                {
                    GameUtility.UtilityHelper.Log("添加异能 " + extraAbilities[i].ToString() + " 失败",
                        GameUtility.LogColor.RED);
                }
            }

            BattleMap.BattleMap.Instance().UnitsList.Add(this);

            Debug.Log("gameUnit " + this);
            //添加当前实例单位的所在地图块儿
            this.mapBlockBelow = mapBlock;

            //添加gameUnit到units_on_me上，且修改单位的父级对象
            mapBlock.AddUnit(this);
            //修改单位的本地坐标系坐标
            this.GetComponent<GameObject>().transform.localPosition = Vector3.zero;
            //修改单位卡图的射线拦截设置

            //TODO 暂时用Text标识血量，以后改为slider
            UnitManager.SetHpInfo(this);

            //单位部署相当于单位驻足地图块儿
            this.nextPos = this.CurPos;

            //部署成功
            UnitManager.UpdateChessImg(this.name, this);
            UnitManager.AddEventModule(this);
            Gameplay.Info.GeneratingUnit = this;
            MsgDispatcher.SendMsg((int)MessageType.GenerateUnit);
            //更新仇恨列表
            Gameplay.Instance().autoController.UpdateAllHatredList(null, mapBlock.units_on_me);
            Gameplay.Info.SummonUnit = new List<GameUnit> { this };
            MsgDispatcher.SendMsg((int)MessageType.Summon);
        }
    }
}
