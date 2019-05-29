using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GamePlay.Event
{
    //     增援：狼影
    //   "amount": 1,
    //   "effect": "在来源战区中随机部署X个狼影(CR Y)",
    //   "factor": "",
    //   "id": "ReinforceWolf",
    //   "name": "增援：狼影",
    //   "source_type": "战区",
    //   "strenth": 1,
    //   "type": "增援",
    //   "weight": 0
    public class ReinforceWolf : Event
    {
        public ReinforceWolf()
        {
            //从数据库读取属性，id名不能错
            EventDataBase.Instance().GetEventProperty("ReinforceWolf", this);
            //初始化条件函数和行动函数
            this.Condition = selfCondition;
            this.Action = selfAction;
        }

        bool selfCondition()
        {
            //do nothing

            return true;
        }

        void selfAction()
        {
            //"在来源战区中随机部署X个狼影(CR Y)",

            //来源
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
                GameUnit.UnitManager.InstantiationUnit("ShadowWolf_1", GameUnit.OwnerEnum.Enemy, battleMapBlock);
                blocks.RemoveAt(pos);
                i++;
            }

        }
    }
}