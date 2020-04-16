//
//  UmengManager.cs
//
//  Created by ZhuCong on 1/1/14.
//  Copyright 2014 Umeng.com . All rights reserved.
//  Version 1.31
//  请不要改动此类的内容,也不要手动添加此类到gameObject上.Umeng SDK会自动实例化此类

using UnityEngine;
using System.Collections;
using Umeng;

public class UmengAndroidLifeCycleCallBack : MonoBehaviour
{
	#if UNITY_ANDROID

	void Awake()
	{
		DontDestroyOnLoad (transform.gameObject);
	}
	

	
	void OnApplicationPause(bool isPause)
	{
		
		//Debug.Log("Umeng:OnApplicationPause" + isPause);
		if (isPause){
			//Debug.Log("Umeng:----onPause");
			GA.onPause();
		}
		else{
			//Debug.Log("Umeng:----onResume");
			GA.onResume();
		}
		
	}
	
	
	
	void OnApplicationQuit()
	{
		//Debug.Log("Umeng:OnApplicationQuit");
		GA.onKillProcess();
	}
	
	
	
	#endif
	
	
	
	
	
}
