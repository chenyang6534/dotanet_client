
using UnityEngine;
using System.Collections.Generic;


#if UNITY_ANDROID

public class MintegralAndroidBanner
{
	private readonly AndroidJavaObject _bannerPlugin;

	public MintegralAndroidBanner (string adUnitId)
	{
		if (Application.platform != RuntimePlatform.Android)
		return;

		_bannerPlugin = new AndroidJavaObject ("com.mintegral.msdk.unity.MTGBanner", adUnitId);
	}


	// Starts loading an banner ad
	public void createBanner (string adUnitId,Mintegral.BannerAdPosition position,int adWidth, int adHeight, bool isShowCloseBtn)
	{
		if (Application.platform != RuntimePlatform.Android)
		return;

		_bannerPlugin.Call ("createBanner",(int)position,adWidth,adHeight,isShowCloseBtn);
	}


	
	public void destroyBanner (string adUnitId)
	{
		if (Application.platform != RuntimePlatform.Android)
		return;

		_bannerPlugin.Call ("destroyBanner");

	}
}
#endif
