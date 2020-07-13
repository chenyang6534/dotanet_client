using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cocosocket4unity;
using FairyGUI;
using UnityEngine.SceneManagement;
using System;

public class LoginUI : MonoBehaviour {
    public static int UID;
    public static int Characterid;
    protected GComponent mRoot;

    protected GComponent mLineUp;

    protected ServerListInfo SelectServer;

    protected GComponent SelectLayer;
    protected Protomsg.CharacterBaseDatas SelectHeroMsg;

    public int MyHeroMaxLevel = 1;

    protected double m_LastQuickLoginTime;

    //public static string GameLauncherUrl = "http://119.23.8.72:6666";
    public static string GameLauncherUrl = "https://www.game5868.top:6666";
    public static string GameNotice = "";
    public static string winMachineid = "2";
    // Use this for initialization
    void Start () {
        Screen.fullScreen = false;
        m_LastQuickLoginTime = 0;
        SaveDataManager.SetFileName(winMachineid);
        //MyKcp.Instance.Destroy();
        //读取存档
        SaveDataManager.Read();

        UMengManager.Instanse.Event_inlogin();

        //UIPackage.AddPackage("FairyGui/GameUI");
        //GComponent view = UIPackage.CreateObject("GameUI","MyInfo").asCom;
        //以下几种方式都可以将view显示出来：
        //1，直接加到GRoot显示出来
        //GRoot.inst.AddChild(view);
        //view.Center();
        Debug.Log("width:" + Screen.width + " height:" + Screen.height);
        UIPackage.AddPackage("FairyGui/GameUI");

        

        //MyKcp.Instance.Create("119.23.8.72", 1118);
        //MyKcp.Instance.Create("119.23.8.72", 1118);
        mRoot = GetComponent<UIPanel>().ui;
        ToolManager();


        //打开gm窗口
        mLineUp = UIPackage.CreateObject("GameUI", "LineUp").asCom;
        GRoot.inst.AddChild(mLineUp);
        mLineUp.visible = false;
        mLineUp.xy = Tool.GetPosition(0.5f, 0.5f);
        mLineUp.GetChild("close").asButton.onClick.Add(() =>
        {
            Tool.NoticeWindonw("你确定要退出排队吗?", () =>
            {
                Protomsg.CS_CancelLineUp msg1 = new Protomsg.CS_CancelLineUp();
                MyKcp.Instance.SendMsg("Login", "CS_CancelLineUp", msg1);
                mLineUp.visible = false;
            });
            
        });
        mLineUp.GetChild("fresh").asButton.onClick.Add(() =>
        {
            Tool.NoticeWindonw("你确定要查看排在你前面的玩家人数吗?", () =>
            {
                Protomsg.CS_GetLineUpFrontCount msg1 = new Protomsg.CS_GetLineUpFrontCount();
                MyKcp.Instance.SendMsg("Login", "CS_GetLineUpFrontCount", msg1);
            });

        });


        //mRoot.GetChild("center")..AddChild(view);
        mRoot.GetChild("login").asButton.onClick.Add(()=> {

            


            //两秒内不能重复登录
            if (Tool.GetTime()-m_LastQuickLoginTime <= 2 || SelectServer == null)
            {
                return;
            }
            UMengManager.Instanse.Event_click_loginbtn();

            m_LastQuickLoginTime = Tool.GetTime();

            //MyKcp.Instance.Destroy();
            //MyKcp.Instance.Create(SelectServer.ip, SelectServer.port);
            MyKcp.Create(SelectServer.Ip, SelectServer.Port);

            Protomsg.CS_MsgQuickLogin msg1 = new Protomsg.CS_MsgQuickLogin();
            //msg1.Machineid = "100001"; //PA
            //msg1.Machineid = "10000";   //剑圣 (技能特效完结)
            //msg1.Machineid = "10002";   //小黑 (技能特效完结)
            //msg1.Machineid = "10003";   //虚空 (技能特效完结)
            //msg1.Machineid = "10004";   //混沌骑士 (技能特效完结)
            //msg1.Machineid = "10005";   //熊战士   (技能特效完结)
            //msg1.Machineid = "10006";   //血魔    (技能特效完结)
            //msg1.Machineid = "10007";   //小娜迦 (技能特效完结)
            //msg1.Machineid = "10008";   //小小 (技能特效完结)
            //msg1.Machineid = "10009";   //风行 (技能特效完结)
            //msg1.Machineid = "10010";   //帕克 (技能特效完结)
            //msg1.Machineid = "10011";   //影魔 (技能特效完结)
            //msg1.Machineid = "10012";   //幽鬼 (技能特效完结)
            //msg1.Machineid = "10013";   //火枪 (技能特效完结)
            //msg1.Machineid = "10014";   //斧王 (技能特效完结)
            //msg1.Machineid = "10015";   //月骑 (技能特效完结)
            //msg1.Machineid = "10016";   //毒龙 (技能特效完结)
            //msg1.Machineid = "10017";   //蓝猫 (技能特效完结)
            //msg1.Machineid = "10018";   //瘟疫法师
            //msg1.Machineid = "10019";   //天怒法师
            Environment.GetCommandLineArgs();
            msg1.Machineid = SystemInfo.deviceUniqueIdentifier;
            if (Application.platform == RuntimePlatform.WindowsPlayer ||
            Application.platform == RuntimePlatform.WindowsEditor)
            {
                msg1.Machineid += winMachineid;
            }
            msg1.Platform = "test";
            MyKcp.Instance.SendMsg("Login", "CS_MsgQuickLogin", msg1);
            UnityEngine.Debug.Log("login onClick");
        });


        InitServerList();

        MsgManager.Instance.AddListener("SC_Logined", new HandleMsg(this.Logined));

        MsgManager.Instance.AddListener("SC_SelectCharacterResult", new HandleMsg(this.SelectCharacterResult));

        MsgManager.Instance.AddListener("SC_NeedLineUp", new HandleMsg(this.SC_NeedLineUp));
        MsgManager.Instance.AddListener("SC_GetLineUpFrontCount", new HandleMsg(this.SC_GetLineUpFrontCount));


    }

