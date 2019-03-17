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
