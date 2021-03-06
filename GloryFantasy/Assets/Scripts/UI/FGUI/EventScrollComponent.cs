using System;
using System.Collections.Generic;
using FairyGUI;
using GamePlay;
using GamePlay.Event;
using GamePlay.Input;
using IMessage;
using UnityEngine;
using Random = UnityEngine.Random;
using EventInfo = GamePlay.Event.EventAssembly.EventInfo;

namespace UI.FGUI
{
    public class EventScrollComponent : IComponent, MsgReceiver
    {
        #region 变量
        #region 事件轴icon素材
        public const string randomEventIcon = "RandomEvent";
        public const string disasterEventIcon = "DisasterEvent";
        public const string addEventIcon = "AddEvent";
        public const string skillEventIcon = "SkillEvent";

        public string[] eventIcons = {
            "RandomEvent",
            "AddEvent",
            "SkillEvent",
            "DisasterEvent"
        };

        #endregion
        #region 事件轴弹窗变量

        /// <summary>
        /// 事件轴上图标点击后展示的说明窗口
        /// </summary>
        private Window _eventDescribeWindow;
	
        private GComponent _eventDescribeFrame;
        /// <summary>
        /// 事件轴图标说明窗口内放具体说明文字的list
        /// </summary>
        private GList _eventDescribeList;
        
        /// <summary>
        /// 事件信息文本框
        /// </summary>
        private GTextField _textField;

        /// <summary>
        /// 下标与对应事件映射，每回合更新
        /// </summary>
        private Dictionary<int, List<string>> _eventNodeDic;
        
        private Dictionary<int, string> _currentEventNodeInfo;

        private int _selectedPhase;

        #endregion
        
        /// <summary>
        /// 事件轴list引用
        /// </summary>
        private GList _eventScrollList;

        private GComponent _button1;
        private GComponent _button2;
        private GComponent _button3;
        private GComponent _button4;
        private GComponent _button5;
        private List<GComponent> _buttonList;
	
        private GObject _lastClickedEventIcon;

        /// <summary>
        /// 事件轴使用的素材资源包定义
        /// </summary>
        private string _pkgName;
        
        #endregion


        public EventScrollComponent(string pkgName, string resName, GList list, List<GComponent> buttonList)
        {
            _lastClickedEventIcon = null;
            _pkgName = pkgName;
            _eventNodeDic = new Dictionary<int, List<string>>();
            _currentEventNodeInfo = new Dictionary<int, string>();
            _eventDescribeWindow = new Window();
            _eventDescribeFrame = UIPackage.CreateObject(pkgName, resName).asCom;
            _eventDescribeWindow.contentPane = _eventDescribeFrame;
            //_eventDescribeList = _eventDescribeFrame.GetChild("contentList").asList;
            
            _eventScrollList = list;

            _buttonList = buttonList;
            _button1 = buttonList[0];
            _button2 = buttonList[1];
            _button3 = buttonList[2];
            _button4 = buttonList[3];
            _button5 = buttonList[4];

            foreach(GComponent obj in _buttonList)
            {
                obj.onClick.Add(()=>
                {

                    Controller controller = obj.GetController("control");
                    if (controller.selectedIndex == 0) // 默认普通状态
                    {
                        for (int i = 0; i < _buttonList.Count; i++)
                        {
                            _buttonList[i].GetController("control").selectedIndex = 0;
                        }
                        controller.selectedIndex = 1;
                    }else
                    {
                        controller.selectedIndex = 0;
                    }
            
                    UpdateEventScroll(_buttonList.IndexOf(obj));
                    _selectedPhase = _buttonList.IndexOf(obj);
                });
            }
            
            MsgDispatcher.RegisterMsg(
                this.GetMsgReceiver(),
                (int)MessageType.EventNodeChange,
                () => { return true;},
                UpdateEventsMessage,
                "EventNode observer"
            );
            
            MsgDispatcher.RegisterMsg(
                this.GetMsgReceiver(),
                (int)MessageType.MPBegin,
                () => { return false; },
                UpdateButtonText,
                "EventButton text synchronize");
		
//            UpdateButtonText();
            UpdateEventsMessage();
//            UpdateEventScroll(0);
            
            // TODO: 改成鼠标悬浮显示
//            _eventScrollList.onClickItem.Add(OnclickEventIcon);
            
            // 手动设置最后一个图标的大小
            //_eventScrollList._children[_eventScrollList._children.Count-1].SetScale(1.5f,1.5f);

            // 随机改变icon的样式
            for (int i = 0; i < _eventScrollList._children.Count; i++)
            {
                _eventScrollList._children[i].icon = UIPackage.GetItemURL(pkgName,
                    eventIcons[Random.Range(0, _eventScrollList._children.Count - 1)]);
            }
        }


