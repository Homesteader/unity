using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameTextChatShowItem : BaseViewWidget
{
    public eChatTextDirectionType mType;//指向类型
    public UISprite mBg;//背景
    public UILabel mLabel;//文本框

    /// <summary>
    /// 设置数据并显示
    /// </summary>
    /// <param name="data">聊天数据</param>
    /// <param name="pos">坐标，世界坐标</param>
    /// <param name="call">回调</param>
    public void SetData(SendReceiveGameChat data, Vector3 pos, eChatTextDirectionType dType)
    {
        //设置位置
        transform.position = pos;
        gameObject.SetActive(true);
        eGameChatContentType _type = (eGameChatContentType)data.chatType;
        if (_type == eGameChatContentType.Chat)//普通文字
            mLabel.text = data.content;
        else if(_type == eGameChatContentType.TexTVoice)//文字语音
        {
            List<ConfigDada> conList = ConfigManager.GetConfigs<TSTGameTxtChatConfig>();
            ConfigDada conData = conList.Find(o => o.conIndex == data.faceIndex.ToString());
            if (conData == null)
                return;
            TSTGameTxtChatConfig con = conData as TSTGameTxtChatConfig;
            mLabel.text = con.name;

            string voice = data.sex == 1 ? con.soundNameman : con.soundNamewoman;
            SoundProcess.PlaySound("ChatSound/" + voice);
        }
        mBg.width = 20 + mLabel.width;//设置背景长度
        DelayRun(2, () =>
        {
            gameObject.SetActive(false);
        });
    }

    private void SetDirection(eChatTextDirectionType dType)
    {
        //设置背景和文字方向
    }
}
