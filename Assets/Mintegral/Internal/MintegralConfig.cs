using UnityEngine;
using System.Collections.Generic;
using System.Runtime.InteropServices;



public class MintegralConfig
{
	private static bool isInit = false;

	#if UNITY_IPHONE

	[DllImport ("__Internal")]
	private static extern void initApplication (string appId, string apiKey);

	[DllImport ("__Internal")]
	private static extern void showUserPrivateInfoTips ();

	[DllImport ("__Internal")]
	private static extern void setUserPrivateInfoType (string statusKey, string statusType);

	[DllImport ("__Internal")]
	private static extern int getAuthPrivateInfoStatus (string statusKey);

	[DllImport ("__Internal")]
	private static extern void setConsentStatusInfoType (int statusType);

	[DllImport ("__Internal")]
	private static extern bool getConsentStatusInfoType ();

	[DllImport ("__Internal")]
	private static extern void setDoNotTrackStatusType (int statusType);

	#elif UNITY_ANDROID

	private static readonly AndroidJavaObject _staticObject = new AndroidJavaObject ("com.mintegral.msdk.unity.UnityAndroidBridge");

	#endif


	public static void initialMTGSDK (string appId,string apiKey)
	{
		if (isInit)
			return;

		isInit = true;
		
		if (Application.platform != RuntimePlatform.OSXEditor) {
			
			#if UNITY_IPHONE

			initApplication (appId, apiKey);

			#elif UNITY_ANDROID

			_staticObject.Call ("initApplication",appId,apiKey);  

			#endif
		}
	}

	public static void showUserPrivateInfoTipsConfig ()
	{
		if (Application.platform != RuntimePlatform.OSXEditor) {

			#if UNITY_IPHONE

			showUserPrivateInfoTips();

			#elif UNITY_ANDROID

			_staticObject.Call ("showUserPrivateInfoTips");  

			#endif
		}
	}

	public static void setUserPrivateInfoTypeConfig (string statusKey, string statusType)
	{
		if (Application.platform != RuntimePlatform.OSXEditor) {

			#if UNITY_IPHONE

			setUserPrivateInfoType(statusKey,statusType);

			#elif UNITY_ANDROID

			_staticObject.Call ("setUserPrivateInfoType",statusKey,statusType);  

			#endif
		}
	}

	public static int userPrivateInfoConfig (string statusKey)
	{
		if (Application.platform != RuntimePlatform.OSXEditor) {

			#if UNITY_IPHONE

			return getAuthPrivateInfoStatus(statusKey);

			#elif UNITY_ANDROID

			return _staticObject.Call <int> ("getAuthPrivateInfoStatus", statusKey);  

			#endif
		} else {
			return 0;
		}
	}

	public static void setConsentStatusInfoTypeConfig (int statusType)
	{
		if (Application.platform != RuntimePlatform.OSXEditor) {

			#if UNITY_IPHONE

			setConsentStatusInfoType(statusType);

			#elif UNITY_ANDROID

			_staticObject.Call ("setConsentStatusInfoType",statusType);  

			#endif
		}
	}

	public static bool getConsentStatusInfoTypeConfig()
	{
		bool ready = false;
		if (Application.platform != RuntimePlatform.OSXEditor) {
				
			#if UNITY_IPHONE

				ready = getConsentStatusInfoType ();

			#elif UNITY_ANDROID

				ready =  _staticObject.Call<bool> ("getConsentStatusInfoType");

			#endif
		}
		return ready;
	}

	public static void setDoNotTrackStatusConfig(int status){
		if (Application.platform != RuntimePlatform.OSXEditor) {

			#if UNITY_IPHONE

			setDoNotTrackStatusType(status);

			#elif UNITY_ANDROID

			_staticObject.Call ("setDoNotTrackStatusType",status);  

			#endif
		}
	}
}

