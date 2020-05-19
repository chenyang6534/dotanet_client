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

        MsgManager.Instance.AddListener("SC_GetItemExtraInfo", new HandleMsg(this.SC_GetItemExtraInfo));
        Protomsg.CS_GetItemExtraInfo msg1 = new Protomsg.CS_GetItemExtraInfo();
        msg1.TypeId = typeid;
        MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_GetItemExtraInfo", msg1);

        main = UIPackage.CreateObject("GameUI", "ItemInfo").asCom;
        GRoot.inst.AddChild(main);
        Vector2 screenPos = new Vector2(Screen.width / 2, Screen.height / 2);
        Vector2 logicScreenPos = GRoot.inst.GlobalToLocal(screenPos);
        main.xy = logicScreenPos;
        Init(clientitem);
    }

    public bool SC_GetItemExtraInfo(Protomsg.MsgBase d1)
    {
        Debug.Log("SC_GetItemExtraInfo:");
        IMessage IMperson = new Protomsg.SC_GetItemExtraInfo();
        Protomsg.SC_GetItemExtraInfo p1 = (Protomsg.SC_GetItemExtraInfo)IMperson.Descriptor.Parser.ParseFrom(d1.Datas);
        if(p1.TypeId != TypeID)
        {
            return false;
        }
        //需要等级
        main.GetChild("needlevel").asTextField.SetVar("p1", p1.EquipNeedLevel+"");
        main.GetChild("needlevel").asTextField.FlushVars();

        

        main.GetChild("droplist").asList.RemoveChildren(0, -1, true);
        //宝箱
        if (p1.Exception == 1)
        {
            var items = p1.ExceptionParam.Split(';');
            for(var i = 0; i < items.Length; i++)
            {
                var oneitem = items[i].Split(':');
                if(oneitem.Length <= 0)
                {
                    continue;
                }
                var item = int.Parse(oneitem[0]);

                var clientitem = ExcelManager.Instance.GetItemManager().GetItemByID(item);
                if (clientitem == null)
                {
                    continue;
                }
                var onedropitem = UIPackage.CreateObject("GameUI", "sellable").asCom;
                onedropitem.GetChild("icon").asLoader.url = clientitem.IconPath;
                onedropitem.GetChild("icon").onClick.Add(() =>
                {
                    new ItemInfo(item);
                });
                onedropitem.GetChild("level").asTextField.text = "lv.1";
                main.GetChild("droplist").asList.AddChild(onedropitem);

            }
        }
        

        return true;
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

        ////需要等级
        //main.GetChild("needlevel").asTextField.text = "";
        
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
        MsgManager.Instance.RemoveListener("SC_GetItemExtraInfo");
        if (main != null)
        {
            main.Dispose();
        }
    }

    
}
