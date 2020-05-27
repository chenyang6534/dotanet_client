using UnityEngine;
using System.Collections.Generic;


#if UNITY_ANDROID

public class MintegralAndroidAppWall
{
	private readonly AndroidJavaObject _appWallPlugin;

	public MintegralAndroidAppWall (string adUnitId)
	{
		if (Application.platform != RuntimePlatform.Android)
			return;

		_appWallPlugin = new AndroidJavaObject ("com.mintegral.msdk.unity.MTGAppWall", adUnitId);
	}

	// developer can call this when app start
	public static void preLoadAppWallAd (string adUnitId)
	{
		AndroidJavaObject appWallPlugin = new AndroidJavaObject ("com.mintegral.msdk.unity.MTGAppWall", adUnitId);
		appWallPlugin.Call ("preloadWall",adUnitId);
	}


	// this will take over the screen and show the ad
	public void showAppWallAd (string adUnitId)
	{
		if (Application.platform != RuntimePlatform.Android)
			return;

		_appWallPlugin.Call ("openWall",adUnitId);

	}
}

#endif
