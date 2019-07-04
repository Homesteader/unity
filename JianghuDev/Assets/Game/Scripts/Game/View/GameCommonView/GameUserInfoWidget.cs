using UnityEngine;
using System.Collections.Generic;

public class GameUserInfoWidget : BaseViewWidget
{

    #region UI

    public GameObject faceItem;

    public UIGrid faceGrid;
    /// <summary>
    /// 头像
    /// </summary>
    public UITexture mHead;

    /// <summary>
    /// 昵称
    /// </summary>
    public UILabel mNickNameLabel;

    /// <summary>
    /// UID
    /// </summary>
    public UILabel mUIDLabel;

    /// <summary>
    /// 定位
    /// </summary>
    public UILabel mAddressLabel;

    /// <summary>
    /// 互动表情
    /// </summary>
    public GameObject mHuDongFaceGo;


    #endregion

    #region 私有

    private string mNickName;

    private string mUID;

    private int mSeatId;

    private CallBack<int> mCallBack;//回调方法


    #endregion

    protected override void Start()
    {
        base.Start();
        SetHDFace();
    }

    #region 外部

    /// <summary>
    /// 设置数据
    /// </summary>
    /// <param name="url"></param>
    /// <param name="name"></param>
    /// <param name="id"></param>
    /// <param name="adr"></param>
    /// <param name="seatId">座位号</param>
    public void SetData(bool show, string url, string name, string id, string adr, int seatId, CallBack<int> callback)
    {
       // show = true;
        mHuDongFaceGo.gameObject.SetActive(show);
        mCallBack = callback;
        mNickName = name;
        mUID = id;
        mSeatId = seatId;

        Assets.LoadIcon(url, (T) =>
        {
            mHead.mainTexture = T;
        });
        Debug.Log("=============" + name);
        mNickNameLabel.text =  name;
    }

    /// <summary>
    /// 返回玩家座位号
    /// </summary>
    /// <returns></returns>
    public int GetSeatId()
    {
        return mSeatId;
    }

    #endregion

    #region UI事件

    /// <summary>
    /// 关闭点击事件
    /// </summary>
    public void OnCloseClick()
    {
        Close<GameUserInfoWidget>();
    }

    /// <summary>
    /// 互动表情点击
    /// </summary>
    /// <param name="go"></param>
    public void OnHuDongFaceItemClick(GameObject go)
    {
        if (mCallBack != null)
            mCallBack(int.Parse(go.name));
        Close();
    }

    #endregion


    private void SetHDFace()
    {
        List<ConfigDada> configData = ConfigManager.GetConfigs<TSTHuDongFaceConfig>();
        if (configData != null && configData.Count > 0)
        {
            GameObject obj;
            TSTHuDongFaceConfig con;
            for (int i = 0; i < configData.Count; i++)
            {
                con = configData[i] as TSTHuDongFaceConfig;
                obj = NGUITools.AddChild(faceGrid.gameObject, faceItem);
                obj.name = con.id.ToString();
                obj.transform.GetChild(0).GetComponent<UISprite>().spriteName = con.foreName + "0";
                obj.transform.GetChild(0).GetComponent<UISprite>().MakePixelPerfect();
                obj.SetActive(true);
            }
            faceGrid.Reposition();
        }
    }
}
