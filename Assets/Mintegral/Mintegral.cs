using UnityEngine;
using System;
using System.Collections.Generic;


#if UNITY_IPHONE || UNITY_ANDROID

#if UNITY_IPHONE

using MTGInterActive		= MintegraliOSInterActive;
using MTGInterstitialVideo	= MintegraliOSInterstitialVideo;
using MTGInterstitial		= MintegraliOSInterstitial;
using MTGRewardedVideo 		= MintegraliOSRewardVideo;
using MTGAppWall 			= MintegraliOSAppWall;
using MTGNative 	 		= MintegraliOSNative;
using MTGBanner 	 		= MintegraliOSBanner;

#elif UNITY_ANDROID

using MTGInterActive		= MintegralAndroidInterActive;
using MTGInterstitialVideo 	= MintegralAndroidInterstitialVideo;
using MTGInterstitial 		= MintegralAndroidInterstitial;
using MTGRewardedVideo 		= MintegralAndroidRewardedVideo;
using MTGAppWall 			= MintegralAndroidAppWall;
using MTGNative	 	 		= MintegralAndroidNative;
using MTGBanner	 		= MintegralAndroidBanner;

#endif

[System.Serializable]
public class MTGCampaign
{
	public string adId;
	public string packageName;
	public string appName;
	public string appDesc;
	public string appSize;
	public string iconUrl;
	public string imageUrl;
	public string adCall;
	public string type;
	public string timestamp;
	public string dataTemplate;
	public string index;// developer should ignore this key.
}


[System.Serializable]
public class MTGCampaignArrayWrapper { 
	public MTGCampaign[] objects;
}




public class Mintegral
{

	public enum BannerAdPosition
	{
        TopLeft,
        TopCenter,
        TopRight,
        Centered,
        BottomLeft,
        BottomCenter,
        BottomRight
	}

	public enum MTGIVRewardMode
	{
		MTGIVRewardPlayMode,
		MTGIVRewardCloseMode
	}

	public const string ADUNIT_NOT_FOUND_MSG = "AdUnit {0} not found: no plugin was initialized";

	public static IntPtr nativeMobManager;

	public static MTGNativeInfo nativeMobInfo;

	private static Dictionary<string, MTGAppWall> _appWallPluginsDict =
		new Dictionary<string, MTGAppWall> ();
	private static Dictionary<string, MTGInterActive> _interActivePluginsDict =
		new Dictionary<string, MTGInterActive> ();
	private static Dictionary<string, MTGInterstitialVideo> _interstitialVideoPluginsDict =
		new Dictionary<string, MTGInterstitialVideo> ();
	private static Dictionary<string, MTGInterstitial> _interstitialPluginsDict =
		new Dictionary<string, MTGInterstitial> ();
	private static Dictionary<string, MTGRewardedVideo> _rewardedVideoPluginsDict =
		new Dictionary<string, MTGRewardedVideo> ();
	
	private static Dictionary<string, MTGNative> _nativePluginsDict =
		new Dictionary<string, MTGNative> ();
	private static Dictionary<string, MTGBanner> _bannerPluginsDict =
		new Dictionary<string, MTGBanner> ();


	public static void loadInterActivePluginsForAdUnits (MTGInterActiveInfo[] interActiveAdInfos)
	{
		foreach (MTGInterActiveInfo interActiveVideoAdInfo in interActiveAdInfos) {
			if (!_interActivePluginsDict.ContainsKey (interActiveVideoAdInfo.adUnitId)) {
				_interActivePluginsDict.Add (interActiveVideoAdInfo.adUnitId, new MTGInterActive (interActiveVideoAdInfo));
			}
		}
	}


	public static void loadInterstitialVideoPluginsForAdUnits (MTGInterstitialVideoInfo[] interstitialVideoAdInfos)
	{
		foreach (MTGInterstitialVideoInfo interstitialVideoAdInfo in interstitialVideoAdInfos) {
			if (!_interstitialVideoPluginsDict.ContainsKey (interstitialVideoAdInfo.adUnitId)) {
				_interstitialVideoPluginsDict.Add (interstitialVideoAdInfo.adUnitId, new MTGInterstitialVideo (interstitialVideoAdInfo));
			}
		}
	}


