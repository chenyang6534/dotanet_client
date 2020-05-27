using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using AOT;


#if UNITY_IPHONE

public class MintegraliOSAppWall
{
	System.IntPtr appWallManager;

 //  	[DllImport ("__Internal")]
	// private static extern System.IntPtr initAppWallAds (string unitId);

 //  	[DllImport ("__Internal")]
	// private static extern void showAppWall (System.IntPtr instance);



  	public  void showAppWallAd (string unitId)
  	{
    	if (Application.platform != RuntimePlatform.OSXEditor) {

      	if (Application.platform == RuntimePlatform.IPhonePlayer){
			if (appWallManager == System.IntPtr.Zero)
          	return; 
        	// showAppWall (appWallManager);
      	}
    	}
  	}

	public MintegraliOSAppWall(string adUnitId)
  	{
		if (Application.platform != RuntimePlatform.OSXEditor) {

			if (Application.platform == RuntimePlatform.IPhonePlayer){
				// appWallManager = initAppWallAds (adUnitId);
        return;
			}
		}
  	}


}
#endif
