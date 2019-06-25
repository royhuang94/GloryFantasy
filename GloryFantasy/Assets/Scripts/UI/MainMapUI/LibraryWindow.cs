using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

public class LibraryWindow : Window
{
    private Color _bgColor;
    private string _pkgName;
    private string _resName;

    
    /// <summary>
    /// 图书馆窗口构造函数
    /// </summary>
    /// <param name="bgColor">背景颜色</param>
    /// <param name="pkgName"></param>
    /// <param name="resName"></param>
    public LibraryWindow(Color bgColor, string pkgName, string resName)
    {
        _bgColor = bgColor;
        _pkgName = pkgName;
        _resName = resName;
    }

    protected override void OnInit()
    {
        this.modal = true;
        UIConfig.modalLayerColor = _bgColor;
        this.contentPane = UIPackage.CreateObject(_pkgName, _resName).asCom;
        this.CenterOn(GRoot.inst, true);
    }
}
