using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections;
using System.Collections.Generic;

namespace Engine
{

    enum SOCKET_ERRCODE
    {
        UNKNOWN = -1,
        FAILED = 0,
        SUCCESS = 1,
        ERR_CONNECT,
        ERR_RECEIVE_BEGIN_EXCEPTION,
        ERR_RECEIVE_END_EXCEPTION,
        ERR_RECEIVE_DEALDATA_EXCEPTION,
        ERR_RECEIVE_OVERBUFFER,
        ERR_SEND_BEGIN_EXCEPTION,
        ERR_SEND_END_EXCEPTION,
        ERR_END_OVERBUFFER,
    }


    enum QSOCKET_EVENTTYPE
    {
        EVNET_LISTEN,
        EVENT_CONNECT,				// 链接消息
        EVENT_RECEIVE,				// 接收到消息
        EVENT_CLOSE,				// 关闭Socket
        EVENT_EXCEPTION,
    };

    class QSocketEvent
    {
        public QSOCKET_EVENTTYPE eventType;
        public SOCKET_ERRCODE errCode;        
        public Connection socket;
        public int sessionID;
        public int msgType;
        public byte[] msgData = null;
    }


    class Connection
    {
        protected Socket        mSocket;

        protected bool          mIsConnected = false;
        protected int           mSessoinID = 0;
        protected long          mStartTime = 0;
        
        public SOCKET_ERRCODE   ExceptionCode;
        public string           ExceptionMessage;
                        
        protected ConnectionManager mSocketManager = null;
        public string Ip;
        public int Port;

        public static int       msgBufferLength = 512 * 1024;
        public CircleBuffer     msgBuffer = new CircleBuffer(msgBufferLength);

        public byte[]           buffer = new byte[BUFFER_SIZE];
        public const int        BUFFER_SIZE = 4096;
		
		public List<byte[]>		mSendList = new List<byte[]>();
		public float			mSendTime = 0;

        public Connection(ConnectionManager manager, string ip, int port)
        {
            mSocketManager = manager;
            Ip = ip;
            Port = port;
        }

        ~Connection()
        {                    
        }


        public Socket socket
        {
            get { return mSocket; }            
        }
        
        public int SessionID 
        {
            get { return mSessoinID; }
            set { mSessoinID = value; }
        }

        
        public bool Connected
        {
            get { return mIsConnected; }
            set { mIsConnected = value; }
        }

        public bool Connect(string hostName, int serviceport)
        {
            if (mIsConnected)
            {
                ExceptionCode = SOCKET_ERRCODE.ERR_CONNECT;
                ExceptionMessage = "error; socket has already connected!";
                return false; 
            }
			String newServerIp = "";
			AddressFamily newAddressFamily = AddressFamily.InterNetwork;
			IPv6SupportMidleware.getIPType (hostName, serviceport.ToString(), out newServerIp, out newAddressFamily);           
			mSocket = new Socket(newAddressFamily, SocketType.Stream, ProtocolType.Tcp);
            mSocket.SendTimeout = 40 * 1000;        //40s
            mSocket.ReceiveTimeout = 40 * 1000;     //40s

			mSendList.Clear();
			mSendTime = 0;
            
            try
            {   
				
				mSocket.BeginConnect(newServerIp, serviceport, new AsyncCallback(mSocketManager.AyncConnectCallback), this);
                
            }
            catch (Exception ex)
            {
                ExceptionCode = SOCKET_ERRCODE.ERR_CONNECT;
                ExceptionMessage = ex.Message;
                return false;
            }

            return true;
        }

        public void Close(SOCKET_ERRCODE error)
        {
            if (Connected)
            {
                mIsConnected = false;

                // 关闭Socket
                if (mSocket != null)
                {
                    mSocket.Close();
					mSocket = null;
                }

            }
            
            //mSocketManager.OnClose(this);
            this.ExceptionCode = error;
            QSocketEvent socketEvent = new QSocketEvent();
            socketEvent.eventType = QSOCKET_EVENTTYPE.EVENT_CLOSE;
            socketEvent.errCode = error;
            socketEvent.sessionID = SessionID;
            socketEvent.socket = this;
            mSocketManager.AddSocketEvent(socketEvent);
           
        }

