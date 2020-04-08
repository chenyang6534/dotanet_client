using FairyGUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cocosocket4unity;
using UnityEngine.SceneManagement;
using Google.Protobuf;

public class GameUI : MonoBehaviour {


    private GComponent mainUI;
    private GTextField gPing;
    private Joystick joystick;
    private int touchID;
    private Vector2 startPos;
    private double startTime;

    private GComponent leftTopHead;

    private GComponent center;

    private GList Bufs;

    private Btnstick attackstick;

    private HeadInfo MyHeadInfo;
    private HeadInfo TargetHeadInfo;

    protected GComponent LittleMapCom;//小地图

    

    protected GButton ShowOffBtn;

    protected GComponent TeamInfo; //队伍信息
    protected int SceneID;

    protected GComponent DieUI; //死亡UI

    protected GComponent LittleChat; //聊天信息

    protected double LastShowNoticeTime;//上次显示提示的时间

    protected bool IsShowAll;//是否显示所有

    static GameUI sInstanse = null;
    void Start () {
        //Debug.Log("gameui:" + Input.multiTouchEnabled + "  " + Input.touchSupported+"  "+ Input.stylusTouchSupported);
        sInstanse = this;
        //Input.multiTouchEnabled = true;
        SceneID = -1;
        SkillCom = new Dictionary<int, Skillstick>();
        ItemSkillCom = new Dictionary<int, Skillstick>();
        BufsRes = new Dictionary<int, GComponent>();
        mainUI = GetComponent<UIPanel>().ui;
        touchID = -1;
        startTime = Tool.GetTime();

        //mainUI.touchable = true;

        center = mainUI.GetChild("center").asCom;
        //Debug.Log("center---------------------------:"+ center.name);

        Bufs = mainUI.GetChild("buflist").asList;

        MyHeadInfo = new HeadInfo(mainUI.GetChild("myHeadInfo").asCom);
        MyHeadInfo.IsMy = true;
        TargetHeadInfo = new HeadInfo(mainUI.GetChild("targetHeadInfo").asCom);
        TargetHeadInfo.IsMy = false;
        LittleMapCom = mainUI.GetChild("littlemap").asCom;



        //死亡界面
        DieUI = mainUI.GetChild("dieui").asCom;
        DieUI.visible = false;
        DieUI.GetChild("goldbtn").asButton.onClick.Add(()=> {
            //金币立即复活
            Protomsg.CS_QuickRevive msg1 = new Protomsg.CS_QuickRevive();
            msg1.ReviveType = 1;
            MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_QuickRevive", msg1);
        });
        DieUI.GetChild("diamondbtn").asButton.onClick.Add(() => {
            //砖石立即复活
            Protomsg.CS_QuickRevive msg1 = new Protomsg.CS_QuickRevive();
            msg1.ReviveType = 2;
            MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_QuickRevive", msg1);
        });


        //显示隐藏组队界面按钮
        //ShowOffBtn = mainUI.GetChild("showoffbtn1").asButton;
        //ShowOffBtn.visible = false;//注销组队功能
        //ShowOffBtn.onClick.Add((EventContext context) => {
        //    TeamInfo.visible = !TeamInfo.visible;
        //    Debug.Log("-----------ShowOffBtn:" + TeamInfo.visible);
        //});

        //聊天信息
        LittleChat = mainUI.GetChild("littlechat").asCom;
        Tool.AddClick(LittleChat.GetChild("contentlist").asList, () => {
            ChatUI.SOpenChatBox("zonghe", "", 0);
        });
        //LittleChat.GetChild("contentlist").asList.onTouchBegin.Add((EventContext context) =>
        //{
        //    InputEvent inputEvent = (InputEvent)context.data;
        //});
        //LittleChat.GetChild("contentlist").asList.onTouchEnd.Add((EventContext context) =>
        //{
        //    //gameObject.GetComponent<ChatUI>().testchat();
        //    ChatUI.SOpenChatBox("zonghe", "", 0);
        //    return;
        //}); ;

        //组队信息
        TeamInfo = mainUI.GetChild("teaminfo").asCom;
        TeamInfoShow(false);

        
        //设置 退出
        LittleMapCom.GetChild("set_btn").asButton.onClick.Add(() =>
        {
            
            
            //弹出断开连接
            var teamrequest = UIPackage.CreateObject("GameUI", "noticeExit").asCom;
            GRoot.inst.AddChild(teamrequest);
            teamrequest.xy = Tool.GetPosition(0.5f, 0.5f);
            AudioManager.Am.Play2DSound(AudioManager.Sound_OpenLittleUI);
            teamrequest.GetChild("ok").onClick.Add(() => {
                teamrequest.Dispose();
                SceneManager.LoadScene(0);
            });
            teamrequest.GetChild("no").onClick.Add(() => {
                teamrequest.Dispose();

            });
            //SceneManager.LoadScene(0);
        });
        //商店
        LittleMapCom.GetChild("store").asButton.onClick.Add(() =>
        {
            //SceneManager.LoadScene(0);
            new StoreInfo();
        });
        //邮件
        LittleMapCom.GetChild("mail").asButton.onClick.Add(() =>
        {
            Mails.SOpen();
            return;
        });
        //好友
        LittleMapCom.GetChild("friend").asButton.onClick.Add(() =>
        {
            Friends.SOpen();
        });
        //交易所
        LittleMapCom.GetChild("exchange").asButton.onClick.Add(() =>
        {
            new ExchangeInfo();
        });

        //公会
        LittleMapCom.GetChild("guild").asButton.onClick.Add(() =>
        {
            new GuildInfo();
        });

        //活动地图
        LittleMapCom.GetChild("activitymap").asButton.onClick.Add(() =>
        {
            new ActivityMap();
        });

        //显示所有
        ShowAllBtn(false);
        LittleMapCom.GetChild("showbtn").asButton.onClick.Add(() =>
        {
            ShowAllBtn(!IsShowAll);
        });

        //屏幕点击
        GObject touch = mainUI.GetChild("touchArea");
        touch.onTouchBegin.Add(OnTouchBegin);
        touch.onTouchMove.Add(OnTouchMove);
        touch.onTouchEnd.Add(OnTouchEnd);
        

        //ping值 
        gPing = mainUI.GetChild("n3").asTextField;
        //移动遥感
        joystick = new Joystick(mainUI.GetChild("n1").asCom);
        joystick.onMove.Add(JoystickMove);
        joystick.onEnd.Add(JoystickEnd);
        //攻击遥感
        attackstick = new Btnstick(mainUI.GetChild("attack").asCom);
        attackstick.onMove.Add(AttackstickMove);
        attackstick.onEnd.Add(AttackstickEnd);
        attackstick.onDown.Add(AttackstickDown);

        InitLeftTopHead();

        //view.AddRelation(GRoot.inst, RelationType.Right_Right);
        //view.AddRelation(GRoot.inst, RelationType.Bottom_Bottom);

        MsgManager.Instance.AddListener("SC_UpdateTeamInfo", new HandleMsg(this.SC_UpdateTeamInfo));
        MsgManager.Instance.AddListener("SC_NoticeWords", new HandleMsg(this.SC_NoticeWords));
        MsgManager.Instance.AddListener("SC_RequestTeam", new HandleMsg(this.SC_RequestTeam));
        MsgManager.Instance.AddListener("CC_Disconnect", new HandleMsg(this.CC_Disconnect));
        

    }
    void OnDestroy()
    {
        MsgManager.Instance.RemoveListener("SC_UpdateTeamInfo");
        MsgManager.Instance.RemoveListener("SC_NoticeWords");
        MsgManager.Instance.RemoveListener("SC_RequestTeam");
        MsgManager.Instance.RemoveListener("CC_Disconnect");
    }
    //显示所有按钮
    public void ShowAllBtn(bool show)
    {
        IsShowAll = show;
        if (IsShowAll)
        {
            LittleMapCom.GetChild("set_btn").visible = true;
            LittleMapCom.GetChild("store").visible = true;
            LittleMapCom.GetChild("mail").visible = true;
            LittleMapCom.GetChild("friend").visible = true;
            LittleMapCom.GetChild("exchange").visible = true;
            LittleMapCom.GetChild("guild").visible = true;
            LittleMapCom.GetChild("activitymap").visible = true;
        }
        else
        {
            LittleMapCom.GetChild("set_btn").visible = false;
            LittleMapCom.GetChild("store").visible = false;
            LittleMapCom.GetChild("mail").visible = false;
            LittleMapCom.GetChild("friend").visible = false;
            LittleMapCom.GetChild("exchange").visible = false;
            LittleMapCom.GetChild("guild").visible = false;
            LittleMapCom.GetChild("activitymap").visible = false;
        }
    }

