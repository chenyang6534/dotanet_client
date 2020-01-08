using cocosocket4unity;
using FairyGUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf;
using System;

public class Mails : MonoBehaviour {
    protected GComponent FriendsCom;//好友界面
    static Mails sInstanse = null;
    protected int ShowID = 0;
    // Use this for initialization
    void Start () {
        sInstanse = this;
        CreateChatBox();
        MsgManager.Instance.AddListener("SC_GetMailsList", new HandleMsg(this.SC_GetMailsList));
        MsgManager.Instance.AddListener("SC_GetMailInfo", new HandleMsg(this.SC_GetMailInfo));
        MsgManager.Instance.AddListener("SC_GetMailRewards", new HandleMsg(this.SC_GetMailRewards));
    }
    void OnDestroy()
    {
        MsgManager.Instance.RemoveListener("SC_GetMailsList");
        MsgManager.Instance.RemoveListener("SC_GetMailInfo");
        MsgManager.Instance.RemoveListener("SC_GetMailRewards");
    }
    public bool SC_GetMailRewards(Protomsg.MsgBase d1)
    {
        Debug.Log("SC_GetMailRewards:");
        IMessage IMperson = new Protomsg.SC_GetMailRewards();
        Protomsg.SC_GetMailRewards p2 = (Protomsg.SC_GetMailRewards)IMperson.Descriptor.Parser.ParseFrom(d1.Datas);
        //1表示成功 0表示失败
        if (p2.Result == 1)
        {
            Tool.NoticeWords("领取成功", null);
            FriendsCom.GetChild("get").visible = false;
        }
        else
        {
            Tool.NoticeWords("领取失败", null);
        }

        return true;
    }
    public bool SC_GetMailInfo(Protomsg.MsgBase d1)
    {
        Debug.Log("SC_GetMailInfo:");
        IMessage IMperson = new Protomsg.SC_GetMailInfo();
        Protomsg.SC_GetMailInfo p2 = (Protomsg.SC_GetMailInfo)IMperson.Descriptor.Parser.ParseFrom(d1.Datas);
        this.ShowID = p2.Id;
        var group = FriendsCom.GetChild("content").asGroup;
        group.visible = true;

        FriendsCom.GetChild("title").asTextField.text = p2.Title;
        FriendsCom.GetChild("day").asTextField.text = p2.Date;
        FriendsCom.GetChild("sendname").asTextField.text = p2.SendName;
        FriendsCom.GetChild("contentwords").asTextField.text = p2.Content;
        //领取状态 0表示未领取，1表示已领取
        if (p2.State == 1)
        {
            FriendsCom.GetChild("get").visible = false;
        }
        else
        {
            FriendsCom.GetChild("get").visible = true;
        }

        var list = FriendsCom.GetChild("rewardslist").asList;
        list.RemoveChildren();

        foreach (var p1 in p2.Rewards)
        {
            var teamrequest = UIPackage.CreateObject("GameUI", "Reward").asCom;
            list.AddChild(teamrequest);
            teamrequest.onClick.Add(() => {
                new ItemInfo(p1.ItemType);
            });
            var clientitem = ExcelManager.Instance.GetItemManager().GetItemByID(p1.ItemType);
            if (clientitem != null)
            {
                teamrequest.GetChild("icon").asLoader.url = clientitem.IconPath;
            }
            teamrequest.GetChild("count").asTextField.text = p1.Count+"";
        }



        return true;
    }
    public bool SC_GetMailsList(Protomsg.MsgBase d1)
    {
        Debug.Log("SC_GetMailsList:");
        IMessage IMperson = new Protomsg.SC_GetMailsList();
        Protomsg.SC_GetMailsList p2 = (Protomsg.SC_GetMailsList)IMperson.Descriptor.Parser.ParseFrom(d1.Datas);

        var list = FriendsCom.GetChild("list").asList;
        list.RemoveChildren();

        
        //处理排序
        Protomsg.MailShortInfoMsg[] allplayer = new Protomsg.MailShortInfoMsg[p2.Mails.Count];
        int index = 0;
        foreach (var item in p2.Mails)
        {
            allplayer[index++] = item;
        }
        //排序
        System.Array.Sort(allplayer, (s1, s2) => {
            if (s1.Id > s2.Id)
            {
                return 1;
            }else if(s1.Id == s2.Id)
            {
                return 0;
            }
            return -1;
        });

        foreach (var p1 in allplayer)
        {
            var teamrequest = UIPackage.CreateObject("GameUI", "MailOne").asCom;
            list.AddChild(teamrequest);
            AudioManager.Am.Play2DSound(AudioManager.Sound_OpenLittleUI);
            
            teamrequest.GetChild("title").asTextField.text = p1.Title;
            teamrequest.GetChild("day").asTextField.text = p1.Date;
            teamrequest.GetChild("sendname").asTextField.text = p1.SendName;
            //领取状态 0表示未领取，1表示已领取
            if ( p1.State == 1)
            {
                teamrequest.GetChild("bg").asImage.color = new Color(0.5f, 0.5f, 0.5f);
            }
            else
            {
                teamrequest.GetChild("bg").asImage.color = new Color(1, 1, 1);
            }
            //查看详细信息
            teamrequest.onClick.Add(() => {
                //解析分隔数据
                Protomsg.CS_GetMailInfo msg = new Protomsg.CS_GetMailInfo();
                msg.Id = p1.Id;
                MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_GetMailInfo", msg);
            });

        }

        return true;
    }

    //创建聊天界面
    public void CreateChatBox()
    {
        if (FriendsCom == null)
        {
            FriendsCom = UIPackage.CreateObject("GameUI", "MailsMain").asCom;
            GRoot.inst.AddChild(FriendsCom);
            FriendsCom.xy = Tool.GetPosition(0.5f, 0.5f);


            //
            FriendsCom.visible = false;
            FriendsCom.GetChild("close").onClick.Add(() => {
                FriendsCom.visible = false;
            });

            FriendsCom.GetChild("count").visible = false;

            FriendsCom.GetChild("get").onClick.Add(() => {
                //解析分隔数据
                Protomsg.CS_GetMailRewards msg = new Protomsg.CS_GetMailRewards();
                msg.Id = this.ShowID;
                MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_GetMailRewards", msg);
            });

        }
    }


    //打开聊天界面
    public static void SOpen()
    {
        if(sInstanse != null)
        {
            sInstanse.FriendsCom.visible = true;
            sInstanse.FriendsCom.parent.AddChild(sInstanse.FriendsCom);

            sInstanse.FriendsCom.GetChild("content").asGroup.visible = false;
            //解析分隔数据
            Protomsg.CS_GetMailsList msg = new Protomsg.CS_GetMailsList();
            MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_GetMailsList", msg);
            
        }
    }

    

    // Update is called once per frame
    void Update () {
		
	}
}
