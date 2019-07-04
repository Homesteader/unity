using UnityEngine;
using System.Collections;

//网络请求接口，由服务器人员提供
public class MJProtoMap
{

    #region 进入游戏&更新模块

    public const int CMD_Update = 1000;    //更新协议 

    #endregion



    #region 登陆模块
    // 详情,请看具体协议文档
    public const int CMD_Login = 1300;   // 登陆 主动 
    public const int CMD_Logout = 1101;  // 登出 主动   
    public const int CMD_Breath = 1102;   // 检测心跳连接 主动
    public const int CMD_OtherLogin = 1103; //其他设备登录 被动
    public const int CMD_Version = 1104;   // 版本 主动
    public const int CMD_CheckVersion = 1105;   // 检查版本 主动
    public const int CMD_StopServerNotice = 1106;//获取停机公共 主动
    public const int Cmd_Auth = 1107;       // 登入游戏服认证
    #endregion

    #region 大厅模块

    public const int CMD_CreateRoom = 3007;   //  创建房间 主动
    public const int CMD_GoIntoRoom = 3008;//加入房间 主动
    public const int CMD_PlayerJoinInRoom = 3009;//有玩家加入房间
    public const int CMD_GetPerInfor = 3009;//查看玩家信息

    public const int CDM_GetAllDetailInfo = 3011;//获取战绩
    #endregion
    

    public const int Cmd_GetGameStart = 3000;  //同步开始游戏放间内所有的信息

    public const int Cmd_SendDealIns = 3001; //发送 操作指令

    public const int Cmd_GetOnWhoOptIns = 3002;//同步谁操作了什么指令

    public const int Cmd_GetOnOptList = 3003;//同步给玩家可以操作什么指令

    public const int Cmd_GetErrorCode = 3004;//错误码
    public const int Cmd_GetSettlement = 3005;//结算
    public const int Cmd_GetBigSem = 3006;//结算
    public const int CMD_SendJoinMJPattern = 3012;//加入平台房
    public const int CMD_SendChangeDesk = 3013;//换桌
    public const int CMD_SendChat = 3014;//发送聊天
    public const int CMD_OnGetChat = 3015;//接收聊天
    public const int CMD_GetGpsInfo = 3016;//距离信息
    public const int CMD_SendAddress = 3017;//发送距离
    public const int CMD_ReadyCountDown = 3018;//准备倒计时

}
