using UnityEngine;
using System.Collections.Generic;

#if UNITY_ANDROID

public class MintegralAndroidNative
{
	private readonly AndroidJavaObject _nativePlugin;

	public MintegralAndroidNative (MTGNativeInfo nativeAdInfo)
	{
		if (Application.platform != RuntimePlatform.Android)
			return;

		MTGTemplateWrapper wrapper = new MTGTemplateWrapper();
		wrapper.objects = nativeAdInfo.supportedTemplate;
		string supportedTemplatesStr = JsonUtility.ToJson(wrapper);

		_nativePlugin = new AndroidJavaObject ("com.mintegral.msdk.unity.MTGNative",nativeAdInfo.adUnitId,nativeAdInfo.fb_placement_id,nativeAdInfo.adCategory.ToString(),supportedTemplatesStr);

	}


	// Starts preloading an native ad
	public void preRequestNativeAd (string adUnitId, string fb_placement_id, string categoryType, MTGTemplate[] supportedTemplates)
	{
		if (Application.platform != RuntimePlatform.Android)
			return;

		MTGTemplateWrapper wrapper = new MTGTemplateWrapper();
		wrapper.objects = supportedTemplates;
		string supportedTemplatesStr = JsonUtility.ToJson(wrapper);

		_nativePlugin.Call ("preloadNative",adUnitId,fb_placement_id,categoryType,supportedTemplatesStr);
	}

	// Starts loading an native ad
	public void requestNativeAd ()
	{
		if (Application.platform != RuntimePlatform.Android)
			return;

		_nativePlugin.Call ("loadNative");
	}


	// Start registration
	public void registerViewNativeAd (int left, int top, int width, int height, string index_campain)
	{
		if (Application.platform != RuntimePlatform.Android)
			return;

		_nativePlugin.Call ("registerView",left,top,width,height,index_campain);
	}

	// Revocation of registration
	public void unRegisterViewNativeAd (string index_campain)
	{
		if (Application.platform != RuntimePlatform.Android)
			return;

		_nativePlugin.Call ("unRegisterView",index_campain);
	}
}

#endif
