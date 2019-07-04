using UnityEngine;
using System.Collections;

public class MessageItem : MonoBehaviour {


    public UILabel mContent;

    public UILabel mSmallContent;

    public UIButton mAgreeBtn;

    public UILabel mTime;

    public UISprite mTimeSp;

    public UIButton mDisAgreeBtn;

    public UIButton mYiDuBtn;


    private CallBack<int> mAgree;


    private CallBack<int> mDisAgree;

    private CallBack<int> mYiDu;

    private SendGetMessageInfo mData;

    private int mMessageId;

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="messageid"></param>
    /// <param name="clubid"></param>
    /// <param name="name"></param>
    /// <param name="content"></param>
    /// <param name="agree"></param>
    /// <param name="disagree"></param>
    public void InitUI(SendGetMessageInfo data, int messageid, string content,CallBack<int> agree, CallBack<int> disagree, CallBack<int> yidu) {
        mMessageId = messageid;
        mContent.text = content;
        mSmallContent.text = content;
        mData = data;
        if (agree == null) {
            mAgreeBtn.gameObject.SetActive(false);
        }
        if (disagree==null) {
            mDisAgreeBtn.gameObject.SetActive(false);
        }

        if (yidu == null) {
            mYiDuBtn.gameObject.SetActive(false);
        }

        if (data.redState != 2) {//未读
            mYiDuBtn.isEnabled = false;
            GameObject label = mYiDuBtn.transform.GetChild(0).gameObject;
            if (label!=null) {
                label.gameObject.SetActive(false);
            }
        }
        mTime.text = data.time;
        mTimeSp.width = mTime.width + 30;

        mAgree = agree;
        mDisAgree = disagree;
        mYiDu = yidu;
    }


    public void OnAgreeClick() {
        if (mAgree!=null) {
            mAgree(mMessageId);
        }
    }


    public void OnDisAgreeClick() {
        if (mDisAgree != null)
        {
            mDisAgree(mMessageId);
        }
    }


    public void OnYiuClick() {
        Global.Inst.GetController<MainController>().SendReadMessage(mData.messageId, () =>
        {
            mYiDuBtn.isEnabled = false;
            mData.redState = 1;
            GameObject label = mYiDuBtn.transform.GetChild(0).gameObject;
            if (label != null)
            {
                label.gameObject.SetActive(false);
            }
        });
    }

}
