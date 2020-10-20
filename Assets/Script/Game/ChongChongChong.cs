using cocosocket4unity;
using FairyGUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf;
using System.Threading;
using System;

public class ChongChongChong
{
    private GComponent main;
    
    public ChongChongChong()
    {

        MsgManager.Instance.AddListener("SC_GetChongChongChong", new HandleMsg(this.SC_GetChongChongChong));
        MsgManager.Instance.AddListener("SC_BuyChongChongChong", new HandleMsg(this.SC_BuyChongChongChong));
        MsgManager.Instance.AddListener("SC_GetMonthCard", new HandleMsg(this.SC_GetMonthCard));

        main = UIPackage.CreateObject("GameUI", "ChongChongChong").asCom;
        GRoot.inst.AddChild(main);
        main.xy = Tool.GetPosition(0.5f, 0.5f);
        main.GetChild("close").asButton.onClick.Add(() =>
        {
            this.Destroy();
        });

        Protomsg.CS_GetChongChongChong msg1 = new Protomsg.CS_GetChongChongChong();
        MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_GetChongChongChong", msg1);

        main.GetChild("pay1").asButton.onClick.Add(() =>
        {
            Protomsg.CS_GetChongChongChong msg = new Protomsg.CS_GetChongChongChong();
            MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_GetChongChongChong", msg);
        });
        main.GetChild("month").asButton.onClick.Add(() =>
        {
            Protomsg.CS_GetMonthCard msg = new Protomsg.CS_GetMonthCard();
            MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_GetMonthCard", msg);
        });

        init();

    }

