using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using AOT;

#if UNITY_IPHONE

public class MintegraliOSRewardVideo
{

	[DllImport ("__Internal")]
	private static extern void loadRewardView (string unitId);

	[DllImport ("__Internal")]
	private static extern bool isReady (string unitId);

	[DllImport ("__Internal")]
	private static extern void showRewardView (string unitId, string rewardId,string userId);

	[DllImport ("__Internal")]
	private static extern void cleanRewardView ();

	[DllImport ("__Internal")]
	private static extern void setAlertDialogWithTitle (string adUnitId,string title,string content,string comfirmText,string cancelText);


	public void cleanVideoFileCache()
	{
		if (Application.platform != RuntimePlatform.OSXEditor) {
	      if (Application.platform == RuntimePlatform.IPhonePlayer){
				cleanRewardView ();
	      }
		}
	}


	public  void showRewardedVideoAd(string adUnitId,string rewardId,string userId)
	{
		if (Application.platform != RuntimePlatform.OSXEditor) {
	      if (Application.platform == RuntimePlatform.IPhonePlayer){

				showRewardView (adUnitId, rewardId,userId);
	        }
		}
	}


	public  bool isVideoReady(string adUnitId)
	{
		bool ready = false;
		if (Application.platform != RuntimePlatform.OSXEditor) {
	      if (Application.platform == RuntimePlatform.IPhonePlayer){
				ready = isReady (adUnitId);
	      }
		}
		return ready;
	}


	public  void requestRewardedVideoAd(string adUnitId)
	{
		if (Application.platform != RuntimePlatform.OSXEditor) {

	      if (Application.platform == RuntimePlatform.IPhonePlayer){

				loadRewardView (adUnitId);
	      }
		}
	}

	public  void setAlertWithTitle(string adUnitId,string title,string content,string comfirmText,string cancelText)
	{
		if (Application.platform != RuntimePlatform.OSXEditor) {

	      if (Application.platform == RuntimePlatform.IPhonePlayer){

				setAlertDialogWithTitle (adUnitId,title,content,comfirmText,cancelText);
	      }
		}
	}


	public MintegraliOSRewardVideo(string adUnitId)
	{

	}
}
#endif
