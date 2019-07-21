using System.Collections.Generic;
using System.Collections.ObjectModel;
using LitJson;

namespace GameCard
{
    public class UnitCard : BaseCard
    {
        #region 变量

        /// <summary>
        /// 对应的UNIT的ID
        /// </summary>
        private string _unitId;
        /// <summary>
        /// 所召唤的单位除了数据库所记述的异能以外额外获得的异能的id
        /// </summary>
        private List<string> _abilitiesOnUnit;

        #endregion

        #region 变量可见性定义
        
        /// <summary>
        /// 对应的UNIT的ID
        /// </summary>
        public string UnitId
        {
            get { return _unitId; }
        }
        
        /// <summary>
        /// 所召唤的单位除了数据库所记述的异能以外额外获得的异能的id
        /// </summary>
        public ReadOnlyCollection<string> AbilitiesOnUnit
        {
            get { return _abilitiesOnUnit.AsReadOnly(); }
        }

        #endregion

        public override void Init(string cardId, JsonData cardData)
        {
            base.Init(cardId, cardData);
            // 因为基类已经做过检查，所以直接使用
            _unitId = cardData["Unit"].ToString();
        }
    }
}