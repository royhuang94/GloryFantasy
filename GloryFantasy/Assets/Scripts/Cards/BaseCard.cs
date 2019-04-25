using System;
using System.Collections.Generic;
using LitJson;
using UnityEngine;

namespace GameCard
{
    /*
     * 暂时仅用单位牌，且使用超级生物的信息进行默认初始化
     */
    public class BaseCard : MonoBehaviour, IMessage.MsgReceiver
    {

        // 暂时没想到好方法限制他的写操作，绝对不要修改
        public string id;
        
        #region 变量定义
        
        private List<string> _abilityId;
        private List<string> _tag;
        
        private int _cd;
        private int _cost;
        
        private string _slot;
        private string _color;
        private string _effect;
        private string _flavourText;
        private string _name;
        private string _relatedToken;
        private string _tokenId;
        private string _type;

        #endregion
        
        #region 所有变量可访问性定义
        /// <summary>
        /// 若没有id，则得到list为空，而不是Null！
        /// </summary>
        public List<string> ability_id
        {
            get { return _abilityId; }
        }

        public List<string> tag
        {
            get { return _tag; }
        }

        public int cd
        {
            get { return _cd; }
        }

        public string slot
        {
            get { return _slot; }
        }

        public int cost
        {
            get { return _cost; }
        }

        public string color
        {
            get { return _color; }
        }

        public string effect
        {
            get { return _effect; }
        }

        public string flavor_text
        {
            get { return _flavourText; }
        }

        public string name
        {
            get { return _name; }
        }

        public string related_token
        {
            get { return _relatedToken; }
        }

        public string tokenID
        {
            get { return _tokenId; }
        }

        public string type
        {
            get { return _type; }
        }
        
        #endregion
        
        T IMessage.MsgReceiver.GetUnit<T>()
        {
            return this as T;
        }

        /// <summary>
        /// 启动时从卡牌数据库请求数据并进行初始化
        /// </summary>
        /// <exception cref="NotImplementedException">无法正常初始化</exception>
        private void Start()
        {
            // 若id为空，则抛出异常，一般在预制件没有做好，或者程序内某个地方挂上了id为空的BaseCard脚本就会抛出错误
            if (id.Length == 0 )
            {
                throw new System.NotImplementedException();
            }
            
            // 调用接口从卡牌数据库获取对应的JsonData
            JsonData cardData = CardManager.GetInstance().GetCardJsonData(id);
            
            // 如果不存在此ID，则抛出异常
            if(cardData == null)
                throw new System.NotImplementedException();

            _cd = int.Parse(cardData["cd"].ToString());
            _cost = int.Parse(cardData["cost"].ToString());
            _slot = cardData["slot"].ToString();
            _color = cardData["color"].ToString();
            _effect = cardData["effect"].ToString();
            _flavourText = cardData["flavor_text"].ToString();
            _name = cardData["name"].ToString();
            _relatedToken = cardData["related_token"].ToString();
            _tokenId = cardData["tokenID"].ToString();
            _type = cardData["type"].ToString();

            _abilityId = new List<string>();
            _tag = new List<string>();

            for (int i = 0; i < cardData["ability_id"].Count; i++)
            {
                _abilityId.Add(string.Copy(cardData["ability_id"][i].ToString()));
            }

            for (int i = 0; i < cardData["tag"].Count; i++)
            {
                _tag.Add((string.Copy(cardData["tag"][i].ToString())));
            }

            foreach (string abilityName in _abilityId)
            {
                gameObject.AddComponent(System.Type.GetType("Ability." +abilityName));
            }
            
        }


        // 记录卡牌冷却回合数
        public int cooldownRounds { get; set; }

    }
    
}