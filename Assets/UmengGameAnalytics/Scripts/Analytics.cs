
//  Created by ZhuCong on 1/1/14.
//  Copyright 2014 Umeng.com . All rights reserved.
//
//#undef UNITY_EDITOR

using UnityEngine;
using System;
using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using System.Text;





namespace Umeng
{

    public partial class Analytics
    {


	    
#if UNITY_ANDROID
		static bool hasInit = false;
        protected static AndroidJavaClass UMConfigure = new AndroidJavaClass("com.umeng.commonsdk.UMConfigure");
        protected static  AndroidJavaClass Agent = new AndroidJavaClass("com.umeng.analytics.game.UMGameAgent");
        protected static  AndroidJavaObject Context = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");


#endif

        private const string Version = "3.2";
	    public enum DeviceType
	    {
		    Phone = 0,
		    Box = 1
	    }


		
		


		




	
        //iOS Android Universal API



        // 
        /// <summary>
        /// 开始友盟统计 默认发送策略为启动时发送
        /// </summary>
        /// <param name="appKey">友盟appKey</param>
        /// <param name="channelId">渠道名称</param>
        ///
        ///
        public static void StartWithAppKeyAndChannelId(string appKey, string channelId)
        {
#if UNITY_EDITOR
            //Debug.LogWarning("友盟统计在iOS/Android 真机上才会向友盟后台服务器发送事件 请在真机上测试");

#elif UNITY_IPHONE

       
	        UMCommonSetAppkeyAndChannel(appKey,channelId);
            //iOS 必须设置 android 无此API
	        _SetScenarioType();


#elif UNITY_ANDROID




	        UMCommonSetAppkeyAndChannel(appKey,channelId);
            //2次init 第一次UMConfigureinit在UMCommonSetAppkeyAndChannel方法内,第二次在这里UMGameAgent.init()
            Agent.CallStatic("init",Context);


			if(!hasInit)
			{
				//Debug.LogWarning("onResume");
				onResume();

				AddUmengAndroidLifeCycleCallBack();
				hasInit = true;
			}


#endif
        }

        private static void UMCommonSetAppkeyAndChannel(string appkey, string channelId,DeviceType deviceType=DeviceType.Phone,string pushSecret=null)
	    {

#if UNITY_EDITOR


#elif UNITY_IPHONE


			_SetWrapperTypeAndVersion("Unity", Version);
			_UMCommonSetAppkeyAndChannel(appkey, channelId);

#elif UNITY_ANDROID
           
		    UMConfigure.CallStatic("setWraperType","Unity",Version);
		    UMConfigure.CallStatic("init",Context,appkey,channelId,(int)deviceType,pushSecret);
#endif
        }


        /// <summary>
        /// 设置是否打印sdk的信息,默认不开启
        /// </summary>
        /// <param name="value">设置为true,Umeng SDK 会输出日志信息,记得release产品时要设置回false.</param>
        /// 
        public static void SetLogEnabled(bool value)
        {
#if UNITY_EDITOR
        //Debug.Log("SetLogEnabled");  
#elif UNITY_IPHONE
        _SetLogEnabled(value);
#elif UNITY_ANDROID
            

	        UMConfigure.CallStatic("setLogEnabled",value);
#endif
        }

        //使用前，请先到友盟App管理后台的设置->编辑自定义事件 中添加相应的事件ID，然后在工程中传入相应的事件ID
        //eventId、attributes中key和value都不能使用空格和特殊字符，且长度不能超过255个字符（否则将截取前255个字符）
        //id， ts， du是保留字段，不能作为eventId及key的名称


        /// <summary>
        /// 基本事件
        /// </summary>
        /// <param name="eventId">友盟后台设定的事件Id</param>
        public static void Event(string eventId)
        {
#if UNITY_EDITOR
        //Debug.Log("Event");  
#elif UNITY_IPHONE
        _Event(eventId);
#elif UNITY_ANDROID
            Agent.CallStatic("onEvent", Context, eventId);
#endif
        }
        //不同的标签会分别进行统计，方便同一事件的不同标签的对比,为nil或空字符串时后台会生成和eventId同名的标签.


        /// <summary>
        /// 基本事件
        /// </summary>
        /// <param name="eventId">友盟后台设定的事件Id</param>
        /// <param name="label">分类标签</param>

