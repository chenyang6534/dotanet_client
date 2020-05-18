using cocosocket4unity;
using FairyGUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf;
using System.Threading;
using System;

public class Battle
{
    private GComponent main;
    
    public Battle()
    {

        MsgManager.Instance.AddListener("SC_GetBattleRankInfo", new HandleMsg(this.SC_GetBattleRankInfo));
        MsgManager.Instance.AddListener("SC_GetBattleMapInfo", new HandleMsg(this.SC_GetBattleMapInfo));

        main = UIPackage.CreateObject("GameUI", "BattleInfo").asCom;
        GRoot.inst.AddChild(main);
        main.xy = Tool.GetPosition(0.5f, 0.5f);
        main.GetChild("close").asButton.onClick.Add(() =>
        {
            this.Destroy();
        });

        Protomsg.CS_GetBattleRankInfo msg1 = new Protomsg.CS_GetBattleRankInfo();
        msg1.RankStart = 0;
        msg1.RankCount = 100;
        MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_GetBattleRankInfo", msg1);

        Protomsg.CS_GetBattleMapInfo msg = new Protomsg.CS_GetBattleMapInfo();
        MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_GetBattleMapInfo", msg);

    }
  

    //获取副本信息
    public bool SC_GetBattleRankInfo(Protomsg.MsgBase d1)
    {
        Debug.Log("SC_GetBattleRankInfo:");
        IMessage IMperson = new Protomsg.SC_GetBattleRankInfo();
        Protomsg.SC_GetBattleRankInfo p1 = (Protomsg.SC_GetBattleRankInfo)IMperson.Descriptor.Parser.ParseFrom(d1.Datas);
        main.GetChild("rankllist").asList.RemoveChildren(0, -1, true);
        Debug.Log("p1.RankInfo.Count:" + p1.RankInfo.Count);

        Protomsg.BattleRankOneInfo[] allplayer = new Protomsg.BattleRankOneInfo[p1.RankInfo.Count];
        p1.RankInfo.CopyTo(allplayer, 0);
        System.Array.Sort(allplayer, (a, b) => {
            if(a.Rank > b.Rank)
            {
                return 1;
            }
            return -1;
        });

        //遍历
        foreach (var item in allplayer)
        {
            
            var clientitem = ExcelManager.Instance.GetUnitInfoManager().GetUnitInfoByID(item.Typeid);
            if (clientitem == null)
            {
                continue;
            }

            var onedropitem = UIPackage.CreateObject("GameUI", "BattleInfoOne").asCom;

            onedropitem.GetChild("heroicon").asLoader.url = clientitem.IconPath;
            onedropitem.GetChild("name").asTextField.text = item.Name;
            onedropitem.GetChild("rank").asTextField.text = item.Rank+"";
            onedropitem.GetChild("score").asTextField.text = item.Score + "";
            
            main.GetChild("rankllist").asList.AddChild(onedropitem);

        }

        //自己
        var myitem = ExcelManager.Instance.GetUnitInfoManager().GetUnitInfoByID(p1.MyRankInfo.Typeid);
        if (myitem == null)
        {
            return true;
        }

        var mybattleinfo = main.GetChild("mybattleinfo").asCom;

        mybattleinfo.GetChild("heroicon").asLoader.url = myitem.IconPath;
        mybattleinfo.GetChild("name").asTextField.text = p1.MyRankInfo.Name;
        mybattleinfo.GetChild("rank").asTextField.text = p1.MyRankInfo.Rank + "";
        mybattleinfo.GetChild("score").asTextField.text = p1.MyRankInfo.Score + "";
        if (p1.MyRankInfo.Rank < 0)
        {
            mybattleinfo.GetChild("rank").asTextField.text = "--";
            mybattleinfo.GetChild("score").asTextField.text = "--";
        }



        return true;
    }
    

    public bool SC_GetBattleMapInfo(Protomsg.MsgBase d1)
    {
        Debug.Log("SC_GetBattleMapInfo:");
        IMessage IMperson = new Protomsg.SC_GetBattleMapInfo();
        Protomsg.SC_GetBattleMapInfo p1 = (Protomsg.SC_GetBattleMapInfo)IMperson.Descriptor.Parser.ParseFrom(d1.Datas);

        main.GetChild("guildlevel").asTextField.text = "Lv."+p1.BattleMapInfo.NeedLevel;
        //进入
        main.GetChild("pipei").asButton.onClick.Add(() =>
        {
            Protomsg.CS_BattlePiPei msg1 = new Protomsg.CS_BattlePiPei();
            msg1.CopyMapID = p1.BattleMapInfo.ID;
            MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_BattlePiPei", msg1);
        });

        //地图信息
        main.GetChild("cancel").asButton.onClick.Add(() =>
        {
            Protomsg.CS_BattleCancel msg1 = new Protomsg.CS_BattleCancel();
            MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_BattleCancel", msg1);
        });
        
        if (p1.BattleMapInfo.State == 1)//可以匹配
        {
            Controller ct = main.GetController("c1");
            ct.SetSelectedIndex(0);

        }
        else if (p1.BattleMapInfo.State == 2)//匹配中
        {
            Controller ct = main.GetController("c1");
            ct.SetSelectedIndex(1);
        }
        return true;
    }
   





    //
    public void Destroy()
    {
        MsgManager.Instance.RemoveListener("SC_GetBattleRankInfo");
        MsgManager.Instance.RemoveListener("SC_GetBattleMapInfo");

        AudioManager.Am.Play2DSound(AudioManager.Sound_CloseUI);
        if (main != null)
        {
            main.Dispose();
        }
    }

    
}
