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
    private GComponent storageinfo;
    private GComponent lianhuainfo;
    private GComponent main;
    private UnityEntity unit;

    private Protomsg.SC_BagInfo BagDataInfo;
    private Protomsg.UnitBoardDatas UnitDataInfo;
    private Protomsg.SC_GetLianHuaInfo LianHuaMsgInfo;

    public bool IsDestroy;
    public MyInfo(UnityEntity unit)
    {
        UMengManager.Instanse.Event_click_myinfo();
        AudioManager.Am.Play2DSound(AudioManager.Sound_OpenUI);
        IsDestroy = false;
        this.unit = unit;
        this.InitNetData();
        main = UIPackage.CreateObject("GameUI", "MyInfo").asCom;
        unitinfo = main.GetChild("heroInfo").asCom;
        baginfo = main.GetChild("bag").asCom;
        itemdropinfo = main.GetChild("drop").asCom;
        storageinfo = main.GetChild("storage").asCom;
        lianhuainfo = main.GetChild("lianhua").asCom;
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
    public void SendHuiShouItem(int srcpos)
    {
        Protomsg.CS_SystemHuiShouItem msg1 = new Protomsg.CS_SystemHuiShouItem();
        msg1.SrcPos = srcpos;
        MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_SystemHuiShouItem", msg1);

    }

    

    //发送交换位置
    public void SendChangePos(int srcpos,int destpos, int srctype, int desttype,bool IsWatchVedio,bool IsMoneyReplaceVedio)
    {
        

        Protomsg.CS_ChangeItemPos msg1 = new Protomsg.CS_ChangeItemPos();
        msg1.SrcPos = srcpos;
        msg1.DestPos = destpos;
        msg1.SrcType = srctype;
        msg1.DestType = desttype;
        msg1.IsWatchVedio = IsWatchVedio;
        msg1.IsMoneyReplaceVedio = IsMoneyReplaceVedio;
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
                    DragDropManager.inst.dragAgent.autoSize = false;
                    DragDropManager.inst.dragAgent.SetPivot(0.5f, 1, true);
                    DragDropManager.inst.dragAgent.fill = FillType.Scale;
                    DragDropManager.inst.dragAgent.SetSize(item.width, item.height);
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

                    SendChangePos(int.Parse(sArray[0]), (int)item.data, int.Parse(sArray[1]), 1, false, false);
                });

                item.onClick.Add(() => {
                    Debug.Log("item click:"+ UnitDataInfo.Equips);
                    Debug.Log("index:" + (int)item.data);
                    var index = (int)item.data;
                    var typeid = -1;
                    var dbitemid = -1;
                    var level = 1;
                    for (var j = 0; j < UnitDataInfo.Equips.Count; j++)
                    {
                        if (UnitDataInfo.Equips[j].Pos == index)
                        {
                            typeid = UnitDataInfo.Equips[j].TypdID;
                            dbitemid = UnitDataInfo.Equips[j].ItemDBID;
                            level = UnitDataInfo.Equips[j].Level;
                        }
                    }
                    if (typeid != -1)
                    {
                        new ItemInfo(typeid, dbitemid, level);
                    }


                });
            }
            //baginfo
            //删除道具
            var destroyitem = baginfo.GetChild("destroy").asButton;
            destroyitem.onClick.Set(() => {
                Tool.NoticeWords("需要把道具拖到此处才能回收道具!", null);
            });
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
                    
                    Tool.NoticeWindonw("你确定要以[color=#FFFF00]" + bagequip.Price * Mathf.Pow(2, bagequip.Level - 1) + Tool.GetPriceTypeName(bagequip.PriceType) + "[/color]的价格卖出道具(" + clientitem.Name + ")吗?", () =>
                    {
                        SendHuiShouItem(int.Parse(sArray[0]));
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
                    DragDropManager.inst.dragAgent.autoSize = false;
                    DragDropManager.inst.dragAgent.SetPivot(0.5f, 1, true);
                    DragDropManager.inst.dragAgent.fill = FillType.Scale;
                    DragDropManager.inst.dragAgent.SetSize(item.width, item.height);
                    DragDropManager.inst.StartDrag(item, item.icon, item.data + ",2", (int)context.data);
                    
                    //DragDropManager.inst.dragAgent.width = 10;
                    //DragDropManager.inst.dragAgent.height = 10;
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
                        if(int.Parse(sArray[0]) == (int)item.data)//位置相同 不处理
                        {
                            return;
                        }

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
                            if(bagequip.Level >= GameScene.Singleton.SvConf.UpgradeItemNeedWatchVedioLevel)
                            {
                                Tool.NoticeWindonw("你确定要观看视频后合成道具(" + clientitem.Name + ")到更高等级吗?[color=#ff2222](极品属性会叠加保留)[/color]", () =>
                                {
                                    //SendDestroyItem(int.Parse(sArray[0]));
                                    TTadMgr.Instanse.ShowVideo((succ, IsMoneyReplaceVedio, useitem) =>
                                    {
                                        if (succ == true)
                                        {
                                            Debug.Log("观看视频成功");
                                            //领取奖励
                                            SendChangePos(int.Parse(sArray[0]), (int)item.data, int.Parse(sArray[1]), 2,true,IsMoneyReplaceVedio);
                                            //统计相关
                                            if (useitem == false)
                                            {
                                                if (IsMoneyReplaceVedio == false)
                                                {
                                                    UMengManager.Instanse.Event_watch_vedio("合成装备");
                                                }
                                                else
                                                {
                                                    UMengManager.Instanse.Event_watch_vedio_moneyreplace("合成装备");
                                                }
                                            }
                                            
                                            
                                        }
                                        else
                                        {
                                            Debug.Log("观看视频失败");
                                        }
                                    });


                                });
                            }
                            else
                            {
                                Tool.NoticeWindonw("你确定要合成道具(" + clientitem.Name + ")到更高等级吗?[color=#ff2222](极品属性会叠加保留)[/color]", () =>
                                {
                                    //领取奖励
                                    SendChangePos(int.Parse(sArray[0]), (int)item.data, int.Parse(sArray[1]), 2,false,false);

                                    //UMengManager.Instanse.Event_watch_vedio("合成装备");
                                });
                            }
                            
                            return;
                        }
                    }
                    SendChangePos(int.Parse(sArray[0]), (int)item.data, int.Parse(sArray[1]), 2, false, false);
                });

                item.onClick.Add(() => {
                    var index = (int)item.data;
                    var typeid = -1;
                    var dbitemid = -1;
                    var level = 1;
                    for (var j = 0; j < BagDataInfo.Equips.Count; j++)
                    {
                        if (BagDataInfo.Equips[j].Pos == index)
                        {
                            typeid = BagDataInfo.Equips[j].TypdID;
                            dbitemid = BagDataInfo.Equips[j].ItemDBID;
                            level = BagDataInfo.Equips[j].Level;
                        }
                    }
                    if (typeid != -1)
                    {
                        ItemInfo a = new ItemInfo(typeid, dbitemid,level);
                        a.SetCallBackBtn("存放",()=> {
                            Protomsg.CS_Save2Storage msg1 = new Protomsg.CS_Save2Storage();
                            msg1.Pos = index;
                            MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_Save2Storage", msg1);
                            a.Destroy();
                        });
                        a.SetUseCallBack(() => {
                            Protomsg.CS_UseItem msg1 = new Protomsg.CS_UseItem();
                            msg1.SrcPos = index;
                            MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_UseItem", msg1);
                            a.Destroy();
                        });
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
                    var dbitemid = -1;
                    var level = 1;
                    for (var j = 0; j < UnitDataInfo.Equips.Count; j++)
                    {
                        if (UnitDataInfo.Equips[j].Pos == index)
                        {
                            typeid = UnitDataInfo.Equips[j].TypdID;
                            dbitemid = UnitDataInfo.Equips[j].ItemDBID;
                            level = UnitDataInfo.Equips[j].Level;
                        }
                    }
                    if (typeid != -1)
                    {
                        new ItemInfo(typeid, dbitemid,level);
                    }


                });
            }
         }

        var storagebtn = baginfo.GetChild("storagebtn").asButton;
        storagebtn.onClick.Add(() => {
            storageinfo.visible = !storageinfo.visible;
            if(storageinfo.visible == true)
            {
                Protomsg.CS_OpenStorage msg1 = new Protomsg.CS_OpenStorage();
                MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_OpenStorage", msg1);
            }

        });

        var lianhuabtn = baginfo.GetChild("lianhuabtn").asButton;
        lianhuabtn.onClick.Add(() => {
            lianhuainfo.visible = !lianhuainfo.visible;
            if (lianhuainfo.visible == true)
            {
                Protomsg.CS_GetLianHuaInfo msg1 = new Protomsg.CS_GetLianHuaInfo();
                MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_GetLianHuaInfo", msg1);
            }

        });

        
    }

    public void InitStorageInfo()
    {
        storageinfo.visible = false;
        storageinfo.GetChild("close").asButton.onClick.Set(() => {
            storageinfo.visible = false;
        });
    }
    public void InitLianHuaInfo()
    {
        lianhuainfo.visible = false;
        lianhuainfo.GetChild("close").asButton.onClick.Set(() => {
            lianhuainfo.visible = false;
            LianHuaMsgInfo = null;
        });

        lianhuainfo.GetChild("ok").asButton.onClick.Set(() => {

            TTadMgr.Instanse.ShowVideo((succ, IsMoneyReplaceVedio, useitem) =>
            {
                if (succ == true)
                {
                    Debug.Log("观看视频成功");
                    //领取奖励
                    // 1炼化装备 2炼化材料 3辅助装备
                    Protomsg.CS_StartLianHua msg1 = new Protomsg.CS_StartLianHua();
                    msg1.IsMoneyReplaceVedio = IsMoneyReplaceVedio;
                    MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_StartLianHua", msg1);
                    //统计相关
                    if (useitem == false)
                    {
                        if (IsMoneyReplaceVedio == false)
                        {
                            UMengManager.Instanse.Event_watch_vedio("炼化装备");
                        }
                        else
                        {
                            UMengManager.Instanse.Event_watch_vedio_moneyreplace("炼化装备");
                        }
                    }
                    

                }
                else
                {
                    Debug.Log("观看视频失败");
                }
            });

            

        });

        lianhuainfo.GetChild("lianhua1").asCom.onDrop.Add((EventContext context) =>
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
            if (bagequip != null)
            {
                var clientitem = ExcelManager.Instance.GetItemManager().GetItemByID(bagequip.TypdID);
                if (clientitem == null)
                {
                    return;
                }
                // 1炼化装备 2炼化材料 3辅助装备
                Protomsg.CS_DragItem msg1 = new Protomsg.CS_DragItem();
                msg1.SrcDBItemID = bagequip.ItemDBID;
                msg1.Dest = 1;
                MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_DragItem", msg1);

            }

        });

        lianhuainfo.GetChild("lianhua2").asCom.onDrop.Add((EventContext context) =>
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
            if (bagequip != null)
            {
                var clientitem = ExcelManager.Instance.GetItemManager().GetItemByID(bagequip.TypdID);
                if (clientitem == null)
                {
                    return;
                }
                // 1炼化装备 2炼化材料 3辅助装备
                Protomsg.CS_DragItem msg1 = new Protomsg.CS_DragItem();
                msg1.SrcDBItemID = bagequip.ItemDBID;
                msg1.Dest = 2;
                MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_DragItem", msg1);

            }

        });

        lianhuainfo.GetChild("droplist").asList.onDrop.Add((EventContext context) =>
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
            if (bagequip != null)
            {
                var clientitem = ExcelManager.Instance.GetItemManager().GetItemByID(bagequip.TypdID);
                if (clientitem == null)
                {
                    return;
                }
                // 1炼化装备 2炼化材料 3辅助装备
                Protomsg.CS_DragItem msg1 = new Protomsg.CS_DragItem();
                msg1.SrcDBItemID = bagequip.ItemDBID;
                msg1.Dest = 3;
                MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_DragItem", msg1);

            }

        });
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
            Debug.Log("SkillDatas:" + item.TypeID);
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

    //初始化技能信息
    public void FreshTalentInfo(string talent,bool ismyunit)
    {

        var skilllist = unitinfo.GetChild("talent_list").asList;
        if (skilllist == null)
        {
            return;
        }
        skilllist.RemoveChildren();

        var skillstrarr = talent.Split(';');
        var count = 0;
        foreach (var item in skillstrarr)
        {
            var itemstrarr = item.Split(',');
            if (itemstrarr.Length < 2)
            {
                Debug.Log("itemstrarr.Length < 2 :" + item);
                continue;
            }
            count++;
        }



        GComponent[] allplayer = new GComponent[count];
        var index = 0;
        //排序
        foreach (var item in skillstrarr)
        {
            var itemstrarr = item.Split(',');
            if (itemstrarr.Length < 2)
            {
                Debug.Log("itemstrarr.Length < 2 :" + item);
                continue;
            }
            var typeid = int.Parse(itemstrarr[0]);
            var level = int.Parse(itemstrarr[1]);

            var clientitem = ExcelManager.Instance.GetTalentManager().GetTalentByID(typeid);
            if (clientitem != null)
            {
                var onedropitemcom = UIPackage.CreateObject("GameUI", "CircleIcon_talent").asCom;
                var onedropitem = onedropitemcom.GetChild("btn").asButton;
                //onedropitem.SetSize(80, 80);
                onedropitem.icon = clientitem.IconPath;

                //onedropitem.GetChild("level").asTextField.text = "Lv." + level;

                if (level <= 0)
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
                        var talent1 = new TalentInfo(clientitem.TypeID,level, ismyunit);
                    }
                });
                onedropitemcom.data = typeid;
                allplayer[index] = onedropitemcom;
                index++;
            }
            
        }
        System.Array.Sort(allplayer, (a, b) => {
            if ((int)(a.data) > (int)(b.data))
            {
                return 1;
            }
            else if ((int)(a.data) < (int)(b.data))
            {
                return -1;
            }
            return 0;
        });

        foreach (var item in allplayer)
        {
            skilllist.AddChild(item);
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

        unitinfo.GetChild("aoshu").asButton.onClick.Add(() => { new AoShuInfo(unit.CharacterID); });
        if(unit.CharacterID <= 0)
        {
            unitinfo.GetChild("aoshu").visible = false;
        }

        var item = ExcelManager.Instance.GetSceneManager().GetSceneByID(GameScene.Singleton.m_SceneID);
        if (item != null)
        {
            var scenename = item.Name;
            var postext = "(" + (int)unit.X + "," + (int)unit.Y + ")";
            var noticewords = ExcelManager.Instance.GetNoticeWordsManager().GetNoticeWordsByID(38);
            Dictionary<string, string> p = new Dictionary<string, string>();
            p["p1"] = item.Name;
            p["p2"] = ("" + (int)unit.X);
            p["p3"] = ("" + (int)unit.Y);
            p["p4"] = (unit.Name);
            if (noticewords != null)
            {
                //定位信息
                unitinfo.GetChild("localpos").asButton.onClick.Add(() => {
                    ChatUI.SOpenChatBoxWithMsg("quanfu", "", 0,Tool.ParseTemplate(noticewords.Words,p));
                });
            }
            
        }
        


        InitItemDrag();
        InitDropItem();
        InitSkillInfo();
        InitStorageInfo();
        InitLianHuaInfo();


        //模型
        var modeeffect = (GameObject)(GameObject.Instantiate(Resources.Load(unit.ModeType)));
        modeeffect.transform.localPosition = new Vector3(0, 0, 100);
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
        MsgManager.Instance.RemoveListener("SC_OpenStorage");

        MsgManager.Instance.RemoveListener("SC_GetLianHuaInfo");
        IsDestroy = true;
    }

    //初始化网络数据
    public void InitNetData()
    {
        MsgManager.Instance.AddListener("SC_UnitInfo", new HandleMsg(this.SC_UnitInfo));
        MsgManager.Instance.AddListener("SC_BagInfo", new HandleMsg(this.SC_BagInfo));
        MsgManager.Instance.AddListener("SC_OpenStorage", new HandleMsg(this.SC_OpenStorage));
        MsgManager.Instance.AddListener("SC_GetLianHuaInfo", new HandleMsg(this.SC_GetLianHuaInfo));
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

    public bool FindDBItemFromLianHuaInfo(Protomsg.SC_GetLianHuaInfo data,int dbid)
    {
        if(data == null)
        {
            return false;
        }
        if(data.LianHuaItem != null && data.LianHuaItem.DBItemID == dbid)
        {
            return true;
        }
        if (data.CaiLiaoItem != null && data.CaiLiaoItem.DBItemID == dbid)
        {
            return true;
        }
        foreach(var item in data.FuZhuItem)
        {
            if(item.DBItemID == dbid)
            {
                return true;
            }
        }

        return false;
    }

    //隐藏背包里的被拖入炼化道具
    public void HideBagItemWithLianHua()
    {
        for (var i = 0; i < BagDataInfo.Equips.Count; i++)
        {
            var itemdata = BagDataInfo.Equips[i];
            if (baginfo.GetChild("bagitem" + (itemdata.Pos + 1)) == null)
            {
                continue;
            }
            baginfo.GetChild("bagitem" + (itemdata.Pos + 1)).alpha = 1.0f;
            if (FindDBItemFromLianHuaInfo(LianHuaMsgInfo, itemdata.ItemDBID))
            {
                baginfo.GetChild("bagitem" + (itemdata.Pos + 1)).alpha = 0.2f;
            }

        }
    }

    //SC_GetLianHuaInfo
    public bool SC_GetLianHuaInfo(Protomsg.MsgBase d1)
    {
        Debug.Log("SC_GetLianHuaInfo--------------");
        IMessage IMperson = new Protomsg.SC_GetLianHuaInfo();
        Protomsg.SC_GetLianHuaInfo p1 = (Protomsg.SC_GetLianHuaInfo)IMperson.Descriptor.Parser.ParseFrom(d1.Datas);
        LianHuaMsgInfo = p1;
        //炼化装备
        if (p1.LianHuaItem == null || p1.LianHuaItem.TypeID <= 0)
        {
            lianhuainfo.GetChild("lianhua1").asCom.GetChild("icon").asLoader.url = "";
            lianhuainfo.GetChild("lianhua1").asCom.GetChild("icon").onClick.Set(() =>
            {
                Tool.NoticeWords("请把需要炼化的装备从背包拖动到这里:", null);
            });
            lianhuainfo.GetChild("lianhua1").asCom.GetChild("level").asTextField.text = "";
        }
        else
        {
            var clientitemdrop = ExcelManager.Instance.GetItemManager().GetItemByID(p1.LianHuaItem.TypeID);
            if (clientitemdrop != null)
            {
                lianhuainfo.GetChild("lianhua1").asCom.GetChild("icon").asLoader.url = clientitemdrop.IconPath;
                lianhuainfo.GetChild("lianhua1").asCom.GetChild("icon").onClick.Set(() =>
                {
                    var item1 = new ItemInfo(p1.LianHuaItem.TypeID, p1.LianHuaItem.DBItemID, p1.LianHuaItem.Level);
                    item1.SetCallBackBtn("取消", () => {
                        Protomsg.CS_CancelItem msg1 = new Protomsg.CS_CancelItem();
                        msg1.SrcDBItemID = p1.LianHuaItem.DBItemID;
                        msg1.Dest = 1;
                        MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_CancelItem", msg1);
                        item1.Destroy();
                    });
                });
                lianhuainfo.GetChild("lianhua1").asCom.GetChild("level").asTextField.text = "Lv."+ p1.LianHuaItem.Level;
            }
            
        }
        //炼化材料
        if (p1.CaiLiaoItem == null || p1.CaiLiaoItem.TypeID <= 0)
        {
            lianhuainfo.GetChild("lianhua2").asCom.GetChild("icon").asLoader.url = "";
            lianhuainfo.GetChild("lianhua2").asCom.GetChild("icon").onClick.Set(() =>
            {
                Tool.NoticeWords("请把需要使用的炼化材料从背包拖动到这里:", null);
            });
            lianhuainfo.GetChild("lianhua2").asCom.GetChild("level").asTextField.text = "";
        }
        else
        {
            var clientitemdrop = ExcelManager.Instance.GetItemManager().GetItemByID(p1.CaiLiaoItem.TypeID);
            if (clientitemdrop != null)
            {
                lianhuainfo.GetChild("lianhua2").asCom.GetChild("icon").asLoader.url = clientitemdrop.IconPath;
                lianhuainfo.GetChild("lianhua2").asCom.GetChild("icon").onClick.Set(() =>
                {
                    var item1 = new ItemInfo(p1.CaiLiaoItem.TypeID, p1.CaiLiaoItem.DBItemID, p1.CaiLiaoItem.Level);
                    item1.SetCallBackBtn("取消", () => {
                        Protomsg.CS_CancelItem msg1 = new Protomsg.CS_CancelItem();
                        msg1.SrcDBItemID = p1.CaiLiaoItem.DBItemID;
                        msg1.Dest = 2;
                        MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_CancelItem", msg1);
                        item1.Destroy();
                    });
                });
                lianhuainfo.GetChild("lianhua2").asCom.GetChild("level").asTextField.text = "Lv." + p1.CaiLiaoItem.Level;
            }

        }

        //成功率
        lianhuainfo.GetChild("succ_persent").asTextField.SetVar("p1", p1.SuccPercent + "");
        lianhuainfo.GetChild("succ_persent").asTextField.FlushVars();

        //炼化需要的时间 以分钟为单位
        //lianhuainfo.GetChild("lianhuades").asTextField.SetVar("p1", p1.LianHuaTime/60 + "");
        //lianhuainfo.GetChild("lianhuades").asTextField.FlushVars();


        Protomsg.ItemOnlyMsg[] allplayer = new Protomsg.ItemOnlyMsg[p1.FuZhuItem.Count];
        p1.FuZhuItem.CopyTo(allplayer, 0);

        lianhuainfo.GetChild("droplist").asList.RemoveChildren(0, -1, true);
        System.Array.Sort(allplayer, (a, b) => {

            if (a.TypeID > b.TypeID)
            {
                return 1;
            }
            else if (a.TypeID < b.TypeID)
            {
                return -1;
            }
            return 0;
        });
        foreach (var itemdrop in allplayer)
        {
            var clientitemdrop = ExcelManager.Instance.GetItemManager().GetItemByID(itemdrop.TypeID);
            if (clientitemdrop == null)
            {
                continue;
            }
            var onedropitemdrop = UIPackage.CreateObject("GameUI", "sellable").asCom;
            onedropitemdrop.GetChild("icon").asLoader.url = clientitemdrop.IconPath;
            onedropitemdrop.GetChild("icon").onClick.Add(() =>
            {
                var item1 = new ItemInfo(itemdrop.TypeID,itemdrop.DBItemID,itemdrop.Level);
                item1.SetCallBackBtn("取消", () => {
                    Protomsg.CS_CancelItem msg1 = new Protomsg.CS_CancelItem();
                    msg1.SrcDBItemID = itemdrop.DBItemID;
                    msg1.Dest = 3;
                    MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_CancelItem", msg1);
                    item1.Destroy();
                });
            });
            onedropitemdrop.GetChild("level").asTextField.text = "Lv."+ itemdrop.Level;
            lianhuainfo.GetChild("droplist").asList.AddChild(onedropitemdrop);
        }


        HideBagItemWithLianHua();

        return true;
    }

    public bool SC_OpenStorage(Protomsg.MsgBase d1)
    {
        Debug.Log("SC_OpenStorage--------------");
        IMessage IMperson = new Protomsg.SC_OpenStorage();
        Protomsg.SC_OpenStorage p1 = (Protomsg.SC_OpenStorage)IMperson.Descriptor.Parser.ParseFrom(d1.Datas);

        Protomsg.ItemOnlyMsg[] allplayer = new Protomsg.ItemOnlyMsg[p1.StorageItems.Count];
        p1.StorageItems.CopyTo(allplayer, 0);

        storageinfo.GetChild("droplist").asList.RemoveChildren(0, -1, true);

        storageinfo.GetChild("count").asTextField.text = p1.StorageItems.Count+"/"+p1.MaxCount;
        for (var i = 0; i < p1.MaxCount; i++)
        {

            var itemid = -1;
            var itemlevel = 0;
            var dbitemid = -1;
            if (i >= allplayer.Length)
            {

            }
            else
            {
                Protomsg.ItemOnlyMsg data = allplayer[i];
                itemid = data.TypeID;
                itemlevel = data.Level;
                dbitemid = data.DBItemID;
            }

            var clientitem = ExcelManager.Instance.GetItemManager().GetItemByID(itemid);
            
            var onedropitem = UIPackage.CreateObject("GameUI", "sellable").asCom;
            onedropitem.GetChild("icon").asLoader.url = "";
            if (clientitem != null)
            {
                var index = i;
                onedropitem.GetChild("icon").asLoader.url = clientitem.IconPath;
                onedropitem.GetChild("icon").onClick.Add(() =>
                {
                    //new ItemInfo(itemid);
                    ItemInfo a = new ItemInfo(itemid, dbitemid,itemlevel);
                    
                    a.SetCallBackBtn("取出", () => {
                        Protomsg.CS_TakeOutFromStorage msg1 = new Protomsg.CS_TakeOutFromStorage();
                        msg1.Pos = index;
                        Debug.Log("取出index:" + index);
                        MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_TakeOutFromStorage", msg1);
                        a.Destroy();
                    });
                });
            }

            if (clientitem != null)
            {
                onedropitem.GetChild("level").asTextField.text = "lv."+ itemlevel;
            }
            else
            {
                onedropitem.GetChild("level").asTextField.text = "";
            }
                


            storageinfo.GetChild("droplist").asList.AddChild(onedropitem);

        }


        return true;
    }

    public bool SC_UnitInfo(Protomsg.MsgBase d1)
    {
        //Debug.Log("SC_Update:");
        IMessage IMperson = new Protomsg.SC_UnitInfo();
        Protomsg.SC_UnitInfo p1 = (Protomsg.SC_UnitInfo)IMperson.Descriptor.Parser.ParseFrom(d1.Datas);

        FreshUnitInfoData(p1.UnitData);
        var ismyunit = false;
        if(p1.UnitData.ID == GameScene.Singleton.m_MyMainUnit.ID)
        {
            ismyunit = true;
        }
        Debug.Log("--Talents:" + p1.UnitData.Talents);

        FreshTalentInfo(p1.UnitData.Talents, ismyunit);

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

        HideBagItemWithLianHua();


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
                    if(itemlist.GetChild(onedrop[0]) != null)
                    {
                        Debug.Log("--repeat--" + onedrop[0]);
                        continue;
                    }
                    //var typeid = 
                    var clientitem = ExcelManager.Instance.GetItemManager().GetItemByID(int.Parse(onedrop[0]));
                    if (clientitem != null)
                    {
                        var onedropitem = UIPackage.CreateObject("GameUI", "dropItemButton1").asButton;
                        onedropitem.icon = clientitem.IconPath;
                        float f1 = System.Convert.ToSingle(onedrop[1]);
                        //onedropitem.GetChild("droptxt").asTextField.text = (f1 * 100).ToString("0.00")+"%";
                        onedropitem.GetChild("droptxt").asTextField.text = "";


                        onedropitem.onClick.Add(() => {
                            //Debug.Log("onClick");
                            if (clientitem.TypeID != -1)
                            {
                               ItemInfo iteminfoa = new ItemInfo(clientitem.TypeID);
                                iteminfoa.AddDes("\n[color=#66ff66]掉率:" + (f1 * 100).ToString("0.00") + "%[/color]");
                            }
                        });

                        onedropitem.name = onedrop[0];
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
        unitinfo.GetChild("level").asTextField.text = "Lv."+((int)unit.Level).ToString();
        ////经验值
        //unitinfo.GetChild("experience").asTextField.text = ((int)unit.Experience).ToString()+"/"+ ((int)unit.MaxExperience).ToString();

        ////今日剩余经验值
        //unitinfo.GetChild("remainexperience").asTextField.text = ((int)data.RemainExperience).ToString();
        ////if(unit.UnitType != 1)
        //if(true)
        //{
        //    unitinfo.GetChild("remainexperience").asTextField.visible = false;
        //    unitinfo.GetChild("remainexlable").asTextField.visible = false;

        //    unitinfo.GetChild("exlable").asTextField.visible = false;
        //    unitinfo.GetChild("experience").asTextField.visible = false;
        //}

        //力量 敏捷 智力
        unitinfo.GetChild("strength").asTextField.text = ((int)data.RawAttributeStrength).ToString()+ "[color=#00EE00]+" + ((int)data.AttributeStrength- (int)data.RawAttributeStrength).ToString()+ "[/color]";
        unitinfo.GetChild("agility").asTextField.text = ((int)data.RawAttributeAgility).ToString() + "[color=#00EE00]+" + ((int)data.AttributeAgility - (int)data.RawAttributeAgility).ToString() + "[/color]"; //((int)data.AttributeAgility).ToString();
        unitinfo.GetChild("intelligence").asTextField.text = ((int)data.RawAttributeIntelligence).ToString() + "[color=#00EE00]+" + ((int)data.AttributeIntelligence - (int)data.RawAttributeIntelligence).ToString() + "[/color]"; //((int)data.AttributeIntelligence).ToString();
        //主属性(1:力量 2:敏捷 3:智力)
        if (data.AttributePrimary == 1)
        {
            unitinfo.GetChild("strength").asTextField.color = new Color(0.8f, 0.1f, 0.4f);
            unitinfo.GetChild("agility").asTextField.color = new Color(1, 1, 1);
            unitinfo.GetChild("intelligence").asTextField.color = new Color(1, 1, 1);
        }
        else if(data.AttributePrimary == 2)
        {
            unitinfo.GetChild("strength").asTextField.color = new Color(1, 1, 1);
            unitinfo.GetChild("agility").asTextField.color = new Color(0.8f, 0.1f, 0.4f);
            unitinfo.GetChild("intelligence").asTextField.color = new Color(1, 1, 1);
            
        }
        else if(data.AttributePrimary == 3)
        {
            unitinfo.GetChild("strength").asTextField.color = new Color(1, 1, 1);
            unitinfo.GetChild("agility").asTextField.color = new Color(1, 1, 1);
            unitinfo.GetChild("intelligence").asTextField.color = new Color(0.8f, 0.1f, 0.4f);
        }
        //攻击
        unitinfo.GetChild("attack").asTextField.text = ((int)data.RawAttack).ToString() + "[color=#00EE00]+" + ((int)data.Attack - (int)data.RawAttack).ToString() + "[/color]"; //((int)data.Attack).ToString();
        unitinfo.GetChild("attackspeed").asTextField.text = ((int)data.RawAttackSpeed).ToString() + "[color=#00EE00]+" + ((int)data.AttackSpeed - (int)data.RawAttackSpeed).ToString() + "[/color]"; //((int)data.AttackSpeed).ToString();
        unitinfo.GetChild("attackrange").asTextField.text = (data.RawAttackRange).ToString("f2") + "[color=#00EE00]+" + (data.AttackRange - data.RawAttackRange).ToString("f2") + "[/color]"; //(data.AttackRange).ToString("f2");
        unitinfo.GetChild("movespeed").asTextField.text = (data.RawMoveSpeed).ToString("f2") + "[color=#00EE00]+" + (data.MoveSpeed - data.RawMoveSpeed).ToString("f2") + "[/color]"; //(data.MoveSpeed).ToString("f2");
        unitinfo.GetChild("magicscale").asTextField.text = ((int)(data.MagicScale*100)).ToString()+"%";
        unitinfo.GetChild("mpregain").asTextField.text = (data.RawMPRegain).ToString("f2") + "[color=#00EE00]+" + (data.MPRegain - data.RawMPRegain).ToString("f2") + "[/color]"; //(data.MPRegain).ToString("f2");
        //防御
        unitinfo.GetChild("physicalamaor").asTextField.text = (data.RawPhysicalAmaor).ToString("f2") + "[color=#00EE00]+" + (data.PhysicalAmaor - data.RawPhysicalAmaor).ToString("f2") + "[/color]"; //((int)data.PhysicalAmaor).ToString();
        unitinfo.GetChild("physicalresist").asTextField.text = ((data.PhysicalResist * 100)).ToString("f2") + "%";
        unitinfo.GetChild("magicamaor").asTextField.text = ((int)(data.MagicAmaor * 100)).ToString() + "%";
        unitinfo.GetChild("stateamaor").asTextField.text = ((int)(data.StatusAmaor * 100)).ToString() + "%";
        unitinfo.GetChild("dodge").asTextField.text = ((int)(data.Dodge * 100)).ToString() + "%";
        unitinfo.GetChild("hpregain").asTextField.text = (data.RawHPRegain).ToString("f2") + "[color=#00EE00]+" + (data.HPRegain - data.RawHPRegain).ToString("f2") + "[/color]"; //(data.HPRegain).ToString("f2");


        FreshDropItem(data.DropItems);

    }
}
