using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GamePlay.Event
{
    //     增援：黑影
    //   "amount": 1,
    //   "effect": "在来源战区中随机部署X个黑影(CR Y)",
    //   "factor": "",
    //   "id": "ReinforceArcher",
    //   "name": "增援：黑影",
    //   "source_type": "战区",
    //   "strenth": 1,
    //   "type": "增援",
    //   "weight": 0
    public class ReinforceWarrior : Event
    {
        public ReinforceWarrior()
        {
            //从数据库读取属性，id名不能错
            EventDataBase.Instance().GetEventProperty("ReinforceWarrior", this);
            //初始化条件函数和行动函数
            this.Condition = selfCondition ;
            this.Action = selfAction;
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
            BattleMap.BattleArea battleArea = this.Source as BattleMap.BattleArea;

            RandomPosSummonMonster(battleArea._battleArea, this.amount);
            //X个
            //this.amount

            //Y
            //this.strenth
        }

        private void RandomPosSummonMonster(List<Vector2> battleMapBlocks, int amount)
        {
            List<BattleMap.BattleMapBlock> blocks = new List<BattleMap.BattleMapBlock>();
            foreach (Vector2 pos in battleMapBlocks)
            {
                BattleMap.BattleMapBlock block = BattleMap.BattleMap.Instance().GetSpecificMapBlock(pos);
                if (block.units_on_me.Count != 0)
                    continue;

                blocks.Add(block);
            }

            for (int i = 0; i < amount && blocks.Count > 0;)
            {
                int pos = UnityEngine.Random.Range(0, blocks.Count - 1);
                BattleMap.BattleMapBlock battleMapBlock = blocks[pos];
                GameUnit.UnitManager.InstantiationUnit("Shadow_1", GameUnit.OwnerEnum.Enemy, battleMapBlock);
                blocks.RemoveAt(pos);
                i++;
            }

        }
    }
}