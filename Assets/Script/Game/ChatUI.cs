using cocosocket4unity;
using FairyGUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf;
public class ChatUI : MonoBehaviour {
    protected GComponent ChatBoxCom;//聊天界面

    //////聊天频道 1附近 2全服 3私聊 4队伍 5公会
    protected int ZongHe_Chanel;//综合页面 发现消息的频道
    //输入框
    protected GTextInput InputContent;//输入框
    //目标玩家uid
    protected int DestPlayerUID;
    protected string DestPlayerName = "";
    static ChatUI sInstanse = null;
    // Use this for initialization
    void Start () {
        sInstanse = this;
        CreateChatBox();
        SetSendMsgChanel(1);//默认为附近频道
        MsgManager.Instance.AddListener("SC_ChatInfo", new HandleMsg(this.SC_ChatInfo));
    }
    void OnDestroy()
    {
        MsgManager.Instance.RemoveListener("SC_ChatInfo");
    }

    //
    public string RemoveTextDest(string text)
    {
        if (text.IndexOf('@') == 0 && text.IndexOf(' ') > 1)
        {
            text = text.Substring(text.IndexOf(' '));
        }
        return text;
    }
    public string RemoveTextContent(string text)
    {
        if (text.IndexOf('@') == 0 && text.IndexOf(' ') > 1)
        {
            text = text.Substring(0,text.IndexOf(' ')+1);
        }
        else
        {
            return "";
        }
        return text;
    }
    public string ChangeTextDest(string text, string t1)
    {
        text = RemoveTextDest(text);
        text = '@' + t1 + " "+text;

        return text;
    }

    //改变发送消息的目标 队伍用@team表示 附近用空表示
    public void ChangeDest(int chanel,string name,int uid)
    {
        //////聊天频道 1附近 2全服 3私聊 4队伍
        if(chanel == 1)
        {
            InputContent.text = RemoveTextDest(InputContent.text);
        }
        else if (chanel == 3)
        {
            if (name.Length > 0 && uid > 0)
            {
                //私聊
                InputContent.text = ChangeTextDest(InputContent.text, name);
            }
            else
            {
                InputContent.text = RemoveTextDest(InputContent.text);
            }
        }
        else if(chanel == 4)
        {
            //Debug.Log("changedest1:" + InputContent.text);
            InputContent.text = ChangeTextDest(InputContent.text, "team");
            //Debug.Log("changedest2:" + InputContent.text);
        }
        else if (chanel == 5)
        {
            //Debug.Log("changedest1:" + InputContent.text);
            InputContent.text = ChangeTextDest(InputContent.text, "公会");
            //Debug.Log("changedest2:" + InputContent.text);
        }
    }

    //设置发送消息频道
    public void SetSendMsgChanel(int chanel)
    {
        ZongHe_Chanel = chanel;
        ChatBoxCom.GetChild("chanelbtn").asButton.title = Tool.GetTextFromChatChanel(chanel);
        ChangeDest(chanel, DestPlayerName, DestPlayerUID);
    }
    //发送消息
    public void SendChatMsg()
    {
        if(InputContent.text.Length <= 0)
        {
            return;
        }
        var curpagename = ChatBoxCom.GetController("page").selectedPage;
        //////聊天频道 1附近 2全服 3私聊 4队伍
        var chanel = 1;
        if (curpagename == "zonghe")
        {
            ChangeDest(ZongHe_Chanel, DestPlayerName, DestPlayerUID);
            chanel = ZongHe_Chanel;
        }
        else
        {
            if(curpagename == "duiwu")
            {
                chanel = 4;
            }else if(curpagename == "siliao")
            {
                chanel = 3;
            }
            else if (curpagename == "guild")
            {
                chanel = 5;
            }
        }

        //解析分隔数据
        Protomsg.CS_ChatInfo msg = new Protomsg.CS_ChatInfo();
        msg.Channel = chanel;
        msg.DestPlayerUID = DestPlayerUID;
        msg.Content = RemoveTextDest(InputContent.text);
        
        MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_ChatInfo", msg);

        Debug.Log("sendchatmsg:" + msg.Content);
        

        InputContent.text = RemoveTextContent(InputContent.text);
    }