    //添加聊天信息
    public static void AddChatMsg(string content)
    {
        //content
        //组装内容
        var msgui = UIPackage.CreateObject("GameUI", "showchatliaotian_content").asCom;
        msgui.GetChild("content").asTextField.text = content;
        var list = sInstanse.LittleChat.GetChild("contentlist").asList;
        list.AddChild(msgui);

        //msgui.onClick.Add(() =>
        //{
        //    ChatUI.SOpenChatBox("zonghe", "", 0);
        //});

        if (list.GetChildIndex(msgui) > 0 && list.IsChildInView(list.GetChildAt(list.GetChildIndex(msgui) - 1)))
        {
            list.ScrollToView(list.GetChildIndex(msgui));
        }

        Debug.Log("AddChatMsg:" + content);
    }


    //掉线
    public bool CC_Disconnect(Protomsg.MsgBase d1)
    {
        Debug.Log("CC_Disconnect:");
        IMessage IMperson = new Protomsg.CC_Disconnect();
        Protomsg.CC_Disconnect p1 = (Protomsg.CC_Disconnect)IMperson.Descriptor.Parser.ParseFrom(d1.Datas);
        //
        //弹出断开连接
        var teamrequest = UIPackage.CreateObject("GameUI", "noticeWindow").asCom;
        GRoot.inst.AddChild(teamrequest);
        teamrequest.xy = Tool.GetPosition(0.5f, 0.5f);
        AudioManager.Am.Play2DSound(AudioManager.Sound_OpenLittleUI);
        teamrequest.GetChild("ok").onClick.Add(() => {
            teamrequest.Dispose();
            SceneManager.LoadScene(0);
        });
        return true;
    }

