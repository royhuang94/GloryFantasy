using IMessage;
using UnityEngine;

namespace GameCard
{
    public class NewCardManager : UnitySingleton<NewCardManager>, MsgReceiver
    {
        #region 变量
        
        
        #endregion
        
        /// <summary>
        /// 仿照主程写的写的接口
        /// </summary>
        T IMessage.MsgReceiver.GetUnit<T>()
        {
            return this as T;
        }
        
        
    }
}