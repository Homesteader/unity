using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
//using YunvaIM;

public class NiuniuSelfPlayer : MonoBehaviour
{
    
    /// <summary>
    /// 准备按钮
    /// </summary>
    public UIButton mReadyBtn;

    /// <summary>
    /// 准备按钮上的文字
    /// </summary>
    public UILabel mReadLabel;

    /// <summary>
    /// 换桌按钮
    /// </summary>
    public UIButton mChangDeskBtn;

    /// <summary>
    /// 邀请好友按钮
    /// </summary>
    public UIButton mInvateBtn;

    /// <summary>
    /// 准备和换桌的排列
    /// </summary>
    public UIGrid mReadDeskGrid;

    /// <summary>
    /// 看牌按钮
    /// </summary>
    public UIButton mLookCardBtn;

    /// <summary>
    /// 搓牌按钮
    /// </summary>
    public UIButton mCuoCardBtn;

    /// <summary>
    /// 更多按钮
    /// </summary>
    public UIButton mMoreBtn;

    /// <summary>
    /// 看牌/搓牌 排列
    /// </summary>
    public UIGrid mLookCuoGrid;

    /// <summary>
    /// 下注的Item
    /// </summary>
    public UIButton mOptBtnItem;

    /// <summary>
    /// 下注的按钮的排列
    /// </summary>
    public UIGrid mOptBtnGrid;

    /// <summary>
    /// 语音聊天记录动画
    /// </summary>
    public GameObject mVoceRecordShow;

    /// <summary>
    /// 还剩下的时间
    /// </summary>
    private int mLastTime = 0;

    /// <summary>
    /// 控制器
    /// </summary>
    private NNGameController mCtr;

    /// <summary>
    /// 更多功能界面
    /// </summary>
    private GameMoreFunctionWidget mFunctionWidget;

    /// <summary>
    /// 房间信息界面
    /// </summary>
    private GameRoomInfoWidget mRoomInfoWidget;

    /// <summary>
    /// 搓牌界面
    /// </summary>
    private NiuNiuCuoCardWidget mCuoCardWidget;

    /// <summary>
    /// 加注列表的集合
    /// </summary>
    private List<UIButton> mAddBetBtnList = new List<UIButton>();


    #region Unity函数

    void Start()
    {
        mCtr = Global.Inst.GetController<NNGameController>();
    }

    #endregion


    #region 按钮状态

    /// <summary>
    /// 看牌和搓牌显示和隐藏
    /// </summary>
    public void HideLiangCuoState()
    {
        mLookCardBtn.gameObject.SetActive(false);
        mCuoCardBtn.gameObject.SetActive(false);
    }

    /// <summary>
    /// 设置准备按钮状态
    /// </summary>
    /// <param name="show"></param>
    public void SetReadybtnState(bool show, string contetn = "准  备")
    {
        mReadLabel.text = contetn;
        mReadyBtn.gameObject.SetActive(show);
        mReadDeskGrid.Reposition();
    }

    /// <summary>
    /// 设置换桌按钮
    /// </summary>
    /// <param name="show"></param>
    public void SetChangDeskBtnState(bool show)
    {
        mChangDeskBtn.gameObject.SetActive(show);
        mReadDeskGrid.Reposition();
    }


    /// <summary>
    /// 设置按钮状态
    /// </summary>
    /// <param name="show"></param>
    /// <param name="btn"></param>
    private void SetBtnState(bool show, UIButton btn)
    {
        btn.gameObject.GetComponent<BoxCollider>().enabled = show;
        if (show)
        {
            btn.SetState(UIButtonColor.State.Normal, true);
        }
        else
        {
            btn.SetState(UIButtonColor.State.Disabled, true);
        }
    }

    /// <summary>
    /// 设置换桌状态
    /// </summary>
    /// <param name="enabel"></param>
    public void SetChanagDeskBtnState(bool enabel)
    {
        SetBtnState(enabel, mChangDeskBtn);
    }

    public void SetReadyBtnState(bool enabel)
    {
        SetBtnState(enabel, mReadyBtn);
    }

    /// <summary>
    /// 设置邀请好友按钮状态
    /// </summary>
    /// <param name="show"></param>
    public void SetInvateBtnState(bool show)
    {
        mInvateBtn.gameObject.SetActive(show);
        mReadDeskGrid.Reposition();
    }
    

