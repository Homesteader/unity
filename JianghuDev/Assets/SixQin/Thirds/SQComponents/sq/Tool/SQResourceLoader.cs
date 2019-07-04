using UnityEngine;
using System.Collections;

public class SQResourceLoader : MonoBehaviour {



	/// <summary>
	///  根据不同的平台进行路径转换加载
	/// </summary>
	/// <returns>The asset bundle.</returns>
	/// <param name="path">Path.</param>
	public static string LoadAssetBundle(string fileName){

		string loadPath = "";

		switch (Application.platform) {
		case RuntimePlatform.Android:
			loadPath = "jar:file://" + Application.dataPath + "!/assets/"+fileName;		
			break;
		case RuntimePlatform.IPhonePlayer:
			loadPath = "file://" + Application.dataPath + "/Raw/"+fileName;		
			break;
		case RuntimePlatform.OSXEditor:
		case RuntimePlatform.WindowsEditor:
			loadPath = "file://" + Application.dataPath + "/StreamingAssets/"+fileName;
			break;

		}
        SQDebug.PrintToScreen("SQDebug-Path=" + loadPath);
		return loadPath;
	}



}
