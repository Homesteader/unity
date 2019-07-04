using UnityEngine;
using System.Collections;

public class ProtoIdMap
{

    public const int CMD_Login = 1100;//登录 主动
    public const int CMD_GameLoginMain = 1102;//游戏服务器登录大厅服务器 主动
    public const int CMD_LoginOut = 1200;//退出登录 主动
    public const int CMD_Breath = 1500;//心跳连接  主动
    public const int CMD_RecieveLoginInOther = 1600;//异地登录 被动
    public const int CMD_GetRank = 2001;//获取排行榜  主动
    public const int CMD_GetEmail = 2004;  //获取邮件列表  主动
    public const int CMD_SendRead = 2005;  //阅读某封邮件    主动
    public const int CMD_SendDeletEmail = 2006; //删除某封邮件  主动
    public const int CMD_GetTask = 2007; //获取任务列表  主动
    public const int CMD_FinishTask = 2009;  //完成一次任务
    public const int CMD_RecieveBroadMsg = 2011;//接收到广播公共 被动
    public const int CMD_GetBagData = 2014;     //获取背包数据  主动
    public const int CMD_UseBagProp = 2015;     //使用背包道具  主动
    public const int CMD_DeletProp = 2016;      //删除道具  主动
    public const int CMD_PropChange = 2017;     //道具变更  被动
    public const int CMD_GetTaskAward = 2010;  //获取任务奖励 主动
    public const int CMD_SendCheckIdCard = 2021;//实名认证
    public const int CMD_SendGetRank = 2022;//过去排行榜
    public const int CMD_UseRedBag = 2023;  //使用红包 主动
    public const int CMD_InstallAddFriend = 2025;//扫二维码加好友

    public const int CMD_GetGameIp = 2018;//获取游戏Ip  主动
    public const int CMD_FeedBack = 2012;//用户反馈 主动

    public const int CMD_SendPayInfo = 2022;  //用户购买商品,获取充值消息  主动
    public const int CMD_ReceivePayInfo = 2024;  //充值消息返回  被动
    public const int CMD_SendLocation = 4003;//更新玩家定位信息
    public const int CMD_ShareRecord = 4009;//分享战绩
    public const int CMD_GetRecord = 2030;//获取战绩


    public const int CMD_SendGetBigWheelReslut = 1461;//获取抽奖结果


    #region xxgame 被动

    public const int CMD_OnGetGoldChang = 1700;//金币改变

    public const int CMD_OnGamePatternPersonChanged = 1701;//人数变化 person 在线人数变化

    public const int CMD_OnNewMessageGet = 2025;//得到新的消息


    #endregion


    #region xxgame 主动
    public const int CMD_SendGetWelfareTex = 2115;//获取福利图片

    public const int CMD_sendGetPrizeInfo = 2112;  //获取转盘信息

    public const int CMD_sendPrizeBack = 2113;     //抽奖返回

    public const int CMD_sendGetPrizeRecord = 2114; //抽奖记录

    public const int CMD_SendGetNotice = 2002;//获取滚动公告

    public const int CMD_SendGetActivity = 2003;//获取活动图片

    public const int CMD_SendGetGameServer = 2004;//获取游戏服务器地址

    public const int CMD_SendGetClubInfo = 2005;//获取俱乐部信息

    public const int CMD_SendAddClubUser = 2006;//添加俱乐部玩家

    public const int CMD_SendGetClubUsers = 2007;//获取俱乐部玩家列表

    public const int CMD_SendCreateClub = 2008;//创建俱乐部

    public const int CMD_SendGetClubCompanys = 2009;//获取联盟列表

    public const int CMD_SendAddClubConpany = 2010;//添加联盟

    public const int CMD_SendDealMessage = 2011;//处理消息

    public const int CMD_SendGetMessage = 2012;//获取消息列表

    public const int CMD_SendDelClubUser = 2013;//删除俱乐部成员

    public const int CMD_SendCancelCompany = 2014;//取消联盟

    public const int CMD_SendBorrowGold = 2015;//借金币

    public const int CMD_SendReturnGold = 2016;//还金币

    public const int CMD_SendGetOutWare = 2017;//取仓库金币

    public const int CMD_SendSaveInWare = 2018;//存仓库金币

    public const int CMD_SendGetWareInfo = 2019;//获取仓库信息

    public const int CMD_SendGetOutWin = 2020;//提取盈利

    public const int CMD_SendGetAgentGain = 2023;//获取代理收益信息

    public const int CMD_OnNewMailShowRed = 2026;//有新的邮件，显示红点

    public const int CMD_SendGetGain = 2027;//获取收益情况

    public const int CMD_FixPsd = 2029;//修改密码

    public const int CMD_SendGainDetail = 2100;//获取收益详情

    public const int CMD_SendGetBRRecord = 2101;//获取获赏打赏记录

    public const int CMD_SendReadMessage = 2106;//发送读消息

    public const int CMD_SendGetPlayerRecordDetail = 2107;//获取玩家获赏打赏记录

    public const int CMD_SendGetRankGameRound = 2108;//获取局数排行榜

    public const int CMD_SendBuyRoomCard = 2102;//购买房卡

    public const int CMD_SendGetUserBRRecord = 2103;//获取玩家的打赏获赏记录 JR GH

    public const int CMD_SendFeedBack = 2105;//反馈

    public const int CMD_SendJoinClub = 2109;//加入俱乐部

    public const int CMD_SendGetGeneralInfo = 2110;//获取总代信息

    public const int CMD_SendCheckUserInfo = 2028;//核实玩家信息


    public const int CMD_SendGetPayUrl = 2111;//获取 购买链接

    public const int CMD_GetPayBackInfo = 2112;//购买返回(推送)

    public const int CMD_GetServiceInfo = 2031;//获取客服信息


    #endregion


    #region 游戏模式

    public const int CMD_SendGetRoomList = 3501;// 获取房间列表

    public const int CMD_SendGetGoldPeopleNum = 3552;//获取平台场的人数
    public const int CMD_SendCheckInGame = 3553;//检测是否在游戏中

    public const int CMD_OnGetRoomChang = 3614;//房间信息发生变化

    public const int CMD_SendChangChouCheng = 4012;//发送设置抽成 id float

    #endregion

}
