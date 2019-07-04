using UnityEngine;
using System.Collections;
using System;

namespace cn.sharesdk.unity3d 
{
	[Serializable]
	public class DevInfoSet
	{
		//public QQ qq = new QQ();
		//public QZone qzone = new QZone();
		public WeChat wechat = new WeChat();
		public WeChatMoments wechatMoments = new WeChatMoments(); 
		
	}

	public class DevInfo 
	{	
		public bool Enable = true;
	}

    [Serializable]
	public class WeChat : DevInfo 
	{	
		#if UNITY_ANDROID
		public string SortId = "5";
		public const int type = (int) PlatformType.WeChat;
		public string AppId = "wx9ec6eb660fd6dbc3";
		public string AppSecret = "d59a5111cb7d8258d24afd83bc56af89";
		//public string userName = "gh_afb25ac019c9@app";
		//public string path = "/page/API/pages/share/share";
		public bool BypassApproval = false;
#elif UNITY_IPHONE
		public const int type = (int) PlatformType.WeChat;
		public string app_id = "wx9ec6eb660fd6dbc3";
		public string app_secret = "d59a5111cb7d8258d24afd83bc56af89";
#endif
    }

    [Serializable]
	public class WeChatMoments : DevInfo 
	{
		#if UNITY_ANDROID
		public string SortId = "6";
		public const int type = (int) PlatformType.WeChatMoments;
		public string AppId = "wx9ec6eb660fd6dbc3";
		public string AppSecret = "d59a5111cb7d8258d24afd83bc56af89";
		public bool BypassApproval = false;
#elif UNITY_IPHONE
		public const int type = (int) PlatformType.WeChatMoments;
		public string app_id = "wx9ec6eb660fd6dbc3";
		public string app_secret = "d59a5111cb7d8258d24afd83bc56af89";
#endif
    }



}