    //创建聊天界面
    public void CreateChatBox()
    {
        if (ChatBoxCom == null)
        {
            ChatBoxCom = UIPackage.CreateObject("GameUI", "chatbox").asCom;
            GRoot.inst.AddChild(ChatBoxCom);
            ChatBoxCom.xy = Tool.GetPosition(0.5f, 0.5f);

            InputContent = ChatBoxCom.GetChild("inputwords").asTextInput;
            //
            ChatBoxCom.visible = false;
            ChatBoxCom.GetChild("close").onClick.Add(() => {
                ChatBoxCom.visible = false;
            });

            //发送消息按钮
            ChatBoxCom.GetChild("ok").onClick.Add(() => {
                SendChatMsg();

            });

            //选择频道按钮
            ChatBoxCom.GetChild("chanelbtn").onClick.Add(() => {

                //GRoot.inst.ShowPopup(ChatBoxCom.GetChild("selectchanel"));
                ChatBoxCom.GetChild("selectchanel").visible = true;
            });
            //////聊天频道 1附近 2全服 3私聊 4队伍
            //私聊
            ChatBoxCom.GetChild("selectchanel").asCom.GetChild("siliao").onClick.Add(() => {
                var curpagename = ChatBoxCom.GetController("page").selectedPage;
                if (curpagename != "siliao" && curpagename != "zonghe")
                {
                    OpenChatBox("siliao", "", 0);
                }
                ChatBoxCom.GetChild("selectchanel").visible = false;
                SetSendMsgChanel(3);
            });
            //队伍
            ChatBoxCom.GetChild("selectchanel").asCom.GetChild("duiwu").onClick.Add(() => {
                var curpagename = ChatBoxCom.GetController("page").selectedPage;
                if (curpagename != "duiwu" && curpagename != "zonghe")
                {
                    OpenChatBox("duiwu", "", 0);
                }
                ChatBoxCom.GetChild("selectchanel").visible = false;
                SetSendMsgChanel(4);
            });
            //公会
            ChatBoxCom.GetChild("selectchanel").asCom.GetChild("guild").onClick.Add(() => {
                var curpagename = ChatBoxCom.GetController("page").selectedPage;
                if (curpagename != "guild" && curpagename != "zonghe")
                {
                    OpenChatBox("guild", "", 0);
                }
                ChatBoxCom.GetChild("selectchanel").visible = false;
                SetSendMsgChanel(5);
            });
            //地图
            ChatBoxCom.GetChild("selectchanel").asCom.GetChild("fujin").onClick.Add(() => {
                var curpagename = ChatBoxCom.GetController("page").selectedPage;
                if (curpagename != "fujin" && curpagename != "zonghe")
                {
                    OpenChatBox("fujin", "", 0);
                }
                ChatBoxCom.GetChild("selectchanel").visible = false;
                SetSendMsgChanel(1);
            });

            //
            ChatBoxCom.GetController("page").onChanged.Add(() => {
                var curpagename = ChatBoxCom.GetController("page").selectedPage;
                if (curpagename != "zonghe")
                {
                    ChatBoxCom.GetChild("chanelbtn").visible = false;
                    var chanel = 1;
                    if (curpagename == "siliao")
                    {
                        chanel = 3;
                    }
                    else if (curpagename == "duiwu")
                    {
                        chanel = 4;
                    }
                    else if (curpagename == "guild")
                    {
                        chanel = 5;
                    }
                    ChangeDest(chanel, DestPlayerName, DestPlayerUID);
                }
                else
                {
                    ChatBoxCom.GetChild("chanelbtn").visible = true;
                    OpenChatBox("zonghe", "", 0);

                }
            });
        }
    }
    public void OpenChatBox(string pagename, string destname, int destuid)
    {
        //弹出断开连接
        if (ChatBoxCom == null)
        {
            CreateChatBox();
        }
        AudioManager.Am.Play2DSound(AudioManager.Sound_OpenLittleUI);
        ChatBoxCom.visible = true;
        ChatBoxCom.parent.AddChild(ChatBoxCom);

        ChatBoxCom.GetController("page").selectedPage = pagename;

        //if (destname.Length > 0 && destuid > 0)
        //{
        DestPlayerName = destname;
        DestPlayerUID = destuid;
        //}


        if (pagename != "zonghe")
        {
            ChatBoxCom.GetChild("chanelbtn").visible = false;
        }
        else
        {
            ChatBoxCom.GetChild("chanelbtn").visible = true;
            if (DestPlayerName.Length > 0 && DestPlayerUID > 0)
            {
                SetSendMsgChanel(3);
            }
            else
            {
                SetSendMsgChanel(1);
            }
        }
    }

    //打开聊天界面
    public static void SOpenChatBox(string pagename, string destname, int destuid)
    {
        
        //selectedPage
        if(sInstanse != null)
        {
            sInstanse.OpenChatBox(pagename, destname, destuid);
        }

    }

    //通过频道类型获取频道文字

