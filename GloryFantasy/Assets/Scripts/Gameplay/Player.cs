using System.Collections;
using System.Collections.Generic;
using GamePlay.Encounter;
using IMessage;
using UnityEngine;
using UnityEngine.UI;

namespace GamePlay
{
    public enum PlayerEnum
    {
        Player,
        AI
    };

    public class Player: UnitySingleton<Player>, IMessage.MsgReceiver
    {
        
        #region 变量
        /// <summary>
        /// 专注值的上限
        /// </summary>
        private int _apUpLimit = 99;
        
        /// <summary>
        /// 专注值，计算方式为：默认专注值 + 增益专注值
        /// </summary>
        private int _ap;
        
        /// <summary>
        /// 默认专注值
        /// </summary>
        private int _defaultAp;
        
        ///// <summary>
        ///// 增益专注值，用于存储因为buff或者其他因素对于已有专注值的影响
        ///// </summary>
        //private int _addOnAp;
        /// <summary>
        /// 战败死页数
        /// </summary>
        private int _maxDeathPage;
        /// <summary>
        /// 当前死页数
        /// </summary>
        private int _deathPage;
        /// <summary>
        /// 具有领导标记的单位
        /// </summary>
        private List<GameUnit.GameUnit> _leaders;
        /// <summary>
        /// 每回合抓牌数
        /// </summary>
        public int drawsEachTurn;
        #endregion

        #region 变量可见性定义

        public int ap
        {
            get{ return _ap; }
            private set
            {
                _ap = value;
                MsgDispatcher.SendMsg((int)MessageType.APChange);
            }
        }

        /// <summary>
        /// 战败死页数
        /// </summary>
        public int MaxDeathPage { get { return _maxDeathPage; } }
        /// <summary>
        /// 当前死页数
        /// </summary>
        public int DeathPage { get { return _deathPage; } }

        public int apUpLimit
        {
            get { return _apUpLimit; }
        }

        public int defaultAp
        {
            get { return _defaultAp; }
        }

        //public int addOnAp
        //{
        //    get { return _addOnAp; }
        //}

        public List<GameUnit.GameUnit> leaders
        {
            get { return _leaders; }
        }
        #endregion

        #region 初始化方法
        /// <summary>
        /// 初始化遭遇数据
        /// </summary>
        private void InitEncounterData(string encounterID)
        {
            _maxDeathPage = EncouterData.Instance()._encounterData[encounterID].deathPage;
        }
        /// <summary>
        /// 初始化玩家数据
        /// </summary>
        /// <param name="playerdata"></param>
        private void InitPlayerData(Playerdata playerdata)
        {
            _defaultAp = playerdata.apLimit;
            drawsEachTurn = playerdata.drawsEachTurn;
            // TODO: 天赋系统
        }
        /// <summary>
        /// 初始化玩家信息和遭遇信息
        /// </summary>
        public void Init()
        {
            InitEncounterData(SceneSwitchController.Instance().encounterId);
            InitPlayerData(SceneSwitchController.Instance().playerdata);
        }
        #endregion
        /// <summary>
        /// 增加死页
        /// </summary>
        /// <param name="amount"></param>
        public void AddDeathPage(int amount)
        {
            _deathPage += amount;
        }

        /// <summary>
        /// 获取"领导者"
        /// </summary>
        public List<GameUnit.GameUnit> GetLeaders()
        {
            List<GameUnit.GameUnit> leaders = new List<GameUnit.GameUnit>();
            foreach (GameUnit.GameUnit unit in BattleMap.BattleMap.Instance().UnitsList)
            {
                if (unit.isLeader == 1)
                {
                    leaders.Add(unit);
                }
            }
            return leaders;
        }

        private void Start()
        {
            
        }

        /// <summary>
        /// 用于注册的condition函数，现在没有具体限制所以返回值永远为true
        /// </summary>
        /// <returns>根据情况返回是否可以进行Action</returns>
        public bool canDoAction()
        {
            return true;
        }

        /// <summary>
        /// 初始化Ap值，根据当前策划，默认上限为3
        /// </summary>
        //public void InitAp()
        //{
        //    //_apUpLimit = 30;
        //    _apUpLimit = 3;
        //    _defaultAp = _apUpLimit;
        //    _addOnAp = 0;
        //    ReCalculateAp();
        //}
        
        /// <summary>
        /// 增加ap值的接口
        /// </summary>
        /// <param name="Ap">要增加的ap值</param>
        public void AddAp(int Ap)
        {
            ap += Ap;
        }

        /// <summary>
        /// 增加ap值的上限值接口，并不立即生效
        /// </summary>
        /// <param name="ApLimit">要增加的上限数值<param>
        //public void AddApUpLimit(int ApLimit)
        //{
        //    _addOnAp += ApLimit;
        //}


        /// <summary>
        /// 清除附加ap值效果，请于回合开始时进行调用
        /// </summary>
        //public void ClearAddOnAp()
        //{
        //    _addOnAp = 0;
        //    ReCalculateAp();
        //}

        /// <summary>
        /// 重新计算ap值的接口,用于更新ap值时调用，也是Action函数
        /// </summary>
        public void ReCalculateAp()
        {
            ap = _defaultAp;
            
            // 超出专注值上限时进行求余操作，等于上限时不做变化
            if (ap > _apUpLimit)
                ap %= _apUpLimit;
        }

        
        /// <summary>
        /// 消耗ap值接口，返回值确定是否成功消耗
        /// </summary>
        /// <param name="Ap">消耗的ap值</param>
        /// <returns>若成功消耗返回true，否则为false并不改变玩家当前ap值</returns>
        public bool CanConsumeAp(int Ap)
        {
            if (_ap < Ap)
                return false;
            
            return true;
        }

        /// <summary>
        /// 消耗AP值的接口，请确定能消耗之后再进行消耗
        /// </summary>
        /// <param name="Ap">要消耗掉的ap值</param>
        public void ConsumeAp(int Ap)
        {
            ap -= Ap;
        }

        /// <summary>
        /// 仿照主程写的写的接口
        /// </summary>
        T IMessage.MsgReceiver.GetUnit<T>()
        {
            return this as T;
        }

        internal class instance
        {
        }
    }

    public class Playerdata
    {
        /// <summary>
        /// 初始手牌数
        /// </summary>
        public int startHand;
        /// <summary>
        /// 每回合抓牌数
        /// </summary>
        public int drawsEachTurn;
        /// <summary>
        /// 每回合恢复到的ap值
        /// </summary>
        public int apLimit;
        /// <summary>
        /// 战斗专长（天赋）。进入战斗时的一些全局增益。当前版本留空就好。
        /// </summary>
        public List<string> feats;
        /// <summary>
        /// 玩家信息的构造方法
        /// </summary>
        /// <param name="startHand">起始手牌数</param>
        /// <param name="drawsEachTurn">每回合抓牌数</param>
        /// <param name="apLimit">默认专注值</param>
        /// <param name="feats">战斗专长</param>
        public Playerdata(int startHand = 2, int drawsEachTurn = 1, int apLimit = 3, List<string> feats = null)
        {
            this.startHand = startHand;
            this.drawsEachTurn = drawsEachTurn;
            this.apLimit = apLimit;
            this.feats = feats;
        }
    }

    public class Computer
    {

    }
}