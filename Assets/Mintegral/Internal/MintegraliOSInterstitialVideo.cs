using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using AOT;


#if UNITY_IPHONE

public class MintegraliOSInterstitialVideo
{
	private System.IntPtr interstitialVideoManager;

	[DllImport ("__Internal")]
	private static extern System.IntPtr initInterstitialVideo (string unitId);

	[DllImport ("__Internal")]
	private static extern void preloadInterstitialVideo (System.IntPtr instance);

	[DllImport ("__Internal")]
	private static extern void showInterstitialVideo (System.IntPtr instance);

	[DllImport ("__Internal")]
	private static extern void setIVRewardModeTime (System.IntPtr instance,Mintegral.MTGIVRewardMode ivRewardMode,int playTime);

	[DllImport ("__Internal")]
	private static extern void setIVRewardModeRate (System.IntPtr instance,Mintegral.MTGIVRewardMode ivRewardMode,float playRate);

	[DllImport ("__Internal")]
	private static extern void setRewardAlertDialogText (System.IntPtr instance,string title,string content,string comfirmText,string cancelText);

	[DllImport ("__Internal")]
	private static extern bool isIVReady (System.IntPtr instance,string unitId);





	public void showInterstitialVideoAd()
	{
		if (Application.platform != RuntimePlatform.OSXEditor) {

			if (Application.platform == RuntimePlatform.IPhonePlayer){
				if (interstitialVideoManager == System.IntPtr.Zero) return;

				showInterstitialVideo (interstitialVideoManager);
			}
		}
	}

	public void requestInterstitialVideoAd()
	{
		if (Application.platform != RuntimePlatform.OSXEditor) {

			if (Application.platform == RuntimePlatform.IPhonePlayer){
				if (interstitialVideoManager == System.IntPtr.Zero)  return;

				preloadInterstitialVideo (interstitialVideoManager);
			}
		}
	}

	public  MintegraliOSInterstitialVideo(MTGInterstitialVideoInfo info)
	{
		if (Application.platform != RuntimePlatform.OSXEditor) { 
			if (Application.platform == RuntimePlatform.IPhonePlayer){
				interstitialVideoManager = initInterstitialVideo (info.adUnitId);
			}
		}
	}

	public bool isVideoReady (string unitId)
	{
		bool ready = false;
		if (Application.platform != RuntimePlatform.OSXEditor) { 
			if (Application.platform == RuntimePlatform.IPhonePlayer){
				if (interstitialVideoManager == System.IntPtr.Zero)  return ready;
				
				ready = isIVReady(interstitialVideoManager,unitId);
			}
		}
		return ready;
	}

	public void setIVRewardMode (Mintegral.MTGIVRewardMode ivRewardMode,int playTime)
	{
		if (Application.platform != RuntimePlatform.OSXEditor) { 
			if (Application.platform == RuntimePlatform.IPhonePlayer){
				if (interstitialVideoManager == System.IntPtr.Zero)  return;
				
				setIVRewardModeTime(interstitialVideoManager,ivRewardMode,playTime);
			}
		}
	}

	public void setIVRewardMode (Mintegral.MTGIVRewardMode ivRewardMode,float playRate)
	{
		if (Application.platform != RuntimePlatform.OSXEditor) { 
			if (Application.platform == RuntimePlatform.IPhonePlayer){
				if (interstitialVideoManager == System.IntPtr.Zero)  return;
				
				setIVRewardModeRate(interstitialVideoManager,ivRewardMode,playRate);
			}
		}
	}

	public void setAlertDialogText(string title,string content,string comfirmText,string cancelText){
		if (Application.platform != RuntimePlatform.OSXEditor) { 
			if (Application.platform == RuntimePlatform.IPhonePlayer){
				if (interstitialVideoManager == System.IntPtr.Zero)  return;
				
				setRewardAlertDialogText(interstitialVideoManager,title,content,comfirmText,cancelText);
			}
		}
	}

}
#endif