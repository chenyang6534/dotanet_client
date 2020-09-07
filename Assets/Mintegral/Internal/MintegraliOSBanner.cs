using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using AOT;

#if UNITY_IPHONE

public class MintegraliOSBanner
{
	private System.IntPtr bannerInstance;

	[DllImport ("__Internal")]
	private static extern System.IntPtr createMTGBanner (string adUnitId,Mintegral.BannerAdPosition position,int adWidth, int adHeight, bool isShowCloseBtn);

	[DllImport ("__Internal")]
	private static extern void destroyMTGBanner (System.IntPtr instance);

	public MintegraliOSBanner (string adUnitId)
	{
		if (Application.platform != RuntimePlatform.OSXEditor) {
	      if (Application.platform != RuntimePlatform.IPhonePlayer){
				return;
	      }
		}
	}


	public void createBanner (string adUnitId,Mintegral.BannerAdPosition position,int adWidth, int adHeight, bool isShowCloseBtn)
	{
		if (Application.platform != RuntimePlatform.OSXEditor) {
	      if (Application.platform == RuntimePlatform.IPhonePlayer){
				bannerInstance =  createMTGBanner (adUnitId,position,adWidth,adHeight,isShowCloseBtn);
	      }
		}
	}


	public  void destroyBanner (string adUnitId)
	{
		if (Application.platform != RuntimePlatform.OSXEditor) {
	      if (Application.platform == RuntimePlatform.IPhonePlayer){
	      		if (bannerInstance == System.IntPtr.Zero) return;
				destroyMTGBanner (bannerInstance);
	        }
		}
	}

}
#endif