    //组队请求
    public bool SC_RequestTeam(Protomsg.MsgBase d1)
    {
        Debug.Log("SC_RequestTeam:");
        IMessage IMperson = new Protomsg.SC_RequestTeam();
        Protomsg.SC_RequestTeam p1 = (Protomsg.SC_RequestTeam)IMperson.Descriptor.Parser.ParseFrom(d1.Datas);

        //弹出组队请求框
        var teamrequest = UIPackage.CreateObject("GameUI", "TeamRequest").asCom;
        GRoot.inst.AddChild(teamrequest);
        teamrequest.xy = Tool.GetPosition(0, 0.3f);

        AudioManager.Am.Play2DSound(AudioManager.Sound_OpenLittleUI);

        //SrcUnitTypeID
        var clientitem = ExcelManager.Instance.GetUnitInfoManager().GetUnitInfoByID(p1.SrcUnitTypeID);
        if(clientitem != null)
        {
            teamrequest.GetChild("headicon").asLoader.url = clientitem.IconPath;
        }
        teamrequest.GetChild("name").asTextField.text = p1.SrcName;
        teamrequest.GetChild("level").asTextField.text = p1.SrcLevel+"";

        teamrequest.GetChild("no_btn").asButton.onClick.Add(() =>
        {

            //回复同意组队请求
            Protomsg.CS_ResponseOrgTeam msg1 = new Protomsg.CS_ResponseOrgTeam();
            msg1.SrcPlayerUID = p1.SrcPlayerUID;
            msg1.RequestType = p1.RequestType;
            msg1.IsAgree = 2;
            MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_ResponseOrgTeam", msg1);

            teamrequest.Dispose();
        });

        teamrequest.GetChild("yes_btn").asButton.onClick.Add(() =>
        {
            //回复同意组队请求
            Protomsg.CS_ResponseOrgTeam msg1 = new Protomsg.CS_ResponseOrgTeam();
            msg1.SrcPlayerUID = p1.SrcPlayerUID;
            msg1.RequestType = p1.RequestType;
            msg1.IsAgree = 1;
            MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_ResponseOrgTeam", msg1);
            teamrequest.Dispose();
        });

        

        return true;
    }
    public IEnumerator ShowNotice(Protomsg.SC_NoticeWords p1,float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        var noticewords = ExcelManager.Instance.GetNoticeWordsManager().GetNoticeWordsByID(p1.TypeID);
        if (noticewords != null)
        {
            Tool.NoticeWordsAnim(noticewords.Words, p1.P, noticewords.AnimName, noticewords.Pos);
            if (noticewords.Sound != null && noticewords.Sound.Length > 0)
            {
                AudioManager.Am.Play2DSound(noticewords.Sound);
            }
            Debug.Log("SC_NoticeWords:" + noticewords.Words + "  sound:" + noticewords.Sound);
        }
    }
    public bool SC_NoticeWords(Protomsg.MsgBase d1)
    {
        //Debug.Log("SC_NoticeWords:");
        IMessage IMperson = new Protomsg.SC_NoticeWords();
        Protomsg.SC_NoticeWords p1 = (Protomsg.SC_NoticeWords)IMperson.Descriptor.Parser.ParseFrom(d1.Datas);
        double delaytime = 1 - (Tool.GetTime() - LastShowNoticeTime);
        if (delaytime <= 0)
        {
            LastShowNoticeTime = Tool.GetTime();
            StartCoroutine(ShowNotice(p1,0));
        }
        else
        {
            LastShowNoticeTime = Tool.GetTime() + delaytime;
            StartCoroutine(ShowNotice(p1, (float)delaytime)); 
        }
        return true;
    }

