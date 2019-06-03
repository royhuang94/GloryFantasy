using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using IMessage;
using BattleMap;
using GameUnit;

namespace GamePlay.Input
{
    //指令类，在UI下面真正执行游戏逻辑的是这些Command类
    //也就是说 玩家输入鼠标 → GUI → Command → GamePlay，BattaleMap等游戏类

    public class Command : GameplayTool
    {
        virtual public void Excute() { }
    }

    public class SelectUnitCommand : Command
    {
        public SelectUnitCommand(GameUnit.GameUnit unit)
        {
            _unit = unit;
        }

        public override void Excute()
        {
            Gameplay.Info.SelectingUnit = _unit;
        }

        private GameUnit.GameUnit _unit;
    }

    /// <summary>
    /// 部署指令。
    /// </summary>
    /// <param name="unitID">要部署的单位的ID。</param>
    /// <param name="owner">部属单位的操控者。</param>
    /// <param name="battleMapBlock">部署在的地格。</param>
    /// <param name="post">是否需要发送部署消息，可缺省，默认为是。请在特殊情况下才设置为否。</param>
   
    public class DispositionCommand : Command
    {
        public DispositionCommand(string unitID, OwnerEnum owner, BattleMapBlock battleMapBlock, bool post = true)  //构造函数
        {
            _unitID = unitID;
            _owner = owner;
            _battleMapBlock = battleMapBlock;
            _post = post;
        }

        public void set(string unitID, OwnerEnum owner, BattleMapBlock battleMapBlock, bool post = true)    //创建对象后通过此方法修改参数
        {
            _unitID = unitID;
            _owner = owner;
            _battleMapBlock = battleMapBlock;
            _post = post;
        }

        public bool Judge()
        {
            if (_battleMapBlock.units_on_me.Count == 0)
                return true;
            return false;
        }

        public override void Excute()
        {
            //更新本此召唤的怪物（覆盖方式）
            UnitManager.InstantiationUnit(_unitID, _owner, _battleMapBlock);
            if (_post)
            {
                this.SetSummonUnit(_battleMapBlock.units_on_me);
                MsgDispatcher.SendMsg((int)MessageType.Summon);
            }

            //更新仇恨列表
            //Gameplay.Instance().autoController.UpdateAllHatredList(null, _battleMapBlock.units_on_me);
        }

        private string _unitID;
        private OwnerEnum _owner;
        private BattleMapBlock _battleMapBlock;
        private bool _post;
    }

    /// <summary>
    /// 部署指令列，用于一次性部署大量单位。
    /// </summary>
    /// <param name="unitID">要部署的单位的ID。</param>
    /// <param name="owner">部属单位的操控者。</param>
    /// <param name="battleMapBlock">部署在的地格。</param>
    /// <param name="post">是否需要发送部署消息，可缺省，默认为是。请在特殊情况下才设置为否。</param>

    public class DispositionCommandList : Command
    {
        public DispositionCommandList(List<string> unitIDs, List<OwnerEnum> owners, List<BattleMapBlock> battleMapBlocks)
        {
            for (int i = 0; i < unitIDs.Count; i++)
            {
                _dispositionCommands.Add(new DispositionCommand(unitIDs[i], owners[i], battleMapBlocks[i], false));
                _blocks.Add(battleMapBlocks[i]);
            }
        }

        public bool Judge()
        {
            foreach(BattleMapBlock block in _blocks)
            {
                if (block.units_on_me.Count > 0)
                    return false;
            }
            return true;
        }

        public override void Excute()
        {
            List<GameUnit.GameUnit> units = new List<GameUnit.GameUnit>();
            //更新本此召唤的怪物（覆盖方式）
            for (int i = 0; i < _dispositionCommands.Count; i++)
            {
                _dispositionCommands[i].Excute();
                units.AddRange(_blocks[i].units_on_me);
            }

            //更新仇恨列表
        }

        private List<DispositionCommand> _dispositionCommands;
        private List<BattleMapBlock> _blocks;
    }


    public class SkillJumpCommand : Command
    {
        public SkillJumpCommand(GameUnit.GameUnit unit, Vector2 targetPos, int maxJumpPower)
        {
            _unit = unit;
            _targetPos = targetPos;
            _maxJumpPower = maxJumpPower;
        }

        public bool Judge()
        {
            Vector2 unit1 = _unit.CurPos;
            Vector2 unit2 = _targetPos;
            int MAN_HA_DUN = Mathf.Abs((int)unit1.x - (int)unit2.x) + Mathf.Abs((int)unit1.y - (int)unit2.y);
            if (MAN_HA_DUN <= _maxJumpPower)
                return true;
            //BattleMap.BattleMap.Instance().MapNavigator
            return false;
        }

