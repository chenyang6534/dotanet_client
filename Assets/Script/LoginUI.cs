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
        mRoot = GetComponent<UIPanel>().ui;
        mRoot.GetChild("n6").asButton.onClick.Add(()=> {

            
            Protomsg.CS_MsgQuickLogin msg1 = new Protomsg.CS_MsgQuickLogin();
            msg1.Machineid = "10000";
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
                msg1.SelectCharacter.Typeid = 1;
                msg1.SelectCharacter.Name = "test1234";

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
            Debug.Log("SelectCharacterResult fail");
        }
        else
        {
            SceneManager.LoadScene(1);
        }

        return false; //中断解析数据
    }

    
}
