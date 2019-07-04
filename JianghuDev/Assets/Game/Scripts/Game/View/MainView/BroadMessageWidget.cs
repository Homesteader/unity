using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BroadMessageWidget : BaseViewWidget
{
    public UILabel mText;//广播
    private Transform mTextTran;//广播的transform

    private List<string> mMsgData;//数据
    private int mIndex;//当前播放的广播
    private int mAllCount;//广播数量
    private const float WIDTH = 971f;//宽度
    private float mDestinationX;//目标地址x坐标
    private const float MOVE_SPEED = 100;//移动速度
    private Vector3 mNextPos;//下一次移动的位置

    protected override void Awake()
    {
        base.Awake();
        mTextTran = mText.transform;
    }

    /// <summary>
    /// 设置数据
    /// </summary>
    /// <param name="data">消息数据，空就是隐藏消息</param>
    public void SetData(List<string> data)
    {
        mMsgData = data;
        if(mMsgData == null || data.Count == 0)
        {
            mAllCount = 0;
            gameObject.SetActive(false);
            return;
        }
        gameObject.SetActive(true);
        mIndex = 0;
        mAllCount = data.Count;
        SetDestinationX(mMsgData[mIndex]);
    }

    /// <summary>
    /// 添加一条数据
    /// </summary>
    /// <param name="msg"></param>
    public void AddMsgData(List<string> msg)
    {
        gameObject.SetActive(true);
        bool hadData = true;
        if (mMsgData == null)
        {
            mMsgData = new List<string>();
            hadData = false;
        }
        mMsgData.AddRange(msg);
        mAllCount = mMsgData.Count;
        if(!hadData)//如果之前没有数据，就开始播放，有数据就不管
        {
            mIndex = 0;
            SetDestinationX(mMsgData[mIndex]);
        }
    }

    void Update()
    {
        if(mTextTran.localPosition.x <= mDestinationX)
        {
            ChangeNextMessage();
        }
        else
        {
            //移动
            mNextPos.x -= (Time.deltaTime * MOVE_SPEED);
            mTextTran.localPosition = mNextPos;
        }
    }

    /// <summary>
    /// 设置下一条数据
    /// </summary>
    private void ChangeNextMessage()
    {
        mIndex += 1;
        mAllCount = mMsgData == null ? 0 : mMsgData.Count;
        if (mAllCount == 0)
            return;
        if (mIndex >= mAllCount)
            mIndex = 0;
        SetDestinationX(mMsgData[mIndex]);
    }

    /// <summary>
    /// 设置目标位置
    /// </summary>
    /// <param name="msg"></param>
    /// <returns></returns>
    private void SetDestinationX(string msg)
    {
        if (string.IsNullOrEmpty(msg))
            return;
        mText.text = msg;
        float startPosX = WIDTH / 2 + 50f;
        mTextTran.localPosition = new Vector3(startPosX, 0, 0);//设置广播的初始位置
        mDestinationX = -mText.width - startPosX;
        mNextPos.x = startPosX;
    }
}
