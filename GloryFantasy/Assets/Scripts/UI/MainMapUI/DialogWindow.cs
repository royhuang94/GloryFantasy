using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;
using StoryDialog;

public class DialogWindow : Window 
{

	private Color _bgColor;
	private string _pkgName;
	private string _resName;

	private string _dialogNameText;
	private string _dialogTextText;
	private GButton _continueBtn;
	private GLoader _picLoader;

	private GTextField _dialogName;
	private GTextField _dialogText;

	private bool _canUpdate;

	/// <summary>
	/// 对话窗口构造函数
	/// </summary>
	/// <param name="bgColor">背景颜色</param>
	/// <param name="pkgName"></param>
	/// <param name="resName"></param>
	public DialogWindow(Color bgColor, string pkgName, string resName)
	{
		Debug.Log("construct");
		_bgColor = bgColor;
		_pkgName = pkgName;
		_resName = resName;
		_canUpdate = false;
	}

	protected override void OnInit()
	{
		Debug.Log("window init");
		this.modal = true;
		UIConfig.modalLayerColor = _bgColor;
		this.contentPane = UIPackage.CreateObject(_pkgName, _resName).asCom;
		this.CenterOn(GRoot.inst, true);

		_dialogName = this.contentPane.GetChild("dialogName").asTextField;
		_dialogText = this.contentPane.GetChild("dialogText").asTextField;

		_picLoader = this.contentPane.GetChild("dialogPic").asLoader;
		_continueBtn = this.contentPane.GetChild("dialogContinueButton").asButton;
		_continueBtn.onClick.Add(OnClickContinue);

		_dialogName.text = _dialogNameText;
		_dialogText.text = _dialogTextText;

		_canUpdate = true;
	}

	public void SetDialogMessage(DialogMessage dialogMessage)
	{
		_dialogNameText = dialogMessage.name;
		_dialogTextText= dialogMessage.text;
		if (_canUpdate)
		{
			_dialogName.text = _dialogNameText;
			_dialogText.text = _dialogTextText;
		}
	}
	
	private void OnClickContinue()
	{
//		this.Hide();
		DialogManager.Instance().test1 = true;
	}
}
