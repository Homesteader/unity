using UnityEngine;
using System.Collections;
using System;

public class Battery2Component : MonoBehaviour
{

    public GameObject[] Icons;
    public UISlider battleSlider;
    public enum BattleType
    {
        Precent,        //百分比
        GeZi            //格子
    }
    public BattleType battleType;

    private float startTime;
    // Use this for initialization
    void Start()
    {
        startTime = Time.time;
        SetBattery();
    }

    /// <summary>
    /// 1~5
    /// </summary>
    public void SetStrength(int strength)
    {

        if (battleType != BattleType.GeZi)
        {
            return;
        }
        if (strength >= Icons.Length)
        {
            return;
        }

        for (int i = 0; i < Icons.Length; i++)
        {
            if (i < strength)
            {
                Icons[i].SetActive(true);
            }
            else
            {
                Icons[i].SetActive(false);
            }
        }
    }

    public void SetPrecent(float val)
    {

        if (battleType == BattleType.Precent)
        {
            this.battleSlider.value = val;
        }


    }

    void Update()
    {
        if ((Time.time - startTime) >= 30)
        {
            startTime = Time.time;
            //
#if UNITY_ANDROID && !UNITY_EDITOR
            SetBattery();
#endif

        }
    }

    private void SetBattery()
    {
        float sp = GetBatteryValue() / 100f;
        SetPrecent(sp);
    }

    private int GetBatteryValue()
    {
#if UNITY_ANDROID
        AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
        SQDebug.Log("woriktr=");
        string level = jo.Call<string>("ShowToast", "Showing on Toast");
        //  NGUIDebug.Log("电池===" + level);
        return int.Parse(level);
#else
        return 1;
#endif
    }

    int batteryPercent
    {
        get
        {
            try
            {
                string CapacityString = System.IO.File.ReadAllText("/sys/class/power_supply/battery/capacity");
                return int.Parse(CapacityString);
            }
            catch (Exception e)
            {
                //SQDebug.Log("Failed to read battery power; " + e.Message);
            }
            return -1;
        }
    }
}
