using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameCreateConfig : ConfigDada
{
    public int gameid;   //游戏ID
    public List<GameCreateSubConfig> subConfig;
}

public class GameCreateSubConfig {
    public int gameid;   //游戏ID
    public string name;   //游戏名
    public bool selected;
}
