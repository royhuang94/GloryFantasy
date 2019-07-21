using LitJson;

namespace GameCard
{
    public class _NewOrderCard : _NewBaseCard
    {
        #region 变量
        /// <summary>
        /// 携带的咒语id
        /// </summary>
        private string _spell;

        #endregion

        #region 变量可见性

        public string Spell
        {
            get { return _spell; }
        }

        #endregion


        public override void Init(string cardId, JsonData cardData)
        {
            base.Init(cardId, cardData);
            // 因为基类已经做过检查，所以直接使用
            _spell = cardData["Spell"].ToString();
        }
    }
}