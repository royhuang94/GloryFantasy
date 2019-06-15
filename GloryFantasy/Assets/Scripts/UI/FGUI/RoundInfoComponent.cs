using FairyGUI;
using GamePlay;
using IMessage;

namespace UI.FGUI
{
    public class RoundInfoComponent : IComponent, MsgReceiver
    {
        
        #region 变量
        
        /// <summary>
        /// 回合结束按钮上面那个说明文字
        /// </summary>
        private GTextField _roundText;
        
        #endregion


        public RoundInfoComponent(GTextField textField)
        {
            _roundText = textField;
            
            _roundText.text = "己方回合";
            MsgDispatcher.RegisterMsg(
                this.GetMsgReceiver(),
                (int)MessageType.MPBegin,
                () => { return true; },
                ShowRoundInfo,
                "Round text synchronize");
		
            MsgDispatcher.RegisterMsg(
                this.GetMsgReceiver(),
                (int)MessageType.AI,
                () => { return true; },
                () => { _roundText.text = "敌方回合";},
                "Round text synchronize");
		
            MsgDispatcher.RegisterMsg(
                this.GetMsgReceiver(),
                (int)MessageType.AIEnd,
                () => { return true;},
                () => { _roundText.text = "己方回合";},
                "Round text synchronize");
		
            MsgDispatcher.RegisterMsg(
                this.GetMsgReceiver(),
                (int)MessageType.UpdateSource,
                () => { return true; },
                () => { _roundText.text = "专注恢复"; },
                "Round text synchronize");
		
            MsgDispatcher.RegisterMsg(
                this.GetMsgReceiver(),
                (int)MessageType.DrawCard,
                () => { return true; },
                () => { _roundText.text = "抽牌阶段"; },
                "Round text synchronize");
        }
        
        
        /// <summary>
        /// 在文字说明处展示回合信息
        /// </summary>
        public void ShowRoundInfo()
        {
            int round = Gameplay.Instance().roundProcessController.State.roundCounter;
            string text = "第";
            text += round;
            text += "回合";
            _roundText.text = text;
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