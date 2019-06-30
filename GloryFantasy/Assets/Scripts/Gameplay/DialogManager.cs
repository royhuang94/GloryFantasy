using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using FairyGUI;
using GameCard;
using UnityEngine.Serialization;
using MainMap;

namespace StoryDialog
{
	public struct DialogMessage
	{
		public int order;
		public int position;
		public string name;
		public string text;
		public string pic;

		public DialogMessage(int order, int position, string name, string text, string pic)
		{
			this.order = order;
			this.position = position;
			this.name = name;
			this.text = text;
			this.pic = pic;
		}
	}
 
	public class DialogManager : UnitySingleton<DialogManager>
	{
        private object vistor;
        private string check;
        private DialogWindow _dialogWindowLeft;
		private DialogWindow _dialogWindowRight;
		private GButton _continueBtn;
		private int _length;
		private List<DialogMessage> _dialogMessages;
		private int i;

		public bool canShowWindow;

		public bool getCanShowWindow
		{
			get { return canShowWindow; }
			set { canShowWindow = value; }
		}

		/// <summary>
		/// 组件初始化
		/// </summary>
		private void Init()
		{
			Debug.Log("dm start");
			_dialogWindowLeft = new DialogWindow(Color.gray, "MainMapUI", "DialogMessage_left");
			_dialogWindowRight = new DialogWindow(Color.gray, "MainMapUI", "DialogMessage_right");
			i = 0;
			canShowWindow = true;
		}

		// Update is called once per frame
		void Update()
		{
			if (i < _length)		// 对话还没完
			{
				if (canShowWindow)		// 显示窗口
				{
					if (_dialogMessages[i].position == 0)		// 为0则在左边
					{
						_dialogWindowLeft.SetDialogMessage(_dialogMessages[i]);
						if (i != 0 && _dialogMessages[i - 1].position == 1)
						{
							UpdateWindow(_dialogWindowLeft, _dialogWindowRight);
						}
						_dialogWindowLeft.Show();
					}
					else
					{
						_dialogWindowRight.SetDialogMessage(_dialogMessages[i]);
						if (i != 0 && _dialogMessages[i - 1].position == 0)
						{
							UpdateWindow(_dialogWindowRight, _dialogWindowLeft);
						}

						_dialogWindowRight.Show();
					}

					i++;
					canShowWindow = false;
				}
			}
			else
			{
				if (!canShowWindow) return;
				_dialogWindowRight.Hide();
				_dialogWindowLeft.Hide();
                DialogOver(vistor, check);
			}
		}
        public bool RequestDialog(object o, string check)
        {
            if(vistor == null && this.check == null)
            {
                vistor = o;
                this.check = check;
                if (o is MainMapManager)
                {
                    ShowDialog(check);
                    return true;
                }
                else if (o is Library)
                {
                    ShowDialog(check);
                    return true;
                }
                else if (o is Monster)
                {
                    ShowDialog(check);
                    return true;
                }
                else if (o is Charactor)
                {
                    return true;
                }
                else
                {
                    Debug.Log("无类型");
                }
            }

            return false;
        }
        public bool DialogOver(object o, string check)
        {
            if(vistor != null && this.check != null)
            {
                if (o is MainMapManager)
                {
                    MainMapManager master = (MainMapManager)o;
                    BeZero();
                    return true;
                }
                else if (o is Monster)
                {
                    Monster master = (Monster)o;
                    master.InToBattle();
                    Debug.Log("TryIntoBattle");
                    BeZero();
                    return true;
                }
                else if (o is Charactor)
                {
                    BeZero();
                    return true;
                }
                BeZero();
            }

            return false;
        }
        private void BeZero()
        {
            if(vistor!=null||check!=null)
            {
                vistor = null;
                check = null;
            }
            Debug.Log("Bezero");
        }
		/// <summary>
		/// 更新窗口，把当前窗口的图片和头像设为可见，上一个窗口设为不可见
		/// </summary>
		/// <param name="currentDialogWindow">当前窗口</param>
		/// <param name="prevDialogWindow">上一个窗口</param>
		private void UpdateWindow(DialogWindow currentDialogWindow, DialogWindow prevDialogWindow)
		{
			if (currentDialogWindow.isShowing)
			{
				currentDialogWindow.contentPane.GetChild("n0").visible = true;
				currentDialogWindow.contentPane.GetChild("dialogPic").visible = true;
				currentDialogWindow.contentPane.GetChild("dialogName").visible = true;
			}

			prevDialogWindow.contentPane.GetChild("n0").visible = false;
			prevDialogWindow.contentPane.GetChild("dialogPic").visible = false;
			prevDialogWindow.contentPane.GetChild("dialogName").visible = false;
		}
		
		/// <summary>
		/// 开放接口，进入对话调用
		/// </summary>
		/// <param name="filename">对话文件名</param>
		public void ShowDialog(string filename)
		{
			Init();
			_dialogMessages = GetDialogList(filename);
			_length = _dialogMessages.Count;

		}

		/// <summary>
		/// 获得对话列表
		/// </summary>
		/// <param name="filename">对话文件名</param>
		/// <returns>列表，每一项元素都是一个 DialogMessage 结构体</returns>
		private List<DialogMessage> GetDialogList(string filename)
		{
			List<DialogMessage> dialogList = new List<DialogMessage>();
			Debug.Log("StoryDialog/" + filename);
			TextAsset json = Resources.Load<TextAsset>("StoryDialog/" + filename);
			JsonData dialogJsonData = JsonMapper.ToObject(json.text);
			for (int i = 0; i <	dialogJsonData.Count; i++)
			{
				JsonData message = dialogJsonData[i];
				int order = (int)message["order"];
				int position = (int) message["position"];
				string name = message["name"].ToString();
				string text = message["text"].ToString();
				string pic = message["pic"].ToString();
				DialogMessage dialogMessage = new DialogMessage(order, position, name, text, pic);
				dialogList.Add(dialogMessage);
				
			}
			return dialogList;
		}
	}
}
