using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using IMessage;

public class FuHuo : MonoBehaviour
{
    string TriggerName;

    private void Start()
    {
        
    }

}

public class TFuHuo : Trigger
{
    TFuHuo(MsgReceiver _speller)
    {
        speller = _speller;
        msgName = (int)TriggerType.Dead;
        condition = Condition;
        action = Action;
    }

    private bool Condition()
    {
        if (GetDead().GetMsgReceiver() == speller)
            return true;
        else
            return false;
    }

    private void Action()
    {

    }
}
