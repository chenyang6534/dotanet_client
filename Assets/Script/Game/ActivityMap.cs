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

        main = UIPackage.CreateObject("GameUI", "ActivityMap").asCom;
        GRoot.inst.AddChild(main);
        main.xy = Tool.GetPosition(0.5f, 0.5f);
        main.GetChild("close").asButton.onClick.Add(() =>
        {
            this.Destroy();
        });

        Protomsg.CS_GetActivityMapsInfo msg1 = new Protomsg.CS_GetActivityMapsInfo();
        MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_GetActivityMapsInfo", msg1);

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
        AudioManager.Am.Play2DSound(AudioManager.Sound_CloseUI);
        if (main != null)
        {
            main.Dispose();
        }
    }

    
}