        /// <summary>
        /// 每回合更新按钮上的信息
        /// </summary>
        private void UpdateButtonText()
        {
            int round = Gameplay.Instance().roundProcessController.State.roundCounter;
            for (int i = 0; i < _buttonList.Count; i++)
            {
                string text = "第";
                text += round;
                text += "回合";
                _buttonList[i].GetChild("text").asTextField.text = text;
                round++;
            }
        }


        
        /// <summary>
        /// 更新事件轴
        /// </summary>
        /// <param name="selectedIndex">要更新第几个回合的事件轴信息，若为0表示当前回合</param>
        public void UpdateEventScroll(int selectedIndex)
        {
            if(_eventNodeDic.Count == 0 || _eventNodeDic[selectedIndex + 1] == null)
                return;
            _eventScrollList.RemoveChildren(0, -1, true);
            for (int i = 0; i < _eventNodeDic[selectedIndex + 1].Count; i++)
            {
                if(_eventNodeDic[selectedIndex + 1][i].Length == 0)
                    continue;
                GObject item = UIPackage.CreateObject(_pkgName, "EventIcon");
                item.icon = UIPackage.GetItemURL(_pkgName,
                    eventIcons[Random.Range(0, _eventScrollList._children.Count - 1)]);
                // 鼠标悬浮显示
                item.onRollOver.Add(() =>
                {
                    // 通过事件节点ID转换成对应下标，最下面是0，往上递增
                    // int index = (24 - int.Parse(obj.id.Substring(obj.id.Length - 2))) / 2;
                    int index = _eventScrollList.GetChildIndex(item);
                    index = index > 0 ? index : 0;            // 取正整数
                    
                    GetEventNodeInfo(_selectedPhase);         // 设置当前回合事件信息字典
                    
                    _eventDescribeFrame.GetChild("content").text = _currentEventNodeInfo[index];
                        
                    // 设置窗口位置
                    _eventDescribeWindow.SetXY((item.x+item.size.x * 1.2f) * item.scaleX, item.y+item.size.y * 2f, true);
                    
                    // 设置窗口显示
                    GRoot.inst.ShowPopup(_eventDescribeWindow);
                    
                });
                
                item.onRollOut.Add(() => {GRoot.inst.HidePopup(_eventDescribeWindow);});
                _eventScrollList.AddChild(item);
            }
        }
        
        
        /// <summary>
        /// 处理事件节点点击
        /// </summary>
        /// <param name="context"></param>
        public void OnclickEventIcon(EventContext context)
        {
            // TODO: 处理执行事件之后显示问题
            // 获取被点击的item
            GObject obj = context.data as GObject;
            if (obj != _lastClickedEventIcon && obj != null)
            {
                // 通过事件节点ID转换成对应下标，最下面是0，往上递增
//                int index = (24 - int.Parse(obj.id.Substring(obj.id.Length - 2))) / 2;
                int index = _eventScrollList.GetChildIndex(obj);
                index = index > 0 ? index : 0;            // 取正整数
                GetEventNodeInfo(_selectedPhase);
                Debug.Log("index: " + index);
                _eventDescribeFrame.GetChild("content").text = _currentEventNodeInfo[index];
                // 点击下标在可选事件列表内
//                if (index < Gameplay.Instance().eventScroll.EventScrollListCount)
//                {
//                    foreach (string eventMsg in GetEventNodeInfo(index + 1))
//                    {
//                        GComponent item = UIPackage.CreateObject(_pkgName, "eventScrollItem").asCom;
//					
//                        _textField = item.GetChild("n0").asTextField;
//					
//                        int line = (eventMsg.Length + 31) / 32;				// 事件信息所占行数，32是试验出来的每行最大字符数，过程暴力
//					
//                        item.SetSize(item.size.x, (line + 0.2f) * item.size.y);
//                        _textField.SetSize(_textField.size.x, _textField.size.y * line);		// 设置文本框大小，高度为单行高度 * 行数
//                        _textField.text = eventMsg;
//
//                        // 添加构造好的item，若要加多个，请根据需要数据添加
//                        _eventDescribeList.AddChild(item);
//                    }
//                    if(GetEventNodeInfo(index + 1).Count == 0)			// 事件全为Empty，即暂无事件
//                    {
//                        GComponent item = UIPackage.CreateObject(_pkgName, "eventScrollItem").asCom;
//                        item.GetChild("n0").asTextField.text = "暂无事件";
//                        _eventDescribeList.AddChild(item);
//                    }
//                }
//                // 点击节点不在事件列表里
//                else
//                {
//                    GComponent item = UIPackage.CreateObject(_pkgName, "eventScrollItem").asCom;
//                    item.GetChild("n0").asTextField.text = "暂无事件";
//                    _eventDescribeList.AddChild(item);
//                }

                // 更新上一次点击对象
                _lastClickedEventIcon = obj;

                // 设置窗口位置
                _eventDescribeWindow.SetXY((obj.x+obj.size.x * 1.2f) * obj.scaleX, obj.y+obj.size.y * 2f, true);
                // 设置窗口显示
                GRoot.inst.ShowPopup(_eventDescribeWindow);

            }
            else
            {
                //_eventDescribeWindow.Hide();
                _lastClickedEventIcon = null;
                GRoot.inst.HidePopup(_eventDescribeWindow);
            }
        }
        
