using UnityEngine;
using System.Collections;

public class MJGameSetingUI : BaseViewWidget
{

    public UISlider mMusic;//音乐
    public UISlider mVoice;//音效
    public UIButton mApplyDissolutionBtn;//申请解散房间按钮
    public UIButton mDissolutionBtn;//解散房间按钮
    public UIButton mLeaveBtn;//离开房间按钮
    public UIGrid mBtnGrid;//按钮grid
    private float mMusicValue;//音乐声音大小
    private float mVoiceValue;//音效声音大小

    private bool mIsMg;//是否是房主

    /// <summary>
    /// 设置数据
    /// </summary>
    /// <param name="isMg">是否是房主</param>
    public void SetData(bool isMg)
    {
        mApplyDissolutionBtn.gameObject.SetActive(false);
        mDissolutionBtn.gameObject.SetActive(false);
        mLeaveBtn.gameObject.SetActive(false);

        mMusic.value = SoundProcess.m_fMusicVolume;
        mVoice.value = SoundProcess.m_fSoundVolume;
        mMusicValue = SoundProcess.m_fMusicVolume;
        mVoiceValue = SoundProcess.m_fSoundVolume;

        /****  只有申请解散房间*******/
        mApplyDissolutionBtn.gameObject.SetActive(true);
        mBtnGrid.Reposition();
        return;
        /*********************************************************************************************************/
        #region 原有代码
        if (MJGameModel.Inst.mRoomType == 3 && MJGameModel.Inst.mState == eMJRoomStatus.READY)
        {//群主房间,并且在准备阶段
            mLeaveBtn.gameObject.SetActive(true);
        }
        if (MJGameModel.Inst.mRoomType == 3 && MJGameModel.Inst.mState != eMJRoomStatus.READY)
        {//群主房间，并且没有在准备阶段
            mApplyDissolutionBtn.gameObject.SetActive(true);
        }

        if (MJGameModel.Inst.mRoomType != 3 && MJGameModel.Inst.mState == eMJRoomStatus.READY)
        {//不是群主房间，在准备阶段
            if (isMg == true)
            {//是房主
                mDissolutionBtn.gameObject.SetActive(true);
            }
            else
            {
                mLeaveBtn.gameObject.SetActive(true);
            }
        }
        if (MJGameModel.Inst.mRoomType != 3 && MJGameModel.Inst.mState != eMJRoomStatus.READY)
        {//不是群主房间，也没在准备阶段
            mApplyDissolutionBtn.gameObject.SetActive(true);
        }

        //mLeaveBtn.gameObject.SetActive(!isMg);//是房主要置回离开按钮
        //mDissolutionBtn.gameObject.SetActive(isMg);//是房主要显示解散按钮
        //mApplyDissolutionBtn.gameObject.SetActive(!isMg);//申请解散按钮

        //eRoomStatus state = GameModel.Inst.mState;
        if (MJGameModel.Inst.mCurPlayCount > 1)//开始比赛后就不能离开
        {
            mLeaveBtn.gameObject.SetActive(false);
            mDissolutionBtn.gameObject.SetActive(false);
            mApplyDissolutionBtn.gameObject.SetActive(true);
        }
        mBtnGrid.Reposition(); 
        #endregion
    }

    public void OnMusicChange(UISlider slider)
    {
        SoundProcess.SetMusicVolume(slider.value);
        //GameModel.Inst.mLastMusicVolume = SoundProcess.MusicVolume;
    }

    public void OnVoiceChange(UISlider slider)
    {
        SoundProcess.SetEffectVolume(slider.value);
        //GameModel.Inst.mLastSoundVolume = SoundProcess.SoundVolume;
    }


    /// <summary>
    /// 点击申请解散
    /// </summary>
    public void OnApplyDissolutionClick()
    {
        Global.Inst.GetController<MJGameController>().SendDissolution(() =>
        {

        });
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 点击解散房间
    /// </summary>
    public void OnDissolutionClick()
    {
        Global.Inst.GetController<CommonTipsController>().ShowTips("确定要解散房间？", "确定|取消", () =>
        {
            Global.Inst.GetController<MJGameController>().SendDissolution(() =>
            {

            });
        }, null);
    }

    /// <summary>
    /// 离开房间
    /// </summary>
    public void OnLeaveClick()
    {
        Global.Inst.GetController<CommonTipsController>().ShowTips("确定要离开房间？", "确定|取消", () =>
        {
            Global.Inst.GetController<MJGameController>().SendLeaveRoom(() =>
            {
                NetProcess.InitNetWork(GameManager.Instance.Ip, GameManager.Instance.port);
                Global.Inst.GetController<MJGameController>().ConnectedToHallServer(null);
            });

        }, null);
    }

    public void OnCloseClick()
    {
        gameObject.SetActive(false);
    }

    void OnDisable()
    {
        if (mMusicValue - mMusic.value > 0.01f || mMusicValue - mMusic.value < -0.01f)//有改变
            MJGameModel.Inst.mLastMusicVolume = SoundProcess.MusicVolume;
        if (mVoiceValue - mVoice.value > 0.01f || mVoiceValue - mVoice.value < -0.01f)
            MJGameModel.Inst.mLastSoundVolume = SoundProcess.SoundVolume;
    }
}
