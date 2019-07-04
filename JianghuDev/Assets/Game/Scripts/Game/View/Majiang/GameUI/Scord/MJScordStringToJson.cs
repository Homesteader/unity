using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MJScordStringToJson{
    public MJScordListItemJson mScordListItemJson = new MJScordListItemJson();
    public List<MJRankListStringRef> mRankListStringRefList = new List<MJRankListStringRef>();
}


public class MJRankListStringRef {
    public string nickName;//玩家昵称
    public int score;//玩家分数
    public string userId;//玩家id
    public string headUrl;//玩家头像地址
}

public class MJScordListItemJson :ReferenceBase<MJRankListStringRef>{
    public void SetData(List<MJRankListStringRef> info)
    {
        data = info;
    }
}