        public override void Excute()
        {
            //Gameplay.Instance().gamePlayInput.HandleSkillConfim(_unit.CurPos, _maxJumpPower);

            if (Judge())
            {
                BattleMap.BattleMap.Instance().MoveUnitToCoordinate(_targetPos, _unit);
                MsgDispatcher.SendMsg((int)MessageType.Moved);
            }

        }

        private GameUnit.GameUnit _unit;
        private Vector2 _targetPos;
        private int _maxJumpPower;
    }

    public class UnitMoveAICommand : Command
    {
        /// <summary>
        /// AI移动类的构造函数
        /// </summary>
        /// <param name="unit">单位</param>
        /// <param name="_toTargetPath">最优路径</param>
        /// <param name="_callback">攻击回调</param>
        public UnitMoveAICommand(GameUnit.GameUnit unit, List<Vector2> _toTargetPath, System.Action _callback)
        {
            _unit = unit;
            toTargetPath = _toTargetPath;
            callback = _callback;
        }

        public override void Excute()
        {
            Debug.Log("Moving Command excusing");
            this.SetMovingUnit(_unit);
            BattleMap.BattleMap.Instance().AIMoveCondition(_unit, toTargetPath, callback);
            //BattleMap.BattleMap.Instance().AIMoveUnitToCoordinate(_unit, toTargetPath, callback);
        }


        private System.Action callback;
        private GameUnit.GameUnit _unit;
        //移动到目标的路径
        private List<Vector2> toTargetPath;
    }

    public class UnitMoveCommand : Command
    {
        public UnitMoveCommand(GameUnit.GameUnit unit, Vector2 unitPositon, Vector2 targetPosion, Vector2 destination)
        {
            _unit = unit;
            _unitPosition = unitPositon;
            _targetPosition = targetPosion;
            _destination = destination;
        }

        public bool Judge()
        {
            Vector2 unit1 = _unitPosition;
            Vector2 unit2 = _targetPosition;
            int MAN_HA_DUN = Mathf.Abs((int)unit1.x - (int)unit2.x) + Mathf.Abs((int)unit1.y - (int)unit2.y);
            if (MAN_HA_DUN <= _unit.getMOV())
                return true;
            //BattleMap.BattleMap.Instance().MapNavigator
            return false;
        }

        public override void Excute()
        {
            Debug.Log("Moving Command excusing");
            if (BattleMap.BattleMap.Instance().MapNavigator.PathSearch(_unitPosition, _targetPosition))
            {
                this.SetMovingUnit(_unit);
                //TODO 产生移动变化，检测
                BattleMap.BattleMap.Instance().MoveUnitToCoordinate(_unit, _targetPosition);
            }

        }

        private GameUnit.GameUnit _unit;
        private Vector2 _destination;
        private Vector2 _unitPosition;
        private Vector2 _targetPosition;
    }

    public class UnitAttackCommand : Command
    {
        public UnitAttackCommand(GameUnit.GameUnit Attacker, GameUnit.GameUnit AttackedUnit)
        {
            _Attacker = Attacker; this.SetAttacker(Attacker);
            _AttackedUnit = AttackedUnit; this.SetAttackedUnit(AttackedUnit);
        }

        /// <summary>
        /// 计算攻击距离是否大于曼哈顿
        /// </summary>
        /// <returns></returns>
        public bool Judge()
        {
            Vector2 unit1 = BattleMap.BattleMap.Instance().GetUnitCoordinate(_Attacker);
            Vector2 unit2 = BattleMap.BattleMap.Instance().GetUnitCoordinate(_AttackedUnit);
            int MAN_HA_DUN = Mathf.Abs((int)unit1.x - (int)unit2.x) + Mathf.Abs((int)unit1.y - (int)unit2.y);
            if (MAN_HA_DUN <= _Attacker.getRNG())
                return true;

            return false;
        }
        /// <summary>
        /// 计算反击距离是否大于曼哈顿
        /// </summary>
        public bool JudgeStrikeBack()
        {
            Vector2 unit1 = BattleMap.BattleMap.Instance().GetUnitCoordinate(_AttackedUnit);
            Vector2 unit2 = BattleMap.BattleMap.Instance().GetUnitCoordinate(_Attacker); 
            int MAN_HA_DUN = Mathf.Abs((int)unit1.x - (int)unit2.x) + Mathf.Abs((int)unit1.y - (int)unit2.y);
            if (MAN_HA_DUN <= _AttackedUnit.getRNG())
                return true;

            return false;
        }

