using cocosocket4unity;
using FairyGUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf;
using System;

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

        //处理排序
        Protomsg.FriendInfoMsg[] allplayer = new Protomsg.FriendInfoMsg[p2.Friends.Count];
        int index = 0;
        foreach (var item in p2.Friends)
        {
            allplayer[index++] = item;
        }
        //排序
        System.Array.Sort(allplayer, (s1, s2) => {
            if (s1.State > s2.State)
            {
                return 1;
            }else if(s1.State == s2.State)
            {
                return 0;
            }
            return -1;
        });

        foreach (var p1 in allplayer)
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

                teamrequest.onClick.Add(() => {
                    //onlinefriendclick
                    var headselect = UIPackage.CreateObject("GameUI", "onlinefriendclick").asCom;
                    GRoot.inst.ShowPopup(headselect);
                    headselect.GetChild("siliao").asButton.onClick.Add(() =>
                    {
                        ChatUI.SOpenChatBox("zonghe", p1.Name, p1.Uid);
                        GRoot.inst.HidePopup(headselect);
                    });
                    //注销组队功能
                    //headselect.GetChild("zudui").asButton.onClick.Add(() =>
                    //{
                    //    Protomsg.CS_OrganizeTeam msg1 = new Protomsg.CS_OrganizeTeam();
                    //    msg1.Player1 = GameScene.Singleton.m_MyMainUnit.ControlID;
                    //    msg1.Player2 = p1.Uid;
                    //    MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_OrganizeTeam", msg1);
                    //    GRoot.inst.HidePopup(headselect);
                    //});
                });
                
                
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
            
            FriendsCom.GetChild("add").onClick.Add(() => {
                //输入名字
                var inputnamecom = UIPackage.CreateObject("GameUI", "InputFriendID").asCom;
                GRoot.inst.AddChild(inputnamecom);
                inputnamecom.xy = Tool.GetPosition(0.5f, 0.5f);

                inputnamecom.GetChild("ok").asButton.onClick.Add(() =>
                {
                    var txt = inputnamecom.GetChild("input").asTextInput.text;
                    if (txt.Length <= 0)
                    {
                        Tool.NoticeWords("请输入ID！", null);
                        inputnamecom.Dispose();
                        return;
                    }
                    var idarray = txt.Split('_');
                    if(idarray.Length != 2)
                    {
                        Tool.NoticeWords("ID错误！", null);
                        inputnamecom.Dispose();
                        return;
                    }
                    int uid = 0;
                    int cid = 0;
                    try
                    {
                        uid = Convert.ToInt32(idarray[0]); //报异常
                        cid = Convert.ToInt32(idarray[1]); //报异常 
                    }
                    catch (SystemException e)
                    {
                        Tool.NoticeWords("ID错误！", null);
                        inputnamecom.Dispose();
                        return;
                    }
                    

                    Protomsg.CS_AddFriendRequest msg1 = new Protomsg.CS_AddFriendRequest();
                    msg1.Uid = uid;
                    msg1.Characterid = cid;
                    MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_AddFriendRequest", msg1);
                    
                    inputnamecom.Dispose();
                });
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
            //解析分隔数据
            Protomsg.CS_GetFriendsList msg = new Protomsg.CS_GetFriendsList();
            MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_GetFriendsList", msg);
            if (GameScene.Singleton.m_MyMainUnit != null)
            {
                var id = GameScene.Singleton.m_MyMainUnit.ControlID + "_" + GameScene.Singleton.m_MyMainUnit.CharacterID;
                sInstanse.FriendsCom.GetChild("id").asTextField.text = id;
            }
        }
    }

    

    // Update is called once per frame
    void Update () {
		
	}
}
