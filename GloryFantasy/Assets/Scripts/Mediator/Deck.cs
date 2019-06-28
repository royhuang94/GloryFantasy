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
            //胡乱塞牌做测试
            deck.Add("GArrowrain_1");
            deck.Add("GCuringwind_1");
            deck.Add("GJump_1");
            _deck = deck;
            _hero = hero;
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
