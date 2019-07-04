using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
//using YunvaIM;

public class GameInteractionView : BaseView
{
    public GameObject mItem;//互动表情item
    public GameObject mItemRoot;//互动表情item父节点
    public Transform mFrom;//开始节点
    public Transform mTo;//目标节点
    public GameTextChatShowItem[] mTextItems;//文字聊天item
    public GameObject mTextRoot;//文字root
    public SpriteAnimation[] mFaces;//表情
    public GameObject mVoiceItem;//语音
    public GameObject mVoiceRoot;//语音root

    private Dictionary<int, GameTextChatShowItem> mTextChat = new Dictionary<int, GameTextChatShowItem>();//文字聊天
    private Dictionary<int, GameObject> mVoiceDic = new Dictionary<int, GameObject>();//语音聊天喇叭

    #region 互动表情
    /// <summary>
    /// 添加一个互动表情
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="con"></param>
    public void AddOneInteractionFace(Vector3 from, Vector3 to, SendReceiveGameChat data)
    {
        List<ConfigDada> conList = ConfigManager.GetConfigs<TSTHuDongFaceConfig>();
        TSTHuDongFaceConfig con = null;
        for (int i = 0; i < conList.Count; i++)
        {
            TSTHuDongFaceConfig temp = conList[i] as TSTHuDongFaceConfig;
            if (temp.id == data.faceIndex)
            {
                con = temp;
                break;
            }
        }
        if (con == null)
            return;
        GameObject go = NGUITools.AddChild(mItemRoot, mItem);
        go.gameObject.SetActive(true);
        TweenPosition tween = go.GetComponent<TweenPosition>();
        UISprite sp = go.GetComponent<UISprite>();
        SpriteAnimation anim = go.GetComponent<SpriteAnimation>();
        //设置起始点和目标点
        mFrom.position = from;
        mTo.position = to;
        from = mFrom.localPosition;
        to = mTo.localPosition;
        //飞行动画和表情动画
        sp.spriteName = con.foreName + "0";
        tween.duration = 0.4f;
        string sound = con.sound;
        tween.AddOnFinished(() =>
        {
            anim.SetBegin(con.foreName, 1, con.length);
            anim.SetDalayDestory(4.0f);
            SoundProcess.PlaySound("HuDongFaceSound/" + sound);
        });
        tween.from = from;
        tween.to = to;
        tween.PlayForward();
    }

    #endregion



    #region 文字聊天，文字语音，语音，表情
    /// <summary>
    /// 文字聊天，文字语音，语音，表情
    /// </summary>
    /// <param name="pos">位置，世界坐标</param>
    /// <param name="_type">类型</param>
    /// <param name="data">数据</param>
    public void AddOneChat(Vector3 pos, eChatTextDirectionType _type, SendReceiveGameChat data)
    {
        eGameChatContentType chatType = (eGameChatContentType)data.chatType;
        switch (chatType)
        {
            case eGameChatContentType.Chat://文字
            case eGameChatContentType.TexTVoice://文字语音
                AddOneText(pos, _type, data);
                break;
            case eGameChatContentType.Face://表情
                AddOneFace(pos, _type, data);
                break;
            case eGameChatContentType.Voice://语音
                AddOneVoice(pos, _type, data);
                break;
        }
    }
    #endregion

    #region 文字聊天或文字语音
    /// <summary>
    /// 添加一个文字聊天或文字语音
    /// </summary>
    /// <param name="pos">位置，世界坐标</param>
    /// <param name="_type">类型</param>
    /// <param name="data">数据</param>
    private void AddOneText(Vector3 pos, eChatTextDirectionType _type, SendReceiveGameChat data)
    {
        int index = (int)_type;
        GameTextChatShowItem item;
        if (mTextChat.ContainsKey(data.fromSeatId))
            item = mTextChat[data.fromSeatId];
        else
        {
            if (index < 0 || index >= mTextItems.Length)
                return;
            GameObject obj = NGUITools.AddChild(mTextRoot, mTextItems[index].gameObject);
            item = obj.GetComponent<GameTextChatShowItem>();
            mTextChat[data.fromSeatId] = item;
        }
        item.SetData(data, pos, _type);
    }
    #endregion

    #region 表情
    /// <summary>
    /// 添加一个表情
    /// </summary>
    /// <param name="pos">位置，世界坐标</param>
    /// <param name="_type">类型</param>
    /// <param name="data">数据</param>
    private void AddOneFace(Vector3 pos, eChatTextDirectionType _type, SendReceiveGameChat data)
    {
        int index = _type.GetHashCode();
        if (index >= 0 && index < mFaces.Length)
        {
            //List<ConfigDada> configs = ConfigManager.GetConfigs<GameFaceConfig>();
            //ConfigDada conData = configs.Find(o => o.conIndex == data.faceIndex);
            //if (conData != null)
            //{
            //    mFaces[index].transform.position = pos;
            //    GameFaceConfig con = conData as GameFaceConfig;
            //    mFaces[index].SetBegin(con.animSprite, con.startEnd[0], con.startEnd[con.startEnd.Length - 1], 2, 0.25f, false);
            //}
        }
    }
    #endregion

    /// <summary>
    /// 添加一个语音
    /// </summary>
    /// <param name="pos">位置，世界坐标</param>
    /// <param name="_type">类型</param>
    /// <param name="data">数据</param>
    private void AddOneVoice(Vector3 pos, eChatTextDirectionType _type, SendReceiveGameChat data)
    {
        bool ised = PlayerPrefs.GetInt("DDL_" + PlayerModel.Inst.UserInfo.userId) == 1 ? true : false;//等于1是勾选了，其他是未勾选
        if (!ised) {
            string ext = DateTime.Now.ToFileTime().ToString();
#if YYVOICE
            YunVaImSDK.instance.RecordStartPlayRequest("", data.content, ext, (data2) =>
            {
                if (data2.result == 0)
                {
                    SQDebug.Log("播放成功");
                }
                else
                {
                    SQDebug.Log("播放失败");
                }
            });

#endif
        }
        int index = data.fromSeatId;
        GameObject obj;
        if (mVoiceDic.ContainsKey(index))
            obj = mVoiceDic[index];
        else
        {
            obj = NGUITools.AddChild(mVoiceRoot, mVoiceItem);
            mVoiceDic[index] = obj;
        }
        obj.SetActive(true);
        obj.transform.position = pos;
        float t = data.voiceChatTime / 1000f;
        DelayRun(t, () =>
         {
             obj.SetActive(false);
         });
    }
}
