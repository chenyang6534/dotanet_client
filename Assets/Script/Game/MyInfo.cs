using cocosocket4unity;
using FairyGUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf;

public class MyInfo {
    private GComponent unitinfo;
    private GComponent main;
    private UnityEntity unit;
    public MyInfo(UnityEntity unit)
    {
        this.unit = unit;
        this.InitNetData();
        main = UIPackage.CreateObject("GameUI", "MyInfo").asCom;
        unitinfo = main.GetChild("heroInfo").asCom;
        GRoot.inst.AddChild(main);
        //main.fairyBatching = true;

        Vector2 screenPos = new Vector2(Screen.width / 2, Screen.height / 2);
        Vector2 logicScreenPos = GRoot.inst.GlobalToLocal(screenPos);
        main.xy = logicScreenPos;

        //main.AddChild()
        Debug.Log("xy:" + main.xy + " screenxy:" + screenPos);

        Init();
        //FreshData();
    }



    //初始化
    public void Init()
    {
        if (unitinfo == null)
        {
            return;
        }
        //关闭按钮
        unitinfo.GetChild("close").asButton.onClick.Add(() => { Destroy(); });

        //模型
        var modeeffect = (GameObject)(GameObject.Instantiate(Resources.Load(unit.ModeType)));
        modeeffect.transform.localPosition = new Vector3(0, 0, 0);
        modeeffect.transform.localScale = new Vector3(100, 100, 100);
        Vector3 rotation = modeeffect.transform.localEulerAngles;
        rotation.x = 10; // 在这里修改坐标轴的值
        rotation.y = 180;
        rotation.z = 0;
        //将旋转的角度赋值给预制出来需要打出去的麻将
        modeeffect.transform.localEulerAngles = rotation;
        GGraph holder = unitinfo.GetChild("heromode").asGraph;
        GoWrapper wrapper = new GoWrapper(modeeffect);
        holder.SetNativeObject(wrapper);

    }

    //
    public void Destroy()
    {
        if(main != null)
        {
            main.Dispose();
        }
        MsgManager.Instance.RemoveListener("SC_UnitInfo");
        MsgManager.Instance.RemoveListener("SC_BagInfo");
    }

    //初始化网络数据
    public void InitNetData()
    {
        MsgManager.Instance.AddListener("SC_UnitInfo", new HandleMsg(this.SC_UnitInfo));
        MsgManager.Instance.AddListener("SC_BagInfo", new HandleMsg(this.SC_BagInfo));

        Protomsg.CS_GetUnitInfo msg1 = new Protomsg.CS_GetUnitInfo();
        msg1.UnitID = unit.ID;
        MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_GetUnitInfo", msg1);
    }
    public bool SC_BagInfo(Protomsg.MsgBase d1)
    {
        //IMessage IMperson = new Protomsg.SC_UnitInfo();
        //Protomsg.SC_UnitInfo p1 = (Protomsg.SC_UnitInfo)IMperson.Descriptor.Parser.ParseFrom(d1.Datas);

        //FreshUnitInfoData(p1.UnitData);

        return true;
    }

    public bool SC_UnitInfo(Protomsg.MsgBase d1)
    {
        //Debug.Log("SC_Update:");
        IMessage IMperson = new Protomsg.SC_UnitInfo();
        Protomsg.SC_UnitInfo p1 = (Protomsg.SC_UnitInfo)IMperson.Descriptor.Parser.ParseFrom(d1.Datas);

        FreshUnitInfoData(p1.UnitData);

        return true;
    }


    //刷新单位信息
    public void FreshUnitInfoData(Protomsg.UnitBoardDatas data)
    {
        if(unitinfo == null || data == null)
        {
            return;
        }
        //力量 敏捷 智力
        unitinfo.GetChild("strength").asTextField.text = ((int)data.AttributeStrength).ToString();
        unitinfo.GetChild("agility").asTextField.text = ((int)data.AttributeAgility).ToString();
        unitinfo.GetChild("intelligence").asTextField.text = ((int)data.AttributeIntelligence).ToString();
        //攻击
        unitinfo.GetChild("attack").asTextField.text = ((int)data.Attack).ToString();
        unitinfo.GetChild("attackspeed").asTextField.text = ((int)data.AttackSpeed).ToString();
        unitinfo.GetChild("attackrange").asTextField.text = (data.AttackRange).ToString("f2");
        unitinfo.GetChild("movespeed").asTextField.text = (data.MoveSpeed).ToString("f2");
        unitinfo.GetChild("magicscale").asTextField.text = ((int)(data.MagicScale*100)).ToString()+"%";
        unitinfo.GetChild("mpregain").asTextField.text = (data.MPRegain).ToString("f2");
        //防御
        unitinfo.GetChild("physicalamaor").asTextField.text = ((int)data.PhysicalAmaor).ToString();
        unitinfo.GetChild("physicalresist").asTextField.text = ((int)(data.PhysicalResist * 100)).ToString() + "%";
        unitinfo.GetChild("magicamaor").asTextField.text = ((int)(data.MagicAmaor * 100)).ToString() + "%";
        unitinfo.GetChild("stateamaor").asTextField.text = ((int)(data.StatusAmaor * 100)).ToString() + "%";
        unitinfo.GetChild("dodge").asTextField.text = ((int)(data.Dodge * 100)).ToString() + "%";
        unitinfo.GetChild("hpregain").asTextField.text = (data.HPRegain).ToString("f2");


    }
}
