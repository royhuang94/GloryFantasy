using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IMessage;

namespace Ability
{
    public class Spell : Ability
    {
        public IMessage.Action OnCast;
        public delegate void SelectionOver();
        public SelectionOver selectionOver;
        public void Cast()
        {
            selectionOver();
            AbilityStack.push(OnCast);
            MsgDispatcher.SendMsg((int)MessageType.CastCard);
            AbilityStack.pump();
        }
    }
}
