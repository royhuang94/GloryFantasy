using System.Collections;
using System.Collections.Generic;
using Ability;
using BattleMap;
using GameCard;
using GamePlay.Input;
using GameUnit;
using UI.FGUI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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
            //如果是地方战区则不能召唤
            int reginID = BattleMap.BattleMap.Instance().battleAreaData.GetReginIDByPos(new Vector2(mapBlock.x, mapBlock.y));
            if (BattleMap.BattleMap.Instance().battleAreaData.battleAreas[reginID]._battleAreaSate != BattleAreaState.Player)
            {
                Debug.Log("你只能部属单位在你所拥有的战区");
                return;
            }
            if (mapBlock.units_on_me.Count > 0)
                return;
            
            //关闭鼠标所在战区的高光显示
            BattleMap.BattleMap.Instance().IsColor = false;
            ArrowManager.Instance().HideArrow();        // 关闭箭头显示
            mapBlock.GetComponent<SpriteRenderer>().color = Color.white;
            // 锁住堆叠直到全部信息发送完毕
            EffectStack.setLocker(false);
            //状态机压入静止状态
            this.FSM.PushState(new InputFSMIdleState(FSM));
            BaseCard card = FSM.selectedCard;
            HandCardManager.Instance().OperateCard(card, CardArea.Hand, false);
            // 扣除消耗的Ap值
            Player.Instance().ConsumeAp(card.Cost);
            //执行部署指令在对应MapBlock生成单位 TODO: 原来的TokenID
            DispositionCommand unitDispose = new DispositionCommand((card as UnitCard).UnitId, OwnerEnum.Player, mapBlock);
            unitDispose.Excute();

            //删掉对应手牌槽的引用
            FSM.selectedCard = null;
            EffectStack.setLocker(true);
        }

        public override void OnPointerEnter(BattleMapBlock mapBlock, PointerEventData eventData)
        {
            Shader shader = Shader.Find("Sprites/2DOutline");
//            Material material = mapBlock.GetComponent<Renderer>().material;
            // 没有选择卡牌或者或者不是友方战区或者友方战区但是该地格已有单位，则不高亮
            int reginID = BattleMap.BattleMap.Instance().battleAreaData.GetReginIDByPos(new Vector2(mapBlock.x, mapBlock.y));
            if(FSM.selectedCard == null || 
               BattleMap.BattleMap.Instance().battleAreaData.battleAreas[reginID]._battleAreaSate != BattleAreaState.Player || 
               BattleMap.BattleMap.Instance().CheckIfHasUnits(mapBlock.position))
                return;
            mapBlock.GetComponent<SpriteRenderer>().color = new Color(0f, 1f, 0f, 0.3f);
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


        public override void OnPointerExit(BattleMapBlock mapBlock, PointerEventData eventData)
        {
//            Material material = mapBlock.GetComponent<Renderer>().material;
            if(FSM.selectedCard == null)
                return;
            mapBlock.GetComponent<SpriteRenderer>().color = Color.white;
        }
    }
}