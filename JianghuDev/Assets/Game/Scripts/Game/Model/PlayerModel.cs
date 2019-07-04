using UnityEngine;
using System.Collections;

public class PlayerModel
{
    private static PlayerModel _inst;

    public static PlayerModel Inst
    {
        get
        {
            if (_inst == null)
                _inst = new PlayerModel();
            return _inst;
        }
    }

    public PlayerModel()
    {
        //address = new SendAddrReq();
        //address.address = "成都市成华区";
        //address.latitude = 28f;
        //address.longitude = 30f;
    }

    /// <summary>
    /// 定位信息
    /// </summary>
    public SendAddrReq address;

    /// <summary>
    /// 登录token
    /// </summary>
    public string Token;
    /// <summary>
    /// 玩家信息
    /// </summary>
    public LoginSR.LoginUserInfo UserInfo;
    /// <summary>
    /// 公众号
    /// </summary>
    public string PublicSign;

    public string OpenInstallArgs;//扫二维码数据

    /// <summary>
    /// 更新房卡和金币
    /// </summary>
    /// <param name="roomCard"></param>
    /// <param name="glods"></param>
    public void UpdateRoomCardGlods(float roomCard,float glods) {

        if (roomCard!=-1) {
            UserInfo.roomCard = roomCard;
        }
        if (glods!=-1) {
            UserInfo.gold = glods;          
        }

        GlobalEvent.dispatch(eEvent.UPDATE_PROP);
    }
}
