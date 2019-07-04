using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameGoldPatternConfig : ConfigDada {

    public int gameId;
    public string name;
    public List<GameGoldPatternConfigConfig> config;
}


public class GameGoldPatternConfigConfig {
    public int lvId;
    public string lvName;
    public float baseScore;//底分
    public int cost;//消耗
    public int minFore;//最小分数
    public int minAfter;//最大分数
    public string minStr;
    public string icon;//图片
    public string effectColor;//描边颜色
    public string tittle;//标题提示

}