    public void init()
    {
        
    }
    public void showchongone(GComponent onedropitem, Protomsg.OneChongChongChong item)
    {
        if (item == null)
        {
            return;
        }
        onedropitem.GetChild("onebtn").asButton.title = item.Money + "元";
        onedropitem.GetChild("onebtn").asButton.onClick.Add(() =>
        {
            //充
            Protomsg.CS_DoChongChongChong msg1 = new Protomsg.CS_DoChongChongChong();
            msg1.ID = item.ID;
            MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_DoChongChongChong", msg1);
            Debug.Log("CS_DoChongChongChong:" + item.ID);
        });

        //奖励
        Debug.Log("rewards:" + item.RewardsStr);
        onedropitem.GetChild("itemlist").asList.RemoveChildren(0, -1, true);
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

            onedropitem.GetChild("itemlist").asList.AddChild(threedropitem);
        }
    }
    public void ShowFirstRewards(string rewardsstr)
    {
        //奖励
        Debug.Log("ShowFirstRewards:" + rewardsstr);
        main.GetChild("chong1page").asCom.GetChild("firstlist").asList.RemoveChildren(0, -1, true);
        var itemarr = rewardsstr.Split(';');
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

            main.GetChild("chong1page").asCom.GetChild("firstlist").asList.AddChild(threedropitem);
        }
    }
    public bool SC_GetChongChongChong(Protomsg.MsgBase d1)
    {
        Debug.Log("SC_GetChongChongChong:");
        IMessage IMperson = new Protomsg.SC_GetChongChongChong();
        Protomsg.SC_GetChongChongChong p1 = (Protomsg.SC_GetChongChongChong)IMperson.Descriptor.Parser.ParseFrom(d1.Datas);

        main.GetChild("chong1page").asCom.GetChild("list").asList.RemoveChildren(0, -1, true);


        Protomsg.OneChongChongChong[] allplayer = new Protomsg.OneChongChongChong[p1.All.Count];
        p1.All.CopyTo(allplayer, 0);
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
            Debug.Log("all:" + item.RewardsStr);
            var onedropitem = UIPackage.CreateObject("GameUI", "Chong1").asCom;

            showchongone(onedropitem, item);


            main.GetChild("chong1page").asCom.GetChild("list").asList.AddChild(onedropitem);

        }

        //显示首次奖励
        ShowFirstRewards(p1.FirstPayRewardsStr);

        return true;
    }


    public void showmonthone(GComponent onedropitem, Protomsg.OneMonthCard item)
    {
        if (item == null)
        {
            return;
        }
        //购买
        onedropitem.GetChild("buy").asButton.title = item.Money + "元";
        onedropitem.GetChild("buy").asButton.onClick.Add(() =>
        {
            //充
            Protomsg.CS_DoChongChongChong msg1 = new Protomsg.CS_DoChongChongChong();
            msg1.ID = item.ID;
            MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_DoChongChongChong", msg1);
            Debug.Log("CS_DoChongChongChong:" + item.ID);
        });
        //领奖
        onedropitem.GetChild("get").asButton.onClick.Add(() =>
        {
            //充
            Protomsg.CS_GetMonthCardReward msg1 = new Protomsg.CS_GetMonthCardReward();
            msg1.ID = item.ID;
            MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_GetMonthCardReward", msg1);
            Debug.Log("CS_DoChongChongChong:" + item.ID);
        });

        //剩余次数
        onedropitem.GetChild("remiancount").asTextField.SetVar("p1", "" + item.RemainCount);
        onedropitem.GetChild("remiancount").asTextField.FlushVars();

        onedropitem.GetChild("get").visible = false;
        if (item.RemainCount > 0)
        {
            onedropitem.GetChild("buy").visible = false;
            if (item.CanGetToday == true)
            {
                onedropitem.GetChild("get").visible = true;
            }
        }
        

        //奖励
        Debug.Log("rewards:" + item.RewardsStr);
        onedropitem.GetChild("itemlist").asList.RemoveChildren(0, -1, true);
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

            onedropitem.GetChild("itemlist").asList.AddChild(threedropitem);
        }
    }
    //SC_GetMonthCard
    public bool SC_GetMonthCard(Protomsg.MsgBase d1)
    {
        Debug.Log("SC_GetMonthCard:");
        IMessage IMperson = new Protomsg.SC_GetMonthCard();
        Protomsg.SC_GetMonthCard p1 = (Protomsg.SC_GetMonthCard)IMperson.Descriptor.Parser.ParseFrom(d1.Datas);
        main.GetChild("monthcard").asCom.GetChild("list").asList.RemoveChildren(0, -1, true);


        Protomsg.OneMonthCard[] allplayer = new Protomsg.OneMonthCard[p1.All.Count];
        p1.All.CopyTo(allplayer, 0);
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
            Debug.Log("all1:" + item.RewardsStr);
            var onedropitem = UIPackage.CreateObject("GameUI", "Chong2").asCom;

            showmonthone(onedropitem, item);


            main.GetChild("monthcard").asCom.GetChild("list").asList.AddChild(onedropitem);

        }

        return true;
    }

    public bool SC_BuyChongChongChong(Protomsg.MsgBase d1)
    {
        Debug.Log("SC_BuyChongChongChong:");
        IMessage IMperson = new Protomsg.SC_BuyChongChongChong();
        Protomsg.SC_BuyChongChongChong p1 = (Protomsg.SC_BuyChongChongChong)IMperson.Descriptor.Parser.ParseFrom(d1.Datas);

        if(p1.Code == 1)//成功
        {
            //打开连接
            Debug.Log("SC_BuyChongChongChong:");
            Application.OpenURL(p1.DataUrl);
            Debug.Log("SC_BuyChongChongChong:"+ p1.DataUrl);
        }
        else
        {
            Debug.Log("SC_BuyChongChongChong failed:" + p1.DataUrl);
        }

        return true;
    }


    //
    public void Destroy()
    {
        MsgManager.Instance.RemoveListener("SC_GetChongChongChong");
        MsgManager.Instance.RemoveListener("SC_BuyChongChongChong");
        MsgManager.Instance.RemoveListener("SC_GetMonthCard");
        

        AudioManager.Am.Play2DSound(AudioManager.Sound_CloseUI);
        if (main != null)
        {
            main.Dispose();
        }
    }

    
}
