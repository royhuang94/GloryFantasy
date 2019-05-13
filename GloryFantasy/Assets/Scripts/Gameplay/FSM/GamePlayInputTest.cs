using BattleMap;
using GamePlay.Input;
using IMessage;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GamePlay.FSM
{
    public class BattleUnitAction: IMessage.MsgReceiver
    {
        // TODO: 这是地图上单位的基类，请继承此类进行行为描述
        T IMessage.MsgReceiver.GetUnit<T>()
        {
            return this as T;
        }
        public BattleUnitAction()
        {
            //注册Idle -> PerpareMove消息
            MsgDispatcher.RegisterMsg(
                this.GetMsgReceiver(),
                (int)MessageType.SIdleToSPerpareMove,
                CanStateTransition,
                 IdleTransitionToPerpareMove,
                "Translation to Move Action"
                );
            //注册 PerpareMove -> Idle 消息
            MsgDispatcher.RegisterMsg(
                this.GetMsgReceiver(),
                (int)MessageType.SPerpareMoveToSIdle,
                CanStateTransition,
                PerpareTransitionToIdle,
                "Translation to Idle Action"
                );
            //注册StartMove -> Idle 消息
            MsgDispatcher.RegisterMsg(
                this.GetMsgReceiver(),
                (int)MessageType.SStartMoveToSIdle,
                CanStateTransition,
                StartMoveTransitionToIdle,
                "Translation to Idle Action"
                );
            //注册Idle -> StartMove 消息
            MsgDispatcher.RegisterMsg(
                this.GetMsgReceiver(),
                (int)MessageType.SIdleToSStartMove,
                CanStateTransition,
                IdleTransitionToStartMove,
                "Translation to Idle Action"
                );
            //注册Idle -> Atk 消息
            MsgDispatcher.RegisterMsg(
                this.GetMsgReceiver(),
                (int)MessageType.SIdleToAtk,
                CanStateTransition,
                IdleTransitionToAtk,
                "Translation to Idle Action"
                );
            //注册Atk -> Idle消息
            MsgDispatcher.RegisterMsg(
                this.GetMsgReceiver(),
                (int)MessageType.SAtkToSIdle,
                CanStateTransition,
                AtkTransitionToIdle,
                "Translation to Idle Action"
                );

        }

        public bool CanStateTransition()
        {
            return true;
        }

        public void IdleTransitionToAny()
        {
            Debug.Log(Gameplay.Instance().gamePlayInput.FSM.CurrentState.StateName + "状态转换至" + "MoveAction");

            Gameplay.Instance().gamePlayInput.FSM.CurrentState.Trigger("BACK_NONE");
        }

        public void AnyTransitionToIdle()
        {
            Debug.Log(Gameplay.Instance().gamePlayInput.FSM.CurrentState.StateName + "状态转换至" + "MoveAction");

            Gameplay.Instance().gamePlayInput.FSM.CurrentState.Trigger("WAIT_PLAYER");
        }

        public void PerpareTransitionToIdle()
        {
            Debug.Log(Gameplay.Instance().gamePlayInput.FSM.CurrentState.StateName + "状态转换至" + "MoveAction");

            Gameplay.Instance().gamePlayInput.FSM.CurrentState.Trigger("WAIT_PLAYER");
            //TODO 此处触发FSM的状态转换
            //FSM.CurrentState.Trigger( "XXXX" );
        }

        public void IdleTransitionToPerpareMove()
        {
            Debug.Log(Gameplay.Instance().gamePlayInput.FSM.CurrentState.StateName + "状态转换至" + "MoveAction");

            Gameplay.Instance().gamePlayInput.FSM.CurrentState.Trigger("PREPARE_MOVING");
            //TODO 此处触发FSM的状态转换
            //FSM.CurrentState.Trigger( "XXXX" );
        }

        public void StartMoveTransitionToIdle()
        {
            Debug.Log(Gameplay.Instance().gamePlayInput.FSM.CurrentState.StateName + "状态转换至" + "MoveAction");

            Gameplay.Instance().gamePlayInput.FSM.CurrentState.Trigger("WAIT_PLAYER");
            //TODO 此处触发FSM的状态转换
            //FSM.CurrentState.Trigger( "XXXX" );
        }

        public void IdleTransitionToStartMove()
        {
            Debug.Log(Gameplay.Instance().gamePlayInput.FSM.CurrentState.StateName + "状态转换至" + "MoveAction");

            Gameplay.Instance().gamePlayInput.FSM.CurrentState.Trigger("START_MOVING");
            //TODO 此处触发FSM的状态转换
            //FSM.CurrentState.Trigger( "XXXX" );
        }
        public void IdleTransitionToAtk()
        {
            Debug.Log(Gameplay.Instance().gamePlayInput.FSM.CurrentState.StateName + "状态转换至" + "MoveAction");

            Gameplay.Instance().gamePlayInput.FSM.CurrentState.Trigger("START_ATTACK");
            //TODO 此处触发FSM的状态转换
            //FSM.CurrentState.Trigger( "XXXX" );
        }
        public void AtkTransitionToIdle()
        {
            Debug.Log(Gameplay.Instance().gamePlayInput.FSM.CurrentState.StateName + "状态转换至" + "MoveAction");

            Gameplay.Instance().gamePlayInput.FSM.CurrentState.Trigger("WAIT_PLAYER");
            //TODO 此处触发FSM的状态转换
            //FSM.CurrentState.Trigger( "XXXX" );
        }
    }


    public class BattleUnitNoneAction : IState
    {
        public void OnEnter(string prevState)
        {
            Debug.Log("无状态");
        }

        public void OnExit(string nextState)
        {
        }

        public void OnUpdate()
        { 
        }
    }

    public class BattleUnitWaitPlayer : IState
    {
        //private GameUnit.GameUnit unit;
        //private Input.GameplayInput gameplayInput;

        public void OnEnter(string prevState)
        {
            Debug.Log("进入等待状态");

        }

        public void OnExit(string nextState)
        {
        }

        public void OnUpdate()
        {
        }
    }

    public class BattleUnitPerpareMoveAction : IState
    {
        private GameUnit.GameUnit unit;
        private Input.GameplayInput gameplayInput;

        public void OnEnter(string prevState)
        {
            Debug.Log("进入准备移动状态");
            gameplayInput = Gameplay.Instance().gamePlayInput;
            unit = gameplayInput.SelectedUnit;
            //TODO 对于对单位释放效果牌的处理，之后思考
            PlayPerpareMoveAction();
        }

        public void OnExit(string nextState)
        {

        }

        public void OnUpdate()
        {

        }

        private void PlayPerpareMoveAction()
        {
            if (gameplayInput.IsMoving)
            {
                //如果移动两次都选择同一个单位，就进行一次待机
                Vector2 pos = BattleMap.BattleMap.Instance().GetUnitCoordinate(unit);
                if (unit.CurPos == pos)
                {
                    GameUtility.UtilityHelper.Log("取消移动，进入攻击,再次点击角色取消攻击", GameUtility.LogColor.RED);
                    gameplayInput.SetMovingIsFalse(unit);
                    gameplayInput.HandleAtkConfirm(BattleMap.BattleMap.Instance().GetUnitCoordinate(unit));
                    unit.restrain = true;
                    gameplayInput.IsAttacking = true; //阔能会被删除
                }
                else
                {
                    //点到其他单位什么都不做
                }
            }            //如果单位可以移动
            else if (unit.restrain == false && unit.owner == GameUnit.OwnerEnum.Player)
            {
                GameUtility.UtilityHelper.Log("准备移动，再次点击角色取消移动进入攻击", GameUtility.LogColor.RED);
                gameplayInput.SetMovingIsTrue(unit);
            }
            //如果单位已经不能移动，但是可以攻击
            else if (unit.restrain == true && unit.disarm == false)
            {
                gameplayInput.BeforeMoveGameUnits.Add(unit);
                GameUtility.UtilityHelper.Log("准备攻击，右键取消攻击", GameUtility.LogColor.RED);
                gameplayInput.IsAttacking = true;
                gameplayInput.TargetList.Add(BattleMap.BattleMap.Instance().GetUnitCoordinate(unit));
            }
            MsgDispatcher.SendMsg((int)MessageType.SPerpareMoveToSIdle);
        }
    }


    public class BattleUnitStartMoveAction : IState
    {
        private Input.GameplayInput gameplayInput;
        private BattleMapBlock battleMapBlock;

        public void OnEnter(string prevState)
        {
            Debug.Log("进入开始移动状态");
            gameplayInput = Gameplay.Instance().gamePlayInput;
            battleMapBlock = gameplayInput.SelectedBlock;

            PlayStartMoveAction();
        }

        public void OnExit(string nextState)
        {
        }

        public void OnUpdate()
        {
        }

        public void PlayStartMoveAction()
        {
            if (gameplayInput.IsMoving)
            {
                GameUnit.GameUnit unit = BattleMap.BattleMap.Instance().GetUnitsOnMapBlock(gameplayInput.TargetList[0]);
                Vector2 startPos = gameplayInput.TargetList[0];
                Vector2 endPos = battleMapBlock.position;
                UnitMoveCommand unitMove = new UnitMoveCommand(unit, startPos, endPos, battleMapBlock.GetSelfPosition());
                if (unitMove.Judge())
                {
                    GameUtility.UtilityHelper.Log("移动完成，进入攻击状态，点击敌人进行攻击，右键点击角色取消攻击", GameUtility.LogColor.RED);
                    unitMove.Excute();
                    gameplayInput.SetMovingIsFalse(unit);//并清空targetList
                    gameplayInput.IsAttacking = true;
                    unit.restrain = true;
                    unit.disarm = true;
                }
                else
                {
                    //如果不符合移动条件，什么都不做
                }
            }
            MsgDispatcher.SendMsg((int)MessageType.SStartMoveToSIdle);
        }
    }

    public class BattleUnitAttackAction : IState
    {
        private Input.GameplayInput gameplayInput;
        private GameUnit.GameUnit unit;
        private BattleMapBlock battleMapBlock;

        public void OnEnter(string prevState)
        {
            Debug.Log("进入攻击状态");
            gameplayInput = Gameplay.Instance().gamePlayInput;
            battleMapBlock = gameplayInput.SelectedBlock;
            unit = gameplayInput.SelectedUnit;
            PlayAtkAction();
        }

        public void OnExit(string nextState)
        {
        }

        public void OnUpdate()
        {
        }

        public void PlayAtkAction()
        {
            if (gameplayInput.IsAttacking)
            {
                if (unit.owner == GameUnit.OwnerEnum.Enemy)
                {
                    //获取攻击者和被攻击者
                    Debug.Log(gameplayInput.BeforeMoveGameUnits[0]);
                    GameUnit.GameUnit Attacker = gameplayInput.BeforeMoveGameUnits[0];
                    GameUnit.GameUnit AttackedUnit = unit;
                    //创建攻击指令
                    UnitAttackCommand unitAtk = new UnitAttackCommand(Attacker, AttackedUnit);
                    //如果攻击指令符合条件则执行
                    if (unitAtk.Judge())
                    {
                        GameUtility.UtilityHelper.Log("触发攻击", GameUtility.LogColor.RED);
                        unitAtk.Excute();
                        gameplayInput.IsAttacking = false;
                        gameplayInput.BeforeMoveGameUnits[0].restrain = true;
                        gameplayInput.IsMoving = false;
                        unit.disarm = true;
                        gameplayInput.HandleAtkCancel(BattleMap.BattleMap.Instance().GetUnitCoordinate(gameplayInput.BeforeMoveGameUnits[0]));////攻击完工攻击范围隐藏  
                        gameplayInput.BeforeMoveGameUnits.Clear();
                        gameplayInput.TargetList.Clear();
                    }
                    else
                    {
                        //如果攻击指令不符合条件就什么都不做
                    }
                }
                MsgDispatcher.SendMsg((int)MessageType.SAtkToSIdle);
            }
        }
    }
}


