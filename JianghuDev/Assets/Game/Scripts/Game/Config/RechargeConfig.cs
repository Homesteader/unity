using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RechargeConfig : ConfigDada
{
    public List<RechargeData> CardConfig;//房卡配置表
}


public struct RechargeData
{
    public int id;//条目id
    public float cost;//花费
    public float getNum;//获得的房卡或金币
    public string spName;//图标名字
    public float giving;//赠送的房卡或金币
    public string desc;//描述
    public string givingDesc;//赠送描述
    public ePropType type;  //道具类型  1金币   2房卡
}

