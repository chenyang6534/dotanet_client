using cocosocket4unity;
using FairyGUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf;
using System.Threading;
using System;

public class ActivityReward
{
    private GComponent main;
    
    public ActivityReward()
    {

        MsgManager.Instance.AddListener("SC_GetWatchVedioRank", new HandleMsg(this.SC_GetWatchVedioRank));


        main = UIPackage.CreateObject("GameUI", "ActivityReward").asCom;
        GRoot.inst.AddChild(main);
        main.xy = Tool.GetPosition(0.5f, 0.5f);
        main.GetChild("close").asButton.onClick.Add(() =>
        {
            this.Destroy();
        });

        Protomsg.CS_GetWatchVedioRank msg1 = new Protomsg.CS_GetWatchVedioRank();
        msg1.Count = 20;
        MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_GetWatchVedioRank", msg1);


       
    }
  
    public void showrankone(GComponent onedropitem,Protomsg.OneWatchVedioRank item)
    {
        if(item == null)
        {
            return;
        }

        

        var clientitem = ExcelManager.Instance.GetUnitInfoManager().GetUnitInfoByID(item.Typeid);
        if (clientitem == null)
        {
            return;
        }
        onedropitem.GetChild("heroicon").asLoader.url = clientitem.IconPath;
        onedropitem.GetChild("heroicon").onClick.Add(() =>
        {
            new HeroSimpleInfo(item.Characterid);
        });

        onedropitem.GetChild("name").asTextField.text = item.Name;
        var sortpercent = Convert.ToString(item.SortPercent * 100);
        onedropitem.GetChild("rank").asTextField.text = item.Rank + "(" + sortpercent + "%)";
        onedropitem.GetChild("score").asTextField.text = "" + item.Count;

        //奖励
        Debug.Log("rewards:" + item.RewardsStr);
        onedropitem.GetChild("rewardslist").asList.RemoveChildren(0, -1, true);
        var itemarr = item.RewardsStr.Split(';');
        foreach (var item2 in itemarr)
        {
            Debug.Log("item2:" + item2);
            var itemdata = item2.Split(':');
            if (itemdata.Length <= 2)
            {
                continue;
            }
            var itemid = int.Parse(itemdata[0]);
            var itemcount = int.Parse(itemdata[1]);
            var itemlevel = itemdata[2];

            var clientitemone = ExcelManager.Instance.GetItemManager().GetItemByID(itemid);
            if (clientitemone == null)
            {
                continue;
            }

            var threedropitem = UIPackage.CreateObject("GameUI", "sellable").asCom;
            threedropitem.GetChild("icon").asLoader.url = clientitemone.IconPath;
            threedropitem.GetChild("icon").onClick.Add(() =>
            {
                new ItemInfo(itemid,-1, int.Parse(itemlevel));

            });
            threedropitem.GetChild("level").asTextField.text = "Lv." + itemlevel;
            if (clientitemone.ShowLevel == 1)
            {
                threedropitem.GetChild("level").visible = true;
            }
            else
            {
                threedropitem.GetChild("level").visible = false;
            }
            threedropitem.GetChild("count").asTextField.text = itemcount + "";
            if (itemcount <= 1)
            {
                threedropitem.GetChild("count").visible = false;
            }
            else
            {
                threedropitem.GetChild("count").visible = true;
            }
            onedropitem.GetChild("rewardslist").asList.AddChild(threedropitem);
        }
    }

    //获取副本信息
    public bool SC_GetWatchVedioRank(Protomsg.MsgBase d1)
    {
        Debug.Log("SC_GetWatchVedioRank:");
        IMessage IMperson = new Protomsg.SC_GetWatchVedioRank();
        Protomsg.SC_GetWatchVedioRank p1 = (Protomsg.SC_GetWatchVedioRank)IMperson.Descriptor.Parser.ParseFrom(d1.Datas);
        main.GetChild("vediopage").asCom.GetChild("rankllist").asList.RemoveChildren(0, -1, true);

        //遍历
        foreach (var item in p1.All)
        {
            Debug.Log("all:"+item.Name);
            var onedropitem = UIPackage.CreateObject("GameUI", "VedioPageOne").asCom;

            showrankone(onedropitem, item);


            main.GetChild("vediopage").asCom.GetChild("rankllist").asList.AddChild(onedropitem);

        }

        showrankone(main.GetChild("vediopage").asCom.GetChild("my").asCom, p1.My);

        return true;
    }
    
    

    //
    public void Destroy()
    {
        MsgManager.Instance.RemoveListener("SC_GetWatchVedioRank");

        AudioManager.Am.Play2DSound(AudioManager.Sound_CloseUI);
        if (main != null)
        {
            main.Dispose();
        }
    }

    
}
