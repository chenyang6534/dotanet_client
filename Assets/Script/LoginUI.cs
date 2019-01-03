using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cocosocket4unity;
using FairyGUI;

public class LoginUI : MonoBehaviour {

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
        

        MsgManager.Instance.AddListener("LoginMsg", new HandleMsg(this.HandleT1));
        
    }
	
	// Update is called once per frame
	void Update () {
        MsgManager.Instance.UpdateMessage();

    }


    public bool HandleT1(Protomsg.MsgBase d1)
    {
        Debug.Log("HandleT1:" + aa);
        return true;
    }


}
