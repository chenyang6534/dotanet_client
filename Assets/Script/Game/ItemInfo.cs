using cocosocket4unity;
using FairyGUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf;
using System.Threading;

public class ItemInfo {
    private GComponent main;

    private int TypeID;
    public ItemInfo(int typeid)
    {
        TypeID = typeid;
        var clientitem = ExcelManager.Instance.GetItemManager().GetItemByID(typeid);
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
    public void Init(ExcelData.Item clientitem)
    {
        if (main == null)
        {
            return;
        }
        //关闭按钮
        main.GetChild("close").asButton.onClick.Add(() => { Destroy(); });

        //图标
        main.GetChild("icon").asLoader.url = clientitem.IconPath;
        //名字
        main.GetChild("name").asTextField.text = clientitem.Name;

        //描述
        main.GetChild("des").asTextField.text = clientitem.Des;
    }

    //增加额外的描述文字
    public void AddDes(string adddes)
    {
        if (main == null)
        {
            return;
        }
        //描述
        main.GetChild("des").asTextField.text = main.GetChild("des").asTextField.text + adddes;
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
