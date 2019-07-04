using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MJGameVoiceData
{
    public int id;//id
    public string name;//名字
    public string[] voice;//音效
    public string[] insTypeVoice;//特殊类型音效
}

public class MJGameVoiceConfig : ConfigDada
{
    public List<MJGameVoiceData> card;//牌音效
    public List<MJGameVoiceData> ins;//操作音效
}

