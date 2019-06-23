using UnityEngine;

using System.Collections;
using UnityEditor;//需要使用到UnityEditor
using IMessage;

public class WindowTest : MonoBehaviour
{
    [MenuItem("MyMenu/YouWin")] 
    static void YouWin()          
    {
        MsgDispatcher.SendMsg((int)IMessage.MessageType.WIN);
    }
}