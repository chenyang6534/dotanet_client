using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ByteDance.Union;
public class TTadMgr : MonoBehaviour {

    public static string rewardadid = "945508163";//正式
    //public static string rewardadid = "901121430";//测试
    public delegate void StartPlayRewardVideoResult(bool issuc);
    public delegate void EndPlayRewardVideoResult(bool issuc);
    public static EndPlayRewardVideoResult CurRewardVideoPlay = null;
    private AdNative adNative;
    private RewardVideoAd rewardAd;

    static public TTadMgr Instanse;
    void Awake()
    {
        Instanse = this;
    }
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    private AdNative AdNative
    {
        get
        {
            if (this.adNative == null)
            {
                this.adNative = SDK.CreateAdNative();
            }
#if UNITY_ANDROID
            SDK.RequestPermissionIfNecessary();
#endif
            return this.adNative;
        }
    }

    public void LoadRewardAd()
    {
        if (this.rewardAd != null)
        {
            Debug.LogError("广告已经加载");
            return;
        }

        string iosSlotID = "900546826";
        string AndroidSlotID = rewardadid;

        var adSlot = new AdSlot.Builder()
#if UNITY_IOS
            .SetCodeId(iosSlotID)
#else
            .SetCodeId(AndroidSlotID)
#endif
            .SetSupportDeepLink(true)
            .SetImageAcceptedSize(1080, 1920)
            .SetRewardName("金币") // 奖励的名称
            .SetRewardAmount(3) // 奖励的数量
            .SetUserID("user123") // 用户id,必传参数
            .SetMediaExtra("media_extra") // 附加参数，可选
            .SetOrientation(AdOrientation.Horizontal) // 必填参数，期望视频的播放方向
            .Build();

        this.AdNative.LoadRewardVideoAd(
            adSlot, new RewardVideoAdListener(this));
    }
    private sealed class RewardVideoAdListener : IRewardVideoAdListener
    {
        private TTadMgr example;

        public RewardVideoAdListener(TTadMgr example)
        {
            this.example = example;
        }

        public void OnError(int code, string message)
        {
            Debug.LogError("OnRewardError: " + message);
        }

        public void OnRewardVideoAdLoad(RewardVideoAd ad)
        {
            Debug.Log("OnRewardVideoAdLoad");

            ad.SetRewardAdInteractionListener(
                new RewardAdInteractionListener(this.example));
            ad.SetDownloadListener(
                new AppDownloadListener(this.example));

            this.example.rewardAd = ad;
        }

        public void OnExpressRewardVideoAdLoad(ExpressRewardVideoAd ad)
        {
        }

        public void OnRewardVideoCached()
        {
            Debug.Log("OnRewardVideoCached");

            if (this.example.rewardAd != null)
            {
                this.example.rewardAd.IsDownloaded = true;
            }
        }
    }

    private sealed class RewardAdInteractionListener : IRewardAdInteractionListener
    {
        private TTadMgr example;

        public RewardAdInteractionListener(TTadMgr example)
        {
            this.example = example;
        }

        public void OnAdShow()
        {
            Debug.Log("rewardVideoAd show");
        }

        public void OnAdVideoBarClick()
        {
            Debug.Log("rewardVideoAd bar click");
        }

        public void OnAdClose()
        {
            Debug.Log("rewardVideoAd close");
            this.example.rewardAd = null;

        }

        public void OnVideoComplete()
        {
            Debug.Log("rewardVideoAd complete");
        }

        public void OnVideoError()
        {
            Debug.LogError("rewardVideoAd error");
        }

        public void OnRewardVerify(
            bool rewardVerify, int rewardAmount, string rewardName)
        {
            Debug.Log("verify:" + rewardVerify + " amount:" + rewardAmount +
                " name:" + rewardName);
        }
    }
    private sealed class AppDownloadListener : IAppDownloadListener
    {
        private TTadMgr example;

        public AppDownloadListener(TTadMgr example)
        {
            this.example = example;
        }

        public void OnIdle()
        {
        }

        public void OnDownloadActive(
            long totalBytes, long currBytes, string fileName, string appName)
        {
            Debug.Log("下载中，点击下载区域暂停");
        }

        public void OnDownloadPaused(
            long totalBytes, long currBytes, string fileName, string appName)
        {
            Debug.Log("下载暂停，点击下载区域继续");
        }

        public void OnDownloadFailed(
            long totalBytes, long currBytes, string fileName, string appName)
        {
            Debug.LogError("下载失败，点击下载区域重新下载");
        }

        public void OnDownloadFinished(
            long totalBytes, string fileName, string appName)
        {
            Debug.Log("下载完成，点击下载区域重新下载");
        }

        public void OnInstalled(string fileName, string appName)
        {
            Debug.Log("安装完成，点击下载区域打开");
        }
    }

    /// <summary>
    /// Show the reward Ad.
    /// </summary>
    public void ShowRewardAd()
    {
        if (this.rewardAd == null)
        {
            Debug.LogError("请先加载广告");
            return;
        }

        if (this.rewardAd.IsDownloaded == true)
        {
            this.rewardAd.ShowRewardVideoAd();
        }

    }


//    public static void ShowVideo(StartPlayRewardVideoResult sp, EndPlayRewardVideoResult ep)
//    {
//#if UNITY_IPHONE11 || UNITY_ANDROID
//        if (Mintegral.isVideoReadyToPlay(adUnit))
//        {

//            Mintegral.showRewardedVideo(adUnit);
//            if (sp != null)
//            {
//                sp(true);
//            }
//            CurRewardVideoPlay = ep;
//        }
//        else
//        {
//            Mintegral.requestRewardedVideo(adUnit);
//            if (sp != null)
//            {
//                sp(false);
//            }
//            Debug.Log("ShowVideo no ready:");
//        }
//#else
//        if (sp != null)
//        {
//            sp(true);
//        }
//        if (ep != null)
//        {
//            ep(true);
//        }
//#endif

//    }
}
