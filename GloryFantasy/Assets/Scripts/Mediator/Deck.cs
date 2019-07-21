﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameCard;
using Random = UnityEngine.Random;

namespace Mediator
{
    public class HeroData
    {
        public string id { get; set; }
        public List<string> additionalBuff;
        public List<string> arks;
        HeroData(string id, List<string> arks, List<string> additionalBuff = null)
        {
            this.id = id;
            this.arks = arks;
            this.additionalBuff = additionalBuff;
            if (this.additionalBuff == null)
                this.additionalBuff = new List<string>();
        }
    }
    public class Deck
    {
        public Dictionary<string, HeroData> _heroes;
        /// <summary>
        /// 卡牌对象引用
        /// </summary>
        public List<_NewBaseCard> _deck;
        Deck(List<HeroData> heroes)
        {
            _heroes = new Dictionary<string, HeroData>();
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
                int pos = Random.Range(0, size);
                _NewBaseCard temp = _deck[i];
                _deck[i] = _deck[pos];
                _deck[pos] = temp;
            }
        }
    }
}
