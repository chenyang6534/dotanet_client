using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using AOT;


#if UNITY_IPHONE

public class MintegraliOSInterstitial
{
  private System.IntPtr interstitialManager;

  [DllImport ("__Internal")]
  private static extern System.IntPtr initInterstitial (string unitId,string adCategory);

  [DllImport ("__Internal")]
	private static extern void preloadInterstitial (System.IntPtr instance);

  [DllImport ("__Internal")]
  private static extern void showInterstitial (System.IntPtr instance);



  public void showInterstitialAd()
  {
  	if (Application.platform != RuntimePlatform.OSXEditor) {

      if (Application.platform == RuntimePlatform.IPhonePlayer){
			if (interstitialManager == System.IntPtr.Zero) return;

				showInterstitial (interstitialManager);
        }
  	}
  }

  public void requestInterstitialAd()
  {
  	if (Application.platform != RuntimePlatform.OSXEditor) {

      if (Application.platform == RuntimePlatform.IPhonePlayer){
		if (interstitialManager == System.IntPtr.Zero)  return;

			preloadInterstitial (interstitialManager);
      }
  	}
  }

	public  MintegraliOSInterstitial(MTGInterstitialInfo info)
  {
  	if (Application.platform != RuntimePlatform.OSXEditor) {
  		string adCategoryStr = ((MTGAdCategory)info.adCategory).ToString ("d");
	      if (Application.platform == RuntimePlatform.IPhonePlayer){
				interstitialManager = initInterstitial (info.adUnitId, adCategoryStr);
	        }
  	}
  }

}
#endif