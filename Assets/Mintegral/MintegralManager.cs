using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


#if UNITY_IPHONE || UNITY_ANDROID
public class MintegralManager : MonoBehaviour
{
  // InterActiveVideo

	// Fired when an InterActive ad is loaded and ready to be shown
	public static event Action<string> onInterActiveLoadedEvent;

	// Fired when an InterActive ad fails to load
	public static event Action<string> onInterActiveFailedEvent;

	// Fired when an InterActive ad is displayed
	public static event Action<string> onInterActiveShownEvent;

	// Fired if meet an errro when show an InterActive ad
	public static event Action<string> onInterActiveShownFailedEvent;

	// Fired when an InterActive ad is clicked
	public static event Action<string> onInterActiveClickedEvent;

	// Fired when an InterActive ad is dismissed
	public static event Action<string> onInterActiveDismissedEvent;

    // Fired when an InterActive ad is Material loaded ready
    public static event Action<string> onInterActiveMaterialLoadedEvent;

    // Fired when an InterActive ad is playing Complete
    public static event Action<string> onInterActivePlayingCompleteEvent;


    // InterstitialVideo

    public static event Action<string> onInterstitialVideoLoadSuccessEvent;

    // Fired when an interstitialVideo ad is loaded and ready to be shown
    public static event Action<string> onInterstitialVideoLoadedEvent;

	// Fired when an interstitialVideo ad fails to load
	public static event Action<string> onInterstitialVideoFailedEvent;

	// Fired when an interstitialVideo ad is displayed
	public static event Action<string> onInterstitialVideoShownEvent;

	// Fired if meet an errro when show an interstitialVideo ad
	public static event Action<string> onInterstitialVideoShownFailedEvent;

	// Fired when an interstitialVideo ad is clicked
	public static event Action<string> onInterstitialVideoClickedEvent;

	// Fired when an interstitialVideo ad is dismissed
	public static event Action<string> onInterstitialVideoDismissedEvent;

    // Fired when an interstitialVideo ad is playCompleted
    public static event Action<string> onInterstitialVideoPlayCompletedEvent;

    // Fired when an interstitialVideo ad is endCardShowSuccess
    public static event Action<string> onInterstitialVideoEndCardShowSuccessEvent;

	public static event Action<MTGAdPlayVideoData> onInterstitialVideoAdPlayVideoEvent;


    // Interstitial

    // Fired when an interstitial ad is loaded and ready to be shown
    public static event Action onInterstitialLoadedEvent;

	// Fired when an interstitial ad fails to load
	public static event Action<string> onInterstitialFailedEvent;

	// Fired when an interstitial ad is displayed
	public static event Action onInterstitialShownEvent;

  	// Fired if meet an errro when show an interstitial ad
  	public static event Action<string> onInterstitialShownFailedEvent;

	// Fired when an interstitial ad is clicked
	public static event Action onInterstitialClickedEvent;

	// Fired when an interstitial ad is dismissed
	public static event Action onInterstitialDismissedEvent;


    //Reward Video

    public static event Action<string> onRewardedVideoLoadSuccessEvent;

    // Fired when a rewarded video finishes loading and is ready to be displayed
    public static event Action<string> onRewardedVideoLoadedEvent;

	// Fired when a rewarded video fails to load. Includes the error message.
	public static event Action<string> onRewardedVideoFailedEvent;

	// Fired when an rewarded video is displayed
	public static event Action onRewardedVideoShownEvent;

  	// Fired if meet an error when show an  rewarded video ad
  	public static event Action<string> onRewardedVideoShownFailedEvent;

  	// Fired when an rewarded video is clicked
 	public static event Action<string> onRewardedVideoClickedEvent;

	// Fired when a rewarded video closes,and give the reward info.
	public static event Action<MTGRewardData> onRewardedVideoClosedEvent;

    // Fired when an rewarded video is playCompleted
    public static event Action<string> onRewardedVideoPlayCompletedEvent;