    /// <summary>
    /// 设置看牌按钮
    /// </summary>
    /// <param name="enable"></param>
    public void SetLiangCardBtnState(bool show)
    {
        mLookCardBtn.gameObject.SetActive(show);
        mLookCuoGrid.Reposition();
        if (NiuniuModel.Inst.mLookCard) {
            UILabel label = mLookCardBtn.GetComponentInChildren<UILabel>();
            if (label!=null) {
                label.text = "停牌";
            }
        }
        else {
            UILabel label = mLookCardBtn.GetComponentInChildren<UILabel>();
            if (label != null)
            {
                label.text = "要牌";
            }
        }
    }

    /// <summary>
    /// 设置搓牌
    /// </summary>
    /// <param name="enable"></param>
    public void SetCuoBtnState(bool show)
    {
        mCuoCardBtn.gameObject.SetActive(show);
        mLookCuoGrid.Reposition();
    }

    public void CloseCuoCardWidget() {
        BaseViewWidget.CloseWidget<NiuNiuCuoCardWidget>();
        mCuoCardWidget = null;
    }

    #endregion


    #region 初始化下注列表

    /// <summary>
    /// 设置下注items的显示和隐藏
    /// </summary>
    /// <param name="show"></param>
    public void SetBetBtnItemState(bool show)
    {
        mOptBtnGrid.gameObject.SetActive(show);
    }

    /// <summary>
    /// 初始化下注和抢庄列表
    /// </summary>
    /// <param name="list"></param>
    public void InitOptItemList<T>(List<T> list)
    {
        NGUITools.DestroyChildren(mOptBtnGrid.transform);
        mAddBetBtnList.Clear();
        for (int i = 0; i < list.Count; i++)
        {
            GameObject go = Assets.InstantiateChild(mOptBtnGrid.gameObject, mOptBtnItem.gameObject);
            go.gameObject.SetActive(true);
            UIButton btn = go.GetComponent<UIButton>();
            if (typeof(T).Name == typeof(int).Name)//抢庄
            {
                go.name = (i + 1) + "Q";
                go.GetComponentInChildren<UILabel>().text = "抢" + list[i] + "";
                if (int.Parse(list[i].ToString())==0) {
                    go.GetComponent<UISprite>().spriteName = "showcard_btnbg";
                    go.GetComponentInChildren<UILabel>().text = "不抢";
                }
            }
            else {//下注
                go.name = (i + 1) + "X";
                go.GetComponentInChildren<UILabel>().text = list[i] + "";
            }

            mAddBetBtnList.Add(btn);
        }
        mOptBtnGrid.Reposition();
    }

    /// <summary>
    /// 显示下注的Items
    /// </summary>
    /// <param name="list"></param>
    public void ShowBetBtnList(List<int> list)
    {
        SetBetBtnItemState(true);
        for (int i = 0; i < mAddBetBtnList.Count; i++)
        {
            mAddBetBtnList[i].GetComponent<BoxCollider>().enabled = false;
            mAddBetBtnList[i].SetState(UIButtonColor.State.Disabled,true);
        }
        for (int i = 0; i < list.Count; i++)
        {
            mAddBetBtnList[list[i] - 1].GetComponent<BoxCollider>().enabled = true;
            mAddBetBtnList[list[i] - 1].SetState(UIButtonColor.State.Normal, true);
        }
        mOptBtnGrid.Reposition();
    }

    #endregion


    #region 按钮点击事件

    /// <summary>
    /// 下注和抢庄的点击
    /// </summary>
    /// <param name="go"></param>
    public void OnBetQiangBtnItemClick(GameObject go)
    {
        NNSendGameOpt req = new NNSendGameOpt();
        int index = int.Parse(go.name.Remove(go.name.Length - 1, 1));
        if (go.name.EndsWith("Q"))
        {//抢庄
            req.ins = (int)eNNOpt.QZ;
            req.qzValue = NiuniuModel.Inst.mQzListValue[index-1];
        }
        else {//下注
            req.ins = (int)eNNOpt.XZ;
            req.xzValue= NiuniuModel.Inst.mXzListValue[index-1];
        }

        Global.Inst.GetController<NNGameController>().SendGameOpt(req);
    }

