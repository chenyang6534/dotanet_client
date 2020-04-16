using UnityEngine;
using System.Collections;
using Umeng;
public class UMengManager : MonoBehaviour
{
    static string appkey;


    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    }
    // Use this for initialization  
    void Start()
    {
#if UNITY_ANDROID
        //导入app key 标识应用 (Android)  
        appkey = "5e96e5df978eea0718fb785a";
#elif UNITY_IPHONE
         //导入app key 标识应用 (ios)  
        appkey = "5e96e5df978eea0718fb785a";
#endif
        Debug.Log("Umeng:----start---------appkey:"+ appkey);
        //设置Umeng Appkey    
        GA.StartWithAppKeyAndChannelId(appkey, "android umeng1");

        //调试时开启日志 发布时设置为false  
        GA.SetLogEnabled(true);

        //触发统计事件 开始关卡         
        GA.StartLevel("1");

        Debug.Log("test2");
        GA.Event("test2");
        GA.Pay(10, GA.PaySource.Paypal, 1000);
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
