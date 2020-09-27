using cocosocket4unity;
using FairyGUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf;
using System.Threading;

public class NoVedioNotice
{
    private GComponent main;

    public delegate void BtnCallBack(int code); //0否 1确定 
    public BtnCallBack UseCallBack;
    
    public NoVedioNotice(string des, BtnCallBack callback)
    {

        init();
        main.GetChild("des").asTextField.text = des;

        main.GetChild("replacemsg").asTextField.SetVar("p1", GameScene.Singleton.SvConf.MoneyReplaceVedioPrice + "");
        main.GetChild("replacemsg").asTextField.FlushVars();

        SetBtnCallBack(callback);
    }
    
    public void init()
    {
        main = UIPackage.CreateObject("GameUI", "novedionotice").asCom;
        GRoot.inst.AddChild(main);
        Vector2 screenPos = new Vector2(Screen.width / 2, Screen.height / 2);
        Vector2 logicScreenPos = GRoot.inst.GlobalToLocal(screenPos);
        main.xy = logicScreenPos;
    }
    
    public void SetBtnCallBack(BtnCallBack callback)
    {
        UseCallBack = callback;
        main.GetChild("close").asButton.onClick.Set(() =>
        {
            if (UseCallBack != null)
            {
                UseCallBack(0);
            }
            Destroy();
        });
        main.GetChild("no").asButton.onClick.Set(() =>
        {
            if (UseCallBack != null)
            {
                UseCallBack(2);
            }
            Destroy();
        });
        main.GetChild("get").asButton.onClick.Set(() =>
        {
            if (UseCallBack != null)
            {
                UseCallBack(1);
            }
            Destroy();
        });
    }
    
    //
    public void Destroy()
    {
        if (main != null)
        {
            main.Dispose();
        }
    }

    
}
