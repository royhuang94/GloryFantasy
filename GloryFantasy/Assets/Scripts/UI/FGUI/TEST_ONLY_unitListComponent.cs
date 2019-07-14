using System;
using System.Collections.Generic;
using FairyGUI;
using GamePlay;
using GamePlay.Input;
using GameUnit;
using IMessage;
using LitJson;
using UnityEngine;

namespace UI.FGUI
{
    //public class TEST_ONLY_unitListComponent : IComponent
    //{

    //    private Window _unitListWindow;
    //    private GComponent _unitListFrame;
    //    private GButton _button;
    //    private GList _unitList;

    //    private string _pkgName;
    //    private const string _unitItem = "unitButton";
    //    private List<string> _enemyList = new List<string>();

    //    private GameObject _Test_Obj;
        
    //    private TEST_ONLY_unitListComponent()
    //    {
            
    //    }

    //    public TEST_ONLY_unitListComponent(string pkgName, string resName, GButton button)
    //    {
    //        _unitListFrame = UIPackage.CreateObject(pkgName, resName).asCom;

    //        _button = button;
    //        if (_unitListFrame == null) return;
    //        _pkgName = pkgName;
    //        _unitList = _unitListFrame.GetChild("n4").asList;
            
    //        _unitListWindow = new Window();
    //        _unitListWindow.contentPane = _unitListFrame;
    //        _unitListWindow.CenterOn(GRoot.inst, true);
            
            
            
    //        _button.onClick.Add(() =>
    //        {
    //            if (!_unitListWindow.isShowing)
    //            {
    //                GRoot.inst.ShowPopup(_unitListWindow);
    //            }
    //        });
            
    //        _Test_Obj = GameObject.Find("TEST_ONLY_Empty");
    //        LoadUnits();
    //        foreach (string enemyId in _enemyList)
    //        {
    //            String name = UnitDataBase.Instance().unitsData[enemyId]["Name"].ToString();
                
    //            GButton item = UIPackage.CreateObject(_pkgName, _unitItem).asButton;

    //            item.GetChild("n3").asTextField.text = name;
                
    //            item.onRollOver.Add(() => { OnRollOver(enemyId); });
    //            item.onRollOut.Add(() =>
    //            {
    //                FGUIInterfaces.Instance().cardDescribeWindow.Hide();
    //            });
    //            item.onClick.Add(()=>{OnClick(enemyId);});
                
    //            _unitList.AddChild(item);
    //        }
    //    }

    //    public void LoadUnits()
    //    {
    //        #region 自己加了一些怪物
    //        _enemyList.Add("Shadow_1");
    //        _enemyList.Add("Shadow_2");
    //        _enemyList.Add("Shadow_3");
    //        _enemyList.Add("Tentacle_1");
    //        _enemyList.Add("Tentacle_2");
    //        _enemyList.Add("Tentacle_3");
    //        _enemyList.Add("ShadowArcher_1");
    //        _enemyList.Add("ShadowArcher_2");
    //        _enemyList.Add("ShadowArcher_3");
    //        #endregion
    //    }

    //    public void OnRollOver(string unitId)
    //    {
    //        JsonData jsonData = UnitDataBase.Instance().unitsData[unitId];

    //        string tagInTotal = "Tag :";
    //        if (jsonData["Tag"].Count != 0)
    //        {
    //            JsonData tagData = jsonData["Tag"];
                
    //            for (int i = 0; i < tagData.Count; i++)
    //            {
    //                tagInTotal += tagData[i].ToString();
    //                tagInTotal += " / ";
    //            }
    //        }

    //        string color = "";
    //        if (jsonData["Color"].Count != 0)
    //        {
    //            JsonData colorData = jsonData["Color"];
    //            for (int i = 0; i < colorData.Count; i++)
    //            {
    //                color += colorData[i].ToString();
    //                color += " / ";
    //            }
    //        }

    //        string valueInfo = "颜色： " + color + "    生命：  " + jsonData["Hp"] + "\n攻击： " + jsonData["Atk"]
    //                           + "    范围： " + jsonData["Rng"] + "\n移动： " + jsonData["Mov"];

    //        string effectInfo = tagInTotal + "  " + jsonData["Effort"];
            
    //        FGUIInterfaces.Instance().SetDescribeWindowContentText(jsonData["Name"].ToString(), valueInfo, effectInfo);
    //        FGUIInterfaces.Instance().SetDescribeWindowShow();
    //    }

    //    public void OnClick(string unitId)
    //    {
    //        GRoot.inst.HidePopup(_unitListWindow);
    //        Ability.Ability testAbility = _Test_Obj.AddComponent<TEST_ONLY_SummonUnit>();
    //        testAbility.Init(unitId);
    //        Gameplay.Instance().gamePlayInput.OnUseOrderCard(testAbility);
    //    }

    //    public override string ToString()
    //    {
    //        return "unitListComponent";
    //    }

    //    #region 不使用的函数
    //    public void Operation()
    //    {
    //        throw new System.NotImplementedException();
    //    }

    //    public void Add(IComponent component)
    //    {
    //        throw new System.NotImplementedException();
    //    }

    //    public IComponent GetChild(string comId)
    //    {
    //        throw new System.NotImplementedException();
    //    }
    //    #endregion
    //}

    //public class TEST_ONLY_SummonUnit : Ability.Ability
    //{
    //    public override void Init(string unitId)
    //    {
    //        base.Init("TEST_ONLY_SummonUnit");
    //        MsgDispatcher.RegisterMsg(
    //            new TTOSummonUnit(this.GetCardReceiver(this), unitId),
    //            "TEST_SUMMON",
    //            true);
    //    }
    //}

    //public class TTOSummonUnit : Trigger
    //{
    //    private string _unitId;

    //    public TTOSummonUnit(MsgReceiver speller, string unitId)
    //    {
    //        register = speller;
    //        _unitId = unitId;
            
    //        msgName = (int) MessageType.CastCard;
    //        condition = Condition;
    //        action = Action;
    //    }

    //    private bool Condition()
    //    {
    //        return true;
    //    }

    //    private void Action()
    //    {
    //        DispositionCommand dispositionCommand = new DispositionCommand(
    //            _unitId,
    //            OwnerEnum.Enemy,
    //            BattleMap.BattleMap.Instance().GetSpecificMapBlock(
    //                Gameplay.Instance().gamePlayInput.InputFSM.TargetList[0])
    //        );
    //        dispositionCommand.Excute();
    //    }
    //}
    
}