    //CS_UpdateTeamInfo
    //更新队伍信息
    public bool SC_UpdateTeamInfo(Protomsg.MsgBase d1)
    {
        Debug.Log("SC_UpdateTeamInfo:");
        IMessage IMperson = new Protomsg.SC_UpdateTeamInfo();
        Protomsg.SC_UpdateTeamInfo p1 = (Protomsg.SC_UpdateTeamInfo)IMperson.Descriptor.Parser.ParseFrom(d1.Datas);

        var teamlist = TeamInfo.GetChild("unitslist").asList;
        var oldcount = teamlist.GetChildren().Length;
        teamlist.RemoveChildren();

        if(p1.TPInfo.Count <= 1)
        {
            return true;
        }
        //自动打开组队界面
        if(oldcount == 0 && p1.TPInfo.Count > 1)
        {

            TeamInfoShow(true);
        }

        //处理排序
        Protomsg.TeamPlayerInfo[] allplayer = new Protomsg.TeamPlayerInfo[p1.TPInfo.Count];
        int index = 0;
        foreach (var item in p1.TPInfo)
        {
            allplayer[index++] = item;
        }

            //排序
        System.Array.Sort(allplayer, (s1, s2) => s1.Name.CompareTo(s2.Name));
        foreach (var item in allplayer)
        {
            //Debug.Log("SC_UpdateTeamInfo11:" + item.HP + "  " + item.MP + "  "+item.MaxHP + "  " + item.MaxMP);
            //Debug.Log("SC_UpdateTeamInfo22:" + (int)((float)item.HP / item.MaxHP * 100) + "  " + (int)((float)item.MP / item.MaxMP * 100));
            //(int)((float)HP / MaxHP * 100)
            var onedropitem = UIPackage.CreateObject("GameUI", "TeamInfo_OneUnit").asCom;
            
            onedropitem.GetChild("hp").asProgress.value = (int)((float)item.HP / item.MaxHP * 100);
            onedropitem.GetChild("mp").asProgress.value = (int)((float)item.MP / item.MaxMP * 100);
            onedropitem.GetChild("name").asTextField.text = item.Name;
            teamlist.AddChild(onedropitem);

            

            onedropitem.onClick.Add(() => {
                Debug.Log("----teamclick :" + p1.MainUID + "  :" + item.ID);
                if(GameScene.Singleton.m_MyMainUnit != null && GameScene.Singleton.m_MyMainUnit.ControlID == item.UID)
                {
                    //点击了自己 
                    //TeamPlayerInfo
                    var headselect = UIPackage.CreateObject("GameUI", "TeamPlayerInfo").asCom;
                    GRoot.inst.ShowPopup(headselect);
                    headselect.GetChild("out").visible = false;
                    headselect.GetChild("leave").asButton.onClick.Add(() =>
                    {


                        Protomsg.CS_OutTeam msg1 = new Protomsg.CS_OutTeam();
                        msg1.OutPlayerUID = item.UID;
                        MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_OutTeam", msg1);
                        GRoot.inst.HidePopup(headselect);
                    });

                    headselect.GetChild("info").asButton.onClick.Add(() =>
                    {
                        var unit = UnityEntityManager.Instance.GetUnityEntity(item.ID);
                        if (unit != null)
                        {
                            MyInfo myinfo = new MyInfo(unit);
                        }
                        GRoot.inst.HidePopup(headselect);
                    });

                    
                }
                else if( GameScene.Singleton.m_MyMainUnit != null && GameScene.Singleton.m_MyMainUnit.ControlID == p1.MainUID)
                {
                    //TeamPlayerInfo//自己是否是队伍的队长
                    var headselect = UIPackage.CreateObject("GameUI", "TeamPlayerInfo").asCom;
                    GRoot.inst.ShowPopup(headselect);
                    headselect.GetChild("leave").visible = false;
                    headselect.GetChild("out").asButton.onClick.Add(() =>
                    {
                        

                        Protomsg.CS_OutTeam msg1 = new Protomsg.CS_OutTeam();
                        msg1.OutPlayerUID = item.UID;
                        MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_OutTeam", msg1);
                        GRoot.inst.HidePopup(headselect);
                    });

                    headselect.GetChild("info").asButton.onClick.Add(() =>
                    {
                        var unit = UnityEntityManager.Instance.GetUnityEntity(item.ID);
                        if (unit != null)
                        {
                            MyInfo myinfo = new MyInfo(unit);
                        }
                        GRoot.inst.HidePopup(headselect);
                    });

                    headselect.GetChild("siliao").asButton.onClick.Add(() =>
                    {

                        ChatUI.SOpenChatBox("zonghe", item.Name, item.UID);
                        GRoot.inst.HidePopup(headselect);
                    });
                }
                else
                {
                    var headselect = UIPackage.CreateObject("GameUI", "TeamPlayerInfo").asCom;
                    GRoot.inst.ShowPopup(headselect);
                    headselect.GetChild("leave").visible = false;
                    headselect.GetChild("out").visible = false;
                    headselect.GetChild("info").asButton.onClick.Add(() =>
                    {
                        var unit = UnityEntityManager.Instance.GetUnityEntity(item.ID);
                        if (unit != null)
                        {
                            MyInfo myinfo = new MyInfo(unit);
                        }
                        GRoot.inst.HidePopup(headselect);
                    });

                    headselect.GetChild("siliao").asButton.onClick.Add(() =>
                    {

                        ChatUI.SOpenChatBox("zonghe", item.Name, item.UID);
                        GRoot.inst.HidePopup(headselect);
                    });
                    //var unit = UnityEntityManager.Instance.GetUnityEntity(item.ID);
                    //if (unit != null)
                    //{
                    //    MyInfo myinfo = new MyInfo(unit);
                    //}
                }

                
                
            });
        }

        return true;
    }

