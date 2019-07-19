using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cocosocket4unity;
using FairyGUI;
using UnityEngine.SceneManagement;

public class LoginUI : MonoBehaviour {
    public static int UID;
    public static int Characterid;
    protected GComponent mRoot;
	// Use this for initialization
	void Start () {


        MyKcp.Instance.Create("127.0.0.1", 1118);
        //MyKcp.Instance.Create("119.23.8.72", 1118);
        mRoot = GetComponent<UIPanel>().ui;
        mRoot.GetChild("n6").asButton.onClick.Add(()=> {

            
            Protomsg.CS_MsgQuickLogin msg1 = new Protomsg.CS_MsgQuickLogin();
            //msg1.Machineid = "100001"; //PA
            //msg1.Machineid = "10000";   //剑圣 (技能特效完结)
            //msg1.Machineid = "10002";   //小黑 (技能特效完结)
            //msg1.Machineid = "10003";   //虚空 (技能特效完结)
            //msg1.Machineid = "10004";   //混沌骑士 (技能特效完结)
            //msg1.Machineid = "10005";   //熊战士   (技能特效完结)
            //msg1.Machineid = "10006";   //血魔    (技能特效完结)
            //msg1.Machineid = "10007";   //小娜迦 (技能特效完结)
            //msg1.Machineid = "10008";   //小小
            //msg1.Machineid = "10009";   //风行 (技能特效完结)
            //msg1.Machineid = "10010";   //帕克 (技能特效完结)
            //msg1.Machineid = "10011";   //影魔 (技能特效完结)
            //msg1.Machineid = "10012";   //幽鬼
            //msg1.Machineid = "10013";   //火枪 (技能特效完结)
            msg1.Machineid = "10014";   //斧王 (技能特效完结)
            //msg1.Machineid = "10015";   //月骑 (技能特效完结)
            //msg1.Machineid = "10016";   //毒龙 (技能特效完结)
            //msg1.Machineid = "10017";   //蓝猫 (技能特效完结)
            //msg1.Machineid = "10018";   //瘟疫法师
            //msg1.Machineid = "10019";   //天怒法师
            msg1.Platform = "win32";
            MyKcp.Instance.SendMsg("Login", "CS_MsgQuickLogin", msg1);
            UnityEngine.Debug.Log("login onClick");
        });
        

        MsgManager.Instance.AddListener("SC_Logined", new HandleMsg(this.Logined));

        MsgManager.Instance.AddListener("SC_SelectCharacterResult", new HandleMsg(this.SelectCharacterResult));
        


    }
	
	// Update is called once per frame
	void Update () {
        //Debug.Log("update:");
        MsgManager.Instance.UpdateMessage();
        

    }
    void OnDestroy()
    {
        //Debug.Log("OnDestroy:");
        MsgManager.Instance.RemoveListener("SC_Logined");
    }


    public bool Logined(Protomsg.MsgBase d1)
    {
        Google.Protobuf.IMessage IMperson = new Protomsg.SC_Logined();
        Protomsg.SC_Logined p1 = (Protomsg.SC_Logined)IMperson.Descriptor.Parser.ParseFrom(d1.Datas);
        UID = p1.Uid;
        if(p1.Code != 1)
        {
            Debug.Log("login fail");
        }
        else
        {
            Protomsg.CS_SelectCharacter msg1 = new Protomsg.CS_SelectCharacter();
            msg1.SelectCharacter = new Protomsg.CharacterBaseDatas();
            //foreach (var item in p1.NewUnits)
            if (p1.Characters.Count <= 0)
            {
                msg1.SelectCharacter.Characterid = -1;
                msg1.SelectCharacter.Typeid = 22;
                msg1.SelectCharacter.Name = "test天怒法师";

                Debug.Log("create");
            }
            else
            {
                msg1.SelectCharacter = p1.Characters[0];

                Debug.Log("select");
            }
            MyKcp.Instance.SendMsg("Login", "CS_SelectCharacter", msg1);
            
            //SceneManager.LoadScene(1);
        }
        
        return false; //中断解析数据
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
        }
        else
        {
            SceneManager.LoadScene(1);
        }

        return false; //中断解析数据
    }

    
}
