using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Engine;
using System.Text;
using System;

public enum NETSTATE
{
    CLOSENET,
    NETREALEASE
}



public class NetProcess : MonoBehaviour
{

    public static NetProcess It;

    public delegate void MessegeCallBack(MessageData msgData);

    public delegate void ConnectCallBack(bool connectResult, int sessionID);
    public delegate void ConnectStateCallBack(NETSTATE state, int sessionId);

    public static List<int> LockResponseId = new List<int>();
    //游戏内部协议 
    public static List<int> mGameTypeList = new List<int>();
    //大厅协议
    public static List<int> mHallTypeList = new List<int>();

    private static ConnectionManager mConnectionManager = null;
    private static Dictionary<int, MessegeCallBack> ResonponseCallDic = new Dictionary<int, MessegeCallBack>();
    private static ConnectCallBack connCallBack = null;
    private static Connection GameCenter = null;
    public static ConnectStateCallBack NetConnectStateCall;
    public static int mCurConnectSessionId = -1;
    //
    private static string mIpAdress;
    private static int mPort;
    private static NetLoadingController mNetmaskCtr;

    void Awake()
    {
        It = this;
        mConnectionManager = new ConnectionManager();
        mConnectionManager.OnConnectEvent = OnExecConnect;
        mConnectionManager.OnReceiveEvent = OnExecReceive;
        mConnectionManager.OnCloseEvent = OnExecClose;
        mNetmaskCtr = Global.Inst.GetController<NetLoadingController>();
    }

    private void Start()
    {

    }

    public static void InitNetWork(string Ip, int port)
    {
        mIpAdress = Ip;
        mPort = port;
    }

    //不用这个
    private static void Connect(bool isGameCenter, ConnectCallBack callBack)
    {
        mNetmaskCtr.ShowLoading(true);
        Connection c = mConnectionManager.CreateConnection(mIpAdress, mPort);
        connCallBack = callBack;
        if (isGameCenter)
        {
            GameCenter = c;
        }
    }

    public static void Connect(string Ip, int port, ConnectCallBack callBack)
    {
        mNetmaskCtr.ShowLoading(true);
        Connection c = mConnectionManager.CreateConnection(Ip, port);
        connCallBack = callBack;
    }

    //释放除了参数外的链接
    public static void ReleaseConnectExpectID(int sessionID = -1)
    {
        mConnectionManager.ReleaseAllExpectOne(sessionID);
    }
    /// <summary>
    /// 清除所有连接
    /// </summary>
    public static void ReleaseAllConnect()
    {
        mConnectionManager.ReleaseAllConnection();
        mConnectionManager.CurrentConnection = null;
        ConnectionManager.Instance.MessageEventQueue.ClearAll();
    }

    /// <summary>
    /// 当前链接是否存在且未连接
    /// </summary>
    /// <returns></returns>
    public static bool IsCurExistConnected()
    {
        if (mConnectionManager == null)
            return true;
        if (mConnectionManager != null && mConnectionManager.CurrentConnection != null)
            return mConnectionManager.CurrentConnection.Connected;
        return false;
    }

    /// <summary>
    /// 大厅是否连接
    /// </summary>
    /// <returns></returns>
    public static bool IsHallConnect()
    {
        if (mConnectionManager != null && mConnectionManager.CurrentConnection != null)
            return mConnectionManager.CurrentConnection.Ip.Equals(GameManager.Instance.Ip) && mConnectionManager.CurrentConnection.Port == GameManager.Instance.port;
        return false;
    }

    public static void BackToGameCenter(int sessionId)
    {
        mConnectionManager.BackToGameCenter(sessionId);
    }

    /// <summary>
    /// Registers the response call back.
    /// </summary>
    /// <param name="msgType">Message type.</param>
    /// <param name="msgCallBack">Message call back.</param>
    /// <param name="gameType">Game type.  不为0的则为游戏内协议 </param>
    public static void RegisterResponseCallBack(int msgType, MessegeCallBack msgCallBack, int gameType = 0)
    {
        ResonponseCallDic[msgType] = msgCallBack;

        if (gameType == 0)
        {
            mHallTypeList.Add(msgType);
        }
        else
        {
            mGameTypeList.Add(msgType);
        }

    }

    public static void UnRegisterResponseCallBack(int msgType)
    {
        if (ResonponseCallDic.ContainsKey(msgType))
        {
            ResonponseCallDic.Remove(msgType);
        }
    }

    public static void CleanResponseProtoByType(int gameType)
    {
        if (gameType == 0)
        {
            for (int i = 0; i < mHallTypeList.Count; i++)
            {
                UnRegisterResponseCallBack(mHallTypeList[i]);
            }
        }
        else
        {

            for (int i = 0; i < mGameTypeList.Count; i++)
            {
                UnRegisterResponseCallBack(mGameTypeList[i]);
            }

        }

    }

    #region XLUA


    public delegate void SendRequestCall(object data);

    /// <summary>
    /// Lua调用方法 
    /// </summary>
    /// <param name="data"></param>
    /// <param name="cmdId"></param>
    /// <param name="call"></param>

    public static void SendMsgByLua(byte[] data, int cmdId, SendRequestCall call, bool isShowLock = true)
    {
        SendRequestLuaMsg(data, cmdId, (msg) =>
        {
            SQDebug.Log("wor2222iyi=" + msg.msgData.Length);
            if (call != null)
            {
                call(msg.msgData);
            }
        }, isShowLock);
    }


    /// <summary>
    /// Lua调用注册方法
    /// </summary>
    /// <param name="msgType"></param>
    /// <param name="call"></param>

