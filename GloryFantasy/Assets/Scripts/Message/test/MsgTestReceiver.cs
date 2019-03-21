using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using IMessage;

enum MsgTestType
{
    A,
    B,
    C,
    D
};

public class MsgTestReceiver : MonoBehaviour, IMessage.MsgReceiver
{
    private void Awake()
    {
        IMessage.MsgDispatcher.RegisterMsg(this, (int)MsgTestType.A, Condition, Action);
        gameObject.SetActive(false);
    }

    private bool Condition()
    {
        return true;
    }

    private void Action()
    {
        gameObject.SetActive(true);
    }
}

//TODO:扩充这个ITrigger
namespace ITrigger
{
    class Trigger : Command
    {
        public int msgName;
        public IMessage.Condition condition;
        public IMessage.Action action;
    }

    class Trigger1 : Trigger
    {
        Trigger1()
        {
            msgName = (int)TriggerType.ActiveAbility;
            condition = Condition;
            action = Action;
        }

        private bool Condition()
        {
            if (GetAttacker().name == "fsaf")
                return false;
            else
                return true;
        }

        private void Action()
        {
            
        }
    }
}