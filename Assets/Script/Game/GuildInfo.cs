using cocosocket4unity;
using FairyGUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf;
using System.Threading;
using System;

public class GuildInfo
{
    private GComponent main;
    
    public GuildInfo()
    {
        if (GameScene.Singleton.m_MyMainUnit == null)
        {
            return;
        }

        MsgManager.Instance.AddListener("SC_GetAllGuildsInfo", new HandleMsg(this.SC_GetAllGuildsInfo));
        MsgManager.Instance.AddListener("SC_GetGuildInfo", new HandleMsg(this.SC_GetGuildInfo));
        MsgManager.Instance.AddListener("SC_GetJoinGuildPlayer", new HandleMsg(this.SC_GetJoinGuildPlayer));
        MsgManager.Instance.AddListener("SC_GetAuctionItems", new HandleMsg(this.SC_GetAuctionItems));

        MsgManager.Instance.AddListener("SC_GetGuildMapsInfo", new HandleMsg(this.SC_GetGuildMapsInfo));

        MsgManager.Instance.AddListener("SC_GotoGuildMap", new HandleMsg(this.SC_GotoGuildMap));

        MsgManager.Instance.AddListener("SC_GetMapInfo", new HandleMsg(this.SC_GetMapInfo));

        if (GameScene.Singleton.m_MyMainUnit.GuildID > 0)
        {
            //自己有公会
            //获取数据
            Protomsg.CS_GetGuildInfo msg1 = new Protomsg.CS_GetGuildInfo();
            msg1.ID = GameScene.Singleton.m_MyMainUnit.GuildID;
            MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_GetGuildInfo", msg1);
            AudioManager.Am.Play2DSound(AudioManager.Sound_OpenUI);
        }
        else
        {
            //自己无公会
            Protomsg.CS_GetAllGuildsInfo msg1 = new Protomsg.CS_GetAllGuildsInfo();
            MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_GetAllGuildsInfo", msg1);
            AudioManager.Am.Play2DSound(AudioManager.Sound_OpenUI);

        }




    }
    //弹出创建公会对话框
    public void createguildwindow(int price,int pricetype)
    {
        var createguildwd = UIPackage.CreateObject("GameUI", "CreateGuild").asCom;
        GRoot.inst.AddChild(createguildwd);
        createguildwd.xy = Tool.GetPosition(0.5f, 0.5f);
        createguildwd.GetChild("close").asButton.onClick.Add(() =>
        {
            createguildwd.Dispose();
        });

        createguildwd.GetChild("create").asButton.onClick.Add(() =>
        {
            var txt = createguildwd.GetChild("input").asTextInput.text;
            if (txt.Length <= 0)
            {
                Tool.NoticeWords("请输入名字！", null);
                return;
            }
            if (Tool.IsChineseOrNumberOrWord(txt) == false)
            {
                Tool.NoticeWords("名字不能含有中文,字母,数字以外的其他字符！", null);
                return;
            }
            //创建 
            Protomsg.CS_CreateGuild msg1 = new Protomsg.CS_CreateGuild();
            msg1.Name = txt;
            MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_CreateGuild", msg1);
            createguildwd.Dispose();
        });

        createguildwd.GetChild("price").asTextField.text = price + "";
        createguildwd.GetChild("pricetype").asLoader.url = Tool.GetPriceTypeIcon(pricetype);
    }


