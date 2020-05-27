using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using AOT;


#if UNITY_IPHONE

public class MintegraliOSInterActive
{
  private System.IntPtr interActiveManager;

  [DllImport ("__Internal")]
	private static extern System.IntPtr initInterActive (string unitId);

  [DllImport ("__Internal")]
	private static extern void loadInterActive (System.IntPtr instance);

  [DllImport ("__Internal")]
	private static extern void showInterActive (System.IntPtr instance);

  [DllImport ("__Internal")]
	private static extern int getInterActiveStatus (System.IntPtr instance);



	public void showInterActiveAd()
	  {
	  	if (Application.platform != RuntimePlatform.OSXEditor) {

	      if (Application.platform == RuntimePlatform.IPhonePlayer){
				if (interActiveManager == System.IntPtr.Zero) return;

				showInterActive (interActiveManager);
	        }
	  	}
	  }

	public void requestInterActiveAd()
	  {
	  	if (Application.platform != RuntimePlatform.OSXEditor) {

	      if (Application.platform == RuntimePlatform.IPhonePlayer){
			if (interActiveManager == System.IntPtr.Zero)  return;

				loadInterActive (interActiveManager);
	      }
	  	}
	  }

	public int getInterActiveStatusAd ()
	{
		if (Application.platform != RuntimePlatform.OSXEditor) {

			return getInterActiveStatus(interActiveManager);

		} else {
			return 0;
		}
	}

	public  MintegraliOSInterActive(MTGInterActiveInfo info)
	  {
	  	if (Application.platform != RuntimePlatform.OSXEditor) { 
	      if (Application.platform == RuntimePlatform.IPhonePlayer){
			interActiveManager = initInterActive (info.adUnitId);
	      }
	  	}
	  }


}
#endif