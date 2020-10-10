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
        MsgManager.Instance.AddListener("SC_GetLuckDrawUIData", new HandleMsg(this.SC_GetLuckDrawUIData));
        MsgManager.Instance.AddListener("SC_GetLuckDraw", new HandleMsg(this.SC_GetLuckDraw));
        MsgManager.Instance.AddListener("SC_GetLuckDrawRank", new HandleMsg(this.SC_GetLuckDrawRank));

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

        main.GetChild("vedio").asButton.onClick.Add(() =>
        {
            Protomsg.CS_GetWatchVedioRank msg = new Protomsg.CS_GetWatchVedioRank();
            msg.Count = 20;
            MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_GetWatchVedioRank", msg);
        });
        main.GetChild("luckdraw").asButton.onClick.Add(() =>
        {
            Protomsg.CS_GetLuckDrawUIData msg = new Protomsg.CS_GetLuckDrawUIData();
            MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_GetLuckDrawUIData", msg);
        });

        init();

    }

    public void init()
    {
        main.GetChild("luckdrawpage").asCom.GetChild("rankbtn").onClick.Set(() => {
            //排行奖励
            Protomsg.CS_GetLuckDrawRank msg = new Protomsg.CS_GetLuckDrawRank();
            MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_GetLuckDrawRank", msg);
        });
        main.GetChild("luckdrawpage").asCom.GetChild("onebtn").onClick.Set(() => {
            //单抽
            Protomsg.CS_GetLuckDraw msg = new Protomsg.CS_GetLuckDraw();
            msg.Count = 1;
            MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_GetLuckDraw", msg);
        });
        main.GetChild("luckdrawpage").asCom.GetChild("tenbtn").onClick.Set(() => {
            //十连抽
            Protomsg.CS_GetLuckDraw msg = new Protomsg.CS_GetLuckDraw();
            msg.Count = 10;
            MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_GetLuckDraw", msg);
        });
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
                threedropitem.GetChild("count").asTextField.text = "";
            }
            else
            {
                threedropitem.GetChild("level").visible = false;
                threedropitem.GetChild("count").asTextField.text = itemcount + "";
            }
            
            onedropitem.GetChild("rewardslist").asList.AddChild(threedropitem);
        }
    }

    //
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

    public bool SC_GetLuckDrawUIData(Protomsg.MsgBase d1)
    {
        Debug.Log("SC_GetLuckDrawUIData:");
        IMessage IMperson = new Protomsg.SC_GetLuckDrawUIData();
        Protomsg.SC_GetLuckDrawUIData p2 = (Protomsg.SC_GetLuckDrawUIData)IMperson.Descriptor.Parser.ParseFrom(d1.Datas);


        //价格
        main.GetChild("luckdrawpage").asCom.GetChild("pricetype").asLoader.url = Tool.GetPriceTypeIcon(p2.PriceType);
        main.GetChild("luckdrawpage").asCom.GetChild("price").asTextField.text = p2.Price + "";

        main.GetChild("luckdrawpage").asCom.GetChild("tenpricetype").asLoader.url = Tool.GetPriceTypeIcon(p2.PriceType);
        main.GetChild("luckdrawpage").asCom.GetChild("tenprice").asTextField.text = Convert.ToInt32(p2.Price*10*p2.TenDiscount) + "";


        var list = main.GetChild("luckdrawpage").asCom.GetChild("itemlist").asList;
        list.RemoveChildren(0, -1, true);

        foreach (var p1 in p2.Rewards)
        {
            var teamrequest = UIPackage.CreateObject("GameUI", "Reward").asCom;
            list.AddChild(teamrequest);
            teamrequest.onClick.Add(() => {
                new ItemInfo(p1.ItemType, p1.ItemDBID, p1.Level);
            });
            var clientitem = ExcelManager.Instance.GetItemManager().GetItemByID(p1.ItemType);
            if (clientitem != null)
            {
                teamrequest.GetChild("icon").asLoader.url = clientitem.IconPath;
                if (clientitem.ShowLevel == 1)
                {
                    teamrequest.GetChild("level").visible = true;
                    teamrequest.GetChild("count").asTextField.text = "";
                }
                else
                {
                    teamrequest.GetChild("level").visible = false;
                    teamrequest.GetChild("count").asTextField.text = p1.Count + "";
                }
            }
            //if (p1.Count <= 1)
            //{
            //    teamrequest.GetChild("count").asTextField.text = "";
            //}
            //else
            //{
            //    teamrequest.GetChild("count").asTextField.text = p1.Count + "";
            //}

            teamrequest.GetChild("level").asTextField.text = "Lv." + p1.Level + "";
        }
        return true;
    }
    public bool SC_GetLuckDraw(Protomsg.MsgBase d1)
    {
        Debug.Log("SC_GetLuckDraw:");
        IMessage IMperson = new Protomsg.SC_GetLuckDraw();
        Protomsg.SC_GetLuckDraw p1 = (Protomsg.SC_GetLuckDraw)IMperson.Descriptor.Parser.ParseFrom(d1.Datas);
        Protomsg.MailRewards[] allplayer = new Protomsg.MailRewards[p1.Rewards.Count];
        p1.Rewards.CopyTo(allplayer, 0);
        new GetRewardListNotice(allplayer);

        return true;
    }


    public void showluckdrawrankone(GComponent onedropitem, Protomsg.OneLuckDrawRank item)
    {
        if (item == null)
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
        onedropitem.GetChild("rank").asTextField.text = item.Rank + "";
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
                Debug.Log("item2 len:" + itemdata.Length);
                continue;
            }
            var itemid = int.Parse(itemdata[0]);
            var itemcount = int.Parse(itemdata[1]);
            var itemlevel = itemdata[2];

            var clientitemone = ExcelManager.Instance.GetItemManager().GetItemByID(itemid);
            if (clientitemone == null)
            {
                Debug.Log("item2 itemid:" + itemid);
                continue;
            }

            var threedropitem = UIPackage.CreateObject("GameUI", "sellable").asCom;
            threedropitem.GetChild("icon").asLoader.url = clientitemone.IconPath;
            threedropitem.GetChild("icon").onClick.Add(() =>
            {
                new ItemInfo(itemid, -1, int.Parse(itemlevel));

            });
            threedropitem.GetChild("level").asTextField.text = "Lv." + itemlevel;
            if (clientitemone.ShowLevel == 1)
            {
                threedropitem.GetChild("level").visible = true;
                threedropitem.GetChild("count").asTextField.text = "";
            }
            else
            {
                threedropitem.GetChild("level").visible = false;
                threedropitem.GetChild("count").asTextField.text = itemcount + "";
            }
            
            
            onedropitem.GetChild("rewardslist").asList.AddChild(threedropitem);
        }
    }
    //SC_GetLuckDrawRank
    public bool SC_GetLuckDrawRank(Protomsg.MsgBase d1)
    {
        Debug.Log("SC_GetLuckDrawRank:");
        IMessage IMperson = new Protomsg.SC_GetLuckDrawRank();
        Protomsg.SC_GetLuckDrawRank p1 = (Protomsg.SC_GetLuckDrawRank)IMperson.Descriptor.Parser.ParseFrom(d1.Datas);
        var mapinfo = UIPackage.CreateObject("GameUI", "LuckDrawRank").asCom;
        GRoot.inst.AddChild(mapinfo);
        mapinfo.xy = Tool.GetPosition(0.5f, 0.5f);
        mapinfo.GetChild("close").asButton.onClick.Add(() =>
        {
            mapinfo.Dispose();
        });

        mapinfo.GetChild("rankllist").asList.RemoveChildren(0, -1, true);

        //遍历
        foreach (var item in p1.All)
        {
            Debug.Log("all:" + item.Name);
            var onedropitem = UIPackage.CreateObject("GameUI", "VedioPageOne").asCom;

            showluckdrawrankone(onedropitem, item);


            mapinfo.GetChild("rankllist").asList.AddChild(onedropitem);

        }

        showluckdrawrankone(mapinfo.GetChild("my").asCom, p1.My);

        return true;
    }
    //
    public void Destroy()
    {
        MsgManager.Instance.RemoveListener("SC_GetWatchVedioRank");
        MsgManager.Instance.RemoveListener("SC_GetLuckDrawUIData");
        MsgManager.Instance.RemoveListener("SC_GetLuckDraw");
        MsgManager.Instance.RemoveListener("SC_GetLuckDrawRank");
        

        AudioManager.Am.Play2DSound(AudioManager.Sound_CloseUI);
        if (main != null)
        {
            main.Dispose();
        }
    }

    
}
