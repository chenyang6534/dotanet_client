using cocosocket4unity;
using GoogleMobileAds.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoogleAdmob : MonoBehaviour
{
    private RewardedAd rewardedAd;
    // Use this for initialization
    public static GoogleAdmob Instanse = null;

    public delegate void EndPlayRewardVideoResult(bool issuc, bool novedio_usezhuanshi, bool useitem);//是否成功 是否没有视频而使用砖石替代
    public static EndPlayRewardVideoResult CurRewardVideoPlay = null;
    //private AdNative adNative;
    //private RewardVideoAd rewardAd;

    public string loaderrormsg;
    void Start()
    {
        
        if (GoogleAdmob.Instanse == null){
            MobileAds.Initialize(initStatus => { });
        }

        MsgManager.Instance.AddListener("SC_UseWatchVedioItem", new HandleMsg(this.SC_UseWatchVedioItem));
        this.CreateAndLoadRewardedAd();

        Instanse = this;
    }

    public void CreateAndLoadRewardedAd()
    {
#if UNITY_ANDROID
        string adUnitId = "ca-app-pub-3940256099942544/5224354917";
#elif UNITY_IPHONE
        string adUnitId = "ca-app-pub-3940256099942544/1712485313";//测试
        //string adUnitId = "ca-app-pub-3664269345038379/6857936057";//正式
#else
            string adUnitId = "unexpected_platform";
#endif

        this.rewardedAd = new RewardedAd(adUnitId);

        // Called when an ad request has successfully loaded.
        this.rewardedAd.OnAdLoaded += HandleRewardedAdLoaded;
        // Called when an ad request failed to load.
        this.rewardedAd.OnAdFailedToLoad += HandleRewardedAdFailedToLoad;
        // Called when an ad is shown.
        this.rewardedAd.OnAdOpening += HandleRewardedAdOpening;
        // Called when an ad request failed to show.
        this.rewardedAd.OnAdFailedToShow += HandleRewardedAdFailedToShow;
        // Called when the user should be rewarded for interacting with the ad.
        this.rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
        // Called when the ad is closed.
        this.rewardedAd.OnAdClosed += HandleRewardedAdClosed;

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the rewarded ad with the request.
        this.rewardedAd.LoadAd(request);
    }
    public void HandleRewardedAdLoaded(object sender, EventArgs args)
    {
        Debug.Log("HandleRewardedAdLoaded event received");
    }

    public void HandleRewardedAdFailedToLoad(object sender, AdErrorEventArgs args)
    {
        Debug.Log(
            "HandleRewardedAdFailedToLoad event received with message: "
                             + args.Message);
        loaderrormsg = args.Message;
    }

    public void HandleRewardedAdOpening(object sender, EventArgs args)
    {
        Debug.Log("HandleRewardedAdOpening event received");
    }

    public void HandleRewardedAdFailedToShow(object sender, AdErrorEventArgs args)
    {
        Debug.Log(
            "HandleRewardedAdFailedToShow event received with message: "
                             + args.Message);
    }
    public void HandleUserEarnedReward(object sender, Reward args)
    {
        string type = args.Type;
        double amount = args.Amount;
        Debug.Log(
            "HandleRewardedAdRewarded event received for "
                        + amount.ToString() + " " + type);
        //Protomsg.CS_LookVedioSucc msg1 = new Protomsg.CS_LookVedioSucc();
        //msg1.ID = 1;
        //MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_LookVedioSucc", msg1);
        if (GoogleAdmob.CurRewardVideoPlay != null)
        {
            GoogleAdmob.CurRewardVideoPlay(true, false, false);
        }

    }
    public void HandleRewardedAdClosed(object sender, EventArgs args)
    {
        this.CreateAndLoadRewardedAd();
    }
    public void UserChoseToWatchAd()
    {
        if (this.rewardedAd.IsLoaded())
        {
            this.rewardedAd.Show();
        }
        else
        {
            this.CreateAndLoadRewardedAd();
        }
    }

    public int WatchVedioState = 0;//0无 1请求服务器中
    public bool SC_UseWatchVedioItem(Protomsg.MsgBase d1)
    {
        Google.Protobuf.IMessage IMperson = new Protomsg.SC_UseWatchVedioItem();
        Protomsg.SC_UseWatchVedioItem p1 = (Protomsg.SC_UseWatchVedioItem)IMperson.Descriptor.Parser.ParseFrom(d1.Datas);
        Debug.Log("SC_UseWatchVedioItem:" + p1);
        if (p1.IsSucc == true)//成功
        {
            if (GoogleAdmob.CurRewardVideoPlay != null)
            {
                GoogleAdmob.CurRewardVideoPlay(true, false, true);
            }

            //Tool.NoticeWords("请输入正确的价格！", null);
        }
        else//失败
        {
            ShowVedio2(GoogleAdmob.CurRewardVideoPlay);
        }
        WatchVedioState = 0;

        return true;
    }
    public void ShowVedio2(EndPlayRewardVideoResult ep)
    {
        if (this.rewardedAd.IsLoaded() == false)
        {
            Debug.Log("请先加载广告");
            UMengManager.Instanse.Event_watch_vedio(this.loaderrormsg);
            new NoVedioNotice(this.loaderrormsg, (code) =>
            {
                if (code == 1)//是
                {
                    if (ep != null)
                    {
                        ep(true, true, false);
                    }
                }
                else//否
                {

                }
            });
            this.CreateAndLoadRewardedAd();

            //TTadMgr.Instanse.LoadRewardAd();
            return;
        }
        else
        {
            this.rewardedAd.Show();
            GoogleAdmob.CurRewardVideoPlay = ep;
        }

        //if (this.rewardAd.IsDownloaded == true)
        //{
        //    this.rewardAd.ShowRewardVideoAd();

        //    TTadMgr.CurRewardVideoPlay = ep;
        //}
        //else
        //{
        //    new NoVedioNotice("还未缓存", (code) =>
        //    {
        //        if (code == 1)//是
        //        {
        //            if (ep != null)
        //            {
        //                ep(true, true, false);
        //            }
        //        }
        //        else//否
        //        {

        //        }
        //    });
        //}
    }
    public void ShowVideo(EndPlayRewardVideoResult ep)
    {
        if (WatchVedioState == 0)
        {
            WatchVedioState = 1;
            GoogleAdmob.CurRewardVideoPlay = ep;
            Protomsg.CS_UseWatchVedioItem msg1 = new Protomsg.CS_UseWatchVedioItem();
            MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_UseWatchVedioItem", msg1);
        }



    }

    // Update is called once per frame
    void Update()
    {

    }
}