    public bool SC_GetAllGuildsInfo(Protomsg.MsgBase d1)
    {
        Debug.Log("SC_GetAllGuildsInfo:");
        IMessage IMperson = new Protomsg.SC_GetAllGuildsInfo();
        Protomsg.SC_GetAllGuildsInfo p1 = (Protomsg.SC_GetAllGuildsInfo)IMperson.Descriptor.Parser.ParseFrom(d1.Datas);

        //创建界面
        if (main != null)
        {
            main.Dispose();
        }
        main = UIPackage.CreateObject("GameUI", "AllGuilds").asCom;
        GRoot.inst.AddChild(main);
        main.xy = Tool.GetPosition(0.5f, 0.5f);
        main.GetChild("close").asButton.onClick.Add(() =>
        {
            this.Destroy();
        });
        //创建公会按钮
        main.GetChild("add").asButton.onClick.Add(() =>
        {
            this.createguildwindow(p1.CreatePrice, p1.CreatePriceType);
        });
        
        //main.GetChild("list").asList.RemoveChildren(0, -1, true);
        //处理排序
        Protomsg.GuildShortInfo[] allplayer = new Protomsg.GuildShortInfo[p1.Guilds.Count];
        int index = 0;
        foreach (var item in p1.Guilds)
        {
            allplayer[index++] = item;
        }
        System.Array.Sort(allplayer, (a, b) => {

            if(a.Level > b.Level)
            {
                return 1;
            }
            else if(a.Level == b.Level)
            {
                if( a.Experience > b.Experience)
                {
                    return 1;
                }
                else
                {
                    return -1;
                }
                
            }
            else
            {
                return -1;
            }
        });
        foreach (var item in allplayer)
        {
            var onedropitem = UIPackage.CreateObject("GameUI", "GuildsOne").asCom;

            onedropitem.GetChild("add").onClick.Add(() =>
            {
                //申请加入公会
                Protomsg.CS_JoinGuild msg1 = new Protomsg.CS_JoinGuild();
                msg1.ID = item.ID;
                MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_JoinGuild", msg1);
            });
            onedropitem.GetChild("name").asTextField.text = item.Name;
            onedropitem.GetChild("level").asTextField.text = item.Level+"";
            onedropitem.GetChild("count").asTextField.text = item.CharacterCount + "/"+item.MaxCount;
            onedropitem.GetChild("president").asTextField.text = item.PresidentName;
            onedropitem.GetChild("levellimit").asTextField.text = item.Joinlevellimit + "";

            main.GetChild("list").asList.AddChild(onedropitem);
        }

        return true;
    }


    //进入公会地图
    public bool SC_GotoGuildMap(Protomsg.MsgBase d1)
    {
        Debug.Log("SC_GotoGuildMap:");
        IMessage IMperson = new Protomsg.SC_GotoGuildMap();
        Protomsg.SC_GotoGuildMap p1 = (Protomsg.SC_GotoGuildMap)IMperson.Descriptor.Parser.ParseFrom(d1.Datas);
        if (main == null)
        {
            return true;
        }

        if(p1.Result == 1)
        {
            this.Destroy();
        }
        return true;
    }

    //获取公会地图信息
    public bool SC_GetGuildMapsInfo(Protomsg.MsgBase d1)
    {
        Debug.Log("CS_GetGuildMapsInfo:");
        IMessage IMperson = new Protomsg.SC_GetGuildMapsInfo();
        Protomsg.SC_GetGuildMapsInfo p1 = (Protomsg.SC_GetGuildMapsInfo)IMperson.Descriptor.Parser.ParseFrom(d1.Datas);
        if (main == null)
        {
            return true;
        }
        main.GetChild("maplist").asList.RemoveChildren(0, -1, true);

        Protomsg.GuildMapInfo[] allplayer = new Protomsg.GuildMapInfo[p1.Maps.Count];
        p1.Maps.CopyTo(allplayer, 0);
        System.Array.Sort(allplayer, (a, b) => {

            if (a.ID > b.ID)
            {
                return 1;
            }
            else if (a.ID < b.ID)
            {
                return -1;
            }
            return 0;
        });

        //遍历
        foreach (var item in allplayer)
        {
            var clientitem = ExcelManager.Instance.GetSceneManager().GetSceneByID(item.NextSceneID);
            if (clientitem == null)
            {
                continue;
            }

            var onedropitem = UIPackage.CreateObject("GameUI", "GuildMapInfo").asCom;


            //onedropitem.GetChild("item").asCom.GetChild("icon").asLoader.url = clientitem.IconPath;
            onedropitem.GetChild("name").asTextField.text = clientitem.Name;
            onedropitem.GetChild("guildlevel").asTextField.text = item.NeedGuildLevel + "";
            onedropitem.GetChild("time").asTextField.text = item.OpenStartTime + "--"+item.OpenEndTime;
            onedropitem.GetChild("week").asTextField.text = "周"+item.OpenWeekDay;

            //进入
            onedropitem.GetChild("goto").asButton.onClick.Add(() =>
            {
                Protomsg.CS_GotoGuildMap msg1 = new Protomsg.CS_GotoGuildMap();
                msg1.ID = item.ID;
                MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_GotoGuildMap", msg1);
            });
            //地图信息
            onedropitem.GetChild("icon").asLoader.onClick.Add(() =>
            {
                Protomsg.CS_GetMapInfo msg1 = new Protomsg.CS_GetMapInfo();
                msg1.SceneID = item.NextSceneID;
                MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_GetMapInfo", msg1);
            });

            main.GetChild("maplist").asList.AddChild(onedropitem);

        }

        return true;
    }
        

