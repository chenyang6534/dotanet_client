using cocosocket4unity;
using FairyGUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf;
using System.Threading;

public class SkillInfo {
    private GComponent main;

    private int TypeID;
    public SkillInfo(int typeid)
    {
        TypeID = typeid;
        var clientitem = ExcelManager.Instance.GetSkillManager().GetSkillByID(typeid);
        if (clientitem == null)
        {
            return;
        }

        main = UIPackage.CreateObject("GameUI", "ItemInfo").asCom;
        GRoot.inst.AddChild(main);
        Vector2 screenPos = new Vector2(Screen.width / 2, Screen.height / 2);
        Vector2 logicScreenPos = GRoot.inst.GlobalToLocal(screenPos);
        main.xy = logicScreenPos;
        Init(clientitem);
    }

   


    //初始化
    public void Init(ExcelData.Skill clientitem)
    {
        if (main == null)
        {
            return;
        }
        //关闭按钮
        main.GetChild("close").asButton.onClick.Add(() => { Destroy(); });

        //图标
        main.GetChild("icon").asLoader.url = clientitem.IconPath;
        //等级
        main.GetChild("level").asTextField.text = "";
        //名字
        main.GetChild("name").asTextField.text = clientitem.Name;

        //描述
        main.GetChild("des").asTextField.text = clientitem.Des;

        main.GetChild("callbackbtn").visible = false;
        main.GetChild("usebtn").visible = false;
    }

    //
    public void Destroy()
    {
        if(main != null)
        {
            main.Dispose();
        }
    }

    
}
