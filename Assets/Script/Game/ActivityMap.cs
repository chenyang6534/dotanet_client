using cocosocket4unity;
using FairyGUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf;
using System.Threading;
using System;

public class ActivityMap
{
    private GComponent main;
    
    public ActivityMap()
    {

        MsgManager.Instance.AddListener("SC_GetActivityMapsInfo", new HandleMsg(this.SC_GetActivityMapsInfo));
        MsgManager.Instance.AddListener("SC_GetMapInfo", new HandleMsg(this.SC_GetMapInfo));
        MsgManager.Instance.AddListener("SC_GotoActivityMap", new HandleMsg(this.SC_GotoActivityMap));
        MsgManager.Instance.AddListener("SC_GetDuoBaoInfo", new HandleMsg(this.SC_GetDuoBaoInfo));
        MsgManager.Instance.AddListener("SC_GetEndlessLevelInfo", new HandleMsg(this.SC_GetEndlessLevelInfo));

        MsgManager.Instance.AddListener("SC_GetWorldMapsInfo", new HandleMsg(this.SC_GetWorldMapsInfo));

        main = UIPackage.CreateObject("GameUI", "ActivityMap").asCom;
        GRoot.inst.AddChild(main);
        main.xy = Tool.GetPosition(0.5f, 0.5f);
        main.GetChild("close").asButton.onClick.Add(() =>
        {
            this.Destroy();
        });

        Protomsg.CS_GetActivityMapsInfo msg1 = new Protomsg.CS_GetActivityMapsInfo();
        MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_GetActivityMapsInfo", msg1);


        main.GetChild("normal").asButton.onClick.Add(() =>
        {
            Protomsg.CS_GetActivityMapsInfo msg = new Protomsg.CS_GetActivityMapsInfo();
            MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_GetActivityMapsInfo", msg);
        });
        main.GetChild("duobao").asButton.onClick.Add(() =>
        {
            Protomsg.CS_GetDuoBaoInfo msg = new Protomsg.CS_GetDuoBaoInfo();
            MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_GetDuoBaoInfo", msg);
        });
        main.GetChild("wujin").asButton.onClick.Add(() =>
        {
            Protomsg.CS_GetEndlessLevelInfo msg = new Protomsg.CS_GetEndlessLevelInfo();
            MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_GetEndlessLevelInfo", msg);
        });
        main.GetChild("world").asButton.onClick.Add(() =>
        {
            Protomsg.CS_GetWorldMapsInfo msg = new Protomsg.CS_GetWorldMapsInfo();
            MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_GetWorldMapsInfo", msg);
        });
    }

    //SC_GetWorldMapsInfo
    public bool SC_GetWorldMapsInfo(Protomsg.MsgBase d1)
    {
        Debug.Log("SC_GetWorldMapsInfo:");
        IMessage IMperson = new Protomsg.SC_GetWorldMapsInfo();
        Protomsg.SC_GetWorldMapsInfo p1 = (Protomsg.SC_GetWorldMapsInfo)IMperson.Descriptor.Parser.ParseFrom(d1.Datas);
        main.GetChild("worldmaplist").asList.RemoveChildren(0, -1, true);

        Protomsg.WorldMapInfo[] allplayer = new Protomsg.WorldMapInfo[p1.Maps.Count];
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

            var onedropitem = UIPackage.CreateObject("GameUI", "WorldMapInfo").asCom;

            //onedropitem.GetChild("item").asCom.GetChild("icon").asLoader.url = clientitem.IconPath;
            onedropitem.GetChild("name").asTextField.text = clientitem.Name;
            onedropitem.GetChild("guildlevel").asTextField.text = item.NeedLevel + "";
            onedropitem.GetChild("pricetype").asLoader.url = Tool.GetPriceTypeIcon(item.PriceType);
            onedropitem.GetChild("price").asTextField.text = item.Price + "";
            //进入
            onedropitem.GetChild("goto").asButton.onClick.Add(() =>
            {
                Protomsg.CS_GotoWorldMap msg1 = new Protomsg.CS_GotoWorldMap();
                msg1.ID = item.ID;
                MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_GotoWorldMap", msg1);
            });

            //地图信息
            onedropitem.GetChild("icon").asLoader.onClick.Add(() =>
            {
                Protomsg.CS_GetMapInfo msg1 = new Protomsg.CS_GetMapInfo();
                msg1.SceneID = item.NextSceneID;
                MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_GetMapInfo", msg1);
            });


            main.GetChild("worldmaplist").asList.AddChild(onedropitem);

        }