	public static void loadInterstitialPluginsForAdUnits (MTGInterstitialInfo[] interstitialAdInfos)
	{
		foreach (MTGInterstitialInfo interstitialAdInfo in interstitialAdInfos) {
			if (!_interstitialPluginsDict.ContainsKey (interstitialAdInfo.adUnitId)) {
				_interstitialPluginsDict.Add (interstitialAdInfo.adUnitId, new MTGInterstitial (interstitialAdInfo));
			}
		}
	}

	public static void loadRewardedVideoPluginsForAdUnits (string[] rewardedVideoAdUnitIds)
	{
		foreach (string rewardedVideoAdUnitId in rewardedVideoAdUnitIds) {
			if (!_rewardedVideoPluginsDict.ContainsKey (rewardedVideoAdUnitId)) {
				_rewardedVideoPluginsDict.Add (rewardedVideoAdUnitId, new MTGRewardedVideo (rewardedVideoAdUnitId));
			}
		}
	}

	public static void loadAppWallPluginsForAdUnits (string[] appWallAdUnitIds)
	{
		foreach (string appWallAdUnitId in appWallAdUnitIds) {
			if (!_appWallPluginsDict.ContainsKey (appWallAdUnitId)) {
				_appWallPluginsDict.Add (appWallAdUnitId, new MTGAppWall (appWallAdUnitId));
			}
		}
	}

	

	public static void loadNativePluginsForAdUnits (MTGNativeInfo[] nativeAdInfos)
	{
		foreach (MTGNativeInfo nativeAdInfo in nativeAdInfos) {
			if (!_nativePluginsDict.ContainsKey (nativeAdInfo.adUnitId)) {
				_nativePluginsDict.Add (nativeAdInfo.adUnitId, new MTGNative (nativeAdInfo));
				nativeMobInfo = nativeAdInfo;
			}
		}
	}

	public static void loadBannerPluginsForAdUnits (string[] bannerAdUnitIds)
	{
		foreach (string AdUnitId in bannerAdUnitIds) {
			if (!_bannerPluginsDict.ContainsKey (AdUnitId)) {
				_bannerPluginsDict.Add (AdUnitId, new MTGBanner (AdUnitId));
			}
		}
	}

	/*
	 *  init MTGSDK
	 */


	public static void initMTGSDK(string appId,string apiKey){ 
		Debug.LogError ("mintegral: initMTGSDK \n" + "------------------------------");
		MintegralConfig.initialMTGSDK (appId,apiKey);
	}

	/*
	 *  show UserPrivateInfoTips
	 */


	public static void showUserPrivateInfoTips(){

		MintegralConfig.showUserPrivateInfoTipsConfig ();
	}

	/*
	 *  set UserPrivateInfoType
	 */
	public static void setUserPrivateInfoType(string statusKey, string statusType){

		MintegralConfig.setUserPrivateInfoTypeConfig (statusKey,statusType);
	}

	/*
	 *  get AuthPrivateInfoStatus
	 */

	public static int userPrivateInfo(string statusKey){

		return MintegralConfig.userPrivateInfoConfig (statusKey);
	}

	/*
	 *  new set function
	 */
	public static void setConsentStatusInfoType(int statusType){

		MintegralConfig.setConsentStatusInfoTypeConfig (statusType);
	}

	/*
	 *  new get function
	 */

	public static bool getConsentStatusInfoType(){

		return MintegralConfig.getConsentStatusInfoTypeConfig ();
	}

	/*
	* ccpa configuration
	*/
	public static void setDoNotTrackStatus(int status){
		MintegralConfig.setDoNotTrackStatusConfig(status);
	}

	/*
	 * InterActive API
	 */
	public static void requestInterActiveAd (string adUnitId)
	{
		MTGInterActive plugin;
		if (_interActivePluginsDict.TryGetValue (adUnitId, out plugin)) {
			plugin.requestInterActiveAd ();
		} else {
			Debug.LogWarning (String.Format (ADUNIT_NOT_FOUND_MSG, adUnitId));
		}
	}


