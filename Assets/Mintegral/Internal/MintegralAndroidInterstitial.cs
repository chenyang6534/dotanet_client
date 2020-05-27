using UnityEngine;
using System.Collections.Generic;


#if UNITY_ANDROID

public class MintegralAndroidInterstitial
{
	private readonly AndroidJavaObject _interstitialPlugin;

	public MintegralAndroidInterstitial (MTGInterstitialInfo info)
	{
		if (Application.platform != RuntimePlatform.Android)
			return;

		string adCategoryStr = ((MTGAdCategory)info.adCategory).ToString ("d");

		_interstitialPlugin = new AndroidJavaObject ("com.mintegral.msdk.unity.MTGInterstitial", info.adUnitId,adCategoryStr);

	}


	// Starts loading an interstitial ad
	public void requestInterstitialAd ()
	{
		if (Application.platform != RuntimePlatform.Android)
			return;

		_interstitialPlugin.Call ("preloadInterstitial");
	}


	// If an interstitial ad is loaded this will take over the screen and show the ad
	public void showInterstitialAd ()
	{
		if (Application.platform != RuntimePlatform.Android)
			return;

		_interstitialPlugin.Call ("showInterstitial");
	}
}

#endif