    /// <summary>
    /// 亮牌点击
    /// </summary>
    public void OnLiangCardClick()
    {
        if (NiuniuModel.Inst.mLookCard)
        {
            NNSendGameOpt req = new NNSendGameOpt();
            req.ins = (int)eNNOpt.LP;
            Global.Inst.GetController<NNGameController>().SendGameOpt(req);
        }
        else
        {
            SetCuoBtnState(false);
            Global.Inst.GetController<NNGameController>().mView.TurnSelfCards();
            NiuniuModel.Inst.mLookCard = true;
            UILabel label = mLookCardBtn.GetComponentInChildren<UILabel>();
            if (label != null)
            {
                label.text = "亮牌";
            }
        }
    }

    /// <summary>
    /// 搓牌点击
    /// </summary>
    public void OnCuoCardClick() {
        if (mCuoCardWidget == null) {
            mCuoCardWidget = BaseView.GetWidget<NiuNiuCuoCardWidget>(AssetsPathDic.NiuNiuCuoCardWidget, Global.Inst.GetController<NNGameController>().mView.transform);
        }
        List<string> handCards = Global.Inst.GetController<NNGameController>().mView.GetSelfCards();
        List<string> mingCards = new List<string>();
        mingCards.Add(handCards[handCards.Count-1]);

        if (handCards != null)
        {
            switch ((eNNSubGameType)NiuniuModel.Inst.mSubGameId) {
                case eNNSubGameType.MingPai:
                    mCuoCardWidget.ShowCuoCards(true, mingCards, () =>
                    {
                        SQDebug.Log("搓牌结束");
                        SetCuoBtnState(false);
                        Global.Inst.GetController<NNGameController>().mView.TurnSelfCards();
                        NiuniuModel.Inst.mLookCard = true;
                        SetLiangCardBtnState(true);
                    });
                    break;
                case eNNSubGameType.NorMal:
                    mCuoCardWidget.ShowCuoCards(false, handCards, () =>
                    {
                        SQDebug.Log("搓牌结束");
                        SetCuoBtnState(false);
                        Global.Inst.GetController<NNGameController>().mView.TurnSelfCards();
                        NiuniuModel.Inst.mLookCard = true;
                        SetLiangCardBtnState(true);
                    });
                    break;
            }
        }
        else {
            SetCuoBtnState(false);
        }
    }

    /// <summary>
    /// 准备点击
    /// </summary>
    public void OnReadyClick()
    {
        Global.Inst.GetController<NNGameController>().SendReady();
    }

    /// <summary>
    /// 查看房间信息点击
    /// </summary>
    public void OnRoomInfoBtnClick()
    {
        string rule = GamePatternModel.Inst.DeserializeRuleJosn(NiuniuModel.Inst.mRoomRules,true,NiuniuModel.Inst.mGoldPattern);
        if (mRoomInfoWidget == null)
        {
            mRoomInfoWidget = BaseView.GetWidget<GameRoomInfoWidget>(AssetsPathDic.GameRoomInfoWidget, Global.Inst.GetController<NNGameController>().mView.transform);
        }
        mRoomInfoWidget.ShowContent(rule);
    }

    /// <summary>
    /// 聊天表情点击
    /// </summary>
    public void OnTxtChatClick()
    {
        GameChatView view = Global.Inst.GetController<GameChatController>().OpenWindow() as GameChatView;
        view.SetData((index)=> {
            NNGameController mCtr = Global.Inst.GetController<NNGameController>();
            SendReceiveGameChat req = new SendReceiveGameChat();
            req.chatType = (int)eGameChatContentType.TexTVoice;
            req.faceIndex = index;
            req.fromSeatId = NiuniuModel.Inst.mMySeatId; ;
            mCtr.SendGameChat(req);
        });
    }

    /// <summary>
    /// 更多按钮点击
    /// </summary>
    public void OnMoreBtnClick()
    {
        mMoreBtn.GetComponent<UISprite>().spriteName = "btn_more2";
        mMoreBtn.normalSprite = "btn_more2";
        mMoreBtn.GetComponent<UISprite>().MakePixelPerfect();
        if (mFunctionWidget == null)
        {
            mFunctionWidget = BaseView.GetWidget<GameMoreFunctionWidget>(AssetsPathDic.GameMoreFunctionWidget, mCtr.mView.transform);
        }
        mFunctionWidget.SetBtnItems(eGameMore.Setting, eGameMore.Distance, eGameMore.Leave);
        mFunctionWidget.SetBtnCallBack(OnSettingClick, OnShowDisClick, OnLeaveClick);
        mFunctionWidget.SetBtnCloseCall(() =>
        {
            mMoreBtn.GetComponent<UISprite>().spriteName = "btn_more1";
            mMoreBtn.normalSprite = "btn_more1";
            mMoreBtn.GetComponent<UISprite>().MakePixelPerfect();
        });
    }