    // Fired when a rewarded video is endCardShowSuccess
    public static event Action<string> onRewardedVideoEndCardShowSuccessEvent;



    // OfferWall
    public static event Action onOfferWallLoadedEvent;

	public static event Action<string> onOfferWallFailedEvent;

	public static event Action onOfferWallDidClickEvent;

	public static event Action onOfferWallShownEvent;

	public static event Action<string> onOfferWallShownFailedEvent;

	public static event Action onOfferWallClosedEvent;

	public static event Action<MTGRewardData[]> onOfferWallEarnedImmediatelyEvent;

	public static event Action <MTGRewardData[]> onOfferWallNotifyCreditsEarnedAfterQueryEvent;

	// Native
	public static event Action<string> onNativeLoadedEvent;

	public static event Action<string> onNativeFailedEvent;

	public static event Action<string> onNativeDidClickEvent;

	public static event Action<string> onNativeLoggingImpressionEvent;

	public static event Action<string> onNativeRedirectionStartEvent;

	public static event Action<string> onNativeRedirectionFinishedEvent;

	//Banner
	public static event Action<string> onBannerLoadedEvent;
	public static event Action<string> onBannerFailedEvent;
	public static event Action<string> onBannerDidClickEvent;
	public static event Action<string> onBannerLoggingImpressionEvent;
	public static event Action<string> onBannerLeaveAppEvent;
	public static event Action<string> onBannerShowFullScreenEvent;
	public static event Action<string> onBannerCloseFullScreenEvent;
	public static event Action<string> onBannerDismissEvent;

	// GDPR
	public static event Action<string> onShowUserInfoTipsEvent;

	public class MTGAdPlayVideoData
	{
		
		public int rewardAlertStatus;
		public bool isComplete;

		public MTGAdPlayVideoData (string json)
		{
			var obj = MintegralInternal.ThirdParty.MiniJSON.Json.Deserialize (json) as Dictionary<string,object>;
			if (obj == null)
				return;

			if (obj.ContainsKey ("isComplete")){

				if(obj ["isComplete"].ToString () == "1"){
					isComplete = true;
				}else{
					isComplete = false;
				}
			}

			if (obj.ContainsKey ("rewardAlertStatus"))
				rewardAlertStatus = int.Parse (obj ["rewardAlertStatus"].ToString ());
		}

		public override string ToString ()
		{
			
			return string.Format ("isComplete: {0}, rewardAlertStatus: {1}", isComplete, rewardAlertStatus);
			
		}
	}

	public class MTGRewardData
	{
		public string rewardName;
		public float rewardAmount;
		public bool converted;

		public MTGRewardData (bool isRewardVideo,string json)
		{
			var obj = MintegralInternal.ThirdParty.MiniJSON.Json.Deserialize (json) as Dictionary<string,object>;
			if (obj == null)
				return;

			if (obj.ContainsKey ("converted")){

				if(obj ["converted"].ToString () == "1"){
					converted = true;
				}else{
					converted = false;
				}
			}

			if (obj.ContainsKey ("rewardName"))
				rewardName = obj ["rewardName"].ToString ();

			if (obj.ContainsKey ("rewardAmount"))
				rewardAmount = float.Parse (obj ["rewardAmount"].ToString ());
		}

		public MTGRewardData (Dictionary<string,object> rewardDict){

			converted = true;

			if (rewardDict.ContainsKey ("rewardName")) {
				rewardName = rewardDict ["rewardName"].ToString ();
			}

			if (rewardDict.ContainsKey ("rewardAmount")) {
				rewardAmount = float.Parse (rewardDict ["rewardAmount"].ToString ());
			}
		}

		public override string ToString ()
		{
			if(converted){
				return string.Format ("rewardName: {0}, rewardAmount: {1}", rewardName, rewardAmount);
			}
			return "convert false,reward  is null";
		}
	}


