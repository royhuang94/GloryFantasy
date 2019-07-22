using Ability.Buff;
using BattleMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cards;
using IMessage;
using UnityEngine;
using GamePlay;
using GameCard;

namespace GameUnit
{
    public class HeroManager
    {
        private List<UnitHero> unitHeroes;
        // TODO:英雄CD池
        public List<HeroCD> CDHeros;
        public List<UnitHero> done;

        public void init(Deck deck)
        {
            done = new List<UnitHero>();
            unitHeroes = new List<UnitHero>();
            CDHeros = new List<HeroCD>();

            foreach(string heroid in deck._heroes.Keys)
            {
                HeroData data = deck._heroes[heroid];
                //根据卡牌id生成单位
                GameObject temp = GameUnitFactory.Instance().GetGameUnit(OwnerEnum.Player);
                Debug.Log("temp: " + temp);
                //把FriendlyUnit掰了换成新的
                if (temp.GetComponent<FriendlyUnit>() != null)
                    GameObject.Destroy(temp.GetComponent<FriendlyUnit>());
                UnitHero hero = temp.AddComponent<UnitHero>();
                unitHeroes.Add(hero);
                hero.isLeader = 0;
                hero.id = data.id;
                hero.owner = OwnerEnum.Player;
                hero.recovorRate = 4;
                hero.maxRecovery = 0;
                CDHeros.Add(new HeroCD(hero));
                hero.extraAbilities = data.additionalAbility;
                hero.Arks = new List<BaseCard>();
                foreach (string cardid in data.arks)
                {
                    BaseCard card = CardDataBase.Instance().GetCardInstanceById(cardid);
                    card.setCarrier(hero);
                    hero.Arks.Add(card);
                    GameplayToolExtend.moveCard(card, CardArea.Deck);
                    hero.extraAbilities.AddRange(card.abilityAttach);
                }
            }
            // TODO: 刷新CD池
        }

        public void sendHeroInCD(UnitHero hero)
        {
            CDHeros.Add(new HeroCD(hero));
        }

        public void fresh()
        {
            foreach(HeroCD cd in CDHeros)
            {
                if (cd.currentRecovery < cd.maxRecovery)
                    cd.currentRecovery += cd.recovorRate;
                else if (!done.Contains(cd.hero))
                {
                    done.Add(cd.hero);
                }
            }
            // 更新CD的UI显示
        }
    }
    public class HeroCD
    {
        /// <summary>
        /// 当前恢复量
        /// </summary>
        public int currentRecovery;
        /// <summary>
        /// 重新部署所需的恢复量
        /// </summary>
        public int maxRecovery;
        /// <summary>
        /// 查询此单位剩余需要恢复多少回合
        /// </summary>
        /// <returns></returns>
        public int leftCD()
        {
            return (currentRecovery + recovorRate - 1) / recovorRate;
        }
        /// <summary>
        /// 保存的英雄实例
        /// </summary>
        public UnitHero hero;
        /// <summary>
        /// 每回合恢复率，默认为4
        /// </summary>
        public int recovorRate;
        

        public HeroCD(UnitHero hero)
        {
            this.hero = hero;
            this.currentRecovery = 0;
            this.maxRecovery = hero.maxRecovery;
            this.recovorRate = hero.recovorRate;
            hero.CDObject = this;
        }
    }

    public class UnitHero: FriendlyUnit
    {
        
        /// <summary>
        /// 单位数据库上未记载的额外异能
        /// </summary>
        public List<string> extraAbilities;
        /// <summary>
        /// 携带的CD状态
        /// </summary>
        public HeroCD CDObject;
        /// <summary>
        /// 每回合恢复率，默认为4
        /// </summary>
        public int recovorRate;
        /// <summary>
        /// 重新部署所需的恢复量
        /// </summary>
        public int maxRecovery;
        /// <summary>
        /// 英雄携带的卡牌实例
        /// </summary>
        public List<BaseCard> Arks;
        /// <summary>
        /// 英雄的部署方法
        /// </summary>
        /// <param name="mapBlock"></param>
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
            this.maxRecovery = this.getMHP();
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
            this.IsDead = false;
            UnitManager.UpdateChessImg(this.name, this);
            UnitManager.AddEventModule(this);
            // 清除CD
            GamePlay.Gameplay.Instance().heroManager.CDHeros.Remove(this.CDObject);
            this.CDObject = null;
            Gameplay.Info.GeneratingUnit = this;
            MsgDispatcher.SendMsg((int)MessageType.GenerateUnit);
            //更新仇恨列表
            Gameplay.Instance().autoController.UpdateAllHatredList(null, mapBlock.units_on_me);
            Gameplay.Info.SummonUnit = new List<GameUnit> { this };
            MsgDispatcher.SendMsg((int)MessageType.Summon);
            // TODO: 更新CD池
        }
    }
}
