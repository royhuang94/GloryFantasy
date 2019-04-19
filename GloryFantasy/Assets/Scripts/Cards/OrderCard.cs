using UnityEngine;

namespace GameUnit
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

        /// <summary>
        /// 点击使用卡牌时调用的函数
        /// </summary>
        /// <returns>若成功使用则返回true，中途取消或其他情况返回false</returns>
        public bool Use()
        {
            UnitManager.Instance.CastCard(GetComponent<Ability.Ability>());

            // 根据情况返回
            //删除这张指令牌
            //CardManager.GetInstance().RemoveCard(this.gameObject);
            return true;
        }
    }
}