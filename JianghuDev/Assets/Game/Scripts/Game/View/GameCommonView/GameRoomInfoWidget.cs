using UnityEngine;
using System.Collections;

public class GameRoomInfoWidget : BaseViewWidget {

    public UILabel mLabel;

    public GameObject mJoinRoomBtn;


    private string mRoomId;

    /// <summary>
    /// 展示内容
    /// </summary>
    /// <param name="content"></param>
    public void ShowContent(string content) {
        mLabel.text = content;
    }

    /// <summary>
    /// 显示加入房间按钮
    /// </summary>
    /// <param name="roomId"></param>
    public void ShowJoinRoomBtn(string roomId) {
        mRoomId = roomId;
        mJoinRoomBtn.gameObject.SetActive(true);
    }

    /// <summary>
    /// 加入房间点击
    /// </summary>
    public void OnJoinRoomClick() {
        switch (GamePatternModel.Inst.mCurGameId)
        {
            case eGameType.MaJiang://麻将
                Global.Inst.GetController<MJGameController>().SendJoinRoom(mRoomId);
                break;
            case eGameType.GoldFlower://金花
                Global.Inst.GetController<XXGoldFlowerGameController>().SendJoinRoomReq(mRoomId);
                break;
            case eGameType.NiuNiu://牛牛
                Global.Inst.GetController<NNGameController>().SendJoinRoomReq(mRoomId);
                break;
        }
    }

    /// <summary>
    /// 关闭按钮点击
    /// </summary>
    public void OnCloseClick() {
        Close<GameRoomInfoWidget>();
    }
}
