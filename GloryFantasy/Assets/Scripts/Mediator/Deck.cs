using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mediator
{
    
    public class Deck
    {
        public List<HeroData> _heros;
        public class HeroData
        {
            string id;
            List<string> additionalBuff;
            List<string> arks;
        }
    }
}