        public static void Event(string eventId, string label)
        {
#if UNITY_EDITOR 
        //Debug.Log("Event");  
#elif UNITY_IPHONE
        _EventWithLabel(eventId, label);
#elif UNITY_ANDROID
            Agent.CallStatic("onEvent", Context, eventId, label);
#endif
        }




        /// <summary>
        /// 属性事件
        /// </summary>
        /// <param name="eventId">友盟后台设定的事件Id</param>
        /// <param name="attributes"> 属性中的Key-Vaule Pair不能超过10个</param>
        public static void Event(string eventId, Dictionary<string, string> attributes)
        {
#if UNITY_EDITOR
        	//Debug.Log("Event");        
#elif UNITY_IPHONE
			_EventWithAttributes(eventId, DictionaryToJson(attributes));
#elif UNITY_ANDROID
            Agent.CallStatic("onEvent", Context, eventId, ToJavaHashMap(attributes));
#endif

        }


	    /// <summary>
	    /// 页面时长统计,记录某个页面被打开多长时间
	    /// </summary>
	    /// <param name="pageName">被统计view名称</param>
	    /// 
        public static void PageBegin(string pageName)
        {
#if UNITY_EDITOR
		    //Debug.Log("PageBegin");
#elif UNITY_IPHONE
            _BeginLogPageView(pageName);
#elif UNITY_ANDROID
            Agent.CallStatic("onPageStart", pageName);
#endif
        }

        /// <summary>
        /// 页面时长统计,记录某个页面被打开多长时间
        /// 与PageBegin配对使用
        /// </summary>
        /// <param name="pageName">被统计view名称</param>
        /// 
        public static void PageEnd(string pageName)
        {
#if UNITY_EDITOR
		    //Debug.Log("PageEnd"); 
#elif UNITY_IPHONE
            _EndLogPageView(pageName);
#elif UNITY_ANDROID
            Agent.CallStatic("onPageEnd", pageName);
#endif
        }

		/// <summary>
		/// 自定义事件 — 计算事件数
		/// </summary>
		public static void Event(string eventId, Dictionary<string, string> attributes, int value)
		{
			try
			{
				if (attributes == null)
					attributes = new System.Collections.Generic.Dictionary<string, string>();
				if (attributes.ContainsKey("__ct__"))
				{
					attributes["__ct__"] = value.ToString();
					Event(eventId, attributes);
				}
				else
				{
					attributes.Add("__ct__", value.ToString());
					Event(eventId, attributes);
					attributes.Remove("__ct__");
				}

			}
			catch (Exception)
			{
			}
		}








		public static string GetTestDeviceInfo()
		{
#if UNITY_EDITOR
			//Unity Editor 模式下 返回null 请在iOS/Anroid真机上测试
			//Debug.Log("GetDeviceInfo return null");
			return null;
#elif UNITY_IPHONE
			return _GetDeviceID();
#elif UNITY_ANDROID

            Debug.Log("GetDeviceInfo return ");
            return UMConfigure.CallStatic<String[]>("getTestDeviceInfo", Context)[0]; 

#else
			return null;

#endif

        }

		//设置是否对日志信息进行加密, 默认false(不加密).
		//value 设置为true, SDK会将日志信息做加密处理
		
		public static void  SetLogEncryptEnabled(bool value)
		{
			#if UNITY_EDITOR
			//Debug.Log("SetLogEncryptEnabled");
			#elif UNITY_IPHONE
			_SetEncryptEnabled(value);
			#elif UNITY_ANDROID
			UMConfigure.CallStatic("setEncryptEnabled",value);
			#endif


		}
		
		
		public static void SetLatency(int value)
		{
			#if UNITY_EDITOR
			//Debug.Log("SetLatency");
			#elif UNITY_IPHONE
			//_SetLatency(value);
			#elif UNITY_ANDROID
			Agent.CallStatic("setLatencyWindow", (long)value);
			#endif

		}
		




        //Android Only

#if  UNITY_ANDROID



        //设置Session时长
        public static void SetContinueSessionMillis(long milliseconds)
        {
#if UNITY_EDITOR
            //Debug.Log("setContinueSessionMillis"); 
#else
            Agent.CallStatic("setSessionContinueMillis", milliseconds);
#endif
        }



