using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MsgTestButton : MonoBehaviour {

    //[SerializeField] public IMessage.MsgReceiver targetReceiver;
    public GameObject targetReceiver;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            //IMessage.MsgDispatcher.SendMsg((int)MsgTestType.A, targetReceiver.GetComponent<IMessage.MsgReceiver>());
            IMessage.MsgDispatcher.SendMsg((int)MsgTestType.A);
        });
    }
}
