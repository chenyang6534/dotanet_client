using cocosocket4unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MintegralMgr : MonoBehaviour {
    //正式
    public static string appid = "128757";
    public static string appkey = "b988092f5cf34f454c69e827d01879e1";
    public static string adUnit = "263234";

    public delegate void StartPlayRewardVideoResult(bool issuc);
    public delegate void EndPlayRewardVideoResult(bool issuc);
    //测试
    //public static string appid = "118690";
    //public static string appkey = "7c22942b749fe6a6e361b675e96b3ee9";
    //public static string adUnit = "146874";
    public static EndPlayRewardVideoResult CurRewardVideoPlay = null;
    void Start () {
#if UNITY_EDITOR
        // Testing UNITY_EDITOR first because the editor also responds to the currently
        // selected platform.
#elif UNITY_IPHONE11 || UNITY_ANDROID
        Mintegral.initMTGSDK(appid, appkey);
        //Mintegral.setConsentStatusInfoType(int GDPR_key);
        string[] rewardVideoUnits = new string[] { adUnit };
        Mintegral.loadRewardedVideoPluginsForAdUnits(rewardVideoUnits);//载入Reward Video类型组件
        //
        


        MintegralManager.onRewardedVideoLoadSuccessEvent += onRewardedVideoLoadSuccessEvent;
        MintegralManager.onRewardedVideoLoadedEvent += onRewardedVideoLoadedEvent;
		MintegralManager.onRewardedVideoFailedEvent += onRewardedVideoFailedEvent;
		MintegralManager.onRewardedVideoShownFailedEvent += onRewardedVideoShownFailedEvent;
		MintegralManager.onRewardedVideoShownEvent += onRewardedVideoShownEvent;
        MintegralManager.onRewardedVideoClickedEvent += onRewardedVideoClickedEvent;
		MintegralManager.onRewardedVideoClosedEvent += onRewardedVideoClosedEvent;
        MintegralManager.onRewardedVideoPlayCompletedEvent += onRewardedVideoPlayCompletedEvent;
        MintegralManager.onRewardedVideoEndCardShowSuccessEvent += onRewardedVideoEndCardShowSuccessEvent;

        Mintegral.requestRewardedVideo (adUnit);
        Debug.Log("MintegralMgr Start: ");
        
#endif

    }
    // Rewarded Video Events
#if UNITY_IPHONE11 || UNITY_ANDROID
    void onRewardedVideoLoadSuccessEvent(string adUnitId)
    {
        
        Debug.Log("onRewardedVideoLoadSuccessEvent: " + adUnitId);
    }

    void onRewardedVideoLoadedEvent(string adUnitId)
    {
        Debug.Log("onRewardedVideoLoadedEvent: " + adUnitId);
    }

    void onRewardedVideoFailedEvent(string errorMsg)
    {
        Debug.Log("onRewardedVideoFailedEvent: " + errorMsg);
    }

    void onRewardedVideoShownFailedEvent(string adUnitId)
    {
        Debug.Log("onRewardedVideoShownFailedEvent: " + adUnitId);
    }

    void onRewardedVideoShownEvent()
    {
        Debug.Log("onRewardedVideoShownEvent");
    }

    void onRewardedVideoClickedEvent(string errorMsg)
    {
        Debug.Log("onRewardedVideoClickedEvent: " + errorMsg);
    }

    void onRewardedVideoClosedEvent(MintegralManager.MTGRewardData rewardData)
    {
        if (rewardData.converted)
        {
            Debug.Log("onRewardedVideoClosedEvent: " + rewardData.ToString());
            if(CurRewardVideoPlay != null)
            {
                CurRewardVideoPlay(true);
            }
        }
        else
        {
            Debug.Log("onRewardedVideoClosedEvent: No Reward");
            if (CurRewardVideoPlay != null)
            {
                CurRewardVideoPlay(false);
            }
        }
        Mintegral.requestRewardedVideo(adUnit);
    }

    void onRewardedVideoPlayCompletedEvent(string adUnitId)
    {
        Debug.Log("onRewardedVideoPlayCompletedEvent: " + adUnitId);
    }

    void onRewardedVideoEndCardShowSuccessEvent(string adUnitId)
    {
        Debug.Log("onRewardedVideoEndCardShowSuccessEvent: " + adUnitId);
    }
#endif
    public static void ShowVideo(StartPlayRewardVideoResult sp, EndPlayRewardVideoResult ep)
    {
#if UNITY_IPHONE11 || UNITY_ANDROID
        if (Mintegral.isVideoReadyToPlay (adUnit)) {

            Mintegral.showRewardedVideo (adUnit);
            if(sp != null)
            {
                sp(true);
            }
            CurRewardVideoPlay = ep;
        }
        else{
            Mintegral.requestRewardedVideo(adUnit);
            if (sp != null)
            {
                sp(false);
            }
            Debug.Log("ShowVideo no ready:");
        }
#else
        if (sp != null)
        {
            sp(true);
        }
        if (ep != null)
        {
            ep(true);
        }
#endif

    }



    // Update is called once per frame
    void Update () {
		
	}
}