        /// <summary>
        /// 更新事件信息字典
        /// </summary>
        private void UpdateEventsMessage()
        {
            int nodeAmount = Gameplay.Instance().eventScroll.EventScrollListCount;        // 获取事件节点个数

            int maxNodeAmount = 5 > nodeAmount ? nodeAmount : 5;		// 获取得到的节点数，取其和5之间的较小值

            _eventNodeDic.Clear();
            for (int i = 0; i < 5; i++)			// 5个节点
            {
                if (i < maxNodeAmount)
                {
                    List<string> eventNodeInfo = new List<string>();		// 处理过的事件信息集合
                    List<EventInfo> eventInfos = Gameplay.Instance().eventScroll.GetEventInfos(i);
                    foreach (EventInfo eventInfo in eventInfos)
                    {
                        if("Empty".Equals(eventInfo.EventName))			// 事件名为Empty则不处理
                            continue;
                        string temp = eventInfo.Effect.Replace("{amount}", eventInfo.Amount.ToString());
                        string effect = temp.Replace("{strenth}", eventInfo.Strength.ToString());
                        
                        eventNodeInfo.Add(eventInfo.EventName + "  " + effect);
                    }
                    _eventNodeDic.Add(i + 1, eventNodeInfo);
                }
                else        // 超过最大节点个数即节点个数不够五个，也就是没有信息了
                {
                    _eventNodeDic.Add(i + 1, null);
                }
                
                UpdateEventScroll(i);        // 更新事件节点信息时同时更新图标
            }
            UpdateButtonText();
        }
        
        /// <summary>
        /// 获取当前点击节点里的事件信息
        /// </summary>
        /// <param name="index">点击节点下标</param>
        /// <returns>该节点处理过的所有事件信息集合</returns>
        private void GetEventNodeInfo(int index)
        {
            int eventNodeIndex = 0;
            if (_eventNodeDic.ContainsKey(index + 1))
            {
                _currentEventNodeInfo.Clear();
                for (int i = 0; i < _eventNodeDic[index + 1].Count; i++)
                {
                    if(_eventNodeDic[index + 1][i].Length == 0)
                        continue;
                    _currentEventNodeInfo.Add(eventNodeIndex, _eventNodeDic[index + 1][i]);        // 字典存储下标对应的处理好的事件信息
                    eventNodeIndex++;
                }  
            }
        }
        
        /// <summary>
        /// 设置事件轴图标，若参数非法则直接结束运行
        /// </summary>
        /// <param name="pos">要设置的位置</param>
        /// <param name="iconName">要设置的图标，目前一共四种分别是redEventIcon,blueEventIcon,redEventIcon,yellowEventIcon</param>
        public void SetEventScrollIcon(int pos, string iconName)
        {
            if (pos < 0 || pos >= _eventScrollList._children.Count)
                return;
            try
            {
                _eventScrollList.GetChildAt(pos).icon = UIPackage.GetItemURL(_pkgName, iconName);
            }
            catch (Exception e)
            {
                Debug.Log("选择icon错误，请检查");
            }
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