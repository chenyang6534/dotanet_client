using cocosocket4unity;
using FairyGUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf;
using System.Threading;

public class TalentInfo {
    private GComponent main;

    private int TypeID;
    private int Level;
    private bool IsMyUnit;
    public GButton LastButton;
    public TalentInfo(int typeid,int level, bool ismyunit)
    {
        TypeID = typeid;
        Level = level;
        IsMyUnit = ismyunit;
        var clientitem = ExcelManager.Instance.GetTalentManager().GetTalentByID(typeid);
        if (clientitem == null)
        {
            return;
        }

        MsgManager.Instance.AddListener("SC_GetTalentInfo", new HandleMsg(this.SC_GetTalentInfo));
        MsgManager.Instance.AddListener("SC_ActiveTalent", new HandleMsg(this.SC_ActiveTalent));
        Protomsg.CS_GetTalentInfo msg1 = new Protomsg.CS_GetTalentInfo();
        msg1.Typeid = typeid;
        MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_GetTalentInfo", msg1);

        main = UIPackage.CreateObject("GameUI", "TalentInfo").asCom;
        GRoot.inst.AddChild(main);
        Vector2 screenPos = new Vector2(Screen.width / 2, Screen.height / 2);
        Vector2 logicScreenPos = GRoot.inst.GlobalToLocal(screenPos);
        main.xy = logicScreenPos;
        Init(clientitem);
    }
    //SC_ActiveTalent
    public bool SC_ActiveTalent(Protomsg.MsgBase d1)
    {
        Debug.Log("SC_ActiveTalent:");
        IMessage IMperson = new Protomsg.SC_ActiveTalent();
        Protomsg.SC_ActiveTalent p1 = (Protomsg.SC_ActiveTalent)IMperson.Descriptor.Parser.ParseFrom(d1.Datas);
        if (p1.Typeid != TypeID)
        {
            return false;
        }

        if(p1.IsSucc == 1)
        {
            main.GetChild("active").visible = false;
            main.GetChild("jihuozhuangtai").asTextField.text = "已激活";

            //if (LastButton != null)
            //{
            //    LastButton.asCom.alpha = 1;
            //}
            Thread t = new Thread(new ThreadStart(FreshUnitBagInfo));//心跳
            t.Start();

            
        }
        

        return true;
    }

    public void FreshUnitBagInfo()
    {
        Thread.Sleep(100);
        Protomsg.CS_GetUnitInfo msg1 = new Protomsg.CS_GetUnitInfo();
        msg1.UnitID = GameScene.Singleton.m_MyMainUnit.ID;
        MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_GetUnitInfo", msg1);

        Protomsg.CS_GetBagInfo msg2 = new Protomsg.CS_GetBagInfo();
        msg2.UnitID = GameScene.Singleton.m_MyMainUnit.ID;
        MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_GetBagInfo", msg2);

    }

    public bool SC_GetTalentInfo(Protomsg.MsgBase d1)
    {
        Debug.Log("SC_GetTalentInfo:");
        IMessage IMperson = new Protomsg.SC_GetTalentInfo();
        Protomsg.SC_GetTalentInfo p1 = (Protomsg.SC_GetTalentInfo)IMperson.Descriptor.Parser.ParseFrom(d1.Datas);
        if(p1.Typeid != TypeID)
        {
            return false;
        }
        //需要等级
        main.GetChild("needlevel").asTextField.SetVar("p1", p1.ActiveNeedLevel + "");
        main.GetChild("needlevel").asTextField.FlushVars();

        

        main.GetChild("activeitemlist").asList.RemoveChildren(0, -1, true);
        //宝箱
        if ( p1.ActiveNeedItemStr.Length > 0)
        {
            var items = p1.ActiveNeedItemStr.Split(';');
            for(var i = 0; i < items.Length; i++)
            {
                var oneitem = items[i].Split(',');
                if(oneitem.Length < 2)
                {
                    continue;
                }
                var item = int.Parse(oneitem[0]);
                var level = int.Parse(oneitem[1]);

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
                onedropitem.GetChild("level").asTextField.text = "lv."+level;
                main.GetChild("activeitemlist").asList.AddChild(onedropitem);

            }
        }
        

        return true;
    }


    //初始化
    public void Init(ExcelData.Talent clientitem)
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

        main.GetChild("active").visible = false;

        if (Level >= 1)
        {
            main.GetChild("jihuozhuangtai").asTextField.text = "已激活";
        }
        else
        {
            main.GetChild("jihuozhuangtai").asTextField.text = "未激活";
            if (IsMyUnit)
            {
                main.GetChild("active").visible = true;
                main.GetChild("active").asButton.onClick.Add(() => {
                    Protomsg.CS_ActiveTalent msg1 = new Protomsg.CS_ActiveTalent();
                    msg1.Typeid = TypeID;
                    MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_ActiveTalent", msg1);
                });
            }

        }

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
        MsgManager.Instance.RemoveListener("SC_GetTalentInfo");
        if (main != null)
        {
            main.Dispose();
        }
    }

    
}