         public void SendMessage(byte[] bytesSend)
         {
			if( mSessoinID == 0 )
				SendMessage(bytesSend, bytesSend.Length);
			else
				mSendList.Add(bytesSend);
         }

         public void SendMessage(byte[] bytesSend, int msgLen)
         {
             if (mSocket == null || !mSocket.Connected) return;

             //System.IAsyncResult iar;
             try
             {
                 /*iar = */mSocket.BeginSend(bytesSend, 0, msgLen, 0, new AsyncCallback(mSocketManager.AsynchSendCallback), this);
                //mSocket.Send(bytesSend);
             }
             catch (Exception ex)
             {
                 ExceptionCode = SOCKET_ERRCODE.ERR_SEND_BEGIN_EXCEPTION;
                 ExceptionMessage = ex.Message;
                 Close(SOCKET_ERRCODE.ERR_SEND_BEGIN_EXCEPTION);
             }
            //SQDebug.LogError("Send Success");
         }




    }

    class ConnectionManager
    {
        protected Hashtable    mConnsTable = null;
        protected MessageQueue mMessageQueue = null;

        protected static ConnectionManager mInstance = null;
        public Connection CurrentConnection = null;

        public bool NoDelay = true;

        public delegate void OnConnectDelegate(Connection so, SOCKET_ERRCODE errCode);
        public delegate bool OnReceiveDelegate(Connection so, int msgType,byte[] byteMsg);
        public delegate void OnCloseDelegate(Connection so, SOCKET_ERRCODE errCode);
                
        private OnReceiveDelegate   OnReceive;
        private OnConnectDelegate   OnConnect;
        public  OnCloseDelegate     OnClose;

        public static int Id = 0; //session id
                

        public ConnectionManager()
        {
            mConnsTable    = new Hashtable();
            mMessageQueue = new MessageQueue();

            mInstance = this;
        }

#region manager

        public static ConnectionManager Instance
        {
            get { return mInstance; }
        }

        public OnConnectDelegate OnConnectEvent
        {
            set { this.OnConnect = value; }
        }

        public OnReceiveDelegate OnReceiveEvent
        {
            set { this.OnReceive = value; }
        }

        public OnCloseDelegate OnCloseEvent
        {
            set { this.OnClose = value; }
        }


        //创建Socket
        public Connection CreateConnection(string ip, int port)
        {
            SQDebug.Log("连接IP：" + ip + "  端口：" + port);
            Connection so = new Connection(this, ip, port);
            so.Connect(ip, port);
            so.SessionID = ++Id;
            mConnsTable.Add(so.SessionID, so);
            NetProcess.mCurConnectSessionId = so.SessionID;
            return so;
        }

        // return a socket by session id
        public Connection GetConnection(int sessionID)
        {
            return (Connection)mConnsTable[sessionID];
        }	
        

        //释放创建的Socket
        public void ReleaseConnection(Connection connection)
        {
            ReleaseConnection(connection.SessionID);
        }


        /// <summary>
        /// 关闭所有连接
        /// </summary>
        public void ReleaseAllConnection()
        {
            if (mConnsTable != null)
            {
                foreach (int key in mConnsTable.Keys)
                {
                    Connection c = mConnsTable[key] as Connection;
                    if (c != null)
                        c.Close(SOCKET_ERRCODE.SUCCESS);
                }
                mConnsTable.Clear();
            }
        }

        //移除除开id外所有连接
        public void ReleaseAllExpectOne(int id)
        {
            if (mConnsTable != null)
            {
                Connection con = null;
                foreach (int key in mConnsTable.Keys)
                {
                    if (key == id)
                    {
                        con = mConnsTable[key] as Connection;
                        continue;
                    }
                    if (mConnsTable[key] != null)
                    {
                        Connection c = mConnsTable[key] as Connection;
                        if (c != null)
                            c.Close(SOCKET_ERRCODE.SUCCESS);
                    }
                }
                mConnsTable.Clear();
                if(con!=null)
                    mConnsTable[id] = con;
            }
        }


        public void ReleaseConnection(int sessionID)
        {
            Connection connection = mConnsTable[sessionID] as Connection;
			if(connection != null)
			{
            	connection.Close(SOCKET_ERRCODE.SUCCESS);
			}
            mConnsTable.Remove(sessionID);
        }

#endregion
 
        public MessageQueue MessageEventQueue
        {
            get { return mMessageQueue; }
        }