    //
    protected void TeamInfoShow(bool isshow)
    {
        TeamInfo.visible = isshow;
        TeamInfo.visible = false;//注销组队功能
    }

    //更新组队信息
    public void UpdateTeamInfo()
    {
        //
        if(GameScene.Singleton.m_MyMainUnit != null)
        {
            if(GameScene.Singleton.m_MyMainUnit.TeamID <= 0)
            {
                var teamlist = TeamInfo.GetChild("unitslist").asList;
                teamlist.RemoveChildren();

                //自动关闭组队界面
                //ShowOffBtn.selected = false;
                TeamInfoShow(false);
            }
        }
    }


    //屏幕点击
    private void OnTouchBegin(EventContext context)
    {
        
        if (touchID == -1)//第一次触摸
        {
            InputEvent inputEvent = (InputEvent)context.data;
            touchID = inputEvent.touchId;
            //Debug.Log("OnTouchBegin111:" + inputEvent.touchId);
            startTime = Tool.GetTime();

            startPos = GRoot.inst.LocalToGlobal(inputEvent.position);
            startPos.y = Screen.height - startPos.y;
            //startStagePos = localPos;

            context.CaptureTouch();
            //Vector2 screenPos = GRoot.inst.LocalToGlobal(localPos);
            ////原点位置转换
            //screenPos.y = Screen.height - screenPos.y;
            ////一般情况下，还需要提供距离摄像机视野正前方distance长度的参数作为screenPos.z(如果需要，将screenPos改为Vector3类型）
            //Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);

            //Debug.Log("touchpos11111111111:" + inputEvent.position);
            //Debug.Log("OnTouchBegin11111111111:"+ startPos);

            //m_IsMoved = false;
        }
    }

    //移动触摸
    private void OnTouchMove(EventContext context)
    {
        InputEvent inputEvent = (InputEvent)context.data;
        //Debug.Log("OnTouchMove111:" + inputEvent.touchId);
        if (touchID != -1 && inputEvent.touchId == touchID)
        {
            //Vector2 localPos = GRoot.inst.GlobalToLocal(new Vector2(inputEvent.x, inputEvent.y));
            Vector2 localPos = GRoot.inst.LocalToGlobal(inputEvent.position);
            localPos.y = Screen.height - localPos.y;

            var dir = (localPos - startPos);
            

            DHCameraManager.Singleton.SetTempCameraMoveOffset(dir);
        }
    }

    //结束触摸
    private void OnTouchEnd(EventContext context)
    {
        InputEvent inputEvent = (InputEvent)context.data;
        //Debug.Log("OnTouchEnd111:" + inputEvent.touchId);
        if (touchID != -1 && inputEvent.touchId == touchID)
        {
            touchID = -1;
            //Debug.Log("OnTouchEnd:" + inputEvent.position);
            Vector2 localPos = GRoot.inst.LocalToGlobal(inputEvent.position);
            localPos.y = Screen.height - localPos.y;
            var dir = (localPos - startPos);

            DHCameraManager.Singleton.AddCameraMoveOffset(dir);

            //点击距离范围
            var len = Vector2.Distance(dir, new Vector2(0, 0));
            if (Tool.GetTime()-startTime <= 0.2 && len <= 10)
            {
                Vector2 clickpos = inputEvent.position;
                clickpos.y = Screen.height - clickpos.y;
                GameScene.Singleton.ClickPos(clickpos);
            }
        }

    }