    //添加聊天数据到指定频道 zonghe siliao duiwu
    public void AddChatMsg2Chanel(Protomsg.SC_ChatInfo msg, string uichanel)
    {
        if (ChatBoxCom == null)
        {
            return;
        }

        var msglist = ChatBoxCom.GetChild(uichanel);
        if (msglist == null)
        {
            return;
        }

        //////聊天频道 1附近 2全服 3私聊 4队伍

        //组装内容
        var msgui = UIPackage.CreateObject("GameUI", "liaotian_content").asCom;
        //var content = "\\[" + "[color=#FFcc99]" + msg.Time + "[/color]" + "]";//时间
        var content = "[color=#66ff66]" + "[" + Tool.GetTextFromChatChanel(msg.Channel) + "]" + "[/color]";
        content += "" + msg.SrcName + "" + ":";//名字
        content += "" + Tool.GetContetColorFromChatChanel(msg.Channel) + msg.Content + "[/color]";
        msgui.GetChild("content").asTextField.text = content;

        //点击
        msgui.onClick.Add(() => {
            Debug.Log("chat click:"+ msg.SrcName+" uid:"+ msg.SrcPlayerUID);
            if(GameScene.Singleton.m_MyMainUnit == null)
            {
                return;
            }
            if(msg.SrcPlayerUID > 0 && msg.SrcPlayerUID != GameScene.Singleton.m_MyMainUnit.ControlID)
            {
                var headselect = UIPackage.CreateObject("GameUI", "playerclick").asCom;
                GRoot.inst.ShowPopup(headselect);
                headselect.GetChild("siliao").asButton.onClick.Add(() =>
                {
                    DestPlayerName = msg.SrcName;
                    DestPlayerUID = msg.SrcPlayerUID;
                    
                    var curpagename = ChatBoxCom.GetController("page").selectedPage;
                    if (curpagename != "zonghe" && curpagename != "siliao")
                    {
                        OpenChatBox("zonghe", DestPlayerName, DestPlayerUID);
                    }
                    else
                    {
                        SetSendMsgChanel(3);
                    }

                    GRoot.inst.HidePopup(headselect);
                });
                //注销组队功能
                //headselect.GetChild("zudui").asButton.onClick.Add(() =>
                //{
                //    Protomsg.CS_OrganizeTeam msg1 = new Protomsg.CS_OrganizeTeam();
                //    msg1.Player1 = GameScene.Singleton.m_MyMainUnit.ControlID;
                //    msg1.Player2 = msg.SrcPlayerUID;
                //    MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_OrganizeTeam", msg1);
                //    GRoot.inst.HidePopup(headselect);
                //});
                headselect.GetChild("haoyou").asButton.onClick.Add(() =>
                {
                    Protomsg.CS_AddFriendRequest msg1 = new Protomsg.CS_AddFriendRequest();
                    msg1.Uid = msg.SrcPlayerUID;
                    MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_AddFriendRequest", msg1);
                    GRoot.inst.HidePopup(headselect);
                });
                

            }
        });

        

        msglist.asList.AddChild(msgui);
        if (msglist.asList.GetChildIndex(msgui) > 0 && msglist.asList.IsChildInView(msglist.asList.GetChildAt(msglist.asList.GetChildIndex(msgui)-1)))
        {
            msglist.asList.ScrollToView(msglist.asList.GetChildIndex(msgui));
        }

        //主界面聊天信息
        if(uichanel == "zonghe")
        {
            GameUI.AddChatMsg(content);
        }
        
        
    }

    public bool SC_ChatInfo(Protomsg.MsgBase d1)
    {
        Debug.Log("SC_ChatInfo:");
        IMessage IMperson = new Protomsg.SC_ChatInfo();
        Protomsg.SC_ChatInfo p1 = (Protomsg.SC_ChatInfo)IMperson.Descriptor.Parser.ParseFrom(d1.Datas);
        //////聊天频道 1附近 2全服 3私聊 4队伍
        //zonghe siliao duiwu

        AddChatMsg2Chanel(p1, "zonghe");
        if (p1.Channel == 1)
        {
            AddChatMsg2Chanel(p1, "fujin");
        }
        if (p1.Channel == 3)
        {
            AddChatMsg2Chanel(p1, "siliao");
        }
        if (p1.Channel == 4)
        {
            AddChatMsg2Chanel(p1, "duiwu");
        }
        if (p1.Channel == 5)
        {
            AddChatMsg2Chanel(p1, "guild");
        }

        return true;
    }
    //测试聊天数据
    public void testchat()
    {
        ////聊天频道 1附近 2全服 3私聊 4队伍
        Protomsg.SC_ChatInfo msg = new Protomsg.SC_ChatInfo();
        msg.Channel = 1;
        msg.SrcName = "t1";
        msg.Time = "10:59";
        msg.Content = "来了 测试 范德萨发 接口了聚隆科技 范德萨发生的";

        Protomsg.MsgBase msg1 = new Protomsg.MsgBase();
        msg1.ModeType = "";
        msg1.MsgType = "SC_ChatInfo";
        if (msg != null)
        {
            msg1.Datas = ByteString.CopyFrom(msg.ToByteArray());
        }
        MsgManager.Instance.AddMessage(msg1);
    }

    // Update is called once per frame
    void Update () {
		
	}
}
