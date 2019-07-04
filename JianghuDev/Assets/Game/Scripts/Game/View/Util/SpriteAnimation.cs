using UnityEngine;
using System.Collections;

public class SpriteAnimation : MonoBehaviour
{
    public UISprite mSprite;
    public string mSp;
    public int mStartIndex;
    public int mEndIndex;
    public float mDelta = 0.25f;//每张图间隔
    public int mTime = 1;//总的播放几遍

    private float mNowTime;//当前已经过去的时间
    private int mCurIndex;//当前index
    private float mCurNum;//当前次数
    public bool mIsStart = true;//是否开始

    /// <summary>
    /// 开始
    /// </summary>
    /// <param name="sp">图片名称</param>
    /// <param name="startIndex">开始index</param>
    /// <param name="endIndex">结束index</param>
    /// <param name="time">循环次数</param>
    /// <param name="delta">每张图片播放间隔</param>
    public void SetBegin(string sp, int startIndex, int endIndex, int time = 1, float delta = 0.25f)
    {
        gameObject.SetActive(true);

        mSp = sp;
        mStartIndex = startIndex;
        mEndIndex = endIndex;
        mTime = time;
        mDelta = delta;
        mCurIndex = mStartIndex;
        mCurNum = 0;
        mNowTime = 0;
        mSprite.spriteName = mSp + mCurIndex;
        mIsStart = true;
    }

    /// <summary>
    /// 延迟销毁
    /// </summary>
    /// <param name="time"></param>
    public void SetDalayDestory(float time) {
        Destroy(gameObject, time);
    }


    // Use this for initialization
    void Start()
    {
        mCurIndex = mStartIndex;
        mCurNum = 0;
        mNowTime = 0;
        if (!string.IsNullOrEmpty(mSp))
            mSprite.spriteName = mSp + mCurIndex;
    }

    // Update is called once per frame
    void Update()
    {
        if (mIsStart)
        {
            mNowTime += Time.deltaTime;
            if (mNowTime >= mDelta)
            {
                if (mCurNum >= mTime)
                {
                    mIsStart = false;
                    gameObject.SetActive(false);
                    return;
                }
                mCurIndex++;

                if (mCurIndex > mEndIndex)
                {
                    mCurIndex = mStartIndex;
                    mCurNum += 1;
                }

                if (mCurNum >= mTime)
                {
                    mIsStart = false;
                    gameObject.SetActive(false);
                    return;
                }

                mSprite.spriteName = mSp + mCurIndex;

                if (mCurIndex > mEndIndex)
                {
                    mCurIndex = mStartIndex;
                    mCurNum += 1;
                }

                mNowTime = 0;
            }
        }
    }
}
