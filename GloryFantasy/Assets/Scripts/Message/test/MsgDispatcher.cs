using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IMessage
{

    public interface MsgReceiver
    {
    }

    public delegate bool Condition();
    public delegate void Action();

    public static class MsgDispatcher
    {
        /// <summary>
        /// Trigger句柄，包含receiver, eventType, Condition和Action
        /// </summary>
        class MsgHandler
        {
            public MsgReceiver receiver;
            public int msgName;
            public Condition condition;
            public Action action;

            public MsgHandler(MsgReceiver receiver, int msgName, Condition condition, Action action)
            {
                this.receiver = receiver;
                this.msgName = msgName;
                this.condition = condition;
                this.action = action;
            }

            /// <summary>
            /// 触发Trigger
            /// </summary>
            public void strike()
            {
                if (condition())
                {
                    action();
                }
            }
        }

        static Dictionary<int, List<MsgHandler>> MsgHandlerDict = new Dictionary<int, List<MsgHandler>>();
        
        /// <summary>
        /// 给msgReciver增加注册MSG的函数
        /// </summary>
        /// <param name="self"></param>
        /// <param name="msgName"></param>
        /// <param name="condition"></param>
        /// <param name="action"></param>
        /// <param name="TriggerName"></param>
        public static void RegisterMsg(this MsgReceiver self, int msgName, Condition condition, Action action, string TriggerName = "NoDefine")
        {
            if (msgName < 0)
            {
                Debug.Log("RegisterMsg: " + TriggerName + "'s "+ msgName + "is not define");
            }
            if (null == condition)
            {
                Debug.Log("RegisterMsg: " + TriggerName + "'s condition" + "is null");
            }
            if (null == action)
            {
                Debug.Log("RegisterMsg: " + TriggerName + "'s action" + "is null");
            }
            
            if (!MsgHandlerDict.ContainsKey(msgName))
            {
                MsgHandlerDict[msgName] = new List<MsgHandler>();
            }

            var handlers = MsgHandlerDict[msgName];

            handlers.Add(new MsgHandler(self, msgName, condition, action));

        }
        /// <summary>
        /// 返回MsgReceiver
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static MsgReceiver GetMsgReceiver(this MsgReceiver self)
        {
            return self;
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="msgName"></param>
        /// <param name="targetReceiver">目标接收者，不填或null为广域广播</param>
        public static void SendMsg(int msgName, MsgReceiver targetReceiver = null)
        {
            if (msgName < 0)
            {
                Debug.Log("SendMsg: " + msgName + "is not define");
            }
            if (!MsgHandlerDict.ContainsKey(msgName))
            {
                Debug.Log("SendMsg: " + msgName + "had not been regeisted");
                return;
            }

            var handlers = MsgHandlerDict[msgName];
            var handlerCount = handlers.Count;

            for (int index = handlerCount - 1; index >= 0; index --)
            {
                var handler = handlers[index];

                if ((MonoBehaviour)handler.receiver != null)
                {

                    //单播
                    if (targetReceiver != null)
                    {
                        if (targetReceiver == handler.receiver)
                        {
                            handler.strike();
                        }
                    }
                    else
                    {
                        //广域广播
                        handler.strike();
                    }
                }
                else
                {
                    //接收者已经不存在则从广播列表里删除
                    handlers.Remove(handler);
                    Debug.Log("SendMsg: One " + msgName + "'s receiver had been destory");
                }
            }
        }
    }

    public class Trigger : Command
    {
        /// <summary>
        /// 注册这个Trigger的游戏物体
        /// </summary>
        public MsgReceiver speller;
        /// <summary>
        /// Trigger会被触发的消息
        /// </summary>
        public int msgName;
        /// <summary>
        /// Trigger的成立限定条件函数
        /// </summary>
        public IMessage.Condition condition;
        /// <summary>
        /// Trigger的执行函数
        /// </summary>
        public IMessage.Action action;
    }
}