        public void AddSocketEvent(QSocketEvent socketEvent)
        {
            mMessageQueue.AddItem(socketEvent);
        }

        public void AyncConnectCallback(IAsyncResult ar)
        {
            Connection so = (Connection)ar.AsyncState;
            Socket sock = so.socket;
            so.Connected =  sock.Connected;


            //TODO: Optimize tcp/ip.
            if (NoDelay)
            {
                sock.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);
            }            

            QSocketEvent socketEvent = new QSocketEvent();
            socketEvent.eventType = QSOCKET_EVENTTYPE.EVENT_CONNECT;
            socketEvent.socket = so;
            socketEvent.sessionID = so.SessionID;

            try
            {
                sock.EndConnect(ar);
            }
            catch (Exception ex)
            {                
                socketEvent.errCode = SOCKET_ERRCODE.ERR_CONNECT;
                so.ExceptionCode = socketEvent.errCode;
                so.ExceptionMessage = ex.Message;
                AddSocketEvent(socketEvent);
                return;
            }

            if (sock.Connected == true)
            {
                socketEvent.errCode = SOCKET_ERRCODE.SUCCESS;                
            }
            else
            {
                socketEvent.errCode = SOCKET_ERRCODE.FAILED;                
            }
            so.ExceptionCode = socketEvent.errCode;

            AddSocketEvent(socketEvent);

            if (so.Connected == true)
            {
                try
                {// 开始接收数据
                    //System.IAsyncResult iar;                    
                    /*iar = */sock.BeginReceive(so.buffer, 0, Connection.BUFFER_SIZE, 0, new AsyncCallback(AsynchReadCallback), so);
                }
                catch (Exception ex)
                {
                    so.ExceptionCode = SOCKET_ERRCODE.ERR_RECEIVE_BEGIN_EXCEPTION;
                    so.ExceptionMessage = ex.Message;
                    so.Close(SOCKET_ERRCODE.ERR_RECEIVE_BEGIN_EXCEPTION);
                }
            }

        }


        private void AsynchReadCallback(System.IAsyncResult ar)
        {
            Connection so = (Connection)ar.AsyncState;
            Socket sock = so.socket;
            if (sock == null || !sock.Connected)
            {
                return;
            }
            
            try
            {
                int read = sock.EndReceive(ar);
                if (read > 0)
                {                 
                    try
                    {
                        if (!so.msgBuffer.AppendData(so.buffer, read))
                        {//内存不够了。
                            so.ExceptionCode = SOCKET_ERRCODE.ERR_RECEIVE_OVERBUFFER;
                            so.ExceptionMessage = "error; not enough message buffer for receive!";
                            so.Close(SOCKET_ERRCODE.ERR_RECEIVE_OVERBUFFER);
                            return;
                        }

                    }
                    catch (Exception ex)
                    {//出现问题了，关闭socket吧。                        
                        so.ExceptionCode = SOCKET_ERRCODE.ERR_RECEIVE_END_EXCEPTION;
                        so.ExceptionMessage = ex.Message;
                        so.Close(SOCKET_ERRCODE.ERR_RECEIVE_END_EXCEPTION);
                    }


                    while (OnReceiveData(ref so) == true) ;
                    //接收完数据，可能SOCKET关闭
                    if (so.Connected == true)
                    {
                        // 开始接收更多数据
                        sock.BeginReceive(so.buffer, 0, Connection.BUFFER_SIZE, 0, new AsyncCallback(AsynchReadCallback), so);
                    }
                }

            }
            catch (System.Exception ex)
            {
                so.ExceptionCode = SOCKET_ERRCODE.ERR_RECEIVE_END_EXCEPTION;
                so.ExceptionMessage = ex.Message;
                so.Close(SOCKET_ERRCODE.ERR_RECEIVE_END_EXCEPTION);
            }
        }


