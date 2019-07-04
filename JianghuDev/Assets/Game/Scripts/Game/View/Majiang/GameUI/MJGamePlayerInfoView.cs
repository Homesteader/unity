using UnityEngine;
using System.Collections;

public class MJGamePlayerInfoView : MonoBehaviour
{

    public UITexture mIcon;//头像
    public UILabel mName;//名字
    public UILabel mIp;//IP
    public UILabel mId;//ID
    public UILabel mPos;//位置
    public UILabel mDistance;//距离
    public UILabel mCard;//房卡
    public UISprite mSex;//性别
    public GameObject mFaceGrid;
    public UIGrid mDetailGrid;//信息grid

    private int mSeatId;//当前玩家的座位号

    public void SetData(PlayerInfoStruct info, int seatid, bool isself = false)
    {
        if (mFaceGrid != null)
        {
            if (isself)
            {
                mFaceGrid.gameObject.SetActive(false);
            }
            else
            {
                mFaceGrid.gameObject.SetActive(true);
            }
        }
        //头像
        Assets.LoadIcon(info.headUrl, (t) => { mIcon.mainTexture = t; });
        //名字
        mName.text = info.nickName;
        //IP
        //mIp.text = info.lastIp;
        //id
        //mId.text = info.userId.ToString();
        //位置
        //mPos.text = info.addr;
        //性别
        //mSex.spriteName = info.userSex == 1 ? "icon_nan" : "icon_nv";
        //座位号
        mSeatId = seatid;
        //房卡
        #region 房卡
        if (isself)//自己才显示
        {
            mCard.transform.parent.gameObject.SetActive(true);
            //mCard.text = info.roomCard.ToString() + "张";
        }
        else
            mCard.transform.parent.gameObject.SetActive(false);
        #endregion
        //距离
        #region 距离            //****************************位置信息没有数据**********************************************//
        //if (isself)//是自己就不显示
        //    mDistance.transform.parent.gameObject.SetActive(false);
        //else
        //{
        //    mDistance.transform.parent.gameObject.SetActive(true);
        //    if (info.distance == -1)
        //        mDistance.text = "未获取到距离";
        //    else if (info.distance < 1000)
        //        mDistance.text = info.distance + "米";
        //    else
        //        mDistance.text = (info.distance / 1000f).ToString("f2") + "千米";
        //}
        #endregion

        mDetailGrid.Reposition();
    }


    public void OnCloseClick()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 当表情点击
    /// </summary>
    /// <param name="go"></param>
    public void OnFaceClick(GameObject go)
    {


        int index = int.Parse(go.name);

        //MJChatProtoData data = new MJChatProtoData();

        SendReceiveGameChat data = new SendReceiveGameChat();
        data.chatType = 4;
        data.faceIndex = index;
        data.fromSeatId = MJGameModel.Inst.mMySeatId;
        Global.Inst.GetController<MJGameController>().SendChat(data);

        //gameObject.SetActive(false);
    }
}