    public bool SC_GetAuctionItems(Protomsg.MsgBase d1)
    {
        Debug.Log("SC_GetAuctionItems:");
        IMessage IMperson = new Protomsg.SC_GetAuctionItems();
        Protomsg.SC_GetAuctionItems p1 = (Protomsg.SC_GetAuctionItems)IMperson.Descriptor.Parser.ParseFrom(d1.Datas);
        if (main == null)
        {
            return true;
        }
        main.GetChild("auctionlist").asList.RemoveChildren(0, -1, true);
        //处理排序
        Protomsg.AuctionItem[] allplayer = new Protomsg.AuctionItem[p1.Items.Count];
        p1.Items.CopyTo(allplayer, 0);
        System.Array.Sort(allplayer, (a, b) => {

            if (a.RemainTime > b.RemainTime)
            {
                return 1;
            }
            else if (a.RemainTime < b.RemainTime)
            {
                return -1;
            }
            return 0;
        });

        //遍历
        foreach (var item in allplayer)
        {
            var clientitem = ExcelManager.Instance.GetItemManager().GetItemByID(item.ItemID);
            if (clientitem == null)
            {
                continue;
            }

            var onedropitem = UIPackage.CreateObject("GameUI", "AuctionOne").asCom;
            if(clientitem.Name == "")
            {
                onedropitem.GetChild("name").asTextField.text = "无";
            }
            else
            {
                onedropitem.GetChild("name").asTextField.text = clientitem.Name;
            }
            
            onedropitem.GetChild("item").asCom.GetChild("icon").asLoader.url = clientitem.IconPath;
            onedropitem.GetChild("item").asCom.GetChild("level").asTextField.text = item.Level + "";
            onedropitem.GetChild("item").asCom.onClick.Add(() =>
            {
                string des = "\n\n参与分红的成员:\n";
                foreach (var item1 in item.ReceivecharactersName)
                {
                    if(item1 == item.ReceivecharactersName[item.ReceivecharactersName.Count - 1])
                    {
                        des += item1;
                    }
                    else
                    {
                        des += item1 + ",";
                    }
                    
                }
                new ItemInfo(item.ItemID).AddDes(des);
            });

            onedropitem.GetChild("pricetype").asLoader.url = Tool.GetPriceTypeIcon(item.PriceType);
            onedropitem.GetChild("price").asTextField.text = item.Price + "";
            onedropitem.GetChild("playername").asTextField.text = item.BidderCharacterName;
            onedropitem.GetChild("remaintime").asTextField.text = Tool.Time2String(item.RemainTime);

            //出价
            onedropitem.GetChild("add").asButton.onClick.Add(() =>
            {
                //售卖
                var sellwindow = UIPackage.CreateObject("GameUI", "NewPrice").asCom;
                GRoot.inst.AddChild(sellwindow);
                sellwindow.xy = Tool.GetPosition(0.5f, 0.5f);
                sellwindow.GetChild("close").onClick.Add(() =>
                {
                    sellwindow.Dispose();
                });
                sellwindow.GetChild("yes_btn").onClick.Add(() =>
                {
                    var txt = sellwindow.GetChild("input").asTextInput.text;
                    if (txt.Length <= 0)
                    {
                        Tool.NoticeWords("请输入价格！", null);
                        return;
                    }

                    int price = 0;
                    try
                    {
                        price = Convert.ToInt32(txt); //报异常
                    }
                    catch (SystemException e)
                    {
                        Tool.NoticeWords("请输入正确的价格！", null);
                        return;
                    }
                    if (price <= 0)
                    {
                        Tool.NoticeWords("请输入正确的价格！", null);
                        return;
                    }
                    //上架
                    Protomsg.CS_NewPriceAuctionItem msg1 = new Protomsg.CS_NewPriceAuctionItem();
                    msg1.Price = price;
                    msg1.ID = item.ID;
                    MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_NewPriceAuctionItem", msg1);
                    sellwindow.Dispose();
                });
                sellwindow.GetChild("item").asCom.GetChild("icon").asLoader.url = clientitem.IconPath;
                sellwindow.GetChild("item").asCom.GetChild("level").asTextField.text = item.Level + "";
                sellwindow.GetChild("name").asTextField.text = clientitem.Name;
                sellwindow.GetChild("pricetype").asLoader.url = Tool.GetPriceTypeIcon(item.PriceType);
                sellwindow.GetChild("input").asTextInput.text = (item.Price+1) +"";
            });

            main.GetChild("auctionlist").asList.AddChild(onedropitem);

        }

        return true;
    }


