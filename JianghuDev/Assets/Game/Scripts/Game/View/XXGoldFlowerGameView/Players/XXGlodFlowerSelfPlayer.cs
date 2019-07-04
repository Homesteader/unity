using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
//using YunvaIM;

public class XXGlodFlowerSelfPlayer : MonoBehaviour
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
    /// 亮牌按钮
    /// </summary>
    public UIButton mShowCardBtn;

    public UIButton mStartBtn;//开始游戏按钮

    /// <summary>
    /// 准备和换桌的排列
    /// </summary>
    public UIGrid mReadDeskGrid;

    

    /// <summary>
    /// 看牌按钮
    /// </summary>
    public UIButton mLookCardBtn;

    /// <summary>
    /// 弃牌按钮
    /// </summary>
    public UIButton mDisCardBtn;

    /// <summary>
    /// 比牌按钮
    /// </summary>
    public UIButton mCompareCardBtn;

    /// <summary>
    /// 跟注按钮
    /// </summary>
    public UIButton mFollowBtn;

    /// <summary>
    /// 全压
    /// </summary>
    public UIButton mAllInBtn;

    /// <summary>
    /// 孤注一掷
    /// </summary>
    public UIButton mJustFuckBtn;

    /// <summary>
    /// //加注按钮
    /// </summary>
    public UIButton mJiazhuBtn;

    /// <summary>
    /// 下注的Item
    /// </summary>
    public UIButton mOptBtnItem;

    /// <summary>
    /// 下注的按钮的排列
    /// </summary>
    public UIGrid mOptBtnGrid;

    /// <summary>
    /// 下注的背景
    /// </summary>
    public UISprite mOptBtnBg;

    /// <summary>
    /// 语音记录界面
    /// </summary>
    public GameObject mVoceRecordShow;

    /// <summary>
    /// 自动跟注的按钮
    /// </summary>
    public GameObject mAutoGenToggleShow;

    public UIButton mAutoGenBtn;//跟到底按鈕

    /// <summary>
    /// 搓牌按钮
    /// </summary>
    public GameObject mCuoCardBtn;

    /// <summary>
    /// 还剩下的时间
    /// </summary>
    private int mLastTime = 0;

    /// <summary>
    /// 控制器
    /// </summary>
    private XXGoldFlowerGameController mCtr;

    /// <summary>
    /// 更多功能界面
    /// </summary>
    private GameMoreFunctionWidget mFunctionWidget;

    /// <summary>
    /// 房间信息界面
    /// </summary>
    private GameRoomInfoWidget mRoomInfoWidget;

    /// <summary>
    /// 加注列表的集合
    /// </summary>
    private List<UIButton> mAddBetBtnList = new List<UIButton>();

    /// <summary>
    /// 下注金币的spriteName
    /// </summary>
    private List<string> mCoinSpriteName = new List<string> { "btn_2", "btn_2", "btn_2", "btn_2", "btn_2" };

    #region Unity函数

    void Start()
    {
        mCtr = Global.Inst.GetController<XXGoldFlowerGameController>();
    }

    #endregion


    #region 按钮状态

    /// <summary>
    /// 左侧六个按钮
    /// </summary>
    public void HideSixBtnState()
    {
        mLookCardBtn.gameObject.SetActive(false);
        mFollowBtn.isEnabled = false;
        mDisCardBtn.isEnabled = false;
        mCompareCardBtn.isEnabled = false;
        mAutoGenBtn.isEnabled = false;
        mAllInBtn.gameObject.SetActive(false);
        mJustFuckBtn.gameObject.SetActive(false);
        mJiazhuBtn.isEnabled = false;
        SetAddBtnItemState(false);
    }
    

    /// <summary>
    /// 设置准备按钮状态
    /// </summary>
    /// <param name="show"></param>
    public void SetReadybtnState(bool show, string contetn = "准  备")
    {
        //mReadLabel.text = contetn;
        //mReadyBtn.gameObject.SetActive(show);
        //mReadDeskGrid.Reposition();
    }

    /// <summary>
    /// 设置换桌按钮
    /// </summary>
    /// <param name="show"></param>
    public void SetChangDeskBtnState(bool show)
    {
        //mChangDeskBtn.gameObject.SetActive(show);
        //mReadDeskGrid.Reposition();
    }

    //设置开始游戏按钮显示
    public void SetStartGameBtn(bool show)
    {
        mStartBtn.gameObject.SetActive(show);
        mReadDeskGrid.Reposition();
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
    /// 设置亮牌按钮
    /// </summary>
    /// <param name="show"></param>
    public void SetShowCardBtnState(bool show)
    {
        //mShowCardBtn.gameObject.SetActive(show);
        //mReadDeskGrid.Reposition();
    }

    /// <summary>
    /// 设置看牌按钮
    /// </summary>
    /// <param name="enable"></param>
    public void SetLookCardBtnState(bool show)
    {
        SQDebug.Log("look：" + show);
        mLookCardBtn.gameObject.SetActive(show);
        //SetBtnState(show, mLookCardBtn);
    }

    /// <summary>
    /// 设置跟注按钮
    /// </summary>
    /// <param name="enable"></param>
    public void SetFollowBtnState(bool show)
    {
        SetBtnState(show, mFollowBtn);
    }

    //设置跟到底按钮
    public void SetAutoFollowState(bool show)
    {
        SetBtnState(show, mAutoGenBtn);
    }

    /// <summary>
    /// 设置加注按钮
    /// </summary>
    /// <param name="show"></param>
    public void SetJiazhuBtnState(bool show)
    {
        SetBtnState(show, mJiazhuBtn);
    }

    /// <summary>
    /// 设置弃牌
    /// </summary>
    /// <param name="enable"></param>
    public void SetDisCardBtnState(bool show)
    {
        SetBtnState(show, mDisCardBtn);
            
    }

    /// <summary>
    /// 比牌按钮
    /// </summary>
    /// <param name="enable"></param>
    public void SetCompareCardBtnState(bool show)
    {
        SetBtnState(show, mCompareCardBtn);
    }

    /// <summary>
    /// 设置全压按钮状态
    /// </summary>
    /// <param name="show"></param>
    public void SetAllInBtnState(bool show)
    {
        mAllInBtn.gameObject.SetActive(show);
    }

    /// <summary>
    /// 设置孤注一掷
    /// </summary>
    public void SetJustFuckBtnState(bool show)
    {
        mJustFuckBtn.gameObject.SetActive(show);
    }

    /// <summary>
    /// 设置自动跟注的状态
    /// </summary>
    /// <param name="value"></param>
    public void SetAutoBtnState(bool value)
    {
        mAutoGenToggleShow.gameObject.SetActive(value);
    }

    /// <summary>
    /// 设置搓牌按钮状态
    /// </summary>
    /// <param name="show"></param>
    public void SetCuoCardBtnState(bool show)
    {
        return;
        //mCuoCardBtn.gameObject.SetActive(show);
        if (show && XXGoldFlowerGameModel.Inst.mCanLookCard && XXGoldFlowerGameModel.Inst.mHasCardSeatList.Contains(XXGoldFlowerGameModel.Inst.mMySeatId))
        {
            //mCuoCardBtn.gameObject.SetActive(show);
            SetAddBtnItemShowWhenActive();
        }
        else
        {
            //mCuoCardBtn.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 设置按钮状态
    /// </summary>
    /// <param name="show"></param>
    /// <param name="btn"></param>
    private void SetBtnState(bool show, UIButton btn)
    {
        if (btn.isEnabled == show) return;
        btn.gameObject.GetComponent<BoxCollider>().enabled = show;
        if (show)
        {
            btn.SetState(UIButtonColor.State.Normal, false);
        }
        else
        {
            btn.SetState(UIButtonColor.State.Disabled, false);
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

    #endregion


    #region 初始化加注列表

    /// <summary>
    /// 设置加注items的显示和隐藏
    /// </summary>
    /// <param name="show"></param>
    public void SetAddBtnItemState(bool show)
    {
        mOptBtnBg.gameObject.SetActive(show);
    }

    /// <summary>
    /// 看牌后如果已经显示加注内容，修改加注数量
    /// </summary>
    public void SetAddBtnItemShowWhenActive()
    {
        if (mOptBtnGrid.gameObject.activeInHierarchy)
        {
            if (XXGoldFlowerGameModel.Inst.mLookCard)
            {
                InitAddList(XXGoldFlowerGameModel.Inst.mStartInfo.lookRate);
            }
            else
            {
                InitAddList(XXGoldFlowerGameModel.Inst.mStartInfo.menRate);
            }
            ShowAddBtnList(XXGoldFlowerGameModel.Inst.mOpt.jiazhuList);
        }
    }
    //设置加倍root显示
    public void SetAddGridShow(bool show)
    {
        mOptBtnGrid.gameObject.SetActive(show);
    }

    /// <summary>
    /// 初始化加注列表
    /// </summary>
    /// <param name="list"></param>
    public void InitAddList(List<float> list)
    {
        NGUITools.DestroyChildren(mOptBtnGrid.transform);
        mAddBetBtnList.Clear();
        for (int i = 1; i < list.Count; i++)
        {
            GameObject go = Assets.InstantiateChild(mOptBtnGrid.gameObject, mOptBtnItem.gameObject);
            go.gameObject.SetActive(true);
            UIButton btn = go.GetComponent<UIButton>();
            go.name = (i + 1) + "";
            go.GetComponentInChildren<UILabel>().text = list[i] + "";
            mAddBetBtnList.Add(btn);
            go.GetComponentInChildren<UISprite>().spriteName = mCoinSpriteName[i - 1];
        }
        mOptBtnGrid.gameObject.SetActive(true);
        mOptBtnGrid.Reposition();
        mOptBtnBg.width = (int)NGUIMath.CalculateRelativeWidgetBounds(mOptBtnGrid.transform).size.x + 30;
    }

    /// <summary>
    /// 显示加注的Items
    /// </summary>
    /// <param name="list"></param>
    public void ShowAddBtnList(List<int> list)
    {
        SetAddBtnItemState(true);
        for (int i = 0; i < mAddBetBtnList.Count; i++)
        {
            mAddBetBtnList[i].GetComponent<BoxCollider>().enabled = false;
            mAddBetBtnList[i].SetState(UIButtonColor.State.Disabled, true);
        }
        for (int i = 0; i < list.Count; i++)
        {
            mAddBetBtnList[list[i] - 2].GetComponent<BoxCollider>().enabled = true;
            mAddBetBtnList[list[i] - 2].SetState(UIButtonColor.State.Normal, true);
        }
        mOptBtnGrid.Reposition();
    }

    #endregion


    #region 按钮点击事件

    /// <summary>
    /// 下注的点击
    /// </summary>
    /// <param name="go"></param>
    public void OnAddBetBtnItemClick(GameObject go)
    {
        SendGoldFlowerOpt req = new SendGoldFlowerOpt();
        int add = int.Parse(go.name);
        req.ins = (int)eGFOptIns.Add;
        req.bet = add;
        Global.Inst.GetController<XXGoldFlowerGameController>().SendOpt(req);
    }

    /// <summary>
    /// 操作点击
    /// </summary>
    public void OnOptBtnClick(GameObject indexGo)
    {
        SendGoldFlowerOpt req = new SendGoldFlowerOpt();
        int index = int.Parse(indexGo.name);
        eGFOptIns optIns = (eGFOptIns)index;
        if (optIns == eGFOptIns.Add)//加注按钮
        {
            if (XXGoldFlowerGameModel.Inst.mLookCard)
            {
                InitAddList(XXGoldFlowerGameModel.Inst.mStartInfo.lookRate);
            }
            else
            {
                InitAddList(XXGoldFlowerGameModel.Inst.mStartInfo.menRate);
            }
            ShowAddBtnList(XXGoldFlowerGameModel.Inst.mOpt.jiazhuList);
        }
        else if (optIns == eGFOptIns.Compare)//比牌
        {
            Global.Inst.GetController<CommonTipsController>().ShowTips("是否确定要比牌？", "比牌|取消", () =>
            {
                XXGoldFlowerGameModel.Inst.mComparingCard = true;
                if (XXGoldFlowerGameModel.Inst.mHasCardSeatList.Count == 2)
                {
                    int temp = 1;
                    for (int i = 0; i < XXGoldFlowerGameModel.Inst.mHasCardSeatList.Count; i++)
                    {
                        if (XXGoldFlowerGameModel.Inst.mHasCardSeatList[i] != XXGoldFlowerGameModel.Inst.mMySeatId)
                        {
                            temp = XXGoldFlowerGameModel.Inst.mHasCardSeatList[i];
                        }
                    }
                    SendGoldFlowerOpt teme = new SendGoldFlowerOpt();
                    teme.ins = (int)eGFOptIns.Compare;
                    teme.otherSeatId = temp;
                    Global.Inst.GetController<XXGoldFlowerGameController>().SendOpt(teme, () =>
                    {
                        XXGoldFlowerGameModel.Inst.mComparingCard = false;
                    });
                }
                else
                {
                    Global.Inst.GetController<XXGoldFlowerGameController>().mView.SetShowCompareSelectSp(true);
                }
            });
        }else if(optIns == eGFOptIns.DisCard)//弃牌提示
        {
            Global.Inst.GetController<CommonTipsController>().ShowTips("是否确定要弃牌？", "确定|取消", () =>
            {
                req.ins = index;
                Global.Inst.GetController<XXGoldFlowerGameController>().SendOpt(req);
            });
        }
        else
        {
            req.ins = index;
            Global.Inst.GetController<XXGoldFlowerGameController>().SendOpt(req);
        }

    }

    //开始游戏
    public void OnStartClick()
    {
        Global.Inst.GetController<XXGoldFlowerGameController>().StartGame();
    }


    /// <summary>
    /// 准备点击
    /// </summary>
    public void OnReadyClick()
    {
        Global.Inst.GetController<XXGoldFlowerGameController>().SendReady();
    }

    /// <summary>
    /// 查看房间信息点击
    /// </summary>
    public void OnRoomInfoBtnClick()
    {
        string rule = GamePatternModel.Inst.DeserializeRuleJosn(XXGoldFlowerGameModel.Inst.mRoomRules, true, XXGoldFlowerGameModel.Inst.mGoldPattern);
        if (mRoomInfoWidget == null)
        {
            mRoomInfoWidget = BaseView.GetWidget<GameRoomInfoWidget>(AssetsPathDic.GameRoomInfoWidget, Global.Inst.GetController<XXGoldFlowerGameController>().mView.transform);
        }
        mRoomInfoWidget.ShowContent(rule);
    }

    /// <summary>
    /// 更多按钮点击
    /// </summary>
    public void OnMoreBtnClick()
    {
        if (mFunctionWidget == null)
        {
            mFunctionWidget = BaseView.GetWidget<GameMoreFunctionWidget>(AssetsPathDic.GameMoreFunctionWidget, mCtr.mView.transform);
        }
        mFunctionWidget.SetBtnItems(eGameMore.Setting, eGameMore.Distance, eGameMore.Leave);
        mFunctionWidget.SetBtnCallBack(OnSettingClick, OnShowDisClick, OnLeaveClick);
    }

    /// <summary>
    /// 设置按钮点击
    /// </summary>
    public void OnSettingClick()
    {
        SettingWidget widget = BaseView.GetWidget<SettingWidget>("MainView/SettingWidget", mCtr.mView.transform);
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
            Global.Inst.GetController<XXGoldFlowerGameController>().SendLeaveRoom();
        }, null, "提示");
    }

    /// <summary>
    /// 显示距离
    /// </summary>
    public void OnShowDisClick()
    {
        Global.Inst.GetController<CommonTipsController>().ShowTips("确定要弃牌？", "确定|取消", () =>
        {
            Global.Inst.GetController<XXGoldFlowerGameController>().SendGetPlayerDistances();
        });
        
    }

    /// <summary>
    /// 换桌点击
    /// </summary>
    public void OnChangDeskClick()
    {
        Global.Inst.GetController<XXGoldFlowerGameController>().SendChangGoldPattern();
        SetChanagDeskBtnState(false);
        SetReadyBtnState(false);
    }

    /// <summary>
    /// 邀请按钮点击
    /// </summary>
    public void OnInvitateBtnClick()
    {
        //string rule = GamePatternModel.Inst.DeserializeRuleJosn(XXGoldFlowerGameModel.Inst.mRoomRules, false);
        //SQDebug.Log(rule);
        SixqinSDKManager.Inst.InviteFriends(XXGoldFlowerGameModel.Inst.mRoomId, "", cn.sharesdk.unity3d.PlatformType.WeChat);
    }

    /// <summary>
    /// 亮牌按钮点击
    /// </summary>
    public void OnShowCardClick()
    {
        Global.Inst.GetController<XXGoldFlowerGameController>().SendShowCard();
    }

    /// <summary>
    /// 自动跟注点击
    /// </summary>
    public void OnAutoGenClick()
    {
        Global.Inst.GetController<XXGoldFlowerGameController>().SendAutoGen(XXGoldFlowerGameModel.Inst.mAutoGen ? 2 : 1, () =>
        {
            SetAutoBtnState(XXGoldFlowerGameModel.Inst.mAutoGen);
        });
    }

    /// <summary>
    /// 加注按钮点击
    /// </summary>
    public void OnJiazhuClick()
    {
        if (XXGoldFlowerGameModel.Inst.mOpt == null)
            return;
        GoldFlowerOpt data = XXGoldFlowerGameModel.Inst.mOpt;
        if (data.jiazhuList != null && data.jiazhuList.Count > 0)
        {
            SetAddBtnItemState(true);
            if (XXGoldFlowerGameModel.Inst.mLookCard)
            {
                InitAddList(XXGoldFlowerGameModel.Inst.mStartInfo.lookRate);
            }
            else
            {
                InitAddList(XXGoldFlowerGameModel.Inst.mStartInfo.menRate);
            }
            ShowAddBtnList(data.jiazhuList);
        }
        else
        {
            SetAddBtnItemState(false);
        }
    }

    


    /// <summary>
    /// 搓牌按钮点击
    /// </summary>
    public void OnCuoCardClick()
    {
        XXGoldFlowerGameModel.Inst.mCuoCarding = true;
        SendGoldFlowerOpt req = new SendGoldFlowerOpt();
        req.ins = (int)(eGFOptIns.LookCard);
        Global.Inst.GetController<XXGoldFlowerGameController>().SendOpt(req);
    }

    /// <summary>
    /// 聊天表情点击
    /// </summary>
    public void OnTxtChatClick()
    {
        GameChatView view = Global.Inst.GetController<GameChatController>().OpenWindow() as GameChatView;
        view.SetData((index) =>
        {
            XXGoldFlowerGameController mCtr = Global.Inst.GetController<XXGoldFlowerGameController>();
            SendReceiveGameChat req = new SendReceiveGameChat();
            req.chatType = (int)eGameChatContentType.Face;
            req.faceIndex = index;
            req.fromSeatId = XXGoldFlowerGameModel.Inst.mMySeatId; ;
            mCtr.SendGameChat(req);
        });
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
              XXGoldFlowerGameController mCtr = Global.Inst.GetController<XXGoldFlowerGameController>();
              SendReceiveGameChat req = new SendReceiveGameChat();
              req.fromSeatId = XXGoldFlowerGameModel.Inst.mMySeatId;
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


}
