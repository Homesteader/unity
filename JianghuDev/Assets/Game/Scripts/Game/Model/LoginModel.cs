using UnityEngine;
using System.Collections;

public class LoginModel : BaseModel
{
    public static LoginModel Inst;

    public int mSessionId = -1;

    public LoginType mLoginType = LoginType.Nil;//登录类型

    public int mReConnectNetNum = 0;//重新连接次数

    public bool mNeedOpenMainView = true;// 是否需要打开大厅界面

    public override void SetController(BaseController c)
    {
        base.SetController(c);
        Inst = this;
    }

    public bool IsVisitor;//是否是游客登录

    /// <summary>
    /// 重置密码界面，获取验证码时间，60秒后才能再获取
    /// </summary>
    public float ResetPasswordTime;

    public LoginSR.SendLogin LoginData;//保存的发送数据
}
