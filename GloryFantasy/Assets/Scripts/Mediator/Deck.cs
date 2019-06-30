using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mediator
{
    public class Deck
    {
        private List<string> _qpUnit = new List<string> { "HKnight_3", "HLunamage_3" };
        public List<string> _deck; // 牌库中所有牌的id
        public string _hero; // 主角牌id
        public List<string> _unitsWithQuickPlat; // 具有即时备战的单卡

        public Deck(List<string> deck, string hero)
        {
            FreshDeck(deck);
            deck.Remove(hero);
            _hero = hero;
            //deck.Add("")
        }
        public void FreshDeck(List<string> deck)
        {
            _deck = deck;
            _unitsWithQuickPlat = new List<string> { _hero };
            for (int i = 0; i < _deck.Count; i++)
            {
                if (_qpUnit.Contains(_deck[i]))
                {
                    _unitsWithQuickPlat.Add(_deck[i]);
                    _deck.Remove(_deck[i--]);
                }
            }

        }
    }
}
