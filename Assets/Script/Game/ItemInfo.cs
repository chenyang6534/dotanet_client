using cocosocket4unity;
using FairyGUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf;
using System.Threading;

public class ItemInfo {
    private GComponent main;

    public BtnCallBack UseCallBack;

    private int TypeID;
    private int DBItemID;
    private int Level;
    private void newone(int typeid, int dbitemid,int level)
    {
        TypeID = typeid;
        DBItemID = dbitemid;
        Level = level;

        var clientitem = ExcelManager.Instance.GetItemManager().GetItemByID(typeid);
        if (clientitem == null)
        {
            return;
        }

        MsgManager.Instance.AddListener("SC_GetItemExtraInfo", new HandleMsg(this.SC_GetItemExtraInfo));
        Protomsg.CS_GetItemExtraInfo msg1 = new Protomsg.CS_GetItemExtraInfo();
        msg1.TypeId = typeid;
        msg1.DBItemID = dbitemid;
        msg1.Level = Level;
        MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_GetItemExtraInfo", msg1);

        main = UIPackage.CreateObject("GameUI", "ItemInfo").asCom;
        GRoot.inst.AddChild(main);
        Vector2 screenPos = new Vector2(Screen.width / 2, Screen.height / 2);
        Vector2 logicScreenPos = GRoot.inst.GlobalToLocal(screenPos);
        main.xy = logicScreenPos;
        Init(clientitem);
    }