    [Serializable]
    public class ServerListInfo
    {
        public string Name;
        public string Ip;
        public int Port;
        public int State; // 1表示空闲 2表示拥挤 3表示已满
        public int Index;//唯一索引
        public ServerListInfo(string name,string ip, int port)
        {
            this.Ip = ip;
            this.Port = port;
            this.Name = name;
        }


    }
    [Serializable]
    public class ServerListInfoArr
    {
        public ServerListInfo[] Servers;
        public int Code;
        public string UpdateUrl;
    }

    void InitServerList()
    {
        
        //测试数据
        //ServerListInfo[] allServerList = {new ServerListInfo("本地服","127.0.0.1",1118), new ServerListInfo("外网", "119.23.8.72", 1118) };
        ////ServerListInfo[] allServerList = { new ServerListInfo("删档测试服", "119.23.8.72", 1118) };

        //ServerListInfoArr t1 = new ServerListInfoArr();
        //t1.Servers = allServerList;
        //var jsonstr = JsonUtility.ToJson(t1);

        //获取公告
        StartCoroutine(Tool.SendGet(GameLauncherUrl+"/getnotice", (WWW data) => {
            //data.text

            if (data.error != null)
            {
                Debug.Log("获取失败:" + data.error);
                return;
            }
            Debug.Log("公告:" + data.text);
            if(data.text != null && data.text.Length > 0)
            {

                GameNotice = data.text;

                //NetNotice
                var createguildwd = UIPackage.CreateObject("GameUI", "NetNotice").asCom;
                GRoot.inst.AddChild(createguildwd);
                createguildwd.xy = Tool.GetPosition(0.5f, 0.5f);
                createguildwd.GetChild("close").asButton.onClick.Add(() =>
                {
                    createguildwd.Dispose();
                });
                var test = data.text;
                //test = "    本次测试为[color=#FF1111]删档测试[/color],截止到[color=#FF1111]6.14号24点[/color]停止测试!\r\n    游戏过程中如有任何建议或者不满都可以去[color=#FF1111]好游快爆[/color]官方论坛发帖留言，我们会认真对待每一位玩家的意见与建议！";
                
                //默认文字
                createguildwd.GetChild("words").asTextField.text = test;
            }

        }));

        var platformstr = "Android";
        if (Application.platform == RuntimePlatform.WindowsPlayer ||
            Application.platform == RuntimePlatform.WindowsEditor)
        {
            platformstr = "Win32";
        }

            //获取服务器列表
            StartCoroutine(Tool.SendGet(GameLauncherUrl + "/getserverlist?Platform="+ platformstr+"&Version=1.0.0", (WWW data) => {
                //data.text

                if (data.error != null)
                {
                    Debug.Log("获取失败:" + data.error);
                    return;
                }

                //默认id
                var defaultid = 0;
                var t2 = JsonUtility.FromJson<ServerListInfoArr>(data.text);

                if (t2.Code != 1)
                {
                    Debug.Log("获取失败");
                    return;
                }
                //ServerListInfo[] allServerList = { new ServerListInfo("删档测试服", "119.23.8.72", 1118) };
                //ServerListInfo[] allServerList = { new ServerListInfo("本地服", "127.0.0.1", 1118), new ServerListInfo("外网", "119.23.8.72", 1118) };

            var serverlist = t2.Servers;
            Debug.Log("-------------len:" + serverlist.Length);
            var defaultserver = serverlist[defaultid];
            SelectServer = defaultserver;
            //建立连接
            //MyKcp.Create(defaultserver.ip, defaultserver.port);
            //
            string[] names = new string[serverlist.Length];
            string[] values = new string[serverlist.Length];
            for (var i = 0; i < serverlist.Length; i++)
            {
                names[i] = serverlist[i].Name;
                values[i] = "" + i;
            }
            GComboBox combo = mRoot.GetChild("server").asComboBox;
            combo.items = names;
            combo.values = values;
            combo.selectedIndex = defaultid;
            combo.onChanged.Add(() => {
                SelectServer = serverlist[combo.selectedIndex];
            });
        }));

        //Debug.Log("-------------InitServerList:" + jsonstr);

        
    }
	
	
    void OnDestroy()
    {
        Debug.Log("main OnDestroy:");
        MsgManager.Instance.RemoveListener("SC_Logined");
        MsgManager.Instance.RemoveListener("SC_SelectCharacterResult");
        MsgManager.Instance.RemoveListener("SC_NeedLineUp");
        MsgManager.Instance.RemoveListener("SC_GetLineUpFrontCount");
    }

