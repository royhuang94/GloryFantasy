using LitJson;
using UnityEngine;

namespace GameCard
{
    /// <summary>
    /// 效果卡牌类
    /// </summary>
    public class OrderCard : BaseCard
    {
        /// <summary>
        /// 预制按钮预制件
        /// </summary>
        public GameObject buttonPrefab;

        // 用于存储当前卡牌是否是战技牌
        private bool _isExSkill;
        
        // 用于战技牌存储对应的场上英雄实例ID
        private int _instanceId;
        
        /// <summary>
        /// 确认当前效果牌是否是战技牌
        /// </summary>
        public bool isExSkill { get { return _isExSkill; } }

        public int instanceId { get { return _instanceId; } set { _instanceId = value; } }
        

        public override void Init(string cardId, JsonData cardData)
        {
            base.Init(cardId, cardData);
            foreach (string abilityName in ability_id)
            {
                gameObject.AddComponent(System.Type.GetType("Ability." +abilityName));
            }
            
            // 根据tag中是否有含有战技关键字设置变量
            _isExSkill = tag.Contains("战技");
        }
        
        /// <summary>
        /// 点击使用卡牌时调用的函数
        /// </summary>
        /// <returns>若成功使用则返回true，中途取消或其他情况返回false</returns>
        public override bool Use()
        {
            //进入指令卡选择目标状态
            GamePlay.Gameplay.Instance().gamePlayInput.OnUseOrderCard(this.GetComponent<Ability.Ability>());
            // 根据情况返回
            //删除这张指令牌
            //CardManager.GetInstance().RemoveCard(this.gameObject);
            return true;
        }
    }
}