    //1:down 2:move 3:end
    private void AttackstickMove(EventContext context)
    {
        Vector2 dir = (Vector2)context.data;
        dir.y = -dir.y;
        //Debug.Log("AttackstickMove:"+ dir);
        GameScene.Singleton.PressAttackBtn(2, dir);
        //GameScene.Singleton.SendControlData(degree, true);


    }
    private void AttackstickDown(EventContext context)
    {
        //Debug.Log("AttackstickDown");

        GameScene.Singleton.PressAttackBtn(1,Vector2.zero);

        //GameScene.Singleton.SendControlData(degree, false);

    }
    private void AttackstickEnd(EventContext context)
    {
        float degree = (float)context.data;
        //Debug.Log("AttackstickEnd");

        GameScene.Singleton.PressAttackBtn(3,Vector2.zero);

        //GameScene.Singleton.SendControlData(degree, false);

    }


    private void JoystickMove(EventContext context)
    {
        float degree = (float)context.data;

        GameScene.Singleton.SendControlData(degree, true);


    }
    private void JoystickEnd(EventContext context)
    {
        float degree = (float)context.data;
        Debug.Log("JoystickEnd");

        GameScene.Singleton.SendControlData(degree, false);

    }
    public Vector2[] ThreeSkillPos = {
        new Vector2(927-1123,617-617),
        new Vector2(988-1123, 477- 617 ),
        new Vector2(1123 - 1123, 418-617) };
    public Vector2[] FourSkillPos = {
        new Vector2(884-1109,624-609),
        new Vector2(909-1109, 508- 609 ),
        new Vector2(1010 - 1109, 419-609),
        new Vector2(1136 - 1109, 399-609)};

    

    public Dictionary<int, Skillstick> SkillCom;
    //刷新技能UI显示
    void FreshSkillUI()
    {
        UnityEntity mainunit = GameScene.Singleton.GetMyMainUnit();
        if(mainunit == null || mainunit.SkillDatas == null)
        {
            return;
        }
        //主动技能个数
        var len = mainunit.SkillDatas.Length;
        
        if(len != SkillCom.Count)
        {
            SkillCom.Clear();
            
            foreach(var item in mainunit.SkillDatas)
            {

                GComponent view = UIPackage.CreateObject("GameUI", "Skillstick").asCom;
                //GRoot.inst.AddChild(view);
                mainUI.GetChild("attack").asCom.AddChildAt(view,0);
                if(item.Index < 4)
                {
                    view.xy = FourSkillPos[item.Index];
                }

                SkillCom[item.TypeID] = new Skillstick(view,false);
            }

            
        }
        //刷新信息

        foreach (var item in mainunit.SkillDatas)
        {
            Skillstick view = SkillCom[item.TypeID];
            if(view == null)
            {
                continue;
            }
            view.SkillDatas = item;
        }
    }

    public Vector2[] ItemSkillPos1 =
    {
        new Vector2(780-1109,624-579),
        new Vector2(680-1109,624-579),
        new Vector2(580-1109,624-579),
        new Vector2(480-1109,624-579),
        new Vector2(380-1109,624-579),
        new Vector2(280-1109,624-579),
        new Vector2(980-1109+20,624-579+20), //回城券位置
    };
    public Dictionary<int, Skillstick> ItemSkillCom;
    //刷新道具技能显示FreshItemSkill(data.ISD);
    void FreshItemSkillUI()
    {
        UnityEntity mainunit = GameScene.Singleton.GetMyMainUnit();
        if (mainunit == null || mainunit.ItemSkillDatas == null)
        {
            foreach (int key in new List<int>(ItemSkillCom.Keys))
            {
                ItemSkillCom[key].Destroy();
                ItemSkillCom.Remove(key);
            }
            //foreach (var item in ItemSkillCom)
            //{
            //    item.Value.Destroy();
            //    ItemSkillCom.Remove(item.Key);
            //}
            return;
        }

        //排序
        System.Array.Sort(mainunit.ItemSkillDatas, (s1, s2) => s1.Index.CompareTo(s2.Index));

        var index = 0;
        foreach (var item in mainunit.ItemSkillDatas)
        {
            if (ItemSkillCom.ContainsKey(item.TypeID))
            {
                ItemSkillCom[item.TypeID].SkillDatas = item;
                
                if (item.Index == 7)
                {
                    ItemSkillCom[item.TypeID].SetXY(ItemSkillPos1[6]);
                }
                else
                {
                    ItemSkillCom[item.TypeID].SetXY(ItemSkillPos1[index]);
                }
                
            }
            else
            {
                Debug.Log("  len:" + ItemSkillPos1.Length);
                GComponent view = UIPackage.CreateObject("GameUI", "Skillstick").asCom;
                //GRoot.inst.AddChild(view);
                mainUI.GetChild("attack").asCom.AddChild(view);
                if (index < 6)
                {
                    view.xy = ItemSkillPos1[index];
                }
                if (item.Index == 7)
                {
                    view.xy = ItemSkillPos1[6];
                }
                ItemSkillCom[item.TypeID] = new Skillstick(view,true);
                ItemSkillCom[item.TypeID].SkillDatas = item;
                Debug.Log("--------------ItemSkillCom:"+item.ToString());
            }
            index++;
        }

        //删除多余的

        foreach (int key in new List<int>(ItemSkillCom.Keys))
        {
            var isfind = false;
            foreach (var item1 in mainunit.ItemSkillDatas)
            {
                if (key == item1.TypeID)
                {
                    isfind = true;
                    break;
                }
            }
            if (isfind == false)
            {
                ItemSkillCom[key].Destroy();
                ItemSkillCom.Remove(key);
            }

            
        }

        //foreach (var item in ItemSkillCom)
        //{
            
        //}




    }