    //开放的英雄
    public int[] openherotypeids1 = {3,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20};
    public List<Protomsg.CharacterBaseDatas> AllOpenHeros = new List<Protomsg.CharacterBaseDatas>();
    public void initOpenHeros(Google.Protobuf.Collections.RepeatedField<Protomsg.CharacterBaseDatas> HaveHeros)
    {
        AllOpenHeros.Clear();
        for(var i = 0; i < openherotypeids1.Length; i++)
        {
            var item = openherotypeids1[i];
            //Debug.Log("--------11----openherotypeids1:" + item);

            bool has = false;
            foreach(var havehero in HaveHeros)
            {
                if( item == havehero.Typeid)
                {
                    AllOpenHeros.Add(havehero);
                    has = true;
                    break;
                }
            }
            if(has == false)
            {
                var hero = new Protomsg.CharacterBaseDatas();
                hero.Typeid = item;
                hero.Characterid = -1;
                hero.Name = "";
                hero.Level = 1;
                AllOpenHeros.Add(hero);
            }

        }
    }
    //选择英雄
    public void SelectHero(Protomsg.CharacterBaseDatas hero)
    {
        SelectHeroMsg = hero;
        SaveDataManager.sData.SelectHeroTypeID = hero.Typeid;

        freshSelectHero();
    }

