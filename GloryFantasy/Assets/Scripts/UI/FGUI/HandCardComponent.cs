using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GamePlay;
using FairyGUI;
using GameCard;
using IMessage;
using LitJson;
using UnityEngine;

namespace UI.FGUI
{
    public class HandCardComponent : MonoBehaviour, IComponent, MsgReceiver
    {
        #region 变量

        /// <summary>
        /// 组件中卡牌list的引用
        /// </summary>
        private GList _handCardList;

        /// <summary>
        /// 卡牌管理器内手牌列表的引用，以后会进行修改
        /// </summary>
        private List<string> __handCardList;

        /// <summary>
        /// 记录上一次点击的item
        /// </summary>
        private GObject _lastClicked;
        
        /// <summary>
        /// 手牌资源包定义
        /// </summary>
        private const string _handCardAssets = "fakeHandcard";

        /// <summary>
        /// 资源包定义
        /// </summary>
        private string _pkgName;

        /// <summary>
        /// 箭头的起始坐标
        /// </summary>
        private Vector3 _startPos;
        #endregion
        
        /// <summary>
        /// 初始化手牌组件
        /// </summary>
        /// <param name="handCardList">提供场景中存在的手牌list的引用</param>
        public HandCardComponent(GList handCardList, string pkgName)
        {
            Init(handCardList, pkgName);
        }

        public void Init(GList handCardList, string pkgName)
        {
	        _handCardList = handCardList;
	        _pkgName = pkgName;
	        _lastClicked = null;
	        
	        __handCardList = CardManager.Instance().cardsInHand;

	        // 设置卡牌渲染模式， 此模式保证选择的卡牌在最上
	        _handCardList.childrenRenderOrder = ChildrenRenderOrder.Arch;
            
	        // 添加手牌点击事件响应
	        _handCardList.onClickItem.Add(OnClickHandCard);
            
	        MsgDispatcher.RegisterMsg(
		        this.GetMsgReceiver(),
		        (int)MessageType.HandcardChange,
		        () => { return true;},
		        UpdateHandCard,
		        "Hand cards observer"
	        );
            
	        UpdateHandCard();
        }
        
        /// <summary>
        /// 更新手牌，与卡牌管理器内同步
        /// </summary>
        public void UpdateHandCard()
        {
	        _handCardList.RemoveChildren(0, -1, true);
	        foreach (string cardId in __handCardList)
	        {
		        GObject item = UIPackage.CreateObject(_pkgName, "handcardItem2");
		        String nid;
		        if (cardId.Contains("#"))
		        {
			        nid = cardId.Substring(0, cardId.IndexOf('#'));
		        }
		        else
		        {
			        nid = cardId;
		        }

		        item.icon = UIPackage.GetItemURL(_handCardAssets, nid.Split('_').First());
		        item.SetPivot(0.5f, 1f);
		        _handCardList.AddChild(item);
		        string id = string.Copy(nid);
		        item.onRollOver.Add(()=>
		        {
			        _handCardList.apexIndex = __handCardList.IndexOf(cardId);
			        JsonData data = CardManager.Instance().GetCardJsonData(id);
			        FGUIInterfaces.Instance().title.text = data["name"].ToString();
			        FGUIInterfaces.Instance().effect.text = data["effect"].ToString();
			        FGUIInterfaces.Instance().value.text = "冷却：" + data["cd"] + "    " + "专注值：" + data["cost"] + "\n" + data["type"];
			        int costAp = int.Parse("" + data["cost"]);
			        if (Player.Instance().CanConsumeAp(costAp)) // 可使用单位才放大
			        {
				        StartCoroutine(FancyHandCardEffect(item, 1.3f));
			        }

			        FGUIInterfaces.Instance().cardDescribeWindow.Show();
		        });

		        item.onRollOut.Add(() =>
		        {
			        StartCoroutine(FancyHandCardEffect(item, 1.0f));
			        FGUIInterfaces.Instance().cardDescribeWindow.Hide();
		        });
	        }

        }

        /// <summary>
        /// 响应手牌点击事件的函数
        /// </summary>
        public void OnClickHandCard(EventContext context)
        {
	        // 如果不是玩家回合，则无法使用卡牌
	        if (!Gameplay.Instance().roundProcessController.IsPlayerRound())
		        return;

	        GObject item = context.data as GObject;
		
	        _startPos = Input.mousePosition;
	        // TODO: 直接释放技能不显示箭头
	        if(CardManager.Instance().handcardsInstance[_handCardList.GetChildIndex(item)].GetComponent<BaseCard>() is UnitCard)
				ArrowManager.Instance().DrawArrow(_startPos, Input.mousePosition);		// 箭头二
	        // 确认当前点击的卡牌和上次点击的不同，此时表明用户想使用这张卡牌
	        if (item != _lastClicked)
	        {
		        // 改变记录
		        _lastClicked = item;
		        // 动效
		        //DoSpecialEffect(item);
		        // 设置当前选中的卡牌
		        CardManager.Instance().SetSelectingCard(_handCardList.GetChildIndex(item));
	        }
	        else // 此时用户点击的牌和上次相同，表示用户想取消使用
	        {
		        ArrowManager.Instance().HideArrow();
		        // 恢复原大小
		        foreach (GObject litem in _handCardList.GetChildren())
		        {
			        StartCoroutine(FancyHandCardEffect(litem, 1.0f));
		        }
			
		        // 重置上次选择项
		        _lastClicked = null;
			
		        // 调用取消使用方法
		        CardManager.Instance().CancleUseCurrentCard();
			
		        // 结束函数执行，因为用户取消使用
		        return;
	        }
		
	        CardManager.Instance().OnUseCurrentCard();
		
        }
        
        private IEnumerator FancyHandCardEffect(GObject item, float finalScale)
        {
	        int frameCount = 15;
		
	        float range = item.scaleX;

	        float step = (range - finalScale) / frameCount;

			//float judge = Mathf.Abs(step / 2 + step);
	        const float judge = 0.0001f;        // 用于判断是否满足规定大小，不能用MinValue, 会越来越大或者小到不见了，谁用谁知道
	        while (Mathf.Abs(range - finalScale) > judge)
	        {
		        range -= step;
		        item.SetScale(range, range);
		        yield return null;
	        }
        }
        
        /// <summary>
        /// 手牌组件的Operation
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public void Operation()
        {
            throw new System.NotImplementedException();
        }

        public override string ToString()
        {
            return "HandCardList";
        }
        
        /// <summary>
        /// 仿照主程写的写的接口
        /// </summary>
        T MsgReceiver.GetUnit<T>()
        {
	        return this as T;
        }

        #region 手牌叶子节点不需要的部分
        /// <summary>
        /// 组件作为叶子节点，不包含此方法
        /// </summary>
        /// <param name="component"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void Add(IComponent component)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// 组件作为叶子节点，不包含此方法
        /// </summary>
        /// <param name="comId"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public IComponent GetChild(string comId)
        {
            throw new System.NotImplementedException();
        }
        #endregion
    }
}