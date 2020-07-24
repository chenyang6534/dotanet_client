using UnityEngine;
using System.Collections;
using Umeng;
public class UMengManager : MonoBehaviour
{
    static string appkey;

    static public UMengManager Instanse;
    //UMengManager.Instanse
    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
        Init();
        Instanse = this;
    }
    //是否是主英雄
    public void SetPreProperty(bool ismainhero)
    {

#if UNITY_ANDROID
        var ismain = new JSONObject();
        ismain["ismainhero"] = ismainhero;
        GA.RegisterPreProperties(ismain);
#endif
    }

    public void Event_inlogin()
    {
#if UNITY_ANDROID
        GA.Event("in_loginui");
#endif
    }
    public void Event_click_loginbtn()
    {
#if UNITY_ANDROID
        GA.Event("click_loginbtn");
#endif
    }
    public void Event_click_startgame()
    {
#if UNITY_ANDROID
        GA.Event("click_startgame");
#endif
    }

    public void Event_goin_scene(string scenename)
    {
#if UNITY_ANDROID
        GA.Event("goin_scene",scenename);
#endif
    }

    public void Event_pop_dieui()
    {
#if UNITY_ANDROID
        GA.Event("pop_dieui");
#endif
    }
    public void Event_click_goldrevive()
    {
#if UNITY_ANDROID
        GA.Event("click_goldrevive");
#endif
    }
    public void Event_click_masonryrevive()
    {
#if UNITY_ANDROID
        GA.Event("click_masonryrevive");
#endif
    }
    public void Event_click_taskbtn()
    {
#if UNITY_ANDROID
        GA.Event("click_taskbtn");
#endif
    }
    public void Event_click_myinfo()
    {
#if UNITY_ANDROID
        GA.Event("click_myinfo");
#endif
    }
    public void Event_hide_dieui()
    {
#if UNITY_ANDROID
        GA.Event("hide_dieui");
#endif
    }
    public void Event_levelup(string level)
    {
#if UNITY_ANDROID
        GA.Event("levelup",level);
#endif
    }

    public void Event_buy_store(string name)
    {
#if UNITY_ANDROID
        GA.Event("buy_store",name);
#endif
    }

    // Use this for initialization  
    void Init()
    {
        //GA.StartLevel("1");
        //GA.RegisterPreProperties();
#if UNITY_ANDROID
        //导入app key 标识应用 (Android)  
        appkey = "5e96e5df978eea0718fb785a";
        Debug.Log("Umeng:----start---------appkey:"+ appkey);
        //设置Umeng Appkey    
        GA.StartWithAppKeyAndChannelId(appkey, "android umeng1");

        //调试时开启日志 发布时设置为false  
        GA.SetLogEnabled(true);

        //触发统计事件 开始关卡         
        //GA.StartLevel("1");
        
        
        //GA.Event("test2");
        //GA.Pay(10, GA.PaySource.Paypal, 1000);
#elif UNITY_IPHONE
         //导入app key 标识应用 (ios)  
        appkey = "5e96e5df978eea0718fb785a";
        Debug.Log("Umeng:----start---------appkey:"+ appkey);
        //设置Umeng Appkey    
        GA.StartWithAppKeyAndChannelId(appkey, "android umeng1");

        //调试时开启日志 发布时设置为false  
        //GA.SetLogEnabled(true);

        ////触发统计事件 开始关卡         
        //GA.StartLevel("1");

        
        //GA.Event("test2");
        //GA.Pay(10, GA.PaySource.Paypal, 1000);
#endif

    }

    // Update is called once per frame  
    //void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Escape))
    //    {
    //        //结束程序  
    //        GA.FinishLevel("your level ID");
    //        Application.Quit();
    //    }
    //}
#if UNITY_ANDROID

    void OnApplicationPause(bool isPause)
    {

        //Debug.Log("Umeng:OnApplicationPause" + isPause);
        if (isPause)
        {
            //Debug.Log("Umeng:----onPause");
            GA.onPause();
        }
        else
        {
            //Debug.Log("Umeng:----onResume");
            GA.onResume();
        }
    }
    void OnApplicationQuit()
    {
        //Debug.Log("Umeng:OnApplicationQuit");
        GA.onKillProcess();
    }
#endif
}
