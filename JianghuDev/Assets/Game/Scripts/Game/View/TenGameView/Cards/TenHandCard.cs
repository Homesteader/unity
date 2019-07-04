using UnityEngine;
using System.Collections;
using System;

public class TenHandCard : TurnCardAnim
{
    public UISprite mCardBgSpr;//牌背面图片
    public UISprite mCardSpr;//牌正面图片

    private Action<GameObject> mMoveCallBack;

    private Vector3 mOrigin;

    private float mLeft;

    private float mRight;

    private float mTop;

    private float mDown;


    private bool mCalled = false;


    #region 移动

    private void Start()
    {
        mCalled = false;
        mOrigin = transform.localPosition;
    }

    private void Update()
    {
        if (transform.localPosition.x - mOrigin.x > mRight ||
            transform.localPosition.x - mOrigin.x<mLeft ||
            transform.localPosition.y - mOrigin.y>mTop ||
            transform.localPosition.y - mOrigin.y<mDown) {
            if (mCalled == false && mMoveCallBack!=null) {
                mCalled = true;
                mMoveCallBack(gameObject);
            }
        }
    }

    /// <summary>
    /// 移动返回
    /// </summary>
    /// <param name="left">负数</param>
    /// <param name="right">正数</param>
    /// <param name="top">正数</param>
    /// <param name="down">负数</param>
    /// <param name="callBack"></param>
    public void SetMove(float left, float right, float top, float down, Action<GameObject> callBack) {
        mLeft = left;
        mRight = right;
        mTop = top;
        mDown = down;
        mMoveCallBack = callBack;
    }


    #endregion


    /// <summary>
    /// 根据序号设置牌正面深度
    /// </summary>
    /// <param name="index">下标，从0开始</param>
    public new void SetCardDeepsByIndex(int index)
    {
        mCardSpr.depth = index * 2 + 10;//牌正面
        mPaiLabel.depth = mCardSpr.depth + 1;//数字
    }

    /// <summary>
    /// 根据序号设置牌背面深度
    /// </summary>
    /// <param name="index">下标，从0开始</param>
    public new void SetCardBgDeepsByIndex(int index)
    {
        mCardBgSpr.depth = index * 2 + 20;
    }

    #region 翻牌
    /// <summary>
    /// 翻牌
    /// </summary>
    /// <param name="time">时间,翻牌一次所用时间</param>
    /// <param name="forword">翻牌方向 true是从背翻到值 false是从值翻到牌背</param>
    /// <param name="num">次数</param>
    public override void TurnCard(float time, bool forword, int num = 1)
    {
        if(mCardBg.activeInHierarchy)//当牌背显示的时候才翻牌
            base.TurnCard(time, forword, num);
    }
    #endregion

    #region 移动
    /// <summary>
    /// 移动
    /// </summary>
    /// <param name="distance">移动距离</param>
    /// <param name="time">时间</param>
    /// <param name="direction">方向，x,y,z轴</param>
    public void Move(float distance, float time, eDirectionType direction)
    {
        Vector3 deltaPos = Vector3.zero;//移动的距离
        if (direction == eDirectionType.x) deltaPos.x = 1;
        else if (direction == eDirectionType.y) deltaPos.y = 1;
        else deltaPos.z = 1;
        deltaPos = deltaPos * distance;
        //移动的目标位置
        Vector3 destPos = transform.localPosition + deltaPos;

        iTween.MoveTo(gameObject, iTween.Hash("position", destPos, "time", time, "islocal", true, "easetype", iTween.EaseType.linear));
    }

    /// <summary>
    /// 移动
    /// </summary>
    /// <param name="destination">目标位置</param>
    /// <param name="time">时间</param>
    /// <param name="isEnlarge">是否放大</param>
    public void Move(Vector3 destination, float time, bool isEnlarge = false)
    {
        if (!isEnlarge)//不用放大
        {
            iTween.MoveTo(gameObject, iTween.Hash("position", destination, "time", time, "islocal", true, "easetype", iTween.EaseType.linear));
        }
        else
        {
            iTween.MoveTo(gameObject, iTween.Hash("position", destination, "time", time, "islocal", true, "easetype", iTween.EaseType.linear));
            iTween.ScaleTo(gameObject, iTween.Hash("scale", Vector3.one * 2f, "time", time, "islocal", true, "easetype", iTween.EaseType.linear));
        }
            
    }
    #endregion
}
