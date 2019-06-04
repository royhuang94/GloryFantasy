using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GamePlay;

namespace IMessage
{

    enum MessageType
    {
        UpdateSource, //？
        RoundEnd,    // 阶段结束消息
        BP, //开始阶段
        MPBegin, //主要阶段开始
        MPEnd, //主要阶段结束
        EP, //结束阶段
        CastCard, //发动卡片
        Summon, //召唤
        DrawCard, //抽牌
        Prepare,//准备阶段
        Discard,//弃牌阶段
        AI,     //敌人行动阶段
        AIEnd,  // 敌人行动结束阶段
        
        WIN,    // 胜利消息
        LOSE,    // 失败消息
        
        HandcardChange,        // 手牌变动消息
        CardsetChange,         // 卡牌堆变动消息
        CooldownlistChange,    // 冷却列表变动消息
        
        AddInHand, //加入手牌
        AnnounceAttack, //攻击宣言
        ActiveAbility, //异能发动
        SelectionOver, // InputFSMSelectState结束消息
        
        RegionChange, //战区归属权变更消息
        
        #region ATK 时点部分
        BeAttacked, //被攻击
        Damage, //造成伤害
        BeDamaged, //被伤害
        Kill, //杀死了什么
        BeKilled, //被杀死
        Dead, //死亡
        ToBeKilled, //即将被杀死
        #endregion

        #region 状态
        HpChanged,
        #endregion

        #region 复合信息
        ColliderChange, // 碰撞器状态变化
        AfterColliderChange, // 碰撞器状态更新完成后
        #endregion

        Move, //开始移动
        Moved, //单位被移动
        Aftermove, //移动结束
        
        RoundsEnd,  //回合结束
        
        Encounter, // 遭遇战

        BattleSate//战区状态

        
    };

    public interface MsgReceiver
    {
        /// <summary>
        /// 返回接收者接口所依附的基类,注意一定要保证请求的基类是正确的
        /// </summary>
        /// <returns></returns>
        T GetUnit<T>() where T : MonoBehaviour;
    }

    public class GlobalReceiver : MsgReceiver
    {
        T MsgReceiver.GetUnit<T>()
        {
            //不想被调用这个方法，所以写成显示接口，可以减少失误调用
            throw new System.NotImplementedException();
        }
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
            public bool DoOnce;

            public MsgHandler(MsgReceiver receiver, int msgName, Condition condition, Action action, bool doOnce)
            {
                this.receiver = receiver;
                this.msgName = msgName;
                this.condition = condition;
                this.action = action;
                this.DoOnce = doOnce;
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

                //如果DoOnce标记为真，则接收者设为null，这样在下次执行该trigger时，会取消执行并将这个trigger清除
                if (DoOnce)
                    receiver = null;
            }
        }
        
        private static Dictionary<int, List<int>> Inverse(Dictionary<int, List<int>> keyValuePairs)
        {
            Dictionary<int, List<int>> res = new Dictionary<int, List<int>>();
            foreach (KeyValuePair<int, List<int> >keyValuePair in keyValuePairs)
            {
                foreach (int i in keyValuePair.Value)
                {
                    if (!res.ContainsKey(i))
                        res.Add(i, new List<int>());
                    res[i].Add(keyValuePair.Key);

                }
            }
            return res;
        }

        private static Dictionary<int, List<int>> ComplexMsgType = Inverse(new Dictionary<int, List<int>>
        {
            {
                (int)MessageType.ColliderChange, new List<int>
                {
                    (int)MessageType.Aftermove,
                    (int)MessageType.Dead,
                    (int)MessageType.Summon,
                    (int)MessageType.Moved,
                }
            },
            {
                (int)MessageType.AfterColliderChange, new List<int>
                {
                    (int)MessageType.ColliderChange
                }
            }
            
        });

        static Dictionary<int, List<MsgHandler>> MsgHandlerDict = new Dictionary<int, List<MsgHandler>>();

        static GlobalReceiver globalReceiver = new GlobalReceiver();

        /// <summary>
        /// 注册全局Trigger，没有特定的接收者，每场遭遇后清空
        /// </summary>
        /// <param name="msgName">注册的消息类型</param>
        /// <param name="condition">条件函数</param>
        /// <param name="action">执行函数</param>
        /// <param name="TriggerName">Debug消息使用的别名</param>
        /// <param name="DoOnce">该Trigger是否只执行一次，默认为false</param>
        public static void RegisterMsg(int msgName, Condition condition, Action action, string TriggerName = "NoDefine", bool DoOnce = false)
        {
            RegisterMsg(globalReceiver, msgName, condition, action, TriggerName, DoOnce);
        }
        /// <summary>
        /// 给msgReciver增加注册MSG的函数
        /// </summary>
        /// <param name="trigger">触发器实例</param>
        /// <param name="TriggerName">触发器名字</param>
        /// <param name="DoOnce">是否只执行一次</param>
        public static void RegisterMsg(Trigger trigger, string TriggerName = "NoDefine", bool DoOnce = false)
        {
            RegisterMsg(trigger.register, trigger.msgName, trigger.condition, trigger.action, TriggerName, DoOnce);
        }
        /// <summary>
        /// 给msgReciver增加注册MSG的函数
        /// </summary>
        /// <param name="self"></param>
        /// <param name="msgName">注册的消息类型</param>
        /// <param name="condition">条件函数</param>
        /// <param name="action">执行函数</param>
        /// <param name="TriggerName">Debug消息使用的别名</param>
        /// <param name="DoOnce">该Trigger是否只执行一次，默认为false</param>
        public static void RegisterMsg(this MsgReceiver self, int msgName, Condition condition, Action action, string TriggerName = "NoDefine", bool DoOnce = false)
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

            handlers.Add(new MsgHandler(self, msgName, condition, action, DoOnce));

            Debug.Log("RegisterMsg: " + TriggerName + "successfully register");

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
                //Debug.Log("SendMsg: " + msgName + "is not define");
            }
            if (!MsgHandlerDict.ContainsKey(msgName))
            {
                //Debug.Log("SendMsg: " + msgName + "had not been regeisted");
                return;
            }

            List<int> queue = new List<int> { msgName };
            int pointer = 0;
            List<MsgHandler> handlers = new List<MsgHandler>();
            while (queue.Count > pointer)
            {
                handlers.AddRange(MsgHandlerDict[queue[pointer]]);
                if (ComplexMsgType.ContainsKey(queue[pointer]))
                {
                    foreach(int i in ComplexMsgType[queue[pointer]])
                    {
                        if (!(queue.Contains(i)))
                            queue.Add(i);
                           
                    }
                }

                pointer += 1;
            }
            var handlerCount = handlers.Count;

            for (int index = handlerCount - 1; index >= 0; index --)
            {
                var handler = handlers[index];
                if (handler.receiver != null && !handler.receiver.Equals(null))
                //if ((MonoBehaviour)handler.receiver != null)
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

    public class Trigger : GameplayTool
    {
        /// <summary>
        /// 注册这个Trigger的游戏物体
        /// </summary>
        public MsgReceiver register;
        /// <summary>
        /// Trigger会被触发的消息
        /// </summary>
        public int msgName;
        /// <summary>
        /// Trigger的成立限定条件函数
        /// </summary>
        public Condition condition;
        /// <summary>
        /// Trigger的执行函数
        /// </summary>
        public Action action;
    }
}