    //初始化头像信息
    void InitLeftTopHead()
    {
        //leftTopHead = mainUI.GetChild("headInfo").asCom;

        //leftTopHead.GetChild("headbtn").asButton.onClick.Add(() => {
        //    MyInfo myinfo = new MyInfo(center, GameScene.Singleton.GetMyMainUnit());
        //});
    }
    //左上角头像信息 刷新
    void FreshHead()
    {
        int myid = -1;
        if(GameScene.Singleton.m_MyMainUnit != null)
        {
            myid = GameScene.Singleton.m_MyMainUnit.ID;
        }
        int targetid = -1;
        if (GameScene.Singleton.m_TargetUnit != null)
        {
            targetid = GameScene.Singleton.m_TargetUnit.ID;
        }
        MyHeadInfo.FreshData(myid);
        TargetHeadInfo.FreshData(targetid);
    }



    public Dictionary<int, GComponent> BufsRes;
    //刷新buf
    void FreshBuf()
    {
        UnityEntity mainunit = GameScene.Singleton.GetMyMainUnit();
        if (mainunit == null || mainunit.BuffDatas == null || mainunit.BuffDatas.Length <= 0)
        {
            BufsRes.Clear();
            Bufs.RemoveChildren();
            return;
        }

        foreach (var item in mainunit.BuffDatas)
        {
            if (BufsRes.ContainsKey(item.TypeID))
            {
                SetBufData(BufsRes[item.TypeID], item);
            }
            else
            {

                AddBuf(item);
            }
        }

        //删除多余的
        foreach (int key in new List<int>(BufsRes.Keys))
        {
            var isfind = false;
            foreach (var item1 in mainunit.BuffDatas)
            {
                if (key == item1.TypeID)
                {
                    isfind = true;
                    break;
                }
            }
            if (isfind == false)
            {
                RemoveBuf(key);
            }
        }
    }
    void RemoveBuf(int key)
    {
        if (BufsRes.ContainsKey(key))
        {
            Bufs.RemoveChild(BufsRes[key]);
            BufsRes.Remove(key);
        }
        
    }
    void AddBuf(Protomsg.BuffDatas data)
    {
        var clientskill = ExcelManager.Instance.GetBuffIM().GetBIByID(data.TypeID);
        if (clientskill == null || clientskill.IconPath.Length <= 0)
        {
            return;
        }

        GComponent view = UIPackage.CreateObject("GameUI", "buf_icon").asCom;
        view.scale = new Vector2(0.75f, 0.75f);
        Bufs.AddChild(view);
        BufsRes[data.TypeID] = view;
        SetBufData(view, data);
    }
    void SetBufData(GComponent obj,Protomsg.BuffDatas data)
    {
        //图标
        var clientskill = ExcelManager.Instance.GetBuffIM().GetBIByID(data.TypeID);
        if (clientskill != null)
        {
            if (clientskill.IconPath.Length > 0)
            {
                obj.GetChild("icon").asLoader.url = clientskill.IconPath;
            }

            if( clientskill.IconTimeEnable == 0)
            {
                obj.GetChild("pro").asProgress.value = 100;
            }
            else
            {
                obj.GetChild("pro").asProgress.value = (data.RemainTime / data.Time) * 100;
            }
        }
        
        
        if(data.TagNum >= 2)
        {
            obj.GetChild("count").asTextField.text = "" + data.TagNum;
        }
        else
        {
            obj.GetChild("count").asTextField.text = "";
        }
        

    }

