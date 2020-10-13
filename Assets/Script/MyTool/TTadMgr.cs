//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
////using ByteDance.Union;
//using cocosocket4unity;

//public class TTadMgr : MonoBehaviour
//{

//    public static string rewardadid = "945508163";//正式
//    //public static string rewardadid = "901121430";//测试
//    public delegate void StartPlayRewardVideoResult(bool issuc);
//    public delegate void EndPlayRewardVideoResult(bool issuc, bool novedio_usezhuanshi, bool useitem);//是否成功 是否没有视频而使用砖石替代
//    public static EndPlayRewardVideoResult CurRewardVideoPlay = null;
//    //private AdNative adNative;
//    //private RewardVideoAd rewardAd;

//    public string loaderrormsg;

//    static public TTadMgr Instanse;
//    void Awake()
//    {
//        Instanse = this;
//    }
//    // Use this for initialization
//    void Start()
//    {

//        LoadRewardAd();//提前加载
//        MsgManager.Instance.AddListener("SC_UseWatchVedioItem", new HandleMsg(this.SC_UseWatchVedioItem));

//    }

//    // Update is called once per frame
//    void Update()
//    {

//    }
//    //    private AdNative AdNative
//    //    {
//    //        get
//    //        {
//    //            if (this.adNative == null)
//    //            {
//    //                this.adNative = SDK.CreateAdNative();
//    //            }
//    //#if UNITY_ANDROID
//    //            Debug.Log("111111111111111");
//    //            SDK.RequestPermissionIfNecessary();
//    //            Debug.Log("222222222222222");
//    //#endif
//    //            return this.adNative;
//    //        }
//    //    }

//    public void LoadRewardAd()
//    {
//#if UNITY_ANDROID

//#endif
//    }
//    public int WatchVedioState = 0;//0无 1请求服务器中
//    public bool SC_UseWatchVedioItem(Protomsg.MsgBase d1)
//    {
//        Google.Protobuf.IMessage IMperson = new Protomsg.SC_UseWatchVedioItem();
//        Protomsg.SC_UseWatchVedioItem p1 = (Protomsg.SC_UseWatchVedioItem)IMperson.Descriptor.Parser.ParseFrom(d1.Datas);
//        Debug.Log("SC_UseWatchVedioItem:" + p1);
//        if (p1.IsSucc == true)//成功
//        {
//            if (TTadMgr.CurRewardVideoPlay != null)
//            {
//                TTadMgr.CurRewardVideoPlay(true, false, true);
//            }

//            //Tool.NoticeWords("请输入正确的价格！", null);
//        }
//        else//失败
//        {
//            ShowVedio2(TTadMgr.CurRewardVideoPlay);
//        }
//        WatchVedioState = 0;

//        return true;
//    }
//    /// <summary>
//    /// Show the reward Ad.
//    /// </summary>
//    /// 
//    public void ShowVedio2(EndPlayRewardVideoResult ep)
//    {
//        //if (this.rewardAd == null)
//        //{
//        //    Debug.Log("请先加载广告");
//        //    UMengManager.Instanse.Event_watch_vedio(this.loaderrormsg);
//        //    new NoVedioNotice(this.loaderrormsg, (code) => {
//        //        if (code == 1)//是
//        //        {
//        //            if (ep != null)
//        //            {
//        //                ep(true, true,false);
//        //            }
//        //        }
//        //        else//否
//        //        {

//        //        }
//        //    });

//        //    TTadMgr.Instanse.LoadRewardAd();
//        //    return;
//        //}

//        //if (this.rewardAd.IsDownloaded == true)
//        //{
//        //    this.rewardAd.ShowRewardVideoAd();

//        //    TTadMgr.CurRewardVideoPlay = ep;
//        //}
//        //else
//        //{
//        //    new NoVedioNotice("还未缓存", (code) => {
//        //        if (code == 1)//是
//        //        {
//        //            if (ep != null)
//        //            {
//        //                ep(true, true,false);
//        //            }
//        //        }
//        //        else//否
//        //        {

//        //        }
//        //    });
//        //}
//    }
//    public void ShowVideo(EndPlayRewardVideoResult ep)
//    {
//        if (WatchVedioState == 0)
//        {
//            WatchVedioState = 1;
//            TTadMgr.CurRewardVideoPlay = ep;
//            Protomsg.CS_UseWatchVedioItem msg1 = new Protomsg.CS_UseWatchVedioItem();
//            MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_UseWatchVedioItem", msg1);
//        }



//    }


//    //    public static void ShowVideo(StartPlayRewardVideoResult sp, EndPlayRewardVideoResult ep)
//    //    {
//    //#if UNITY_IPHONE11 || UNITY_ANDROID
//    //        if (Mintegral.isVideoReadyToPlay(adUnit))
//    //        {

//    //            Mintegral.showRewardedVideo(adUnit);
//    //            if (sp != null)
//    //            {
//    //                sp(true);
//    //            }
//    //            CurRewardVideoPlay = ep;
//    //        }
//    //        else
//    //        {
//    //            Mintegral.requestRewardedVideo(adUnit);
//    //            if (sp != null)
//    //            {
//    //                sp(false);
//    //            }
//    //            Debug.Log("ShowVideo no ready:");
//    //        }
//    //#else
//    //        if (sp != null)
//    //        {
//    //            sp(true);
//    //        }
//    //        if (ep != null)
//    //        {
//    //            ep(true);
//    //        }
//    //#endif

//    //    }
//}
