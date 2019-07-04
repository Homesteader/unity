using UnityEngine;
using System.Collections;

public class SettingWidget : BaseViewWidget
{
    public UISlider mMusic;//音乐
    public UISlider mVoice;//音效
    public GameObject mLoginOutBtn;//退出登录按钮

    protected override void Start()
    {
        base.Start();
        //初始化音量
        float v = SoundProcess.MusicVolume;
        mMusic.value = v;
        v = SoundProcess.SoundVolume;
        mVoice.value = v;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
    }


    /// <summary>
    /// 设置数据并显示
    /// </summary>
    /// <param name="isShowBtn">是否显示按钮</param>
    public void SetData(bool isShowBtn)
    {
        mLoginOutBtn.SetActive(isShowBtn);
        
    }


    #region 按钮点击
    /// <summary>
    /// 关闭音乐按钮点击
    /// </summary>
    public void OnStopMusicClick()
    {
        mMusic.value = 0;
    }


    /// <summary>
    /// 关闭音效按钮点击
    /// </summary>
    public void OnStopVoiceClick()
    {
        mVoice.value = 0;
    }

    /// <summary>
    /// 关闭按钮点击
    /// </summary>
    public void OnCloseClick()
    {
        Close<SettingWidget>();
    }

   

    /// <summary>
    /// 退出登录点击
    /// </summary>
    public void OnLoginOutClick()
    {
        Global.Inst.GetController<CommonTipsController>().ShowTips("是否退出登录？", "退出|取消", () =>
        {
            PlayerPrefs.DeleteKey("wechat_login_data");//清除登录信息
            if (Application.platform != RuntimePlatform.WindowsEditor)
                SixqinSDKManager.Inst.CanelLogin(cn.sharesdk.unity3d.PlatformType.WeChat);
            Global.Inst.GetController<LoginController>().LoginOut();
        });
    }
    

    #endregion
    /// <summary>
    /// 音乐改变
    /// </summary>
    public void OnMusicValueChange()
    {
        SoundProcess.SetMusicVolume(mMusic.value);
    }

    /// <summary>
    /// 音效改变
    /// </summary>
    public void OnVoiceValueChange()
    {
        SoundProcess.SetEffectVolume(mVoice.value);
    }
}
