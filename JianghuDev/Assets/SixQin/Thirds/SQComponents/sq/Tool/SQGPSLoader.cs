using UnityEngine;
using System.Collections;

public class SQGPSLoader : MonoBehaviour {

    //经度
    private float mGpsN = 0f;
    //续度
    private float mGpnE = 0f;
    //是否获取完毕
    private bool isFinished = false;

    public delegate void GetGpsInformationEvent(GPSType type, float n, float e, double time);
    public GetGpsInformationEvent OnGetGpsInfoCall;

	// Use this for initialization
	void Start () {
	    
	}

    public void StartGpsInfo()
    {
        StartCoroutine(StartGPS());
    }

    IEnumerator StartGPS()
    {
        // Input.location 用于访问设备的位置属性（手持设备）, 静态的LocationService位置  
        // LocationService.isEnabledByUser 用户设置里的定位服务是否启用  
        if (!Input.location.isEnabledByUser)
        {
            SQDebug.Log( "isEnabledByUser value is:" + Input.location.isEnabledByUser.ToString() + " Please turn on the GPS");
            if (OnGetGpsInfoCall != null)
            {
                OnGetGpsInfoCall(GPSType.Error, mGpsN, mGpsN, Input.location.lastData.timestamp);
            }
            yield return false;
        }

        // LocationService.Start() 启动位置服务的更新,最后一个位置坐标会被使用  
        Input.location.Start(10.0f, 10.0f);

        SQDebug.PrintToScreen("Input.location.status=" + Input.location.status);

        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            SQDebug.PrintToScreen("status=" + Input.location.status);
            // 暂停协同程序的执行(1秒)  
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        if (maxWait < 1)
        {
            SQDebug.Log("Init GPS service time out");
            yield return false;
        }

        if (Input.location.status == LocationServiceStatus.Failed)
        {
            SQDebug.Log("Unable to determine device location");
            yield return false;
        }
        else
        {
            SQDebug.PrintToScreen("N:" + Input.location.lastData.latitude + " E:" + Input.location.lastData.longitude);
            SQDebug.Log("N:" + Input.location.lastData.latitude + " E:" + Input.location.lastData.longitude);
            mGpsN = Input.location.lastData.latitude;
            mGpnE = Input.location.lastData.longitude;

            if(OnGetGpsInfoCall != null)
            {
                OnGetGpsInfoCall(GPSType.Normal, mGpsN, mGpnE, Input.location.lastData.timestamp);
            }
            //this.gps_info = this.gps_info + " Time:" + Input.location.lastData.timestamp;
            yield return new WaitForSeconds(100);
        }
    }
}

public enum GPSType
{
    Normal = 1,
    Error =2
}