	// If an interActive ad is loaded this will take over the screen and show the ad
	public static void showInterActiveAd (string adUnitId)
	{
		MTGInterActive plugin;
		if (_interActivePluginsDict.TryGetValue (adUnitId, out plugin)) {
			plugin.showInterActiveAd ();
		} else {
			Debug.LogWarning (String.Format (ADUNIT_NOT_FOUND_MSG, adUnitId));
		}
	} 


	// int getInterActiveStatusAd
	public static int getInterActiveStatusAd (string adUnitId)
	{
		MTGInterActive plugin;
		if (_interActivePluginsDict.TryGetValue (adUnitId, out plugin)) {
			return plugin.getInterActiveStatusAd ();
		} else {
			return 0;
		}
	} 

	/*
	 * InterstitialVideo API
	 */
	public static void requestInterstitialVideoAd (string adUnitId)
	{
		MTGInterstitialVideo plugin;
		if (_interstitialVideoPluginsDict.TryGetValue (adUnitId, out plugin)) {
			plugin.requestInterstitialVideoAd ();
		} else {
			Debug.LogWarning (String.Format (ADUNIT_NOT_FOUND_MSG, adUnitId));
		}
	}

	public static bool isReady (string adUnitId)
	{
		bool ready = false;
		MTGInterstitialVideo plugin;
		if (_interstitialVideoPluginsDict.TryGetValue (adUnitId, out plugin)) {
			ready = plugin.isVideoReady (adUnitId);
		} else {
			Debug.LogWarning (String.Format (ADUNIT_NOT_FOUND_MSG, adUnitId));
		}
		return ready;
	}

	// If an interstitialVideo ad is loaded this will take over the screen and show the ad
	public static void showInterstitialVideoAd (string adUnitId)
	{
		MTGInterstitialVideo plugin;
		if (_interstitialVideoPluginsDict.TryGetValue (adUnitId, out plugin)) {
			plugin.showInterstitialVideoAd ();
		} else {
			Debug.LogWarning (String.Format (ADUNIT_NOT_FOUND_MSG, adUnitId));
		}
	} 

	public static void setIVRewardMode(string adUnitId, MTGIVRewardMode ivRewardMode,int playTime){
		MTGInterstitialVideo plugin;
		if (_interstitialVideoPluginsDict.TryGetValue (adUnitId, out plugin)) {
			plugin.setIVRewardMode(ivRewardMode,playTime);
		} else {
			Debug.LogWarning (String.Format (ADUNIT_NOT_FOUND_MSG, adUnitId));
		}
	}

	public static void setIVRewardMode(string adUnitId, MTGIVRewardMode ivRewardMode,float playRate){
		MTGInterstitialVideo plugin;
		if (_interstitialVideoPluginsDict.TryGetValue (adUnitId, out plugin)) {
			plugin.setIVRewardMode(ivRewardMode,playRate);
		} else {
			Debug.LogWarning (String.Format (ADUNIT_NOT_FOUND_MSG, adUnitId));
		}
	}

	public static void setAlertDialogText(string adUnitId, string title,string content,string comfirmText,string cancelText){
		MTGInterstitialVideo plugin;
		if (_interstitialVideoPluginsDict.TryGetValue (adUnitId, out plugin)) {
			plugin.setAlertDialogText (title,content,comfirmText,cancelText);
		} else {
			Debug.LogWarning (String.Format (ADUNIT_NOT_FOUND_MSG, adUnitId));
		}
	}


	/*
	 * Interstitial API
	 */
	public static void requestInterstitialAd (string adUnitId)
	{
		MTGInterstitial plugin;
		if (_interstitialPluginsDict.TryGetValue (adUnitId, out plugin)) {
			plugin.requestInterstitialAd ();
		} else {
			Debug.LogWarning (String.Format (ADUNIT_NOT_FOUND_MSG, adUnitId));
		}
	}

	

	// If an interstitial ad is loaded this will take over the screen and show the ad
	public static void showInterstitialAd (string adUnitId)
	{
		MTGInterstitial plugin;
		if (_interstitialPluginsDict.TryGetValue (adUnitId, out plugin)) {
			plugin.showInterstitialAd ();
		} else {
			Debug.LogWarning (String.Format (ADUNIT_NOT_FOUND_MSG, adUnitId));
		}
	}
		


