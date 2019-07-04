using UnityEngine;
using System.Collections;

/// <summary>
/// 调试使用工具
/// </summary>
public class ToolReporter : MonoBehaviour {

    // Use this for initialization
    void Start () {
	
	}
	
    public static void Create()
    {

#if TOOL_REPORTER
        GameObject Obj = new GameObject();
        Obj.name = "Reporter";
        Obj.AddComponent<ReporterMessageReceiver>();
        Obj.AddComponent<Reporter>();
        GameObject.DontDestroyOnLoad(Obj);
#endif

    }


}
