using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cocosocket4unity;
using FairyGUI;
using UnityEngine.SceneManagement;

public class LoginUI : MonoBehaviour {
    public static int UID;
    protected GComponent mRoot;
    public string aa;
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
        Debug.Log("Logined:" + aa);
        Google.Protobuf.IMessage IMperson = new Protomsg.SC_Logined();
        Protomsg.SC_Logined p1 = (Protomsg.SC_Logined)IMperson.Descriptor.Parser.ParseFrom(d1.Datas);
        UID = p1.Uid;
        if(p1.Code != 1)
        {
            Debug.Log("login fail");
        }
        else
        {
            SceneManager.LoadScene(1);
        }
        
        return false; //中断解析数据
    }


}