    public ItemInfo(int typeid)
    {
        newone(typeid, -1,1);
    }
    public ItemInfo(int typeid,int dbitemid,int level)
    {

        newone(typeid, dbitemid,level);
        
    }
    public bool JiPing(bool isshow)
    {
        if (isshow == false)
        {
            AddDes("\n极品属性:\n");
        }
        return true;
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
        if(p1.EquipNeedLevel <= 0)
        {
            main.GetChild("needlevel").visible = false;
        }
        else
        {
            main.GetChild("needlevel").visible = true;
            main.GetChild("needlevel").asTextField.SetVar("p1", p1.EquipNeedLevel + "");
            main.GetChild("needlevel").asTextField.FlushVars();
        }

        var isshowjipingword = false;
        AddDes("[color=#ff2222]");
        if (Tool.IsZero(p1.AddHP) == false)
        {
            isshowjipingword = JiPing(isshowjipingword);
            AddDes("血量上限:+" + p1.AddHP + "   ");
        }
        if (Tool.IsZero(p1.AddMP) == false)
        {
            isshowjipingword = JiPing(isshowjipingword);
            AddDes("魔法上限:+" + p1.AddMP + "   ");
        }
        if (Tool.IsZero(p1.AttributePrimaryCV) == false)
        {
            isshowjipingword = JiPing(isshowjipingword);
            AddDes("主属性:+" + p1.AttributePrimaryCV + "   ");
        }
        if (Tool.IsZero(p1.AttributeStrengthCV) == false)
        {
            isshowjipingword = JiPing(isshowjipingword);
            AddDes("力量:+" + p1.AttributeStrengthCV + "   ");
        }
        if (Tool.IsZero(p1.AttributeIntelligenceCV) == false)
        {
            isshowjipingword = JiPing(isshowjipingword);
            AddDes("智力:+" + p1.AttributeIntelligenceCV + "   ");
        }
        if (Tool.IsZero(p1.AttributeAgilityCV) == false)
        {
            isshowjipingword = JiPing(isshowjipingword);
            AddDes("敏捷:+" + p1.AttributeAgilityCV + "   ");
        }
        if (Tool.IsZero(p1.AttackSpeedCV) == false)
        {
            isshowjipingword = JiPing(isshowjipingword);
            AddDes("攻击速度:+" + p1.AttackSpeedCV + "   ");
        }
        if (Tool.IsZero(p1.AttackCV) == false)
        {
            isshowjipingword = JiPing(isshowjipingword);
            AddDes("攻击力:+" + p1.AttackCV + "   ");
        }
        if (Tool.IsZero(p1.MoveSpeedCV) == false)
        {
            isshowjipingword = JiPing(isshowjipingword);
            AddDes("移动速度:+" + p1.MoveSpeedCV + "   ");
        }
        if (Tool.IsZero(p1.MagicScaleCV) == false)
        {
            isshowjipingword = JiPing(isshowjipingword);
            AddDes("技能增强:+" + (p1.MagicScaleCV * 100).ToString("0.00") + "%" + "   ");
        }
        if (Tool.IsZero(p1.MPRegainCV) == false)
        {
            isshowjipingword = JiPing(isshowjipingword);
            AddDes("魔法恢复:+" + p1.MPRegainCV + "   ");
        }
        if (Tool.IsZero(p1.PhysicalAmaorCV) == false)
        {
            isshowjipingword = JiPing(isshowjipingword);
            AddDes("护甲:+" + p1.PhysicalAmaorCV + "   ");
        }
        if (Tool.IsZero(p1.MagicAmaorCV) == false)
        {
            isshowjipingword = JiPing(isshowjipingword);
            AddDes("魔抗:+" + (p1.MagicAmaorCV * 100).ToString("0.00") + "%" + "   ");
        }
        if (Tool.IsZero(p1.StatusAmaorCV) == false)
        {
            isshowjipingword = JiPing(isshowjipingword);
            AddDes("状态抗性:+" + (p1.StatusAmaorCV * 100).ToString("0.00") + "%" + "   ");
        }
        if (Tool.IsZero(p1.DodgeCV) == false)
        {
            isshowjipingword = JiPing(isshowjipingword);
            AddDes("闪避:+" + (p1.DodgeCV * 100).ToString("0.00") + "%" + "   ");
        }
        if (Tool.IsZero(p1.HPRegainCV) == false)
        {
            isshowjipingword = JiPing(isshowjipingword);
            AddDes("生命恢复:+" + p1.HPRegainCV + "   ");
        }
        if (Tool.IsZero(p1.ManaCostCV) == false)
        {
            isshowjipingword = JiPing(isshowjipingword);
            AddDes("使用技能魔法消耗降低:" + (-p1.ManaCostCV * 100).ToString("0.00") + "%" + "   ");
        }
        if (Tool.IsZero(p1.MagicCDCV) == false)
        {
            isshowjipingword = JiPing(isshowjipingword);
            AddDes("技能CD降低:" + (p1.MagicCDCV * 100).ToString("0.00") + "%" + "   ");
        }
        if (Tool.IsZero(p1.PhysicalHurtAddHP) == false)
        {
            isshowjipingword = JiPing(isshowjipingword);
            AddDes("吸血:+" + (p1.PhysicalHurtAddHP * 100).ToString("0.00") + "%" + "   ");
        }
        if (Tool.IsZero(p1.MagicHurtAddHP) == false)
        {
            isshowjipingword = JiPing(isshowjipingword);
            AddDes("技能吸血:+" + (p1.MagicHurtAddHP * 100).ToString("0.00") + "%"+"   ");
        }
        if (Tool.IsZero(p1.SuccCount) == false)
        {
            //isshowjipingword = JiPing(isshowjipingword);
            AddDes("\n");
            AddDes("炼化次数:" + p1.SuccCount  + "   ");
        }
        
        AddDes("[/color]");


        //使用按钮
        if (UseCallBack != null && p1.ClientUseAble == 1)
        {
            main.GetChild("usebtn").visible = true;
        }
        

        main.GetChild("droplist").asList.RemoveChildren(0, -1, true);


        //宝箱
        if (p1.Exception == 1)
        {
            Debug.Log("droplist:" + p1.ExceptionParam);
            var items = p1.ExceptionParam.Split(';');
            for(var i = 0; i < items.Length; i++)
            {
                Debug.Log("droplistaa:" + items[i]);
                if (items[i].Length <= 0)
                {
                    continue;
                }
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

    public void SetUseCallBack(BtnCallBack callback)
    {
        UseCallBack = callback;
        main.GetChild("usebtn").asButton.onClick.Set(() =>
        {
            if (UseCallBack != null)
            {
                UseCallBack();
            }
        });
    }

    public delegate void BtnCallBack();
    public void SetCallBackBtn(string name, BtnCallBack callback)
    {
        main.GetChild("callbackbtn").visible = true;
        main.GetChild("callbackbtn").asButton.title = name;

        main.GetChild("callbackbtn").asButton.onClick.Set(() =>
        {
            if(callback != null)
            {
                callback();
            }
        });
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
        //等级
        main.GetChild("level").asTextField.text = "Lv."+Level;

        //名字
        main.GetChild("name").asTextField.text = clientitem.Name;

        //描述
        main.GetChild("des").asTextField.text = clientitem.Des;

        main.GetChild("callbackbtn").visible = false;
        main.GetChild("usebtn").visible = false;

        main.GetChild("needlevel").visible = false;


        

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