	static MintegralManager ()
	{
		var type = typeof(MintegralManager);
		try {
		// first we see if we already exist in the scene
			var obj = FindObjectOfType (type) as MonoBehaviour;
			if (obj != null)
				return;

		// create a new GO for our manager
			var managerGO = new GameObject (type.ToString ());
			managerGO.AddComponent (type);
			DontDestroyOnLoad (managerGO);
		} catch (UnityException) {
			Debug.LogWarning ("It looks like you have the " + type +
				" on a GameObject in your scene. Please remove the script from your scene.");
		}
	}

	// InterActive Listeners
	void onInterActiveLoaded (string msg)
	{
		if (onInterActiveLoadedEvent != null)
			onInterActiveLoadedEvent (msg);
	}


	void onInterActiveFailed (string errorMsg)
	{
		if (onInterActiveFailedEvent != null)
			onInterActiveFailedEvent (errorMsg);
	}


	void onInterActiveShown (string msg)
	{
		if (onInterActiveShownEvent != null)
			onInterActiveShownEvent (msg);
	}

	void onInterActiveShownFailed (string errorMsg)
	{
		if (onInterActiveShownFailedEvent != null)
			onInterActiveShownFailedEvent (errorMsg);
	}


	void onInterActiveClicked (string msg)
	{
		if (onInterActiveClickedEvent != null)
			onInterActiveClickedEvent (msg);
	}


	void onInterActiveDismissed (string msg)
	{
		if (onInterActiveDismissedEvent != null)
			onInterActiveDismissedEvent (msg);
	}

    void onInterActiveMaterialLoaded(string msg)
    {
        if (onInterActiveMaterialLoadedEvent != null)
            onInterActiveMaterialLoadedEvent(msg);
    }


    void onInterActivePlayingComplete(string msg)
    {
        if (onInterActivePlayingCompleteEvent != null)
            onInterActivePlayingCompleteEvent(msg);
    }



    // InterstitialVideo Listeners
    void onInterstitialVideoLoadSuccess(string msg)
    {
        if (onInterstitialVideoLoadSuccessEvent != null)
            onInterstitialVideoLoadSuccessEvent(msg);
    }

    void onInterstitialVideoLoaded (string msg)
	{
		if (onInterstitialVideoLoadedEvent != null)
			onInterstitialVideoLoadedEvent (msg);
	}


	void onInterstitialVideoFailed (string errorMsg)
	{
		if (onInterstitialVideoFailedEvent != null)
			onInterstitialVideoFailedEvent (errorMsg);
	}


	void onInterstitialVideoShown (string msg)
	{
		if (onInterstitialVideoShownEvent != null)
			onInterstitialVideoShownEvent (msg);
	}

	void onInterstitialVideoShownFailed (string errorMsg)
	{
		if (onInterstitialVideoShownFailedEvent != null)
			onInterstitialVideoShownFailedEvent (errorMsg);
	}


	void onInterstitialVideoClicked (string msg)
	{
		if (onInterstitialVideoClickedEvent != null)
			onInterstitialVideoClickedEvent (msg);
	}


	void onInterstitialVideoDismissed (string msg)
	{
		if (onInterstitialVideoDismissedEvent != null)
			onInterstitialVideoDismissedEvent (msg);
	}

    void onInterstitialVideoPlayCompleted(string msg)
    {
        if (onInterstitialVideoPlayCompletedEvent != null)
            onInterstitialVideoPlayCompletedEvent(msg);
    }


    void onInterstitialVideoEndCardShowSuccess(string msg)
    {
        if (onInterstitialVideoEndCardShowSuccessEvent != null)
            onInterstitialVideoEndCardShowSuccessEvent(msg);
    }

	void onInterstitialVideoAdPlayVideo(string msg)
	{
		if(onInterstitialVideoAdPlayVideoEvent != null){
			onInterstitialVideoAdPlayVideoEvent((new MTGAdPlayVideoData (msg)));
		}
	}