    //刷新选择英雄界面
    public void freshSelectHero()
    {
        var herolist = SelectLayer.GetChild("heros_list").asList;
        herolist.RemoveChildren();
        foreach (var item in AllOpenHeros)
        {
            var heroinfo = ExcelManager.Instance.GetUnitInfoManager().GetUnitInfoByID(item.Typeid);
            if (heroinfo == null)
            {
                Debug.Log("no hero:" + item.Name);
                continue;
            }
            //所有英雄中的最大等级
            if(item.Level > MyHeroMaxLevel)
            {
                MyHeroMaxLevel = item.Level;
            }

            var heroiconcom = UIPackage.CreateObject("Package1", "HeroIcon").asCom;


            //Debug.Log("------------hero:" + heroinfo.IconPath);
            heroiconcom.GetChild("pic").asLoader.url = heroinfo.IconPath;
            heroiconcom.GetChild("pic").asLoader.onClick.Add(() =>
            {
                AudioManager.Am.Play2DSound(AudioManager.Sound_Click);
                //选择
                SelectHero(item);
            });
            heroiconcom.GetChild("level").asTextField.text = item.Level + "";
            if (item.Typeid == SaveDataManager.sData.SelectHeroTypeID)
            {
                SelectHeroMsg = item;
                heroiconcom.GetChild("select").asImage.visible = true;

                //显示选择的英雄信息
                SelectLayer.GetChild("selectheroicon").asCom.GetChild("pic").asLoader.url = heroinfo.IconPath;
                SelectLayer.GetChild("selectheroicon").asCom.GetChild("level").asTextField.text = item.Level + "";
                SelectLayer.GetChild("heroname").asTextField.text = heroinfo.HeroName;
                SelectLayer.GetChild("player_name").asTextField.text = item.Name;
                //主属性(1:力量 2:敏捷 3:智力)
                if (heroinfo.AttributePrimary == 1)
                {
                    SelectLayer.GetChild("type_attribute").asTextField.text = "力量";
                }
                else if(heroinfo.AttributePrimary == 2)
                {
                    SelectLayer.GetChild("type_attribute").asTextField.text = "敏捷";
                }
                else if (heroinfo.AttributePrimary == 3)
                {
                    SelectLayer.GetChild("type_attribute").asTextField.text = "智力";
                }
                SelectLayer.GetChild("attack_range").asTextField.text = heroinfo.Attack_Range;
                SelectLayer.GetChild("des").asTextField.text = heroinfo.Des;
                //技能
                var skillcom = SelectLayer.GetChild("skill_list").asList;
                skillcom.RemoveChildren(0,-1,true);
                var skills_str = heroinfo.Skills_ID.Split(',');
                foreach(var skilltype in skills_str)
                {
                    var clientitem = ExcelManager.Instance.GetSkillManager().GetSkillByID(int.Parse(skilltype));
                    if (clientitem != null)
                    {
                        var onedropitem = UIPackage.CreateObject("GameUI", "HeroInfo_Skill").asButton;
                        onedropitem.icon = clientitem.IconPath;
                        onedropitem.GetChild("level").asTextField.text = "";
                        onedropitem.onClick.Add(() => {
                            //Debug.Log("onClick");
                            if (clientitem.TypeID != -1)
                            {
                                new SkillInfo(clientitem.TypeID);
                            }
                        });
                        skillcom.AddChild(onedropitem);
                    }
                }
                
            }
            else
            {
                heroiconcom.GetChild("select").asImage.visible = false;
            }
            herolist.AddChild(heroiconcom);
        }
    }

   
    //显示选择英雄界面
    public void showSelectHero()
        {
            SelectLayer = UIPackage.CreateObject("Package1", "SelectHero").asCom;
            GRoot.inst.AddChildAt(SelectLayer,0);
            Vector2 screenPos = new Vector2(Screen.width / 2, Screen.height / 2);
            Vector2 logicScreenPos = GRoot.inst.GlobalToLocal(screenPos);
            SelectLayer.xy = logicScreenPos;

            //---设置默认选择英雄
            if(SaveDataManager.sData.SelectHeroTypeID <= 0)
            {
                SaveDataManager.sData.SelectHeroTypeID = openherotypeids1[0];
            }

            freshSelectHero();

            SelectLayer.GetChild("startbtn").asButton.onClick.Add(()=> {

                UMengManager.Instanse.Event_click_startgame();
                if(SelectHeroMsg.Level >= MyHeroMaxLevel)
                {
                    UMengManager.Instanse.SetPreProperty(true);
                }
                else
                {
                    UMengManager.Instanse.SetPreProperty(false);
                }

                if (SelectHeroMsg.Name.Length <= 0)
                {
                    //输入名字
                    var inputnamecom = UIPackage.CreateObject("Package1", "InputName").asCom;
                    GRoot.inst.AddChild(inputnamecom);
                    inputnamecom.xy = logicScreenPos;

                    inputnamecom.GetChild("ok").asButton.onClick.Add(() =>
                    {
                        var txt = inputnamecom.GetChild("input").asTextInput.text;
                        if (txt.Length <= 0)
                        {
                            Tool.NoticeWords("请输入名字！",null);
                            return;
                        }
                        if (Tool.IsChineseOrNumberOrWord(txt) == false)
                        {
                            Tool.NoticeWords("名字不含有中文,字母,数字以外的其他字符！",null);
                            return;
                        }
                        SelectHeroMsg.Name = txt;
                        Debug.Log("name:" + txt);
                        

                        Protomsg.CS_SelectCharacter msg1 = new Protomsg.CS_SelectCharacter();
                        msg1.SelectCharacter = SelectHeroMsg;
                        MyKcp.Instance.SendMsg("Login", "CS_SelectCharacter", msg1);

                        SelectHeroMsg.Name = "";//清空名字
                        inputnamecom.Dispose();
                    });

                    inputnamecom.GetChild("input").asTextInput.onKeyDown.Add((EventContext context) =>
                    {
                        if (context.inputEvent.keyCode == KeyCode.Return)
                        {
                            inputnamecom.GetChild("ok").asButton.onClick.Call();
                        }
                    });
                }
                else
                {
                    Protomsg.CS_SelectCharacter msg1 = new Protomsg.CS_SelectCharacter();
                    msg1.SelectCharacter = SelectHeroMsg;
                    MyKcp.Instance.SendMsg("Login", "CS_SelectCharacter", msg1);
                
                }

            
            });
        }

