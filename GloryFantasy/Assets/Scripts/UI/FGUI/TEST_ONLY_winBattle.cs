using FairyGUI;
using IMessage;
using UnityEngine;

namespace UI.FGUI.cardBook
{
    public class TEST_ONLY_winBattle : IComponent
    {

        private GButton _button;
        
        private TEST_ONLY_winBattle()
        {
            
        }

        public TEST_ONLY_winBattle(GButton winButton)
        {
            _button = winButton;
            _button.onClick.Add(OnClickWinButton);
        }
        
        /// <summary>
        /// 一键胜利按钮点击事件处理函数
        /// </summary>
        private void OnClickWinButton()
        {
            Debug.Log("Oh Shit");
            MsgDispatcher.SendMsg((int)MessageType.WIN);        // 发送胜利消息
        }

        public override string ToString()
        {
            return "OneKeyToWinComponent";
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