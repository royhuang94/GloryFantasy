using BattleMap;
using IMessage;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GamePlay.FSM
{
    public class InputFSMPlatState : InputFSMState
    {
        public InputFSMPlatState(InputFSM fsm) : base(fsm)
        { }
        List<BattleMapBlock> pos;

        private int judge()
        {
            List<Vector2> res = new List<Vector2>();
            foreach (GameObject instance in BattleMap.BattleMap.Instance().blocks)
            {
                BattleMapBlock block;
                if (instance.activeSelf == true)
                    block = instance.GetComponent<BattleMapBlock>();
                else
                    continue;
                if (BattleMap.BattleMap.Instance().WarZoneBelong(block.position) == BattleAreaState.Player)
                    res.Add(block.position);
            }
            return res.Count;
            
        }

        public override void OnEnter()
        {
            base.OnEnter();
            
            if (BattleMap.BattleMap.Instance()._quickplat.Count <= 0)
                FSM.PushState(new InputFSMIdleState(FSM));
            else if (judge() <= 0)
            {
                FSM.PushState(new InputFSMIdleState(FSM));
            }
            else { 
                pos = new List<BattleMapBlock>();
            }
            
        }

        public override void OnPointerDownBlock(BattleMapBlock mapBlock, PointerEventData eventData)
        {
            base.OnPointerDownBlock(mapBlock, eventData);

            //如果不是左键直接跳出
            if (eventData.button != PointerEventData.InputButton.Left)
            {
                return;
            }

            //判断是是否符合Ability中的自制对象约束
            if (BattleMap.BattleMap.Instance().WarZoneBelong(mapBlock.position) != BattleAreaState.Player)
                return;

            if (mapBlock.units_on_me.Count <= 0 && !pos.Contains(mapBlock))
                pos.Add(mapBlock);
            // 如果已经选够了目标就发送选择完毕消息
            if (pos.Count == judge() || pos.Count == BattleMap.BattleMap.Instance()._quickplat.Count)
            {
                List<GameUnit.OwnerEnum> owners = new List<GameUnit.OwnerEnum>();
                List<string> units = new List<string>();
                for (int i = 0; i < pos.Count; i++)
                {
                    owners.Add(GameUnit.OwnerEnum.Player);
                    units.Add(BattleMap.BattleMap.Instance()._quickplat[i]);
                }
                Input.DispositionCommandList dispositionCommandList = new Input.DispositionCommandList(units, owners, pos);
                dispositionCommandList.Excute();
                FSM.PushState(new InputFSMIdleState(FSM));
            }
        }
        
    }
}