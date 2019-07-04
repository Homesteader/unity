using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MJGameRuleConfig: ConfigDada
{
    public class GameRuleDetail
    {
        /// <summary>
        /// 规则ID
        /// </summary>
        public int ruleId;
        /// <summary>
        /// 名称
        /// </summary>
        public string name;
        /// <summary>
        /// 英文名
        /// </summary>
        public string enName;
        /// <summary>
        /// 是否默认选中
        /// </summary>
        public bool isSelected;
        /// <summary>
        /// 分组
        /// </summary>
        public int group;
        /// <summary>
        /// 是否会现在在界面上
        /// </summary>
        public bool isShow;
    }

    public int gameIndex;//游戏index
    public string gameName;//游戏名字
    public GameRuleDetail[] gameRule;//游戏规则

}

