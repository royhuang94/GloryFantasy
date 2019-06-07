using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GamePlay.Event
{
    public class Event : GameplayTool
    {
        /// <summary>
        /// 事件源
        /// </summary>
        public object Source;           
        /// <summary>
        /// 事件ID
        /// </summary>
        public string id;           
        /// <summary>
        /// 事件所包含的子效果生效的次数
        /// </summary>
        public int amount;
        /// <summary>
        /// 事件所包含的子效果的强度
        /// </summary>
        public int strenth;
        /// <summary>
        /// 参数x——事件源带来的 事件具体效果实施的次数 强化/弱化
        /// </summary>
        public int delta_x_amount;
        /// <summary>
        /// 参数y——事件源带来的 事件具体效果的强度 强化/弱化
        /// </summary>
        public int delta_y_strenth;

        /*      ---->这里由于事件暂只有 源 而没有单独的 目标 属性，所以以下此两项属性暂搁置
        /// <summary>
        /// 战区信息（如事件作用目标是战区）
        /// </summary>
        public BattleMap.BattleArea _target_BattleArea;
        /// <summary>
        /// 单位信息（如事件作用目标是单位）
        /// </summary>
        public GameUnit.GameUnit _target_Unit;
        */

        /// <summary>
        /// 事件权重
        /// </summary>
        public int weight;          
        /// <summary>
        /// 事件效果说明——文字
        /// </summary>
        public string effect;           
        public string factor;               
        /// <summary>
        /// 事件的中文名
        /// </summary>  
        public string name;
        /// <summary>
        /// 事件源名称
        /// </summary>         
        public string source_type;      //事件源名称
        /// <summary>
        /// ？？？
        /// </summary>  
        public List<string> type;
        /// <summary>
        /// 事件的条件函数
        /// </summary>  
        protected Func<bool> Condition;
        /// <summary>
        /// 事件的执行函数
        /// </summary>  
        protected Action Action;
        /// <summary>
        /// 执行事件
        /// </summary>
        /// <returns></returns>
        public bool Execute()
        {
            if (Condition())
            {
                Action();
                return true;
                ;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 获取事件信息，简单应用，后期可更改
        /// </summary>
        /// <returns></returns>
        public string getEventMessage()
        {
            return name + "  " + effect;
        }
        
        /// <summary>
        /// 从事件源获取其 X 与 Y 的值
        /// </summary>  
        public int Get_Source_Message()            //获取 x 和 y 的基类方法，省的每个事件再去重载了
        {
            //获取和事件源有关的信息
            System.Type tempType = Source.GetType();
            if (tempType.ToString() == "GameUnit.GameUnit")     //若此 源 是一个单位
            {
                GameUnit.GameUnit Unit = this.Source as GameUnit.GameUnit;
                this.delta_x_amount = Unit.delta_x_amount;
                this.delta_x_amount = Unit.delta_y_strenth;
                return 1;
            }
            if (tempType.ToString() == "BattleMap.BattleArea")   //若此 源 是一个战区
            {
                BattleMap.BattleArea battleArea = this.Source as BattleMap.BattleArea;
                this.delta_x_amount = battleArea.delta_x_amount;
                this.delta_x_amount = battleArea.delta_y_strenth;
                return 2;
            }
            else return 0;
        }

        /// <summary>
        /// 在指定战区召唤指定编号的单位(敌方单位)
        /// </summary>  
        /// <param name="target_area">一个实质是 战区 的抽象object</param>
        /// <param name="amount">召唤数量</param>
        /// <param name="Unit_id">希望召唤的单位的ID</param>
        public void SummonMonster_in_Area(object target_area, int amount, String Unit_id)//参数意义： 允许生成单位的地图范围(现在只允许以一个战区为自然范围)、生成单位的数量、生成单位的ID
        {
            BattleMap.BattleArea battleArea = target_area as BattleMap.BattleArea;
            List<Vector2> battleMapBlocks = battleArea._battleArea;
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
                //GameUnit.UnitManager.InstantiationUnit(Unit_id, GameUnit.OwnerEnum.Enemy, battleMapBlock);    原来的召唤方法
                GamePlay.Input.DispositionCommand Command = new Input.DispositionCommand(Unit_id, GameUnit.OwnerEnum.Enemy, battleMapBlock, true);
                //Command.set(Unit_id, GameUnit.OwnerEnum.Enemy, battleMapBlock);
                Command.Excute();//执行

                blocks.RemoveAt(pos);
                i++;
            }

        }

        /// <summary>
        /// 在指定单位周围（3爆发范围）召唤指定编号的单位：（我方或敌方单位）
        /// </summary>  
        /// <param name="target_unit">一个实质是 单位 的抽象object</param>
        /// <param name="amount">召唤数量</param>
        /// <param name="Unit_id">希望召唤的单位的ID</param>
        public void SummonMonster_in_Unit_Around(object target_unit,int amount, String Unit_id)
        {
            GameUnit.GameUnit Unit = target_unit as GameUnit.GameUnit;
            Vector2 Unit_Pos = Unit.CurPos;
            List<BattleMap.BattleMapBlock> AroundBlocks;
            AroundBlocks = GameplayToolExtend.getBlocksByBound(Unit_Pos,GameplayToolExtend.Area[2]);
            List<BattleMap.BattleMapBlock> UsefulBlocks = new List<BattleMap.BattleMapBlock>();
            foreach (BattleMap.BattleMapBlock block in AroundBlocks)
            {
                if (block.units_on_me.Count == 0)   //没有单位

                  UsefulBlocks.Add(block);
            }

            for (int i = 0; i < amount && UsefulBlocks.Count > 0;)
            {
                //随机选择一个可行坐标，在此地格上生成单位
                int pos = UnityEngine.Random.Range(0, UsefulBlocks.Count - 1);//
                BattleMap.BattleMapBlock battleMapBlock = UsefulBlocks[pos];
                //召唤单位的所属为 源的所属
                if (Unit.owner == GameUnit.OwnerEnum.Enemy)     
                {
                    GamePlay.Input.DispositionCommand Command = new Input.DispositionCommand(Unit_id, GameUnit.OwnerEnum.Enemy, battleMapBlock, true);
                    Command.Excute();//执行
                }
                if (Unit.owner == GameUnit.OwnerEnum.Player)
                {
                    GamePlay.Input.DispositionCommand Command = new Input.DispositionCommand(Unit_id, GameUnit.OwnerEnum.Player, battleMapBlock, true);
                    Command.Excute();//执行
                }


                AroundBlocks.RemoveAt(pos);
                i++;
            }
        }
        /// <summary>
        /// 升级指定的单位
        /// </summary>  
        /// <param name="Unit">希望升级的 单位实体 </param>
        /// <param name="Unit">若希望事件源转换为 升级后新单位 则置1 </param>
        public void Unit_Upgrade(GameUnit.GameUnit Unit,int flag)     
        {
            string _CR = Unit.id.Substring(Unit.id.Length - 1, 1);
            string Unit_Type = Unit.id.Substring(0, Unit.id.Length - 2);
            if (Convert.ToInt32(_CR) <= 2)
            {
                _CR = Convert.ToString(Convert.ToInt32(_CR) + 1);   //_CR的值+1
                GameUnit.UnitManager.Kill(null, this.Source as GameUnit.GameUnit);
                GameUnit.GameUnit newUnit = this.Regenerate("SandwormHead_" + _CR, Unit.mapBlockBelow,Unit.owner);
                if (flag == 1)
                {
                    this.Source = newUnit;
                }
            }
            else
            {
                //等级已经到头了，啥都不发生
            }

        }
    }
}