    public bool SC_GetMapInfo(Protomsg.MsgBase d1)
    {
        Debug.Log("SC_GetMapInfo:");
        IMessage IMperson = new Protomsg.SC_GetMapInfo();
        Protomsg.SC_GetMapInfo p1 = (Protomsg.SC_GetMapInfo)IMperson.Descriptor.Parser.ParseFrom(d1.Datas);

        var mapinfo = UIPackage.CreateObject("GameUI", "MapInfo").asCom;
        GRoot.inst.AddChild(mapinfo);
        mapinfo.xy = Tool.GetPosition(0.5f, 0.5f);
        mapinfo.GetChild("close").asButton.onClick.Add(() =>
        {
            mapinfo.Dispose();
        });
        var sceneitem = ExcelManager.Instance.GetSceneManager().GetSceneByID(p1.SceneID);
        if (sceneitem != null)
        {
            mapinfo.GetChild("name").asTextField.text = sceneitem.Name;
        }

        mapinfo.GetChild("time").asTextField.text = Tool.Time2String(p1.BossFreshTime);

        //掉落道具
        mapinfo.GetChild("maplist").asList.RemoveChildren(0, -1, true);
        int[] allplayer = new int[p1.DropItems.Count];
        p1.DropItems.CopyTo(allplayer, 0);
        System.Array.Sort(allplayer, (a, b) => {

            if (a > b)
            {
                return 1;
            }
            else if (a < b)
            {
                return -1;
            }
            return 0;
        });
        foreach (var item in allplayer)
        {
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
            mapinfo.GetChild("maplist").asList.AddChild(onedropitem);
        }
        return true;
    }

