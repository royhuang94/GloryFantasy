using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        Deck(List<HeroData> heroes)
        {
            _heroes = new Dictionary<string, HeroData>();
            foreach(HeroData heroData in heroes)
            {
                _heroes.Add(heroData.id, heroData);
            }
        }
    }
}
