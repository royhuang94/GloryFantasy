using System.Collections;
using System.Collections.Generic;
using BattleMap;
using GameCard;
using GamePlay.Input;
using GameUnit;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GamePlay.FSM
{
    public class InputFSMSummonState : InputFSMState
    {
        private static readonly int RimColor = Shader.PropertyToID("_RimColor");
        private static readonly int MainColor = Shader.PropertyToID("_MainColor");

        public InputFSMSummonState(InputFSM fsm) : base(fsm)
        { }

        public override void OnPointerDownBlock(BattleMapBlock mapBlock, PointerEventData eventData)
        {
            base.OnPointerDownBlock(mapBlock, eventData);
            //把这张手牌从手牌里删掉
            CardManager.Instance().RemoveCardToMapList(FSM.selectedCard.gameObject);
            // 扣除消耗的Ap值
            Player.Instance().ConsumeAp(FSM.selectedCard.GetComponent<BaseCard>().cost);  
            //执行部署指令在对应MapBlock生成单位
            DispositionCommand unitDispose = new DispositionCommand(FSM.selectedCard.id, OwnerEnum.Player, mapBlock);
            unitDispose.Excute();
            //删掉对应手牌槽的引用
            FSM.selectedCard = null;
            //关闭鼠标所在战区的高光显示
            BattleMap.BattleMap.Instance().IsColor = false;
            //状态机压入静止状态
            this.FSM.PushState(new InputFSMIdleState(FSM));
        }

        public override void OnPointerEnter(BattleMapBlock mapBlock, PointerEventData eventData)
        {
            Shader shader = Shader.Find("Sprites/2DOutline");
            Material material = mapBlock.GetComponent<Renderer>().material;
            if(FSM.selectedCard == null)
                return;
            if (shader != null)
            {
//                Debug.Log("ready to high light block");
//                Debug.Log("before: " + material.name + " " + material.color);
//                material.shader = shader;
//                Debug.Log("after: " + material.name + " " + material.color);
//                material.SetColor(RimColor, Color.yellow);
//                material.SetColor(MainColor, Color.red);
//                mapBlock.GetComponent<Renderer>().material.color = Color.red;
            }
            
            else
            {
                Debug.Log("shader is null");
            }
        }
    }
}