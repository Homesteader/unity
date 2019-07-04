using UnityEngine;
using System.Collections;

/// <summary>
/// 道具配置表
/// </summary>
public class PropConfig : ConfigDada
{
    public string type;//道具id
    public string name;//道具名称
    public string icon;//图标
    public PropAward[] award;//奖励道具 
}

/// <summary>
/// 道具奖励数据
/// </summary>
public struct PropAward
{
    public string id;//道具id
    public int num;//数量
}
