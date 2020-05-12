using cocosocket4unity;
using FairyGUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf;
using System.Threading;
using System;

public class CopyMap
{
    private GComponent main;
    
    public CopyMap()
    {

        MsgManager.Instance.AddListener("SC_GetCopyMapsInfo", new HandleMsg(this.SC_GetCopyMapsInfo));
        MsgManager.Instance.AddListener("SC_GetMapInfo", new HandleMsg(this.SC_GetMapInfo));

        main = UIPackage.CreateObject("GameUI", "CopyMap").asCom;
        GRoot.inst.AddChild(main);
        main.xy = Tool.GetPosition(0.5f, 0.5f);
        main.GetChild("close").asButton.onClick.Add(() =>
        {
            this.Destroy();
        });

        Protomsg.CS_GetCopyMapsInfo msg1 = new Protomsg.CS_GetCopyMapsInfo();
        MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_GetCopyMapsInfo", msg1);


       
    }
  

    //获取副本信息
    public bool SC_GetCopyMapsInfo(Protomsg.MsgBase d1)
    {
        Debug.Log("SC_GetCopyMapsInfo:");
        IMessage IMperson = new Protomsg.SC_GetCopyMapsInfo();
        Protomsg.SC_GetCopyMapsInfo p1 = (Protomsg.SC_GetCopyMapsInfo)IMperson.Descriptor.Parser.ParseFrom(d1.Datas);
        main.GetChild("maplist").asList.RemoveChildren(0, -1, true);

        main.GetChild("playcount").asTextField.SetVar("p1", p1.RemainPlayTimes+"");
        main.GetChild("playcount").asTextField.FlushVars();

        Protomsg.CopyMapInfo[] allplayer = new Protomsg.CopyMapInfo[p1.Maps.Count];
        p1.Maps.CopyTo(allplayer, 0);
        System.Array.Sort(allplayer, (a, b) => {

            if (a.State > b.State)
            {
                return -1;
            }
            else if (a.State == b.State)
            {
                if(a.NeedLevel > b.NeedLevel)
                {
                    return -1;
                }
                return 1;
            }
            return 1;
        });

        //遍历
        foreach (var item in allplayer)
        {
            var clientitem = ExcelManager.Instance.GetSceneManager().GetSceneByID(item.NextSceneID);
            if (clientitem == null)
            {
                continue;
            }

            var onedropitem = UIPackage.CreateObject("GameUI", "CopyMapInfo").asCom;

            //onedropitem.GetChild("item").asCom.GetChild("icon").asLoader.url = clientitem.IconPath;
            onedropitem.GetChild("name").asTextField.text = clientitem.Name;
            onedropitem.GetChild("guildlevel").asTextField.text = item.NeedLevel + "";
            onedropitem.GetChild("playercount").asTextField.text = item.PlayerCount + "";
            
            //进入
            onedropitem.GetChild("pipei").asButton.onClick.Add(() =>
            {
                Protomsg.CS_CopyMapPiPei msg1 = new Protomsg.CS_CopyMapPiPei();
                msg1.CopyMapID = item.ID;
                MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_CopyMapPiPei", msg1);
            });

            //地图信息
            onedropitem.GetChild("cancel").asButton.onClick.Add(() =>
            {
                Protomsg.CS_CopyMapCancel msg1 = new Protomsg.CS_CopyMapCancel();
                MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_CopyMapCancel", msg1);
            });
            //地图信息
            onedropitem.GetChild("icon").asLoader.onClick.Add(() =>
            {
                Protomsg.CS_GetMapInfo msg1 = new Protomsg.CS_GetMapInfo();
                msg1.SceneID = item.NextSceneID;
                MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_GetMapInfo", msg1);
            });

            if (item.State == 1)//可以匹配
            {
                Controller ct = onedropitem.GetController("c1");
                ct.SetSelectedIndex(0);

            }
            else if(item.State == 2)//匹配中
            {
                Controller ct = onedropitem.GetController("c1");
                ct.SetSelectedIndex(1);
            }


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
   





    //
    public void Destroy()
    {
        MsgManager.Instance.RemoveListener("SC_GetCopyMapsInfo");
        MsgManager.Instance.RemoveListener("SC_GetMapInfo");

        AudioManager.Am.Play2DSound(AudioManager.Sound_CloseUI);
        if (main != null)
        {
            main.Dispose();
        }
    }

    
}
