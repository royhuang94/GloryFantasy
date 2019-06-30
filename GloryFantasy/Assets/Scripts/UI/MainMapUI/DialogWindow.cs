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
	private string _dialogPicPath;
	private GButton _continueBtn;
	private GButton _fullContinueBtn;
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
		_bgColor = bgColor;
		_pkgName = pkgName;
		_resName = resName;
		_canUpdate = false;
	}

	protected override void OnInit()
	{
		if(_canUpdate)
			return;
		this.modal = true;
		UIConfig.modalLayerColor = _bgColor;
		this.contentPane = UIPackage.CreateObject(_pkgName, _resName).asCom;
		this.CenterOn(GRoot.inst, true);

		_dialogName = this.contentPane.GetChild("dialogName").asTextField;
		_dialogText = this.contentPane.GetChild("dialogText").asTextField;

		_picLoader = this.contentPane.GetChild("dialogPic").asLoader;
		_continueBtn = this.contentPane.GetChild("dialogContinueButton").asButton;
		_continueBtn.onClick.Add(OnClickContinue);
		_fullContinueBtn = this.contentPane.GetChild("FullContinueButton").asButton;
		_fullContinueBtn.onClick.Add(OnClickContinue);
		
		_dialogName.text = _dialogNameText;
		_dialogText.text = _dialogTextText;

		_canUpdate = true;
	}

	public void SetDialogMessage(DialogMessage dialogMessage)
	{
		if(!_canUpdate)
			OnInit();
		_dialogNameText = dialogMessage.name;
		_dialogTextText= dialogMessage.text;
		_dialogPicPath = dialogMessage.pic;
		if (_canUpdate)
		{
			_dialogName.text = _dialogNameText;
			_dialogText.text = _dialogTextText;
			// 设置对话人物图片
			_picLoader.icon = UIPackage.GetItemURL("MainMapUI", _dialogPicPath);
		}
	}

	/// <summary>
	/// 设置该窗口组件可见性，用于窗口显示
	/// </summary>
	/// <param name="dialogVisible"></param>
	public void SetDialogWindowVisible(bool dialogVisible)
	{
		this.contentPane.GetChild("n0").visible = dialogVisible;
		this._picLoader.visible = dialogVisible;
		this._dialogName.visible = dialogVisible;
		this._dialogText.visible = dialogVisible;
		this._continueBtn.visible = dialogVisible;
	}
	
	
	private void OnClickContinue()
	{
//		this.Hide();
		DialogManager.Instance().getCanShowWindow = true;
	}
}
