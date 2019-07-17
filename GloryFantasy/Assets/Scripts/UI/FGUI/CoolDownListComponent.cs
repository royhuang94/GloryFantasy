using System.Collections.Generic;
using System.Linq;
using FairyGUI;
using GameCard;
using GameUnit;
using IMessage;
using LitJson;
using UnityEngine;

namespace UI.FGUI
{
    public class CoolDownListComponent : IComponent, MsgReceiver
    {
        
        #region 变量
        ///// <summary>
        ///// 存储上一次点击的冷却卡牌
        ///// </summary>
        //private cdObject _lastClickedCoolDownCard;
        /// <summary>
        /// 冷却池卡牌列表
        /// </summary>
        private GList _cooldownList;

        /// <summary>
        /// 英雄管理器内冷却list的引用
        /// </summary>
        private List<GameUnit.HeroCD> __cooldownList;
        
        /// <summary>
        /// 冷却池用的卡牌素材资源定义
        /// </summary>
        private const string _cooldownCardAssets = "card628";

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
            GameUnit.UnitHero CDHero = __cooldownList[index].hero;

            GamePlay.Gameplay.Instance().gamePlayInput.InputFSM.OnPointerDownCDObject(CDHero, context);
        }

        /// <summary>
        /// 更新冷却区卡牌
        /// </summary>
        public void UpdateCooldownList()
        {
            _cooldownList.RemoveChildren(0, -1, true);
            foreach (HeroCD cooldownUnit in __cooldownList)
            {
                GObject item = UIPackage.CreateObject(_pkgName, "cooldownItem");
                item.icon = UIPackage.GetItemURL(_numsPkg, "cdNum" + cooldownUnit.leftCD());
                item.asCom.GetChild("n2").asLoader.url = UIPackage.GetItemURL(_cooldownCardAssets,
                    cooldownUnit.hero.id.Split('_').First());
                _cooldownList.AddChild(item);
            }
        }

        public CoolDownListComponent(GList cooldownList, string pkgName)
        {
            _cooldownList = cooldownList;
            _pkgName = pkgName;
            
            //_lastClickedCoolDownCard = null;

            // 获取引用
            __cooldownList = GamePlay.Gameplay.Instance().heroManager.CDHeros;
            // 添加点击事件
            _cooldownList.onClickItem.Add(OnClickCoolDownCard);
            //_cooldownList.onRightClickItem.Add(OnClickCoolDownCard);
            
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