using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using GameUnit;
using IMessage;
using LitJson;
using UnityEngine;

namespace GameCard
{
    public class BaseCard : MonoBehaviour, MsgReceiver
    {
        #region 变量

        /// <summary>
        /// 卡牌的ID
        /// </summary>
        private string _id;
        /// <summary>
        /// 卡牌名称
        /// </summary>
        private string _name;
        /// <summary>
        /// 卡牌携带的颜色列表，无颜色则count为0
        /// </summary>
        private List<string> _color;
        /// <summary>
        /// 卡牌携带的标签列表，无标签则count为0
        /// </summary>
        private List<string> _tag;
        /// <summary>
        /// 卡牌所记述的规则文字
        /// </summary>
        private string _ruleText;
        /// <summary>
        /// 卡牌所记述的风味文字
        /// </summary>
        private string _flavorText;
        /// <summary>
        /// 关于卡牌是否在生效后被销毁
        /// </summary>
        private bool _willDestroy;
        /// <summary>
        /// 卡牌记录的使用者，实用性存疑
        /// </summary>
        private UnitHero _carrier;
        /// <summary>
        /// 卡牌的类型
        /// </summary>
        private string _type;
        /// <summary>
        /// 使用卡牌需要消耗的AP值
        /// </summary>
        private int _cost;

        #endregion

        #region 变量可见性定义
        /// <summary>
        /// 卡牌的ID
        /// </summary>
        public string Id
        {
            get { return _id; }
        }
        /// <summary>
        /// 卡牌名称
        /// </summary>
        public string Name
        {
            get { return _name; }
        }
        /// <summary>
        /// 卡牌携带的颜色列表，无颜色则count为0
        /// </summary>
        public ReadOnlyCollection<string> Color
        {
            get { return _color.AsReadOnly(); }
        }
        /// <summary>
        /// 卡牌携带的标签列表，无标签则count为0
        /// </summary>
        public ReadOnlyCollection<string> Tag
        {
            get { return _tag.AsReadOnly(); }
        }
        /// <summary>
        /// 卡牌所记述的规则文字
        /// </summary>
        public string RuleText
        {
            get { return _ruleText; }
        }
        /// <summary>
        /// 卡牌所记述的风味文字
        /// </summary>
        public string FlavorText
        {
            get { return _flavorText; }
        }
        /// <summary>
        /// 关于卡牌是否在生效后被销毁
        /// </summary>
        public bool WillDestroy
        {
            get { return _willDestroy; }
        }
        /// <summary>
        /// 卡牌记录的使用者，实用性存疑
        /// </summary>
        public UnitHero Carrier
        {
            get { return _carrier; }
        }
        /// <summary>
        /// 卡牌类型
        /// </summary>
        public string Type
        {
            get { return _type; }
        }
        /// <summary>
        /// 使用卡牌要消耗的AP值
        /// </summary>
        public int Cost
        {
            get { return _cost; }
        }

        #endregion
        
        /// <summary>
        /// 依据给定的数据进行初始化，会对提供的ID和json数据进行检查
        /// </summary>
        /// <param name="cardId">卡牌ID</param>
        /// <param name="cardData">卡牌的Json数据</param>
        /// <exception cref="NotImplementedException">若提供数据异常则抛出此错误</exception>
        public virtual void Init(string cardId, JsonData cardData)
        {
            // 检查提供的数据
            if (string.IsNullOrEmpty(cardId) || cardData == null)
            {
                throw new NotImplementedException();
            }
            
            // 初始化基础属性
            _id = cardId;
            _name = cardData["Name"].ToString();
            _ruleText = cardData["Text"].ToString();
            _flavorText = cardData["Flavor"].ToString();
            _type = cardData["Type"].ToString();
            _cost = int.Parse(cardData["Cost"].ToString());

            
            // 读入所有color及tag标签
            _color = new List<string>();
            _tag = new List<string>();
            JsonData colorArray = cardData["Color"];
            JsonData tagArray = cardData["Tag"];
            
            for (int i = 0; i < colorArray.Count; i++)
            {    
                // 如果color标签长度不为0， 则为合法color，复制后放入list
                if(colorArray[i].ToString().Length > 0)
                    _color.Add(colorArray[i].ToString());
            }

            for (int i = 0; i < tagArray.Count; i++)
            {
                // 如果tag标签长度不为0， 则为合法color，复制后放入list
                if(tagArray[i].ToString().Length > 0)
                    _tag.Add(tagArray[i].ToString());
            }
        }

        T IMessage.MsgReceiver.GetUnit<T>()
        {
            return this as T;
        }
    }
}