    // Interstitial Listeners
    void onInterstitialLoaded (string msg)
	{
		if (onInterstitialLoadedEvent != null)
			onInterstitialLoadedEvent ();
	}


	void onInterstitialFailed (string errorMsg)
	{
		if (onInterstitialFailedEvent != null)
			onInterstitialFailedEvent (errorMsg);
	}
		

	void onInterstitialShown (string msg)
	{
		if (onInterstitialShownEvent != null)
			onInterstitialShownEvent ();
	}

	void onInterstitialShownFailed (string errorMsg)
	{
		if (onInterstitialShownFailedEvent != null)
			onInterstitialShownFailedEvent (errorMsg);
	}


	void onInterstitialClicked (string msg)
	{
		if (onInterstitialClickedEvent != null)
			onInterstitialClickedEvent ();
	}


	void onInterstitialDismissed (string msg)
	{
		if (onInterstitialDismissedEvent != null)
			onInterstitialDismissedEvent ();
	}



    // Rewarded Video Listeners

    void onRewardedVideoLoadSuccess(string msg)
    {
        if (onRewardedVideoLoadSuccessEvent != null)
            onRewardedVideoLoadSuccessEvent(msg);
    }

    void onRewardedVideoLoaded (string msg)
	{
		if (onRewardedVideoLoadedEvent != null)
			onRewardedVideoLoadedEvent (msg);
	}


	void onRewardedVideoFailed (string errorMsg)
	{
		if (onRewardedVideoFailedEvent != null)
			onRewardedVideoFailedEvent (errorMsg);
	}


	void onRewardedVideoShown (string msg)
	{
		if (onRewardedVideoShownEvent != null)
			onRewardedVideoShownEvent ();
	}
		

	void onRewardedVideoShownFailed (string errorMsg)
	{
		if (onRewardedVideoShownFailedEvent != null)
			onRewardedVideoShownFailedEvent (errorMsg);
	}



	void onRewardedVideoClosed (string msg)
	{
		if (onRewardedVideoClosedEvent != null)
			onRewardedVideoClosedEvent (new MTGRewardData (true,msg));
	}


	void onRewardedVideoClicked (string msg)
	{
		if (onRewardedVideoClickedEvent != null)
			onRewardedVideoClickedEvent (msg);
	}

    void onRewardedVideoPlayCompleted(string msg)
    {
        if (onRewardedVideoPlayCompletedEvent != null)
            onRewardedVideoPlayCompletedEvent(msg);
    }


    void onRewardedVideoEndCardShowSuccess(string msg)
    {
        if (onRewardedVideoEndCardShowSuccessEvent != null)
            onRewardedVideoEndCardShowSuccessEvent(msg);
    }



    // OfferWall  Listeners

    void onOfferWallLoaded (string msg){
		if (onOfferWallLoadedEvent != null)
			onOfferWallLoadedEvent ();
	}


	void onOfferWallFailed (string errorMsg){
		if (onOfferWallFailedEvent != null)
			onOfferWallFailedEvent (errorMsg);
	}


	void onOfferWallDidClick (string msg){
		if (onOfferWallDidClickEvent != null)
			onOfferWallDidClickEvent ();
	}


	void onOfferWallShown (string msg){
		if (onOfferWallShownEvent != null)
			onOfferWallShownEvent ();
	}


	void onOfferWallShownFailed (string errorMsg){
		if (onOfferWallShownFailedEvent != null)
			onOfferWallShownFailedEvent (errorMsg);
	}


	void onOfferWallClosed (string msg){
		if (onOfferWallClosedEvent != null)
			onOfferWallClosedEvent ();
	}