    public bool Logined(Protomsg.MsgBase d1)
    {
        Google.Protobuf.IMessage IMperson = new Protomsg.SC_Logined();
        Protomsg.SC_Logined p1 = (Protomsg.SC_Logined)IMperson.Descriptor.Parser.ParseFrom(d1.Datas);
        UID = p1.Uid;
        if(p1.Code != 1)
        {
            Tool.NoticeWords("登录失败！请2秒钟后重试", null);
            Debug.Log("login fail");
        }
        else
        {
            
            Protomsg.CS_SelectCharacter msg1 = new Protomsg.CS_SelectCharacter();
            msg1.SelectCharacter = new Protomsg.CharacterBaseDatas();
            Debug.Log("Logined:" + p1.Characters);
            initOpenHeros(p1.Characters);
            showSelectHero();
            
        }
        
        return false; //中断解析数据
    }
    //MsgManager.Instance.AddListener("SC_NeedLineUp", new HandleMsg(this.SC_NeedLineUp));
    //    MsgManager.Instance.AddListener("SC_GetLineUpFrontCount", new HandleMsg(this.SC_GetLineUpFrontCount));
    public bool SC_NeedLineUp(Protomsg.MsgBase d1)
    {
        Google.Protobuf.IMessage IMperson = new Protomsg.SC_NeedLineUp();
        Protomsg.SC_NeedLineUp p1 = (Protomsg.SC_NeedLineUp)IMperson.Descriptor.Parser.ParseFrom(d1.Datas);

        mLineUp.visible = true;
        mLineUp.GetChild("playercount").asTextField.text = p1.FrontCount + "";

        return true;
    }
    public bool SC_GetLineUpFrontCount(Protomsg.MsgBase d1)
    {
        Google.Protobuf.IMessage IMperson = new Protomsg.SC_GetLineUpFrontCount();
        Protomsg.SC_GetLineUpFrontCount p1 = (Protomsg.SC_GetLineUpFrontCount)IMperson.Descriptor.Parser.ParseFrom(d1.Datas);
        mLineUp.visible = true;
        mLineUp.GetChild("playercount").asTextField.text = p1.FrontCount + "";

        return true;
    }

    public bool SelectCharacterResult(Protomsg.MsgBase d1)
    {
        Google.Protobuf.IMessage IMperson = new Protomsg.SC_SelectCharacterResult();
        Protomsg.SC_SelectCharacterResult p1 = (Protomsg.SC_SelectCharacterResult)IMperson.Descriptor.Parser.ParseFrom(d1.Datas);
        Characterid = p1.Characterid;

        

        Debug.Log("SelectCharacterid :"+ Characterid);
        if (p1.Code != 1)
        {
            Debug.Log("SelectCharacterResult fail"+ p1.Error);
            var word = "";
            if(p1.Error == 3)
            {
                word = "这个名字已经存在,请重新取名！";
            }
            else if(p1.Error == 1)
            {
                word = "找不到该角色";
            }
            else if (p1.Error == 4)
            {
                word = "名字中含有非法字符!";
            }
            Tool.NoticeWords(word,null);

            
        }
        else
        {
            
            SaveDataManager.Save();
            SceneManager.LoadScene(1);
            SelectLayer.Dispose();
            mLineUp.Dispose();
            Debug.Log("main OnDestroy:aaaa");
            

            Debug.Log("main OnDestroy:bbbbb");
        }

        return false; //中断解析数据
    }

