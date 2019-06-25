using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using FairyGUI;
using GameCard;

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

		private DialogWindow _dialogWindowLeft;
		private DialogWindow _dialogWindowRight;
		private GButton _continueBtn;
		private int _length;
		private List<DialogMessage> _dialogMessages;
		private int i;

		public bool test;

		public bool test1
		{
			get { return test; }
			set { test = value; }
		}

		void Start()
		{
			_dialogWindowLeft = new DialogWindow(Color.gray, "MainMapUI", "DialogMessage_left");
			_dialogWindowRight = new DialogWindow(Color.gray, "MainMapUI", "DialogMessage_right");
			i = 0;
			test = true;
		}

		// Update is called once per frame
		void Update()
		{
			if (i < _length)
			{
//				if (!_dialogWindowLeft.isShowing && !_dialogWindowRight.isShowing)
//				{
//					Debug.Log("i: " + i + " p:" + _dialogMessages[i].position + " index:" + _dialogMessages[i].order);
//					if (_dialogMessages[i].position == 0)
//					{
//						_dialogWindowLeft.SetDialogMessage(_dialogMessages[i]);
//						_dialogWindowLeft.Show();
//					}
//					else
//					{
//						_dialogWindowRight.SetDialogMessage(_dialogMessages[i]);
//						_dialogWindowRight.Show();
//					}
//
//					i++;
//				}
				if (test)
				{
					if (_dialogMessages[i].position == 0)
					{
						_dialogWindowLeft.SetDialogMessage(_dialogMessages[i]);
						_dialogWindowLeft.Show();
					}
					else
					{
						_dialogWindowRight.SetDialogMessage(_dialogMessages[i]);
						_dialogWindowRight.Show();
					}

					i++;
					test = false;
				}
			}
			else
			{
				if (!test) return;
				_dialogWindowRight.Hide();
				_dialogWindowLeft.Hide();
			}
		}

		public void ShowDialog(string filename)
		{
			_dialogMessages = GetDialogList(filename);
			_length = _dialogMessages.Count;

		}

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
//				string pic = message["pic"].ToString();
				DialogMessage dialogMessage = new DialogMessage(order, position, name, text, null);
				dialogList.Add(dialogMessage);
				
			}
			return dialogList;
		}
	}
}
