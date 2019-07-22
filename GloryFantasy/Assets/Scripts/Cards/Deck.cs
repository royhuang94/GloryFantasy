using System.Collections.Generic;
using GameCard;
using Random = UnityEngine.Random;

namespace Cards
{
    public class HeroData
    {
        public string id { get; set; }
        public List<string> additionalAbility;
        public List<string> arks;
        public HeroData(string id, List<string> arks, List<string> additionalAbility = null)
        {
            this.id = id;
            this.arks = arks;
            this.additionalAbility = additionalAbility;
            if (this.additionalAbility == null)
                this.additionalAbility = new List<string>();
        }
    }
    public class Deck
    {
        public Dictionary<string, HeroData> _heroes;
        /// <summary>
        /// 卡牌对象引用
        /// </summary>
        public List<BaseCard> _deck;
        
        /// <summary>
        /// 记录卡牌数量，维护展示列表的list
        /// </summary>
        private Dictionary<string, int> _deckRecorder;
        
        public Deck(List<HeroData> heroes)
        {
            _heroes = new Dictionary<string, HeroData>();
            _deckRecorder = new Dictionary<string, int>();
            _deck = new List<BaseCard>();
            foreach(HeroData heroData in heroes)
            {
                _heroes.Add(heroData.id, heroData);
            }
        }

        /// <summary>
        /// 洗牌
        /// </summary>
        public void Shuffle()
        {
            int size = _deck.Count;

            for (int i = 0; i < size; i++)
            {
                int pos = Random.Range(i, size);
                BaseCard temp = _deck[i];
                _deck[i] = _deck[pos];
                _deck[pos] = temp;
            }
        }
    }
}
