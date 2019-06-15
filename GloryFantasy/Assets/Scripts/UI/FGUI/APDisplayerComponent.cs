using FairyGUI;
using GamePlay;
using IMessage;

namespace UI.FGUI
{
    public class APDisplayerComponent : IComponent, MsgReceiver
    {
        #region 变量
        
        private GTextField _APText;
        
        
        #endregion

        /// <summary>
        /// 初始化组件函数
        /// </summary>
        /// <param name="component">对应的展示组件</param>
        public APDisplayerComponent(GComponent component)
        {
            _APText = component.GetChild("APText").asTextField;
            
            MsgDispatcher.RegisterMsg(
                this.GetMsgReceiver(),
                (int)MessageType.APChange,
                () => { return true; },
                () => { _APText.text = Player.Instance().ap.ToString();});
        }
        
        
        /// <summary>
        /// 仿照主程写的写的接口
        /// </summary>
        T MsgReceiver.GetUnit<T>()
        {
            return this as T;
        }
        
        #region 不使用的函数
        
        public void Operation()
        {
            throw new System.NotImplementedException();
        }

        public void Add(IComponent component)
        {
            throw new System.NotImplementedException();
        }

        public IComponent GetChild(string comId)
        {
            throw new System.NotImplementedException();
        }
        
        #endregion
    }
}