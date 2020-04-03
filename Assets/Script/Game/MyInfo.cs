using cocosocket4unity;
using FairyGUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf;
using System.Threading;

public class MyInfo {
    private GComponent unitinfo;
    private GComponent baginfo;
    private GComponent itemdropinfo;
    private GComponent main;
    private UnityEntity unit;

    private Protomsg.SC_BagInfo BagDataInfo;
    private Protomsg.UnitBoardDatas UnitDataInfo;

    public bool IsDestroy;
    public MyInfo(UnityEntity unit)
    {
        AudioManager.Am.Play2DSound(AudioManager.Sound_OpenUI);
        IsDestroy = false;
        this.unit = unit;
        this.InitNetData();
        main = UIPackage.CreateObject("GameUI", "MyInfo").asCom;
        unitinfo = main.GetChild("heroInfo").asCom;
        baginfo = main.GetChild("bag").asCom;
        itemdropinfo = main.GetChild("drop").asCom;
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
    public void SendDestroyItem(int srcpos)
    {
        Protomsg.CS_DestroyItem msg1 = new Protomsg.CS_DestroyItem();
        msg1.SrcPos = srcpos;
        MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_DestroyItem", msg1);
    }

    //发送交换位置
    public void SendChangePos(int srcpos,int destpos, int srctype, int desttype)
    {
        

        Protomsg.CS_ChangeItemPos msg1 = new Protomsg.CS_ChangeItemPos();
        msg1.SrcPos = srcpos;
        msg1.DestPos = destpos;
        msg1.SrcType = srctype;
        msg1.DestType = desttype;
        MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_ChangeItemPos", msg1);

        Thread t = new Thread(new ThreadStart(GetUnitInfo));//心跳
        t.Start();

        //MonoBehaviour.Invoke("GetUnitInfo", 1.0f);

        //StartCoroutine(GetUnitInfo());

        Debug.Log("-----SendChangePos----:"+srcpos+ ":" + destpos + ":" + srctype +":"+desttype);
    }
    public void  GetUnitInfo()
    {
        Thread.Sleep(100);
        Protomsg.CS_GetUnitInfo msg1 = new Protomsg.CS_GetUnitInfo();
        msg1.UnitID = unit.ID;
        MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_GetUnitInfo", msg1);

        Thread.Sleep(2000);
        if(IsDestroy == false)
        {
            Protomsg.CS_GetUnitInfo msg2 = new Protomsg.CS_GetUnitInfo();
            msg2.UnitID = unit.ID;
            MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_GetUnitInfo", msg2);
        }
        
    }

    //获取背包里的装备信息通过位置
    public Protomsg.UnitEquip GetUnitEquipByPos(int pos)
    {
        for (var j = 0; j < BagDataInfo.Equips.Count; j++)
        {
            if (BagDataInfo.Equips[j].Pos == pos)
            {
                return BagDataInfo.Equips[j];
            }
        }
        return null;

    }

    public void InitItemDrag()
    {
        if(GameScene.Singleton.m_MyMainUnit == unit)
        {
            

            //道具拖动
            for (var i = 0; i < 6; i++)
            {
                //

                //道具
                var item = unitinfo.GetChild("item" + (i + 1)).asButton;
                //item.z = 12;
                item.data = i;
                item.draggable = true;
                item.onDragStart.Add((EventContext context) =>
                {
                    //Cancel the original dragging, and start a new one with a agent.
                    context.PreventDefault();
                    InputEvent inputEvent = (context.inputEvent);
                    Stage.inst.CancelClick(inputEvent.touchId);

                    //item.SetState(OVER);

                    //Debug.Log("onDragStart");

                    //1表示装备栏 2表示背包
                    DragDropManager.inst.StartDrag(item, item.icon, item.data + ",1", (int)context.data);
                });
                //DragDropManager.inst.dragAgent.onDragEnd.Add((EventContext context) =>
                //{
                //    item.touchable = true;
                //    //Debug.Log("onDragEnd");
                //});

                item.onDrop.Add((EventContext context) =>
                {
                    string[] sArray = ((string)context.data).Split(',');
                    if (sArray.Length != 2)
                    {
                        return;
                    }

                    SendChangePos(int.Parse(sArray[0]), (int)item.data, int.Parse(sArray[1]), 1);
                });

                item.onClick.Add(() => {
                    Debug.Log("item click:"+ UnitDataInfo.Equips);
                    Debug.Log("index:" + (int)item.data);
                    var index = (int)item.data;
                    var typeid = -1;
                    for (var j = 0; j < UnitDataInfo.Equips.Count; j++)
                    {
                        if (UnitDataInfo.Equips[j].Pos == index)
                        {
                            typeid = UnitDataInfo.Equips[j].TypdID;
                        }
                    }
                    if (typeid != -1)
                    {
                        new ItemInfo(typeid);
                    }


                });
            }
            //baginfo
            //删除道具
            var destroyitem = baginfo.GetChild("destroy").asButton;
            destroyitem.onDrop.Add((EventContext context) =>
            {
                string[] sArray = ((string)context.data).Split(',');
                if (sArray.Length != 2)
                {
                    return;
                }
                //背包
                if (int.Parse(sArray[1]) != 2)
                {
                    return;
                }

                //获取位置上的道具
                var bagequip = GetUnitEquipByPos(int.Parse(sArray[0]));
                if(bagequip != null)
                {
                    var clientitem = ExcelManager.Instance.GetItemManager().GetItemByID(bagequip.TypdID);
                    if (clientitem == null)
                    {
                        return;
                    }
                    Tool.NoticeWindonw("你确定要删除道具(" + clientitem.Name + ")吗?", () =>
                    {
                        SendDestroyItem(int.Parse(sArray[0]));
                    });
                }
                //for (var j = 0; j < BagDataInfo.Equips.Count; j++)
                //{
                //    if (BagDataInfo.Equips[j].Pos == int.Parse(sArray[0]))
                //    {
                        
                        
                //    }
                //}

                
                
            });
            for (var i = 0; i < 25; i++)
            {
                //道具
                var item = baginfo.GetChild("bagitem" + (i + 1)).asButton;
                item.data = i;
                item.draggable = true;
                item.onDragStart.Add((EventContext context) =>
                {
                    //Cancel the original dragging, and start a new one with a agent.
                    context.PreventDefault();
                    InputEvent inputEvent = (context.inputEvent);
                    Stage.inst.CancelClick(inputEvent.touchId);
                    DragDropManager.inst.StartDrag(item, item.icon, item.data + ",2", (int)context.data);
                });

                item.onDrop.Add((EventContext context) =>
                {
                    string[] sArray = ((string)context.data).Split(',');
                    if (sArray.Length != 2)
                    {
                        return;
                    }
                    //判断是否是合成
                    if(int.Parse(sArray[1]) == 2)//背包里
                    {
                        var bagequip = GetUnitEquipByPos(int.Parse(sArray[0]));
                        var bagequip1 = GetUnitEquipByPos((int)item.data);
                        if (bagequip != null && bagequip1 != null && bagequip1.TypdID == bagequip.TypdID && bagequip1.Level == bagequip.Level)
                        {
                            //道具相同且等级相同则合成
                            var clientitem = ExcelManager.Instance.GetItemManager().GetItemByID(bagequip.TypdID);
                            if (clientitem == null)
                            {
                                return;
                            }
                            Tool.NoticeWindonw("你确定要合成道具(" + clientitem.Name + ")到更高等级吗?", () =>
                            {
                                //SendDestroyItem(int.Parse(sArray[0]));
                                SendChangePos(int.Parse(sArray[0]), (int)item.data, int.Parse(sArray[1]), 2);
                            });
                            return;
                        }
                    }
                    SendChangePos(int.Parse(sArray[0]), (int)item.data, int.Parse(sArray[1]), 2);
                });

                item.onClick.Add(() => {
                    var index = (int)item.data;
                    var typeid = -1;
                    for (var j = 0; j < BagDataInfo.Equips.Count; j++)
                    {
                        if (BagDataInfo.Equips[j].Pos == index)
                        {
                            typeid = BagDataInfo.Equips[j].TypdID;
                        }
                    }
                    if (typeid != -1)
                    {
                        new ItemInfo(typeid);
                    }


                });
            }
        }
        else
        {
            baginfo.visible = false;
            //道具拖动
            for (var i = 0; i < 6; i++)
            {
                //道具
                var item = unitinfo.GetChild("item" + (i + 1)).asButton;
                item.data = i;
                item.onClick.Add(() => {
                    var index = (int)item.data;
                    var typeid = -1;
                    for (var j = 0; j < UnitDataInfo.Equips.Count; j++)
                    {
                        if (UnitDataInfo.Equips[j].Pos == index)
                        {
                            typeid = UnitDataInfo.Equips[j].TypdID;
                        }
                    }
                    if (typeid != -1)
                    {
                        new ItemInfo(typeid);
                    }


                });
            }
         }
    }

    public void InitDropItem()
    {
        //阵营(1:玩家 2:NPC)
        if (unit.Camp != 1)
        {
            itemdropinfo.visible = true;
        }
        else
        {
            itemdropinfo.visible = false;
        }
    }
    
    //初始化技能信息
    public void InitSkillInfo()
    {
        if (unit == null || unit.SkillDatas == null)
        {
            //unitinfo.GetChild("skill_list").asList.visible = false;
            return;
        }
        var skilllist = unitinfo.GetChild("skill_list").asList;
        if(skilllist == null)
        {
            return;
        }
        skilllist.RemoveChildren();

        //排序
        System.Array.Sort(unit.SkillDatas, (s1, s2) => s1.Index.CompareTo(s2.Index));
        foreach (var item in unit.SkillDatas)
        {
            var clientitem = ExcelManager.Instance.GetSkillManager().GetSkillByID(item.TypeID);
            if (clientitem != null && item.Visible == 1)
            {
                var onedropitem = UIPackage.CreateObject("GameUI", "HeroInfo_Skill").asButton;
                onedropitem.icon = clientitem.IconPath;

                onedropitem.GetChild("level").asTextField.text = "Lv."+item.Level;

                if(item.Level <= 0)
                {
                    onedropitem.asCom.alpha = 0.2f;
                }
                else
                {
                    onedropitem.asCom.alpha = 1;
                }
                
                onedropitem.onClick.Add(() => {
                    //Debug.Log("onClick");
                    if (clientitem.TypeID != -1)
                    {
                        new SkillInfo(clientitem.TypeID);
                    }
                });
                skilllist.AddChild(onedropitem);
            }
        }
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


        InitItemDrag();
        InitDropItem();
        InitSkillInfo();


        //模型
        var modeeffect = (GameObject)(GameObject.Instantiate(Resources.Load(unit.ModeType)));
        modeeffect.transform.localPosition = new Vector3(0, 0, 0);
        var box = modeeffect.GetComponent<BoxCollider>();
        modeeffect.transform.localScale = new Vector3(100, 100, 100);
        if (box != null)
        {
            var scale = box.size.y / 1.2f;
            modeeffect.transform.localScale = new Vector3(100/ scale, 100 / scale, 100 / scale);
        }
        
        
        Vector3 rotation = modeeffect.transform.localEulerAngles;
        rotation.x = 10; // 在这里修改坐标轴的值
        rotation.y = 180;
        rotation.z = 0;
        //将旋转的角度赋值给预制出来需要打出去的麻将
        modeeffect.transform.localEulerAngles = rotation;
        GGraph holder = unitinfo.GetChild("heromode").asGraph;
        GoWrapper wrapper = new GoWrapper(modeeffect);
        holder.SetNativeObject(wrapper);
        holder.z = 10;

    }

    //
    public void Destroy()
    {
        if(main != null)
        {
            main.Dispose();
            AudioManager.Am.Play2DSound(AudioManager.Sound_CloseUI);
        }
        MsgManager.Instance.RemoveListener("SC_UnitInfo");
        MsgManager.Instance.RemoveListener("SC_BagInfo");
        IsDestroy = true;
    }

    //初始化网络数据
    public void InitNetData()
    {
        MsgManager.Instance.AddListener("SC_UnitInfo", new HandleMsg(this.SC_UnitInfo));
        MsgManager.Instance.AddListener("SC_BagInfo", new HandleMsg(this.SC_BagInfo));

        Protomsg.CS_GetUnitInfo msg1 = new Protomsg.CS_GetUnitInfo();
        msg1.UnitID = unit.ID;
        MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_GetUnitInfo", msg1);

        if (GameScene.Singleton.m_MyMainUnit == unit)
        {
            Protomsg.CS_GetBagInfo msg2 = new Protomsg.CS_GetBagInfo();
            msg2.UnitID = unit.ID;
            MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_GetBagInfo", msg2);
        }
            
    }
    public bool SC_BagInfo(Protomsg.MsgBase d1)
    {
        IMessage IMperson = new Protomsg.SC_BagInfo();
        Protomsg.SC_BagInfo p1 = (Protomsg.SC_BagInfo)IMperson.Descriptor.Parser.ParseFrom(d1.Datas);

        FreshBagInfoData(p1);

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
    public void FreshBagInfoData(Protomsg.SC_BagInfo data)
    {
        BagDataInfo = data;
        if (baginfo == null || data == null)
        {
            return;
        }
        for (var i = 0; i < 25; i++)
        {
            //道具
            baginfo.GetChild("bagitem" + (i + 1)).asButton.icon = "";
            baginfo.GetChild("bagitem" + (i + 1)).asButton.GetChild("level").asTextField.text = "";
        }
        for (var i = 0; i < data.Equips.Count; i++)
        {
            
            var itemdata = data.Equips[i];
            if (baginfo.GetChild("bagitem" + (itemdata.Pos + 1)) == null)
            {
                continue;
            }

            var clientitem = ExcelManager.Instance.GetItemManager().GetItemByID(itemdata.TypdID);
            if (clientitem == null)
            {
                //道具"n71"
                //unitinfo.GetChild("item" + (itemdata.Pos + 1)).asLoader.url = "";// "ui://GameUI/黯灭";
                baginfo.GetChild("bagitem" + (itemdata.Pos + 1)).asButton.icon = "";
                baginfo.GetChild("bagitem" + (itemdata.Pos + 1)).asButton.GetChild("level").asTextField.text = "";
                continue;
            }
            //道具
            //unitinfo.GetChild("item" + (itemdata.Pos + 1)).asLoader.url = clientitem.IconPath;// "ui://GameUI/黯灭";
            baginfo.GetChild("bagitem" + (itemdata.Pos + 1)).asButton.icon = clientitem.IconPath;
            baginfo.GetChild("bagitem" + (itemdata.Pos + 1)).asButton.GetChild("level").asTextField.text = "Lv." + itemdata.Level + "";
        }
        

    }

    //刷新掉落道具
    public void FreshDropItem(string dropitem)
    {
        Debug.Log("FreshDropItem:" + dropitem);
        var itemlist = itemdropinfo.GetChild("list").asList;
        itemlist.RemoveChildren();
        if(dropitem.Length > 0)
        {
            string[] sArray = dropitem.Split(';');
            foreach (string i in sArray)
            {
                string[] onedrop = i.Split(',');
                if(onedrop.Length == 2)
                {
                    Debug.Log("--FreshDropItem--" + onedrop[0]+"   "+ onedrop[1]);
                    //var typeid = 
                    var clientitem = ExcelManager.Instance.GetItemManager().GetItemByID(int.Parse(onedrop[0]));
                    if (clientitem != null)
                    {
                        var onedropitem = UIPackage.CreateObject("GameUI", "dropItemButton1").asButton;
                        onedropitem.icon = clientitem.IconPath;
                        float f1 = System.Convert.ToSingle(onedrop[1]);
                        onedropitem.GetChild("droptxt").asTextField.text = (f1 * 100).ToString("0.00")+"%";


                        onedropitem.onClick.Add(() => {
                            //Debug.Log("onClick");
                            if (clientitem.TypeID != -1)
                            {
                                new ItemInfo(clientitem.TypeID);
                            }
                        });


                        itemlist.AddChild(onedropitem);

                        Debug.Log("--FreshDropItem:"+ clientitem.IconPath);
                    }
                    
                }
            }
        }
    }

    //刷新单位信息
    public void FreshUnitInfoData(Protomsg.UnitBoardDatas data)
    {
        UnitDataInfo = data;
        if (unitinfo == null || data == null)
        {
            return;
        }
        for (var i = 0; i < 6; i++)
        {
            //道具
            unitinfo.GetChild("item" + (i + 1)).asButton.icon = "";
        }
        for (var i = 0; i < data.Equips.Count; i++)
        {
            var itemdata = data.Equips[i];
            
            var clientitem = ExcelManager.Instance.GetItemManager().GetItemByID(itemdata.TypdID);
            if(clientitem == null)
            {
                //道具"n71"
                //unitinfo.GetChild("item" + (itemdata.Pos + 1)).asLoader.url = "";// "ui://GameUI/黯灭";
                unitinfo.GetChild("item" + (itemdata.Pos + 1)).asButton.icon = "";
                unitinfo.GetChild("item" + (itemdata.Pos + 1)).asButton.GetChild("level").asTextField.text = "";
                continue;
            }
            //道具
            //unitinfo.GetChild("item" + (itemdata.Pos + 1)).asLoader.url = clientitem.IconPath;// "ui://GameUI/黯灭";
            unitinfo.GetChild("item" + (itemdata.Pos + 1)).asButton.icon = clientitem.IconPath;
            unitinfo.GetChild("item" + (itemdata.Pos + 1)).asButton.GetChild("level").asTextField.text = "Lv." + itemdata.Level + "";
        }


        //等级
        unitinfo.GetChild("level").asTextField.text = ((int)unit.Level).ToString();
        //经验值
        unitinfo.GetChild("experience").asTextField.text = ((int)unit.Experience).ToString()+"/"+ ((int)unit.MaxExperience).ToString();

        //今日剩余经验值
        unitinfo.GetChild("remainexperience").asTextField.text = ((int)data.RemainExperience).ToString();
        if(unit.UnitType != 1)
        {
            unitinfo.GetChild("remainexperience").asTextField.visible = false;
            unitinfo.GetChild("remainexlable").asTextField.visible = false;

            unitinfo.GetChild("exlable").asTextField.visible = false;
            unitinfo.GetChild("experience").asTextField.visible = false;
        }

        //力量 敏捷 智力
        unitinfo.GetChild("strength").asTextField.text = ((int)data.AttributeStrength).ToString();
        unitinfo.GetChild("agility").asTextField.text = ((int)data.AttributeAgility).ToString();
        unitinfo.GetChild("intelligence").asTextField.text = ((int)data.AttributeIntelligence).ToString();
        //主属性(1:力量 2:敏捷 3:智力)
        if (data.AttributePrimary == 1)
        {
            unitinfo.GetChild("strength").asTextField.color = new Color(0.1f, 0.8f, 0.4f);
            unitinfo.GetChild("agility").asTextField.color = new Color(1, 1, 1);
            unitinfo.GetChild("intelligence").asTextField.color = new Color(1, 1, 1);
        }
        else if(data.AttributePrimary == 2)
        {
            unitinfo.GetChild("strength").asTextField.color = new Color(1, 1, 1);
            unitinfo.GetChild("agility").asTextField.color = new Color(0.1f, 0.8f, 0.4f);
            unitinfo.GetChild("intelligence").asTextField.color = new Color(1, 1, 1);
            
        }
        else if(data.AttributePrimary == 3)
        {
            unitinfo.GetChild("strength").asTextField.color = new Color(1, 1, 1);
            unitinfo.GetChild("agility").asTextField.color = new Color(1, 1, 1);
            unitinfo.GetChild("intelligence").asTextField.color = new Color(0.1f, 0.8f, 0.4f);
        }
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


        FreshDropItem(data.DropItems);

    }
}
