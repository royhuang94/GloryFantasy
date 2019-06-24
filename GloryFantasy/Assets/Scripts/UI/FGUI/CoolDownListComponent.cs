using System.Collections.Generic;
using System.Linq;
using FairyGUI;
using GameCard;
using IMessage;
using LitJson;
using UnityEngine;

namespace UI.FGUI
{
    public class CoolDownListComponent : IComponent, MsgReceiver
    {
        
        #region 变量
        /// <summary>
        /// 存储上一次点击的冷却卡牌
        /// </summary>
        private cdObject _lastClickedCoolDownCard;
        /// <summary>
        /// 冷却池卡牌列表
        /// </summary>
        private GList _cooldownList;

        /// <summary>
        /// 卡牌管理器内冷却卡牌list的引用
        /// </summary>
        private List<cdObject> __cooldownList;
        
        /// <summary>
        /// 冷却池用的卡牌素材资源定义
        /// </summary>
        private const string _cooldownCardAssets = "fakeHandcard";

        /// <summary>
        /// 冷却池用数字遮罩资源包定义
        /// </summary>
        private const string _numsPkg = "newCdNums";

        /// <summary>
        /// 冷却池使用的资源包定义
        /// </summary>
        private string _pkgName;
        #endregion

        public void OnClickCoolDownCard(EventContext context)
        {
            // 获取冷却列表点击下标
            int index = _cooldownList.GetChildIndex(context.data as GObject);

            // 根据点击下标获取对应冷却牌
            cdObject cooldownCard = __cooldownList[index];
			
            // 冷却牌ID
            string cardID = cooldownCard.objectId;

            if (cardID.Contains('#'))
                cardID = cardID.Split('#').First();
            

            // 获取展示数据
            JsonData data = CardManager.Instance().GetCardJsonData(cardID);
		
            // 现在用的展示界面和手牌及已部署单位是同一个界面。
            // 不等于上一个点击的冷却牌，则窗口显示其他冷却牌信息
            if(cooldownCard != _lastClickedCoolDownCard)
            {
                FGUIInterfaces.Instance().title.text = data["name"].ToString();
                FGUIInterfaces.Instance().effect.text = data["effect"].ToString();
                FGUIInterfaces.Instance().value.text = "总冷却：" + data["cd"] + "     剩余冷却：" + cooldownCard.leftCd;

                _lastClickedCoolDownCard = cooldownCard; 		// 重置上一个点击的为当前点击冷却牌
		
                FGUIInterfaces.Instance().cardDescribeWindow.Show();
            }
            else
            {
                // 相等则关闭显示框
                _lastClickedCoolDownCard = null;
                FGUIInterfaces.Instance().cardDescribeWindow.Hide();
            }
        }

        /// <summary>
        /// 更新冷却区卡牌
        /// </summary>
        public void UpdateCooldownList()
        {
            _cooldownList.RemoveChildren(0, -1, true);
            foreach (cdObject cooldownCard in __cooldownList)
            {
                GObject item = UIPackage.CreateObject(_pkgName, "cooldownItem");
                item.icon = UIPackage.GetItemURL(_numsPkg, "cdNum" + cooldownCard.leftCd);
                item.asCom.GetChild("n2").asLoader.url = UIPackage.GetItemURL(_cooldownCardAssets,
                    cooldownCard.objectId.Split('_').First());
                _cooldownList.AddChild(item);
            }
        }

        public CoolDownListComponent(GList cooldownList, string pkgName)
        {
            _cooldownList = cooldownList;
            _pkgName = pkgName;
            
            _lastClickedCoolDownCard = null;
            
            // 获取引用
            __cooldownList = CardManager.Instance().cooldownCards;
            // 添加右键点击事件
            //_cooldownList.onClickItem.Add(OnClickCoolDownCard);
            _cooldownList.onRightClickItem.Add(OnClickCoolDownCard);
            
            MsgDispatcher.RegisterMsg(
                this.GetMsgReceiver(),
                (int)MessageType.CooldownlistChange,
                () => { return true;},
                UpdateCooldownList,
                "Cooldown list observer"
            );
            
            UpdateCooldownList();
        }

        public override string ToString()
        {
            return "CoolDownList";
        }

        /// <summary>
        /// 仿照主程写的写的接口
        /// </summary>
        T MsgReceiver.GetUnit<T>()
        {
            return this as T;
        }
    
        #region 不使用的接口
        
        public void Operation()
        {
            throw new System.NotImplementedException();
        }

        public void Add(IComponent component)
        {
            throw new System.NotImplementedException();
        }

        public IComponent GetChild(string comId)
        {
            throw new System.NotImplementedException();
        }

        public T GetUnit<T>() where T : MonoBehaviour
        {
            throw new System.NotImplementedException();
        }
        
        #endregion
    }
}