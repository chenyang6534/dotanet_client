using UnityEngine;
using System.Collections.Generic;


#if UNITY_ANDROID

public class MintegralAndroidInterActive
{
	private readonly AndroidJavaObject _interActivePlugin;

	public MintegralAndroidInterActive (MTGInterActiveInfo info)
	{
		if (Application.platform != RuntimePlatform.Android)
			return;
 

		_interActivePlugin = new AndroidJavaObject ("com.mintegral.msdk.unity.MTGInterActive", info.adUnitId);

	}


	// Starts loading an InterActive ad
	public void requestInterActiveAd ()
	{
		if (Application.platform != RuntimePlatform.Android)
			return;

		_interActivePlugin.Call ("preloadInterActive");
	}


	// If an InterActive ad is loaded this will take over the screen and show the ad
	public void showInterActiveAd ()
	{
		if (Application.platform != RuntimePlatform.Android)
			return;

		_interActivePlugin.Call ("showInterActive");
	}

	//
	public int getInterActiveStatusAd ()
	{
		if (Application.platform != RuntimePlatform.Android) {
			return 0;
		} else { 
			return _interActivePlugin.Call <int> ("getInterActiveStatus"); 
		}
	}
}

#endif
