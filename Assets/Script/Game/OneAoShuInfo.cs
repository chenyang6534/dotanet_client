using cocosocket4unity;
using FairyGUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf;
using System.Threading;

public class OneAoShuInfo
{
    private GComponent main;

    private int TypeID;
    private int State;
    private bool IsMyUnit;
    public GButton LastButton;
    public OneAoShuInfo(int typeid,int state, bool ismyunit)
    {
        TypeID = typeid;
        State = state;
        IsMyUnit = ismyunit;
        var clientitem = ExcelManager.Instance.GetAoShuManager().GetAoShuByID(typeid);
        if (clientitem == null)
        {
            return;
        }

        MsgManager.Instance.AddListener("SC_GetOneAoShuInfo", new HandleMsg(this.SC_GetOneAoShuInfo));
        //MsgManager.Instance.AddListener("SC_ActiveTalent", new HandleMsg(this.SC_ActiveTalent));
        Protomsg.CS_GetOneAoShuInfo msg1 = new Protomsg.CS_GetOneAoShuInfo();
        msg1.ID = typeid;
        MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_GetOneAoShuInfo", msg1);

        main = UIPackage.CreateObject("GameUI", "OneAoShuInfo").asCom;
        GRoot.inst.AddChild(main);
        Vector2 screenPos = new Vector2(Screen.width / 2, Screen.height / 2);
        Vector2 logicScreenPos = GRoot.inst.GlobalToLocal(screenPos);
        main.xy = logicScreenPos;
        Init(clientitem);
    }
    

   

    public bool SC_GetOneAoShuInfo(Protomsg.MsgBase d1)
    {
        Debug.Log("SC_GetOneAoShuInfo:");
        IMessage IMperson = new Protomsg.SC_GetOneAoShuInfo();
        Protomsg.SC_GetOneAoShuInfo p1 = (Protomsg.SC_GetOneAoShuInfo)IMperson.Descriptor.Parser.ParseFrom(d1.Datas);
        if(p1.ID != TypeID)
        {
            return false;
        }
        //需要等级
        main.GetChild("needlevel").asTextField.SetVar("p1", p1.ActiveNeedAoShuFragment + "");
        main.GetChild("needlevel").asTextField.FlushVars();
        
        return true;
    }


    //初始化
    public void Init(ExcelData.AoShu clientitem)
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

        // 0:未解锁 1:已经解锁未激活 2:已经激活
        if (State == 0)
        {
            main.GetChild("jihuozhuangtai").asTextField.text = "未解锁";
            main.GetChild("jihuozhuangtai").asTextField.color = new Color(0.4f, 1, 0.4f);
        }
        else if (State == 1)
        {
            main.GetChild("jihuozhuangtai").asTextField.text = "已解锁";
            main.GetChild("jihuozhuangtai").asTextField.color = new Color(0.4f, 1, 0.4f);
            if (IsMyUnit)
            {
                main.GetChild("active").visible = true;
                main.GetChild("active").asButton.onClick.Add(() => {
                    Protomsg.CS_ActiveAoShu msg1 = new Protomsg.CS_ActiveAoShu();
                    msg1.ID = TypeID;
                    MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_ActiveAoShu", msg1);
                    Destroy();
                });
            }
        }
        else if (State == 2)
        {
            main.GetChild("jihuozhuangtai").asTextField.text = "已激活";
            main.GetChild("jihuozhuangtai").asTextField.color = new Color(0.4f, 1, 0.4f);
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
        MsgManager.Instance.RemoveListener("SC_GetOneAoShuInfo");
        if (main != null)
        {
            main.Dispose();
        }
    }

    
}