    protected List<GGraph> allunitImage = new List<GGraph>();
    protected double LastFreshLittleMapTime = Tool.GetTime();
    void FreshLittleMap()
    {
        if(Tool.GetTime()-LastFreshLittleMapTime <= 1)
        {
            return;
        }
        LastFreshLittleMapTime = Tool.GetTime();

        if (SceneID != GameScene.Singleton.m_SceneID)
        {
            var item = ExcelManager.Instance.GetSceneManager().GetSceneByID(GameScene.Singleton.m_SceneID);
            if(item != null)
            {
                //小地图
                LittleMapCom.GetChild("bg").asLoader.url = item.Little_BG;
                //名字
                LittleMapCom.GetChild("name").asTextField.text = item.Name;
                SceneID = GameScene.Singleton.m_SceneID;
            }
            else
            {
                return;
            }
        }

        //坐标
        if(GameScene.Singleton.m_MyMainUnit != null)
        {
            LittleMapCom.GetChild("pos").asTextField.text = "(" + (int)(GameScene.Singleton.m_MyMainUnit.X) + "," + (int)(GameScene.Singleton.m_MyMainUnit.Y) + ")";
        }



        //GImage aImage = UIPackage.CreateObject("包名","图片名").asImage;
        foreach (var image in allunitImage)
        {
            image.Dispose();
        }
        allunitImage.Clear();

        var allunit = UnityEntityManager.Instance.GetAllUnity();
        foreach(var unit in allunit)
        {
            var color1 = new Color(1, 0.1f, 0.1f);
            var scale = new Vector2(1, 1);
            string iconpath = "Minimap_UnitPin_Foreground_Leader";
            if (unit.Value == GameScene.Singleton.m_MyMainUnit)
            {
                //自己
                iconpath = "Minimap_UnitPin_Foreground_Leader";
                //iconpath = "Minimap_UnitPin_Foreground_Friendly";
                color1 = new Color(0.1f, 1f, 0.1f);
                scale = new Vector2(2, 2);
            }
            else if (unit.Value.TeamID == GameScene.Singleton.m_MyMainUnit.TeamID && GameScene.Singleton.m_MyMainUnit.TeamID > 0)
            {
                //队伍
                color1 = new Color(1f, 0.8f, 0.2f);
                scale = new Vector2(2, 2);
            }
            else
            {
                var isenemy = UnityEntityManager.Instance.CheckIsEnemy(GameScene.Singleton.m_MyMainUnit, unit.Value);
                if(isenemy == true)
                {
                    iconpath = "Minimap_UnitPin_Enemy";
                    color1 = new Color(1, 0.1f, 0.1f);
                }
                else
                {
                    iconpath = "Minimap_UnitPin_Foreground_Friendly";
                    color1 = new Color(0.5f, 1f, 0.5f);
                }
            }

            GGraph aImage = new GGraph();
            aImage.SetSize(5, 5);
            aImage.DrawEllipse(5, 5, color1);

            //GImage aImage = UIPackage.CreateObject("GameUI", iconpath).asImage;
            aImage.pivot = new Vector2(0.5f, 0.5f);
            aImage.pivotAsAnchor = true;
            LittleMapCom.AddChild(aImage);
            if (unit.Value == GameScene.Singleton.m_MyMainUnit)
            {
                aImage.sortingOrder = 100;
                //aImage.scale = new Vector2(2, 2);
            }
            else
            {
                aImage.sortingOrder = 10;
                
            }
            aImage.scale = scale;
            allunitImage.Add(aImage);

            var item = ExcelManager.Instance.GetSceneManager().GetSceneByID(SceneID);
            if (item != null)
            {
                var scaleX = (LittleMapCom.width) / (item.EndX- item.StartX);
                var scaleY = (LittleMapCom.height) / (item.EndY - item.StartY);
                aImage.SetXY((unit.Value.X - item.StartX)* scaleX , LittleMapCom.height - ((unit.Value.Y - item.StartY) * scaleY ));
            }
            
        }


    }

    public void CheckDieUI()
    {
        if(GameScene.Singleton.m_MyMainUnit == null)
        {
            return;
        }
        //死亡
        if(GameScene.Singleton.m_MyMainUnit.IsDeath == 1)
        {
            DieUI.visible = true;
        }
        else
        {
            DieUI.visible = false;
        }

        DieUI.GetChild("time").asTextField.text = (int)(GameScene.Singleton.m_MyMainUnit.RemainReviveTime) + "";
        DieUI.GetChild("needgold").asTextField.text = GameScene.Singleton.m_MyMainUnit.ReviveGold + "";
        DieUI.GetChild("needdiamond").asTextField.text = GameScene.Singleton.m_MyMainUnit.ReviveDiamond + "";
        //
    }

    // Update is called once per frame
    void Update () {
        //if(Input.touchCount > 0)
        //{
        //    Debug.Log("update Input.touchCount:" + Input.touchCount);
        //}
        gPing.text = "" + MyKcp.PingValue;
        FreshSkillUI();
        FreshItemSkillUI();
        FreshHead();
        FreshBuf();
        FreshLittleMap();
        UpdateTeamInfo();
        CheckDieUI();
        //Debug.Log("aaaa time:" + Tool.GetTime());
    }
    //void LateUpdate()
    //{
    //    if (Input.touchCount > 0)
    //    {
    //        Debug.Log("lateupdate Input.touchCount:" + Input.touchCount);
    //    }
    //}
}