    public static void RegisterMsgByLua(int msgType, SendRequestCall call)
    {
        RegisterResponseCallBack(msgType, (msg) =>
        {
            if (call != null)
            {
                call(msg.msgData);
            }
        });
    }

    #endregion

    public static void SendRequest<T>(object o, int msgType, MessegeCallBack msgCallBack = null, bool showlock = true)
    {
        if (showlock)
        {
            mNetmaskCtr.ShowLoading(true);
        }
        Action act = () =>
        {
            MessageData msg = new MessageData(o, msgType);
            msg.Write<T>(o, msgType);
            ResonponseCallDic[msgType] = msgCallBack;
            if (msgCallBack != null)
            {
                SetLockScreen(msgType, showlock);
            }
            ConnectionManager.Instance.CurrentConnection.SendMessage(msg.msgData);
        };

        if (mConnectionManager.CurrentConnection != null && mConnectionManager.CurrentConnection.Connected)
        {
            act();
        }
        else
        {
            //Connect(false, (result, sessionId) =>
            //{
            //    if (result)
            //    {
            //        act();
            //    }
            //});
        }
    }

    /// <summary>
    /// 提供给LUA过来调用的接口
    /// </summary>
    /// <param name="o"></param>
    /// <param name="msgType"></param>
    /// <param name="msgCallBack"></param>
    /// <param name="showlock"></param>
	public static void SendRequestLuaMsg(byte[] o, int msgType, MessegeCallBack msgCallBack = null, bool showlock = true)
    {
        Action act = () =>
        {
            MessageData msg = new MessageData(o, msgType);
            msg.WriteLua(o, msgType);
            ResonponseCallDic[msgType] = msgCallBack;
            if (msgCallBack != null)
            {
                SetLockScreen(msgType, showlock);
            }
            ConnectionManager.Instance.CurrentConnection.SendMessage(msg.msgData);
        };

        if (mConnectionManager.CurrentConnection.Connected)
        {
            act();
        }
        else
        {
            Connect(GameManager.Instance.Ip, GameManager.Instance.port, (result, sessionId) =>
            {
                if (result)
                {
                    act();
                }
            });
        }
    }




    public static void SetLockScreen(int unLockResponseID, bool isShow)
    {
        if (!LockResponseId.Contains(unLockResponseID))
        {
            if (LockResponseId.Count == 0 && isShow)
            {
                //Global.It.mCommonBoxCtr.ShowLoading(true);
            }
            LockResponseId.Add(unLockResponseID);
        }
    }

    private int score = 0;
    void Update()
    {
        if (mConnectionManager != null)
        {
            mConnectionManager.Update();
            // NGUISQDebug.Log(this.score);
        }
    }

    //Socket connected event.
    void OnExecConnect(Connection connection, SOCKET_ERRCODE errCode)
    {
        mNetmaskCtr.ShowLoading(false);
        if (errCode != SOCKET_ERRCODE.SUCCESS)
        {
            SQDebug.Log("failed to connect to server:" + connection.SessionID.ToString() + "; socket:" + connection.ToString() + " ; error code : " + errCode.ToString());
            connCallBack(false, 0);
            return;
        }
        SQDebug.Log("success to connect server; socket:" + connection.SessionID.ToString());
        ConnectionManager.Instance.CurrentConnection = connection;
        connCallBack(true, connection.SessionID);
    }

    bool OnExecReceive(Connection connection, int key, byte[] msgData)
    {
        if (key != 1500)
        {
            SQDebug.Log("收到消息ID为：" + key.ToString());
        }
        if (LockResponseId.Contains(key))
        {
            LockResponseId.Remove(key);
            if (LockResponseId.Count == 0)
            {
                //CommonUI.Instance.HideLockScreen();
            }
        }
        if (key != 1500)
            mNetmaskCtr.ShowLoading(false);
        if (msgData == null)
        {
            return false;
        }

        MessageData msg = new MessageData(msgData);

        string jsonString = Encoding.UTF8.GetString(msgData, 0, msgData.Length);
        //if (key != 1102)
        //{
        //    SQDebug.PrintToScreen("Json=" + jsonString);
        //    SQDebug.Log("Json=" + jsonString);
        //}

        if (!ResonponseCallDic.ContainsKey(key))
        {
            SQDebug.Log("Client Don`t wanna handle this messege ,msgType : " + key);
            return false;
        }
        MessegeCallBack callBack = ResonponseCallDic[key];
        if (callBack == null)
        {
            SQDebug.Log("callBack Is Null ,msgType : " + key);
            return false;
        }
        else
        {
            //Global.It.mCommonBoxCtr.ShowLoading(false);
            //CommonUI.It.HideLoad();
        }

        if (!callBack.Method.IsStatic && callBack.Target.ToString() == "null")
        {
            SQDebug.LogWarning("对象已经被销毁了，而消息还想被处理");

            if (NetConnectStateCall != null)
            {
                //连接已释放
                NetConnectStateCall(NETSTATE.NETREALEASE, connection.SessionID);
            }
            return false;

        }

        callBack(msg);
        return true;
    }


    void OnExecClose(Connection connection, SOCKET_ERRCODE errCode)
    {
        SQDebug.Log("Close Socket! socket sessionid :" + connection.SessionID + "; errCode:" + connection.ExceptionCode + "; errMsg:" + connection.ExceptionMessage);
        ConnectionManager.Instance.ReleaseConnection(connection);
        ConnectionManager.Instance.MessageEventQueue.ClearAll();
        mNetmaskCtr.ShowLoading(false);
        GameManager.Instance.ShowNetTips();
        if (NetConnectStateCall != null)
        {
            NetConnectStateCall(NETSTATE.CLOSENET, connection.SessionID);
        }

    }
}