		/*
			android6.0中采集mac方式变更，新增接口 public static void setCheckDevice(boolean enable) 该接口默认参数是true，即采集mac地址，
			但如果开发者需要在googleplay发布，考虑到审核风险，可以调用该接口，参数设置为 false 就不会采集mac地址。

		*/
		public static void SetCheckDevice(bool value)
		{
#if UNITY_EDITOR
		
#else
			Agent.CallStatic("setCheckDevice", value);
#endif

		}
		



		
		

		

#endif



        //iOS Only
#if UNITY_IPHONE



    /// <summary>
    /// 页面时长统计,记录某个view被打开多长时间,与调用PageBegin,PageEnd计时等价
    /// </summary>
    /// <param name="pageName">被统计view名称</param>
    /// <param name="seconds">时长单位为秒</param>
    /// 
    public static void LogPageViewWithSeconds(string pageName, int seconds)
    {
#if UNITY_EDITOR
        //Debug.Log("LogPageViewWithSeconds"); 
#else
        _LogPageViewWithSeconds(pageName, seconds);
#endif
    }



    /// <summary>
    /// 判断设备是否越狱，判断方法根据 apt和Cydia.app的path来判断
    /// </summary>
    /// <returns>是否越狱</returns>
    public static bool IsJailBroken()
    {
#if UNITY_EDITOR
        //always return false in UNITY_EDITOR mode
        //Debug.Log("IsJailBroken always return false in UNITY_EDITOR mode");
        return false;
#else
        return _IsJailBroken();
#endif
    }

    /// <summary>
    /// 判断你的App是否被破解
    /// </summary>
    /// <returns>是否破解</returns>
    public static bool IsPirated()
    {
#if UNITY_EDITOR
        //always return false in UNITY_EDITOR mode
        //Debug.Log("IsPirated always return false in UNITY_EDITOR mode");
        return false;
#else
        return _IsPirated();
#endif
    }







#endif



		





        #region Wrapper


	    private static void AddUmengAndroidLifeCycleCallBack()
        {
            GameObject go = new GameObject();
            go.AddComponent<UmengAndroidLifeCycleCallBack>();
            go.name = "UmengManager";
        }

#if UNITY_ANDROID

        public static void onResume()
        {
#if UNITY_EDITOR

#else
            Agent.CallStatic("onResume", Context);
#endif
        }



        public  static void onPause()
        {
#if UNITY_EDITOR

#else
            Agent.CallStatic("onPause", Context);
#endif
        }

	    public static void onKillProcess()
	    {
#if UNITY_EDITOR

#else
		    Agent.CallStatic("onKillProcess", Context);

#endif
	    }

	  







#endif


#if UNITY_IPHONE && !UNITY_EDITOR



     


        static string DictionaryToJson(Dictionary<string, string> dict)
        {

            var builder = new StringBuilder("{");
            foreach (KeyValuePair<string, string> kv in dict)
            {
                builder.AppendFormat("\"{0}\":\"{1}\",", kv.Key, kv.Value);
            }
            builder[builder.Length - 1] = '}';
            return builder.ToString();


        }





	    [DllImport("__Internal")]
	    private static extern void _SetWrapperTypeAndVersion(string wrapperType,string wrapperVersion);
		    
		[DllImport("__Internal")]
		private static extern void _SetLogEnabled(bool value);
	    
	    [DllImport("__Internal")]
	    private static extern void _SetScenarioType();
	    
	    [DllImport("__Internal")]
	    private static extern void _UMCommonSetAppkeyAndChannel(string appkey, string channelId);









    [DllImport("__Internal")]
    private static extern void _Event(string eventId);



    [DllImport("__Internal")]
    private static extern void _EventWithLabel(string eventId, string label);



    [DllImport("__Internal")]
    private static extern void _EventWithAttributes(string eventId, string jsonstring);





    [DllImport("__Internal")]
    private static extern void _LogPageViewWithSeconds(string pageName, int seconds);

    [DllImport("__Internal")]
    private static extern void _BeginLogPageView(string pageName);

    [DllImport("__Internal")]
    private static extern void _EndLogPageView(string pageName);

    [DllImport("__Internal")]
    private static extern bool _IsJailBroken();
	
    [DllImport("__Internal")]
    private static extern bool _IsPirated();
	

	[DllImport("__Internal")]
	private static extern string _GetDeviceID();


		
	[DllImport("__Internal")]
	private static extern  void _SetEncryptEnabled(bool value);
		


				
#endif
        #endregion
    }
}