        return true;
    }

    //进入公会地图
    public bool SC_GetActivityMapsInfo(Protomsg.MsgBase d1)
    {
        Debug.Log("SC_GetActivityMapsInfo:");
        IMessage IMperson = new Protomsg.SC_GetActivityMapsInfo();
        Protomsg.SC_GetActivityMapsInfo p1 = (Protomsg.SC_GetActivityMapsInfo)IMperson.Descriptor.Parser.ParseFrom(d1.Datas);
        main.GetChild("maplist").asList.RemoveChildren(0, -1, true);

        Protomsg.ActivityMapInfo[] allplayer = new Protomsg.ActivityMapInfo[p1.Maps.Count];
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

            var onedropitem = UIPackage.CreateObject("GameUI", "ActivityMapInfo").asCom;

            //onedropitem.GetChild("item").asCom.GetChild("icon").asLoader.url = clientitem.IconPath;
            onedropitem.GetChild("name").asTextField.text = clientitem.Name;
            onedropitem.GetChild("guildlevel").asTextField.text = item.NeedLevel + "";
            onedropitem.GetChild("time").asTextField.text = item.OpenStartTime + "--" + item.OpenEndTime;
            onedropitem.GetChild("week").asTextField.text = "周" + item.OpenWeekDay;
            onedropitem.GetChild("pricetype").asLoader.url = Tool.GetPriceTypeIcon(item.PriceType);
            onedropitem.GetChild("price").asTextField.text = item.Price + "";
            //进入
            onedropitem.GetChild("goto").asButton.onClick.Add(() =>
            {
                Protomsg.CS_GotoActivityMap msg1 = new Protomsg.CS_GotoActivityMap();
                msg1.ID = item.ID;
                MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_GotoActivityMap", msg1);
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
    //无尽关卡SC_GetEndlessLevelInfo
    public bool SC_GetEndlessLevelInfo(Protomsg.MsgBase d1)
    {
        Debug.Log("SC_GetEndlessLevelInfo:");
        IMessage IMperson = new Protomsg.SC_GetEndlessLevelInfo();
        Protomsg.SC_GetEndlessLevelInfo p1 = (Protomsg.SC_GetEndlessLevelInfo)IMperson.Descriptor.Parser.ParseFrom(d1.Datas);
        
        var onedropitem = main.GetChild("wujincom").asCom;

        onedropitem.GetChild("guildlevel").asTextField.text = "" + p1.NeedLevel;
        onedropitem.GetChild("pricetype").asLoader.url = Tool.GetPriceTypeIcon(p1.PriceType);
        onedropitem.GetChild("price").asTextField.text = p1.Price + "";
        //进入
        onedropitem.GetChild("goto").asButton.onClick.Set(() =>
        {
            Protomsg.CS_GotoEndlessLevel msg1 = new Protomsg.CS_GotoEndlessLevel();
            MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_GotoEndlessLevel", msg1);

            this.Destroy();
        });
        //onedropitem.GetChild("item").asCom.GetChild("icon").asLoader.url = clientitem.IconPath;


        return true;
    }

    //夺宝奇兵 SC_GetDuoBaoInfo
    public bool SC_GetDuoBaoInfo(Protomsg.MsgBase d1)
    {
        Debug.Log("SC_GetDuoBaoInfo:");
        IMessage IMperson = new Protomsg.SC_GetDuoBaoInfo();
        Protomsg.SC_GetDuoBaoInfo p1 = (Protomsg.SC_GetDuoBaoInfo)IMperson.Descriptor.Parser.ParseFrom(d1.Datas);
        var item = p1.MapGoInInfo;
        var clientitem = ExcelManager.Instance.GetSceneManager().GetSceneByID(item.NextSceneID);
        if (clientitem == null)
        {
            return true;
        }
        var onedropitem = main.GetChild("duobaocom").asCom;

        onedropitem.GetChild("des").asTextField.SetVar("p1", p1.Minute+"");
        onedropitem.GetChild("des").asTextField.FlushVars();

        //onedropitem.GetChild("item").asCom.GetChild("icon").asLoader.url = clientitem.IconPath;
        onedropitem.GetChild("name").asTextField.text = clientitem.Name;
        onedropitem.GetChild("guildlevel").asTextField.text = item.NeedLevel + "";
        onedropitem.GetChild("time").asTextField.text = item.OpenStartTime + "--" + item.OpenEndTime;
        onedropitem.GetChild("week").asTextField.text = "周" + item.OpenWeekDay;
        onedropitem.GetChild("pricetype").asLoader.url = Tool.GetPriceTypeIcon(item.PriceType);
        onedropitem.GetChild("price").asTextField.text = item.Price + "";
        //进入
        onedropitem.GetChild("goto").asButton.onClick.Set(() =>
        {
            Protomsg.CS_GotoActivityMap msg1 = new Protomsg.CS_GotoActivityMap();
            msg1.ID = item.ID;
            MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_GotoActivityMap", msg1);
        });

        //地图信息
        //掉落道具
        onedropitem.GetChild("droplist").asList.RemoveChildren(0, -1, true);
        int[] allplayer = new int[p1.MapInfo.DropItems.Count];
        p1.MapInfo.DropItems.CopyTo(allplayer, 0);
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
        foreach (var itemdrop in allplayer)
        {
            var clientitemdrop = ExcelManager.Instance.GetItemManager().GetItemByID(itemdrop);
            if (clientitemdrop == null)
            {
                continue;
            }
            var onedropitemdrop = UIPackage.CreateObject("GameUI", "sellable").asCom;
            onedropitemdrop.GetChild("icon").asLoader.url = clientitemdrop.IconPath;
            onedropitemdrop.GetChild("icon").onClick.Add(() =>
            {
                new ItemInfo(itemdrop);
            });
            onedropitemdrop.GetChild("level").asTextField.text = "lv.1";
            onedropitem.GetChild("droplist").asList.AddChild(onedropitemdrop);
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
        if(p1.BossFreshTime <= 0)
        {
            mapinfo.GetChild("time").asTextField.text = "已经刷新";
        }
        else
        {
            mapinfo.GetChild("time").asTextField.text = Tool.Time2String(p1.BossFreshTime);
        }
        

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
    public bool SC_GotoActivityMap(Protomsg.MsgBase d1)
    {
        Debug.Log("SC_GotoActivityMap:");
        IMessage IMperson = new Protomsg.SC_GotoActivityMap();
        Protomsg.SC_GotoActivityMap p1 = (Protomsg.SC_GotoActivityMap)IMperson.Descriptor.Parser.ParseFrom(d1.Datas);
        if(p1.Result == 1)
        {
            this.Destroy();
        }

        return true;
    }





    //
    public void Destroy()
    {
        MsgManager.Instance.RemoveListener("SC_GetActivityMapsInfo");
        MsgManager.Instance.RemoveListener("SC_GetMapInfo");
        MsgManager.Instance.RemoveListener("SC_GotoActivityMap");
        MsgManager.Instance.RemoveListener("SC_GetDuoBaoInfo");
        MsgManager.Instance.RemoveListener("SC_GetEndlessLevelInfo");
        MsgManager.Instance.RemoveListener("SC_GetWorldMapsInfo");
        


        AudioManager.Am.Play2DSound(AudioManager.Sound_CloseUI);
        if (main != null)
        {
            main.Dispose();
        }
    }

    
}