    /// <summary>
    /// 换桌点击
    /// </summary>
    public void OnChangDeskClick()
    {
        Global.Inst.GetController<NNGameController>().SendChangGoldPattern();
        SetChanagDeskBtnState(false);
        SetReadyBtnState(false);
    }

    /// <summary>
    /// 邀请按钮点击
    /// </summary>
    public void OnInvitateBtnClick() {
        string rule = GamePatternModel.Inst.DeserializeRuleJosn(NiuniuModel.Inst.mRoomRules, false);
        SQDebug.Log(rule);
        SixqinSDKManager.Inst.InviteFriends(NiuniuModel.Inst.mRoomId, rule, cn.sharesdk.unity3d.PlatformType.WeChat);
    }

    /// <summary>
    /// 语音聊天按下
    /// </summary>
    public void OnVoicePressed()
    {
        mVoceRecordShow.gameObject.SetActive(true);
        SpriteAnimation anim = mVoceRecordShow.GetComponentInChildren<SpriteAnimation>();
        anim.SetBegin("yx_yuyin_0", 1, 5, 500000);
        string filePath = string.Format("{0}/{1}.amr", Application.persistentDataPath, DateTime.Now.ToFileTime());
        SQDebug.Log("FilePath:" + filePath);
#if YYVOICE
        YunVaImSDK.instance.RecordStartRequest(filePath, 1);
#endif
    }

    /// <summary>
    /// 语音聊天抬起
    /// </summary>
    public void OnVoiceRelase()
    {
        mVoceRecordShow.gameObject.SetActive(false);
#if YYVOICE
        uint mCurRecordTime = 0;
        YunVaImSDK.instance.RecordStopRequest((data1) =>
        {
            mCurRecordTime = data1.time;
            SQDebug.Log("停止录音返回:" + data1.strfilepath);
        },
          (data2) =>
          {
              SQDebug.Log("上传返回:" + data2.fileurl);
              NNGameController mCtr = Global.Inst.GetController<NNGameController>();
              SendReceiveGameChat req = new SendReceiveGameChat();
              req.fromSeatId = NiuniuModel.Inst.mMySeatId;
              req.chatType = (int)eGameChatContentType.Voice;
              req.content = data2.fileurl;
              req.voiceChatTime = (int) mCurRecordTime;
              mCtr.SendGameChat(req);
          },
          (data3) =>
          {
              SQDebug.Log("识别返回:" + data3.text);
          });
#endif
    }


    #endregion

    #region 准备倒计时

    /// <summary>
    /// 开始准备倒计时
    /// </summary>
    public void StartReadyDownTime()
    {
        if (IsInvoking("DownTime"))
        {
            CancelInvoke("DownTime");
        }

        mLastTime = 30;
        InvokeRepeating("DownTime", 0.01f, 1.0f);
    }


    private void DownTime()
    {
        mLastTime--;
        if (mLastTime >= 0)
        {
            if (mLastTime >= 10)
            {
                mReadLabel.text = "准备(" + mLastTime + ")";
            }
            else
            {
                mReadLabel.text = "准备(0" + mLastTime + ")";
            }
        }
        else
        {
            mReadLabel.text = "准备";
            CancelInvoke("DownTime");
            //todo 发送离开
        }
    }

    #endregion


    #region 更多按钮点击出来的按钮点击回调

    /// <summary>
    /// 设置按钮点击
    /// </summary>
    public void OnSettingClick()
    {
        SettingWidget widget = BaseView.GetWidget<SettingWidget>(AssetsPathDic.SettingWidget, mCtr.mView.transform);
        widget.SetData(false);
    }

    /// <summary>
    /// 离开点击
    /// </summary>
    public void OnLeaveClick()
    {
        Global.Inst.GetController<CommonTipsController>().ShowTips("您确定要离开吗？", "取消|确定", () =>
        {

        }, () =>
        {
            Global.Inst.GetController<NNGameController>().SendLeaveRoom();
        }, null, "提示");
    }

    /// <summary>
    /// 显示距离
    /// </summary>
    public void OnShowDisClick()
    {
        Global.Inst.GetController<NNGameController>().SendGetPlayerDistances();
    }

    #endregion
}