	void onOfferWallEarnedImmediately (string json){
		if (onOfferWallEarnedImmediatelyEvent != null) {

			if (json != null) {

				List<object> rewardList = MintegralInternal.ThirdParty.MiniJSON.Json.Deserialize (json) as List<object>;

				MTGRewardData[] rewardDatas = new MTGRewardData[rewardList.Count];

				for (int i = 0; i < rewardList.Count; i++){

					var obj = rewardList [i] as Dictionary<string,object>;
					if (obj == null) {
						continue;
					}
					MTGRewardData reward = new MTGRewardData (obj);
					rewardDatas [i] = reward;
				}

				onOfferWallEarnedImmediatelyEvent (rewardDatas);
			} else {
				onOfferWallEarnedImmediatelyEvent (null);
			}

		}
	}

	void onOfferWallNotifyCreditsEarnedAfterQuery (string json){
		if (onOfferWallNotifyCreditsEarnedAfterQueryEvent != null) {

			if (json != null) {

					List<object> rewardList = MintegralInternal.ThirdParty.MiniJSON.Json.Deserialize (json) as List<object>;

					MTGRewardData[] rewardDatas = new MTGRewardData[rewardList.Count];

					for (int i = 0; i < rewardList.Count; i++){

						var obj = rewardList [i] as Dictionary<string,object>;
						if (obj == null) {
							continue;
						}
						MTGRewardData reward = new MTGRewardData (obj);
						rewardDatas [i] = reward;
					}
				onOfferWallNotifyCreditsEarnedAfterQueryEvent (rewardDatas);
			} else {
				onOfferWallNotifyCreditsEarnedAfterQueryEvent (null);
			}

		}
	}


	// Native Listener

	void onNativeLoaded (string msg){
		if (onNativeLoadedEvent != null)
			onNativeLoadedEvent (msg);
	}

	void onNativeFailed (string errorMsg){
		if (onNativeFailedEvent != null)
			onNativeFailedEvent (errorMsg);
	}

	void onNativeDidClick (string msg){
		if (onNativeDidClickEvent != null)
			onNativeDidClickEvent (msg);
	}

	void onNativeLoggingImpression (string msg){
		if (onNativeLoggingImpressionEvent != null)
			onNativeLoggingImpressionEvent (msg);
	}

	void onNativeRedirectionStart (string msg){
		if (onNativeRedirectionStartEvent != null)
			onNativeRedirectionStartEvent (msg);
	}

	void onNativeRedirectionFinished (string msg){
		if (onNativeRedirectionFinishedEvent != null)
			onNativeRedirectionFinishedEvent (msg);
	}

	//Banner Listener
	void onBannerLoadSuccessed (string msg)
	{
		if (onBannerLoadedEvent != null)
			onBannerLoadedEvent (msg);
	}


	void onBannerLoadFailed (string errorMsg)
	{
		if (onBannerFailedEvent != null)
			onBannerFailedEvent (errorMsg);
	}


	void onBannerImpression (string msg)
	{
		if (onBannerLoggingImpressionEvent != null)
			onBannerLoggingImpressionEvent (msg);
	}

	void onBannerClick (string msg)
	{
		if (onBannerDidClickEvent != null)
			onBannerDidClickEvent (msg);
	}

	void onBannerLeaveApp (string msg)
	{
		if (onBannerLeaveAppEvent != null)
			onBannerLeaveAppEvent (msg);
	}

	void onBannerShowFullScreen (string msg)
	{
		if (onBannerShowFullScreenEvent != null)
			onBannerShowFullScreenEvent (msg);
	}
	
	void onBannerCloseFullScreen (string msg)
	{
		if (onBannerCloseFullScreenEvent != null)
			onBannerCloseFullScreenEvent (msg);
	}

	void onCloseBanner (string msg)
	{
		if (onBannerDismissEvent != null)
			onBannerDismissEvent (msg);
	}

	// GDPR Listener

	void onShowUserInfoTips (string msg){
		if (onShowUserInfoTipsEvent != null)
			onShowUserInfoTipsEvent (msg);
	}

}
#endif
