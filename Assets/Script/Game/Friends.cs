using cocosocket4unity;
using FairyGUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf;
public class Friends : MonoBehaviour {
    protected GComponent FriendsCom;//好友界面
    static Friends sInstanse = null;
    // Use this for initialization
    void Start () {
        sInstanse = this;
        CreateChatBox();
        MsgManager.Instance.AddListener("SC_GetFriendsList", new HandleMsg(this.SC_GetFriendsList));
    }
    void OnDestroy()
    {
        MsgManager.Instance.RemoveListener("SC_GetFriendsList");
    }
    public bool SC_GetFriendsList(Protomsg.MsgBase d1)
    {
        Debug.Log("SC_GetFriendsList:");
        IMessage IMperson = new Protomsg.SC_GetFriendsList();
        Protomsg.SC_GetFriendsList p2 = (Protomsg.SC_GetFriendsList)IMperson.Descriptor.Parser.ParseFrom(d1.Datas);

        var list = FriendsCom.GetChild("list").asList;
        list.RemoveChildren();

        foreach (var p1 in p2.FriendsRequest)
        {
            var teamrequest = UIPackage.CreateObject("GameUI", "FriendRequest").asCom;
            list.AddChild(teamrequest);
            AudioManager.Am.Play2DSound(AudioManager.Sound_OpenLittleUI);
            //SrcUnitTypeID
            var clientitem = ExcelManager.Instance.GetUnitInfoManager().GetUnitInfoByID(p1.Typeid);
            if (clientitem != null)
            {
                teamrequest.GetChild("headicon").asLoader.url = clientitem.IconPath;
            }
            teamrequest.GetChild("name").asTextField.text = p1.Name;
            teamrequest.GetChild("level").asTextField.text = p1.Level + "";

            teamrequest.GetChild("no_btn").asButton.onClick.Add(() =>
            {
                //回复拒绝好友请求
                Protomsg.CS_AddFriendResponse msg = new Protomsg.CS_AddFriendResponse();
                msg.Result = 2;//1同意  2拒绝
                msg.FriendInfo = p1;
                MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_AddFriendResponse", msg);

                list.RemoveChild(teamrequest);
            });

            teamrequest.GetChild("yes_btn").asButton.onClick.Add(() =>
            {
                //回复同意组队请求
                Protomsg.CS_AddFriendResponse msg = new Protomsg.CS_AddFriendResponse();
                msg.Result = 1;//1同意  2拒绝
                msg.FriendInfo = p1;
                MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_AddFriendResponse", msg);
                list.RemoveChild(teamrequest);
            });
        }

        foreach (var p1 in p2.Friends)
        {
            var teamrequest = UIPackage.CreateObject("GameUI", "FriendOne").asCom;
            list.AddChild(teamrequest);
            AudioManager.Am.Play2DSound(AudioManager.Sound_OpenLittleUI);
            //SrcUnitTypeID
            var clientitem = ExcelManager.Instance.GetUnitInfoManager().GetUnitInfoByID(p1.Typeid);
            if (clientitem != null)
            {
                teamrequest.GetChild("headicon").asLoader.url = clientitem.IconPath;
            }
            teamrequest.GetChild("name").asTextField.text = p1.Name;
            teamrequest.GetChild("level").asTextField.text = p1.Level + "";
            if( p1.State == 1)
            {
                //在线
                teamrequest.GetChild("discript").asTextField.text = "在线";
                teamrequest.GetChild("discript").asTextField.color = new Color(1, 1, 1);
            }
            else
            {
                //离线
                teamrequest.GetChild("discript").asTextField.text = "离线";
                teamrequest.GetChild("discript").asTextField.color = new Color(0.2f, 0.2f, 0.2f);
            }
        }

        return true;
    }

    //创建聊天界面
    public void CreateChatBox()
    {
        if (FriendsCom == null)
        {
            FriendsCom = UIPackage.CreateObject("GameUI", "FriendMain").asCom;
            GRoot.inst.AddChild(FriendsCom);
            FriendsCom.xy = Tool.GetPosition(1f, 0.5f);


            //
            FriendsCom.visible = false;
            FriendsCom.GetChild("close").onClick.Add(() => {
                FriendsCom.visible = false;
            });

            
        }
    }


    //打开聊天界面
    public static void SOpen()
    {
        if(sInstanse != null)
        {
            sInstanse.FriendsCom.visible = true;
            //解析分隔数据
            Protomsg.CS_GetFriendsList msg = new Protomsg.CS_GetFriendsList();
            MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_GetFriendsList", msg);
            
        }
    }

    

    // Update is called once per frame
    void Update () {
		
	}
}