	/*
	 * Rewarded Video API
	 */
	// Starts loading a rewarded video ad
	public static void requestRewardedVideo (string adUnitId)
	{
		MTGRewardedVideo plugin;
		if (_rewardedVideoPluginsDict.TryGetValue (adUnitId, out plugin)) {
			plugin.requestRewardedVideoAd (adUnitId);
		} else {
			Debug.LogWarning (String.Format (ADUNIT_NOT_FOUND_MSG, adUnitId));
		}
	}


	// If a rewarded video ad is loaded this will take over the screen and show the ad
	public static void showRewardedVideo (string adUnitId,string rewardId="rewardid",string userId="userId")
	{
		if (rewardId.Length == 0 || rewardId == null) {
			rewardId = "defaultRewardId";
		}
		MTGRewardedVideo plugin;
		if (_rewardedVideoPluginsDict.TryGetValue (adUnitId, out plugin)) {
			plugin.showRewardedVideoAd (adUnitId, rewardId, userId);
		} else {
			Debug.LogWarning (String.Format (ADUNIT_NOT_FOUND_MSG, adUnitId));
		}
	}

	

	public static bool isVideoReadyToPlay (string adUnitId)
	{
		bool ready = false;
		MTGRewardedVideo plugin;
		if (_rewardedVideoPluginsDict.TryGetValue (adUnitId, out plugin)) {
			ready = plugin.isVideoReady (adUnitId);
		} else {
			Debug.LogWarning (String.Format (ADUNIT_NOT_FOUND_MSG, adUnitId));
		}
		return ready;
	}


	public static void cleanAllVideoFileCache (string adUnitId)
	{
		MTGRewardedVideo plugin;
		if (_rewardedVideoPluginsDict.TryGetValue (adUnitId, out plugin)) {
			plugin.cleanVideoFileCache ();
		} else {
			Debug.LogWarning (String.Format (ADUNIT_NOT_FOUND_MSG, adUnitId));
		}
	}

	public static void setAlertWithTitle(string adUnitId, string title,string content,string comfirmText,string cancelText){
		MTGRewardedVideo plugin;
		if (_rewardedVideoPluginsDict.TryGetValue (adUnitId, out plugin)) {
			plugin.setAlertWithTitle (adUnitId,title,content,comfirmText,cancelText);
		} else {
			Debug.LogWarning (String.Format (ADUNIT_NOT_FOUND_MSG, adUnitId));
		}
	
	}

	/*
	 * AppWall API
	 */
	 // If an appWall ad is loaded this will take over the screen and show the ad
	public static void showAppWallAd (string adUnitId)
	 {
		MTGAppWall plugin;
		if (_appWallPluginsDict.TryGetValue (adUnitId, out plugin)) {
			plugin.showAppWallAd (adUnitId);
		} else {
			Debug.LogWarning (String.Format (ADUNIT_NOT_FOUND_MSG, adUnitId));
		}
	 }


	

  

	/*
	 * Native API
	 */

	public static void preRequestNativeAd (string adUnitId, string fb_placement_id, string categoryType, MTGTemplate[] supportedTemplate)
	{
		MTGNative plugin;
		if (_nativePluginsDict.TryGetValue (adUnitId, out plugin)) {

			plugin.preRequestNativeAd (adUnitId,fb_placement_id,categoryType,supportedTemplate);
		} else {
			Debug.LogWarning (String.Format (ADUNIT_NOT_FOUND_MSG, adUnitId));
		}
	}

	public static void requestNativeAd (string adUnitId)
	{ 
		#if UNITY_IPHONE
			MintegraliOSNative plugin;
			if (_nativePluginsDict.TryGetValue (adUnitId, out plugin)) {
				nativeMobManager =  plugin.nativeManager;
				plugin.requestNativeAd ();
			} else {
				Debug.LogWarning (String.Format (ADUNIT_NOT_FOUND_MSG, adUnitId));
			}
		#elif UNITY_ANDROID
			MintegralAndroidNative plugin;
			if (_nativePluginsDict.TryGetValue (adUnitId, out plugin)) {
				plugin.requestNativeAd ();
			} else {
				Debug.LogWarning (String.Format (ADUNIT_NOT_FOUND_MSG, adUnitId));
			}
		#endif
	}