        public virtual bool OnReceiveData(ref Connection so)
        {

            if (!so.Connected || so.socket == null) return false;
            try
            {
                if (so.msgBuffer.Length < Message.MessageHeadSize) return false;
                //Get  msg  size
                byte[] bytesBodySize = new byte[2];
                so.msgBuffer.ReadData(ref bytesBodySize, 0);
                byte b = bytesBodySize[0];
                bytesBodySize[0] = bytesBodySize[1];
                bytesBodySize[1] = b;
                int bodySize = BitConverter.ToInt16(bytesBodySize, 0) - 4;
                //完整数据包还没有到达
                if (so.msgBuffer.Length < bodySize + Message.MessageHeadSize)
                {                    
                    return false;
                }
                so.msgBuffer.SubmitReadData(ref bytesBodySize);
                byte[] bytesHeadConfirm = new byte[4];
                so.msgBuffer.FectchData(ref bytesHeadConfirm);

                QSocketEvent socketEvent = new QSocketEvent();
                socketEvent.eventType = QSOCKET_EVENTTYPE.EVENT_RECEIVE;
                socketEvent.socket = so;
                socketEvent.sessionID = so.SessionID;
                socketEvent.errCode = SOCKET_ERRCODE.SUCCESS;
                //get msg type
                byte[] msgCmd = new byte[4];
                so.msgBuffer.FectchData(ref msgCmd);
                string msgCmdStr = Encoding.ASCII.GetString(msgCmd);
                int.TryParse(msgCmdStr, out socketEvent.msgType);
                //get body
                socketEvent.msgData = new byte[bodySize - 4];
                so.msgBuffer.FectchData(ref socketEvent.msgData);
                AddSocketEvent(socketEvent);
                return true;

            }
            catch(Exception ex)
            {
                so.ExceptionCode = SOCKET_ERRCODE.ERR_RECEIVE_DEALDATA_EXCEPTION;
                so.ExceptionMessage = ex.Message;
                //so.Close(SOCKET_ERRCODE.ERR_RECEIVE_DEALDATA_EXCEPTION);
            }

            return false;
        }


        public void AsynchSendCallback(System.IAsyncResult ar)
        {
            Connection so = (Connection)ar.AsyncState;
            Socket s = so.socket;

            try
            {                
                if (s == null || !s.Connected)
                {                  
                    return;
                }
                /*int send = */s.EndSend(ar);
            }
            catch (Exception ex)
			{
                so.ExceptionCode = SOCKET_ERRCODE.ERR_SEND_END_EXCEPTION;
                so.ExceptionMessage = ex.Message;
                so.Close(SOCKET_ERRCODE.ERR_SEND_END_EXCEPTION);
            }
        }


        public void BackToGameCenter(int sessionId)
        {
            if (mConnsTable.ContainsKey(sessionId))
            {
                CurrentConnection = mConnsTable[sessionId] as Connection;
            }
        }

        //进程循环检查函数
        public void Update()
        {
            bool run = true;
            while (run)
            {
				if( CurrentConnection != null )
				{
					if( CurrentConnection.mSendTime != 0 )
					{
						if( Time.realtimeSinceStartup >= CurrentConnection.mSendTime )
							CurrentConnection.mSendTime = 0;
					}
					else if( CurrentConnection.mSendList.Count > 0 )
					{
						int maxCount = Mathf.Min(15, CurrentConnection.mSendList.Count);
						for( int i = 0; i < maxCount; i++ )
							CurrentConnection.SendMessage(CurrentConnection.mSendList[i], CurrentConnection.mSendList[i].Length);
						CurrentConnection.mSendList.RemoveRange(0, maxCount);

						if( CurrentConnection.mSendList.Count > 0 )
							CurrentConnection.mSendTime = Time.realtimeSinceStartup + 1.0f;
					}
				}

                QSocketEvent socketEvent = null;
                if(mMessageQueue.Count > 0)
                {
                    socketEvent = (QSocketEvent)mMessageQueue.DelItem();
                }
                else
                {
                    run = false;
                }

                if (null != socketEvent)
                {
                    switch (socketEvent.eventType)
                    {
                        case QSOCKET_EVENTTYPE.EVENT_CONNECT:
                            this.OnConnect(socketEvent.socket, socketEvent.errCode);
                            break;
                        case QSOCKET_EVENTTYPE.EVENT_RECEIVE:
                            this.OnReceive(socketEvent.socket, socketEvent.msgType,socketEvent.msgData);
                            break;
                        case QSOCKET_EVENTTYPE.EVENT_CLOSE:
                            this.OnClose(socketEvent.socket, socketEvent.errCode);
                            break;
                        default:
                            SQDebug.Log("Invalidate message; message type:" +  socketEvent.eventType);
                            break;
                    }
                }

            }

        }
        
    }
}
