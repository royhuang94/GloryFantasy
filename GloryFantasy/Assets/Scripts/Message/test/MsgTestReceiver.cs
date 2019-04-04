using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using IMessage;

//测试中的消息中心使用样例

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
    //继承TOOL让策划有限定的读取方法使用
    class Trigger : GameplayTool
    {
        public int msgName;
        public IMessage.Condition condition;
        public IMessage.Action action;
    }

    //一个Trigger的样例
    class Trigger1 : Trigger
    {
        public Trigger1()
        {
            //消息类型
            msgName = (int)TriggerType.ActiveAbility;
            //Condition和Action的初始化
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

    public class MsgTestReceiver : MonoBehaviour, IMessage.MsgReceiver
    {
        private void Awake()
        {
            //实例化对应的trigger
            Trigger trigger = new Trigger1();
            //进行注册
            IMessage.MsgDispatcher.RegisterMsg(this, trigger.msgName, trigger.condition, trigger.action);
        }
    }
}