    void ToolManager()
    {
        Debug.Log("platform:" + Application.platform);
        mRoot.GetChild("gmset").visible = false;


        //if (Application.platform == RuntimePlatform.Android)
        if (Application.platform == RuntimePlatform.IPhonePlayer ||
            Application.platform == RuntimePlatform.WindowsPlayer ||
            Application.platform == RuntimePlatform.WindowsEditor)
        {
            mRoot.GetChild("gmset").visible = true;
            mRoot.GetChild("gmset").asButton.onClick.Add(() => {
                //打开gm窗口
                var gmtool = UIPackage.CreateObject("GameUI", "GMTool").asCom;
                GRoot.inst.AddChild(gmtool);
                gmtool.xy = Tool.GetPosition(0.5f, 0.5f);
                gmtool.GetChild("close").asButton.onClick.Add(() =>
                {
                    gmtool.Dispose();
                });
                //开服
                gmtool.GetChild("opengame").asButton.onClick.Add(() =>
                {
                    Tool.NoticeWindonw("你确定要打开服务器吗?", () =>
                    {
                        //获取服务器列表
                        StartCoroutine(Tool.SendGet(GameLauncherUrl + "/startgame", (WWW data) => {
                            //data.text
                            if (data.error != null)
                            {
                                Tool.NoticeWords("打开失败:" + data.error, null);
                                return;
                            }
                            Tool.NoticeWords("开服服务器返回:" + data.text, null);
                        }));
                    });
                    
                });
                //关服
                gmtool.GetChild("closegame").asButton.onClick.Add(() =>
                {
                    Tool.NoticeWindonw("你确定要关闭服务器吗?", () =>
                    {
                        //获取服务器列表
                        StartCoroutine(Tool.SendGet(GameLauncherUrl + "/endgame", (WWW data) => {
                            //data.text
                            if (data.error != null)
                            {
                                Tool.NoticeWords("关服失败:" + data.error, null);
                                return;
                            }
                            Tool.NoticeWords("关服服务器返回:" + data.text, null);
                        }));
                    });
                    
                });

                //刷新人数
                gmtool.GetChild("freshplayercount").asButton.onClick.Add(() =>
                {
                    //获取服务器列表
                    StartCoroutine(Tool.SendGet("http://119.23.8.72:9999/sd", (WWW data) => {
                        //data.text
                        if (data.error != null)
                        {
                            Tool.NoticeWords("刷新人数失败:" + data.error, null);
                            return;
                        }
                        gmtool.GetChild("playercount").asTextField.text = data.text;
                        Tool.NoticeWords("刷新人数服务器返回:" + data.text, null);
                    }));
                });

                //修改公告
                gmtool.GetChild("editnotice").asButton.onClick.Add(() =>
                {
                    var createguildwd = UIPackage.CreateObject("GameUI", "EditorGuildNotice").asCom;
                    GRoot.inst.AddChild(createguildwd);
                    createguildwd.xy = Tool.GetPosition(0.5f, 0.5f);
                    createguildwd.GetChild("close").asButton.onClick.Add(() =>
                    {
                        createguildwd.Dispose();
                    });

                    //默认文字
                    createguildwd.GetChild("input").asTextInput.text = GameNotice;

                    createguildwd.GetChild("create").asButton.onClick.Add(() =>
                    {
                        var txt = createguildwd.GetChild("input").asTextInput.text;
                        //if (txt.Length <= 0)
                        //{
                        //    Tool.NoticeWords("请输入文字！", null);
                        //    return;
                        //}
                        Tool.NoticeWindonw("你确定修改公告吗?", () =>
                        {
                            //创建 
                            Debug.Log(txt);
                            //获取服务器列表
                            StartCoroutine(Tool.SendGet(GameLauncherUrl + "/setnotice?txt="+ txt, (WWW data) => {
                                //data.text
                                if (data.error != null)
                                {
                                    Tool.NoticeWords("修改公告失败:" + data.error, null);
                                    return;
                                }
                                Tool.NoticeWords("修改公告服务器返回:" + data.text,null);
                            }));
                            createguildwd.Dispose();
                        });


                    });
                });

            });
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0)
        {
            Debug.Log("login update Input.touchCount:" + Input.touchCount);
        }
        //Debug.Log("update:");
        MsgManager.Instance.UpdateMessage();


    }
    void LateUpdate()
    {
        if (Input.touchCount > 0)
        {
            Debug.Log("login lateupdate Input.touchCount:" + Input.touchCount);
        }
    }


}
