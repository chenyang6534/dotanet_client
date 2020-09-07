
using UnityEngine;
using System.Collections.Generic;


#if UNITY_ANDROID

public class MintegralAndroidInterstitialVideo
{
	private readonly AndroidJavaObject _interstitialVideoPlugin;

	public MintegralAndroidInterstitialVideo (MTGInterstitialVideoInfo info)
	{
		if (Application.platform != RuntimePlatform.Android)
		return;

		_interstitialVideoPlugin = new AndroidJavaObject ("com.mintegral.msdk.unity.MTGInterstitialVideo", info.adUnitId);
	}


	// Starts loading an interstitial ad
	public void requestInterstitialVideoAd ()
	{
		if (Application.platform != RuntimePlatform.Android)
		return;

		_interstitialVideoPlugin.Call ("preloadInterstitialVideo");
	}


	// If an interstitial ad is loaded this will take over the screen and show the ad
	public void showInterstitialVideoAd ()
	{
		if (Application.platform != RuntimePlatform.Android)
		return;

		_interstitialVideoPlugin.Call ("showInterstitialVideo");

	}

	public bool isVideoReady(string adUnitId){
		if (Application.platform != RuntimePlatform.Android)
			return false;

		return _interstitialVideoPlugin.Call<bool> ("isReady");
	}

	public void setIVRewardMode(Mintegral.MTGIVRewardMode ivRewardMode,int playTime){
		if (Application.platform != RuntimePlatform.Android)
			return ;

		_interstitialVideoPlugin.Call ("setIVRewardTime",(int)ivRewardMode,playTime);
	}

	public void setIVRewardMode(Mintegral.MTGIVRewardMode ivRewardMode,float playRate){
		if (Application.platform != RuntimePlatform.Android)
			return ;

		_interstitialVideoPlugin.Call ("setIVRewardRate",(int)ivRewardMode,playRate);
	}

	public void setAlertDialogText(string title,string content,string comfirmText,string cancelText){
		if (Application.platform != RuntimePlatform.Android)
			return ;

		_interstitialVideoPlugin.Call("setAlertDialogText",title,content,comfirmText,cancelText);
	}
}
#endif
