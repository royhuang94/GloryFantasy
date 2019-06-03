﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GamePlay.Event
{
    //     增援：弓手之影
     //   "amount": 1,
     //   "effect": "在来源战区中随机部署X个弓手之影(CR Y)",
     //   "factor": "",
     //   "id": "ReinforceArcher",
     //   "name": "增援：弓手之影",
     //   "source_type": "战区",
     //   "strenth": 1,
     //   "type": "增援",
     //   "weight": 0
    public class ReinforceArcher : Event
    {
        public ReinforceArcher()
        {
            //从数据库读取属性，id名不能错
            EventDataBase.Instance().GetEventProperty("ReinforceArcher", this);
            //实例化该事件的 触发条件 和 效果
            this.Condition = selfCondition;
            this.Action = selfAction;
            //以下两个属性暂时无处读取，设为0   需要从源读取
            this.delta_x_amount = 0;
            this.delta_y_strenth = 0;
        }

        bool selfCondition()
        {
            //do nothing

            return true;
        }

        void selfAction()
        {
            //"在来源战区中随机部署X个弓手之影(CR Y)",

            //来源
            //this.Source as GameUnit.GameUnit
            //来源
            //this.Source as GameUnit.GameUnit
            BattleMap.BattleArea battleArea = this.Source as BattleMap.BattleArea;


            //X次效果 最终值为读取的初始值与delta值的加和
            this.amount += delta_x_amount;
            //Y为效果强度 最终值为读取的初始值与delta值的加和
            this.strenth += delta_y_strenth;
            //根据X和Y的最终值决定召唤结果
            switch (strenth)
            {
                case 1: RandomPosSummonMonster(battleArea._battleArea, this.amount, "WArcher_1"); break;
                case 2: RandomPosSummonMonster(battleArea._battleArea, this.amount, "WArcher_2"); break;
                case 3: RandomPosSummonMonster(battleArea._battleArea, this.amount, "WArcher_3"); break;
                default: break;
            }
        }
        private void RandomPosSummonMonster(List<Vector2> battleMapBlocks, int amount, String Unit_id)//参数意义： 允许生成单位的地图范围、生成单位的数量、生成单位的ID
        {
            List<BattleMap.BattleMapBlock> blocks = new List<BattleMap.BattleMapBlock>();
            foreach (Vector2 pos in battleMapBlocks)    //遍历给出的每一个二维坐标
            {
                //BattleMapBlock 指的是战斗地图中的一个地格
                BattleMap.BattleMapBlock block = BattleMap.BattleMap.Instance().GetSpecificMapBlock(pos);
                if (block.units_on_me.Count != 0)
                    continue;

                blocks.Add(block);
            }

            for (int i = 0; i < amount && blocks.Count > 0;)
            {
                //随机选择一个可行坐标，在此地格上生成单位
                int pos = UnityEngine.Random.Range(0, blocks.Count - 1);//
                BattleMap.BattleMapBlock battleMapBlock = blocks[pos];
                //GameUnit.UnitManager.InstantiationUnit(Unit_id, GameUnit.OwnerEnum.Enemy, battleMapBlock);
                GamePlay.Input.DispositionCommand Command = new Input.DispositionCommand(Unit_id, GameUnit.OwnerEnum.Enemy, battleMapBlock, true);
                //Command.set(Unit_id, GameUnit.OwnerEnum.Enemy, battleMapBlock);
                Command.Excute();//执行
                blocks.RemoveAt(pos);
                i++;
            }

        }
    }
}