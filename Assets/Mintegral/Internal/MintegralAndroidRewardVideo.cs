using UnityEngine;
using System.Collections.Generic;


#if UNITY_ANDROID

public class MintegralAndroidRewardedVideo
{
	private readonly AndroidJavaObject _plugin;

	public MintegralAndroidRewardedVideo (string adUnitId)
	{
		if (Application.platform != RuntimePlatform.Android)
			return;

		_plugin = new AndroidJavaObject ("com.mintegral.msdk.unity.MTGReward", adUnitId);
	}


	// Starts loading a rewarded video ad
	public void requestRewardedVideoAd (string adUnitId)
	{
		if (Application.platform != RuntimePlatform.Android)
			return;

		_plugin.Call ("loadRewardView");
	}


	// If a rewarded video ad is loaded this will take over the screen and show the ad
	public void showRewardedVideoAd (string adUnitId,string rewardId,string userId)
	{
		if (Application.platform != RuntimePlatform.Android)
			return;

		_plugin.Call ("showRewardView",rewardId,userId);
	}

	public void cleanVideoFileCache()
	{
		if (Application.platform != RuntimePlatform.Android)
			return;

		_plugin.Call ("cleanRewardView");
	}

	public bool isVideoReady(string adUnitId)
	{
		if (Application.platform != RuntimePlatform.Android)
			return false;

		return _plugin.Call<bool> ("isReady");
	}

	public void setAlertWithTitle(string adUnitId,string title,string content,string comfirmText,string cancelText){
		if (Application.platform != RuntimePlatform.Android)
			return ;

		_plugin.Call ("setAlertWithTitle",title,content,comfirmText,cancelText);
	}
}

#endif
