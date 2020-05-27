using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using AOT;

#if UNITY_IPHONE
public class MintegraliOSNative
{
	public IntPtr nativeManager;

	[DllImport ("__Internal")]
	private static extern IntPtr initNative (string unitId, string fbPlacementId, 
	bool autoCacheImage,  string adCategory,string supportedTemplates);

	[DllImport ("__Internal")]
	private static extern void preloadNative (string unitId, string fbPlacementId, 
	bool autoCacheImage,  string adCategory,string supportedTemplates);

	[DllImport ("__Internal")]
	private static extern void loadNative (IntPtr instance);

	public MintegraliOSNative (MTGNativeInfo nativeAdInfo)
	{
		if (Application.platform != RuntimePlatform.OSXEditor) {
			if (Application.platform == RuntimePlatform.IPhonePlayer){

				MTGTemplateWrapper wrapper = new MTGTemplateWrapper();
				wrapper.objects = nativeAdInfo.supportedTemplate;
				string supportedTemplatesStr = JsonUtility.ToJson(wrapper);

				nativeManager = initNative (nativeAdInfo.adUnitId,nativeAdInfo.fb_placement_id,nativeAdInfo.autoCacheImage,nativeAdInfo.adCategory.ToString(),supportedTemplatesStr);
			}
		}
	}

	// Starts preloading an native ad
	public void preRequestNativeAd (string adUnitId, string fb_placement_id, string categoryType, MTGTemplate[] supportedTemplates)
	{
		if (Application.platform != RuntimePlatform.OSXEditor) {

			if (Application.platform == RuntimePlatform.IPhonePlayer){
				if (nativeManager == IntPtr.Zero)  return;

				MTGTemplateWrapper wrapper = new MTGTemplateWrapper();
				wrapper.objects = supportedTemplates;
				string supportedTemplatesStr = JsonUtility.ToJson(wrapper);

				preloadNative (adUnitId,fb_placement_id,false,categoryType,supportedTemplatesStr);
			}
		}
	}

	// Starts loading an native ad
	public void requestNativeAd ()
	{
		if (Application.platform != RuntimePlatform.OSXEditor) {

			if (Application.platform == RuntimePlatform.IPhonePlayer){
				if (nativeManager == IntPtr.Zero)  return;

				loadNative (nativeManager);
			}
		}
	}

}
#endif

