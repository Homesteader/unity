using UnityEngine;
using System.Collections;

public class CreateRoomRule : ConfigDada {
    public int gameId;     //游戏id
    public string name;    //名字
    public CreatRule[] rules;//规则
}


public class CreatRule
{
    public int id;//id 
    public string name;//名字  
    public int limit;//一排的限制个数
    public CreatRuleData[] content;//内容

}

public struct CreatRuleData
{
    public int Index; //index 
    public bool isSelected;//是否选中        
    public int group;  //toggle分组
    public int optionType;//类型，1：单选按钮，2：复选框 都用uitoggle做，toggle的分组就用group的值
    public string name;//名字
    public int value;
    public int connect;
    public bool none;
}

/// <summary>
/// 麻将
/// </summary>
public class MJGameCreateConfig : CreateRoomRule { }

/// <summary>
/// 金花
/// </summary>
public class JinHuaGameCreateConfig : CreateRoomRule { }


public class NiuNiuGameCreateConfig : CreateRoomRule { }


public class TenGameCreateConfig : CreateRoomRule { }