        public override void Excute()
        {
            MsgDispatcher.SendMsg((int)MessageType.AnnounceAttack);
            //根据伤害优先级对伤害请求排序
            DamageRequestList = DamageRequest.CaculateDamageRequestList(_Attacker, _AttackedUnit);

            for (int i = 0; i < DamageRequestList.Count; i++)
            {
                //优先级相同并且两方互打的伤害请求作为同时处理
                if (i != DamageRequestList.Count - 1 && DamageRequestList[i].priority == DamageRequestList[i + 1].priority
                    && DamageRequestList[i]._attacker == DamageRequestList[i + 1]._attackedUnit
                    && DamageRequestList[i]._attackedUnit == DamageRequestList[i + 1]._attacker)
                {
                    //判断被攻击者的反击距离
                    if (JudgeStrikeBack())
                        DamageRequestList[i].ExcuteSameTime();
                    else
                        DamageRequestList[i].Excute(); //距离不够，无法进行反击

                    i++;
                }
                else if (!_AttackedUnit.IsDead() && !_Attacker.IsDead() && JudgeStrikeBack()) //符合反击要求
                {
                    DamageRequestList[i].Excute();
                }
                else if(!_AttackedUnit.IsDead() && !_Attacker.IsDead() && !JudgeStrikeBack()) //距离不够，无法进行反击
                {
                    DamageRequestList[i].Excute();
                    i++;
                }
            }
        }

        //TODO 攻击制作
        //1. 通过变量_Attacker _AttackedUnit 保存宣言攻击者和被攻击者
        //2. 通过DamageRequestList  —> Damange类中
        //3. 通过Damage类与Command类来执行攻击环节，注意细节修改

        private List<DamageRequest> DamageRequestList;
        private GameUnit.GameUnit _Attacker; //宣言攻击者
        private GameUnit.GameUnit _AttackedUnit; //被攻击者
    }

    public class ReleaseSkillCommand : Command
    {
        public ReleaseSkillCommand(GameUnit.GameUnit skillMaker, int range,Vector2 makerPosition,Vector2 targetPosition)
        {
            _skillMaker = skillMaker;
            _range = range;
            _targetPosition = targetPosition;
            _makerPosition = makerPosition;
        }

        //判断技能释放范围目标是否超出释放范围
        public bool Judge()
        {
            Vector2 skillMaker = _makerPosition;
            Vector2 target = _targetPosition;
            int MAN_HA_DUN = Mathf.Abs((int)skillMaker.x - (int)target.x) + Mathf.Abs((int)skillMaker.y - (int)target.y);
            if (MAN_HA_DUN <= _range)
                return true;
            return false;
        }

        public override void Excute()
        {

        }
        private GameUnit.GameUnit _skillMaker;//技能释放者
        private int _range;//技能范围
        private Vector2 _targetPosition;//释放技能的目标点(中心点)
        private Vector2 _makerPosition;//释放者坐标
    }
    /// <summary>
    /// 指令类：创建一个 直接事件对象 并将它加入到事件系统中
    /// </summary>

    public class Command_Creat_DirectEvent : Command
    {
        public int _turn;
        public string _EventID;
        public GamePlay.Event.DirectEvent _DirectEvent;
        /// <summary>
        /// 该指令类的构造函数：创建一个 直接事件对象 并将它加入到事件系统中
        /// </summary>
        /// <param name="expect_trun">希望此事件在 expect_trun 回合生效</param>
        /// <param name="EventID">该事件的事件ID</param>
        public Command_Creat_DirectEvent(int expect_trun, string EventID) //构造函数
        {
            this._turn = expect_trun;
            this._EventID = EventID;
            _DirectEvent = new GamePlay.Event.DirectEvent(EventID, expect_trun);    // 生成 直接事件对象 类
        }
        public override void Excute()   //触发函数
        {
            int now_biggest_turn = Gameplay.Instance().eventScroll.nowBigestTurn;   //获取事件轴的最大回合数
            //
            int delta_turn = now_biggest_turn - _turn;    
            if(delta_turn <= Gameplay.Instance().eventScroll.EventScrollCount - 1 && delta_turn >= 0)  //期望插入的回合已经抽象出了事件队列::此情况下该 直接事件 直接入轴
            {
                Gameplay.Instance().eventScroll.AddDirectEvent_to_Scroll(_DirectEvent);//封装好的：插入 事件轴 函数
            }
            else  if(delta_turn < 0)                                                                  //期望插入的回合暂未抽象出事件队列::此情况下 直接事件 进入仲裁器中 直接事件队列 进行等待
            {
                Gameplay.Instance().eventScroll.AddDirectEvent_to_Judge(_DirectEvent);//封装好的：插入 直接事件队列 函数
            }
            else                                                                                       //这种情况是 想要插入已经“过期的事件”导致的 如在最大回合数为100的事件轴中插入第2回合触发的事件
            {
                //todo:加入错误提示
            }
            {

            }
        }
    }

}