    public bool SC_GetGuildInfo(Protomsg.MsgBase d1)
    {
        Debug.Log("SC_GetGuildInfo:");
        IMessage IMperson = new Protomsg.SC_GetGuildInfo();
        Protomsg.SC_GetGuildInfo p1 = (Protomsg.SC_GetGuildInfo)IMperson.Descriptor.Parser.ParseFrom(d1.Datas);
        //创建界面
        if (main != null)
        {
            main.Dispose();
        }
        main = UIPackage.CreateObject("GameUI", "GuildInfo").asCom;
        GRoot.inst.AddChild(main);
        main.xy = Tool.GetPosition(0.5f, 0.5f);
        main.GetChild("close").asButton.onClick.Add(() =>
        {
            this.Destroy();
        });
        //自己退出公会
        main.GetChild("exit").asButton.onClick.Add(() =>
        {
            Tool.NoticeWindonw("你确定要退出公会吗?", () =>
            {
                Protomsg.CS_GuildOperate msg1 = new Protomsg.CS_GuildOperate();
                msg1.Code = 1;//
                MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_GuildOperate", msg1);
            });
        });
        //解散公会
        main.GetChild("dismiss").asButton.onClick.Add(() =>
        {
            Tool.NoticeWindonw("你确定要解散公会吗?", () =>
            {
                Protomsg.CS_GuildOperate msg1 = new Protomsg.CS_GuildOperate();
                msg1.Code = 2;//
                MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_GuildOperate", msg1);
            });
        });


        //
        main.GetChild("request").asButton.onClick.Add(() =>
        {
            //查看申请列表
            Protomsg.CS_GetJoinGuildPlayer msg1 = new Protomsg.CS_GetJoinGuildPlayer();
            msg1.ID = p1.GuildBaseInfo.ID;
            MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_GetJoinGuildPlayer", msg1);
        });
        //查看拍卖行
        main.GetChild("auction").asButton.onClick.Add(() =>
        {
            //查看申请列表
            Protomsg.CS_GetAuctionItems msg1 = new Protomsg.CS_GetAuctionItems();
            MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_GetAuctionItems", msg1);
        });

        //查看公会地图
        main.GetChild("huodong").asButton.onClick.Add(() =>
        {
            //查看申请列表
            Protomsg.CS_GetGuildMapsInfo msg1 = new Protomsg.CS_GetGuildMapsInfo();
            msg1.ID = p1.GuildBaseInfo.ID;
            MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_GetGuildMapsInfo", msg1);
        });

        //-------------------------公会成员--------------------------
        //处理排序
        Protomsg.GuildChaInfo[] allplayer = new Protomsg.GuildChaInfo[p1.Characters.Count];
        int index = 0;
        foreach (var item in p1.Characters)
        {
            allplayer[index++] = item;
            Debug.Log("SC_GetGuildInfo111   :" + item.Level + " name:" + item.Name);
        }
        Array.Sort(allplayer, (a, b) => {
            if (a.Level > b.Level)
            {
                return -1;
            }
            else if (a.Level == b.Level)
            {
                //return 0;
                if (a.PinLevel > b.PinLevel)
                {
                    return -1;
                }
                else if (a.PinLevel == b.PinLevel)
                {
                    if (a.PinExperience > b.PinExperience)
                    {
                        return -1;
                    }
                    else
                    {
                        return 1;
                    }
                }
                else
                {
                    return 1;
                }

            }
            else
            {
                return 1;
            }
        });
        foreach (var item in allplayer)
        {
            Debug.Log("SC_GetGuildInfo   :"+item.Level+" name:"+item.Name );
            var onedropitem = UIPackage.CreateObject("GameUI", "GuildPlayerOne").asCom;

            onedropitem.GetChild("add").onClick.Add(() =>
            {
                //踢出公会
                Tool.NoticeWindonw("你确定要把("+ item.Name+")踢出公会吗?", () =>
                {
                    Protomsg.CS_DeleteGuildPlayer msg1 = new Protomsg.CS_DeleteGuildPlayer();
                    msg1.Characterid = item.Characterid;
                    MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_DeleteGuildPlayer", msg1);
                });
                
            });
            onedropitem.GetChild("name").asTextField.text = item.Name;
            onedropitem.GetChild("level").asTextField.text = item.Level + "";
            onedropitem.GetChild("pinlevel").asTextField.text = item.PinLevelName;
            onedropitem.GetChild("post").asTextField.text = item.PostName;
            onedropitem.GetChild("experience").asTextField.text = item.PinExperience+"/"+item.PinMaxExperience;

            var clientitem = ExcelManager.Instance.GetUnitInfoManager().GetUnitInfoByID(item.Typeid);
            if (clientitem != null)
            {
                onedropitem.GetChild("heroicon").asLoader.url = clientitem.IconPath;
            }

            main.GetChild("mainlist").asList.AddChild(onedropitem);
        }
        //-----------------公会信息------------------
        main.GetChild("name").asTextField.text = p1.GuildBaseInfo.Name;
        main.GetChild("level").asTextField.text = "Lv."+p1.GuildBaseInfo.Level;
        main.GetChild("experience").asTextField.text = p1.GuildBaseInfo.Experience+"/"+p1.GuildBaseInfo.MaxExperience;
        main.GetChild("playercount").asTextField.text = p1.GuildBaseInfo.CharacterCount+"/"+p1.GuildBaseInfo.MaxCount;
        main.GetChild("gonggao").asTextField.text = p1.GuildBaseInfo.Notice;
        return true;
    }
    public bool SC_GetJoinGuildPlayer(Protomsg.MsgBase d1)
    {
        Debug.Log("SC_GetJoinGuildPlayer:");
        IMessage IMperson = new Protomsg.SC_GetJoinGuildPlayer();
        Protomsg.SC_GetJoinGuildPlayer p1 = (Protomsg.SC_GetJoinGuildPlayer)IMperson.Descriptor.Parser.ParseFrom(d1.Datas);
        //创建界面
        if (main == null)
        {
            return true;
        }

        //-------------------------公会成员--------------------------
        main.GetChild("requestlist").asList.RemoveChildren(0, -1, true);
        //处理排序
        Protomsg.GuildChaInfo[] allplayer = new Protomsg.GuildChaInfo[p1.RequestCharacters.Count];
        int index = 0;
        foreach (var item in p1.RequestCharacters)
        {
            allplayer[index++] = item;
        }
        System.Array.Sort(allplayer, (a, b) => {

            if (a.Level > b.Level)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        });
        foreach (var item in allplayer)
        {
            var onedropitem = UIPackage.CreateObject("GameUI", "GuildRequestPlayerOne").asCom;

            onedropitem.GetChild("agree").onClick.Add(() =>
            {
                //同意
                Protomsg.CS_ResponseJoinGuildPlayer msg1 = new Protomsg.CS_ResponseJoinGuildPlayer();
                msg1.Characterid = item.Characterid;
                msg1.Result = 1;
                MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_ResponseJoinGuildPlayer", msg1);
            });
            onedropitem.GetChild("no").onClick.Add(() =>
            {
                //拒绝
                Protomsg.CS_ResponseJoinGuildPlayer msg1 = new Protomsg.CS_ResponseJoinGuildPlayer();
                msg1.Characterid = item.Characterid;
                msg1.Result = 0;
                MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_ResponseJoinGuildPlayer", msg1);
            });
            onedropitem.GetChild("name").asTextField.text = item.Name;
            onedropitem.GetChild("level").asTextField.text = item.Level + "";
            

            var clientitem = ExcelManager.Instance.GetUnitInfoManager().GetUnitInfoByID(item.Typeid);
            if (clientitem != null)
            {
                onedropitem.GetChild("heroicon").asLoader.url = clientitem.IconPath;
            }

            main.GetChild("requestlist").asList.AddChild(onedropitem);
        }
       
        return true;
    }
    

    //
    public void Destroy()
    {
        MsgManager.Instance.RemoveListener("SC_GetAllGuildsInfo");
        MsgManager.Instance.RemoveListener("SC_GetGuildInfo");
        MsgManager.Instance.RemoveListener("SC_GetJoinGuildPlayer");
        MsgManager.Instance.RemoveListener("SC_GetAuctionItems");

        MsgManager.Instance.RemoveListener("SC_GetGuildMapsInfo");
        MsgManager.Instance.RemoveListener("SC_GotoGuildMap");
        MsgManager.Instance.RemoveListener("SC_GetMapInfo");

        AudioManager.Am.Play2DSound(AudioManager.Sound_CloseUI);
        if (main != null)
        {
            main.Dispose();
        }
    }

    
}