	public static void registerViewNativeAd (string adUnitId,int left, int top, int width, int height, string index_campain)
	{
		#if UNITY_IPHONE

		#elif UNITY_ANDROID
			MintegralAndroidNative plugin;
			if (_nativePluginsDict.TryGetValue (adUnitId, out plugin)) {
				plugin.registerViewNativeAd (left,top,width,height,index_campain);
			} else {
				Debug.LogWarning (String.Format (ADUNIT_NOT_FOUND_MSG, adUnitId));
			}
		#endif
	}



	public static void unRegisterViewNativeAd (string adUnitId, string index_campain)
	{
		#if UNITY_IPHONE

		#elif UNITY_ANDROID
			MintegralAndroidNative plugin;
			if (_nativePluginsDict.TryGetValue (adUnitId, out plugin)) {
				plugin.unRegisterViewNativeAd (index_campain);
			} else {
				Debug.LogWarning (String.Format (ADUNIT_NOT_FOUND_MSG, adUnitId));
			}
		#endif
	}

	public static void createBanner (string adUnitId,BannerAdPosition position,int adWidth, int adHeight, bool isShowCloseBtn)
	{
		MTGBanner plugin;
		if (_bannerPluginsDict.TryGetValue (adUnitId, out plugin)) {
			plugin.createBanner (adUnitId,position,adWidth,adHeight,isShowCloseBtn);
		} else {
			Debug.LogWarning (String.Format (ADUNIT_NOT_FOUND_MSG, adUnitId));
		}
	}

	public static void destroyBanner (string adUnitId)
	{
		MTGBanner plugin;
		if (_bannerPluginsDict.TryGetValue (adUnitId, out plugin)) {
			plugin.destroyBanner (adUnitId);
		} else {
			Debug.LogWarning (String.Format (ADUNIT_NOT_FOUND_MSG, adUnitId));
		}
	}

}

public struct MTGNativeInfo
{
	public string adUnitId;
	public string fb_placement_id;
	public bool autoCacheImage;
	public MTGAdCategory adCategory;
	public MTGTemplate[] supportedTemplate;
}


public struct MTGOfferWallInfo
{
	public string adUnitId;
	public string userId;
	public MTGOfferWallAlertTips alertTips;
	public MTGAdCategory adCategory;
}

public struct MTGInterActiveInfo
{
	public string adUnitId;
}

public struct MTGInterstitialVideoInfo
{
	public string adUnitId;
}

public struct MTGInterstitialInfo
{
	public string adUnitId;
	public MTGAdCategory adCategory;
}



public struct MTGOfferWallAlertTips
{
	public string leftButtonTitle;
	public string rightButtonTitle;
	public string alertContent;
}


[System.Serializable]
public enum MTGAdCategory 
{
	MTGAD_CATEGORY_ALL  = 0,
	MTGAD_CATEGORY_GAME = 1,
	MTGAD_CATEGORY_APP  = 2
};

public enum MTGAdTemplateType 
{
	MTGAD_TEMPLATE_BIG_IMAGE = 2,
	MTGAD_TEMPLATE_ONLY_ICON = 3
};

public enum MTGAdSourceType 
{
	MTGAD_SOURCE_API_OFFER = 1,
	MTGAD_SOURCE_MY_OFFER  = 2,
	MTGAD_SOURCE_FACEBOOK  = 3,
	MTGAD_SOURCE_MINTEGRAL  = 4,
	MTGAD_SOURCE_PUBNATIVE = 5,
	MTGAD_SOURCE_ADMOB     = 6,
	MTGAD_SOURCE_MYTARGET  = 7,
	MTGAD_SOURCE_NATIVEX   = 8,
	MTGAD_SOURCE_APPLOVIN  = 9
};

[System.Serializable]
public struct MTGTemplate
{
	public	MTGAdTemplateType templateType;
	public  int adsNum;
}

[System.Serializable]
public class MTGTemplateWrapper { 
	public MTGTemplate[] objects; 
}

[System.Serializable]
public class MTGCampaignTrack{ 
	public MTGCampaign campaign;
	public string url;
}



#endif
